import { useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import {
  Archive, ArrowRight, BadgeCheck, Building2, ChevronLeft, GraduationCap, Hash, BookOpenText, ClipboardPlus, CreditCard, FileClock, FileSpreadsheet, IdCard, Lock, Phone, Plus, ReceiptText,
  Hourglass, MoreHorizontal, Printer, BadgePercent, Tags, UserCheck, UserX, RotateCcw, Search, ShieldOff, UserCog, UserPlus, UserRound, Users, Wallet, X,
} from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { PageHead } from '../components/Shell'
import { Drawer, Field, Segmented, Spinner, Switch } from '../components/ui'
import { RegistrationWizard, type RegStudent } from './Registration'
import { PayInvoice, type PayTarget } from './PayInvoice'
import { printRegistration } from '../lib/print'
import { StudentStatement } from './Statement'
import { FeeForm, type FeeTarget } from './FeeForm'
import { ReturnWizard, type ReturnStart } from './ReturnForm'
import { DiscountForm, type DiscountTarget } from './DiscountForm'
import { changeCase, DocHistory, pickFile, StudyInfo, toggleActive } from './ReceptionActions'

/* مكتب الاستقبال (MainFindStudent): search current students, fall back to the old archive (S_SSTLI2024). */
type By = 'nid' | 'tel' | 'name'
interface FileRow {
  Code: string; StudentGuid: string; AccountGuid: string; AcadmyId: string; StudentName: string; StudentTel: string; NationalId: string; Email: string
  IsUse: boolean; Locked: boolean; StudentType: number; StudentNational: number; RegBranchName: string; RegBranchGuid: string; RegCaseName: string
  FileId: number | null; DiplomGuid: string | null; DiplomName: string | null; StudyBranchGuid: string | null; StudyBranchName: string | null
  RegDocGuid: string | null; CaseName: string | null; RegType: number | null; LevelActive: boolean; Balance: number
}
interface ArchiveRow { name: string; mobile: string; nationalId: string; branchName: string; accountName: string; oldBranchId: number; custNo: string; balance: number; accepted: boolean }
interface StudentGroup { student: FileRow; files: FileRow[] }

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const initials = (n: string) => n.trim().split(/\s+/).slice(0, 2).map(w => w[0]).join('')
const PLACEHOLDER: Record<By, string> = { nid: 'اكتب رقم الهوية (10 أرقام)', tel: 'اكتب رقم الجوال (10 أرقام)', name: 'اكتب اسم الطالب' }

export function Reception({ group }: { group: string }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [by, setBy] = useState<By>('nid')
  const [text, setText] = useState('')
  const [submitted, setSubmitted] = useState<{ q: string; by: By } | null>(null)
  const [branch, setBranch] = useState('')
  const [card, setCard] = useState<{ guid: string | null; archive?: ArchiveRow; prefill?: Partial<StudentForm> } | null>(null)
  const inputRef = useRef<HTMLInputElement>(null)
  const lookups = useQuery({ queryKey: ['reception', 'lookups'], queryFn: () => api<Lookups>('/reception/lookups'), staleTime: 5 * 60_000 })
  const [regCase, setRegCase] = useState('')
  const [registering, setRegistering] = useState<RegStudent | null>(null)
  const [feeFor, setFeeFor] = useState<FeeTarget | null>(null)
  const [studyFile, setStudyFile] = useState<number | null>(null)
  const [historyOf, setHistoryOf] = useState<{ account: string; name: string } | null>(null)
  const [returning, setReturning] = useState<ReturnStart | null>(null)
  const [discountFor, setDiscountFor] = useState<DiscountTarget | null>(null)
  const refresh = () => qc.invalidateQueries({ queryKey: ['reception'] })

  /** Side actions of the student card (study info, returns, case, suspend, history, discount). */
  const more = async (key: MoreKey, g: StudentGroup) => {
    const s = g.student
    const deny = (form: string, p: 'add' | 'edit', msg = 'لا تملك صلاحية إضافة جديد') => { if (can(form, p)) return false; alerts.error(msg); return true }
    switch (key) {
      case 'study': { const f = await pickFile(g.files); if (f?.FileId) setStudyFile(f.FileId); break }
      case 'history': setHistoryOf({ account: s.AccountGuid, name: s.StudentName }); break
      case 'case': if (!deny('addstudent', 'edit', 'لا تملك صلاحية تعديل البيانات')) changeCase(s.AccountGuid, lookups.data?.cases ?? [], g.files, refresh); break
      case 'active': if (!deny('addstudent', 'edit', 'لا تملك صلاحية تعديل البيانات')) toggleActive(s.AccountGuid, s.StudentName, s.IsUse, refresh); break
      case 'discount': if (!deny('discount', 'add')) setDiscountFor({ studentGuid: s.StudentGuid, studentName: s.StudentName, nationalId: s.NationalId, guid: null }); break
      case 'return':
        if (!can('reregister', 'add') && !can('repay', 'add')) { alerts.error('لا تملك صلاحية عمل المرتجعات'); break }
        setReturning({ account: s.AccountGuid, studentName: s.StudentName, nationalId: s.NationalId }); break
    }
  }
  const [statementOf, setStatementOf] = useState<{ account: string; name: string } | null>(null)
  const [archiveStatementOf, setArchiveStatementOf] = useState<ArchiveRow | null>(null)
  const [paying, setPaying] = useState<PayTarget | null>(null)

  // Desktop behaviour: search fires by itself once the id / mobile reaches 10 digits.
  useEffect(() => {
    const t = text.trim()
    if (by !== 'name' && t.length === 10 && /^\d+$/.test(t)) setSubmitted({ q: t, by })
  }, [text, by])

  const params = submitted ? { q: submitted.q, by: submitted.by, branch: branch || null, regCase: regCase || null } : { branch: branch || null, regCase: regCase || null }
  const enabled = !!submitted || !!branch || !!regCase
  const search = useQuery({ queryKey: ['reception', 'search', params], queryFn: () => api<FileRow[]>(`/reception/search${qs(params)}`), enabled })

  const groups = useMemo<StudentGroup[]>(() => {
    const m = new Map<string, StudentGroup>()
    for (const r of search.data ?? []) {
      if (!m.has(r.StudentGuid)) m.set(r.StudentGuid, { student: r, files: [] })
      if (r.FileId != null) m.get(r.StudentGuid)!.files.push(r)
    }
    return [...m.values()]
  }, [search.data])

  // Not found + exact 10-digit id/mobile → the old archive, like OldDataGrid.
  const archiveQ = submitted && submitted.by !== 'name' && submitted.q.length === 10 && search.isSuccess && groups.length === 0 ? submitted : null
  const archive = useQuery({ queryKey: ['reception', 'archive', archiveQ], queryFn: () => api<ArchiveRow[]>(`/reception/archive${qs({ q: archiveQ!.q, by: archiveQ!.by })}`), enabled: !!archiveQ, staleTime: 60_000 })
  const [archiveByName, setArchiveByName] = useState(false)
  const archiveName = useQuery({
    queryKey: ['reception', 'archive-name', submitted?.q], enabled: archiveByName && submitted?.by === 'name',
    queryFn: () => api<ArchiveRow[]>(`/reception/archive${qs({ q: submitted!.q, by: 'name' })}`), staleTime: 60_000,
  })
  const archiveRows = archive.data ?? (archiveByName ? archiveName.data : undefined)

  const go = () => { const t = text.trim(); if (!t) { setSubmitted(null); return } if (by === 'name' && t.length < 3) { notify.info('اكتب 3 حروف على الأقل من الاسم'); return } setArchiveByName(false); setSubmitted({ q: t, by }) }
  const reset = () => { setText(''); setSubmitted(null); setBranch(''); setRegCase(''); setArchiveByName(false); inputRef.current?.focus() }

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !card && can('addstudent', 'add')) { e.preventDefault(); setCard({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [card, can])

  // نتيجة واحدة: ملف الطالب مباشرة. أكثر من نتيجة: قائمة مختصرة والضغط على الطالب يفتح ملفه.
  const [selected, setSelected] = useState<string | null>(null)
  useEffect(() => { setSelected(null) }, [search.data])
  const shown = groups.length === 1 ? groups[0] : groups.find(g => g.student.StudentGuid === selected) ?? null

  const handlersFor = (g: StudentGroup): Handlers => ({
    onOpen: () => openStudent(g.student),
    onMore: key => more(key, g),
    onFee: () => { if (!can('regfess', 'add')) { alerts.error('لا تملك صلاحية إضافة جديد'); return } setFeeFor({ studentGuid: g.student.StudentGuid, studentName: g.student.StudentName, nationalId: g.student.NationalId, guid: null }) },
    onStatement: () => { if (!can('studentstatment', 'find')) { alerts.error('لا تملك صلاحية عرض كشف حساب الطالب'); return } setStatementOf({ account: g.student.AccountGuid, name: g.student.StudentName }) },
    onPay: (kind, fileId) => { if (!can('salesinvoice', 'add')) { alerts.error('لا تملك صلاحية إضافة جديد'); return } setPaying({ StudentGuid: g.student.StudentGuid, StudentName: g.student.StudentName, NationalId: g.student.NationalId, FileId: fileId, kind }) },
    onRegister: () => { if (!can('addregdoc', 'add')) { alerts.error('لا تملك صلاحية إضافة جديد'); return } setRegistering({ ...g.student, StudyBranchGuid: g.files[0]?.StudyBranchGuid ?? null }) },
  })

  const openStudent = (s: FileRow) => {
    if (!can('addstudent', 'find')) { alerts.error('لا تملك صلاحية استعراض البيانات'); return }
    setCard({ guid: s.StudentGuid })
  }

  return (
    <>
      <PageHead title="مكتب الاستقبال" subtitle="ابحث عن الطالب بالهوية أو الجوال أو الاسم، ومن هنا تبدأ كل عمليات الطالب" group={group}
        actions={can('addstudent', 'add') && <button className="btn" onClick={() => setCard({ guid: null })}><UserPlus size={17} /> طالب جديد <kbd>F1</kbd></button>} />

      <motion.div className="card search-hero" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }}>
        <div className="row search-tools">
          <Segmented value={by} onChange={v => { setBy(v); setText(''); setSubmitted(null); inputRef.current?.focus() }} options={[
          { value: 'nid', label: 'رقم الهوية' }, { value: 'tel', label: 'رقم الجوال' }, { value: 'name', label: 'الاسم' },
        ]} />
          <span style={{ flex: 1 }} />
          <select className="input" style={{ width: 'auto', minWidth: 200 }} value={branch} onChange={e => setBranch(e.target.value)}>
            <option value="">كل فروع التسجيل</option>
            {branches.data?.filter(b => b.isUse).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
          </select>
          <select className="input" style={{ width: 'auto', minWidth: 180 }} value={regCase} onChange={e => setRegCase(e.target.value)}>
            <option value="">كل حالات التسجيل</option>
            {lookups.data?.cases.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
          </select>
          {enabled && search.isSuccess && <span className="muted">{groups.length.toLocaleString('ar-EG')} طالب</span>}
        </div>
        <div className="search-box">
          {by === 'nid' ? <IdCard size={22} /> : by === 'tel' ? <Phone size={22} /> : <UserRound size={22} />}
          <input ref={inputRef} autoFocus value={text} placeholder={PLACEHOLDER[by]} dir={by === 'name' ? 'rtl' : 'ltr'} inputMode={by === 'name' ? 'text' : 'numeric'}
            maxLength={by === 'name' ? 60 : 10} onChange={e => setText(by === 'name' ? e.target.value : e.target.value.replace(/\D/g, ''))}
            onKeyDown={e => { if (e.key === 'Enter') go() }} />
          {(search.isFetching || archive.isFetching || archiveName.isFetching) && <Spinner />}
          {text && <button className="btn ghost sm icon" onClick={reset} aria-label="مسح"><X size={16} /></button>}
          <button className="btn" onClick={go}><Search size={16} /> بحث</button>
        </div>
      </motion.div>

      {!enabled && (
        <div className="card soon">
          <div className="ic"><Search size={36} /></div>
          <h2>ابدأ بالبحث عن الطالب</h2>
          <p>بمجرد كتابة 10 أرقام للهوية أو الجوال يتم البحث تلقائياً. وإذا لم يكن الطالب موجوداً فسيتم البحث في أرشيف النظام السابق.</p>
        </div>
      )}

      {enabled && search.isPending && <div className="grid" style={{ marginTop: 18 }}>{[0, 1].map(i => <div key={i} className="sk" style={{ height: 150, borderRadius: 22 }} />)}</div>}

      {enabled && search.isSuccess && groups.length > 0 && (
        <div className="students">
          {shown ? (
            <StudentProfile key={shown.student.StudentGuid} g={shown} h={handlersFor(shown)} total={groups.length} onBack={groups.length > 1 ? () => setSelected(null) : undefined} />
          ) : (
            <motion.div className="card rp-list" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }}>
              <div className="rp-list-head"><Users size={17} /> {groups.length.toLocaleString('ar-EG')} طالب مطابق، اختر الطالب لفتح ملفه</div>
              {groups.slice(0, 60).map(g => <StudentRow key={g.student.StudentGuid} g={g} onSelect={() => setSelected(g.student.StudentGuid)} />)}
              {groups.length > 60 && <div className="alert info" style={{ margin: 12 }}>يتم عرض أول 60 طالب، حدد البحث أكثر للوصول لطالب معين.</div>}
            </motion.div>
          )}
        </div>
      )}

      {/* ---------- Archive ---------- */}
      {enabled && search.isSuccess && groups.length === 0 && (
        <motion.div className="card archive" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }}>
          <div className="archive-head">
            <span className="ic"><Archive size={22} /></span>
            <div>
              <h3>الطالب غير مسجل في النظام الحالي</h3>
              <p className="muted">{archiveQ ? 'جاري البحث في أرشيف النظام القديم (S_SSTLI2024)...' : 'يمكنك إضافته كطالب جديد أو البحث عنه في الأرشيف.'}</p>
            </div>
            <div className="actions">
              {submitted?.by === 'name' && !archiveByName && <button className="btn secondary" onClick={() => setArchiveByName(true)}><Archive size={16} /> بحث في الأرشيف بالاسم</button>}
              {can('addstudent', 'add') && <button className="btn" onClick={() => setCard({ guid: null, prefill: submitted?.by === 'nid' ? { NationalId: submitted.q } : submitted?.by === 'tel' ? { StudentTel: submitted.q } : { StudentName: submitted?.q ?? '' } })}><UserPlus size={16} /> إضافة طالب جديد</button>}
            </div>
          </div>
          {(archive.isFetching || archiveName.isFetching) && <div className="center-fill" style={{ minHeight: 120 }}><Spinner size={26} /><span className="muted">جارٍ احتساب الأرصدة من الأرشيف...</span></div>}
          {archiveRows && archiveRows.length === 0 && <div className="empty"><b>لا يوجد الطالب في الأرشيف القديم</b></div>}
          {archiveRows && archiveRows.length > 0 && (
            <div className="archive-list">
              {archiveRows.map(r => (
                <div key={`${r.custNo}-${r.oldBranchId}`} className="archive-row">
                  <span className="avatar">{initials(r.name ?? '؟')}</span>
                  <div className="who"><b>{r.name}</b><small dir="ltr">{r.nationalId} • {r.mobile}</small></div>
                  <span className="muted">{r.branchName}</span>
                  <span className={`badge ${r.balance > 0 ? 'warn' : r.balance < 0 ? 'ok' : 'muted'}`}>الرصيد: {money(r.balance)}</span>
                  <div className="archive-actions">
                    <button className="btn sm secondary" onClick={() => setArchiveStatementOf(r)}><FileSpreadsheet size={15} /> كشف حساب</button>
                    {r.accepted ? <span className="badge ok"><BadgeCheck size={12} /> تم قبوله</span>
                      : can('addstudent', 'add') && <button className="btn sm" onClick={() => setCard({ guid: null, archive: r })}><ClipboardPlus size={15} /> قبول من الأرشيف</button>}
                  </div>
                </div>
              ))}
            </div>
          )}
        </motion.div>
      )}

      <AnimatePresence>
        {paying && <PayInvoice key={`${paying.StudentGuid}-${paying.kind}`} target={paying} onClose={() => setPaying(null)} />}
        {studyFile && <StudyInfo fileId={studyFile} onClose={() => setStudyFile(null)} />}
        {historyOf && <DocHistory account={historyOf.account} name={historyOf.name} onClose={() => setHistoryOf(null)} />}
        {returning && <ReturnWizard start={returning} onClose={() => setReturning(null)} />}
        {discountFor && <DiscountForm target={discountFor} onClose={() => setDiscountFor(null)} />}
        {feeFor && <FeeForm target={feeFor} onClose={() => setFeeFor(null)} />}
        {statementOf && <StudentStatement account={statementOf.account} name={statementOf.name} onClose={() => setStatementOf(null)} />}
        {archiveStatementOf && <ArchiveStatement row={archiveStatementOf} onClose={() => setArchiveStatementOf(null)} />}
        {registering && <RegistrationWizard key={registering.StudentGuid} student={registering} onClose={() => setRegistering(null)} />}
      </AnimatePresence>
      <AnimatePresence>
        {card && <StudentCard key={card.guid ?? card.archive?.custNo ?? 'new'} guid={card.guid} archive={card.archive} prefill={card.prefill} lookups={lookups.data}
          onClose={() => setCard(null)} onSaved={nid => { setBy('nid'); setText(nid); setSubmitted({ q: nid, by: 'nid' }) }} />}
      </AnimatePresence>
    </>
  )
}


type MoreKey = 'study' | 'history' | 'case' | 'active' | 'discount' | 'return'
interface Handlers { onOpen: () => void; onRegister: () => void; onPay: (kind: 1 | 2, fileId: number) => void; onStatement: () => void; onFee: () => void; onMore: (key: MoreKey) => void }
type ActionItem = { icon: typeof Wallet; label: string; onClick: () => void; danger?: boolean }

/** Student operations with the desktop guards, in the same order. */
function useStudentActions(g: StudentGroup, h: Handlers) {
  const s = g.student
  const blocked = !s.IsUse
  const guard = (needFile: boolean, finishedMsg?: string): string | null => {
    if (blocked) return 'الطالب موقوف عن التعامل'
    if (s.Locked) return 'ملف الطالب مغلق، يرجى التواصل مع الإدارة'
    const f = g.files[0]
    if (finishedMsg && f && !f.LevelActive) return finishedMsg
    if (needFile && !f) return 'لا تتوفر أي بيانات تدريبية للطالب، يرجى تسجيل استمارة للطالب أولاً أو طلب قبول التسجيل من مشرف الفرع'
    return null
  }
  const locked = (): boolean => { if (s.Locked) { alerts.error('ملف الطالب مغلق، يرجى التواصل مع الإدارة'); return true } return false }
  // Payments go to a training file: ask which one when the student has several.
  const payOn = async (kind: 1 | 2) => {
    const e = guard(true, kind === 1 ? 'أنهى الطالب الدراسة في هذا الملف التدريبي، ولذلك لا يمكن إجراء أي عملية سداد عليه' : 'أنهى الطالب الدراسة في هذا الملف التدريبي، ولذلك لا يمكن إجراء أي عملية سداد رسوم عليه')
    if (e) { alerts.error(e); return }
    const f = await pickFile(g.files.filter(x => x.LevelActive), 'اختر الملف التدريبي للسداد')
    if (f?.FileId) h.onPay(kind, f.FileId)
  }
  const doRegister = () => { const g0 = guard(false); if (g0) { alerts.error(g0); return } h.onRegister() }
  const register: ActionItem = { icon: ClipboardPlus, label: 'تسجيل استمارة', onClick: doRegister }
  const quick: ActionItem[] = [
    { icon: CreditCard, label: 'سداد', onClick: () => payOn(1) },
    { icon: ReceiptText, label: 'سداد رسوم', onClick: () => payOn(2) },
    { icon: Wallet, label: 'كشف حساب', onClick: h.onStatement },
    { icon: UserCog, label: 'بيانات الطالب', onClick: h.onOpen },
  ]
  const menu: { title: string; items: ActionItem[] }[] = [
    { title: 'المستندات', items: [
      { icon: FileSpreadsheet, label: 'استمارة رسوم', onClick: () => { const e = guard(false, 'أنهى الطالب الدراسة في هذا الملف التدريبي، ولذلك لا يمكن إضافة استمارة رسوم له'); if (e) { alerts.error(e); return } h.onFee() } },
      { icon: BadgePercent, label: 'سند خصم', onClick: () => { const e = guard(false); if (e) { alerts.error(e); return } h.onMore('discount') } },
      { icon: RotateCcw, label: 'مرتجع', onClick: () => { if (!locked()) h.onMore('return') } },
      { icon: FileClock, label: 'سجل المستندات', onClick: () => h.onMore('history') },
    ] },
    { title: 'الدراسة والطالب', items: [
      { icon: BookOpenText, label: 'بيانات الدراسة', onClick: () => { if (locked()) return; if (!g.files.length) { alerts.error('لا تتوفر أي بيانات تدريبية للطالب. إذا كان للطالب استمارة تسجيل فيرجى طلب قبول التسجيل من مشرف الفرع، وإلا فيرجى تسجيل استمارة للطالب أولاً.'); return } h.onMore('study') } },
      { icon: Tags, label: 'تغيير الحالة', onClick: () => { if (!locked()) h.onMore('case') } },
      { icon: blocked ? UserCheck : UserX, label: blocked ? 'تفعيل الطالب' : 'إيقاف الطالب', onClick: () => { if (!locked()) h.onMore('active') }, danger: !blocked },
    ] },
  ]
  return { blocked, register, quick, menu }
}

/* ---------------- كشف حساب الأرشيف (OldStudentStatment) ---------------- */
interface ArchivePay { date: string; docNo: string; maden: number; daen: number; notes: string; ref: string; bank: string }
interface ArchiveReg { date: string; type: string; docNo: string; maden: number; daen: number; notes: string; seller: string }
interface ArchiveStmt {
  payments: ArchivePay[]; registrations: ArchiveReg[]; open: { maden: number; daen: number }
  totals: { paymentsMaden: number; paymentsDaen: number; regMaden: number; regDaen: number; maden: number; daen: number; balance: number }
}

function ArchiveStatement({ row, onClose }: { row: ArchiveRow; onClose: () => void }) {
  const q = useQuery({
    queryKey: ['reception', 'archive-statement', row.custNo, row.oldBranchId], staleTime: 60_000,
    queryFn: () => api<ArchiveStmt>(`/reception/archive/statement${qs({ custNo: row.custNo, oldBranchId: row.oldBranchId })}`),
  })
  const d = q.data
  const amt = (n: number) => (n ? money(n) : '')
  return (
    <Drawer size="xl" onClose={onClose} icon={<Archive size={22} />} title={`كشف حساب الأرشيف - ${row.name}`}
      subtitle={<>{row.branchName} • رقم العميل القديم <span dir="ltr">{row.custNo}</span></>}>
      {q.isLoading && <div className="center-fill" style={{ minHeight: 160 }}><Spinner size={26} /><span className="muted">جارٍ تحميل كشف الحساب من الأرشيف...</span></div>}
      {q.isError && <div className="empty"><b>تعذر تحميل كشف الحساب</b></div>}
      {d && (
        <>
          <div className="stats" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))', gap: 12, marginBottom: 18 }}>
            <div className="card" style={{ padding: 14 }}><small className="muted">رصيد افتتاحي</small><div><b>{money(d.open.maden)}</b> <span className="muted">مدين</span> • <b>{money(d.open.daen)}</b> <span className="muted">دائن</span></div></div>
            <div className="card" style={{ padding: 14 }}><small className="muted">إجمالي المدين</small><div><b className="due">{money(d.totals.maden)}</b></div></div>
            <div className="card" style={{ padding: 14 }}><small className="muted">إجمالي الدائن</small><div><b>{money(d.totals.daen)}</b></div></div>
            <div className={`card ${balanceTone(d.totals.balance)}`} style={{ padding: 14 }}><small className="muted">الرصيد</small><div><b>{money(Math.abs(d.totals.balance))}</b> <span className="muted">{balanceText(d.totals.balance)}</span></div></div>
          </div>

          <h4 style={{ margin: '6px 0 10px' }}>استمارات التسجيل</h4>
          <table className="dt ledger">
            <thead><tr><th>التاريخ</th><th>نوع المستند</th><th>رقم المستند</th><th className="n">مدين</th><th className="n">دائن</th><th>البيان</th><th>مندوب البيع</th></tr></thead>
            <tbody>
              {d.registrations.length === 0 && <tr><td colSpan={7}><div className="empty"><b>لا توجد استمارات</b></div></td></tr>}
              {d.registrations.map((r, i) => (
                <tr key={`r${i}`}><td className="nowrap">{r.date}</td><td>{r.type}</td><td>{r.docNo}</td><td className="n due">{amt(r.maden)}</td><td className="n">{amt(r.daen)}</td><td className="notes">{r.notes}</td><td>{r.seller}</td></tr>
              ))}
              {d.registrations.length > 0 && <tr className="total-row"><td colSpan={3}><b>الإجمالي</b></td><td className="n due"><b>{money(d.totals.regMaden)}</b></td><td className="n"><b>{money(d.totals.regDaen)}</b></td><td colSpan={2} /></tr>}
            </tbody>
          </table>

          <h4 style={{ margin: '22px 0 10px' }}>السندات</h4>
          <table className="dt ledger">
            <thead><tr><th>التاريخ</th><th>رقم المستند</th><th className="n">مدين</th><th className="n">دائن</th><th>البيان</th><th>المرجع</th><th>البنك</th></tr></thead>
            <tbody>
              {d.payments.length === 0 && <tr><td colSpan={7}><div className="empty"><b>لا توجد سندات</b></div></td></tr>}
              {d.payments.map((p, i) => (
                <tr key={`p${i}`}><td className="nowrap">{p.date}</td><td>{p.docNo}</td><td className="n due">{amt(p.maden)}</td><td className="n">{amt(p.daen)}</td><td className="notes">{p.notes}</td><td>{p.ref}</td><td>{p.bank}</td></tr>
              ))}
              {d.payments.length > 0 && <tr className="total-row"><td colSpan={2}><b>الإجمالي</b></td><td className="n due"><b>{money(d.totals.paymentsMaden)}</b></td><td className="n"><b>{money(d.totals.paymentsDaen)}</b></td><td colSpan={3} /></tr>}
            </tbody>
          </table>
        </>
      )}
    </Drawer>
  )
}

const balanceText = (b: number) => (b > 0 ? 'مستحق على الطالب' : b < 0 ? 'رصيد للطالب' : 'الحساب متزن')
const balanceTone = (b: number) => (b > 0 ? 'due' : b < 0 ? 'credit' : '')

/* ---------- Several results: one compact line per student ---------- */
function StudentRow({ g, onSelect }: { g: StudentGroup; onSelect: () => void }) {
  const s = g.student
  return (
    <button className={`rp-item ${s.IsUse ? '' : 'blocked'}`} onClick={onSelect}>
      <span className="avatar">{initials(s.StudentName)}</span>
      <span className="rp-item-who">
        <b>{s.StudentName}{!s.IsUse && <span className="badge off">موقوف</span>}{s.Locked && <span className="badge warn">ملف مغلق</span>}</b>
        <small><span dir="ltr">{s.NationalId}</span> • <span dir="ltr">{s.StudentTel}</span>{s.RegBranchName ? ` • ${s.RegBranchName}` : ''}</small>
      </span>
      <span className="rp-item-files">{g.files.length ? g.files.map(f => f.DiplomName).filter(Boolean).join('، ') : 'بدون ملف تدريبي'}</span>
      <span className={`rp-item-bal ${balanceTone(s.Balance)}`}><b dir="ltr">{money(Math.abs(s.Balance))}</b><small>{balanceText(s.Balance)}</small></span>
      <ChevronLeft size={18} className="rp-item-go" />
    </button>
  )
}

/* ---------- Student profile: identity + actions on the side, details in tabs ---------- */
type TabKey = 'files' | 'reqs' | 'pending'

function StudentProfile({ g, h, total, onBack }: { g: StudentGroup; h: Handlers; total: number; onBack?: () => void }) {
  const s = g.student
  const { blocked, register, quick, menu } = useStudentActions(g, h)
  const [menuOpen, setMenuOpen] = useState(false)
  const reqQ = useQuery({ queryKey: ['reception', 'requests', s.AccountGuid], queryFn: () => api<SentRequest[]>(`/requests/student/${s.AccountGuid}`), staleTime: 20_000, meta: { silent: true } })
  const penQ = useQuery({ queryKey: ['reception', 'pending', s.AccountGuid], queryFn: () => api<PendingReg[]>(`/reception/students/${s.AccountGuid}/pending-regs`), staleTime: 30_000 })
  const reqs = reqQ.data ?? []
  const waiting = reqs.filter(r => r.status === 0 || (r.kind === 'reg' && r.status === 3)).length
  const pend = penQ.data?.length ?? 0
  const [tab, setTab] = useState<TabKey | null>(null)
  const cur: TabKey = tab ?? (pend && !g.files.length ? 'pending' : 'files')
  const tabs: { key: TabKey; label: string; icon: typeof Wallet; count: number; alert?: boolean }[] = [
    { key: 'files', label: 'الملفات التدريبية', icon: BookOpenText, count: g.files.length },
    { key: 'reqs', label: 'طلبات الالتحاق والسداد', icon: CreditCard, count: reqs.length, alert: waiting > 0 },
    ...(pend ? [{ key: 'pending' as TabKey, label: 'بانتظار قبول المشرف', icon: Hourglass, count: pend, alert: true }] : []),
  ]
  const run = (fn: () => void) => () => { setMenuOpen(false); fn() }
  const info: [typeof Wallet, string, ReactNode][] = [
    [IdCard, 'رقم الهوية', <span key="n" dir="ltr">{s.NationalId}</span>],
    [Phone, 'رقم الجوال', <span key="t" dir="ltr">{s.StudentTel}</span>],
    [Hash, 'كود الطالب', <span key="c" className="code-chip">#{s.Code}</span>],
    ...(s.AcadmyId ? [[GraduationCap, 'الكود الأكاديمي', <span key="a" dir="ltr">{s.AcadmyId}</span>] as [typeof Wallet, string, ReactNode]] : []),
    ...(s.RegBranchName ? [[Building2, 'فرع التسجيل', s.RegBranchName] as [typeof Wallet, string, ReactNode]] : []),
  ]

  return (
    <motion.div className="rp" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.25 }}>
      {onBack && <button className="btn ghost sm rp-back" onClick={onBack}><ArrowRight size={16} /> كل النتائج ({total.toLocaleString('ar-EG')})</button>}

      <aside className={`card rp-side ${blocked ? 'blocked' : ''}`}>
        <div className="rp-id">
          <span className="avatar rp-avatar">{initials(s.StudentName)}</span>
          <div className="rp-id-txt">
            <button className="rp-name" onClick={h.onOpen} title="فتح بيانات الطالب">{s.StudentName}</button>
            <div className="rp-badges">
              {blocked ? <span className="badge off"><ShieldOff size={12} /> موقوف</span> : <span className="badge ok">فعال</span>}
              {s.Locked && <span className="badge warn"><Lock size={12} /> ملف مغلق</span>}
              {s.RegCaseName && <span className="badge muted">{s.RegCaseName}</span>}
            </div>
          </div>
        </div>

        <dl className="rp-info">
          {info.map(([Ic, label, val]) => <div key={label}><dt><Ic size={14} /> {label}</dt><dd>{val}</dd></div>)}
        </dl>

        <div className={`rp-balance ${balanceTone(s.Balance)}`}>
          <small>الرصيد</small>
          <b dir="ltr">{money(Math.abs(s.Balance))}</b>
          <span>{balanceText(s.Balance)}</span>
        </div>

        <div className="rp-actions">
          <button className="btn rp-main" onClick={register.onClick}><register.icon size={17} /> {register.label}</button>
          <div className="rp-quick">
            {quick.map(a => <button key={a.label} className="btn secondary sm" onClick={a.onClick}><a.icon size={15} /> {a.label}</button>)}
          </div>
          <div className="pop-anchor">
            <button className={`btn ghost sm rp-more ${menuOpen ? 'on' : ''}`} onClick={() => setMenuOpen(m => !m)} aria-expanded={menuOpen}><MoreHorizontal size={16} /> إجراءات أخرى</button>
            {menuOpen && <>
              <div className="click-away" onClick={() => setMenuOpen(false)} />
              <div className="menu-pop up sc-menu rp-menu">
                {menu.map(gr => (
                  <div key={gr.title}>
                    <small className="sc-mt">{gr.title}</small>
                    {gr.items.map(it => <button key={it.label} className={`item ${it.danger ? 'danger' : ''}`} onClick={run(it.onClick)}><it.icon size={16} /> {it.label}</button>)}
                  </div>
                ))}
              </div>
            </>}
          </div>
        </div>
      </aside>

      <section className="card rp-main-col">
        <div className="tabs rp-tabs" role="tablist">
          {tabs.map(t => (
            <button key={t.key} role="tab" aria-selected={cur === t.key} className={cur === t.key ? 'on' : ''} onClick={() => setTab(t.key)}>
              <t.icon size={16} /> {t.label}
              <span className={`tab-badge ${t.alert ? 'alert' : ''}`}>{t.count.toLocaleString('ar-EG')}</span>
              {cur === t.key && <motion.span layoutId={`rp-tab-${s.StudentGuid}`} className="tab-line" transition={{ type: 'spring', stiffness: 500, damping: 40 }} />}
            </button>
          ))}
        </div>

        <div className="rp-pane">
          {cur === 'files' && (g.files.length ? (
            <div className="rp-files">
              {g.files.map(f => (
                <div key={f.FileId} className={`rp-file ${f.LevelActive ? '' : 'done'}`}>
                  <span className="rp-file-ic"><BookOpenText size={20} /></span>
                  <div className="rp-file-body">
                    <b>{f.DiplomName ?? '—'}</b>
                    <small>{[f.StudyBranchName, f.CaseName].filter(Boolean).join(' • ') || '—'}</small>
                  </div>
                  <span className="badge muted">{f.RegType === 1 ? 'دورة' : 'دبلوم'}</span>
                  {f.LevelActive ? <span className="badge ok">مستمر في الدراسة</span> : <span className="badge warn">أنهى الدراسة</span>}
                </div>
              ))}
            </div>
          ) : (
            <div className="rp-empty">
              <Users size={28} />
              <b>لا توجد ملفات تدريبية بعد</b>
              <span>{pend ? 'للطالب استمارة تسجيل بانتظار قبول المشرف، ويُفتح الملف التدريبي بعد القبول.' : 'ابدأ بـ "تسجيل استمارة" لعمل استمارة للطالب.'}</span>
              {pend ? <button className="btn secondary sm" onClick={() => setTab('pending')}><Hourglass size={15} /> عرض الاستمارة</button> : null}
            </div>
          ))}

          {cur === 'reqs' && (reqQ.isPending ? <div className="sk" style={{ height: 120, borderRadius: 14 }} /> : reqs.length ? <RequestList rows={reqs} /> : (
            <div className="rp-empty"><CreditCard size={28} /><b>لا توجد طلبات لهذا الطالب</b><span>طلبات الالتحاق والسداد المرسلة للمراجعة تظهر هنا مع حالتها.</span></div>
          ))}

          {cur === 'pending' && <PendingRegs account={s.AccountGuid} hasFiles />}
        </div>
      </section>
    </motion.div>
  )
}

/* ---------- Requests sent for this student (طلبات الالتحاق / السداد) ---------- */
interface SentRequest { kind: 'reg' | 'pay'; guid: string; code: number; date: string; status: number; statusName: string; title: string | null; amount: number; notes: string | null; doc: string | null }

function RequestList({ rows }: { rows: SentRequest[] }) {
  // Waiting ones and the ones reviewed during the last 7 days first; older ones behind a toggle.
  const recent = rows.filter(r => r.status === 0 || (r.kind === 'reg' && r.status === 3) || Date.now() - new Date(r.date).getTime() < 7 * 86_400_000)
  const [all, setAll] = useState(recent.length === 0)
  const shown = all ? rows : recent
  const tone = (r: SentRequest) => (r.status === 0 ? 'warn' : r.status === 1 ? 'ok' : r.kind === 'reg' && r.status === 3 ? 'info' : 'off')
  return (
    <div className="rp-reqs">
      {shown.map(r => (
        <div key={r.guid} className={`rp-req s${r.status}`}>
          <span className="rp-req-ic">{r.kind === 'reg' ? <ClipboardPlus size={17} /> : <CreditCard size={17} />}</span>
          <div className="rp-req-body">
            <b>{r.kind === 'reg' ? 'طلب التحاق' : 'طلب سداد'} <span className="rp-req-code">#{r.code}</span></b>
            <small>{[r.title, r.doc ? `${r.kind === 'reg' ? 'استمارة' : 'فاتورة'} ${r.doc}` : null, new Date(r.date).toLocaleDateString('ar-EG')].filter(Boolean).join(' • ')}</small>
            {r.notes && r.status !== 1 && <small className="rp-req-why">السبب: {r.notes}</small>}
          </div>
          <span className={`badge ${tone(r)}`}>{r.statusName}</span>
          <span className="rp-req-amt">{money(r.amount)}</span>
        </div>
      ))}
      {rows.length > recent.length && <button className="sc-toggle" onClick={() => setAll(v => !v)}>{all ? 'عرض الطلبات الحديثة فقط' : `عرض كل الطلبات (${rows.length.toLocaleString('ar-EG')})`}</button>}
    </div>
  )
}

/* ---------- Registration forms waiting for acceptance (NewStudentHome) ---------- */
interface PendingReg {
  guid: string; code: string; regDate: string; regType: number; docSubTotal: number; paid: number; payCode: string | null
  branchName: string | null; batchName: string | null; levelName: string | null; courseName: string | null; sellerName: string | null
  diploms: { diplomGuid: string; diplomName: string }[]
}
const esc = (t: string) => t.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))

function PendingRegs({ account, hasFiles }: { account: string; hasFiles: boolean }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const q = useQuery({ queryKey: ['reception', 'pending', account], queryFn: () => api<PendingReg[]>(`/reception/students/${account}/pending-regs`), staleTime: 30_000 })
  const accept = useMutation({
    mutationFn: ({ guid, body }: { guid: string; body: object }) => api<{ message: string }>(`/reception/regdocs/${guid}/accept`, { method: 'POST', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['reception'] }); alerts.success('تم قبول التسجيل', r.message) },
  })

  const onAccept = async (r: PendingReg) => {
    if (!can('newstudenthome', 'add')) { alerts.error('لا تملك صلاحية قبول التسجيل (قائمة الطلاب الجدد)'); return }
    const multi = r.diploms.length > 1
    const v = await alerts.form<{ diplomGuid: string | null; attachNotes: string; contractNotes: string }>({
      title: `قبول تسجيل الاستمارة #${r.code}`,
      confirmText: 'قبول وفتح الملف التدريبي',
      html: `<div class="sm-form">
        ${multi ? `<label class="sm-lbl">الاستمارة فيها أكثر من دبلوم، اختر الدبلوم الذي سيدرسه الطالب</label>
          <select id="acc-dip" class="input"><option value="">— اختر —</option>${r.diploms.map(d => `<option value="${d.diplomGuid}">${esc(d.diplomName)}</option>`).join('')}</select>` : ''}
        <label class="sm-chk"><input type="checkbox" id="acc-att"> مستندات التسجيل مكتملة ومراجَعة</label>
        <input id="acc-att-n" class="input" maxlength="250" placeholder="ملاحظات المستندات (اختياري)">
        <label class="sm-chk"><input type="checkbox" id="acc-con"> تم مراجعة اتفاقية التدريب</label>
        <input id="acc-con-n" class="input" maxlength="250" placeholder="ملاحظات الاتفاقية (اختياري)">
      </div>`,
      read: el => {
        const val = (id: string) => (el.querySelector(`#${id}`) as HTMLInputElement | null)
        if (multi && !val('acc-dip')?.value) return 'يرجى اختيار الدبلوم'
        if (!val('acc-att')?.checked) return 'يرجى تحديد حالة مستندات التسجيل'
        if (!val('acc-con')?.checked) return 'يرجى تحديد حالة اتفاقية التدريب'
        return { diplomGuid: multi ? val('acc-dip')!.value : null, attachNotes: val('acc-att-n')!.value, contractNotes: val('acc-con-n')!.value }
      },
    })
    if (v) accept.mutate({ guid: r.guid, body: { diplomGuid: v.diplomGuid, attachOk: true, attachNotes: v.attachNotes, contractOk: true, contractNotes: v.contractNotes } })
  }

  if (q.isPending) return hasFiles ? null : <div className="sk" style={{ height: 64, borderRadius: 14, margin: '10px 0' }} />
  const rows = q.data ?? []
  if (rows.length === 0) return hasFiles ? null : (
    <div className="st-files empty-files"><Users size={15} /> لا توجد ملفات تدريبية بعد. ابدأ بـ "تسجيل" لعمل استمارة للطالب.</div>
  )
  return (
    <div className="pending-regs">
      {rows.map(r => {
        const left = Math.max(0, r.docSubTotal - r.paid)
        const pct = r.docSubTotal > 0 ? Math.min(100, (r.paid / r.docSubTotal) * 100) : 0
        return (
          <div key={r.guid} className="pending-reg">
            <div className="pr-top">
              <span className="pr-ic"><Hourglass size={17} /></span>
              <div className="pr-title">
                <b>استمارة تسجيل #{r.code} <span className="badge warn">بانتظار قبول المشرف</span></b>
                <small>
                  {r.diploms.map(d => d.diplomName).join(' + ') || r.courseName}{r.regType === 1 && r.courseName ? ` • كود الدورة ${r.courseName}` : ''}
                  {r.branchName && ` • ${r.branchName}`}{r.batchName && ` • ${r.batchName}`}{r.levelName && ` • ${r.levelName}`}
                  {` • ${new Date(r.regDate).toLocaleDateString('ar-EG')}`}
                </small>
              </div>
              <div className="pr-btns">
                <button className="btn ghost sm" onClick={() => printRegistration(r.guid)}><Printer size={15} /> طباعة</button>
                {can('newstudenthome', 'add') && <button className="btn sm" disabled={accept.isPending} onClick={() => onAccept(r)}><BadgeCheck size={15} /> قبول التسجيل</button>}
              </div>
            </div>
            <div className="pr-money">
              <span><small>قيمة الاستمارة</small><b>{money(r.docSubTotal)}</b></span>
              <span className="ok"><small>دفعة مقدمة{r.payCode ? ` (فاتورة ${r.payCode})` : ''}</small><b>{money(r.paid)}</b></span>
              <span className="due"><small>المتبقي</small><b>{money(left)}</b></span>
              <span className="pr-bar"><i style={{ width: `${pct}%` }} /></span>
            </div>
            {r.diploms.length > 1 && r.regType === 0 && <div className="alert warn" style={{ marginTop: 8 }}>تنبيه: تحتوي هذه الاستمارة على أكثر من دبلوم (أُنشئت قبل التحديث)، وعند القبول سيتم اختيار دبلوم واحد للملف التدريبي.</div>}
            <p className="pr-hint">يُفتح الملف التدريبي وتُتاح عمليات السداد بعد قبول المشرف للتسجيل، وفق نظام المعهد.</p>
          </div>
        )
      })}
    </div>
  )
}

/* ============================== Student card (EncodingStudent) ============================== */
interface Lookups {
  cases: { Guid: string; Name: string }[]; diploms: { Guid: string; Name: string; StudyType: number }[]; batches: { Guid: string; Name: string }[]
  levels: { Guid: string; Name: string }[]; courses: { Guid: string; Name: string; DiplomGuid: string }[]
}
interface StudentForm {
  StudentName: string; StudentNameEn: string; BirthDate: string; StudentTel: string; NationalId: string; Email: string; BranchGuid: string; RegCaseGuid: string
  StudentType: number; StudentNational: number; IsUse: boolean; NoData: boolean; Notes: string
}
const EMPTY: StudentForm = { StudentName: '', StudentNameEn: '', BirthDate: '', StudentTel: '', NationalId: '', Email: '', BranchGuid: '', RegCaseGuid: '', StudentType: 1, StudentNational: 0, IsUse: true, NoData: false, Notes: '' }

function StudentCard({ guid: initialGuid, archive, prefill, lookups, onClose, onSaved }: {
  guid: string | null; archive?: ArchiveRow; prefill?: Partial<StudentForm>; lookups?: Lookups; onClose: () => void; onSaved: (nid: string) => void
}) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const isNew = !initialGuid
  const canSave = isNew ? can('addstudent', 'add') : can('addstudent', 'edit')
  const ro = !canSave
  const details = useQuery({ queryKey: ['reception', 'student', initialGuid], queryFn: () => api<Record<string, unknown>>(`/reception/students/${initialGuid}`), enabled: !!initialGuid, staleTime: 0 })
  const preview = useQuery({
    queryKey: ['reception', 'archive-preview', archive?.custNo, archive?.oldBranchId], enabled: !!archive, staleTime: 60_000,
    queryFn: () => api<{ maden: number; daen: number }>(`/reception/archive/preview${qs({ custNo: archive!.custNo, oldBranchId: archive!.oldBranchId, nationalId: archive!.nationalId })}`),
  })

  const [f, setF] = useState<StudentForm>(() => ({
    ...EMPTY, ...prefill,
    ...(archive ? { StudentName: archive.name ?? '', StudentTel: (archive.mobile ?? '').replace(/\D/g, '').slice(-10), NationalId: archive.nationalId ?? '', Notes: `منقول من الأرشيف - ${archive.accountName ?? ''}`.slice(0, 250) } : {}),
  }))
  const [arc, setArc] = useState({ type: 0, diplom: '', batch: '', level: '', course: '' })
  const baseline = useRef(JSON.stringify(f))
  const dirty = JSON.stringify(f) !== baseline.current
  useEffect(() => {
    const d = details.data
    if (!d) return
    const v: StudentForm = {
      StudentName: String(d.StudentName ?? ''), StudentNameEn: String(d.StudentNameEn ?? ''), BirthDate: String(d.BirthDate ?? ''), StudentTel: String(d.StudentTel ?? ''), NationalId: String(d.NationalId ?? ''), Email: String(d.Email ?? ''),
      BranchGuid: String(d.BranchGuid ?? ''), RegCaseGuid: String(d.RegCaseGuid ?? ''), StudentType: Number(d.StudentType ?? 1), StudentNational: Number(d.StudentNational ?? 0),
      IsUse: !!d.IsUse, NoData: !!d.NoData, Notes: String(d.Notes ?? ''),
    }
    setF(v); baseline.current = JSON.stringify(v)
  }, [details.data])
  const set = (p: Partial<StudentForm>) => setF(x => ({ ...x, ...p }))

  const enOk = /^[A-Za-z][A-Za-z .'-]*$/.test(f.StudentNameEn.trim())
  const bdRange = (() => { const t = new Date(), iso = (y: number) => `${t.getFullYear() - y}-${String(t.getMonth() + 1).padStart(2, '0')}-${String(t.getDate()).padStart(2, '0')}`; return { min: iso(100), max: iso(10) } })()
  const bdOk = !!f.BirthDate && f.BirthDate >= bdRange.min && f.BirthDate <= bdRange.max
  const validate = (): string | null => {
    if (!f.StudentName.trim()) return 'يرجى إدخال اسم الطالب'
    if (isNew && !f.StudentNameEn.trim()) return 'يرجى إدخال اسم الطالب بالإنجليزي'
    if (f.StudentNameEn.trim() && !enOk) return 'الاسم بالإنجليزي يُكتب بحروف إنجليزية فقط'
    if (isNew && !f.BirthDate) return 'يرجى إدخال تاريخ ميلاد الطالب'
    if (f.BirthDate && !bdOk) return 'يرجى إدخال تاريخ ميلاد صحيح'
    if (!f.StudentTel.trim()) return 'يرجى إدخال رقم الجوال'
    if (!/^\d{10}$/.test(f.StudentTel)) return 'يرجى إدخال رقم جوال صحيح'
    if (!f.NationalId.trim()) return 'يرجى إدخال رقم الهوية'
    if (!/^[1-9]\d{9}$/.test(f.NationalId)) return 'يرجى إدخال رقم هوية صحيح'
    if (!f.BranchGuid) return 'يرجى اختيار الفرع القائم بتسجيل الطالب'
    if (!f.RegCaseGuid) return 'يرجى اختيار حالة تسجيل الطالب'
    if (!f.Email.trim()) return 'يرجى إدخال البريد الإلكتروني الخاص بالطالب'
    if (f.NoData && !f.Notes.trim()) return 'يرجى إدخال بيانات المعهد الآخر المسجل به الطالب'
    if (archive) {
      if (arc.type === 0 && !arc.diplom) return 'يرجى اختيار الدبلوم أولاً'
      if (arc.type === 0 && !arc.batch) return 'يرجى اختيار الدفعة أولاً'
      if (arc.type === 0 && !arc.level) return 'يرجى اختيار المستوى أولاً'
      if (arc.type === 1 && !arc.course) return 'يرجى اختيار الدورة أولاً'
    }
    return null
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = {
        ...f, StudentNameEn: f.StudentNameEn.trim() || null, BirthDate: f.BirthDate || null, branchGuid: f.BranchGuid || null, regCaseGuid: f.RegCaseGuid || null,
        archive: archive ? {
          custNo: archive.custNo, oldBranchId: archive.oldBranchId, type: arc.type,
          diplomGuid: arc.type === 0 ? arc.diplom || null : lookups?.courses.find(c => c.Guid === arc.course)?.DiplomGuid ?? null,
          batchGuid: arc.batch || null, levelGuid: arc.level || null, courseGuid: arc.course || null,
        } : null,
      }
      if (isNew) {
        if (archive && !(await alerts.confirm({ title: 'قبول الطالب من الأرشيف؟', text: `سيتم إنشاء ملف الطالب ونقل رصيده السابق (${money((preview.data?.maden ?? 0) - (preview.data?.daen ?? 0))}) كرصيد افتتاحي`, confirmText: 'قبول' }))) return null
        return api<{ message: string; code: string; guid: string }>('/reception/students', { method: 'POST', json: body })
      }
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/reception/students/${initialGuid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async r => {
      if (!r) return
      baseline.current = JSON.stringify(f)
      qc.invalidateQueries({ queryKey: ['reception'] })
      await alerts.success(r.message, `كود الطالب: ${r.code}`)
      onSaved(f.NationalId)
      onClose()
    },
  })
  const submit = () => { const e = validate(); if (e) { alerts.error(e); return } save.mutate() }
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const d = details.data
  const loading = !!initialGuid && details.isPending
  const nidOk = /^[1-9]\d{9}$/.test(f.NationalId), telOk = /^\d{10}$/.test(f.StudentTel)
  const bal = (preview.data?.maden ?? 0) - (preview.data?.daen ?? 0)
  return (
    <Drawer size="lg" onClose={requestClose} icon={archive ? <Archive size={22} /> : <UserRound size={22} />}
      title={archive ? 'قبول طالب من الأرشيف' : isNew ? 'طالب جديد' : String(d?.StudentName ?? 'بطاقة الطالب')}
      subtitle={archive ? `${archive.branchName} • رقم العميل القديم ${archive.custNo}` : isNew ? 'سيتم إنشاء حساب الطالب المحاسبي تلقائياً' : `كود ${d?.Code} • حساب ${d?.AccountCode}`}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : archive ? <><ClipboardPlus size={17} /> قبول الطالب</> : <><Plus size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <div className="grid cols-2">
          {archive && (
            <div className="span-all archive-balance">
              <Wallet size={22} />
              <div><small>الرصيد في النظام القديم</small><b dir="ltr">{preview.isPending ? '…' : money(Math.abs(bal))}</b></div>
              <span className={`badge ${bal > 0 ? 'warn' : bal < 0 ? 'ok' : 'muted'}`}>{bal > 0 ? 'مدين (مستحق على الطالب)' : bal < 0 ? 'دائن (للطالب)' : 'لا يوجد رصيد'}</span>
              <span className="muted" style={{ marginInlineStart: 'auto', fontSize: 12 }}>يُسجَّل تلقائياً كقيد رصيد افتتاحي</span>
            </div>
          )}
          <div className="section-label span-all">البيانات الأساسية</div>
          <Field label="اسم الطالب" required className="span-2"><input className="input" autoFocus readOnly={ro} maxLength={150} value={f.StudentName} onChange={e => set({ StudentName: e.target.value })} /></Field>
          <Field label="الاسم بالإنجليزي" required={isNew} error={f.StudentNameEn.trim() && !enOk ? 'حروف إنجليزية فقط' : undefined}>
            <input className={`input ${f.StudentNameEn.trim() && !enOk ? 'invalid' : ''}`} dir="ltr" lang="en" autoComplete="off" placeholder="Full name in English" readOnly={ro} maxLength={150} value={f.StudentNameEn} onChange={e => set({ StudentNameEn: e.target.value })} />
          </Field>
          <Field label="تاريخ الميلاد" required={isNew} error={f.BirthDate && !bdOk ? 'تاريخ الميلاد غير صحيح' : undefined}>
            <input className={`input ${f.BirthDate && !bdOk ? 'invalid' : ''}`} type="date" min={bdRange.min} max={bdRange.max} readOnly={ro} value={f.BirthDate} onChange={e => set({ BirthDate: e.target.value })} />
          </Field>
          <Field label="رقم الهوية" required error={f.NationalId && !nidOk ? 'رقم الهوية غير صحيح' : undefined} hint={nidOk ? '✓ رقم الهوية صحيح' : 'عشرة أرقام ولا يبدأ بصفر'}>
            <input className={`input ${f.NationalId && !nidOk ? 'invalid' : ''}`} dir="ltr" inputMode="numeric" maxLength={10} readOnly={ro} value={f.NationalId} onChange={e => set({ NationalId: e.target.value.replace(/\D/g, '') })} />
          </Field>
          <Field label="رقم الجوال" required error={f.StudentTel && !telOk ? 'يجب أن يتكون رقم الجوال من 10 أرقام' : undefined}>
            <input className={`input ${f.StudentTel && !telOk ? 'invalid' : ''}`} dir="ltr" inputMode="numeric" maxLength={10} readOnly={ro} value={f.StudentTel} onChange={e => set({ StudentTel: e.target.value.replace(/\D/g, '') })} />
          </Field>
          <Field label="البريد الإلكتروني" required className="span-2"><input className="input" dir="ltr" type="email" readOnly={ro} maxLength={150} value={f.Email} onChange={e => set({ Email: e.target.value })} /></Field>
          <Field label="النوع" required><Segmented value={f.StudentType} onChange={v => !ro && set({ StudentType: v })} options={[{ value: 0, label: 'ذكر' }, { value: 1, label: 'أنثى' }]} /></Field>
          <Field label="الجنسية" required><Segmented value={f.StudentNational} onChange={v => !ro && set({ StudentNational: v })} options={[{ value: 0, label: 'مواطن' }, { value: 1, label: 'أجنبي' }]} /></Field>
          <div className="section-label span-all">التسجيل</div>
          <Field label="فرع التسجيل" required>
            <select className="input" disabled={ro} value={f.BranchGuid} onChange={e => set({ BranchGuid: e.target.value })}>
              <option value="">اختر...</option>
              {branches.data?.filter(b => b.isUse || b.guid === f.BranchGuid).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
            </select>
          </Field>
          <Field label="حالة التسجيل" required>
            <select className="input" disabled={ro} value={f.RegCaseGuid} onChange={e => set({ RegCaseGuid: e.target.value })}>
              <option value="">اختر...</option>
              {lookups?.cases.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
            </select>
          </Field>

          {archive && (
            <>
              <div className="section-label span-all">بيانات الدراسة في النظام القديم</div>
              <Field label="نوع التسجيل" required className="span-2"><Segmented value={arc.type} onChange={v => setArc(a => ({ ...a, type: v }))} options={[{ value: 0, label: 'دبلوم' }, { value: 1, label: 'دورة' }]} /></Field>
              {arc.type === 0 ? <>
                <Field label="الدبلوم" required className="span-2"><select className="input" value={arc.diplom} onChange={e => setArc(a => ({ ...a, diplom: e.target.value }))}><option value="">اختر...</option>{lookups?.diploms.filter(x => x.StudyType === 0).map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</select></Field>
                <Field label="الدفعة" required><select className="input" value={arc.batch} onChange={e => setArc(a => ({ ...a, batch: e.target.value }))}><option value="">اختر...</option>{lookups?.batches.map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</select></Field>
                <Field label="المستوى" required><select className="input" value={arc.level} onChange={e => setArc(a => ({ ...a, level: e.target.value }))}><option value="">اختر...</option>{lookups?.levels.map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</select></Field>
              </> : (
                <Field label="الدورة" required className="span-2"><select className="input" value={arc.course} onChange={e => setArc(a => ({ ...a, course: e.target.value }))}><option value="">اختر...</option>{lookups?.courses.map(x => <option key={x.Guid} value={x.Guid}>{x.Name}</option>)}</select></Field>
              )}
            </>
          )}

          <div className="section-label span-all">ملاحظات</div>
          <div className="span-2"><Switch label="الطالب مسجل في معهد آخر (يُستفسر من الطالب)" checked={f.NoData} disabled={ro} onChange={v => set({ NoData: v })} /></div>
          <Field label={f.NoData ? 'بيانات المعهد الآخر' : 'ملاحظات'} required={f.NoData} className="span-2"><textarea className="input" readOnly={ro} maxLength={250} value={f.Notes} onChange={e => set({ Notes: e.target.value })} /></Field>
          {!isNew && <div className="span-2"><Switch label="الطالب فعال (إيقافه يمنع أي عملية عليه)" checked={f.IsUse} disabled={ro} onChange={v => set({ IsUse: v })} /></div>}
        </div>
      )}
    </Drawer>
  )
}
