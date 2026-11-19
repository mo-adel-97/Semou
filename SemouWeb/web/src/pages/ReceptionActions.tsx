import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { motion } from 'motion/react'
import { BookOpenText, CalendarDays, FileClock, History, Link2, UserRound } from 'lucide-react'
import { api } from '../api'
import { alerts, notify } from '../lib/alerts'
import { Drawer, Field, Spinner } from '../components/ui'

/* Reception side actions: study info + class, registration case, suspend / activate, document history. */
const esc = (t: string) => t.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))
const day = (v?: string | null) => (!v ? '—' : /^\d{4}-\d{2}-\d{2}/.test(v) ? new Date(v).toLocaleDateString('ar-EG', { year: 'numeric', month: 'long', day: 'numeric' }) : v)

export interface FileOption { FileId: number | null; DiplomName: string | null; RegType: number | null; StudyBranchName: string | null; LevelActive: boolean }

/** When a student has several training files, ask which one (SweetAlert select). */
export async function pickFile<T extends FileOption>(files: T[], title = 'اختر الملف التدريبي'): Promise<T | null> {
  const list = files.filter(f => f.FileId)
  if (list.length <= 1) return list[0] ?? null
  const v = await alerts.form<number>({
    title, confirmText: 'متابعة',
    html: `<div class="sm-form"><select id="pf" class="input">${list.map(f => `<option value="${f.FileId}">${esc(f.DiplomName ?? '—')} • ${f.RegType === 1 ? 'دورة' : 'دبلوم'} • ${esc(f.StudyBranchName ?? '')}${f.LevelActive ? '' : ' (منتهٍ)'}</option>`).join('')}</select></div>`,
    read: el => Number((el.querySelector('#pf') as HTMLSelectElement).value),
  })
  return v == null ? null : list.find(f => f.FileId === v) ?? null
}

/** تغيير حالة التسجيل: the student's case and (optionally) the chosen training file. */
export async function changeCase(account: string, cases: { Guid: string; Name: string }[], files: FileOption[], onDone: () => void) {
  const withFiles = files.filter(f => f.FileId)
  const v = await alerts.form<{ caseGuid: string; fileId: number | null; reason: string }>({
    title: 'تغيير حالة التسجيل', confirmText: 'حفظ',
    html: `<div class="sm-form">
      <label class="sm-lbl">الحالة الجديدة</label>
      <select id="cc" class="input"><option value="">— اختر —</option>${cases.map(c => `<option value="${c.Guid}">${esc(c.Name)}</option>`).join('')}</select>
      ${withFiles.length ? `<label class="sm-lbl">تطبيق الحالة على الملف التدريبي</label>
        <select id="cf" class="input">${withFiles.map(f => `<option value="${f.FileId}">${esc(f.DiplomName ?? '—')} • ${esc(f.StudyBranchName ?? '')}</option>`).join('')}<option value="">بيانات الطالب فقط</option></select>` : ''}
      <label class="sm-lbl">سبب التغيير</label>
      <textarea id="cr" class="input" maxlength="250" placeholder="يُسجَّل السبب في سجل حركات المستخدمين"></textarea>
    </div>`,
    read: el => {
      const caseGuid = (el.querySelector('#cc') as HTMLSelectElement).value
      const f = el.querySelector('#cf') as HTMLSelectElement | null
      const reason = (el.querySelector('#cr') as HTMLTextAreaElement).value.trim()
      if (!caseGuid) return 'يرجى اختيار حالة التسجيل'
      if (!reason) return 'يرجى إدخال سبب تغيير الحالة'
      return { caseGuid, fileId: f?.value ? Number(f.value) : null, reason }
    },
  })
  if (!v) return
  try {
    const r = await api<{ message: string }>(`/reception/students/${account}/case`, { method: 'PUT', json: v })
    notify.success(r.message); onDone()
  } catch (e) { alerts.error((e as Error).message) }
}

/** إيقاف / تفعيل الطالب (ChangeStudentStaut). */
export async function toggleActive(account: string, name: string, isUse: boolean, onDone: () => void) {
  const next = !isUse
  const ok = await alerts.confirm({
    title: next ? `تفعيل الطالب ${name}؟` : `إيقاف الطالب ${name} عن التعامل؟`,
    text: next ? 'سيتمكن الموظفون من إجراء العمليات على الطالب مرة أخرى.' : 'لن يمكن تسجيل الطالب أو إجراء أي عملية سداد له حتى يتم تفعيله.',
    confirmText: next ? 'تفعيل' : 'إيقاف', danger: !next,
  })
  if (!ok) return
  const reason = await alerts.reason(next ? 'سبب تفعيل الطالب' : 'سبب إيقاف الطالب')
  if (!reason) return
  try {
    const r = await api<{ message: string }>(`/reception/students/${account}/active`, { method: 'PUT', json: { isUse: next, reason } })
    notify.success(r.message); onDone()
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------------- بيانات الدراسة ---------------- */
interface Study {
  file: { FileId: number; RegType: number; DiplomName: string | null; BranchName: string | null; BatchName: string | null; LevelName: string | null; CourseName: string | null
    DateStart: string | null; DateEnd: string | null; CaseName: string | null; Notes: string | null; ClassGuid: string | null; ClassName: string | null
    Active: boolean; RegCode: string | null; RegDate: string | null; StudentName: string; NationalId: string }
  classes: { Guid: string; Name: string | null; Code: string }[]
}

export function StudyInfo({ fileId, onClose }: { fileId: number; onClose: () => void }) {
  const qc = useQueryClient()
  const q = useQuery({ queryKey: ['reception', 'study', fileId], queryFn: () => api<Study>(`/reception/files/${fileId}/study`), staleTime: 0 })
  const [cls, setCls] = useState<string | null>(null)
  const save = useMutation({
    mutationFn: (classGuid: string) => api<{ message: string }>(`/reception/files/${fileId}/class`, { method: 'PUT', json: { classGuid } }),
    onSuccess: r => { notify.success(r.message); qc.invalidateQueries({ queryKey: ['reception', 'study', fileId] }) },
  })
  const f = q.data?.file
  const current = cls ?? f?.ClassGuid ?? ''
  const isCourse = f?.RegType === 1
  return (
    <Drawer onClose={onClose} icon={<BookOpenText size={22} />} title="بيانات الدراسة" subtitle={f ? `${f.StudentName} • ${f.DiplomName ?? ''}` : 'جارٍ التحميل...'}
      footer={<button className="btn ghost" onClick={onClose}>إغلاق</button>}>
      {!f ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 16 }}>
          <div className="study-hero">
            <span className="ic"><BookOpenText size={22} /></span>
            <div><b>{f.DiplomName ?? f.CourseName}</b><small>{isCourse ? 'دورة تدريبية' : 'دبلوم'} • {f.BranchName}{isCourse && f.CourseName ? <> • كود الدورة <span dir="ltr">{f.CourseName}</span></> : null}</small></div>
            <span className={`badge ${f.Active ? 'ok' : 'warn'}`}>{f.Active ? 'على رأس الدراسة' : 'أنهى الدراسة'}</span>
          </div>
          <div className="fee-info">
            {isCourse ? <>
              <span><small>تاريخ البداية</small><b>{day(f.DateStart)}</b></span>
              <span><small>تاريخ النهاية</small><b>{day(f.DateEnd)}</b></span>
            </> : <>
              <span><small>الدفعة</small><b>{f.BatchName ?? '—'}</b></span>
              <span><small>المستوى</small><b>{f.LevelName ?? '—'}</b></span>
            </>}
            <span><small>حالة التسجيل</small><b>{f.CaseName ?? '—'}</b></span>
            <span><small>استمارة التسجيل</small><b>{f.RegCode ? `#${f.RegCode}` : '—'}</b></span>
          </div>
          {f.Notes && <div className="alert info">{f.Notes}</div>}
          {!isCourse && (
            <Field label="الشعبة" hint={q.data!.classes.length ? 'اختر الشعبة ثم اضغط ربط' : 'لا توجد شعب معرّفة لهذا الدبلوم والمستوى في الفرع'}>
              <div style={{ display: 'flex', gap: 8 }}>
                <select className="input" value={current} onChange={e => setCls(e.target.value)} disabled={!q.data!.classes.length}>
                  <option value="">— غير مربوط بشعبة —</option>
                  {q.data!.classes.map(c => <option key={c.Guid} value={c.Guid}>{c.Name ?? c.Code}</option>)}
                </select>
                <button className="btn" disabled={!current || current === f.ClassGuid || save.isPending} onClick={() => save.mutate(current)}>
                  {save.isPending ? <Spinner /> : <><Link2 size={16} /> ربط</>}
                </button>
              </div>
            </Field>
          )}
          {isCourse && <div className="alert info">ملف الدورة التدريبية لا يُربط بشعبة.</div>}
        </div>
      )}
    </Drawer>
  )
}

/* ---------------- سجل المستندات ---------------- */
interface HistoryRow { date: string; time: string | null; userName: string | null; action: string | null; reason: string | null; code: string | null; docType: string | null }

export function DocHistory({ account, name, onClose }: { account: string; name: string; onClose: () => void }) {
  const q = useQuery({ queryKey: ['reception', 'history', account], queryFn: () => api<HistoryRow[]>(`/reception/students/${account}/history`), staleTime: 0 })
  const time = (r: HistoryRow) => (r.time ? new Date(r.time).toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' }) : '')
  return (
    <Drawer size="lg" onClose={onClose} icon={<FileClock size={22} />} title="سجل المستندات" subtitle={`${name} • كل العمليات التي تمت على الطالب ومستنداته`}
      footer={<button className="btn ghost" onClick={onClose}>إغلاق</button>}>
      {q.isPending ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div>
        : !q.data?.length ? <div className="empty"><History size={30} /><b>لا توجد عمليات مسجلة بعد</b></div> : (
          <div className="timeline">
            {q.data.map((r, i) => (
              <motion.div key={i} className="tl-item" initial={{ opacity: 0, x: 8 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: Math.min(i, 20) * 0.02 }}>
                <span className={`tl-dot ${/حذف|إلغاء|ايقاف|إيقاف/.test(r.action ?? '') ? 'bad' : /تعديل|تغيير/.test(r.action ?? '') ? 'warn' : ''}`} />
                <div className="tl-body">
                  <div className="tl-top"><b>{r.action ?? '—'}</b>{r.docType && <span className="badge muted">{r.docType}{r.code ? ` #${r.code}` : ''}</span>}</div>
                  {r.reason && <p className="tl-reason">السبب: {r.reason}</p>}
                  <small><UserRound size={12} /> {r.userName ?? '—'} &nbsp; <CalendarDays size={12} /> {day(r.date)} {time(r)}</small>
                </div>
              </motion.div>
            ))}
          </div>
        )}
    </Drawer>
  )
}

