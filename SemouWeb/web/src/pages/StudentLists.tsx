import { useEffect, useMemo, useState } from 'react'
import { keepPreviousData, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { ArrowUpCircle, BookOpenText, Flag, GraduationCap, NotebookPen, Search, Shuffle, Tags, Wallet } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { exportCsv, printReport, type ReportCol } from '../lib/print'
import { PageHead } from '../components/Shell'
import { ReportGrid, type RCol } from '../components/ReportGrid'
import { Segmented, Spinner } from '../components/ui'
import { StudentStatement } from './Statement'
import { StudyInfo } from './ReceptionActions'

/* طلاب الدبلومات (LevelReportForm) / طلاب الدورات (StudentCourseList): one row per training file. */
type Kind = 'diploma' | 'course'
interface Row {
  FileId: number; StudentGuid: string; AccountGuid: string; AcadmyId: string | null; StudentName: string; NationalId: string; StudentTel: string
  DiplomName: string | null; CourseCode: string | null; DateStart: string | null; DateEnd: string | null; BatchName: string | null; LevelName: string | null
  CaseName: string | null; Notes: string | null; Mark: number; Active: boolean; ClassName: string | null; RegBranch: string | null; StudyBranch: string | null
  StudentActive: boolean; Balance: number
}
interface Opt { guid: string; name: string; code?: string }
interface Lookups { diploms: Opt[]; courses: Opt[]; cases: Opt[]; batches: Opt[]; levels: Opt[] }
const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
/** Course dates are stored as Hijri text (dd/MM/yyyy); ISO dates are formatted. */
const day = (v: string | null) => (!v ? '' : /^\d{4}-\d{2}-\d{2}/.test(v) ? new Date(v).toLocaleDateString('en-GB') : v)
const esc = (t: string) => t.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))
const options = (list: Opt[], empty = '— اختر —') => `<option value="">${empty}</option>${list.map(o => `<option value="${o.guid}">${esc(o.name)}</option>`).join('')}`

export const DiplomaStudents = ({ group }: { group: string }) => <StudentList kind="diploma" group={group} />
export const CourseStudents = ({ group }: { group: string }) => <StudentList kind="course" group={group} />

function StudentList({ kind, group }: { kind: Kind; group: string }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [branch, setBranch] = useState('')
  const [f, setF] = useState({ diplom: '', course: '', case: '', batch: '', level: '', active: 'true', q: '' })
  const [params, setParams] = useState<Record<string, string | null> | null>(null)
  const [sel, setSel] = useState<Set<string>>(new Set())
  const [statement, setStatement] = useState<Row | null>(null)
  const [study, setStudy] = useState<number | null>(null)
  const isCourse = kind === 'course'
  const form = isCourse ? 'coursestudent' : 'diplomstudent'

  useEffect(() => { if (!branch && branches.data?.length) setBranch(branches.data.find(b => b.isUse)?.guid ?? '') }, [branches.data, branch])
  const lk = useQuery({ queryKey: ['student-lists', kind, 'lookups', branch], enabled: !!branch, staleTime: 5 * 60_000, queryFn: () => api<Lookups>(`/students/${kind}/lookups${qs({ branch })}`) })
  const list = useQuery({ queryKey: ['student-lists', kind, params], enabled: !!params, placeholderData: keepPreviousData, queryFn: () => api<Row[]>(`/students/${kind}${qs(params!)}`) })
  const run = () => { setSel(new Set()); setParams({ branch, diplom: f.diplom || null, course: f.course || null, case: f.case || null, batch: f.batch || null, level: f.level || null, active: f.active || null, q: f.q.trim() || null }) }
  useEffect(() => { if (branch && !params) run() }, [branch]) // eslint-disable-line react-hooks/exhaustive-deps
  const rows = list.data ?? []
  const ids = useMemo(() => rows.filter(r => sel.has(String(r.FileId))).map(r => r.FileId), [rows, sel])
  const refresh = () => { qc.invalidateQueries({ queryKey: ['student-lists', kind] }); setSel(new Set()) }

  const post = async (path: string, body: object) => {
    try { const r = await api<{ message: string }>(`/students/${kind}/${path}`, { method: 'POST', json: body }); notify.success(r.message); refresh() }
    catch (e) { alerts.error((e as Error).message) }
  }
  /* ---- bulk actions (on the ticked students) ---- */
  const bulkLevel = async () => {
    const v = await alerts.form<{ levelGuid: string }>({ title: `ترحيل ${ids.length} طالب إلى مستوى`, confirmText: 'ترحيل',
      html: `<div class="sm-form"><label class="sm-lbl">المستوى الجديد</label><select id="lv" class="input">${options(lk.data?.levels ?? [])}</select></div>`,
      read: el => { const g = (el.querySelector('#lv') as HTMLSelectElement).value; return g ? { levelGuid: g } : 'يرجى اختيار المستوى المراد الترحيل إليه' } })
    if (v) post('level', { fileIds: ids, ...v })
  }
  const bulkCase = async () => {
    const v = await alerts.form<{ caseGuid: string; reason: string }>({ title: `تغيير حالة ${ids.length} طالب`, confirmText: 'حفظ',
      html: `<div class="sm-form"><label class="sm-lbl">الحالة الجديدة</label><select id="cs" class="input">${options(lk.data?.cases ?? [])}</select>
        <label class="sm-lbl">السبب</label><textarea id="rs" class="input" maxlength="250"></textarea></div>`,
      read: el => { const g = (el.querySelector('#cs') as HTMLSelectElement).value, r = (el.querySelector('#rs') as HTMLTextAreaElement).value.trim(); return !g ? 'يرجى اختيار حالة التسجيل' : !r ? 'يرجى إدخال سبب تغيير الحالة' : { caseGuid: g, reason: r } } })
    if (v) post('case', { fileIds: ids, ...v })
  }
  const bulkTransfer = async () => {
    const bs = (branches.data ?? []).filter(b => b.isUse).map(b => ({ guid: b.guid, name: b.name }))
    const v = await alerts.form<{ studyBranchGuid: string | null; regBranchGuid: string | null; reason: string }>({ title: `نقل ${ids.length} طالب`, confirmText: 'نقل',
      html: `<div class="sm-form"><label class="sm-lbl">فرع الدراسة الجديد</label><select id="sb" class="input">${options(bs, 'بدون تغيير')}</select>
        <label class="sm-lbl">فرع التسجيل الجديد</label><select id="rb" class="input">${options(bs, 'بدون تغيير')}</select>
        <label class="sm-lbl">السبب</label><textarea id="rs" class="input" maxlength="250"></textarea></div>`,
      read: el => {
        const sb = (el.querySelector('#sb') as HTMLSelectElement).value, rb = (el.querySelector('#rb') as HTMLSelectElement).value, r = (el.querySelector('#rs') as HTMLTextAreaElement).value.trim()
        return !sb && !rb ? 'يرجى اختيار الفرع المراد النقل إليه' : !r ? 'يرجى إدخال سبب النقل' : { studyBranchGuid: sb || null, regBranchGuid: rb || null, reason: r }
      } })
    if (v) post('transfer', { fileIds: ids, ...v })
  }
  /* ---- one student ---- */
  const notes = async (r: Row) => {
    const v = await alerts.form<{ notes: string }>({ title: `ملاحظات: ${r.StudentName}`, confirmText: 'حفظ',
      html: `<div class="sm-form"><textarea id="nt" class="input" rows="5" maxlength="1000">${esc(r.Notes ?? '')}</textarea></div>`,
      read: el => ({ notes: (el.querySelector('#nt') as HTMLTextAreaElement).value }) })
    if (v) post('notes', { fileId: r.FileId, ...v })
  }
  const finish = async (r: Row) => {
    if (!can('finshstudy', 'add')) { alerts.error('لا تملك صلاحية إنهاء الدراسة'); return }
    const v = await alerts.form<{ caseGuid: string; mark: number; notes: string }>({ title: `إنهاء دراسة: ${r.StudentName}`, confirmText: 'إنهاء الدراسة',
      html: `<div class="sm-form"><label class="sm-lbl">الحالة بعد الإنهاء</label><select id="cs" class="input">${options(lk.data?.cases ?? [])}</select>
        <label class="sm-lbl">نسبة الاختبار %</label><input id="mk" class="input" dir="ltr" inputmode="decimal" placeholder="0 - 100">
        <label class="sm-lbl">ملاحظات</label><textarea id="nt" class="input" maxlength="1000">${esc(r.Notes ?? '')}</textarea></div>`,
      read: el => {
        const g = (el.querySelector('#cs') as HTMLSelectElement).value, m = (el.querySelector('#mk') as HTMLInputElement).value.trim()
        if (!g) return 'يرجى اختيار حالة التسجيل'
        if (m === '' || isNaN(Number(m)) || Number(m) < 0 || Number(m) > 100) return 'يرجى تحديد نسبة الاختبار الخاصة بالطالب (من 0 إلى 100)'
        return { caseGuid: g, mark: Number(m), notes: (el.querySelector('#nt') as HTMLTextAreaElement).value }
      } })
    if (v) post('finish', { fileId: r.FileId, ...v })
  }

  const cols: RCol<Row>[] = [
    { key: 'AcadmyId', label: 'الرقم الأكاديمي', width: 100 },
    { key: 'StudentName', label: 'اسم الطالب', width: 210, render: r => <b className={r.StudentActive ? '' : 'muted'}>{r.StudentName}</b> },
    { key: 'NationalId', label: 'رقم الهوية', width: 110 },
    { key: 'StudentTel', label: 'الجوال', width: 110 },
    { key: 'DiplomName', label: isCourse ? 'الدورة' : 'الدبلوم', width: 200 },
    ...(isCourse
      ? [{ key: 'CourseCode', label: 'كود الدورة', width: 110 }, { key: 'DateStart', label: 'البداية', width: 95, value: (r: Row) => day(r.DateStart) }] as RCol<Row>[]
      : [{ key: 'BatchName', label: 'الدفعة', width: 100 }, { key: 'LevelName', label: 'المستوى', width: 95 }, { key: 'ClassName', label: 'الشعبة', width: 90, hidden: true }] as RCol<Row>[]),
    { key: 'CaseName', label: 'الحالة', width: 110 },
    { key: 'Active', label: 'الدراسة', width: 90, value: r => (r.Active ? 'مستمر' : 'أنهى الدراسة'), render: r => <span className={`badge ${r.Active ? 'ok' : 'muted'}`}>{r.Active ? 'مستمر' : 'أنهى الدراسة'}</span> },
    { key: 'Balance', label: 'الرصيد', width: 110, num: true, tone: 'sign', render: r => <span className={r.Balance > 0 ? 'due' : r.Balance < 0 ? 'ok' : ''}>{money(r.Balance)}</span> },
    { key: 'Notes', label: 'ملاحظات', width: 160, hidden: true },
    { key: 'RegBranch', label: 'فرع التسجيل', width: 150, hidden: true },
    { key: 'actions', label: 'الإجراءات', width: 150, noFilter: true, foot: false, value: () => '', render: r => (
      <span className="rg-actions rg-icons" onClick={e => e.stopPropagation()}>
        {can('studentstatment', 'find') && <button className="btn ghost sm icon" title="كشف حساب" onClick={() => setStatement(r)}><Wallet size={15} /></button>}
        <button className="btn ghost sm icon" title="بيانات الدراسة" onClick={() => setStudy(r.FileId)}><BookOpenText size={15} /></button>
        <button className="btn ghost sm icon" title="ملاحظات" onClick={() => notes(r)}><NotebookPen size={15} /></button>
        {r.Active && <button className="btn ghost sm icon" title="إنهاء الدراسة" onClick={() => finish(r)}><Flag size={15} /></button>}
      </span>) },
  ]
  const exportCols = cols.filter(c => c.key !== 'actions')
  const toReport = () => ({
    cols: exportCols.map(c => ({ label: c.label, num: c.num }) as ReportCol),
    rows: rows.map(r => exportCols.map(c => (c.value ? c.value(r) : (r as unknown as Record<string, unknown>)[c.key] as string | number))),
  })
  const bName = branches.data?.find(b => b.guid === branch)?.name ?? ''
  const set = (k: keyof typeof f, v: string) => setF(x => ({ ...x, [k]: v }))
  const sel2 = (k: keyof typeof f, label: string, list: Opt[] | undefined, all: string, w = 170) => (
    <label className="rb-f" style={{ minWidth: w }}><small>{label}</small>
      <select className="input" value={f[k]} onChange={e => set(k, e.target.value)}><option value="">{all}</option>{list?.map(o => <option key={o.guid} value={o.guid}>{o.name}{o.code ? ` (${o.code})` : ''}</option>)}</select></label>
  )
  return (
    <>
      <PageHead title={isCourse ? 'طلاب الدورات' : 'طلاب الدبلومات'} subtitle={isCourse ? 'طلاب الدورات التدريبية في الفرع وحالاتهم وأرصدتهم' : 'طلاب الدبلومات حسب الدبلوم والدفعة والمستوى، مع الترحيل والنقل وإنهاء الدراسة'} group={group} />
      <div className="rb">
        <label className="rb-f" style={{ minWidth: 220 }}><small>الفرع</small>
          <select className="input" value={branch} onChange={e => setBranch(e.target.value)}>{branches.data?.filter(b => b.isUse).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}</select></label>
        {isCourse ? sel2('course', 'الدورة', lk.data?.courses, 'كل الدورات', 240) : sel2('diplom', 'الدبلوم', lk.data?.diploms, 'كل الدبلومات', 220)}
        {!isCourse && sel2('batch', 'الدفعة', lk.data?.batches, 'كل الدفعات', 140)}
        {!isCourse && sel2('level', 'المستوى', lk.data?.levels, 'كل المستويات', 130)}
        {sel2('case', 'الحالة', lk.data?.cases, 'كل الحالات', 140)}
        <label className="rb-f"><small>الدراسة</small><Segmented value={f.active} onChange={v => set('active', v)} options={[{ value: 'true', label: 'مستمر' }, { value: 'false', label: 'أنهى' }, { value: '', label: 'الكل' }]} /></label>
        <label className="rb-f" style={{ minWidth: 200 }}><small>بحث</small><input className="input" placeholder="الاسم أو الهوية أو الجوال" value={f.q} onChange={e => set('q', e.target.value)} onKeyDown={e => e.key === 'Enter' && run()} /></label>
        <button className="btn rb-run" onClick={run} disabled={!branch || list.isFetching}>{list.isFetching ? <Spinner /> : <><Search size={16} /> عرض</>}</button>
      </div>
      {!params || (list.isPending && !list.data) ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div> : <>
        <div className="rs">
          <div className="brand"><small>عدد الطلاب</small><b>{rows.length.toLocaleString('ar-EG')}</b></div>
          <div><small>مستمرون في الدراسة</small><b>{rows.filter(r => r.Active).length.toLocaleString('ar-EG')}</b></div>
          <div className="due"><small>إجمالي المستحق</small><b>{money(rows.reduce((s, r) => s + Math.max(0, r.Balance), 0))}</b></div>
          <div><small>المحدد</small><b>{ids.length.toLocaleString('ar-EG')}</b></div>
        </div>
        {ids.length > 0 && (
          <div className="bulk-bar">
            <span><GraduationCap size={16} /> تم تحديد <b>{ids.length}</b> طالب</span>
            {!isCourse && <button className="btn sm" onClick={bulkLevel}><ArrowUpCircle size={15} /> ترحيل لمستوى</button>}
            {can('changestaut', 'add') && <button className="btn sm secondary" onClick={bulkCase}><Tags size={15} /> تغيير الحالة</button>}
            {can('addstudent', 'edit') && <button className="btn sm secondary" onClick={bulkTransfer}><Shuffle size={15} /> نقل فرع</button>}
            <button className="btn ghost sm" onClick={() => setSel(new Set())}>إلغاء التحديد</button>
          </div>
        )}
        <ReportGrid columns={cols} rows={rows} rowKey={r => String(r.FileId)} selection={{ keys: sel, onChange: setSel }} footLabel={`${rows.length} طالب`}
          rowClass={r => (r.Active ? undefined : 'dim')}
          toolbar={<>
            <button className="btn ghost sm" onClick={() => { const x = toReport(); printReport(isCourse ? 'طلاب الدورات' : 'طلاب الدبلومات', bName, x.cols, x.rows) }}>طباعة</button>
            <button className="btn ghost sm" onClick={() => { const x = toReport(); exportCsv(`${isCourse ? 'طلاب الدورات' : 'طلاب الدبلومات'} ${bName}`, x.cols, x.rows) }}>Excel</button>
          </>} />
      </>}
      <AnimatePresence>
        {statement && <StudentStatement account={statement.AccountGuid} name={statement.StudentName} onClose={() => setStatement(null)} />}
        {study && <StudyInfo fileId={study} onClose={() => setStudy(null)} />}
      </AnimatePresence>
      {!can(form, 'view') && <div className="alert error">لا تملك صلاحية فتح هذه الشاشة</div>}
    </>
  )
}
