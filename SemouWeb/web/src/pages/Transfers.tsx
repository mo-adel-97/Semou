import { useDeferredValue, useEffect, useMemo, useRef, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { ArrowLeftRight, ArrowLeft, CalendarRange, ClipboardList, Plus, Save, Search, Wallet, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/* تحويل نقدية (EncodingTransMoney). */
const FORM = 'transmoney'
interface Row { Code: string; Guid: string; Date: string; FromName: string; ToName: string; Amount: number; Ref: string; Notes: string; IsUse: boolean; BranchName: string | null }
interface CashAccount { Guid: string; Name: string; CashType: number }
const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const monthStart = () => today().slice(0, 8) + '01'
const NO_ROWS: Row[] = []

export function Transfers({ group }: { group: string }) {
  const { can } = useAuth()
  const [range, setRange] = useState<'month' | 'all' | 'custom'>('month')
  const [from, setFrom] = useState(monthStart())
  const [to, setTo] = useState(today())
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const canFind = can(FORM, 'find'), canAdd = can(FORM, 'add')
  const params = range === 'all' ? {} : range === 'month' ? { from: monthStart(), to: today() } : { from, to }
  const list = useQuery({ queryKey: ['transfers', 'list', params], queryFn: () => api<Row[]>(`/transfers${qs(params)}`), enabled: canFind, placeholderData: keepPreviousData })
  const rows = list.data ?? NO_ROWS

  const columns = useMemo<GridColumn<Row>[]>(() => [
    { accessorKey: 'Code', header: 'الرقم', sortFn: 'alphanumeric', meta: { width: 80 }, cell: x => <span className="code-chip">{x.getValue() as string}</span> },
    { accessorKey: 'Date', header: 'التاريخ', cell: x => <span dir="ltr">{String(x.getValue()).slice(0, 10)}</span> },
    { id: 'route', header: 'من ← إلى', accessorFn: r => `${r.FromName} ${r.ToName}`, cell: x => <span className="route"><b>{x.row.original.FromName}</b><ArrowLeft size={14} /><b>{x.row.original.ToName}</b></span> },
    { accessorKey: 'Amount', header: 'المبلغ', cell: x => <span className="num-strong">{money(x.getValue() as number)}</span> },
    { accessorKey: 'Ref', header: 'المرجع' },
    { accessorKey: 'Notes', header: 'البيان', cell: x => <span className="clip">{String(x.getValue() ?? '')}</span> },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} offText="ملغي" /> },
  ], [])

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  return (
    <>
      <PageHead title="تحويل النقدية" subtitle="نقل الأموال بين الخزائن والبنوك مع قيدها المحاسبي" group={group}
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> تحويل جديد <kbd>F1</kbd></button>} />
      {!canFind ? <div className="card soon"><div className="ic"><Search size={36} /></div><h2>لا تملك صلاحية استعراض البيانات</h2></div> : (
        <>
          <div className="stats">
            <Stat icon={<ArrowLeftRight size={22} />} value={list.isPending ? '…' : rows.length} label="عدد التحويلات" />
            <Stat icon={<Wallet size={22} />} value={list.isPending ? '…' : money(rows.filter(r => r.IsUse).reduce((s, r) => s + r.Amount, 0))} label="إجمالي المحول (ر.س)" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : rows.filter(r => !r.IsUse).length} label="ملغي" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} searchPlaceholder="ابحث بالرقم أو المرجع أو الخزينة..."
            onRowClick={r => setEditing({ guid: r.Guid })}
            toolbar={<>
              <Segmented value={range} onChange={setRange} options={[{ value: 'month', label: 'هذا الشهر' }, { value: 'custom', label: 'فترة' }, { value: 'all', label: 'الكل' }]} />
              {range === 'custom' && <div className="row" style={{ gap: 6 }}><CalendarRange size={16} className="muted" />
                <input type="date" className="input" style={{ width: 150 }} value={from} onChange={e => setFrom(e.target.value)} /><span className="muted">إلى</span>
                <input type="date" className="input" style={{ width: 150 }} value={to} onChange={e => setTo(e.target.value)} /></div>}
            </>} />
        </>
      )}
      <AnimatePresence>{editing && <TransferCard key={editing.guid ?? 'new'} guid={editing.guid} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

function TransferCard({ guid: initialGuid, onClose }: { guid: string | null; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [created, setCreated] = useState<{ code: string; guid: string; journal: string | null } | null>(null)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid
  const canSave = isNew ? can(FORM, 'add') : can(FORM, 'edit')
  const ro = !canSave
  const cash = useQuery({ queryKey: ['vouchers', 'cash-accounts'], queryFn: () => api<CashAccount[]>('/vouchers/lookups/cash-accounts'), staleTime: 60_000 })
  const details = useQuery({ queryKey: ['transfers', 'one', initialGuid], queryFn: () => api<Record<string, unknown>>(`/transfers/${initialGuid}`), enabled: !!initialGuid, staleTime: 0 })

  const [f, setF] = useState({ date: today(), branch: '', from: '', to: '', cost: '', costName: '', amount: '', ref: '', notes: '', isUse: true })
  const [pick, setPick] = useState(false)
  const baseline = useRef('')
  const dirty = baseline.current !== '' && JSON.stringify(f) !== baseline.current
  useEffect(() => { if (isNew && baseline.current === '') baseline.current = JSON.stringify(f) }) // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => {
    const d = details.data
    if (!d) return
    const v = { date: String(d.Date).slice(0, 10), branch: String(d.BranchGuid ?? ''), from: String(d.FromGuid ?? ''), to: String(d.ToGuid ?? ''), cost: String(d.CostCenterGuid ?? ''),
      costName: String(d.CostName ?? ''), amount: String(d.Amount ?? ''), ref: String(d.Ref ?? ''), notes: String(d.Notes ?? ''), isUse: !!d.IsUse }
    setF(v); baseline.current = JSON.stringify(v)
  }, [details.data])
  const set = (patch: Partial<typeof f>) => setF(x => ({ ...x, ...patch }))
  const amt = Number(f.amount) || 0
  const deferred = useDeferredValue(amt)
  const words = useQuery({ queryKey: ['tafkeet', deferred], queryFn: () => api<{ text: string }>(`/vouchers/lookups/tafkeet?amount=${deferred}`), enabled: deferred > 0, staleTime: Infinity })

  const save = useMutation({
    mutationFn: async () => {
      const body = { date: f.date, branchGuid: f.branch || null, fromGuid: f.from || null, toGuid: f.to || null, costCenterGuid: f.cost || null, amount: amt, ref: f.ref, notes: f.notes, isUse: f.isUse }
      if (isNew) return api<{ message: string; code: string; guid: string; journalCode: string | null }>('/transfers', { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string; journalCode: string | null }>(`/transfers/${guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async r => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid, journal: r.journalCode })
      baseline.current = JSON.stringify(f)
      qc.invalidateQueries({ queryKey: ['transfers'] })
      await alerts.success(r.message, `رقم التحويل: ${r.code}${r.journalCode ? ` • رقم القيد: ${r.journalCode}` : ''}`)
    },
  })
  const validate = () => {
    if (!f.branch) return 'يرجى اختيار الفرع أولاً'
    if (!f.from) return 'يرجى اختيار الخزينة / البنك المحول منها'
    if (!f.to) return 'يرجى اختيار الخزينة / البنك المحول إليها'
    if (f.from === f.to) return 'لا يمكن التحويل من وإلى نفس الخزينة / البنك'
    if (!f.cost) return 'يرجى اختيار مركز التكلفة الخاص بعملية السداد'
    if (!f.ref.trim()) return 'يرجى إدخال رقم المرجع الخاص بعملية السداد'
    if (amt <= 0) return 'يرجى إدخال المبلغ الخاص بعملية السداد'
    if (!f.notes.trim()) return 'يرجى إدخال البيان الخاص بعملية التحويل'
    return null
  }
  const submit = () => { const e = validate(); if (e) { alerts.error(e); return } save.mutate() }
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !pick && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const boxSelect = (value: string, onChange: (v: string) => void) => (
    <select className="input" disabled={ro} value={value} onChange={e => onChange(e.target.value)}>
      <option value="">اختر...</option>
      <optgroup label="الخزائن">{cash.data?.filter(x => x.CashType === 0).map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</optgroup>
      <optgroup label="البنوك">{cash.data?.filter(x => x.CashType === 1).map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</optgroup>
    </select>
  )
  const loading = !!initialGuid && details.isPending
  const journal = created?.journal ?? (details.data?.JournalCode as string | undefined)
  return (
    <Drawer onClose={requestClose} icon={<ArrowLeftRight size={22} />} title={isNew ? 'تحويل نقدية جديد' : `تحويل رقم ${created?.code ?? details.data?.Code}`}
      subtitle={journal ? `قيد اليومية رقم ${journal}` : 'سيتم إنشاء قيد اليومية تلقائياً'}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid">{Array.from({ length: 6 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <div className="grid cols-2">
          <Field label="التاريخ" required><input type="date" className="input" readOnly={ro} value={f.date} onChange={e => set({ date: e.target.value })} /></Field>
          <Field label="الفرع" required>
            <select className="input" disabled={ro} value={f.branch} onChange={e => set({ branch: e.target.value })}>
              <option value="">اختر...</option>
              {branches.data?.filter(b => b.isUse || b.guid === f.branch).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
            </select>
          </Field>
          <motion.div className="span-all transfer-route" layout>
            <Field label="من (الخزينة / البنك المحول منها)" required>{boxSelect(f.from, v => set({ from: v }))}</Field>
            <div className="arrow"><ArrowLeft size={22} /></div>
            <Field label="إلى (الخزينة / البنك المحول إليها)" required>{boxSelect(f.to, v => set({ to: v }))}</Field>
          </motion.div>
          <Field label="المبلغ" required><div className="input-suffix"><input className="input big" dir="ltr" inputMode="decimal" readOnly={ro} value={f.amount} placeholder="0.00" onChange={e => set({ amount: e.target.value.replace(/[^\d.]/g, '') })} /><span>ر.س</span></div></Field>
          <Field label="رقم المرجع" required><input className="input" dir="ltr" readOnly={ro} maxLength={50} value={f.ref} onChange={e => set({ ref: e.target.value })} /></Field>
          {amt > 0 && <div className="tafkeet span-all">{words.data?.text?.trim() || '…'}</div>}
          <Field label="مركز التكلفة" required className="span-2">
            <div className="input-wrap"><ClipboardList size={17} />
              <input className="input" readOnly placeholder="اختر مركز التكلفة" value={f.costName} onClick={() => !ro && setPick(true)} style={{ cursor: ro ? 'default' : 'pointer' }} />
              {!ro && <button type="button" className="btn secondary sm trail" onClick={() => setPick(true)}><Search size={14} /> اختيار</button>}
            </div>
          </Field>
          <Field label="البيان" required className="span-2"><textarea className="input" readOnly={ro} value={f.notes} onChange={e => set({ notes: e.target.value })} /></Field>
          {!isNew && <div className="span-2"><Switch label="التحويل فعال" checked={f.isUse} disabled={ro} onChange={v => set({ isUse: v })} /></div>}
        </div>
      )}
      <AnimatePresence>{pick && <CostCenterPicker onClose={() => setPick(false)} onPick={x => { set({ cost: x.guid, costName: x.name }); setPick(false) }} />}</AnimatePresence>
    </Drawer>
  )
}
