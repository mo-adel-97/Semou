import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Ban, Check, FileSpreadsheet, Pencil, Printer, Save, Search } from 'lucide-react'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { printFeeForm } from '../lib/print'
import { Drawer, Field, Spinner } from '../components/ui'

/* استمارة الرسوم (RegFessForm): exam / certificate / other fees charged to the student's account. */
interface Book { guid: string; code: number; docName: string; branchName: string | null; chkVat: boolean }
interface Item { code: string; name: string; diplomGuid: string; cost: number; tax: number; total: number }
interface Prep { book: Book; items: Item[]; sellers: { ManFullName: string; Guid: string }[] }
interface Details {
  head: { Guid: string; Code: string; Date: string; BookGuid: string; BookName: string; BookCode: number; ChkVat: boolean; SellerGuid: string; SellerName: string | null
    StudentName: string; NationalId: string; Total: number; Tax: number; SubTotal: number; Notes: string | null; IsUse: boolean; BranchName: string | null; JournalCode: string | null }
  lines: Item[]
}
export interface FeeTarget { studentGuid: string; studentName: string; nationalId: string; guid: string | null }

const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const r2 = (n: number) => Math.round(n * 100) / 100
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)

export function FeeForm({ target, onClose }: { target: FeeTarget; onClose: () => void }) {
  const { can, me } = useAuth()
  const qc = useQueryClient()
  const [guid, setGuid] = useState(target.guid)
  const [editing, setEditing] = useState(!target.guid)
  const [book, setBook] = useState('')
  const [seller, setSeller] = useState('')
  const [date, setDate] = useState(today())
  const [notes, setNotes] = useState('')
  const [picked, setPicked] = useState<string[]>([])
  const [q, setQ] = useState('')

  const details = useQuery({ queryKey: ['fee-form', guid], queryFn: () => api<Details>(`/fee-forms/${guid}`), enabled: !!guid, staleTime: 0 })
  const books = useQuery({ queryKey: ['fee-books'], queryFn: () => api<Book[]>('/fee-forms/books'), enabled: editing, staleTime: 5 * 60_000 })
  const prep = useQuery({ queryKey: ['fee-prep', book], queryFn: () => api<Prep>(`/fee-forms/prepare?book=${book}`), enabled: editing && !!book, staleTime: 60_000 })

  // Fill the form from a saved document.
  useEffect(() => {
    const h = details.data?.head
    if (!h) return
    setBook(h.BookGuid); setSeller(h.SellerGuid); setDate(h.Date.slice(0, 10)); setNotes(h.Notes ?? ''); setPicked(details.data!.lines.map(l => l.diplomGuid))
  }, [details.data])
  // New form: one book → pick it; default seller = the logged-in user when allowed.
  useEffect(() => { if (!guid && books.data?.length === 1) setBook(books.data[0].guid) }, [books.data, guid])
  useEffect(() => {
    const s = prep.data?.sellers
    if (!s || seller) return
    setSeller(s.find(x => x.Guid === me?.sellerGuid)?.Guid ?? (s.length === 1 ? s[0].Guid : ''))
  }, [prep.data, seller, me?.sellerGuid])

  const items = editing ? prep.data?.items ?? [] : details.data?.lines ?? []
  const sel = useMemo(() => items.filter(i => picked.includes(i.diplomGuid)), [items, picked])
  const tot = { cost: r2(sel.reduce((s, x) => s + x.cost, 0)), tax: r2(sel.reduce((s, x) => s + x.tax, 0)), total: r2(sel.reduce((s, x) => s + x.total, 0)) }
  const h = details.data?.head

  const save = useMutation({
    mutationFn: (body: object) => guid
      ? api<{ message: string; guid: string; code: string }>(`/fee-forms/${guid}`, { method: 'PUT', json: body })
      : api<{ message: string; guid: string; code: string }>('/fee-forms', { method: 'POST', json: body }),
    onSuccess: async r => {
      qc.invalidateQueries({ queryKey: ['reception'] }); qc.invalidateQueries({ queryKey: ['fee-form'] })
      setGuid(r.guid); setEditing(false)
      if (await alerts.confirm({ title: r.message, text: 'هل تريد طباعة الاستمارة الآن؟', confirmText: 'طباعة', cancelText: 'لاحقاً', icon: 'success' })) printFeeForm(r.guid)
    },
  })

  const submit = async (isUse = true) => {
    if (!book) return alerts.error('يرجى اختيار نوع المستند')
    if (!seller) return alerts.error('يرجى اختيار مندوب البيع')
    if (!sel.length || tot.total === 0) return alerts.error('لا يمكن حفظ الاستمارة بدون اختيار الرسوم')
    let reason: string | null = null
    if (guid) { reason = await alerts.reason(isUse ? 'سبب تعديل استمارة الرسوم' : 'سبب إلغاء استمارة الرسوم'); if (!reason) return }
    save.mutate({ studentGuid: target.studentGuid, bookGuid: book, sellerGuid: seller, date, notes, lines: picked.map(d => ({ diplomGuid: d })), isUse, reason })
  }

  const toggle = (d: string) => editing && setPicked(p => p.includes(d) ? p.filter(x => x !== d) : [...p, d])
  const shown = items.filter(i => !q || i.name.includes(q))
  const loading = (!!guid && details.isPending) || (editing && books.isPending)

  return (
    <Drawer size="lg" onClose={onClose} icon={<FileSpreadsheet size={22} />}
      title={guid ? `استمارة رسوم رقم ${h?.Code ?? ''}` : 'استمارة رسوم جديدة'}
      subtitle={<>{target.studentName} • <span dir="ltr">{target.nationalId}</span>{h?.JournalCode && <> • قيد رقم {h.JournalCode}</>}</>}
      footer={<>
        {editing && <button className="btn" disabled={save.isPending || loading} onClick={() => submit(true)}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>}</button>}
        {!editing && guid && h?.IsUse && can('regfess', 'edit') && <button className="btn secondary" onClick={() => setEditing(true)}><Pencil size={16} /> تعديل</button>}
        {!editing && guid && <button className="btn secondary" onClick={() => printFeeForm(guid)}><Printer size={16} /> طباعة</button>}
        {editing && guid && can('regfess', 'edit') && <button className="btn danger ghost" disabled={save.isPending} onClick={() => submit(false)}><Ban size={16} /> إلغاء الاستمارة</button>}
        <button className="btn ghost" onClick={onClose}>إغلاق</button>
        {h && !h.IsUse && <span className="hint"><span className="badge off">ملغاة</span></span>}
      </>}>
      {loading ? <div className="center-fill" style={{ minHeight: 280 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 18 }}>
          {editing ? (
            <Field label="نوع المستند (دفتر الرسوم)" required>
              {books.data?.length === 0 ? <div className="alert error">لا يوجد دفتر رسوم مسموح لك، يرجى مراجعة مدير النظام</div> : (
                <div className="pick-grid">
                  {books.data?.map(b => (
                    <button key={b.guid} type="button" className={`pick-card ${book === b.guid ? 'on' : ''}`} onClick={() => { if (b.guid !== book) { setBook(b.guid); setPicked([]); setSeller('') } }}>
                      <b>{b.docName}</b><small>{b.branchName} • {b.chkVat ? 'شامل ضريبة القيمة المضافة' : 'بدون ضريبة'}</small>
                    </button>
                  ))}
                </div>
              )}
            </Field>
          ) : h && (
            <div className="fee-info">
              <span><small>الدفتر</small><b>{h.BookName}</b></span>
              <span><small>التاريخ</small><b>{new Date(h.Date).toLocaleDateString('ar-EG')}</b></span>
              <span><small>مندوب البيع</small><b>{h.SellerName ?? '—'}</b></span>
              <span><small>الفرع</small><b>{h.BranchName ?? '—'}</b></span>
            </div>
          )}

          {editing && book && (
            <div className="grid cols-2">
              <Field label="مندوب البيع" required>
                <select className="input" value={seller} onChange={e => setSeller(e.target.value)} disabled={prep.isPending}>
                  <option value="">— اختر —</option>
                  {prep.data?.sellers.map(s => <option key={s.Guid} value={s.Guid}>{s.ManFullName}</option>)}
                </select>
              </Field>
              <Field label="التاريخ" required><input className="input" type="date" value={date} onChange={e => setDate(e.target.value)} /></Field>
            </div>
          )}

          {(book || !editing) && (
            <div>
              <div className="lines-head"><h4>الرسوم {editing && <small className="muted" style={{ fontWeight: 500 }}>اختر رسماً أو أكثر</small>}</h4>
                {editing && items.length > 6 && <div className="input-wrap" style={{ marginInlineStart: 'auto', width: 240 }}><Search size={15} /><input className="input" placeholder="بحث..." value={q} onChange={e => setQ(e.target.value)} /></div>}</div>
              {editing && prep.isPending ? <div className="sk" style={{ height: 120, borderRadius: 14 }} /> : (
                <div className="item-list">
                  {shown.length === 0 && <div className="empty"><b>لا توجد رسوم في قائمة أسعار هذا الدفتر</b></div>}
                  {(editing ? shown : sel).map(r => {
                    const on = picked.includes(r.diplomGuid)
                    return (
                      <div key={r.diplomGuid} className={`item ${on ? 'on' : ''}`} onClick={() => toggle(r.diplomGuid)} style={editing ? undefined : { cursor: 'default' }}>
                        <span className="cbx-lg">{on && <Check size={15} />}</span>
                        <div className="nm"><b>{r.name}</b><small>كود {r.code} • قبل الضريبة {money(r.cost)} • ضريبة {money(r.tax)}</small></div>
                        <b className="num-strong price-tag">{money(r.total)}</b>
                      </div>
                    )
                  })}
                </div>
              )}
            </div>
          )}

          {(editing ? book : true) && (
            <>
              <Field label="ملاحظات"><textarea className="input" maxLength={500} value={notes} disabled={!editing} onChange={e => setNotes(e.target.value)} /></Field>
              <div className="totals">
                <div><small>الإجمالي قبل الضريبة</small><b>{money(tot.cost)}</b></div>
                <div><small>الضريبة</small><b>{money(tot.tax)}</b></div>
                <div className="grand"><small>إجمالي الاستمارة</small><b>{money(tot.total)}</b></div>
              </div>
              <div className="alert info">تُسجِّل الاستمارة الرسوم مستحقةً على حساب الطالب، ويتم السداد لاحقاً من "سداد رسوم".</div>
            </>
          )}
        </div>
      )}
    </Drawer>
  )
}
