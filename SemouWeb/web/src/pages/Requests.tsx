import { useEffect, useMemo, useState, type ReactNode } from 'react'
import { keepPreviousData, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import {
  AlertTriangle, Ban, CheckCircle2, Clock3, Copy, ExternalLink, FileText, History, ImageOff, Inbox, Pencil, Printer, Receipt,
  RefreshCw, ShieldAlert, ShieldCheck, UploadCloud, UserRound, Wallet, X, XCircle, ZoomIn,
} from 'lucide-react'
import { api, qs, tokenStore } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { printRegistration, printRegOrder, printTaxInvoice } from '../lib/print'
import { PageHead } from '../components/Shell'
import { ReportGrid, type RCol } from '../components/ReportGrid'
import { Dates, F, money } from '../components/ReportKit'
import { Drawer, Segmented, Spinner } from '../components/ui'
import { StudentStatement } from './Statement'

/*
 * طلبات الالتحاق (المبيعات) و طلبات السداد (الحسابات العامة): the review screens of the web workflow.
 * A request holds everything that was entered; approving it runs the same checks as a direct save and creates
 * the documents. Pending requests are always listed (the period only applies to reviewed ones).
 */
type Kind = 'reg' | 'pay'
interface Row {
  guid: string; code: number; orderDate: string; status: number; accountGuid: string; studentGuid: string; studentName: string; nationalId: string | null; studentTel: string | null
  branch: string | null; programs: string | null; regType: number; kind: number; batch: string | null; seller: string | null; subTotal: number; payAmount: number; cash: string | null
  payRef: string | null; payDate: string | null; createdBy: string | null; reviewedBy: string | null; reviewDate: string | null; reviewNotes: string | null
  docCode: string | null; docGuid: string | null; billGuid: string | null; taxNo: string | null; files: number; duplicates: number
}
interface FileInfo { guid: string; code: string; name: string; type: string; size: number }
interface LogItem { action: string; at: string; notes: string | null; user: string | null }
interface StudentInfo {
  info: { guid: string; accountGuid: string; name: string; nationalId: string; tel: string; academyId: string | null; branch: string | null; active: boolean; balance: number; registrations: number }
  files: { program: string | null; batch: string | null; level: string | null; active: boolean; regType: number }[]
}
interface Check { ok: boolean; message: string | null }

const REG_STATUS = [{ v: 0, label: 'بانتظار المراجعة', tone: 'warn' }, { v: 3, label: 'بانتظار تأكيد السداد', tone: 'info' }, { v: 1, label: 'مقبول', tone: 'ok' }, { v: 2, label: 'مرفوض', tone: 'off' }]
const PAY_STATUS = [{ v: 0, label: 'بانتظار المراجعة', tone: 'warn' }, { v: 1, label: 'مؤكد', tone: 'ok' }, { v: 2, label: 'مكرر', tone: 'muted' }, { v: 3, label: 'ملغي', tone: 'off' }]
const statusOf = (k: Kind, s: number) => (k === 'reg' ? REG_STATUS : PAY_STATUS).find(x => x.v === s) ?? { v: s, label: '—', tone: 'muted' }
const Status = ({ k, s }: { k: Kind; s: number }) => { const x = statusOf(k, s); return <span className={`badge ${x.tone}`}>{x.label}</span> }
const dt = (v: string | null) => (v ? new Date(v).toLocaleString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '')
const d = (v: string | null) => (v ? new Date(v).toLocaleDateString('en-GB') : '')
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const daysAgo = (n: number) => new Date(Date.now() - n * 86_400_000 - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
/** "منذ ٣ ساعات" for requests waiting for review. */
function waited(v: string) {
  const m = Math.max(0, Math.round((Date.now() - new Date(v).getTime()) / 60_000))
  if (m < 1) return 'الآن'
  if (m < 60) return `منذ ${m.toLocaleString('ar-EG')} دقيقة`
  const h = Math.round(m / 60); if (h < 24) return `منذ ${h.toLocaleString('ar-EG')} ساعة`
  return `منذ ${Math.round(h / 24).toLocaleString('ar-EG')} يوم`
}
const esc = (t: string) => t.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))

export const AdmissionRequests = ({ group }: { group: string }) => <RequestsPage kind="reg" group={group} />
export const PaymentRequests = ({ group }: { group: string }) => <RequestsPage kind="pay" group={group} />

/* =============================== List =============================== */

function RequestsPage({ kind, group }: { kind: Kind; group: string }) {
  const branches = useBranches()
  const [status, setStatus] = useState<string>('0')
  const [from, setFrom] = useState(daysAgo(30)); const [to, setTo] = useState(today())
  const [branch, setBranch] = useState(''); const [q, setQ] = useState('')
  const [open, setOpen] = useState<string | null>(null)
  const path = kind === 'reg' ? 'registration' : 'payment'
  const params = { from, to, status: status === 'all' ? null : status, branch: branch || null, q: q.trim() || null }
  const list = useQuery({
    queryKey: ['requests', path, params], placeholderData: keepPreviousData, refetchInterval: 30_000,
    queryFn: () => api<{ rows: Row[]; counts: Record<string, number> }>(`/requests/${path}${qs(params)}`),
  })
  const rows = list.data?.rows ?? []
  const c = list.data?.counts ?? {}
  const tabs = kind === 'reg'
    ? [{ value: '0', label: `بانتظار المراجعة (${(c.pending ?? 0).toLocaleString('ar-EG')})` }, { value: '3', label: `بانتظار السداد (${(c.awaiting ?? 0).toLocaleString('ar-EG')})` }, { value: '1', label: `المقبولة (${(c.approved ?? 0).toLocaleString('ar-EG')})` }, { value: '2', label: `المرفوضة (${(c.rejected ?? 0).toLocaleString('ar-EG')})` }, { value: 'all', label: 'الكل' }]
    : [{ value: '0', label: `بانتظار المراجعة (${(c.pending ?? 0).toLocaleString('ar-EG')})` }, { value: '1', label: `المؤكدة (${(c.confirmed ?? 0).toLocaleString('ar-EG')})` }, { value: '2', label: `المكررة (${(c.duplicate ?? 0).toLocaleString('ar-EG')})` }, { value: '3', label: `الملغاة (${(c.cancelled ?? 0).toLocaleString('ar-EG')})` }, { value: 'all', label: 'الكل' }]

  const cols: RCol<Row>[] = [
    { key: 'code', label: 'الطلب', width: 80, render: r => <b className="code-chip">{r.code}</b> },
    { key: 'orderDate', label: 'تاريخ الطلب', width: 120, value: r => dt(r.orderDate), render: r => <span className="rq-date">{dt(r.orderDate)}{r.status === 0 && <small>{waited(r.orderDate)}</small>}</span> },
    { key: 'studentName', label: 'الطالب', width: 200, priority: 2, cardTitle: true, render: r => <span className="rq-stu"><b>{r.studentName}</b><small dir="ltr">{r.nationalId}</small></span> },
    ...(kind === 'reg'
      ? [
        { key: 'regType', label: 'النوع', width: 78, value: (r: Row) => (r.regType === 1 ? 'دورة' : 'دبلوم') },
        { key: 'programs', label: 'البرنامج', width: 180 },
        { key: 'batch', label: 'الدفعة', width: 100 },
        { key: 'seller', label: 'المسوق', width: 115 },
        { key: 'subTotal', label: 'الاستمارة', width: 105, num: true, priority: 3 },
        { key: 'payAmount', label: 'المقدَّم', width: 100, num: true, blankZero: true, tone: 'ok' },
      ] as RCol<Row>[]
      : [
        { key: 'programs', label: 'البند', width: 190, render: (r: Row) => r.kind === 0 ? <span className="rq-ref"><span className="badge warn">دفعة مقدمة</span>{r.programs?.replace(/^دفعة مقدمة — /, '')}</span> : r.programs },
        { key: 'payAmount', label: 'المبلغ', width: 105, num: true, tone: 'ok', priority: 3 },
        { key: 'cash', label: 'الخزينة / البنك', width: 120 },
        { key: 'payRef', label: 'رقم المرجع', width: 110, render: r => <span className="rq-ref">{r.payRef}{r.duplicates > 0 && r.status === 0 && <span className="rq-dup" title="رقم المرجع مستخدم في فاتورة أو طلب آخر"><Copy size={12} /> مكرر؟</span>}</span> },
        { key: 'payDate', label: 'الحوالة', width: 95, value: (r: Row) => d(r.payDate) },
      ] as RCol<Row>[]),
    { key: 'branch', label: 'الفرع', width: 140, hidden: true },
    { key: 'createdBy', label: 'أرسله', width: 115 },
    { key: 'status', label: 'الحالة', width: 120, priority: 1, cardBadge: true, value: r => statusOf(kind, r.status).label, render: r => <Status k={kind} s={r.status} /> },
    { key: 'reviewedBy', label: 'راجعه', width: 120, hidden: status === '0' },
    { key: 'docCode', label: kind === 'reg' ? 'رقم الاستمارة' : 'رقم الفاتورة', width: 95, hidden: status === '0', value: r => (kind === 'pay' ? r.taxNo ?? r.docCode : r.docCode) },
    { key: 'files', label: 'مرفقات', width: 80, num: true, int: true, blankZero: true, foot: false, hidden: true },
  ]
  const pendingRows = rows.filter(r => r.status === 0)
  return (
    <>
      <PageHead title={kind === 'reg' ? 'طلبات الالتحاق' : 'طلبات السداد'}
        subtitle={kind === 'reg' ? 'مراجعة طلبات التسجيل قبل إنشاء استمارات التسجيل وفواتير الدفعات المقدمة' : 'مراجعة الحوالات قبل إصدار الفواتير الضريبية وسندات القبض'} group={group} />
      <div className="rb">
        <F label="الحالة"><Segmented value={status} onChange={setStatus} options={tabs} /></F>
        {status !== '0' && <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />}
        <F label="الفرع" w={180}><select className="input" value={branch} onChange={e => setBranch(e.target.value)}><option value="">كل الفروع</option>{branches.data?.map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}</select></F>
        <F label="بحث" w={220}><input className="input" placeholder={kind === 'reg' ? 'الاسم أو الهوية أو رقم الطلب' : 'الاسم أو الهوية أو رقم المرجع'} value={q} onChange={e => setQ(e.target.value)} /></F>
        <button className="btn ghost rb-run" onClick={() => list.refetch()} disabled={list.isFetching} title="تحديث">{list.isFetching ? <Spinner /> : <><RefreshCw size={16} /> تحديث</>}</button>
      </div>
      {status === '0' && (
        <div className={`rq-hero ${pendingRows.length ? '' : 'empty'}`}>
          {pendingRows.length
            ? <><Inbox size={22} /><div><b>{pendingRows.length.toLocaleString('ar-EG')} طلب بانتظار المراجعة</b>
              <small>أقدمها {waited(pendingRows[pendingRows.length - 1].orderDate)} • الإجمالي {money(pendingRows.reduce((t, r) => t + (kind === 'reg' ? r.subTotal : r.payAmount), 0))} ر.س</small></div>
              <button className="btn sm" onClick={() => setOpen(pendingRows[pendingRows.length - 1].guid)}>مراجعة الأقدم</button></>
            : <><CheckCircle2 size={22} /><div><b>لا توجد طلبات بانتظار المراجعة</b><small>تتحدث القائمة تلقائياً كل ٣٠ ثانية</small></div></>}
        </div>
      )}
      {!list.data ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div>
        : <ReportGrid columns={cols} rows={rows} rowKey={r => r.guid} onRowClick={r => setOpen(r.guid)} footLabel={`${rows.length.toLocaleString('ar-EG')} طلب`}
            rowClass={r => (r.status === 0 ? 'clickable' : r.status === 1 ? 'clickable' : 'clickable dim')}
            emptyText={status === '0' ? 'لا توجد طلبات بانتظار المراجعة' : 'لا توجد طلبات في هذه الفترة'} />}
      <AnimatePresence>
        {open && (kind === 'reg'
          ? <RegReview guid={open} onClose={() => setOpen(null)} />
          : <PayReview guid={open} onClose={() => setOpen(null)} />)}
      </AnimatePresence>
    </>
  )
}

/* =============================== طلباتي =============================== */

type MyRow = Row & { type: Kind }
/** What a request means for whoever sent it: still waiting, done, or turned down. */
const stageOf = (r: MyRow) => (r.status === 0 || (r.type === 'reg' && r.status === 3) ? 'wait' : r.status === 1 ? 'done' : 'no')

/** طلباتي: the enrollment and payment requests the signed-in user sent, and where each one stands. */
export function MyRequests({ group }: { group: string }) {
  const [type, setType] = useState<'all' | Kind>('all')
  const [stage, setStage] = useState<'all' | 'wait' | 'done' | 'no'>('all')
  const [from, setFrom] = useState(daysAgo(30)); const [to, setTo] = useState(today())
  const [q, setQ] = useState('')
  const params = { from, to, q: q.trim() || null }
  const list = useQuery({
    queryKey: ['requests', 'mine', params], placeholderData: keepPreviousData, refetchInterval: 30_000,
    queryFn: () => api<{ reg: Row[]; pay: Row[] }>(`/requests/mine${qs(params)}`),
  })
  const all = useMemo<MyRow[]>(() => [
    ...(list.data?.reg ?? []).map(r => ({ ...r, type: 'reg' as Kind })),
    ...(list.data?.pay ?? []).map(r => ({ ...r, type: 'pay' as Kind })),
  ].sort((a, b) => +new Date(b.orderDate) - +new Date(a.orderDate)), [list.data])
  const ofType = all.filter(r => type === 'all' || r.type === type)
  const rows = ofType.filter(r => stage === 'all' || stageOf(r) === stage)
  const n = (s: string) => ofType.filter(r => stageOf(r) === s).length.toLocaleString('ar-EG')

  const cols: RCol<MyRow>[] = [
    { key: 'type', label: 'نوع الطلب', width: 105, priority: 2, value: r => (r.type === 'reg' ? 'طلب التحاق' : 'طلب سداد'),
      render: r => <span className="rq-ref">{r.type === 'reg' ? <FileText size={14} /> : <Wallet size={14} />} {r.type === 'reg' ? 'طلب التحاق' : 'طلب سداد'}</span> },
    { key: 'code', label: 'رقم الطلب', width: 80, render: r => <b className="code-chip">{r.code}</b> },
    { key: 'orderDate', label: 'تاريخ الطلب', width: 120, value: r => dt(r.orderDate), render: r => <span className="rq-date">{dt(r.orderDate)}{stageOf(r) === 'wait' && <small>{waited(r.orderDate)}</small>}</span> },
    { key: 'studentName', label: 'الطالب', width: 200, priority: 2, cardTitle: true, render: r => <span className="rq-stu"><b>{r.studentName}</b><small dir="ltr">{r.nationalId}</small></span> },
    { key: 'programs', label: 'البرنامج / البند', width: 190 },
    { key: 'payAmount', label: 'المبلغ', width: 105, num: true, tone: 'ok', priority: 3, value: r => (r.type === 'reg' ? r.subTotal : r.payAmount) },
    { key: 'status', label: 'الحالة', width: 140, priority: 1, cardBadge: true, value: r => statusOf(r.type, r.status).label, render: r => <Status k={r.type} s={r.status} /> },
    { key: 'print', label: 'طباعة', width: 70, priority: 1, noFilter: true, value: () => '', render: r => r.type === 'reg' ? <button className="btn ghost sm" title="طباعة طلب الالتحاق" onClick={e => { e.stopPropagation(); printRegOrder(r.guid) }}><Printer size={15} /></button> : null },
    { key: 'reviewedBy', label: 'راجعه', width: 120 },
    { key: 'reviewDate', label: 'تاريخ المراجعة', width: 120, value: r => dt(r.reviewDate) },
    { key: 'reviewNotes', label: 'ملاحظات المراجعة', width: 220 },
    { key: 'docCode', label: 'الاستمارة / الفاتورة', width: 110, value: r => (r.type === 'pay' ? r.taxNo ?? r.docCode : r.docCode) },
    { key: 'branch', label: 'الفرع', width: 140, hidden: true },
  ]
  return (
    <>
      <PageHead title="طلباتي" subtitle="طلبات الالتحاق والسداد التي أرسلتها، وحالة كل طلب أولاً بأول" group={group} />
      <div className="rb">
        <F label="نوع الطلب"><Segmented value={type} onChange={v => setType(v as typeof type)} options={[{ value: 'all', label: 'الكل' }, { value: 'reg', label: 'طلبات الالتحاق' }, { value: 'pay', label: 'طلبات السداد' }]} /></F>
        <F label="الحالة"><Segmented value={stage} onChange={v => setStage(v as typeof stage)} options={[
          { value: 'all', label: 'الكل' }, { value: 'wait', label: `قيد المراجعة (${n('wait')})` }, { value: 'done', label: `المقبولة (${n('done')})` }, { value: 'no', label: `المرفوضة والملغاة (${n('no')})` },
        ]} /></F>
        <Dates from={from} to={to} setFrom={setFrom} setTo={setTo} />
        <F label="بحث" w={220}><input className="input" placeholder="الاسم أو الهوية أو رقم الطلب" value={q} onChange={e => setQ(e.target.value)} /></F>
        <button className="btn ghost rb-run" onClick={() => list.refetch()} disabled={list.isFetching} title="تحديث">{list.isFetching ? <Spinner /> : <><RefreshCw size={16} /> تحديث</>}</button>
      </div>
      {!list.data ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div>
        : <ReportGrid columns={cols} rows={rows} rowKey={r => r.type + r.guid} footLabel={`${rows.length.toLocaleString('ar-EG')} طلب`}
            rowClass={r => (stageOf(r) === 'no' ? 'dim' : '')}
            emptyText="لا توجد طلبات أرسلتها في هذه الفترة" />}
    </>
  )
}

/* =============================== Shared pieces =============================== */

/** A file of the request, fetched with the user's token (img/src cannot send it). */
function useFileUrl(guid: string | null) {
  const [url, setUrl] = useState<string | null>(null)
  const [err, setErr] = useState(false)
  useEffect(() => {
    if (!guid) return
    let alive = true, made: string | null = null
    fetch(`/api/requests/files/${guid}`, { headers: { Authorization: `Bearer ${tokenStore.get()}` } })
      .then(r => (r.ok ? r.blob() : Promise.reject(new Error())))
      .then(b => { if (alive) { made = URL.createObjectURL(b); setUrl(made) } })
      .catch(() => alive && setErr(true))
    return () => { alive = false; if (made) URL.revokeObjectURL(made) }
  }, [guid])
  return { url, err }
}

const FILE_NAMES: Record<string, string> = { ID: 'صورة الهوية', Certifcat: 'الشهادة الدراسية', trans: 'صورة الحوالة', trans2: 'صورة الحوالة الثانية', contract: 'اتفاقية التدريب', Letter: 'خطاب' }

function FileTile({ f, big, onZoom }: { f: FileInfo; big?: boolean; onZoom: (url: string, f: FileInfo) => void }) {
  const { url, err } = useFileUrl(f.guid)
  const isImg = f.type.startsWith('image/')
  return (
    <div className={`rq-file ${big ? 'big' : ''}`}>
      <div className="rq-thumb" onClick={() => url && (isImg ? onZoom(url, f) : window.open(url, '_blank'))}>
        {err ? <ImageOff size={26} /> : !url ? <Spinner /> : isImg ? <img src={url} alt={f.name} /> : <FileText size={34} />}
        {url && <span className="rq-zoom">{isImg ? <ZoomIn size={16} /> : <ExternalLink size={16} />}</span>}
      </div>
      <div className="rq-fname"><b>{FILE_NAMES[f.code] ?? f.code}</b><small title={f.name}>{f.name} • {(f.size / 1024).toLocaleString('ar-EG', { maximumFractionDigits: 0 })} ك.ب</small></div>
    </div>
  )
}

function Lightbox({ url, title, onClose }: { url: string; title: string; onClose: () => void }) {
  const [zoom, setZoom] = useState(1)
  useEffect(() => { const k = (e: KeyboardEvent) => e.key === 'Escape' && onClose(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k) }, [onClose])
  return (
    <motion.div className="rq-lightbox" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose}>
      <div className="rq-lb-bar" onClick={e => e.stopPropagation()}>
        <b>{title}</b>
        <button className="btn ghost sm" onClick={() => setZoom(z => Math.min(4, z + 0.5))}>تكبير</button>
        <button className="btn ghost sm" onClick={() => setZoom(z => Math.max(0.5, z - 0.5))}>تصغير</button>
        <a className="btn ghost sm" href={url} target="_blank" rel="noreferrer"><ExternalLink size={15} /> فتح في نافذة</a>
        <button className="btn ghost sm icon" onClick={onClose} aria-label="إغلاق"><X size={18} /></button>
      </div>
      <div className="rq-lb-img" onClick={e => e.stopPropagation()}><img src={url} alt={title} style={{ transform: `scale(${zoom})` }} /></div>
    </motion.div>
  )
}

const KV = ({ items }: { items: [string, ReactNode][] }) => (
  <div className="rq-kv">{items.filter(([, v]) => v !== null && v !== undefined && v !== '').map(([k, v]) => <div key={k}><span>{k}</span><b>{v}</b></div>)}</div>
)
const Section = ({ title, icon, children, tools }: { title: string; icon: ReactNode; children: ReactNode; tools?: ReactNode }) => (
  <section className="rq-sec"><h4>{icon}{title}{tools && <span className="rq-tools">{tools}</span>}</h4>{children}</section>
)

function StudentCard({ s, onStatement }: { s: StudentInfo | null; onStatement?: () => void }) {
  if (!s) return null
  const i = s.info
  return (
    <Section title="الطالب" icon={<UserRound size={16} />} tools={onStatement && <button className="btn ghost sm" onClick={onStatement}><Wallet size={14} /> كشف الحساب</button>}>
      <div className="rq-student">
        <div className="rq-avatar">{i.name.trim().slice(0, 1)}</div>
        <div><b>{i.name}</b><small dir="ltr">{i.nationalId} • {i.tel}</small>{!i.active && <span className="badge off">موقوف</span>}</div>
        <div className={`rq-bal ${i.balance > 0 ? 'due' : i.balance < 0 ? 'ok' : ''}`}><small>الرصيد الحالي</small><b>{money(i.balance)}</b></div>
      </div>
      <KV items={[['الرقم الأكاديمي', i.academyId], ['فرع الطالب', i.branch], ['استمارات سابقة', i.registrations.toLocaleString('ar-EG')]]} />
      {s.files.length > 0 && <div className="rq-chips">{s.files.map((f, n) => <span key={n} className={`badge ${f.active ? 'ok' : 'muted'}`}>{f.program}{f.batch ? ` • ${f.batch}` : ''}{f.level ? ` • ${f.level}` : ''}</span>)}</div>}
    </Section>
  )
}

function Timeline({ log }: { log: LogItem[] }) {
  return (
    <Section title="سجل الطلب" icon={<History size={16} />}>
      <ol className="rq-log">{log.map((l, n) => (
        <li key={n}><i /><div><b>{l.action}</b><small>{l.user} • {dt(l.at)}</small>{l.notes && <p>{l.notes}</p>}</div></li>
      ))}</ol>
    </Section>
  )
}

function CheckBanner({ check, pending }: { check: Check; pending: boolean }) {
  if (!pending) return null
  return check.ok
    ? <div className="rq-check ok"><ShieldCheck size={18} /><div><b>الطلب جاهز للاعتماد</b><small>تم التحقق الآن من الأسعار والمقاعد والدفتر وبيانات السداد</small></div></div>
    : <div className="rq-check bad"><ShieldAlert size={18} /><div><b>لا يمكن اعتماد الطلب حالياً</b><small>{check.message}</small></div></div>
}

/* =============================== طلب التحاق =============================== */

interface RegDetail {
  order: Row; statusName: string; notes: string | null; contract: boolean
  snapshot: { book: string | null; branchName: string | null; batch: string | null; level: string | null; course: string | null; seller: string | null; plan?: string | null
    items: { name: string; type: number; cost: number; tax: number; total: number }[]; total: number; tax: number; subTotal: number
    hasPayment: boolean; payLines: { name: string; cost: number; tax: number; total: number }[]; paySub: number; cash: string | null; cost: string | null; cash2: string | null; cost2: string | null }
  payRef: string | null; payDate: string | null; payNotes: string | null; split: boolean; ref2: string | null; payDate2: string | null; amount1: number | null; amount2: number | null
  student: StudentInfo | null; files: FileInfo[]; log: LogItem[]; check: Check
  payOrder: { guid: string; code: number; status: number } | null
}

function RegReview({ guid, onClose }: { guid: string; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const q = useQuery({ queryKey: ['requests', 'registration', 'one', guid], queryFn: () => api<RegDetail>(`/requests/registration/${guid}`), staleTime: 0 })
  const [zoom, setZoom] = useState<{ url: string; title: string } | null>(null)
  const [stmt, setStmt] = useState(false)
  const x = q.data
  const pending = x?.order.status === 0
  const refresh = () => { qc.invalidateQueries({ queryKey: ['requests'] }); qc.invalidateQueries({ queryKey: ['reception'] }) }

  const approve = async () => {
    if (!x) return
    const v = await alerts.form<{ notes: string }>({
      title: `قبول طلب الالتحاق رقم ${x.order.code}`, confirmText: x.snapshot.hasPayment ? 'قبول وإرسال الدفعة للحسابات' : 'قبول وإنشاء الاستمارة', icon: 'question',
      html: `<div class="sm-form">${x.snapshot.hasPayment
        ? `<p class="sm-text">تتحوّل الدفعة المقدمة (${esc(money(x.snapshot.paySub))} ر.س) إلى <b>طلب سداد</b> يُراجَع في الحسابات.</p>
           <p class="sm-text">لا تُنشأ استمارة التسجيل إلا بعد تأكيد الحسابات للدفعة، فتصدر حينها الاستمارة والفاتورة وسند القبض معاً باسم ${esc(x.order.createdBy ?? '')}. وإذا ألغت الحسابات الدفعة يُرفض طلب الالتحاق تلقائياً.</p>`
        : `<p class="sm-text">سيتم إنشاء استمارة التسجيل وقيدها باسم ${esc(x.order.createdBy ?? '')}.</p>`}
        <label class="sm-lbl">ملاحظات المراجعة (اختياري)</label><textarea id="nt" class="input" maxlength="500"></textarea></div>`,
      read: el => ({ notes: (el.querySelector('#nt') as HTMLTextAreaElement).value.trim() }),
    })
    if (!v) return
    alerts.loading(x.snapshot.hasPayment ? 'جاري قبول الطلب...' : 'جاري إنشاء الاستمارة...')
    try {
      const r = await api<{ message: string; saved: { regCode: string; regGuid: string } | null; payOrderCode: number | null }>(`/requests/registration/${guid}/approve`, { method: 'POST', json: v })
      refresh()
      if (r.saved) { if (await alerts.confirm({ title: r.message, text: `استمارة التسجيل رقم ${r.saved.regCode}`, confirmText: 'طباعة الاستمارة', cancelText: 'إغلاق', icon: 'success' })) await printRegistration(r.saved.regGuid) }
      else alerts.success('تم قبول طلب الالتحاق', `الدفعة المقدمة حُوّلت إلى طلب السداد رقم ${r.payOrderCode} في الحسابات. تُنشأ استمارة التسجيل والفاتورة وسند القبض عند تأكيده.`)
      q.refetch()
    } catch (e) { alerts.error((e as Error).message, 'تعذّر قبول الطلب') }
  }
  const reject = async () => {
    const reason = await alerts.reason(`سبب رفض طلب الالتحاق رقم ${x?.order.code}`)
    if (!reason) return
    try { const r = await api<{ message: string }>(`/requests/registration/${guid}/reject`, { method: 'POST', json: { notes: reason } }); refresh(); alerts.success(r.message); q.refetch() }
    catch (e) { alerts.error((e as Error).message) }
  }

  return (
    <>
      <Drawer size="xl" onClose={onClose} icon={<Inbox size={22} />}
        title={x ? `طلب التحاق رقم ${x.order.code}` : 'طلب التحاق'} subtitle={x ? `${x.order.studentName} • ${x.snapshot.book ?? ''}` : ''}
        footer={x && <>
          {pending && can('webregorder', 'add') && <button className="btn" onClick={approve} disabled={!x.check.ok} title={x.check.ok ? '' : x.check.message ?? ''}><CheckCircle2 size={17} /> قبول وإنشاء الاستمارة</button>}
          {pending && can('webregorder', 'delete') && <button className="btn danger" onClick={reject}><XCircle size={17} /> رفض الطلب</button>}
          <button className="btn secondary" onClick={() => printRegOrder(x.order.guid)}><Printer size={16} /> طباعة الطلب</button>
          {x.order.status === 1 && x.order.docGuid && <button className="btn secondary" onClick={() => printRegistration(x.order.docGuid!)}><Printer size={16} /> طباعة الاستمارة</button>}
          {x.order.status === 1 && x.order.billGuid && <button className="btn secondary" onClick={() => printTaxInvoice(x.order.billGuid!)}><Receipt size={16} /> طباعة الفاتورة</button>}
          <button className="btn ghost" onClick={onClose}>إغلاق</button>
          <span className="hint"><Status k="reg" s={x.order.status} /></span>
        </>}>
        {!x ? <div className="grid">{Array.from({ length: 6 }).map((_, n) => <div key={n} className="sk" style={{ height: 60 }} />)}</div> : (
          <div className="rq-detail">
            <div className="rq-main">
              <CheckBanner check={x.check} pending={pending} />
              {x.order.status !== 0 && (
                <div className={`rq-check ${x.order.status === 1 ? 'ok' : x.order.status === 3 ? 'info' : 'bad'}`}>{x.order.status === 1 ? <CheckCircle2 size={18} /> : x.order.status === 3 ? <Wallet size={18} /> : <Ban size={18} />}
                  <div><b>{x.order.status === 1 ? `تم القبول — استمارة رقم ${x.order.docCode}${x.order.taxNo ? ` • فاتورة الدفعة المقدمة ${x.order.taxNo}` : ''}`
                    : x.order.status === 3 ? `مقبول من المبيعات — بانتظار تأكيد الحسابات للدفعة المقدمة${x.payOrder ? ` (طلب السداد رقم ${x.payOrder.code})` : ''}، ولم تُنشأ الاستمارة بعد`
                    : 'تم رفض الطلب'}</b>
                    <small>{x.order.reviewedBy} • {dt(x.order.reviewDate)}{x.order.reviewNotes ? ` — ${x.order.reviewNotes}` : ''}</small></div></div>
              )}
              <Section title="بيانات الطلب" icon={<FileText size={16} />}>
                <KV items={[
                  ['نوع التسجيل', x.order.regType === 1 ? 'دورة' : 'دبلوم'], ['دفتر التسجيل', x.snapshot.book], ['فرع الدراسة', x.snapshot.branchName],
                  ['الدفعة', x.snapshot.batch], ['المستوى', x.snapshot.level], ['الدورة', x.snapshot.course], ['باقة التسجيل', x.snapshot.plan ?? null], ['المسوق', x.snapshot.seller],
                  ['اتفاقية التدريب', x.order.regType === 0 ? (x.contract ? 'وقّعها المتدرب' : 'لم يوقّعها بعد') : null],
                  ['أرسله', `${x.order.createdBy ?? ''} • ${dt(x.order.orderDate)}`], ['ملاحظات', x.notes],
                ]} />
              </Section>
              <Section title="البنود والأسعار" icon={<Receipt size={16} />}>
                <table className="rq-table"><thead><tr><th>البند</th><th>النوع</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead>
                  <tbody>{x.snapshot.items.map((i, n) => <tr key={n}><td><b>{i.name}</b></td><td>{i.type === 0 ? (x.order.regType === 1 ? 'دورة' : 'دبلوم') : 'رسوم'}</td><td className="n">{money(i.cost)}</td><td className="n">{money(i.tax)}</td><td className="n"><b>{money(i.total)}</b></td></tr>)}</tbody>
                  <tfoot><tr><td colSpan={2}>إجمالي الاستمارة</td><td className="n">{money(x.snapshot.total)}</td><td className="n">{money(x.snapshot.tax)}</td><td className="n"><b>{money(x.snapshot.subTotal)}</b></td></tr></tfoot></table>
              </Section>
              {x.snapshot.hasPayment && (
                <Section title="الدفعة المقدمة" icon={<Wallet size={16} />}>
                  <div className="rq-amount"><small>المبلغ المدفوع</small><b>{money(x.snapshot.paySub)} <em>ر.س</em></b></div>
                  <KV items={[
                    ['الخزينة / البنك', x.snapshot.cash], ['مركز التكلفة', x.snapshot.cost], ['رقم المرجع', x.payRef], ['تاريخ الحوالة', d(x.payDate)],
                    ...(x.split ? [['الحوالة الأولى', money(x.amount1 ?? 0)], ['البنك الثاني', x.snapshot.cash2], ['مركز التكلفة الثاني', x.snapshot.cost2], ['المرجع الثاني', x.ref2], ['تاريخ الحوالة الثانية', d(x.payDate2)], ['الحوالة الثانية', money(x.amount2 ?? 0)]] as [string, ReactNode][] : []),
                    ['ملاحظات السداد', x.payNotes],
                  ]} />
                </Section>
              )}
              <Timeline log={x.log} />
            </div>
            <aside className="rq-side">
              <StudentCard s={x.student} onStatement={can('studentstatment', 'find') ? () => setStmt(true) : undefined} />
              <Section title={`المرفقات (${x.files.length.toLocaleString('ar-EG')})`} icon={<UploadCloud size={16} />}>
                {x.files.length === 0 ? <p className="muted">لا توجد مرفقات محفوظة مع الطلب</p>
                  : <div className="rq-files">{x.files.map(f => <FileTile key={f.guid} f={f} onZoom={(url, ff) => setZoom({ url, title: FILE_NAMES[ff.code] ?? ff.name })} />)}</div>}
              </Section>
            </aside>
          </div>
        )}
      </Drawer>
      <AnimatePresence>
        {zoom && <Lightbox url={zoom.url} title={zoom.title} onClose={() => setZoom(null)} />}
        {stmt && x && <StudentStatement account={x.order.accountGuid} name={x.order.studentName} onClose={() => setStmt(false)} />}
      </AnimatePresence>
    </>
  )
}

/* =============================== طلب سداد =============================== */

interface PayDetail {
  order: Row; statusName: string; filesArchived: boolean; down: { regCode: string; regDocGuid: string } | null
  regOrder: { guid: string; code: number; status: number; regCode: string | null } | null
  snapshot: { book: string | null; program: string | null; lines: { name: string; cost: number; tax: number; total: number }[]; sub: number; tax: number; cash: string | null; cost: string | null; cash2: string | null; cost2: string | null }
  input: { cashAccountGuid: string | null; costCenterGuid: string | null; ref: string | null; payDate: string | null; notes: string | null; split: boolean; amount1: number; amount2: number
    cashAccountGuid2: string | null; costCenterGuid2: string | null; ref2: string | null; payDate2: string | null; kind: number }
  duplicates: { bills: { code: string; taxNo: string | null; date: string; student: string; amount: number; ref: string }[]; orders: { code: number; status: number; date: string; student: string; amount: number }[] }
  lastBills: { taxNo: string | null; code: string; date: string; amount: number; ref: string | null }[]
  student: StudentInfo | null; files: FileInfo[]; log: LogItem[]; check: Check
  cashBoxes: { guid: string; name: string; type: number; costGuid: string | null; costName: string | null }[]
}

function PayReview({ guid, onClose }: { guid: string; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const q = useQuery({ queryKey: ['requests', 'payment', 'one', guid], queryFn: () => api<PayDetail>(`/requests/payment/${guid}`), staleTime: 0 })
  const [zoom, setZoom] = useState<{ url: string; title: string } | null>(null)
  const [stmt, setStmt] = useState(false)
  const x = q.data
  const pending = x?.order.status === 0
  const dups = (x?.duplicates.bills.length ?? 0) + (x?.duplicates.orders.length ?? 0)
  const refresh = () => { qc.invalidateQueries({ queryKey: ['requests'] }); qc.invalidateQueries({ queryKey: ['reception'] }) }
  const transfers = useMemo(() => (x?.files ?? []).filter(f => f.code.startsWith('trans')), [x])

  const confirm = async () => {
    if (!x) return
    const v = await alerts.form<{ notes: string }>({
      title: `تأكيد طلب السداد رقم ${x.order.code}`, confirmText: x.regOrder ? 'تأكيد وإنشاء الاستمارة والفاتورة' : 'تأكيد وإصدار الفاتورة', icon: 'question',
      html: `<div class="sm-form">${x.regOrder ? `<p class="sm-text">ستُنشأ <b>استمارة التسجيل</b> لطلب الالتحاق رقم ${x.regOrder.code} وقيدها، ثم:</p>` : ''}<p class="sm-text">ستصدر فاتورة ضريبية بقيمة ${esc(money(x.snapshot.sub))} ر.س وسند قبض على ${esc(x.snapshot.cash ?? '')}، وتُرسل الفاتورة لطابور الزكاة والدخل، وتُرفع صورة الحوالة إلى الأرشيف.</p>
        ${dups ? `<p class="sm-warn">تنبيه: رقم المرجع مستخدم في ${dups} مستند/طلب آخر. تأكد أنها ليست حوالة مكررة.</p>` : ''}
        <label class="sm-lbl">ملاحظات المراجعة (اختياري)</label><textarea id="nt" class="input" maxlength="500"></textarea></div>`,
      read: el => ({ notes: (el.querySelector('#nt') as HTMLTextAreaElement).value.trim() }),
    })
    if (!v) return
    alerts.loading('جاري إصدار الفاتورة...')
    try {
      const r = await api<{ message: string; billGuid: string; taxNo: string; receiptCode: string; warning: string | null }>(`/requests/payment/${guid}/confirm`, { method: 'POST', json: v })
      refresh()
      if (r.warning) await alerts.warning(r.warning)
      if (await alerts.confirm({ title: r.message, text: `سند القبض ${r.receiptCode}`, confirmText: 'طباعة الفاتورة', cancelText: 'إغلاق', icon: 'success' })) await printTaxInvoice(r.billGuid)
      q.refetch()
    } catch (e) { alerts.error((e as Error).message, 'تعذّر تأكيد الطلب') }
  }
  const setStatus = async (status: 2 | 3) => {
    if (x?.regOrder && !await alerts.confirm({
      title: status === 2 ? 'اعتبار الدفعة المقدمة مكررة؟' : 'إلغاء الدفعة المقدمة؟', icon: 'warning', confirmText: 'متابعة', cancelText: 'تراجع',
      text: `هذه دفعة مقدمة لطلب الالتحاق رقم ${x.regOrder.code}. عند ${status === 2 ? 'اعتبارها مكررة' : 'إلغائها'} يُرفض طلب الالتحاق تلقائياً ولا تُنشأ له استمارة تسجيل.`,
    })) return
    const reason = await alerts.reason(status === 2 ? `سبب اعتبار الطلب رقم ${x?.order.code} مكرراً` : `سبب إلغاء الطلب رقم ${x?.order.code}`)
    if (!reason) return
    try { const r = await api<{ message: string }>(`/requests/payment/${guid}/status`, { method: 'POST', json: { status, reason } }); refresh(); alerts.success(r.message); q.refetch() }
    catch (e) { alerts.error((e as Error).message) }
  }
  const edit = async () => {
    if (!x) return
    const i = x.input
    const opts = (sel: string | null) => x.cashBoxes.map(c => `<option value="${c.guid}" ${c.guid === sel ? 'selected' : ''}>${esc(c.name)}</option>`).join('')
    const v = await alerts.form<Record<string, string | null>>({
      title: `تصحيح بيانات الحوالة — طلب ${x.order.code}`, confirmText: 'حفظ التعديل',
      html: `<div class="sm-form">
        <label class="sm-lbl">الخزينة / البنك</label><select id="cb" class="input">${opts(i.cashAccountGuid)}</select>
        <label class="sm-lbl">رقم المرجع</label><input id="rf" class="input" dir="ltr" value="${esc(i.ref ?? '')}">
        <label class="sm-lbl">تاريخ الحوالة</label><input id="pd" class="input" type="date" value="${(i.payDate ?? '').slice(0, 10)}">
        ${i.split ? `<label class="sm-lbl">البنك الثاني</label><select id="cb2" class="input">${opts(i.cashAccountGuid2)}</select>
        <label class="sm-lbl">رقم المرجع الثاني</label><input id="rf2" class="input" dir="ltr" value="${esc(i.ref2 ?? '')}">
        <label class="sm-lbl">تاريخ الحوالة الثانية</label><input id="pd2" class="input" type="date" value="${(i.payDate2 ?? '').slice(0, 10)}">` : ''}
        <label class="sm-lbl">سبب التعديل</label><textarea id="rs" class="input" maxlength="250"></textarea></div>`,
      read: el => {
        const val = (id: string) => (el.querySelector(`#${id}`) as HTMLInputElement | null)?.value?.trim() ?? null
        if (!val('rf')) return 'يرجى إدخال رقم المرجع'
        if (!val('pd')) return 'يرجى إدخال تاريخ الحوالة'
        if (!val('rs')) return 'يرجى كتابة سبب التعديل'
        const cb = val('cb'), cb2 = val('cb2')
        return {
          cashAccountGuid: cb, costCenterGuid: x.cashBoxes.find(c => c.guid === cb)?.costGuid ?? i.costCenterGuid, ref: val('rf'), payDate: val('pd'),
          cashAccountGuid2: i.split ? cb2 : null, costCenterGuid2: i.split ? x.cashBoxes.find(c => c.guid === cb2)?.costGuid ?? i.costCenterGuid2 : null,
          ref2: i.split ? val('rf2') : null, payDate2: i.split ? val('pd2') : null, reason: val('rs'),
        }
      },
    })
    if (!v) return
    try { const r = await api<{ message: string }>(`/requests/payment/${guid}`, { method: 'PUT', json: v }); refresh(); alerts.success(r.message); q.refetch() }
    catch (e) { alerts.error((e as Error).message, 'تعذّر حفظ التعديل') }
  }
  const retryArchive = async () => {
    try { const r = await api<{ message: string }>(`/requests/payment/${guid}/archive`, { method: 'POST' }); alerts.success(r.message); q.refetch() }
    catch (e) { alerts.error((e as Error).message) }
  }

  return (
    <>
      <Drawer size="xl" onClose={onClose} icon={<Wallet size={22} />}
        title={x ? `طلب سداد رقم ${x.order.code}` : 'طلب سداد'} subtitle={x ? `${x.order.studentName} • ${x.snapshot.book ?? ''}` : ''}
        footer={x && <>
          {pending && can('webpayorder', 'add') && <button className="btn" onClick={confirm} disabled={!x.check.ok} title={x.check.ok ? '' : x.check.message ?? ''}><CheckCircle2 size={17} /> تأكيد وإصدار الفاتورة</button>}
          {pending && can('webpayorder', 'edit') && <button className="btn secondary" onClick={edit}><Pencil size={16} /> تصحيح بيانات الحوالة</button>}
          {pending && can('webpayorder', 'delete') && <button className="btn secondary" onClick={() => setStatus(2)}><Copy size={16} /> مكرر</button>}
          {pending && can('webpayorder', 'delete') && <button className="btn danger" onClick={() => setStatus(3)}><XCircle size={16} /> إلغاء الطلب</button>}
          {x.order.status === 1 && x.order.billGuid && <button className="btn secondary" onClick={() => printTaxInvoice(x.order.billGuid!)}><Printer size={16} /> طباعة الفاتورة</button>}
          {x.order.status === 1 && !x.filesArchived && x.files.length > 0 && can('webpayorder', 'add') && <button className="btn ghost" onClick={retryArchive}><UploadCloud size={16} /> رفع صور الحوالة للأرشيف</button>}
          <button className="btn ghost" onClick={onClose}>إغلاق</button>
          <span className="hint"><Status k="pay" s={x.order.status} /></span>
        </>}>
        {!x ? <div className="grid">{Array.from({ length: 6 }).map((_, n) => <div key={n} className="sk" style={{ height: 60 }} />)}</div> : (
          <div className="rq-detail pay">
            <aside className="rq-side rq-viewer">
              <Section title="صورة الحوالة" icon={<UploadCloud size={16} />}>
                {transfers.length === 0 ? <p className="muted">{x.snapshot.cash === 'صندوق المكتب' ? 'سداد نقدي من صندوق المكتب (لا تلزم صورة)' : 'لا توجد صورة مرفقة'}</p>
                  : <div className="rq-files big">{transfers.map(f => <FileTile key={f.guid} f={f} big onZoom={(url, ff) => setZoom({ url, title: FILE_NAMES[ff.code] ?? ff.name })} />)}</div>}
              </Section>
            </aside>
            <div className="rq-main">
              <CheckBanner check={x.check} pending={pending} />
              {pending && dups > 0 && (
                <div className="rq-check warn"><AlertTriangle size={18} /><div><b>رقم المرجع مستخدم من قبل</b>
                  <small>{[...x.duplicates.bills.map(b => `فاتورة ${b.taxNo ?? b.code} للطالب ${b.student} (${money(b.amount)}) بتاريخ ${d(b.date)}`),
                    ...x.duplicates.orders.map(o => `طلب سداد ${o.code} للطالب ${o.student} (${money(o.amount)}) — ${statusOf('pay', o.status).label}`)].join(' • ')}</small></div></div>
              )}
              {x.order.status !== 0 && (
                <div className={`rq-check ${x.order.status === 1 ? 'ok' : 'bad'}`}>{x.order.status === 1 ? <CheckCircle2 size={18} /> : <Ban size={18} />}
                  <div><b>{x.order.status === 1 ? `تم التأكيد — ${x.regOrder?.regCode ? `استمارة رقم ${x.regOrder.regCode} • ` : ''}فاتورة رقم ${x.order.taxNo}` : x.order.status === 2 ? 'تم تعليم الطلب مكرراً' : 'تم إلغاء الطلب'}{x.order.status !== 1 && x.regOrder ? ` — رُفض طلب الالتحاق رقم ${x.regOrder.code}` : ''}</b>
                    <small>{x.order.reviewedBy} • {dt(x.order.reviewDate)}{x.order.reviewNotes ? ` — ${x.order.reviewNotes}` : ''}{x.order.status === 1 && !x.filesArchived && x.files.length ? ' — صور الحوالة لم تُرفع للأرشيف بعد' : ''}</small></div></div>
              )}
              <Section title="بيانات السداد" icon={<Wallet size={16} />}>
                <div className="rq-amount"><small>{x.down || x.regOrder ? x.snapshot.program : `${x.input.kind === 2 ? 'سداد رسوم' : 'سداد'} — ${x.snapshot.program}`}</small><b>{money(x.snapshot.sub)} <em>ر.س</em></b></div>
                <KV items={[
                  ['الخزينة / البنك', x.snapshot.cash], ['مركز التكلفة', x.snapshot.cost], ['رقم المرجع', <span dir="ltr">{x.input.ref}</span>], ['تاريخ الحوالة', d(x.input.payDate)],
                  ...(x.input.split ? [['الحوالة الأولى', money(x.input.amount1)], ['البنك الثاني', x.snapshot.cash2], ['المرجع الثاني', x.input.ref2], ['تاريخ الحوالة الثانية', d(x.input.payDate2)], ['الحوالة الثانية', money(x.input.amount2)]] as [string, ReactNode][] : []),
                  ['دفتر الفاتورة', x.snapshot.book], ['ملاحظات', x.input.notes], ['أرسله', `${x.order.createdBy ?? ''} • ${dt(x.order.orderDate)}`],
                ]} />
                <table className="rq-table"><thead><tr><th>البند</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead>
                  <tbody>{x.snapshot.lines.map((l, n) => <tr key={n}><td><b>{l.name}</b></td><td className="n">{money(l.cost)}</td><td className="n">{money(l.tax)}</td><td className="n"><b>{money(l.total)}</b></td></tr>)}</tbody></table>
              </Section>
              <StudentCard s={x.student} onStatement={can('studentstatment', 'find') ? () => setStmt(true) : undefined} />
              {x.lastBills.length > 0 && (
                <Section title="آخر فواتير الطالب" icon={<Clock3 size={16} />}>
                  <table className="rq-table"><thead><tr><th>الفاتورة</th><th>التاريخ</th><th>المرجع</th><th>المبلغ</th></tr></thead>
                    <tbody>{x.lastBills.map((b, n) => <tr key={n}><td>{b.taxNo ?? b.code}</td><td>{d(b.date)}</td><td dir="ltr">{b.ref}</td><td className="n">{money(b.amount)}</td></tr>)}</tbody></table>
                </Section>
              )}
              <Timeline log={x.log} />
            </div>
          </div>
        )}
      </Drawer>
      <AnimatePresence>
        {zoom && <Lightbox url={zoom.url} title={zoom.title} onClose={() => setZoom(null)} />}
        {stmt && x && <StudentStatement account={x.order.accountGuid} name={x.order.studentName} onClose={() => setStmt(false)} />}
      </AnimatePresence>
    </>
  )
}
