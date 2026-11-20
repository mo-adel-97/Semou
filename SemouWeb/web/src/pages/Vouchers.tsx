import { useDeferredValue, useEffect, useMemo, useRef, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { ArrowDownToLine, ArrowUpFromLine, CalendarRange, Link2, Plus, Printer, Save, Scale, Search, Trash2, Wallet, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { PRINT_BASE_CSS, sealHtml } from '../lib/print'
import { useBranches } from '../lib/lookups'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { AccountPicker, type AccountLookup } from '../components/AccountPicker'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/* سند قبض / سند صرف (EncodingCashDayReceipt / EncodingCashDayPay). Each save also posts the journal entry. */
export type VoucherKind = 'receipt' | 'payment'
const CFG = {
  receipt: { title: 'سندات القبض', singular: 'سند قبض', form: 'receipt', head: 'مدين', line: 'دائن', headLabel: 'الخزينة / البنك (المدين)', icon: ArrowDownToLine },
  payment: { title: 'سندات الصرف', singular: 'سند صرف', form: 'pay', head: 'دائن', line: 'مدين', headLabel: 'الخزينة / البنك (الدائن)', icon: ArrowUpFromLine },
} as const

interface Row { Code: string; Guid: string; Date: string; Amount: number; AccountName: string; Notes: string; IsUse: boolean; BranchName: string | null; VendorName: string | null; LinkName: string | null; LinkRef: string | null }
interface CashAccount { Guid: string; Name: string; CashType: number; CostCenterGuid: string | null; CostName: string | null }
interface Line { key: number; accountGuid: string; accountCode: string; accountName: string; accountType: number | null; amount: string; costGuid: string; costName: string; notes: string }

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const dateOnly = (s: string) => s?.slice(0, 10)
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const monthStart = () => today().slice(0, 8) + '01'
const NO_ROWS: Row[] = []
let lineKey = 1

export function Vouchers({ kind, group }: { kind: VoucherKind; group: string }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const [range, setRange] = useState<'month' | 'all' | 'custom'>('month')
  const [from, setFrom] = useState(monthStart())
  const [to, setTo] = useState(today())
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const canFind = can(c.form, 'find'), canAdd = can(c.form, 'add')

  const params = range === 'all' ? {} : range === 'month' ? { from: monthStart(), to: today() } : { from, to }
  const list = useQuery({
    queryKey: ['vouchers', kind, 'list', params],
    queryFn: () => api<Row[]>(`/vouchers/${kind}${qs(params)}`),
    enabled: canFind, placeholderData: keepPreviousData,
  })
  const rows = list.data ?? NO_ROWS
  const active = rows.filter(r => r.IsUse)
  const total = active.reduce((s, r) => s + r.Amount, 0)

  const columns = useMemo<GridColumn<Row>[]>(() => [
    { accessorKey: 'Code', header: 'رقم السند', sortFn: 'alphanumeric', meta: { width: 100 }, cell: x => <span className="code-chip">{x.getValue() as string}</span> },
    { accessorKey: 'Date', header: 'التاريخ', cell: x => <span dir="ltr">{dateOnly(x.getValue() as string)}</span> },
    { accessorKey: 'AccountName', header: kind === 'receipt' ? 'الخزينة / البنك' : 'صرف من', cell: x => <b>{x.getValue() as string}</b> },
    { accessorKey: 'Amount', header: 'المبلغ', cell: x => <span className="num-strong">{money(x.getValue() as number)}</span> },
    { accessorKey: 'Notes', header: 'البيان', cell: x => <span className="clip" title={String(x.getValue() ?? '')}>{String(x.getValue() ?? '')}</span> },
    ...(kind === 'payment' ? [{ accessorKey: 'VendorName', header: 'المورد', cell: (x: { getValue: () => unknown }) => String(x.getValue() || '—') } as GridColumn<Row>] : []),
    { accessorKey: 'LinkName', header: 'مرتبط بـ', enableGlobalFilter: false, cell: x => (x.getValue() ? <span className="badge warn"><Link2 size={11} /> {String(x.getValue())}</span> : '—') },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} offText="ملغي" /> },
  ], [kind])

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  const Icon = c.icon
  return (
    <>
      <PageHead title={c.title} subtitle="كل سند يُنشئ قيد اليومية الخاص به تلقائياً" group={group}
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> {c.singular} جديد <kbd>F1</kbd></button>} />
      {!canFind ? <div className="card soon"><div className="ic"><Search size={36} /></div><h2>لا تملك صلاحية استعراض البيانات</h2></div> : (
        <>
          <div className="stats">
            <Stat icon={<Icon size={22} />} value={list.isPending ? '…' : rows.length.toLocaleString('ar-EG')} label="عدد السندات" />
            <Stat icon={<Wallet size={22} />} value={list.isPending ? '…' : money(total)} label="إجمالي السندات الفعالة (ر.س)" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : rows.length - active.length} label="سندات ملغاة" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} searchPlaceholder="ابحث برقم السند أو البيان أو الحساب..."
            onRowClick={r => setEditing({ guid: r.Guid })}
            toolbar={<>
              <Segmented value={range} onChange={setRange} options={[{ value: 'month', label: 'هذا الشهر' }, { value: 'custom', label: 'فترة' }, { value: 'all', label: 'الكل' }]} />
              {range === 'custom' && <div className="row" style={{ gap: 6 }}>
                <CalendarRange size={16} className="muted" />
                <input type="date" className="input" style={{ width: 150 }} value={from} onChange={e => setFrom(e.target.value)} />
                <span className="muted">إلى</span>
                <input type="date" className="input" style={{ width: 150 }} value={to} onChange={e => setTo(e.target.value)} />
              </div>}
              {list.isFetching && !list.isPending && <Spinner size={16} />}
            </>} />
        </>
      )}
      <AnimatePresence>{editing && <VoucherCard key={editing.guid ?? 'new'} kind={kind} guid={editing.guid} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

/* ============================== Voucher card ============================== */
interface Details {
  head: { Code: string; Guid: string; Date: string; AccountGuid: string; AccountName: string; BranchGuid: string | null; Amount: number; Notes: string; IsUse: boolean
    VendorName: string | null; VendorTax: string | null; VendorTel: string | null; LinkName: string | null; LinkRef: string | null; JournalCode: string | null; AmountText: string; BranchName: string | null }
  lines: { AccountGuid: string; AccountCode: string; AccountName: string; AccountType: number | null; Amount: number; CostGuid: string | null; CostName: string | null; Notes: string | null }[]
}
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
const newLine = (notes = '', cost?: { guid: string; name: string }): Line => ({ key: lineKey++, accountGuid: '', accountCode: '', accountName: '', accountType: null, amount: '', costGuid: cost?.guid ?? '', costName: cost?.name ?? '', notes })

function VoucherCard({ kind, guid: initialGuid, onClose }: { kind: VoucherKind; guid: string | null; onClose: () => void }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [created, setCreated] = useState<{ code: string; guid: string; journal: string | null } | null>(null)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid

  const details = useQuery({ queryKey: ['vouchers', kind, 'one', initialGuid], queryFn: () => api<Details>(`/vouchers/${kind}/${initialGuid}`), enabled: !!initialGuid, staleTime: 0 })
  const cash = useQuery({ queryKey: ['vouchers', 'cash-accounts'], queryFn: () => api<CashAccount[]>('/vouchers/lookups/cash-accounts'), staleTime: 60_000 })
  const branchCosts = useQuery({ queryKey: ['vouchers', 'branch-costs'], queryFn: () => api<{ Code: string; CenterName: string; Guid: string }[]>('/vouchers/lookups/branch-cost-centers'), staleTime: 60_000, enabled: kind === 'payment' })
  const defaults = useQuery({ queryKey: ['vouchers', 'defaults'], queryFn: () => api<{ ChkBranch: boolean; BranchGuid: string | null; ZeroAmount: boolean }>('/vouchers/lookups/defaults'), staleTime: 60_000 })

  const linked = !!details.data?.head.LinkName
  const canSave = !linked && (isNew ? can(c.form, 'add') : can(c.form, 'edit'))
  const ro = !canSave

  const [date, setDate] = useState(today())
  const [account, setAccount] = useState('')
  const [branch, setBranch] = useState('')
  const [amount, setAmount] = useState('')
  const [notes, setNotes] = useState('')
  const [isUse, setIsUse] = useState(true)
  const [vendor, setVendor] = useState({ name: '', tax: '', tel: '' })
  const [lines, setLines] = useState<Line[]>(() => [newLine()])
  const [picker, setPicker] = useState<{ type: 'account' | 'cost'; key: number } | null>(null)
  const baseline = useRef('')
  const snapshot = JSON.stringify([date, account, branch, amount, notes, isUse, vendor, lines.map(({ key: _k, ...l }) => l)])
  const dirty = baseline.current !== '' && snapshot !== baseline.current
  useEffect(() => { if (isNew && baseline.current === '') baseline.current = snapshot }) // eslint-disable-line react-hooks/exhaustive-deps

  // Default branch for receipts (ChkDefBranch).
  useEffect(() => { if (isNew && kind === 'receipt' && !branch && defaults.data?.BranchGuid) setBranch(defaults.data.BranchGuid) }, [defaults.data, isNew, kind, branch])

  useEffect(() => {
    const d = details.data
    if (!d) return
    const h = d.head
    setDate(dateOnly(h.Date)); setAccount(h.AccountGuid); setBranch(h.BranchGuid && h.BranchGuid !== EMPTY_GUID ? h.BranchGuid : '')
    setAmount(String(h.Amount ?? '')); setNotes(h.Notes ?? ''); setIsUse(!!h.IsUse)
    setVendor({ name: h.VendorName ?? '', tax: h.VendorTax ?? '', tel: h.VendorTel ?? '' })
    const ls = d.lines.map(l => ({ key: lineKey++, accountGuid: l.AccountGuid, accountCode: l.AccountCode, accountName: l.AccountName, accountType: l.AccountType,
      amount: String(l.Amount ?? ''), costGuid: l.CostGuid && l.CostGuid !== EMPTY_GUID ? l.CostGuid : '', costName: l.CostName ?? '', notes: l.Notes ?? '' }))
    setLines(ls.length ? ls : [newLine()])
    baseline.current = JSON.stringify([dateOnly(h.Date), h.AccountGuid, h.BranchGuid && h.BranchGuid !== EMPTY_GUID ? h.BranchGuid : '', String(h.Amount ?? ''), h.Notes ?? '', !!h.IsUse,
      { name: h.VendorName ?? '', tax: h.VendorTax ?? '', tel: h.VendorTel ?? '' }, ls.map(({ key: _k, ...l }) => l)])
  }, [details.data])

  const cashAcc = cash.data?.find(x => x.Guid === account)
  const amt = Number(amount) || 0
  const lineTotal = Math.round(lines.reduce((s, l) => s + (Number(l.amount) || 0), 0) * 100) / 100
  const diff = Math.round((amt - lineTotal) * 100) / 100
  const balanced = diff === 0 && (amt > 0 || !!defaults.data?.ZeroAmount)

  const deferredAmt = useDeferredValue(amt)
  const words = useQuery({ queryKey: ['tafkeet', deferredAmt], queryFn: () => api<{ text: string }>(`/vouchers/lookups/tafkeet?amount=${deferredAmt}`), enabled: deferredAmt > 0, staleTime: Infinity })

  const setLine = (key: number, patch: Partial<Line>) => setLines(ls => ls.map(l => (l.key === key ? { ...l, ...patch } : l)))
  const needsCost = (l: Line) => kind === 'receipt' || l.accountType === 0 || l.accountType === 1
  const addLine = () => setLines(ls => [...ls, newLine(notes, kind === 'receipt' && cashAcc?.CostCenterGuid ? { guid: cashAcc.CostCenterGuid, name: cashAcc.CostName ?? '' } : undefined)])
  const fillRest = (key: number) => { const others = lines.filter(l => l.key !== key).reduce((s, l) => s + (Number(l.amount) || 0), 0); setLine(key, { amount: String(Math.max(0, Math.round((amt - others) * 100) / 100)) }) }

  const save = useMutation({
    mutationFn: async () => {
      const body = {
        date, accountGuid: account || null, branchGuid: branch || null, amount: amt, notes, isUse,
        vendorName: vendor.name, vendorTax: vendor.tax, vendorTel: vendor.tel,
        lines: lines.filter(l => l.accountGuid).map(l => ({ accountGuid: l.accountGuid, amount: Number(l.amount) || 0, costGuid: l.costGuid || null, notes: l.notes })),
      }
      if (isNew) return api<{ message: string; code: string; guid: string; journalCode: string | null }>(`/vouchers/${kind}`, { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string; journalCode: string | null }>(`/vouchers/${kind}/${guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async r => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid, journal: r.journalCode })
      baseline.current = snapshot
      qc.invalidateQueries({ queryKey: ['vouchers', kind] })
      await alerts.success(r.message, `رقم السند: ${r.code}${r.journalCode ? ` • رقم القيد: ${r.journalCode}` : ''}`)
    },
  })

  // Client-side copy of VALIDATE_DATA for instant feedback (the server checks everything again).
  const validate = (): string | null => {
    if (kind === 'receipt' && !branch) return 'يرجى اختيار الفرع أولاً'
    if (!account) return kind === 'receipt' ? 'يرجى اختيار الحساب المدين أولاً' : 'يرجى اختيار الحساب الدائن أولاً'
    if (!notes.trim()) return `يرجى إدخال ملاحظات ${kind === 'receipt' ? 'سند القبض' : 'سند الصرف'} لتسهيل عملية مراجعة الحسابات`
    if (amt <= 0 && !defaults.data?.ZeroAmount) return `يرجى إدخال قيمة ${kind === 'receipt' ? 'سند القبض' : 'سند الصرف'}`
    const used = lines.filter(l => l.accountGuid)
    if (!used.length) return `يرجى التأكد من المبالغ المدرجة في الجانب ال${c.line}`
    if (diff !== 0) return 'السند غير متزن، يرجى مراجعة تفاصيل السند'
    if (used.some(l => needsCost(l) && !l.costGuid)) return 'لا يمكن الحفظ بدون اختيار مركز التكلفة'
    if (used.some(l => !l.notes.trim())) return 'لا يمكن الحفظ بدون إدخال البيان لتسهيل عملية المراجعة'
    return null
  }
  const submit = () => { const err = validate(); if (err) { alerts.error(err); return } save.mutate() }

  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !picker && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const print = () => {
    if (dirty) { notify.info('يرجى حفظ التعديلات أولاً قبل الطباعة'); return }
    if (guid) printVoucher(kind, guid, {
      code: created?.code ?? details.data?.head.Code ?? '', date, account: cashAcc?.Name ?? details.data?.head.AccountName ?? '', amount: amt,
      words: words.data?.text ?? details.data?.head.AmountText ?? '', notes, branch: branches.data?.find(b => b.guid === branch)?.name ?? '',
      vendor: vendor.name, lines: lines.filter(l => l.accountGuid),
    })
  }

  const loading = !!initialGuid && details.isPending
  const code = created?.code ?? details.data?.head.Code
  const journal = created?.journal ?? details.data?.head.JournalCode
  const Icon = c.icon

  return (
    <Drawer size="lg" onClose={requestClose} icon={<Icon size={22} />} title={isNew ? `${c.singular} جديد` : `${c.singular} رقم ${code}`}
      subtitle={journal ? `قيد اليومية رقم ${journal}` : 'سيتم إنشاء قيد اليومية تلقائياً عند الحفظ'}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        {!isNew && <button className="btn secondary" onClick={print}><Printer size={16} /> طباعة</button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{linked ? <><Link2 size={14} /> مرتبط بـ {details.data?.head.LinkName} ({details.data?.head.LinkRef}) ولا يمكن تعديله</> : ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <div className="grid" style={{ gap: 20 }}>
          <div className="grid cols-3">
            <Field label="التاريخ" required><input type="date" className="input" readOnly={ro} value={date} onChange={e => setDate(e.target.value)} /></Field>
            <Field label={c.headLabel} required>
              <select className="input" disabled={ro} value={account} onChange={e => {
                setAccount(e.target.value)
                const a = cash.data?.find(x => x.Guid === e.target.value)
                if (kind === 'receipt' && a?.CostCenterGuid) setLines(ls => ls.map(l => (l.costGuid ? l : { ...l, costGuid: a.CostCenterGuid!, costName: a.CostName ?? '' })))
              }}>
                <option value="">اختر...</option>
                <optgroup label="الخزائن">{cash.data?.filter(x => x.CashType === 0).map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</optgroup>
                <optgroup label="البنوك">{cash.data?.filter(x => x.CashType === 1).map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</optgroup>
              </select>
            </Field>
            {kind === 'receipt' ? (
              <Field label="الفرع" required>
                <select className="input" disabled={ro} value={branch} onChange={e => setBranch(e.target.value)}>
                  <option value="">اختر...</option>
                  {branches.data?.filter(b => b.isUse || b.guid === branch).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
                </select>
              </Field>
            ) : <Field label="اسم المورد"><input className="input" readOnly={ro} maxLength={150} value={vendor.name} onChange={e => setVendor(v => ({ ...v, name: e.target.value }))} /></Field>}
            {kind === 'payment' && <>
              <Field label="الرقم الضريبي للمورد"><input className="input" dir="ltr" readOnly={ro} maxLength={50} value={vendor.tax} onChange={e => setVendor(v => ({ ...v, tax: e.target.value }))} /></Field>
              <Field label="هاتف المورد"><input className="input" dir="ltr" readOnly={ro} maxLength={50} value={vendor.tel} onChange={e => setVendor(v => ({ ...v, tel: e.target.value }))} /></Field>
              <div />
            </>}
            <Field label="المبلغ" required className="amount-field">
              <div className="input-suffix"><input className="input big" dir="ltr" inputMode="decimal" readOnly={ro} value={amount} placeholder="0.00" onChange={e => setAmount(e.target.value.replace(/[^\d.]/g, ''))} /><span>ر.س</span></div>
            </Field>
            <Field label="البيان / الملاحظات" required className="span-2">
              <input className="input" readOnly={ro} maxLength={200} value={notes} onChange={e => setNotes(e.target.value)} placeholder="مثال: سداد فاتورة..." />
            </Field>
          </div>
          {amt > 0 && <div className="tafkeet">{words.data?.text?.trim() || '…'}</div>}

          {/* -------- Lines -------- */}
          <div>
            <div className="lines-head">
              <h4>الحسابات ال{c.line === 'دائن' ? 'دائنة' : 'مدينة'}</h4>
              <div className={`balance ${balanced ? 'ok' : 'bad'}`}>
                <Scale size={16} />
                {balanced ? 'السند متزن' : <>الفرق: <b dir="ltr">{money(diff)}</b></>}
              </div>
            </div>
            <div className="lines">
              <div className="ln head"><span>#</span><span>الحساب</span><span>البيان</span><span>المبلغ</span><span>مركز التكلفة</span><span /></div>
              <AnimatePresence initial={false}>
                {lines.map((l, i) => (
                  <motion.div key={l.key} className="ln" initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }}>
                    <span className="muted">{i + 1}</span>
                    <button type="button" className={`pick ${l.accountGuid ? '' : 'empty'}`} disabled={ro} onClick={() => setPicker({ type: 'account', key: l.key })}>
                      {l.accountGuid ? <><span className="code-chip">{l.accountCode}</span> {l.accountName}</> : 'اختر الحساب...'}
                    </button>
                    <input className="input" readOnly={ro} maxLength={200} value={l.notes} placeholder="البيان" onChange={e => setLine(l.key, { notes: e.target.value })} />
                    <div className="input-suffix">
                      <input className="input" dir="ltr" inputMode="decimal" readOnly={ro} value={l.amount} placeholder="0.00"
                        onChange={e => setLine(l.key, { amount: e.target.value.replace(/[^\d.]/g, '') })}
                        onDoubleClick={() => !ro && fillRest(l.key)} title="انقر نقراً مزدوجاً لوضع المبلغ المتبقي" />
                    </div>
                    {needsCost(l) ? (
                      kind === 'payment' ? (
                        <select className="input" disabled={ro} value={l.costGuid} onChange={e => setLine(l.key, { costGuid: e.target.value, costName: e.target.selectedOptions[0]?.text ?? '' })}>
                          <option value="">اختر...</option>
                          {branchCosts.data?.map(x => <option key={x.Guid} value={x.Guid}>{x.CenterName}</option>)}
                        </select>
                      ) : (
                        <button type="button" className={`pick ${l.costGuid ? '' : 'empty'}`} disabled={ro} onClick={() => setPicker({ type: 'cost', key: l.key })}>{l.costName || 'اختر...'}</button>
                      )
                    ) : <span className="muted" style={{ fontSize: 12 }}>لا يحتاج</span>}
                    <button type="button" className="btn ghost sm icon" disabled={ro || lines.length === 1} onClick={() => setLines(ls => ls.filter(x => x.key !== l.key))} aria-label="حذف السطر"><Trash2 size={15} /></button>
                  </motion.div>
                ))}
              </AnimatePresence>
              <div className="ln foot">
                <span />
                {!ro ? <button type="button" className="btn ghost sm" onClick={addLine}><Plus size={15} /> إضافة سطر</button> : <span />}
                <span className="muted" style={{ textAlign: 'end' }}>الإجمالي</span>
                <b dir="ltr" className="num-strong">{money(lineTotal)}</b>
                <span /><span />
              </div>
            </div>
          </div>

          {!isNew && <Switch label="السند فعال (إيقافه يلغي أثره في القيد)" checked={isUse} disabled={ro} onChange={setIsUse} />}
        </div>
      )}

      <AnimatePresence>
        {picker?.type === 'account' && <AccountPicker onClose={() => setPicker(null)} onPick={(a: AccountLookup) => {
          const keep = kind === 'receipt' || a.AccountType === 0 || a.AccountType === 1
          setLine(picker.key, { accountGuid: a.Guid, accountCode: a.Code, accountName: a.Name, accountType: a.AccountType, ...(keep ? {} : { costGuid: '', costName: '' }) })
          setPicker(null)
        }} />}
        {picker?.type === 'cost' && <CostCenterPicker onClose={() => setPicker(null)} onPick={x => { setLine(picker.key, { costGuid: x.guid, costName: x.name }); setPicker(null) }} />}
      </AnimatePresence>
    </Drawer>
  )
}

/* ---------------- Printable voucher ---------------- */
async function printVoucher(kind: VoucherKind, guid: string, v: { code: string; date: string; account: string; amount: number; words: string; notes: string; branch: string; vendor: string; lines: Line[] }) {
  const title = kind === 'receipt' ? 'سند قبض' : 'سند صرف'
  const esc = (s: string) => s.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))
  const brand = getComputedStyle(document.documentElement).getPropertyValue('--brand').trim() || '#4a7f5f'
  const rows = v.lines.map((l, i) => `<tr><td>${i + 1}</td><td>${esc(l.accountCode)} - ${esc(l.accountName)}</td><td>${esc(l.notes)}</td><td>${esc(l.costName || '—')}</td><td class="n">${money(Number(l.amount) || 0)}</td></tr>`).join('')
  let seal = ''
  try { seal = await sealHtml('cash', guid) } catch (e) { alerts.error((e as Error).message); return }
  const w = window.open('', '_blank', 'width=900,height=700')
  if (!w) { alerts.error('المتصفح منع فتح نافذة الطباعة، اسمح بالنوافذ المنبثقة'); return }
  w.document.write(`<!doctype html><html dir="rtl" lang="ar"><head><meta charset="utf-8"><title>${title} ${esc(v.code)}</title>
  <link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;800&display=swap" rel="stylesheet">
  <style>${PRINT_BASE_CSS}
    *{box-sizing:border-box} body{font-family:Cairo,sans-serif;margin:0;padding:28px;color:#111}
    .top{display:flex;align-items:center;gap:16px;border-bottom:3px solid ${brand};padding-bottom:14px}
    .top img{height:70px} .top h1{margin:0;font-size:20px} .top small{color:#666}
    .title{margin-inline-start:auto;text-align:center;border:2px solid ${brand};border-radius:12px;padding:6px 18px}
    .title b{display:block;font-size:22px;color:${brand}} .meta{display:grid;grid-template-columns:repeat(3,1fr);gap:10px;margin:18px 0}
    .meta div{background:#f6f6f2;border-radius:10px;padding:8px 12px} .meta span{display:block;color:#777;font-size:12px}
    .amount{display:flex;gap:14px;align-items:center;border:1px dashed ${brand};border-radius:12px;padding:12px 16px;margin-bottom:16px}
    .amount b{font-size:24px;color:${brand};direction:ltr} table{width:100%;border-collapse:collapse;font-size:13px}
    th{background:${brand};color:#fff;padding:8px;text-align:start} td{border-bottom:1px solid #ddd;padding:8px} .n{text-align:left;direction:ltr;font-weight:800}
    .sign{display:grid;grid-template-columns:repeat(3,1fr);gap:30px;margin-top:60px;text-align:center} .sign div{border-top:1px solid #999;padding-top:6px}
    @media print{body{padding:0}}
  </style></head><body>
  <div class="top"><img src="${location.origin}/logo.png"><div><h1>معهد السمو المتخصص العالي للتدريب</h1><small>${esc(v.branch)}</small></div>
    <div class="title"><b>${title}</b>رقم ${esc(v.code)}</div></div>
  <div class="meta"><div><span>التاريخ</span>${esc(v.date)}</div><div><span>${kind === 'receipt' ? 'إلى' : 'من'} الخزينة / البنك</span>${esc(v.account)}</div><div><span>${kind === 'payment' ? 'المورد' : 'الفرع'}</span>${esc((kind === 'payment' ? v.vendor : v.branch) || '—')}</div></div>
  <div class="amount"><b>${money(v.amount)} ر.س</b><span>${esc(v.words.trim())}</span></div>
  <p><b>البيان:</b> ${esc(v.notes)}</p>
  <table><thead><tr><th>#</th><th>الحساب</th><th>البيان</th><th>مركز التكلفة</th><th>المبلغ</th></tr></thead><tbody>${rows}</tbody></table>
  <div class="sign"><div>المحاسب</div><div>المدير المالي</div><div>${kind === 'receipt' ? 'المستلم منه' : 'المستلم'}</div></div>
  ${seal}
  <script>window.onload=()=>{setTimeout(()=>window.print(),300)}<\/script></body></html>`)
  w.document.close()
}

