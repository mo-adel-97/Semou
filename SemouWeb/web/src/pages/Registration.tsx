import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { BookOpenCheck, Check, ChevronLeft, ChevronRight, ClipboardList, FileCheck2, FileUp, GraduationCap, Paperclip, Receipt, Search, Send, ShieldCheck, Upload, Wallet } from 'lucide-react'
import { api, ApiError, tokenStore } from '../api'
import { alerts } from '../lib/alerts'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Drawer, Field, Spinner, Switch } from '../components/ui'

/* استمارة التسجيل (WhatRegTypeNeed → RegisterForm → AttachForm) as a step-by-step wizard. */
interface PriceRow { code: string; name: string; diplomGuid: string; diplomType: number; cost: number; tax: number; total: number; payTaxRate: number }
interface Prep {
  student: { Guid: string; StudentName: string; NationalId: string; foreign: boolean }
  setting: { guid: string; code: number; docName: string; branchName: string; chkOtherFees: boolean; chkStartPay: boolean; chkAmountPay: boolean; amountPay: number; docPayName: string | null; chkVat: boolean }
  zeroAmount: boolean
  diploms: PriceRow[]; fees: PriceRow[]
  batches: { Code: string; BatchName: string; TotalCapacity: number; RegisteredStudents: number; AvailableSeats: number; Guid: string }[]
  levels: { Guid: string; Name: string; IsDef: boolean }[]
  courses: { Guid: string; Code: string; Name: string; CourseCode: string | null; DiplomName: string; DateStart: string; DateEnd: string; Time: string; Capacity: number; Registered: number }[]
  sellers: { ManFullName: string; ManUseName: string; Guid: string }[]
  cashBoxes: { Guid: string; Name: string; CashType: number; CostCenterGuid: string | null; CostName: string | null }[]
  attachments: { code: string; name: string; required: boolean; rule: string | null }[]
}
export interface RegStudent { StudentGuid: string; StudentName: string; NationalId: string; RegBranchGuid: string; StudyBranchGuid: string | null }

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const r2 = (n: number) => Math.round(n * 100) / 100
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const OFFICE = 'صندوق المكتب'
type Step = 'type' | 'data' | 'items' | 'pay' | 'files' | 'review'

export function RegistrationWizard({ student, onClose }: { student: RegStudent; onClose: () => void }) {
  const qc = useQueryClient()
  const branches = useQuery({ queryKey: ['registration', 'branches'], queryFn: () => api<{ Guid: string; Name: string }[]>('/registration/branches'), staleTime: 300_000 })
  const [step, setStep] = useState<Step>('type')
  const [branch, setBranch] = useState(student.StudyBranchGuid ?? student.RegBranchGuid ?? '')
  const [regType, setRegType] = useState<0 | 1>(0)
  const [contract, setContract] = useState(false)
  const [prep, setPrep] = useState<Prep | null>(null)
  const [loadingPrep, setLoadingPrep] = useState(false)

  // Step data
  const [regDate, setRegDate] = useState(today())
  const [batch, setBatch] = useState('')
  const [level, setLevel] = useState('')
  const [course, setCourse] = useState('')
  const [seller, setSeller] = useState('')
  const [notes, setNotes] = useState('')
  const [picked, setPicked] = useState<Record<string, number | null>>({}) // diplomGuid -> overridden total (null = list price)
  const [pickedFees, setPickedFees] = useState<Record<string, number | null>>({})
  const [coursePrices, setCoursePrices] = useState<PriceRow[] | null>(null)
  const [pay, setPay] = useState({ cash: '', cost: '', costName: '', ref: '', date: today(), notes: '', lines: {} as Record<string, string>, split: false, amount1: '', amount2: '', cash2: '', cost2: '', costName2: '', ref2: '', date2: today() })
  const [pickCost, setPickCost] = useState<1 | 2 | null>(null)
  const [files, setFiles] = useState<Record<string, File | null>>({})
  const [uploaded, setUploaded] = useState<string[]>([])
  const [uploading, setUploading] = useState(false)

  const start = async () => {
    if (!branch) { alerts.error('يرجى اختيار فرع الدراسة أولاً'); return }
    if (regType === 0 && !(await alerts.confirm({ title: 'اتفاقية التدريب', text: 'هل وقع المتدرب اتفاقية التدريب؟', confirmText: 'نعم، وقّع', cancelText: 'لا' }).then(v => { setContract(v); return true }))) return
    setLoadingPrep(true)
    try {
      const p = await api<Prep>(`/registration/prepare?student=${student.StudentGuid}&branch=${branch}&regType=${regType}`)
      setPrep(p)
      setLevel(p.levels.find(l => l.IsDef)?.Guid ?? '')
      if (p.sellers.length === 1) setSeller(p.sellers[0].Guid)
      const up = await api<{ uploaded: string[] }>(`/registration/attachments?nationalId=${student.NationalId}`)
      setUploaded(up.uploaded)
      setStep('data')
    } catch (e) { alerts.error((e as Error).message) }
    finally { setLoadingPrep(false) }
  }

  // Course → its own priced items (SHOWPRICESALEONLYCOURSE), auto-selected.
  useEffect(() => {
    if (!prep || regType !== 1 || !course) return
    api<PriceRow[]>(`/registration/course-prices?student=${student.StudentGuid}&branch=${branch}&course=${course}`)
      .then(r => { setCoursePrices(r); setPicked(Object.fromEntries(r.map(x => [x.diplomGuid, null]))) })
      .catch(e => alerts.error((e as Error).message))
  }, [course, prep, regType, student.StudentGuid, branch])

  // Diploma seats in the chosen batch (AskForDiplomSeat per diploma). Changing the batch clears the choice.
  const [seats, setSeats] = useState<Record<string, Seat> | null>(null)
  useEffect(() => {
    if (!prep || regType !== 0 || !batch) { setSeats(null); return }
    setPicked({})
    let live = true
    api<(Seat & { diplomGuid: string })[]>(`/registration/diplom-seats?branch=${branch}&batch=${batch}`)
      .then(r => { if (live) setSeats(Object.fromEntries(r.map(x => [x.diplomGuid, x]))) })
      .catch(e => alerts.error((e as Error).message))
    return () => { live = false }
  }, [batch, prep, regType, branch])

  const items = useMemo(() => (regType === 1 && coursePrices ? coursePrices : prep?.diploms ?? []), [regType, coursePrices, prep])
  const eff = (p: PriceRow, override: number | null | undefined) => (override != null && override !== p.total ? { ...p, cost: override, tax: 0, total: override } : p)
  const selDips = items.filter(p => p.diplomGuid in picked).map(p => eff(p, picked[p.diplomGuid]))
  const selFees = (prep?.fees ?? []).filter(p => p.diplomGuid in pickedFees).map(p => eff(p, pickedFees[p.diplomGuid]))
  const all = [...selDips, ...selFees]
  const totals = { cost: r2(all.reduce((s, x) => s + x.cost, 0)), tax: r2(all.reduce((s, x) => s + x.tax, 0)), total: r2(all.reduce((s, x) => s + x.total, 0)) }
  const dSub = r2(selDips.reduce((s, x) => s + x.total, 0))
  const payLines = selDips.map(d => {
    const t = Number(pay.lines[d.diplomGuid] ?? '') || 0
    const cost = d.payTaxRate ? Math.round(t / (1 + d.payTaxRate / 100) * 1000) / 1000 : t
    const tax = d.payTaxRate ? Math.round(cost * d.payTaxRate / 100 * 1000) / 1000 : 0
    return { d, total: t, cost, tax }
  })
  const paySub = r2(payLines.reduce((s, x) => s + x.total, 0)), payTax = r2(payLines.reduce((s, x) => s + x.tax, 0))
  const cash = prep?.cashBoxes.find(c => c.Guid === pay.cash), cash2 = prep?.cashBoxes.find(c => c.Guid === pay.cash2)

  // AttachForm required rules
  const required = (code: string) => code === 'ID' || (code === 'Certifcat' && regType === 0) || (code === 'contract' && contract && regType === 0)
    || (code === 'trans' && !!cash && cash.Name !== OFFICE) || (code === 'trans2' && pay.split && !!cash2 && cash2.Name !== OFFICE)
  const missingFiles = (prep?.attachments ?? []).filter(a => required(a.code) && !uploaded.includes(a.code))

  const steps: { key: Step; label: string; icon: typeof Check; show: boolean }[] = [
    { key: 'type', label: 'نوع التسجيل', icon: GraduationCap, show: true },
    { key: 'data', label: 'بيانات التسجيل', icon: ClipboardList, show: true },
    { key: 'items', label: 'الدبلومات والرسوم', icon: BookOpenCheck, show: true },
    { key: 'pay', label: 'الدفعة المقدمة', icon: Wallet, show: !!prep?.setting.chkStartPay },
    { key: 'files', label: 'المرفقات', icon: Paperclip, show: true },
    { key: 'review', label: 'المراجعة والإرسال', icon: ShieldCheck, show: true },
  ]
  const visible = steps.filter(s => s.show)
  const idx = visible.findIndex(s => s.key === step)

  // Desktop VALIDATE_DATAFOR_STEP1 / STEP2 / VALIDATE_DATA, per step.
  const check = (s: Step): string | null => {
    if (!prep) return null
    if (s === 'data') {
      if (regType === 0 && !batch) return 'يرجى اختيار الدفعة'
      if (regType === 0 && !level) return 'يرجى اختيار المستوى المراد تسجيل الطالب عليه'
      if (regType === 1 && !course) return 'يرجى اختيار الدورة'
      if (regType === 1) { const c = prep.courses.find(x => x.Guid === course); if (c && c.Capacity - c.Registered <= 0) return 'لا توجد أي مقاعد متاحة للتسجيل' }
      if (regType === 0) { const b = prep.batches.find(x => x.Guid === batch); if (b && b.AvailableSeats <= 0) return 'لا توجد مقاعد متاحة في هذه الدفعة' }
      if (!seller) return 'يرجى اختيار مندوب البيع'
    }
    if (s === 'items') {
      if (!selDips.length) return 'لا يمكن التسجيل بدون اختيار الدبلوم - الدورة'
      if (regType === 0 && selDips.length !== 1) return 'استمارة تسجيل الدبلوم تكون لدبلوم واحد فقط'
      if (regType === 0 && seats && (seats[selDips[0].diplomGuid]?.available ?? 0) <= 0) return 'عذراً، التخصص الذي اخترته لا تتوفر فيه أي مقاعد للتسجيل'
      if (prep.setting.chkOtherFees && !selFees.length) return 'لا يمكن التسجيل بدون اختيار رسوم الاختبار - الشهادة'
    }
    if (s === 'pay' && prep.setting.chkStartPay) {
      if (!pay.cash) return 'يرجى تحديد الخزينة / البنك أولاً'
      if (!pay.cost) return 'يرجى اختيار مركز التكلفة الخاص بعملية السداد'
      if (!pay.ref.trim()) return 'يرجى إدخال رقم المرجع الخاص بعملية السداد'
      if (!pay.date) return 'يرجى إدخال تاريخ الحوالة البنكية / السداد'
      for (const l of payLines) {
        if (l.total && l.total < prep.setting.amountPay) return `لا يمكن إدخال قيمة أقل من القيمة المحددة للدفعة المقدمة (${money(prep.setting.amountPay)})`
        if (l.total > dSub) return 'لا يمكن إدخال قيمة أكبر من قيمة الدبلوم / الدورة دون الرسوم'
      }
      if (paySub === 0 && !prep.zeroAmount) return 'يرجى إدخال المبلغ الخاص بعملية السداد'
      if (pay.split) {
        if (!pay.cash2) return 'يرجى تحديد الخزينة / البنك الثاني'
        if (!pay.cost2) return 'يرجى اختيار مركز التكلفة الثاني الخاص بعملية السداد'
        if (!pay.ref2.trim()) return 'يرجى إدخال رقم المرجع الثاني الخاص بعملية السداد'
        if (!(Number(pay.amount1) > 0)) return 'يرجى إدخال قيمة الحوالة الأولى'
        if (!(Number(pay.amount2) > 0)) return 'يرجى إدخال قيمة الحوالة الثانية'
        if (r2(Number(pay.amount1) + Number(pay.amount2)) !== paySub) return 'مبالغ الحوالات لا تساوي إجمالي الفاتورة'
      }
    }
    if (s === 'files' && missingFiles.length) return `لا يمكن التسجيل بدون إدخال المرفقات الخاصة بالتسجيل: ${missingFiles.map(f => f.name).join('، ')}`
    return null
  }
  const next = () => {
    if (step === 'type') { start(); return }
    const e = check(step); if (e) { alerts.error(e); return }
    setStep(visible[idx + 1].key)
  }
  const back = () => idx > 0 && setStep(visible[idx - 1].key)

  const upload = async () => {
    const list = Object.entries(files).filter(([, f]) => f) as [string, File][]
    if (!list.length) { alerts.error('لا يمكن الحفظ بدون اختيار الملفات المرفقة'); return }
    const fd = new FormData()
    fd.append('nationalId', student.NationalId)
    for (const [code, f] of list) fd.append(code, f, f.name)
    setUploading(true)
    try {
      const res = await fetch('/api/registration/attachments', { method: 'POST', body: fd, headers: { Authorization: `Bearer ${tokenStore.get()}` } })
      const data = await res.json().catch(() => null)
      if (!res.ok) throw new ApiError(res.status, data?.message ?? 'فشل رفع المرفقات')
      setUploaded(data.uploaded); setFiles({})
      alerts.success('تم رفع المرفقات بنجاح')
    } catch (e) { alerts.error((e as Error).message) }
    finally { setUploading(false) }
  }

  const save = useMutation({
    mutationFn: async () => {
      for (const s of visible.map(v => v.key)) { const e = check(s); if (e) throw Object.assign(new Error(e), { status: 400 }) }
      if (!(await alerts.confirm({ title: 'إرسال طلب الالتحاق؟', text: prep?.setting.chkStartPay
        ? `سيُرسل الطلب إلى المبيعات للمراجعة. بعد قبوله تُنشأ استمارة التسجيل وفاتورة الدفعة المقدمة (${money(paySub)}) وسند القبض.`
        : 'سيُرسل الطلب إلى المبيعات للمراجعة، وبعد قبوله تُنشأ استمارة التسجيل وقيدها المحاسبي.', confirmText: 'إرسال الطلب' }))) return null
      alerts.loading('جاري إرسال الطلب...')
      return api<{ message: string; code: number; guid: string }>('/requests/registration', {
        method: 'POST', json: {
          studentGuid: student.StudentGuid, regType, studyBranchGuid: branch, chkContract: contract, regDate, batchGuid: batch || null, levelGuid: level || null,
          courseGuid: course || null, sellerGuid: seller || null, notes,
          diploms: selDips.map(d => ({ diplomGuid: d.diplomGuid, total: picked[d.diplomGuid] ?? null })),
          fees: selFees.map(d => ({ diplomGuid: d.diplomGuid, total: pickedFees[d.diplomGuid] ?? null })),
          payment: prep?.setting.chkStartPay ? {
            cashAccountGuid: pay.cash || null, costCenterGuid: pay.cost || null, ref: pay.ref, payDate: pay.date, notes: pay.notes,
            lines: payLines.map(l => ({ diplomGuid: l.d.diplomGuid, total: l.total })),
            split: pay.split, amount1: Number(pay.amount1) || 0, amount2: Number(pay.amount2) || 0, cashAccountGuid2: pay.cash2 || null, costCenterGuid2: pay.cost2 || null, ref2: pay.ref2, payDate2: pay.date2,
          } : null,
        },
      })
    },
    onSuccess: async r => {
      if (!r) return
      qc.invalidateQueries({ queryKey: ['reception'] }); qc.invalidateQueries({ queryKey: ['requests'] })
      await alerts.success(r.message, 'ستظهر حالة الطلب في بطاقة الطالب، وتُنشأ الاستمارة فور قبوله من المبيعات.')
      onClose()
    },
  })

  return (
    <Drawer size="lg" onClose={async () => { if (step === 'type' || await alerts.confirm({ title: 'إلغاء التسجيل؟', text: 'لن يتم حفظ البيانات التي أدخلتها', confirmText: 'إلغاء التسجيل', cancelText: 'رجوع', danger: true })) onClose() }}
      icon={<GraduationCap size={22} />} title={`تسجيل: ${student.StudentName}`} subtitle={prep ? `${prep.setting.docName} • ${prep.setting.branchName}` : 'استمارة تسجيل جديدة'}
      tabs={<div className="wizard-steps">{visible.map((s, i) => (
        <div key={s.key} className={`ws ${i < idx ? 'done' : i === idx ? 'on' : ''}`}>
          <span className="dot">{i < idx ? <Check size={14} /> : <s.icon size={14} />}</span><span className="lbl">{s.label}</span>
        </div>
      ))}</div>}
      footer={<>
        {idx > 0 && <button className="btn ghost" onClick={back}><ChevronRight size={16} /> السابق</button>}
        {step !== 'review' ? <button className="btn" onClick={next} disabled={loadingPrep}>{loadingPrep ? <Spinner /> : <>التالي <ChevronLeft size={16} /></>}</button>
          : <button className="btn" onClick={() => save.mutate()} disabled={save.isPending}>{save.isPending ? <Spinner /> : <><Send size={17} /> إرسال طلب الالتحاق</>}</button>}
        <span className="hint">{prep && <>الإجمالي: <b className="num-strong">{money(totals.total)}</b> ر.س</>}</span>
      </>}>
      <AnimatePresence mode="wait">
        <motion.div key={step} initial={{ opacity: 0, x: -16 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: 16 }} transition={{ duration: 0.18 }}>
          {/* ---------- 1. Type (WhatRegTypeNeed) ---------- */}
          {step === 'type' && (
            <div className="grid cols-2">
              <Field label="فرع الدراسة" required className="span-2" hint={student.StudyBranchGuid ? 'فرع الدراسة المسجل للطالب' : 'لا يوجد فرع دراسة مسجل، تم اقتراح فرع التسجيل'}>
                <select className="input" value={branch} onChange={e => setBranch(e.target.value)}>
                  <option value="">اختر...</option>
                  {branches.data?.map(b => <option key={b.Guid} value={b.Guid}>{b.Name}</option>)}
                </select>
              </Field>
              <Field label="نوع التسجيل" required className="span-2">
                <div className="type-cards">
                  {([[0, 'دبلوم', 'تسجيل في دبلوم على دفعة ومستوى'], [1, 'دورة', 'تسجيل في دورة قصيرة بمقاعد محددة']] as const).map(([v, t, d]) => (
                    <button key={v} type="button" className={`type-card ${regType === v ? 'on' : ''}`} onClick={() => setRegType(v)}>
                      {v === 0 ? <GraduationCap size={26} /> : <BookOpenCheck size={26} />}<b>{t}</b><small>{d}</small>
                    </button>
                  ))}
                </div>
              </Field>
            </div>
          )}

          {/* ---------- 2. Data ---------- */}
          {step === 'data' && prep && (
            <div className="grid cols-2">
              <Field label="تاريخ التسجيل" required><input type="date" className="input" value={regDate} onChange={e => setRegDate(e.target.value)} /></Field>
              <Field label="مندوب البيع" required>
                <select className="input" value={seller} onChange={e => setSeller(e.target.value)}>
                  <option value="">اختر...</option>
                  {prep.sellers.map(s => <option key={s.Guid} value={s.Guid}>{s.ManFullName}</option>)}
                </select>
              </Field>
              {regType === 0 ? <>
                <Field label="الدفعة" required className="span-2">
                  <div className="pick-grid">
                    {prep.batches.filter(b => b.TotalCapacity > 0 || b.Guid === batch).map(b => (
                      <button key={b.Guid} type="button" className={`pick-card ${batch === b.Guid ? 'on' : ''} ${b.AvailableSeats <= 0 ? 'full' : ''}`} onClick={() => setBatch(b.Guid)}>
                        <b>{b.BatchName}</b><small>{b.AvailableSeats > 0 ? `${b.AvailableSeats} مقعد متاح من ${b.TotalCapacity}` : 'مكتملة'}</small>
                        <span className="seat-bar"><i style={{ width: `${Math.min(100, (b.RegisteredStudents / Math.max(1, b.TotalCapacity)) * 100)}%` }} /></span>
                      </button>
                    ))}
                    {!prep.batches.some(b => b.TotalCapacity > 0) && <div className="muted">لا توجد دفعات بها مقاعد لهذا الفرع (ضبط أعداد الدفعات)</div>}
                  </div>
                </Field>
                <Field label="المستوى" required>
                  <select className="input" value={level} onChange={e => setLevel(e.target.value)}>
                    <option value="">اختر...</option>{prep.levels.map(l => <option key={l.Guid} value={l.Guid}>{l.Name}{l.IsDef ? ' (افتراضي)' : ''}</option>)}
                  </select>
                </Field>
              </> : (
                <Field label="الدورة" required className="span-2">
                  <div className="pick-grid">
                    {prep.courses.map(c => {
                      const ava = c.Capacity - c.Registered
                      return (
                        <button key={c.Guid} type="button" className={`pick-card ${course === c.Guid ? 'on' : ''} ${ava <= 0 ? 'full' : ''}`} onClick={() => setCourse(c.Guid)}>
                          <b>{c.Name}</b>{c.CourseCode && <small>كود الدورة: <span dir="ltr">{c.CourseCode}</span></small>}
                          <small dir="ltr">{String(c.DateStart).slice(0, 10)} → {String(c.DateEnd).slice(0, 10)} {c.Time ?? ''}</small>
                          <small>{ava > 0 ? `${ava} مقعد متاح من ${c.Capacity}` : 'مكتملة'}</small>
                        </button>
                      )
                    })}
                  </div>
                </Field>
              )}
              <Field label="ملاحظات" className="span-2"><textarea className="input" maxLength={250} value={notes} onChange={e => setNotes(e.target.value)} /></Field>
              {regType === 0 && <div className="span-2"><Switch label="المتدرب وقّع اتفاقية التدريب" checked={contract} onChange={setContract} /></div>}
            </div>
          )}

          {/* ---------- 3. Items ---------- */}
          {step === 'items' && prep && (
            <div className="grid" style={{ gap: 18 }}>
              {prep.student.foreign && <div className="alert info">الطالب أجنبي: يتم احتساب ضريبة القيمة المضافة 15% على البنود.</div>}
              <ItemList title={regType === 1 ? 'بنود الدورة' : 'الدبلومات'} rows={items} picked={picked} setPicked={setPicked} editable={prep.zeroAmount} single={regType === 0} seats={regType === 0 ? seats : null} locked={regType === 1} />
              {prep.setting.chkOtherFees && <ItemList title="رسوم الاختبار والشهادة" rows={prep.fees} picked={pickedFees} setPicked={setPickedFees} editable={prep.zeroAmount} />}
              <div className="totals">
                <div><small>الإجمالي قبل الضريبة</small><b>{money(totals.cost)}</b></div>
                <div><small>الضريبة</small><b>{money(totals.tax)}</b></div>
                <div className="grand"><small>الإجمالي</small><b>{money(totals.total)}</b></div>
              </div>
            </div>
          )}

          {/* ---------- 4. Down payment ---------- */}
          {step === 'pay' && prep && (
            <div className="grid cols-2">
              <div className="span-all alert info"><Receipt size={16} /> سيتم إصدار فاتورة مبيعات ({prep.setting.docPayName}) وسند قبض بالمبلغ المدفوع{prep.setting.amountPay ? `، الحد الأدنى للدفعة ${money(prep.setting.amountPay)} ر.س` : ''}.</div>
              <Field label="الخزينة / البنك" required>
                <select className="input" value={pay.cash} onChange={e => { const c = prep.cashBoxes.find(x => x.Guid === e.target.value); setPay(p => ({ ...p, cash: e.target.value, cost: c?.CostCenterGuid ?? p.cost, costName: c?.CostName ?? p.costName })) }}>
                  <option value="">اختر...</option>{prep.cashBoxes.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
                </select>
              </Field>
              <Field label="مركز التكلفة" required>
                <button type="button" className={`input pick-input ${pay.cost ? '' : 'empty'}`} onClick={() => setPickCost(1)}>{pay.costName || 'اختر...'}<Search size={14} /></button>
              </Field>
              <Field label="رقم المرجع / الحوالة" required><input className="input" dir="ltr" maxLength={50} value={pay.ref} onChange={e => setPay(p => ({ ...p, ref: e.target.value }))} /></Field>
              <Field label="تاريخ السداد / الحوالة" required><input type="date" className="input" value={pay.date} onChange={e => setPay(p => ({ ...p, date: e.target.value }))} /></Field>
              <div className="span-all mini-table bal">
                <div className="mt-head"><span>البند</span><span>قيمة البند</span><span>المدفوع الآن</span></div>
                {selDips.map(d => (
                  <div key={d.diplomGuid} className="mt-row">
                    <span>{d.name} <small className="muted">(ضريبة {d.payTaxRate}%)</small></span>
                    <span className="num-strong" dir="ltr">{money(d.total)}</span>
                    <input className="input" dir="ltr" inputMode="decimal" placeholder={prep.setting.amountPay ? String(prep.setting.amountPay) : '0'} value={pay.lines[d.diplomGuid] ?? ''}
                      onChange={e => setPay(p => ({ ...p, lines: { ...p.lines, [d.diplomGuid]: e.target.value.replace(/[^\d.]/g, '') } }))} />
                  </div>
                ))}
              </div>
              <div className="span-all totals">
                <div><small>قبل الضريبة</small><b>{money(r2(paySub - payTax))}</b></div><div><small>الضريبة</small><b>{money(payTax)}</b></div>
                <div className="grand"><small>إجمالي الفاتورة</small><b>{money(paySub)}</b></div>
              </div>
              <Field label="ملاحظات الفاتورة" className="span-2"><input className="input" maxLength={250} value={pay.notes} onChange={e => setPay(p => ({ ...p, notes: e.target.value }))} /></Field>
              <div className="span-2"><Switch label="تقسيم السداد على بنك / خزينة ثانية" checked={pay.split} onChange={v => setPay(p => ({ ...p, split: v }))} /></div>
              {pay.split && <>
                <Field label="مبلغ الحوالة الأولى" required><input className="input" dir="ltr" inputMode="decimal" value={pay.amount1} onChange={e => setPay(p => ({ ...p, amount1: e.target.value.replace(/[^\d.]/g, '') }))} /></Field>
                <Field label="مبلغ الحوالة الثانية" required hint={`يجب أن يساوي المجموع ${money(paySub)}`}><input className="input" dir="ltr" inputMode="decimal" value={pay.amount2} onChange={e => setPay(p => ({ ...p, amount2: e.target.value.replace(/[^\d.]/g, '') }))} /></Field>
                <Field label="الخزينة / البنك 2" required>
                  <select className="input" value={pay.cash2} onChange={e => { const c = prep.cashBoxes.find(x => x.Guid === e.target.value); setPay(p => ({ ...p, cash2: e.target.value, cost2: c?.CostCenterGuid ?? p.cost2, costName2: c?.CostName ?? p.costName2 })) }}>
                    <option value="">اختر...</option>{prep.cashBoxes.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
                  </select>
                </Field>
                <Field label="مركز التكلفة 2" required><button type="button" className={`input pick-input ${pay.cost2 ? '' : 'empty'}`} onClick={() => setPickCost(2)}>{pay.costName2 || 'اختر...'}<Search size={14} /></button></Field>
                <Field label="رقم المرجع 2" required><input className="input" dir="ltr" maxLength={50} value={pay.ref2} onChange={e => setPay(p => ({ ...p, ref2: e.target.value }))} /></Field>
                <Field label="تاريخ الحوالة 2" required><input type="date" className="input" value={pay.date2} onChange={e => setPay(p => ({ ...p, date2: e.target.value }))} /></Field>
              </>}
            </div>
          )}

          {/* ---------- 5. Attachments ---------- */}
          {step === 'files' && prep && (
            <div className="grid" style={{ gap: 14 }}>
              <p className="muted" style={{ margin: 0 }}>يتم رفع الملفات على خادم الأرشيف باسم <code dir="ltr">نوع_رقم الهوية</code> كما في البرنامج السابق. المسموح: PDF أو صور حتى 10 ميجابايت.</p>
              {prep.attachments.map(a => {
                const done = uploaded.includes(a.code), req = required(a.code)
                return (
                  <div key={a.code} className={`attach-row ${done ? 'done' : ''}`}>
                    <span className="ic">{done ? <FileCheck2 size={20} /> : <FileUp size={20} />}</span>
                    <div className="who"><b>{a.name}</b><small>{done ? 'تم الرفع' : req ? 'مطلوب' : a.rule ?? 'اختياري'}</small></div>
                    {!done && <label className="btn secondary sm"><Upload size={14} /> {files[a.code]?.name ?? 'اختيار ملف'}
                      <input type="file" hidden accept=".pdf,image/*" onChange={e => setFiles(f => ({ ...f, [a.code]: e.target.files?.[0] ?? null }))} /></label>}
                    {req && !done && <span className="badge off">مطلوب</span>}
                  </div>
                )
              })}
              <div className="row"><button className="btn" onClick={upload} disabled={uploading || !Object.values(files).some(Boolean)}>{uploading ? <Spinner /> : <><Upload size={16} /> رفع الملفات المختارة</>}</button>
                {missingFiles.length > 0 && <span className="muted">متبقي: {missingFiles.map(f => f.name).join('، ')}</span>}</div>
            </div>
          )}

          {/* ---------- 6. Review ---------- */}
          {step === 'review' && prep && (
            <div className="review">
              <div className="rv"><small>الطالب</small><b>{student.StudentName}</b><span dir="ltr">{student.NationalId}</span></div>
              <div className="rv"><small>الدفتر</small><b>{prep.setting.docName}</b><span>{prep.setting.branchName}</span></div>
              <div className="rv"><small>{regType === 0 ? 'الدفعة / المستوى' : 'الدورة'}</small>
                <b>{regType === 0 ? prep.batches.find(b => b.Guid === batch)?.BatchName : prep.courses.find(c => c.Guid === course)?.Name}</b>
                <span>{regType === 0 ? prep.levels.find(l => l.Guid === level)?.Name : ''}</span></div>
              <div className="rv"><small>مندوب البيع</small><b>{prep.sellers.find(s => s.Guid === seller)?.ManFullName}</b><span>{contract ? 'وقّع اتفاقية التدريب' : 'لم يوقّع الاتفاقية بعد (مهلة 30 يوماً)'}</span></div>
              <div className="rv span"><small>البنود</small>{all.map(x => <div key={x.diplomGuid} className="row" style={{ justifyContent: 'space-between' }}><span>{x.name}</span><b className="num-strong">{money(x.total)}</b></div>)}</div>
              {prep.setting.chkStartPay && <div className="rv span"><small>الدفعة المقدمة</small><b className="num-strong">{money(paySub)} ر.س</b><span>{cash?.Name}{pay.split ? ` + ${cash2?.Name}` : ''} • مرجع {pay.ref}</span></div>}
              <div className="totals span"><div className="grand"><small>إجمالي الاستمارة</small><b>{money(totals.total)}</b></div></div>
            </div>
          )}
        </motion.div>
      </AnimatePresence>
      <AnimatePresence>
        {pickCost && <CostCenterPicker onClose={() => setPickCost(null)} onPick={x => { setPay(p => (pickCost === 1 ? { ...p, cost: x.guid, costName: x.name } : { ...p, cost2: x.guid, costName2: x.name })); setPickCost(null) }} />}
      </AnimatePresence>
    </Drawer>
  )
}

type Seat = { capacity: number; registered: number; available: number }

function ItemList({ title, rows, picked, setPicked, editable, single, seats, locked }: {
  title: string; rows: PriceRow[]; picked: Record<string, number | null>; setPicked: (f: (p: Record<string, number | null>) => Record<string, number | null>) => void
  editable: boolean; single?: boolean; seats?: Record<string, Seat> | null; locked?: boolean
}) {
  const [q, setQ] = useState('')
  // With seats: diplomas that have seats first, full / not offered in this batch last.
  const free = (r: PriceRow) => !seats || (seats[r.diplomGuid]?.available ?? 0) > 0
  const shown = rows.filter(r => !q || r.name.includes(q)).sort((a, b) => Number(free(b)) - Number(free(a)))
  const toggle = (r: PriceRow, on: boolean) => {
    if (locked) return
    if (!free(r)) {
      const st = seats?.[r.diplomGuid]
      alerts.warning(st ? `الدبلوم مكتمل في هذه الدفعة (${st.registered} من ${st.capacity})` : 'هذا الدبلوم غير متاح للتسجيل في الدفعة المختارة (لا توجد مقاعد محددة له)', 'لا توجد مقاعد')
      return
    }
    setPicked(p => { if (single) return on ? {} : { [r.diplomGuid]: null }; const n = { ...p }; if (on) delete n[r.diplomGuid]; else n[r.diplomGuid] = null; return n })
  }
  return (
    <div>
      <div className="lines-head"><h4>{title}{single && <small className="muted" style={{ fontWeight: 500, marginInlineStart: 8 }}>اختر دبلوماً واحداً</small>}</h4>{rows.length > 6 && <div className="input-wrap" style={{ marginInlineStart: 'auto', width: 240 }}><Search size={15} /><input className="input" placeholder="بحث..." value={q} onChange={e => setQ(e.target.value)} /></div>}</div>
      <div className="item-list">
        {shown.length === 0 && <div className="empty"><b>لا توجد بنود في قائمة الأسعار</b></div>}
        {shown.map(r => {
          const on = r.diplomGuid in picked
          return (
            <div key={r.diplomGuid} className={`item ${on ? 'on' : ''} ${free(r) ? '' : 'full'}`} onClick={() => toggle(r, on)}>
              <span className={single ? 'radio-lg' : 'cbx-lg'}>{on && (single ? <i /> : <Check size={15} />)}</span>
              <div className="nm"><b>{r.name}</b><small>كود {r.code} • قبل الضريبة {money(r.cost)} • ضريبة {money(r.tax)}</small></div>
              {seats && <SeatBadge s={seats[r.diplomGuid]} />}
              {on && editable ? (
                <input className="input price" dir="ltr" inputMode="decimal" value={picked[r.diplomGuid] ?? r.total} onClick={e => e.stopPropagation()} title="تعديل القيمة (صلاحية المبالغ الصفرية)"
                  onChange={e => { const v = e.target.value.replace(/[^\d.]/g, ''); setPicked(p => ({ ...p, [r.diplomGuid]: v === '' ? 0 : Number(v) })) }} />
              ) : <b className="num-strong price-tag">{money(r.total)}</b>}
            </div>
          )
        })}
      </div>
    </div>
  )
}

function SeatBadge({ s }: { s?: Seat }) {
  if (!s) return <span className="seat-badge none">غير متاح بالدفعة</span>
  if (s.available <= 0) return <span className="seat-badge none">مكتمل {s.registered}/{s.capacity}</span>
  return <span className={`seat-badge ${s.available <= 3 ? 'low' : ''}`}>{s.available} مقعد متاح</span>
}
