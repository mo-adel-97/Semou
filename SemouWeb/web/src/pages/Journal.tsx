import { useEffect, useMemo, useRef, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { BookOpen, BookOpenCheck, CalendarRange, Link2, Plus, Printer, Save, Scale, Search, Sigma, Trash2, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { PRINT_BASE_CSS, sealHtml } from '../lib/print'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { AccountPicker, type AccountLookup } from '../components/AccountPicker'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/* قيد يومية (EncodingDaily) / قيد افتتاحي (EncodingOpenDaily). */
export type JournalKind = 'daily' | 'opening'
const CFG = {
  daily: { title: 'قيود اليومية', singular: 'قيد يومية', form: 'daily', icon: BookOpen },
  opening: { title: 'القيود الافتتاحية', singular: 'قيد افتتاحي', form: 'opendaily', icon: BookOpenCheck },
} as const

interface Row { Code: string; Guid: string; Date: string; Notes: string; IsUse: boolean; Linked: boolean; SourceName: string | null; SourceCode: string | null; Total: number }
interface Line { key: number; accountGuid: string; accountCode: string; accountName: string; accountType: number | null; maden: string; daen: string; costGuid: string; costName: string; notes: string }
interface Details {
  head: { Code: string; Guid: string; Date: string; Notes: string; IsUse: boolean; Linked: boolean; SourceName: string | null; SourceCode: string | null }
  lines: { accountCode: string; accountName: string; notes: string | null; maden: number; daen: number; costName: string | null; accountGuid: string; costGuid: string | null; accountType: number | null }[]
}

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const monthStart = () => today().slice(0, 8) + '01'
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
const NO_ROWS: Row[] = []
let lineKey = 1
const newLine = (notes = ''): Line => ({ key: lineKey++, accountGuid: '', accountCode: '', accountName: '', accountType: null, maden: '', daen: '', costGuid: '', costName: '', notes })
const num = (s: string) => Number(s) || 0

export function Journal({ kind, group }: { kind: JournalKind; group: string }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const [range, setRange] = useState<'month' | 'all' | 'custom'>(kind === 'opening' ? 'all' : 'month')
  const [source, setSource] = useState<'all' | 'manual' | 'linked'>('all')
  const [from, setFrom] = useState(monthStart())
  const [to, setTo] = useState(today())
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const canFind = can(c.form, 'find'), canAdd = can(c.form, 'add')

  const params = range === 'all' ? {} : range === 'month' ? { from: monthStart(), to: today() } : { from, to }
  const list = useQuery({ queryKey: ['journal', kind, 'list', params], queryFn: () => api<Row[]>(`/journal/${kind}${qs(params)}`), enabled: canFind, placeholderData: keepPreviousData })
  const all = list.data ?? NO_ROWS
  const rows = useMemo(() => (source === 'all' ? all : all.filter(r => (source === 'linked' ? r.Linked : !r.Linked))), [all, source])

  const columns = useMemo<GridColumn<Row>[]>(() => [
    { accessorKey: 'Code', header: 'رقم القيد', sortFn: 'alphanumeric', meta: { width: 100 }, cell: x => <span className="code-chip">{x.getValue() as string}</span> },
    { accessorKey: 'Date', header: 'التاريخ', cell: x => <span dir="ltr">{String(x.getValue()).slice(0, 10)}</span> },
    { accessorKey: 'Notes', header: 'البيان', cell: x => <span className="clip" title={String(x.getValue() ?? '')}>{String(x.getValue() ?? '')}</span> },
    { accessorKey: 'Total', header: 'الإجمالي', cell: x => <span className="num-strong">{money(x.getValue() as number)}</span> },
    { accessorKey: 'SourceName', header: 'المصدر', cell: x => { const r = x.row.original; return r.Linked ? <span className="badge warn"><Link2 size={11} /> {r.SourceName} {r.SourceCode}</span> : <span className="badge muted">يدوي</span> } },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} offText="ملغي" /> },
  ], [])

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  const Icon = c.icon
  return (
    <>
      <PageHead title={c.title} subtitle={kind === 'daily' ? 'القيود اليدوية والقيود المتولدة من السندات والفواتير' : 'أرصدة أول المدة'} group={group}
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> {c.singular} جديد <kbd>F1</kbd></button>} />
      {!canFind ? <div className="card soon"><div className="ic"><Search size={36} /></div><h2>لا تملك صلاحية استعراض البيانات</h2></div> : (
        <>
          <div className="stats">
            <Stat icon={<Icon size={22} />} value={list.isPending ? '…' : rows.length.toLocaleString('ar-EG')} label="عدد القيود" />
            <Stat icon={<Sigma size={22} />} value={list.isPending ? '…' : money(rows.filter(r => r.IsUse).reduce((s, r) => s + r.Total, 0))} label="إجمالي القيود الفعالة (ر.س)" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : rows.filter(r => !r.IsUse).length} label="قيود ملغاة" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} searchPlaceholder="ابحث برقم القيد أو البيان..."
            onRowClick={r => setEditing({ guid: r.Guid })}
            toolbar={<>
              <Segmented value={range} onChange={setRange} options={[{ value: 'month', label: 'هذا الشهر' }, { value: 'custom', label: 'فترة' }, { value: 'all', label: 'الكل' }]} />
              {range === 'custom' && <div className="row" style={{ gap: 6 }}>
                <CalendarRange size={16} className="muted" />
                <input type="date" className="input" style={{ width: 150 }} value={from} onChange={e => setFrom(e.target.value)} />
                <span className="muted">إلى</span>
                <input type="date" className="input" style={{ width: 150 }} value={to} onChange={e => setTo(e.target.value)} />
              </div>}
              {kind === 'daily' && <Segmented value={source} onChange={setSource} options={[{ value: 'all', label: 'كل المصادر' }, { value: 'manual', label: 'يدوي' }, { value: 'linked', label: 'من المستندات' }]} />}
              {list.isFetching && !list.isPending && <Spinner size={16} />}
            </>} />
        </>
      )}
      <AnimatePresence>{editing && <JournalCard key={editing.guid ?? 'new'} kind={kind} guid={editing.guid} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

/* ============================== Card ============================== */
function JournalCard({ kind, guid: initialGuid, onClose }: { kind: JournalKind; guid: string | null; onClose: () => void }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const qc = useQueryClient()
  const [created, setCreated] = useState<{ code: string; guid: string } | null>(null)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid
  const details = useQuery({ queryKey: ['journal', kind, 'one', initialGuid], queryFn: () => api<Details>(`/journal/${kind}/${initialGuid}`), enabled: !!initialGuid, staleTime: 0 })
  const branchCosts = useQuery({ queryKey: ['vouchers', 'branch-costs'], queryFn: () => api<{ Code: string; CenterName: string; Guid: string }[]>('/vouchers/lookups/branch-cost-centers'), staleTime: 60_000 })
  const defaults = useQuery({ queryKey: ['vouchers', 'defaults'], queryFn: () => api<{ ZeroAmount: boolean }>('/vouchers/lookups/defaults'), staleTime: 60_000 })

  const linked = !!details.data?.head.Linked
  const canSave = !linked && (isNew ? can(c.form, 'add') : can(c.form, 'edit'))
  const ro = !canSave

  const [date, setDate] = useState(today())
  const [notes, setNotes] = useState('')
  const [isUse, setIsUse] = useState(true)
  const [lines, setLines] = useState<Line[]>(() => [newLine(), newLine()])
  const [picker, setPicker] = useState<number | null>(null)
  const baseline = useRef('')
  const snapshot = JSON.stringify([date, notes, isUse, lines.map(({ key: _k, ...l }) => l)])
  const dirty = baseline.current !== '' && snapshot !== baseline.current
  useEffect(() => { if (isNew && baseline.current === '') baseline.current = snapshot }) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    const d = details.data
    if (!d) return
    const ls = d.lines.map(l => ({ key: lineKey++, accountGuid: l.accountGuid, accountCode: l.accountCode, accountName: l.accountName, accountType: l.accountType,
      maden: l.maden ? String(l.maden) : '', daen: l.daen ? String(l.daen) : '', costGuid: l.costGuid && l.costGuid !== EMPTY_GUID ? l.costGuid : '', costName: l.costName ?? '', notes: l.notes ?? '' }))
    setDate(d.head.Date.slice(0, 10)); setNotes(d.head.Notes ?? ''); setIsUse(!!d.head.IsUse); setLines(ls.length ? ls : [newLine()])
    baseline.current = JSON.stringify([d.head.Date.slice(0, 10), d.head.Notes ?? '', !!d.head.IsUse, ls.map(({ key: _k, ...l }) => l)])
  }, [details.data])

  const tMaden = Math.round(lines.reduce((s, l) => s + num(l.maden), 0) * 100) / 100
  const tDaen = Math.round(lines.reduce((s, l) => s + num(l.daen), 0) * 100) / 100
  const diff = Math.round((tMaden - tDaen) * 100) / 100
  const balanced = diff === 0 && (tMaden > 0 || !!defaults.data?.ZeroAmount)
  const needsCost = (l: Line) => l.accountType === 0 || l.accountType === 1
  const setLine = (key: number, patch: Partial<Line>) => setLines(ls => ls.map(l => (l.key === key ? { ...l, ...patch } : l)))
  const balanceLine = (l: Line) => { // put the remaining difference on this line
    const others = lines.filter(x => x.key !== l.key)
    const d = Math.round((others.reduce((s, x) => s + num(x.maden), 0) - others.reduce((s, x) => s + num(x.daen), 0)) * 100) / 100
    setLine(l.key, d >= 0 ? { daen: d ? String(d) : '', maden: '' } : { maden: String(-d), daen: '' })
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = { date, notes, isUse, lines: lines.filter(l => l.accountGuid).map(l => ({ accountGuid: l.accountGuid, maden: num(l.maden), daen: num(l.daen), costGuid: l.costGuid || null, notes: l.notes })) }
      if (isNew) return api<{ message: string; code: string; guid: string }>(`/journal/${kind}`, { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/journal/${kind}/${guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async r => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid })
      baseline.current = snapshot
      qc.invalidateQueries({ queryKey: ['journal', kind] })
      await alerts.success(r.message, `رقم القيد: ${r.code}`)
    },
  })

  const validate = (): string | null => {
    if (!notes.trim()) return 'يرجى إدخال ملاحظات القيد لتسهيل عملية مراجعة الحسابات'
    const used = lines.filter(l => l.accountGuid)
    if (!used.length || (tMaden === 0 && !defaults.data?.ZeroAmount)) return 'يرجى التأكد من المبالغ المدرجة في الجانب المدين'
    if (tDaen === 0 && !defaults.data?.ZeroAmount) return 'يرجى التأكد من المبالغ المدرجة في الجانب الدائن'
    if (diff !== 0) return 'القيد غير متزن، يرجى مراجعة تفاصيل القيد'
    if (used.some(l => num(l.maden) > 0 && num(l.daen) > 0)) return 'السطر الواحد يكون مدين أو دائن فقط'
    if (used.some(l => needsCost(l) && !l.costGuid)) return 'لا يمكن الحفظ بدون اختيار مركز التكلفة'
    if (used.some(l => !l.notes.trim())) return 'لا يمكن الحفظ بدون إدخال البيان لتسهيل عملية المراجعة'
    return null
  }
  const submit = () => { const e = validate(); if (e) { alerts.error(e); return } save.mutate() }
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && picker === null && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const print = () => {
    if (dirty) { notify.info('يرجى حفظ التعديلات أولاً قبل الطباعة'); return }
    if (guid) printJournal(guid, c.singular, created?.code ?? details.data?.head.Code ?? '', date, notes, lines.filter(l => l.accountGuid), tMaden, tDaen)
  }

  const loading = !!initialGuid && details.isPending
  const code = created?.code ?? details.data?.head.Code
  const Icon = c.icon
  return (
    <Drawer size="lg" onClose={requestClose} icon={<Icon size={22} />} title={isNew ? `${c.singular} جديد` : `${c.singular} رقم ${code}`}
      subtitle={linked ? `قيد متولد من ${details.data?.head.SourceName} رقم ${details.data?.head.SourceCode}` : 'قيد يدوي'}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        {!isNew && <button className="btn secondary" onClick={print}><Printer size={16} /> طباعة</button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{linked ? <><Link2 size={14} /> لا يمكن التعديل على قيد يومية مرتبط بمستند آخر</> : ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <div className="grid" style={{ gap: 20 }}>
          <div className="grid cols-3">
            <Field label="التاريخ" required><input type="date" className="input" readOnly={ro} value={date} onChange={e => setDate(e.target.value)} /></Field>
            <Field label="البيان / الملاحظات" required className="span-2"><input className="input" readOnly={ro} maxLength={200} value={notes} onChange={e => setNotes(e.target.value)} placeholder="وصف القيد" /></Field>
          </div>
          <div>
            <div className="lines-head">
              <h4>تفاصيل القيد</h4>
              <div className={`balance ${balanced ? 'ok' : 'bad'}`}><Scale size={16} />{balanced ? 'القيد متزن' : <>الفرق: <b dir="ltr">{money(diff)}</b></>}</div>
            </div>
            <div className="lines jl">
              <div className="ln head"><span>#</span><span>الحساب</span><span>البيان</span><span>مدين</span><span>دائن</span><span>مركز التكلفة</span><span /></div>
              <AnimatePresence initial={false}>
                {lines.map((l, i) => (
                  <motion.div key={l.key} className="ln" initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }}>
                    <span className="muted">{i + 1}</span>
                    <button type="button" className={`pick ${l.accountGuid ? '' : 'empty'}`} disabled={ro} onClick={() => setPicker(l.key)}>
                      {l.accountGuid ? <><span className="code-chip">{l.accountCode}</span> {l.accountName}</> : 'اختر الحساب...'}
                    </button>
                    <input className="input" readOnly={ro} maxLength={200} value={l.notes} placeholder="البيان" onChange={e => setLine(l.key, { notes: e.target.value })} />
                    <input className="input" dir="ltr" inputMode="decimal" readOnly={ro} value={l.maden} placeholder="0.00" title="انقر نقراً مزدوجاً لموازنة القيد"
                      onDoubleClick={() => !ro && balanceLine(l)} onChange={e => setLine(l.key, { maden: e.target.value.replace(/[^\d.]/g, ''), ...(e.target.value ? { daen: '' } : {}) })} />
                    <input className="input" dir="ltr" inputMode="decimal" readOnly={ro} value={l.daen} placeholder="0.00" title="انقر نقراً مزدوجاً لموازنة القيد"
                      onDoubleClick={() => !ro && balanceLine(l)} onChange={e => setLine(l.key, { daen: e.target.value.replace(/[^\d.]/g, ''), ...(e.target.value ? { maden: '' } : {}) })} />
                    {needsCost(l) ? (
                      <select className="input" disabled={ro} value={l.costGuid} onChange={e => setLine(l.key, { costGuid: e.target.value, costName: e.target.selectedOptions[0]?.text ?? '' })}>
                        <option value="">اختر...</option>
                        {l.costGuid && !branchCosts.data?.some(x => x.Guid === l.costGuid) && <option value={l.costGuid}>{l.costName}</option>}
                        {branchCosts.data?.map(x => <option key={x.Guid} value={x.Guid}>{x.CenterName}</option>)}
                      </select>
                    ) : <span className="muted" style={{ fontSize: 12 }}>{l.accountGuid ? 'لا يحتاج' : '—'}</span>}
                    <button type="button" className="btn ghost sm icon" disabled={ro || lines.length <= 1} onClick={() => setLines(ls => ls.filter(x => x.key !== l.key))} aria-label="حذف"><Trash2 size={15} /></button>
                  </motion.div>
                ))}
              </AnimatePresence>
              <div className="ln foot">
                <span />
                {!ro ? <button type="button" className="btn ghost sm" onClick={() => setLines(ls => [...ls, newLine(notes)])}><Plus size={15} /> إضافة سطر</button> : <span />}
                <span className="muted" style={{ textAlign: 'end' }}>الإجمالي</span>
                <b dir="ltr" className="num-strong">{money(tMaden)}</b>
                <b dir="ltr" className="num-strong">{money(tDaen)}</b>
                <span /><span />
              </div>
            </div>
          </div>
          {!isNew && <Switch label="القيد فعال" checked={isUse} disabled={ro} onChange={setIsUse} />}
        </div>
      )}
      <AnimatePresence>
        {picker !== null && <AccountPicker onClose={() => setPicker(null)} onPick={(a: AccountLookup) => {
          const keep = a.AccountType === 0 || a.AccountType === 1
          setLine(picker, { accountGuid: a.Guid, accountCode: a.Code, accountName: a.Name, accountType: a.AccountType, ...(keep ? {} : { costGuid: '', costName: '' }) })
          setPicker(null)
        }} />}
      </AnimatePresence>
    </Drawer>
  )
}

async function printJournal(guid: string, title: string, code: string, date: string, notes: string, lines: Line[], tm: number, td: number) {
  const esc = (s: string) => s.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))
  const brand = getComputedStyle(document.documentElement).getPropertyValue('--brand').trim() || '#4a7f5f'
  const rows = lines.map((l, i) => `<tr><td>${i + 1}</td><td>${esc(l.accountCode)} - ${esc(l.accountName)}</td><td>${esc(l.notes)}</td><td>${esc(l.costName || '—')}</td><td class="n">${num(l.maden) ? money(num(l.maden)) : ''}</td><td class="n">${num(l.daen) ? money(num(l.daen)) : ''}</td></tr>`).join('')
  let seal = ''
  try { seal = await sealHtml('jv', guid) } catch (e) { alerts.error((e as Error).message); return }
  const w = window.open('', '_blank', 'width=900,height=700')
  if (!w) { alerts.error('المتصفح منع فتح نافذة الطباعة، اسمح بالنوافذ المنبثقة'); return }
  w.document.write(`<!doctype html><html dir="rtl" lang="ar"><head><meta charset="utf-8"><title>${title} ${esc(code)}</title>
  <link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;800&display=swap" rel="stylesheet">
  <style>${PRINT_BASE_CSS}*{box-sizing:border-box}body{font-family:Cairo,sans-serif;padding:28px;color:#111}.top{display:flex;align-items:center;gap:16px;border-bottom:3px solid ${brand};padding-bottom:14px}
  .top img{height:66px}.top h1{margin:0;font-size:19px}.t{margin-inline-start:auto;border:2px solid ${brand};border-radius:12px;padding:6px 18px;text-align:center}.t b{display:block;font-size:20px;color:${brand}}
  p{margin:16px 0}table{width:100%;border-collapse:collapse;font-size:13px}th{background:${brand};color:#fff;padding:8px;text-align:start}td{border-bottom:1px solid #ddd;padding:8px}
  .n{text-align:left;direction:ltr;font-weight:700}tfoot td{font-weight:800;background:#f4f4ef}.sign{display:grid;grid-template-columns:repeat(3,1fr);gap:30px;margin-top:60px;text-align:center}.sign div{border-top:1px solid #999;padding-top:6px}</style></head><body>
  <div class="top"><img src="${location.origin}/logo.jpeg"><h1>معهد السمو المتخصص العالي للتدريب</h1><div class="t"><b>${title}</b>رقم ${esc(code)} • ${esc(date)}</div></div>
  <p><b>البيان:</b> ${esc(notes)}</p>
  <table><thead><tr><th>#</th><th>الحساب</th><th>البيان</th><th>مركز التكلفة</th><th>مدين</th><th>دائن</th></tr></thead><tbody>${rows}</tbody>
  <tfoot><tr><td colspan="4">الإجمالي</td><td class="n">${money(tm)}</td><td class="n">${money(td)}</td></tr></tfoot></table>
  <div class="sign"><div>المحاسب</div><div>المراجع</div><div>المدير المالي</div></div>
  ${seal}
  <script>window.onload=()=>setTimeout(()=>window.print(),300)<\/script></body></html>`)
  w.document.close()
}
