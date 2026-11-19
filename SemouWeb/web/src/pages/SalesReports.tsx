import { useMemo, useState } from 'react'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { Printer, Receipt, Undo2, Wallet } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { printRegistration, printReturn, printTaxInvoice } from '../lib/print'
import { PageHead } from '../components/Shell'
import { ReportGrid, type RCol } from '../components/ReportGrid'
import { Segmented, Switch } from '../components/ui'
import { Bar, BranchSelect, CostSelect, Dates, Exports, F, SearchBox, Select, Summary, Waiting, count, day, money, monthStart, today, type Params } from '../components/ReportKit'
import { StudentStatement } from './Statement'

/*
 * قائمة التقارير: التسجيلات، المبيعات، مبيعات الضرائب، مرتجع مبيعات الضرائب، المسوقين، متابعة السداد، أرصدة العملاء، تحليل المصروفات.
 * Amounts follow the invoice print: Total = before VAT, SubTotal = including VAT.
 */
type Opt = { guid: string; name: string }
interface Lookups { sellers: Opt[]; users: Opt[]; batches: Opt[]; diploms: Opt[]; courses: Opt[]; books: Opt[]; cashBoxes: (Opt & { type: number })[] }
const useLookups = () => useQuery({ queryKey: ['sales-reports', 'lookups'], queryFn: () => api<Lookups>('/reports/sales/lookups'), staleTime: 10 * 60_000 })
const useReport = <T,>(path: string, params: Params | null) =>
  useQuery({ queryKey: ['sales-reports', path, params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<T>(`/reports/sales/${path}${qs(params!)}`) })
const regTypeName = (t: number) => (t === 1 ? 'دورة' : 'دبلوم')
const Badge = ({ ok, yes, no }: { ok: boolean; yes: string; no: string }) => <span className={`badge ${ok ? 'ok' : 'muted'}`}>{ok ? yes : no}</span>
type Stmt = { account: string; name: string } | null
const sub = (p: Params | null) => (p ? `من ${p.from} إلى ${p.to}` : '')

/* ======================= تقرير التسجيلات ======================= */
interface RegRow { guid: string; accountGuid: string; date: string; code: string; acadmyId: string | null; studentName: string; nationalId: string | null; studentTel: string | null
  programs: string | null; regType: number; batch: string | null; seller: string | null; branch: string | null; book: string | null; total: number; tax: number; subTotal: number; accepted: boolean; returned: boolean; notes: string | null }

export function RegistrationsReport({ group }: { group: string }) {
  const { can } = useAuth()
  const lk = useLookups()
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [seller, setSeller] = useState(''); const [batch, setBatch] = useState('')
  const [type, setType] = useState(''); const [program, setProgram] = useState(''); const [q, setQ] = useState('')
  const [params, setParams] = useState<Params | null>(null)
  const [stmt, setStmt] = useState<Stmt>(null)
  const r = useReport<RegRow[]>('registrations', params)
  const rows = r.data ?? []
  const programs = type === '1' ? lk.data?.courses : type === '0' ? lk.data?.diploms : [...(lk.data?.diploms ?? []), ...(lk.data?.courses ?? [])]
  const cols: RCol<RegRow>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: x => day(x.date) },
    { key: 'code', label: 'الاستمارة', width: 95 },
    { key: 'studentName', label: 'اسم الطالب', width: 200, render: x => <b>{x.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 105 },
    { key: 'studentTel', label: 'الجوال', width: 105 },
    { key: 'regType', label: 'النوع', width: 82, value: x => regTypeName(x.regType) },
    { key: 'programs', label: 'البرنامج', width: 185 },
    { key: 'batch', label: 'الدفعة', width: 95 },
    { key: 'seller', label: 'المسوق', width: 125 },
    { key: 'branch', label: 'الفرع', width: 150, hidden: true },
    { key: 'total', label: 'قبل الضريبة', width: 105, num: true },
    { key: 'tax', label: 'الضريبة', width: 95, num: true },
    { key: 'subTotal', label: 'الإجمالي', width: 110, num: true, tone: 'due' },
    { key: 'status', label: 'الحالة', width: 130, value: x => (x.returned ? 'مرتجع' : x.accepted ? 'مقبول' : 'بانتظار القبول'),
      render: x => <span className={`badge ${x.returned ? 'due' : x.accepted ? 'ok' : 'warn'}`}>{x.returned ? 'مرتجع' : x.accepted ? 'مقبول' : 'بانتظار القبول'}</span> },
    { key: 'acadmyId', label: 'الرقم الأكاديمي', width: 95, hidden: true },
    { key: 'book', label: 'الدفتر', width: 130, hidden: true },
    { key: 'notes', label: 'ملاحظات', width: 160, hidden: true },
    { key: 'actions', label: '', width: 80, noFilter: true, foot: false, value: () => '', render: x => (
      <span className="rg-actions rg-icons" onClick={e => e.stopPropagation()}>
        <button className="btn ghost sm icon" title="طباعة الاستمارة" onClick={() => printRegistration(x.guid)}><Printer size={15} /></button>
        {can('studentstatment', 'find') && <button className="btn ghost sm icon" title="كشف حساب الطالب" onClick={() => setStmt({ account: x.accountGuid, name: x.studentName })}><Wallet size={15} /></button>}
      </span>) },
  ]
  const live = rows.filter(x => !x.returned)
  return (
    <>
      <PageHead title="تقرير التسجيلات" subtitle="استمارات التسجيل خلال الفترة حسب الفرع والمسوق والدفعة والبرنامج" group={group} />
      <Bar onRun={() => setParams({ from, to, branch: branch || null, seller: seller || null, batch: batch || null, regType: type || null, program: program || null, q: q.trim() || null })} loading={r.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <F label="النوع"><Segmented value={type} onChange={v => { setType(v); setProgram('') }} options={[{ value: '', label: 'الكل' }, { value: '0', label: 'دبلوم' }, { value: '1', label: 'دورة' }]} /></F>
        <Select label="البرنامج" value={program} onChange={setProgram} list={programs} all="كل البرامج" w={200} />
        {type !== '1' && <Select label="الدفعة" value={batch} onChange={setBatch} list={lk.data?.batches} all="كل الدفعات" w={130} />}
        <Select label="المسوق" value={seller} onChange={setSeller} list={lk.data?.sellers} all="كل المسوقين" w={170} />
        <SearchBox value={q} onChange={setQ} placeholder="الاسم أو الهوية أو رقم الاستمارة" />
      </Bar>
      {!r.data ? <Waiting loading={r.isFetching} error={r.error?.message} /> : <>
        <Summary items={[
          { label: 'عدد الاستمارات', value: count(rows.length), tone: 'brand' },
          { label: 'دبلومات / دورات', value: `${count(live.filter(x => x.regType === 0).length)} / ${count(live.filter(x => x.regType === 1).length)}` },
          { label: 'بانتظار القبول', value: count(live.filter(x => !x.accepted).length) },
          { label: 'مرتجعة', value: count(rows.length - live.length), tone: rows.length - live.length ? 'due' : undefined },
          { label: 'صافي التسجيلات شامل الضريبة', value: money(live.reduce((s, x) => s + x.subTotal, 0)), tone: 'ok' },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={x => x.guid} rowClass={x => (x.returned ? 'dim' : undefined)} footLabel={`${rows.length} استمارة`}
          toolbar={<Exports title="تقرير التسجيلات" sub={sub(params)} cols={cols} rows={rows} />} />
      </>}
      <AnimatePresence>{stmt && <StudentStatement account={stmt.account} name={stmt.name} onClose={() => setStmt(null)} />}</AnimatePresence>
    </>
  )
}

/* ======================= تقرير المبيعات ======================= */
interface SaleRow { guid: string; accountGuid: string; date: string; code: string; taxNo: string; book: string | null; invoiceType: number; studentName: string; nationalId: string | null; studentTel: string | null
  notes: string | null; total: number; tax: number; subTotal: number; branch: string | null; user: string | null; active: boolean }
const KINDS = [{ v: '0', label: 'دفعة التسجيل' }, { v: '1', label: 'الأقساط' }, { v: '2', label: 'الرسوم' }]
const kindName = (k: number) => KINDS.find(x => x.v === String(k))?.label ?? 'أخرى'

export function SalesInvoicesReport({ group }: { group: string }) {
  const { can } = useAuth()
  const lk = useLookups()
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [user, setUser] = useState(''); const [book, setBook] = useState('')
  const [state, setState] = useState('true'); const [kinds, setKinds] = useState<string[]>(['0', '1', '2']); const [q, setQ] = useState('')
  const [params, setParams] = useState<Params | null>(null)
  const [stmt, setStmt] = useState<Stmt>(null)
  const r = useReport<SaleRow[]>('invoices', params)
  const rows = r.data ?? []
  const cols: RCol<SaleRow>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: x => day(x.date) },
    { key: 'taxNo', label: 'رقم الفاتورة', width: 95 },
    { key: 'invoiceType', label: 'نوع الفاتورة', width: 105, value: x => kindName(x.invoiceType) },
    { key: 'book', label: 'الدفتر', width: 140, hidden: !!params?.book },
    { key: 'studentName', label: 'اسم الطالب', width: 200, render: x => <b>{x.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 105 },
    { key: 'studentTel', label: 'الجوال', width: 105, hidden: true },
    { key: 'total', label: 'قبل الضريبة', width: 105, num: true },
    { key: 'tax', label: 'الضريبة', width: 90, num: true },
    { key: 'subTotal', label: 'الإجمالي', width: 110, num: true, tone: 'ok' },
    { key: 'branch', label: 'الفرع', width: 150, hidden: true },
    { key: 'user', label: 'المستخدم', width: 130, hidden: !!params?.user },
    { key: 'active', label: 'الحالة', width: 80, value: x => (x.active ? 'سارية' : 'ملغاة'), render: x => <Badge ok={x.active} yes="سارية" no="ملغاة" /> },
    { key: 'notes', label: 'البيان', width: 180, hidden: true },
    { key: 'code', label: 'الرقم الداخلي', width: 80, hidden: true },
    { key: 'actions', label: '', width: 80, noFilter: true, foot: false, value: () => '', render: x => (
      <span className="rg-actions rg-icons" onClick={e => e.stopPropagation()}>
        <button className="btn ghost sm icon" title="طباعة الفاتورة" onClick={() => printTaxInvoice(x.guid)}><Receipt size={15} /></button>
        {can('studentstatment', 'find') && <button className="btn ghost sm icon" title="كشف حساب الطالب" onClick={() => setStmt({ account: x.accountGuid, name: x.studentName })}><Wallet size={15} /></button>}
      </span>) },
  ]
  const toggleKind = (k: string) => setKinds(ks => (ks.includes(k) ? ks.filter(x => x !== k) : [...ks, k]))
  const sum = (k: 'total' | 'tax' | 'subTotal') => rows.reduce((s, x) => s + x[k], 0)
  return (
    <>
      <PageHead title="تقرير المبيعات" subtitle="الفواتير الضريبية الصادرة خلال الفترة" group={group} />
      <Bar onRun={() => setParams({ from, to, branch: branch || null, user: user || null, book: book || null, active: state || null, kinds: kinds.join(','), q: q.trim() || null })} loading={r.isFetching} disabled={!kinds.length}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <Select label="الدفتر" value={book} onChange={setBook} list={lk.data?.books} all="كل الدفاتر" w={170} />
        <Select label="المستخدم" value={user} onChange={setUser} list={lk.data?.users} all="كل المستخدمين" w={160} />
        <F label="نوع الفاتورة"><div className="rb-checks">{KINDS.map(k => <Switch key={k.v} label={k.label} checked={kinds.includes(k.v)} onChange={() => toggleKind(k.v)} />)}</div></F>
        <F label="الحالة"><Segmented value={state} onChange={setState} options={[{ value: 'true', label: 'سارية' }, { value: 'false', label: 'ملغاة' }, { value: '', label: 'الكل' }]} /></F>
        <SearchBox value={q} onChange={setQ} placeholder="الاسم أو الهوية أو رقم الفاتورة" />
      </Bar>
      {!r.data ? <Waiting loading={r.isFetching} error={r.error?.message} /> : <>
        <Summary items={[
          { label: 'عدد الفواتير', value: count(rows.length), tone: 'brand' },
          { label: 'قبل الضريبة', value: money(sum('total')) },
          { label: 'ضريبة القيمة المضافة', value: money(sum('tax')), tone: 'due' },
          { label: 'الإجمالي شامل الضريبة', value: money(sum('subTotal')), tone: 'ok' },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={x => x.guid} rowClass={x => (x.active ? undefined : 'dim')} footLabel={`${rows.length} فاتورة`}
          toolbar={<Exports title="تقرير المبيعات" sub={sub(params)} cols={cols} rows={rows} />} />
      </>}
      <AnimatePresence>{stmt && <StudentStatement account={stmt.account} name={stmt.name} onClose={() => setStmt(null)} />}</AnimatePresence>
    </>
  )
}

/* ======================= مبيعات الضرائب / مرتجعها ======================= */
interface TaxRow { guid: string; date: string; code: string; taxNo: string; book: string | null; studentName: string; nationalId: string | null; studentTel: string | null
  program: string | null; duration: string | null; cost: number; tax: number; subTotal: number; branch: string | null; user: string | null; notes: string | null; invoiceTaxNo: string | null; active: boolean }

export const TaxReport = ({ group }: { group: string }) => <TaxLines kind="sales" group={group} />
export const TaxReturnReport = ({ group }: { group: string }) => <TaxLines kind="returns" group={group} />

function TaxLines({ kind, group }: { kind: 'sales' | 'returns'; group: string }) {
  const ret = kind === 'returns'
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [state, setState] = useState('true'); const [q, setQ] = useState('')
  const [params, setParams] = useState<Params | null>(null)
  const r = useReport<TaxRow[]>(ret ? 'tax-returns' : 'tax', params)
  const rows = r.data ?? []
  const title = ret ? 'تقرير مرتجع مبيعات الضرائب' : 'تقرير مبيعات الضرائب'
  const cols: RCol<TaxRow>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: x => day(x.date) },
    { key: 'taxNo', label: ret ? 'رقم المرتجع' : 'رقم الفاتورة', width: 85 },
    ...(ret ? [{ key: 'invoiceTaxNo', label: 'الفاتورة الأصلية', width: 90 }] as RCol<TaxRow>[] : []),
    { key: 'studentName', label: 'اسم العميل', width: 190, render: x => <b>{x.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 105 },
    { key: 'program', label: 'البند', width: 200 },
    { key: 'duration', label: 'المدة', width: 80 },
    { key: 'cost', label: 'قبل الضريبة', width: 105, num: true },
    { key: 'tax', label: 'الضريبة', width: 95, num: true, tone: 'due' },
    { key: 'subTotal', label: 'الإجمالي', width: 110, num: true, tone: 'ok' },
    { key: 'book', label: 'الدفتر', width: 130, hidden: true },
    { key: 'branch', label: 'الفرع', width: 150, hidden: true },
    { key: 'user', label: 'المستخدم', width: 120, hidden: true },
    { key: 'studentTel', label: 'الجوال', width: 105, hidden: true },
    { key: 'active', label: 'الحالة', width: 80, value: x => (x.active ? 'سارية' : 'ملغاة'), render: x => <Badge ok={x.active} yes="سارية" no="ملغاة" /> },
    { key: 'actions', label: '', width: 50, noFilter: true, foot: false, value: () => '', render: x => (
      <span className="rg-actions rg-icons" onClick={e => e.stopPropagation()}>
        {ret ? <button className="btn ghost sm icon" title="طباعة المرتجع" onClick={() => printReturn('bill', x.guid)}><Undo2 size={15} /></button>
             : <button className="btn ghost sm icon" title="طباعة الفاتورة" onClick={() => printTaxInvoice(x.guid)}><Receipt size={15} /></button>}
      </span>) },
  ]
  const docs = new Set(rows.map(x => x.guid)).size
  const sum = (k: 'cost' | 'tax' | 'subTotal') => rows.reduce((s, x) => s + x[k], 0)
  return (
    <>
      <PageHead title={title} subtitle={ret ? 'بنود مرتجعات الفواتير الضريبية وضريبتها خلال الفترة' : 'بنود الفواتير الضريبية وضريبة القيمة المضافة المستحقة خلال الفترة'} group={group} />
      <Bar onRun={() => setParams({ from, to, branch: branch || null, active: state || null, q: q.trim() || null })} loading={r.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <F label="الحالة"><Segmented value={state} onChange={setState} options={[{ value: 'true', label: 'سارية' }, { value: 'false', label: 'ملغاة' }, { value: '', label: 'الكل' }]} /></F>
        <SearchBox value={q} onChange={setQ} placeholder={ret ? 'الاسم أو الهوية أو رقم المرتجع' : 'الاسم أو الهوية أو رقم الفاتورة'} />
      </Bar>
      {!r.data ? <Waiting loading={r.isFetching} error={r.error?.message} /> : <>
        <Summary items={[
          { label: ret ? 'عدد المرتجعات' : 'عدد الفواتير', value: count(docs), tone: 'brand' },
          { label: 'المبيعات قبل الضريبة', value: money(sum('cost')) },
          { label: ret ? 'ضريبة مستردة' : 'ضريبة القيمة المضافة', value: money(sum('tax')), tone: 'due' },
          { label: 'الإجمالي شامل الضريبة', value: money(sum('subTotal')), tone: 'ok' },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={(x, i) => `${x.guid}-${i}`} rowClass={x => (x.active ? undefined : 'dim')} footLabel={`${rows.length} بند`}
          toolbar={<Exports title={title} sub={sub(params)} cols={cols} rows={rows} />} />
      </>}
    </>
  )
}

/* ======================= تقرير المسوقين ======================= */
interface SellerRow { guid: string; accountGuid: string; date: string; code: string; seller: string | null; branch: string | null; studentName: string; nationalId: string | null; studentTel: string | null
  programs: string | null; regType: number; subTotal: number; returned: boolean }

export function MarketersReport({ group }: { group: string }) {
  const lk = useLookups()
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [seller, setSeller] = useState(''); const [type, setType] = useState('')
  const [params, setParams] = useState<Params | null>(null)
  const [view, setView] = useState('summary')
  const r = useReport<SellerRow[]>('sellers', params)
  const rows = r.data ?? []
  type Agg = { seller: string; total: number; diplomas: number; courses: number; returned: number; net: number; amount: number }
  const agg = useMemo<Agg[]>(() => {
    const m = new Map<string, Agg>()
    for (const x of rows) {
      const k = x.seller ?? '—'
      const a = m.get(k) ?? { seller: k, total: 0, diplomas: 0, courses: 0, returned: 0, net: 0, amount: 0 }
      a.total++; if (x.returned) a.returned++; else { a.net++; a.amount += x.subTotal; if (x.regType === 1) a.courses++; else a.diplomas++ }
      m.set(k, a)
    }
    return [...m.values()].sort((a, b) => b.net - a.net || b.amount - a.amount)
  }, [rows])
  const best = agg[0]?.net ?? 0
  const aggCols: RCol<Agg>[] = [
    { key: 'seller', label: 'المسوق', width: 220, render: x => <b>{x.seller}</b> },
    { key: 'net', label: 'صافي التسجيلات', width: 110, num: true, int: true },
    { key: 'bar', label: 'الأداء', width: 200, noFilter: true, foot: false, value: x => x.net, render: x => <span className="perf"><i style={{ width: `${best ? x.net / best * 100 : 0}%` }} /></span> },
    { key: 'diplomas', label: 'دبلومات', width: 90, num: true, int: true },
    { key: 'courses', label: 'دورات', width: 90, num: true, int: true },
    { key: 'returned', label: 'مرتجعة', width: 90, num: true, int: true, blankZero: true, tone: 'due' },
    { key: 'amount', label: 'قيمة التسجيلات', width: 140, num: true, tone: 'ok' },
  ]
  const cols: RCol<SellerRow>[] = [
    { key: 'seller', label: 'المسوق', width: 160 },
    { key: 'date', label: 'التاريخ', width: 100, value: x => day(x.date) },
    { key: 'code', label: 'رقم الاستمارة', width: 85 },
    { key: 'studentName', label: 'اسم الطالب', width: 200, render: x => <b>{x.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 105 },
    { key: 'studentTel', label: 'الجوال', width: 105 },
    { key: 'regType', label: 'النوع', width: 82, value: x => regTypeName(x.regType) },
    { key: 'programs', label: 'البرنامج', width: 220 },
    { key: 'subTotal', label: 'القيمة', width: 110, num: true },
    { key: 'branch', label: 'الفرع', width: 150, hidden: true },
    { key: 'returned', label: 'الحالة', width: 80, value: x => (x.returned ? 'مرتجع' : 'سارٍ'), render: x => <Badge ok={!x.returned} yes="سارٍ" no="مرتجع" /> },
  ]
  return (
    <>
      <PageHead title="تقرير المسوقين" subtitle="تسجيلات كل مسوق خلال الفترة وترتيبهم حسب الأداء" group={group} />
      <Bar onRun={() => setParams({ from, to, branch: branch || null, seller: seller || null, regType: type || null })} loading={r.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <Select label="المسوق" value={seller} onChange={setSeller} list={lk.data?.sellers} all="كل المسوقين" w={190} />
        <F label="النوع"><Segmented value={type} onChange={setType} options={[{ value: '', label: 'الكل' }, { value: '0', label: 'دبلوم' }, { value: '1', label: 'دورة' }]} /></F>
      </Bar>
      {!r.data ? <Waiting loading={r.isFetching} error={r.error?.message} /> : <>
        <Summary items={[
          { label: 'عدد المسوقين', value: count(agg.length), tone: 'brand' },
          { label: 'صافي التسجيلات', value: count(agg.reduce((s, a) => s + a.net, 0)) },
          { label: 'المرتجعة', value: count(agg.reduce((s, a) => s + a.returned, 0)), tone: 'due' },
          { label: 'قيمة التسجيلات', value: money(agg.reduce((s, a) => s + a.amount, 0)), tone: 'ok' },
          { label: 'الأعلى أداءً', value: agg[0] ? `${agg[0].seller} (${count(agg[0].net)})` : '—' },
        ]} />
        {view === 'summary'
          ? <ReportGrid columns={aggCols} rows={agg} rowKey={x => x.seller} footLabel={`${agg.length} مسوق`}
              toolbar={<><Segmented value={view} onChange={setView} options={[{ value: 'summary', label: 'ملخص المسوقين' }, { value: 'detail', label: 'التفاصيل' }]} />
                <Exports title="تقرير المسوقين - ملخص" sub={sub(params)} cols={aggCols.filter(c => c.key !== 'bar')} rows={agg} /></>} />
          : <ReportGrid columns={cols} rows={rows} rowKey={x => x.guid} rowClass={x => (x.returned ? 'dim' : undefined)} footLabel={`${rows.length} استمارة`}
              toolbar={<><Segmented value={view} onChange={setView} options={[{ value: 'summary', label: 'ملخص المسوقين' }, { value: 'detail', label: 'التفاصيل' }]} />
                <Exports title="تقرير المسوقين" sub={sub(params)} cols={cols} rows={rows} /></>} />}
      </>}
    </>
  )
}

/* ======================= متابعة السداد / أرصدة العملاء ======================= */
interface BalRow { accountGuid: string; code: string; acadmyId: string | null; studentName: string; studentTel: string | null; nationalId: string | null; branch: string | null
  opening: number; maden: number; daen: number; balance: number }

export const PaymentFollow = ({ group }: { group: string }) => <Balances kind="follow" group={group} />
export const CustomerBalances = ({ group }: { group: string }) => <Balances kind="balances" group={group} />

function Balances({ kind, group }: { kind: 'follow' | 'balances'; group: string }) {
  const { can } = useAuth()
  const lk = useLookups()
  const follow = kind === 'follow'
  const [from, setFrom] = useState(`${new Date().getFullYear()}-01-01`); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [diplom, setDiplom] = useState(''); const [batch, setBatch] = useState(''); const [q, setQ] = useState('')
  const [show, setShow] = useState(follow ? 'due' : 'all')
  const [params, setParams] = useState<Params | null>(null)
  const [stmt, setStmt] = useState<Stmt>(null)
  const r = useReport<BalRow[]>(follow ? 'payment-follow' : 'customer-balances', params)
  const all = r.data ?? []
  const rows = useMemo(() => all.filter(x => show === 'due' ? x.balance > 0.009 : show === 'credit' ? x.balance < -0.009 : show === 'zero' ? Math.abs(x.balance) < 0.01 : true), [all, show])
  const title = follow ? 'متابعة السداد' : 'أرصدة العملاء'
  const cols: RCol<BalRow>[] = [
    { key: 'code', label: 'الكود', width: 70 },
    { key: 'studentName', label: 'اسم الطالب', width: 210, render: x => <b>{x.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 105 },
    { key: 'studentTel', label: 'الجوال', width: 105 },
    { key: 'branch', label: 'الفرع', width: 150, hidden: true },
    { key: 'opening', label: 'رصيد سابق', width: 110, num: true, tone: 'sign' },
    { key: 'maden', label: 'مستحق خلال الفترة', width: 120, num: true, blankZero: true, tone: 'due' },
    { key: 'daen', label: 'مسدد خلال الفترة', width: 120, num: true, blankZero: true, tone: 'ok' },
    { key: 'balance', label: 'الرصيد', width: 120, num: true, tone: 'sign', render: x => <b className={x.balance > 0 ? 'due' : x.balance < 0 ? 'ok' : 'muted'}>{money(x.balance)}</b> },
    { key: 'acadmyId', label: 'الرقم الأكاديمي', width: 95, hidden: true },
    { key: 'actions', label: '', width: 50, noFilter: true, foot: false, value: () => '', render: x => can('studentstatment', 'find')
      ? <span className="rg-actions rg-icons" onClick={e => e.stopPropagation()}><button className="btn ghost sm icon" title="كشف حساب الطالب" onClick={() => setStmt({ account: x.accountGuid, name: x.studentName })}><Wallet size={15} /></button></span> : null },
  ]
  const due = all.filter(x => x.balance > 0.009)
  return (
    <>
      <PageHead title={title} subtitle={follow ? 'أرصدة الطلاب المستحقة حسب الفرع والدبلوم والدفعة' : 'أرصدة العملاء الذين لهم حركة خلال الفترة'} group={group} />
      <Bar onRun={() => setParams({ from, to, branch: branch || null, diplom: diplom || null, batch: batch || null, q: q.trim() || null })} loading={r.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        {follow && <BranchSelect value={branch} onChange={setBranch} />}
        {follow && <Select label="الدبلوم / الدورة" value={diplom} onChange={setDiplom} list={[...(lk.data?.diploms ?? []), ...(lk.data?.courses ?? [])]} all="الكل" w={200} />}
        {follow && <Select label="الدفعة" value={batch} onChange={setBatch} list={lk.data?.batches} all="كل الدفعات" w={130} />}
        <SearchBox value={q} onChange={setQ} placeholder="الاسم أو الهوية أو الجوال" />
      </Bar>
      {!r.data ? <Waiting loading={r.isFetching} error={r.error?.message} /> : <>
        <Summary items={[
          { label: 'عدد الطلاب', value: count(all.length), tone: 'brand' },
          { label: 'عليهم مستحقات', value: count(due.length), tone: 'due' },
          { label: 'إجمالي المستحقات', value: money(due.reduce((s, x) => s + x.balance, 0)), tone: 'due' },
          { label: 'المسدد خلال الفترة', value: money(all.reduce((s, x) => s + x.daen, 0)), tone: 'ok' },
          { label: 'أرصدة دائنة (دفعات مقدمة)', value: money(-all.filter(x => x.balance < 0).reduce((s, x) => s + x.balance, 0)) },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={x => x.accountGuid} footLabel={`${rows.length} طالب`}
          toolbar={<>
            <Segmented value={show} onChange={setShow} options={[{ value: 'due', label: 'عليهم مستحقات' }, { value: 'credit', label: 'دائنة' }, { value: 'zero', label: 'مسددة' }, { value: 'all', label: 'الكل' }]} />
            <Exports title={title} sub={sub(params)} cols={cols} rows={rows} />
          </>} />
      </>}
      <AnimatePresence>{stmt && <StudentStatement account={stmt.account} name={stmt.name} onClose={() => setStmt(null)} />}</AnimatePresence>
    </>
  )
}

/* ======================= تحليل المصروفات ======================= */
interface ExpRow { date: string; code: string; accountCode: string | null; account: string; maden: number; daen: number; balance: number; notes: string | null; vendor: string | null; vendorTax: string | null; vendorTel: string | null; costCenter: string | null }
interface ExpData { rows: ExpRow[]; byAccount: { code: string | null; account: string; amount: number; count: number }[]; total: number; vouchers: number }

export function ExpenseAnalysis({ group }: { group: string }) {
  const lk = useLookups()
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [cash, setCash] = useState(''); const [cost, setCost] = useState<{ guid: string; name: string } | null>(null)
  const [params, setParams] = useState<Params | null>(null)
  const [view, setView] = useState('accounts')
  const r = useReport<ExpData>('expenses', params)
  const d = r.data
  type Acc = ExpData['byAccount'][number]
  const accCols: RCol<Acc>[] = [
    { key: 'code', label: 'الكود', width: 90 },
    { key: 'account', label: 'بند المصروف', width: 260, render: x => <b>{x.account}</b> },
    { key: 'count', label: 'عدد السندات', width: 100, num: true, int: true },
    { key: 'amount', label: 'المبلغ', width: 140, num: true, tone: 'due' },
    { key: 'pct', label: 'النسبة', width: 220, noFilter: true, foot: '100%', value: x => (d?.total ? Math.round(x.amount / d.total * 1000) / 10 : 0),
      render: x => { const p = d?.total ? x.amount / d.total * 100 : 0; return <span className="perf"><i style={{ width: `${p}%` }} /><em>{p.toFixed(1)}%</em></span> } },
  ]
  const cols: RCol<ExpRow>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: x => day(x.date) },
    { key: 'code', label: 'رقم السند', width: 80 },
    { key: 'account', label: 'بند المصروف', width: 200 },
    { key: 'maden', label: 'مدين', width: 110, num: true, blankZero: true, tone: 'due' },
    { key: 'daen', label: 'دائن', width: 100, num: true, blankZero: true, tone: 'ok' },
    { key: 'balance', label: 'الرصيد التراكمي', width: 120, num: true, foot: d ? money(d.total) : '' },
    { key: 'notes', label: 'البيان', width: 240 },
    { key: 'vendor', label: 'المورد', width: 150 },
    { key: 'vendorTax', label: 'الرقم الضريبي للمورد', width: 130, hidden: true },
    { key: 'vendorTel', label: 'جوال المورد', width: 110, hidden: true },
    { key: 'costCenter', label: 'مركز التكلفة', width: 150 },
  ]
  const cashName = lk.data?.cashBoxes.find(c => c.guid === params?.cash)?.name ?? ''
  const toolbarView = <Segmented value={view} onChange={setView} options={[{ value: 'accounts', label: 'حسب البند' }, { value: 'detail', label: 'تفاصيل السندات' }]} />
  return (
    <>
      <PageHead title="تحليل المصروفات" subtitle="مصروفات سندات الصرف من خزينة أو بنك موزعة على بنود المصروفات" group={group} />
      <Bar onRun={() => setParams({ from, to, cash: cash || null, cost: cost?.guid ?? null })} loading={r.isFetching} disabled={!cash}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <F label="الخزينة / البنك" w={210}><select className="input" value={cash} onChange={e => setCash(e.target.value)}>
          <option value="">— اختر —</option>
          <optgroup label="الخزائن">{lk.data?.cashBoxes.filter(c => c.type !== 1).map(c => <option key={c.guid} value={c.guid}>{c.name}</option>)}</optgroup>
          <optgroup label="البنوك">{lk.data?.cashBoxes.filter(c => c.type === 1).map(c => <option key={c.guid} value={c.guid}>{c.name}</option>)}</optgroup>
        </select></F>
        <CostSelect value={cost} onChange={setCost} />
      </Bar>
      {!d ? <Waiting loading={r.isFetching} error={r.error?.message} text={cash ? 'اضغط "عرض"' : 'اختر الخزينة أو البنك ثم اضغط "عرض"'} /> : <>
        <Summary items={[
          { label: 'إجمالي المصروفات', value: money(d.total), tone: 'due' },
          { label: 'عدد السندات', value: count(d.vouchers), tone: 'brand' },
          { label: 'عدد البنود', value: count(d.byAccount.length) },
          { label: 'أكبر بند', value: d.byAccount[0] ? `${d.byAccount[0].account}` : '—' },
        ]} />
        {view === 'accounts'
          ? <ReportGrid columns={accCols} rows={d.byAccount} rowKey={x => `${x.code}-${x.account}`} footLabel={`${d.byAccount.length} بند`} emptyText="لا توجد مصروفات في الفترة"
              toolbar={<>{toolbarView}<Exports title="تحليل المصروفات" sub={`${cashName} ${sub(params)}`} cols={accCols} rows={d.byAccount} /></>} />
          : <ReportGrid columns={cols} rows={d.rows} rowKey={(x, i) => `${x.code}-${i}`} footLabel={`${d.rows.length} سطر`} emptyText="لا توجد مصروفات في الفترة"
              toolbar={<>{toolbarView}<Exports title="تحليل المصروفات - تفاصيل" sub={`${cashName} ${sub(params)}`} cols={cols} rows={d.rows} /></>} />}
      </>}
    </>
  )
}

