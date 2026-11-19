import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Ban, FileText, Pencil, Printer, ReceiptText, RotateCcw, Save } from 'lucide-react'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { printReturn } from '../lib/print'
import { Drawer, Field, Spinner } from '../components/ui'

/* المرتجعات: kind "reg" = مرتجع استمارة تسجيل (the whole form), "bill" = مرتجع فاتورة (full or partial per line). */
export type ReturnKind = 'reg' | 'bill'
export interface ReturnTarget { kind: ReturnKind; studentName: string; nationalId: string; source?: string; guid?: string }
interface Line { code: string; name: string; diplomGuid: string; diplomType: number; cost: number; tax: number; total: number; taxRate: number }
interface Book { guid: string; code: number; docName: string; branchName: string | null; chkVat: boolean }
interface Prep { source: { guid: string; code: string; date: string; studentName: string; subTotal: number; sellerGuid: string | null }; lines: Line[]; books: Book[]; sellers: { ManFullName: string; Guid: string }[] }
interface Details {
  head: { Guid: string; Code: string; Date: string; BookGuid: string; BookName: string; SellerGuid: string | null; SellerName: string | null; SourceGuid: string; SourceCode: string
    Total: number; Tax: number; SubTotal: number; Notes: string | null; IsUse: boolean; JournalCode: string | null }
  lines: Line[]; original: Line[]
}

const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const r2 = (n: number) => Math.round(n * 100) / 100
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const FORM: Record<ReturnKind, string> = { reg: 'reregister', bill: 'repay' }
const TITLE: Record<ReturnKind, string> = { reg: 'مرتجع استمارة تسجيل', bill: 'مرتجع فاتورة مبيعات' }

export function ReturnForm({ target, onClose }: { target: ReturnTarget; onClose: () => void }) {
  const { kind } = target
  const { can } = useAuth()
  const qc = useQueryClient()
  const [guid, setGuid] = useState(target.guid ?? null)
  const [editing, setEditing] = useState(!target.guid)
  const [book, setBook] = useState('')
  const [seller, setSeller] = useState('')
  const [date, setDate] = useState(today())
  const [notes, setNotes] = useState('')
  const [amounts, setAmounts] = useState<Record<string, string>>({})

  const details = useQuery({ queryKey: ['return', kind, guid], queryFn: () => api<Details>(`/returns/${kind}/${guid}`), enabled: !!guid, staleTime: 0 })
  const source = target.source ?? details.data?.head.SourceGuid
  // Books / sellers / original lines: for a new return from the source; for an edit, also from the source.
  const prep = useQuery({ queryKey: ['return-prep', kind, source, !!guid], queryFn: () => api<Prep>(`/returns/${kind}/prepare?source=${source}`), enabled: editing && !!source && !guid, staleTime: 0 })
  const editBooks = useQuery({ queryKey: ['return-books', kind, guid], enabled: editing && !!guid && !!source,
    queryFn: () => api<Prep>(`/returns/${kind}/prepare?source=${source}&edit=true`) })

  useEffect(() => {
    const p = prep.data
    if (!p) return
    if (p.books.length === 1) setBook(p.books[0].guid)
    if (p.source.sellerGuid && p.sellers.some(s => s.Guid === p.source.sellerGuid)) setSeller(p.source.sellerGuid)
    setAmounts(Object.fromEntries(p.lines.map(l => [l.diplomGuid, String(l.total)])))
  }, [prep.data])
  useEffect(() => {
    const d = details.data
    if (!d) return
    setBook(d.head.BookGuid); setSeller(d.head.SellerGuid ?? ''); setDate(d.head.Date.slice(0, 10)); setNotes(d.head.Notes ?? '')
    setAmounts(Object.fromEntries(d.lines.map(l => [l.diplomGuid, String(l.total)])))
  }, [details.data])

  const original = guid ? details.data?.original ?? [] : prep.data?.lines ?? []
  const books = guid ? editBooks.data?.books ?? (details.data ? [{ guid: details.data.head.BookGuid, code: 0, docName: details.data.head.BookName, branchName: null, chkVat: false }] : []) : prep.data?.books ?? []
  const sellers = guid ? editBooks.data?.sellers ?? [] : prep.data?.sellers ?? []
  const lines = useMemo(() => original.map(o => {
    const total = kind === 'reg' ? o.total : Math.min(o.total, Math.max(0, Number(amounts[o.diplomGuid] ?? 0) || 0))
    const cost = kind === 'reg' ? o.cost : o.taxRate ? r2(total / (1 + o.taxRate / 100)) : total
    return { ...o, total, cost, tax: kind === 'reg' ? o.tax : r2(total - cost) }
  }), [original, amounts, kind])
  const tot = { cost: r2(lines.reduce((s, x) => s + x.cost, 0)), tax: r2(lines.reduce((s, x) => s + x.tax, 0)), total: r2(lines.reduce((s, x) => s + x.total, 0)) }
  const h = details.data?.head

  const save = useMutation({
    mutationFn: (body: object) => guid
      ? api<{ message: string; guid: string }>(`/returns/${kind}/${guid}`, { method: 'PUT', json: body })
      : api<{ message: string; guid: string }>(`/returns/${kind}`, { method: 'POST', json: body }),
    onSuccess: async r => {
      qc.invalidateQueries({ queryKey: ['reception'] }); qc.invalidateQueries({ queryKey: ['return'] })
      setGuid(r.guid); setEditing(false)
      if (await alerts.confirm({ title: r.message, text: 'هل تريد طباعة المرتجع الآن؟', confirmText: 'طباعة', cancelText: 'لاحقاً', icon: 'success' })) printReturn(kind, r.guid)
    },
  })

  const submit = async (isUse = true) => {
    if (!book) return alerts.error('يرجى اختيار نوع المستند')
    if (kind === 'reg' && !seller) return alerts.error('يرجى اختيار مندوب البيع')
    if (!notes.trim()) return alerts.error(kind === 'reg' ? 'لا يمكن حفظ مرتجع التسجيل بدون ملاحظات' : 'لا يمكن حفظ مرتجع المبيعات بدون ملاحظات')
    if (tot.total <= 0) return alerts.error('يرجى تحديد قيمة المرتجع')
    let reason: string | null = null
    if (guid) { reason = await alerts.reason(isUse ? 'سبب تعديل المرتجع' : 'سبب إلغاء المرتجع'); if (!reason) return }
    else if (!(await alerts.confirm({ title: `تأكيد ${TITLE[kind]}`, text: `سيتم إرجاع ${money(tot.total)} ر.س وتسجيل القيد المحاسبي${kind === 'reg' ? '، وتصبح الاستمارة مرتجعة' : '، وتصبح الفاتورة مرتجعة'}.`, confirmText: 'تأكيد المرتجع', danger: true }))) return
    save.mutate({ sourceGuid: source, bookGuid: book, sellerGuid: seller || null, date, notes, isUse, reason, lines: lines.map(l => ({ diplomGuid: l.diplomGuid, total: l.total })) })
  }

  const loading = (!!guid && details.isPending) || (!guid && prep.isPending)
  const ro = !editing
  return (
    <Drawer size="lg" onClose={onClose} icon={<RotateCcw size={22} />}
      title={guid ? `${TITLE[kind]} رقم ${h?.Code ?? ''}` : TITLE[kind]}
      subtitle={<>{target.studentName} • <span dir="ltr">{target.nationalId}</span>
        {(h?.SourceCode || prep.data?.source.code) && <> • {kind === 'reg' ? 'الاستمارة' : 'الفاتورة'} #{h?.SourceCode ?? prep.data?.source.code}</>}
        {h?.JournalCode && <> • قيد رقم {h.JournalCode}</>}</>}
      footer={<>
        {editing && <button className="btn danger" disabled={save.isPending || loading} onClick={() => submit(true)}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ المرتجع</>}</button>}
        {!editing && guid && h?.IsUse && can(FORM[kind], 'edit') && <button className="btn secondary" onClick={() => setEditing(true)}><Pencil size={16} /> تعديل</button>}
        {!editing && guid && <button className="btn secondary" onClick={() => printReturn(kind, guid)}><Printer size={16} /> طباعة</button>}
        {editing && guid && can(FORM[kind], 'edit') && <button className="btn danger ghost" disabled={save.isPending} onClick={() => submit(false)}><Ban size={16} /> إلغاء المرتجع</button>}
        <button className="btn ghost" onClick={onClose}>إغلاق</button>
        {h && !h.IsUse && <span className="hint"><span className="badge off">ملغى</span></span>}
      </>}>
      {loading ? <div className="center-fill" style={{ minHeight: 280 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 18 }}>
          {editing && books.length === 0 && (
            <div className="alert error" style={{ alignItems: 'center' }}>
              <span style={{ flex: 1 }}>لا يتوفر لك دفتر "{kind === 'reg' ? 'مرتجع تسجيل' : 'مرتجع مبيعات'}" في فرع المستند. يمكن تعريفه من شاشة "إعدادات التسجيل" وإضافتك ضمن مستخدميه.</span>
              {can('regsetting', 'add') && <a className="btn sm" href="/s/reg-settings">فتح إعدادات التسجيل</a>}
            </div>
          )}
          <Field label="نوع المستند (الدفتر)" required>
            <div className="pick-grid">
              {books.map(b => (
                <button key={b.guid} type="button" disabled={ro} className={`pick-card ${book === b.guid ? 'on' : ''}`} onClick={() => setBook(b.guid)}>
                  <b>{b.docName}</b><small>{b.branchName ?? ''}{b.chkVat ? ' • شامل الضريبة' : ''}</small>
                </button>
              ))}
            </div>
          </Field>
          <div className="grid cols-2">
            {kind === 'reg' && (
              <Field label="مندوب البيع" required>
                <select className="input" value={seller} disabled={ro} onChange={e => setSeller(e.target.value)}>
                  <option value="">— اختر —</option>
                  {sellers.map(s => <option key={s.Guid} value={s.Guid}>{s.ManFullName}</option>)}
                  {ro && h?.SellerName && !sellers.length && <option value={seller}>{h.SellerName}</option>}
                </select>
              </Field>
            )}
            <Field label="التاريخ" required><input className="input" type="date" value={date} disabled={ro} onChange={e => setDate(e.target.value)} /></Field>
          </div>

          <div>
            <div className="lines-head"><h4>{kind === 'reg' ? 'بنود الاستمارة (تُرتجع بالكامل)' : 'بنود الفاتورة'}</h4>
              {kind === 'bill' && editing && <small className="muted">عدّل القيمة لعمل مرتجع جزئي، أو اجعلها صفراً لاستبعاد البند</small>}</div>
            <div className="card" style={{ overflow: 'auto' }}>
              <table className="dt ledger">
                <thead><tr><th>البند</th><th className="n">قيمة البند</th><th className="n">قبل الضريبة</th><th className="n">الضريبة</th><th className="n">المرتجع</th></tr></thead>
                <tbody>
                  {lines.map(l => {
                    const o = original.find(x => x.diplomGuid === l.diplomGuid)!
                    return (
                      <tr key={l.diplomGuid}>
                        <td><b>{l.name}</b><div className="muted small">كود {l.code}{l.taxRate ? ` • ضريبة ${l.taxRate}%` : ''}</div></td>
                        <td className="n">{money(o.total)}</td>
                        <td className="n">{money(l.cost)}</td>
                        <td className="n">{money(l.tax)}</td>
                        <td className="n">{kind === 'bill' && editing
                          ? <input className="input price" dir="ltr" inputMode="decimal" style={{ width: 120 }} value={amounts[l.diplomGuid] ?? ''}
                              onChange={e => { const v = e.target.value.replace(/[^\d.]/g, ''); setAmounts(a => ({ ...a, [l.diplomGuid]: Number(v) > o.total ? String(o.total) : v })) }} />
                          : <b>{money(l.total)}</b>}</td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          </div>

          <Field label="ملاحظات / سبب المرتجع" required><textarea className="input" maxLength={500} value={notes} disabled={ro} onChange={e => setNotes(e.target.value)} /></Field>
          <div className="totals">
            <div><small>قبل الضريبة</small><b>{money(tot.cost)}</b></div>
            <div><small>الضريبة</small><b>{money(tot.tax)}</b></div>
            <div className="grand"><small>إجمالي المرتجع</small><b>{money(tot.total)}</b></div>
          </div>
        </div>
      )}
    </Drawer>
  )
}

/* ---------------- Step 1: choose the document to return ---------------- */
interface Cand { guid: string; code: string; date: string; total: number; title: string | null; returned: boolean }
export interface ReturnStart { account: string; studentName: string; nationalId: string }

/** مرتجع: pick a registration form or an invoice (cards), then the return form opens for it. */
export function ReturnWizard({ start, onClose }: { start: ReturnStart; onClose: () => void }) {
  const [picked, setPicked] = useState<{ kind: ReturnKind; source: string } | null>(null)
  if (picked) return <ReturnForm key={picked.source} target={{ ...picked, studentName: start.studentName, nationalId: start.nationalId }} onClose={onClose} />
  return <ReturnPicker start={start} onPick={setPicked} onClose={onClose} />
}

function ReturnPicker({ start, onPick, onClose }: { start: ReturnStart; onPick: (p: { kind: ReturnKind; source: string }) => void; onClose: () => void }) {
  const { can } = useAuth()
  const canReg = can('reregister', 'add'), canBill = can('repay', 'add')
  const q = useQuery({ queryKey: ['return-candidates', start.account], queryFn: () => api<{ regs: Cand[]; bills: Cand[] }>(`/returns/candidates/${start.account}`), staleTime: 0 })
  const day = (v: string) => new Date(v).toLocaleDateString('ar-EG', { year: 'numeric', month: 'short', day: 'numeric' })
  const section = (kind: ReturnKind, title: string, hint: string, rows: Cand[], allowed: boolean, Icon: typeof FileText) => (
    <section className="rp-section">
      <div className="rp-head"><span className="ic"><Icon size={18} /></span><div><b>{title}</b><small>{hint}</small></div></div>
      {!allowed ? <div className="alert warn">لا تملك صلاحية {title}.</div>
        : rows.length === 0 ? <div className="empty"><b>لا توجد مستندات</b></div> : (
          <div className="rp-grid">
            {rows.map(r => (
              <button key={r.guid} type="button" className={`rp-card ${r.returned ? 'done' : ''}`} disabled={r.returned} onClick={() => onPick({ kind, source: r.guid })}>
                <div className="rp-top"><b>{kind === 'reg' ? 'استمارة' : 'فاتورة'} #{r.code}</b>{r.returned ? <span className="badge off">تم إرجاعها</span> : <span className="badge ok">متاحة</span>}</div>
                {r.title && <span className="rp-title">{kind === 'bill' ? `الرقم الضريبي ${r.title}` : r.title}</span>}
                <div className="rp-foot"><small>{day(r.date)}</small><b className="num-strong">{money(r.total)} <em>ر.س</em></b></div>
              </button>
            ))}
          </div>
        )}
    </section>
  )
  return (
    <Drawer size="lg" onClose={onClose} icon={<RotateCcw size={22} />} title="عمل مرتجع" subtitle={<>{start.studentName} • <span dir="ltr">{start.nationalId}</span> • اختر المستند المراد إرجاعه</>}
      footer={<button className="btn ghost" onClick={onClose}>إغلاق</button>}>
      {q.isPending ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 22 }}>
          {section('reg', 'مرتجع استمارة تسجيل', 'تُرتجع الاستمارة بالكامل ويُحرَّر مقعد الطالب', q.data?.regs ?? [], canReg, FileText)}
          {section('bill', 'مرتجع فاتورة مبيعات', 'يمكن إرجاع الفاتورة كاملة أو جزء من بنودها', q.data?.bills ?? [], canBill, ReceiptText)}
        </div>
      )}
    </Drawer>
  )
}
