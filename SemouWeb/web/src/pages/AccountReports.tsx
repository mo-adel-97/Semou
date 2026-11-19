import { useMemo, useState } from 'react'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { TrendingDown, TrendingUp } from 'lucide-react'
import { api, qs } from '../api'
import { useBranches } from '../lib/lookups'
import { PageHead } from '../components/Shell'
import { AccountPicker } from '../components/AccountPicker'
import { ReportGrid, type RCol } from '../components/ReportGrid'
import { Switch } from '../components/ui'
import { Bar, BranchSelect, CostSelect, Dates, Exports, F, Picker, Summary, Waiting, day, money, monthStart, today, yearStart, type Params } from '../components/ReportKit'

/* تقارير الحسابات العامة: كشف حساب، اليومية العامة، ميزان المراجعة، قائمة الدخل، حركة الخزينة، الأرصدة الافتتاحية. */
interface Lookups { types: { guid: string; name: string }[]; cashBoxes: { guid: string; name: string; type: number }[]; users: { guid: string; name: string }[] }
const useLookups = () => useQuery({ queryKey: ['acc-reports', 'lookups'], queryFn: () => api<Lookups>('/reports/accounts/lookups'), staleTime: 10 * 60_000 })

/* ======================= كشف حساب ======================= */
interface StLine { kind?: 'open'; journal: string; date: string | null; type: string; docNo: string; maden: number; daen: number; balance: number; notes: string; costCenter: string | null; branch: string | null }
interface StatementData { account: { Code: string; AccountName: string }; opening: { maden: number; daen: number; balance: number }; lines: StLine[]; totals: { maden: number; daen: number; balance: number } }

export function AccountStatement({ group }: { group: string }) {
  const [acc, setAcc] = useState<{ guid: string; label: string } | null>(null)
  const [picking, setPicking] = useState(false)
  const [from, setFrom] = useState(yearStart()); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [cost, setCost] = useState<{ guid: string; name: string } | null>(null)
  const [params, setParams] = useState<Params | null>(null)
  const q = useQuery({ queryKey: ['acc-reports', 'statement', params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<StatementData>(`/reports/accounts/statement${qs(params!)}`) })
  const d = q.data
  const rows = useMemo<StLine[]>(() => d ? [{ kind: 'open', journal: '', date: null, type: 'رصيد سابق', docNo: '', maden: d.opening.maden, daen: d.opening.daen, balance: d.opening.balance, notes: `الرصيد حتى ${day(params!.from!)}`, costCenter: null, branch: null }, ...d.lines] : [], [d, params])
  const cols: RCol<StLine>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: r => (r.date ? day(r.date) : '') },
    { key: 'type', label: 'نوع المستند', width: 140 },
    { key: 'docNo', label: 'رقم المستند', width: 100 },
    { key: 'journal', label: 'رقم القيد', width: 90 },
    { key: 'notes', label: 'البيان', width: 360 },
    { key: 'maden', label: 'مدين', width: 130, num: true, blankZero: true, tone: 'due' },
    { key: 'daen', label: 'دائن', width: 130, num: true, blankZero: true, tone: 'ok' },
    { key: 'balance', label: 'الرصيد', width: 140, num: true, tone: 'sign', foot: d ? money(d.totals.balance) : '' },
    { key: 'costCenter', label: 'مركز التكلفة', width: 170 },
    { key: 'branch', label: 'الفرع', width: 170 },
  ]
  return (
    <>
      <PageHead title="كشف حساب" subtitle="حركة الحساب خلال الفترة مع الرصيد السابق والرصيد التراكمي" group={group} />
      <Bar onRun={() => { if (!acc) { setPicking(true); return } setParams({ account: acc.guid, from, to, branch: branch || null, cost: cost?.guid ?? null }) }} loading={q.isFetching}>
        <Picker label="الحساب" value={acc?.label ?? null} placeholder="اختر الحساب..." onOpen={() => setPicking(true)} w={260} />
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <CostSelect value={cost} onChange={setCost} />
      </Bar>
      <AnimatePresence>{picking && <AccountPicker onClose={() => setPicking(false)} onPick={a => { setAcc({ guid: a.Guid, label: `${a.Code} - ${a.Name}` }); setPicking(false) }} />}</AnimatePresence>
      {!d ? <Waiting loading={q.isFetching} error={q.error?.message} text="اختر الحساب والفترة ثم اضغط عرض" /> : <>
        <Summary items={[
          { label: 'الحساب', value: `${d.account.Code} - ${d.account.AccountName}`, tone: 'brand' },
          { label: 'الرصيد السابق', value: money(d.opening.balance) },
          { label: 'حركة مدينة', value: money(d.totals.maden), tone: 'due' },
          { label: 'حركة دائنة', value: money(d.totals.daen), tone: 'ok' },
          { label: 'الرصيد الختامي', value: `${money(Math.abs(d.totals.balance))} ${d.totals.balance >= 0 ? 'مدين' : 'دائن'}` },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={(r, i) => `${r.journal}-${i}`} rowClass={r => (r.kind === 'open' ? 'open' : undefined)}
          toolbar={<Exports title="كشف حساب" sub={`${d.account.AccountName} من ${params!.from} إلى ${params!.to}`} cols={cols} rows={rows} />} />
      </>}
    </>
  )
}

/* ======================= اليومية العامة ======================= */
interface JLine { code: string; date: string; type: string; docNo: string; account: string; maden: number; daen: number; notes: string; costCenter: string | null; band: number }
interface JournalData { entries: { code: string; date: string; type: string; docNo: string; lines: { account: string; maden: number; daen: number; notes: string; costCenter: string | null }[] }[]; totals: { maden: number; daen: number; count: number } }

export function GeneralJournal({ group }: { group: string }) {
  const lk = useLookups()
  const [from, setFrom] = useState(monthStart()); const [to, setTo] = useState(today())
  const [type, setType] = useState(''); const [branch, setBranch] = useState(''); const [cost, setCost] = useState<{ guid: string; name: string } | null>(null)
  const [params, setParams] = useState<Params | null>(null)
  const q = useQuery({ queryKey: ['acc-reports', 'journal', params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<JournalData>(`/reports/accounts/journal${qs(params!)}`) })
  const d = q.data
  // Flat like the desktop grid: one row per journal line; entries shaded alternately.
  const rows = useMemo<JLine[]>(() => d ? d.entries.flatMap((e, i) => e.lines.map(l => ({ code: e.code, date: e.date, type: e.type, docNo: e.docNo, band: i % 2, ...l }))) : [], [d])
  const cols: RCol<JLine>[] = [
    { key: 'code', label: 'رقم القيد', width: 90 },
    { key: 'date', label: 'تاريخ القيد', width: 100, value: r => day(r.date) },
    { key: 'type', label: 'نوع المستند', width: 140 },
    { key: 'docNo', label: 'رقم المستند', width: 100 },
    { key: 'account', label: 'الحساب', width: 230 },
    { key: 'maden', label: 'مدين', width: 120, num: true, blankZero: true, tone: 'due' },
    { key: 'daen', label: 'دائن', width: 120, num: true, blankZero: true, tone: 'ok' },
    { key: 'notes', label: 'البيان', width: 380 },
    { key: 'costCenter', label: 'مركز التكلفة', width: 170 },
  ]
  const diff = d ? d.totals.maden - d.totals.daen : 0
  return (
    <>
      <PageHead title="اليومية العامة" subtitle="كل قيود الفترة بأطرافها" group={group} />
      <Bar onRun={() => setParams({ from, to, type: type || null, branch: branch || null, cost: cost?.guid ?? null })} loading={q.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <F label="نوع المستند" w={180}><select className="input" value={type} onChange={e => setType(e.target.value)}><option value="">كل الأنواع</option>{lk.data?.types.map(t => <option key={t.guid} value={t.guid}>{t.name}</option>)}</select></F>
        <BranchSelect value={branch} onChange={setBranch} />
        <CostSelect value={cost} onChange={setCost} />
      </Bar>
      {!d ? <Waiting loading={q.isFetching} error={q.error?.message} /> : <>
        <Summary items={[
          { label: 'عدد القيود', value: d.totals.count.toLocaleString('ar-EG'), tone: 'brand' },
          { label: 'إجمالي المدين', value: money(d.totals.maden), tone: 'due' },
          { label: 'إجمالي الدائن', value: money(d.totals.daen), tone: 'ok' },
          { label: 'الفرق', value: Math.abs(diff) < 0.01 ? '0 (متزنة)' : money(diff), tone: Math.abs(diff) < 0.01 ? undefined : 'due' },
        ]} />
        <ReportGrid columns={cols} rows={rows} rowKey={(r, i) => `${r.code}-${i}`} rowClass={r => (r.band ? 'band' : undefined)}
          toolbar={<Exports title="اليومية العامة" sub={`من ${params!.from} إلى ${params!.to}`} cols={cols} rows={rows} />} />
      </>}
    </>
  )
}

/* ======================= ميزان المراجعة ======================= */
interface TRow { guid: string; parentGuid: string | null; code: string; name: string; level: number; parent: boolean; openMaden: number; openDaen: number; transMaden: number; transDaen: number; balMaden: number; balDaen: number }
interface TrialData { rows: TRow[]; totals: Omit<TRow, 'guid' | 'parentGuid' | 'code' | 'name' | 'level' | 'parent'> }

export function TrialBalance({ group }: { group: string }) {
  const [from, setFrom] = useState(yearStart()); const [to, setTo] = useState(today()); const [branch, setBranch] = useState('')
  const [hideZero, setHideZero] = useState(true)
  // Tree: only the first level is shown at first; the user opens any account to see the accounts under it.
  const [open, setOpen] = useState<Set<string>>(new Set())
  const [params, setParams] = useState<Params | null>(null)
  const q = useQuery({ queryKey: ['acc-reports', 'trial', params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<TrialData>(`/reports/accounts/trial${qs(params!)}`) })
  const d = q.data
  const all = useMemo(() => (d?.rows ?? []).filter(r => !hideZero || r.openMaden || r.openDaen || r.transMaden || r.transDaen), [d, hideZero])
  const rows = useMemo(() => {
    const visible = new Set<string>()
    return all.filter(r => {
      const show = !r.parentGuid || (open.has(r.parentGuid) && visible.has(r.parentGuid))
      if (show) visible.add(r.guid)
      return show
    })
  }, [all, open])
  const toggle = (r: TRow) => r.parent && setOpen(o => { const n = new Set(o); if (n.has(r.guid)) { n.delete(r.guid); for (const x of all) if (x.level > r.level && n.has(x.guid) && isUnder(x, r.guid)) n.delete(x.guid) } else n.add(r.guid); return n })
  const byGuid = useMemo(() => new Map(all.map(r => [r.guid, r])), [all])
  const isUnder = (x: TRow, g: string) => { let p = x.parentGuid; while (p) { if (p === g) return true; p = byGuid.get(p)?.parentGuid ?? null } return false }
  const openLevel = (lvl: number) => setOpen(new Set(all.filter(r => r.parent && r.level < lvl).map(r => r.guid)))
  const t = d?.totals
  const n = (key: keyof NonNullable<typeof t>, label: string, grp: string, tone?: 'due' | 'ok'): RCol<TRow> => ({ key, label, group: grp, width: 130, num: true, blankZero: true, tone, foot: t ? money(t[key]) : '' })
  const cols: RCol<TRow>[] = [
    { key: 'code', label: 'الكود', width: 110 },
    { key: 'name', label: 'الحساب', width: 320, render: r => (
      <span className={`tb-name ${r.parent ? 'p' : ''}`} style={{ paddingInlineStart: (r.level - 1) * 18 }}>
        {r.parent ? <span className={`tb-caret ${open.has(r.guid) ? 'on' : ''}`}>‹</span> : <span className="tb-dot" />}{r.name}
      </span>) },
    n('openMaden', 'مدين', 'الرصيد الافتتاحي'), n('openDaen', 'دائن', 'الرصيد الافتتاحي'),
    n('transMaden', 'مدين', 'حركة الفترة'), n('transDaen', 'دائن', 'حركة الفترة'),
    n('balMaden', 'مدين', 'الرصيد', 'due'), n('balDaen', 'دائن', 'الرصيد', 'ok'),
  ]
  const exportCols: RCol<TRow>[] = [cols[0], { key: 'name', label: 'الحساب', width: 320, value: r => `${'    '.repeat(r.level - 1)}${r.name}` }, ...cols.slice(2)]
  const diff = t ? t.balMaden - t.balDaen : 0
  const maxLevel = all.reduce((m, r) => Math.max(m, r.level), 1)
  return (
    <>
      <PageHead title="ميزان المراجعة" subtitle="اضغط على أي حساب رئيسي لعرض الحسابات التابعة له. الطباعة والتصدير يشملان ما هو مفتوح فقط." group={group} />
      <Bar onRun={() => { setOpen(new Set()); setParams({ from, to, branch: branch || null }) }} loading={q.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <BranchSelect value={branch} onChange={setBranch} />
        <F label=" " w={190}><Switch label="إخفاء الحسابات الصفرية" checked={hideZero} onChange={setHideZero} /></F>
      </Bar>
      {!d ? <Waiting loading={q.isFetching} error={q.error?.message} /> : <>
        <Summary items={[
          { label: 'إجمالي الرصيد المدين', value: money(t!.balMaden), tone: 'due' },
          { label: 'إجمالي الرصيد الدائن', value: money(t!.balDaen), tone: 'ok' },
          { label: 'التوازن', value: Math.abs(diff) < 0.05 ? 'الميزان متزن' : `فرق ${money(diff)}`, tone: Math.abs(diff) < 0.05 ? 'brand' : 'due' },
        ]} />
        <ReportGrid tree pageSize={0} columns={cols} rows={rows} rowKey={r => r.guid} onRowClick={toggle}
          rowClass={r => [r.parent ? `parent l${Math.min(r.level, 4)}` : '', r.parent ? 'clickable' : ''].join(' ')}
          toolbar={<>
            <button className="btn ghost sm" onClick={() => setOpen(new Set())}>طي الكل</button>
            {Array.from({ length: Math.max(0, maxLevel - 1) }, (_, i) => <button key={i} className="btn ghost sm" onClick={() => openLevel(i + 2)}>فتح حتى المستوى {i + 2}</button>)}
            <Exports title="ميزان المراجعة" sub={`من ${params!.from} إلى ${params!.to}`} cols={exportCols} rows={rows}
              foot={t ? ['', 'الإجمالي', t.openMaden, t.openDaen, t.transMaden, t.transDaen, t.balMaden, t.balDaen] : undefined} />
          </>} />
      </>}
    </>
  )
}

/* ======================= قائمة الدخل ======================= */
interface IRow { code: string; name: string; amount: number }
interface IncomeData { revenues: IRow[]; expenses: IRow[]; totals: { revenues: number; expenses: number; net: number } }

export function IncomeStatement({ group }: { group: string }) {
  const [from, setFrom] = useState(yearStart()); const [to, setTo] = useState(today()); const [cost, setCost] = useState<{ guid: string; name: string } | null>(null)
  const [params, setParams] = useState<Params | null>(null)
  const q = useQuery({ queryKey: ['acc-reports', 'income', params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<IncomeData>(`/reports/accounts/income${qs(params!)}`) })
  const d = q.data
  const colsOf = (total: number): RCol<IRow>[] => [
    { key: 'code', label: 'الكود', width: 120 },
    { key: 'name', label: 'الحساب', width: 280 },
    { key: 'amount', label: 'المبلغ', width: 150, num: true },
    { key: 'pct', label: 'النسبة', width: 90, value: r => (total ? Math.round(r.amount / total * 1000) / 10 : 0), render: r => `${total ? (r.amount / total * 100).toFixed(1) : '0'}%`, foot: '100%' },
  ]
  const all = d ? [...d.revenues.map(r => ({ ...r, side: 'إيرادات' })), ...d.expenses.map(r => ({ ...r, side: 'مصروفات' }))] : []
  return (
    <>
      <PageHead title="قائمة الدخل" subtitle="الإيرادات والمصروفات وصافي الربح أو الخسارة" group={group} />
      <Bar onRun={() => setParams({ from, to, cost: cost?.guid ?? null })} loading={q.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <CostSelect value={cost} onChange={setCost} />
      </Bar>
      {!d ? <Waiting loading={q.isFetching} error={q.error?.message} /> : <>
        <div className={`inc-net ${d.totals.net >= 0 ? 'profit' : 'loss'}`}>
          {d.totals.net >= 0 ? <TrendingUp size={26} /> : <TrendingDown size={26} />}
          <div><small>{d.totals.net >= 0 ? 'صافي الربح' : 'صافي الخسارة'}</small><b>{money(Math.abs(d.totals.net))} <em>ر.س</em></b></div>
          <span>الإيرادات {money(d.totals.revenues)} − المصروفات {money(d.totals.expenses)}</span>
          <div className="inc-actions"><Exports title="قائمة الدخل" sub={`من ${params!.from} إلى ${params!.to}`}
            cols={[{ key: 'side', label: 'البند', width: 0 }, { key: 'code', label: 'الكود', width: 0 }, { key: 'name', label: 'الحساب', width: 0 }, { key: 'amount', label: 'المبلغ', width: 0, num: true }]}
            rows={all} foot={['', '', 'صافي الربح / الخسارة', d.totals.net]} /></div>
        </div>
        <div className="inc-grid">
          <div><h3 className="inc-title rev"><TrendingUp size={18} /> الإيرادات</h3>
            <ReportGrid columns={colsOf(d.totals.revenues)} rows={d.revenues} rowKey={r => r.code} height="calc(100vh - 420px)" emptyText="لا توجد إيرادات" /></div>
          <div><h3 className="inc-title exp"><TrendingDown size={18} /> المصروفات</h3>
            <ReportGrid columns={colsOf(d.totals.expenses)} rows={d.expenses} rowKey={r => r.code} height="calc(100vh - 420px)" emptyText="لا توجد مصروفات" /></div>
        </div>
      </>}
    </>
  )
}

/* ======================= حركة الخزينة - البنك ======================= */
interface CRow { docNo: string; date: string; type: string; maden: number; daen: number; notes: string; student: string | null; costCenter: string | null; branch: string | null; vendor: string | null; reference: string | null; user: string | null; linkType: string | null; linkRef: string | null }

export function CashHistory({ group }: { group: string }) {
  const lk = useLookups()
  const [from, setFrom] = useState(today()); const [to, setTo] = useState(today())
  const [cash, setCash] = useState(''); const [type, setType] = useState(''); const [user, setUser] = useState(''); const [branch, setBranch] = useState('')
  const [cost, setCost] = useState<{ guid: string; name: string } | null>(null)
  const [params, setParams] = useState<Params | null>(null)
  const q = useQuery({ queryKey: ['acc-reports', 'cash', params], enabled: !!params, placeholderData: keepPreviousData,
    queryFn: () => api<{ rows: CRow[]; totals: { in: number; out: number; net: number; count: number } }>(`/reports/accounts/cash${qs(params!)}`) })
  const d = q.data
  const cols: RCol<CRow>[] = [
    { key: 'date', label: 'التاريخ', width: 100, value: r => day(r.date) },
    { key: 'type', label: 'نوع المستند', width: 120 },
    { key: 'docNo', label: 'رقم المستند', width: 100 },
    { key: 'maden', label: 'وارد', width: 120, num: true, blankZero: true, tone: 'ok' },
    { key: 'daen', label: 'منصرف', width: 120, num: true, blankZero: true, tone: 'due' },
    { key: 'student', label: 'الطالب', width: 200 },
    { key: 'vendor', label: 'المورد', width: 160 },
    { key: 'notes', label: 'البيان', width: 340 },
    { key: 'reference', label: 'المرجع', width: 120 },
    { key: 'user', label: 'المحصل', width: 150 },
    { key: 'linkType', label: 'مرتبط بـ', width: 170, value: r => [r.linkType, r.linkRef].filter(Boolean).join(' ') },
    { key: 'costCenter', label: 'مركز التكلفة', width: 170 },
    { key: 'branch', label: 'الفرع', width: 170 },
  ]
  return (
    <>
      <PageHead title="حركة الخزينة - البنك" subtitle="المقبوضات والمدفوعات على الخزائن والبنوك" group={group} />
      <Bar onRun={() => setParams({ from, to, cash: cash || null, type: type || null, user: user || null, branch: branch || null, cost: cost?.guid ?? null })} loading={q.isFetching}>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <F label="الخزينة / البنك" w={180}><select className="input" value={cash} onChange={e => setCash(e.target.value)}><option value="">الكل</option>{lk.data?.cashBoxes.map(c => <option key={c.guid} value={c.guid}>{c.name}</option>)}</select></F>
        <F label="نوع المستند" w={160}><select className="input" value={type} onChange={e => setType(e.target.value)}><option value="">الكل</option>{lk.data?.types.map(t => <option key={t.guid} value={t.guid}>{t.name}</option>)}</select></F>
        <F label="المحصل" w={160}><select className="input" value={user} onChange={e => setUser(e.target.value)}><option value="">الكل</option>{lk.data?.users.map(u => <option key={u.guid} value={u.guid}>{u.name}</option>)}</select></F>
        <BranchSelect value={branch} onChange={setBranch} />
        <CostSelect value={cost} onChange={setCost} />
      </Bar>
      {!d ? <Waiting loading={q.isFetching} error={q.error?.message} /> : <>
        <Summary items={[
          { label: 'الوارد', value: money(d.totals.in), tone: 'ok' },
          { label: 'المنصرف', value: money(d.totals.out), tone: 'due' },
          { label: 'الصافي', value: money(d.totals.net), tone: 'brand' },
          { label: 'عدد الحركات', value: d.totals.count.toLocaleString('ar-EG') },
        ]} />
        <ReportGrid columns={cols} rows={d.rows} rowKey={(r, i) => `${r.docNo}-${i}`}
          toolbar={<Exports title="حركة الخزينة - البنك" sub={`من ${params!.from} إلى ${params!.to}`} cols={cols} rows={d.rows} />} />
      </>}
    </>
  )
}

/* ======================= الأرصدة الافتتاحية ======================= */
interface ORow { code: string; name: string; maden: number; daen: number; notes: string | null }

export function OpeningBalances({ group }: { group: string }) {
  const branches = useBranches()
  const [branch, setBranch] = useState('')
  const q = useQuery({ queryKey: ['acc-reports', 'opening', branch], enabled: !!branch, queryFn: () => api<{ rows: ORow[]; totals: { maden: number; daen: number } }>(`/reports/accounts/opening${qs({ branch })}`) })
  const d = q.data
  const cols: RCol<ORow>[] = [
    { key: 'code', label: 'الكود', width: 130 },
    { key: 'name', label: 'الحساب', width: 320 },
    { key: 'maden', label: 'مدين', width: 140, num: true, blankZero: true, tone: 'due' },
    { key: 'daen', label: 'دائن', width: 140, num: true, blankZero: true, tone: 'ok' },
    { key: 'notes', label: 'ملاحظات', width: 300 },
  ]
  const bName = branches.data?.find(b => b.guid === branch)?.name ?? ''
  return (
    <>
      <PageHead title="الأرصدة الافتتاحية" subtitle="أرصدة أول المدة لكل فرع (تُعدّل من شجرة الحسابات)" group={group} />
      <Bar onRun={() => q.refetch()} loading={q.isFetching}><BranchSelect value={branch} onChange={setBranch} all="— اختر الفرع —" /></Bar>
      {!branch ? <Waiting loading={false} text="اختر الفرع لعرض أرصدته الافتتاحية" /> : !d ? <Waiting loading /> :
        <ReportGrid columns={cols} rows={d.rows} rowKey={(r, i) => `${r.code}-${i}`} toolbar={<Exports title="الأرصدة الافتتاحية" sub={bName} cols={cols} rows={d.rows} />} />}
    </>
  )
}
