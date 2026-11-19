import { useMemo, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { Globe2, Inbox, Laptop, MessageCircle, Phone, RefreshCw, School, Search, UserCheck, X } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { PageHead } from '../components/Shell'
import { Segmented, Spinner } from '../components/ui'

/*
 * المبيعات ← طلبات الدراسة: every "سجلي الآن" request from alsmow.com, pulled automatically every minute
 * (Website sync in the API). Split حضوري / عن بعد; sales update the status and write follow-up notes.
 */
interface Row {
  guid: string; id: number; requestDate: string; studentName: string; phone: string | null; email: string | null; nationalId: string | null
  city: string | null; program: string | null; studyMode: number; notes: string | null; status: number; followNotes: string | null
  studentGuid: string | null; existingStudent: string | null; updatedAt: string | null; updatedBy: string | null
}
interface Data {
  rows: Row[]; counts: { mode: number; status: number; n: number }[]; statuses: string[]
  sync: { enabled: boolean; lastPull: string | null; lastPush: string | null }
}

const MODES = [{ v: 1, label: 'حضوري', icon: School }, { v: 2, label: 'عن بعد', icon: Laptop }, { v: 0, label: 'غير محدد', icon: Globe2 }]
const TONES = ['warn', 'info', 'ok', 'off']
const dt = new Intl.DateTimeFormat('ar-SA-u-ca-gregory-nu-latn', { day: 'numeric', month: 'short', hour: 'numeric', minute: '2-digit' })
const fmt = (d: string | null) => (d ? dt.format(new Date(d)) : '')
const wa = (p: string | null) => {
  const d = (p ?? '').replace(/\D+/g, '')
  return d.startsWith('966') ? d : d.startsWith('05') ? `966${d.slice(1)}` : d
}

export function StudyRequests({ group }: { group?: string }) {
  const { can } = useAuth()
  const canEdit = can('webstudyrequest', 'edit')
  const qc = useQueryClient()
  const [mode, setMode] = useState<number>(1)
  const [status, setStatus] = useState<number | null>(null)
  const [q, setQ] = useState('')
  const [open, setOpen] = useState<Row | null>(null)

  const params = { mode, status: status ?? undefined, q: q.trim() || undefined }
  const { data, isLoading, isFetching } = useQuery({
    queryKey: ['study-requests', params],
    queryFn: () => api<Data>(`/study-requests${qs(params)}`),
    placeholderData: keepPreviousData,
    refetchInterval: 60_000,
  })
  const sync = useMutation({
    mutationFn: () => api<{ message: string }>('/study-requests/sync', { method: 'POST' }),
    onSuccess: (r) => { notify.success(r.message); qc.invalidateQueries({ queryKey: ['study-requests'] }); qc.invalidateQueries({ queryKey: ['requests', 'counts'] }) },
  })

  const countOf = (m: number, s?: number) => (data?.counts ?? []).filter(c => c.mode === m && (s === undefined || c.status === s)).reduce((t, c) => t + c.n, 0)
  const statuses = data?.statuses ?? ['جديد', 'جاري التواصل', 'تم التسجيل', 'غير مهتم']

  return (
    <div className="sr">
      <style>{CSS}</style>
      <PageHead
        title="طلبات الدراسة"
        subtitle="الطلبات الواردة من موقع السمو (سجلي الآن) — تصل تلقائيًا كل دقيقة"
        group={group}
        actions={
          <button className="btn secondary" onClick={() => sync.mutate()} disabled={sync.isPending}>
            {sync.isPending ? <Spinner /> : <RefreshCw size={16} />} تحديث الآن
          </button>
        }
      />

      <div className="sr-modes">
        {MODES.filter(m => m.v !== 0 || countOf(0) > 0).map(m => (
          <button key={m.v} className={`sr-mode ${mode === m.v ? 'on' : ''}`} onClick={() => { setMode(m.v); setStatus(null) }}>
            <span className="sr-mode-ic"><m.icon size={22} /></span>
            <span className="sr-mode-t">
              <b>{m.label}</b>
              <small>{countOf(m.v).toLocaleString('ar-EG')} طلب{countOf(m.v, 0) > 0 && <> · <em>{countOf(m.v, 0).toLocaleString('ar-EG')} جديد</em></>}</small>
            </span>
            {mode === m.v && <motion.span layoutId="sr-mode-bar" className="sr-mode-bar" />}
          </button>
        ))}
      </div>

      <div className="sr-tools">
        <Segmented<number | null>
          value={status}
          onChange={setStatus}
          options={[{ value: null, label: 'الكل' }, ...statuses.map((s, i) => ({ value: i, label: `${s} (${countOf(mode, i)})` }))]}
        />
        <label className="sr-search">
          <Search size={16} />
          <input className="input" placeholder="بحث بالاسم أو الجوال أو الهوية أو البرنامج" value={q} onChange={e => setQ(e.target.value)} />
        </label>
        {isFetching && !isLoading && <Spinner size={16} />}
      </div>

      {isLoading ? (
        <div className="sr-list">{Array.from({ length: 5 }, (_, i) => <div key={i} className="sk" style={{ height: 76, borderRadius: 16 }} />)}</div>
      ) : !data?.rows.length ? (
        <div className="sr-empty"><Inbox size={40} /><b>لا توجد طلبات</b><span>ستظهر هنا طلبات «سجلي الآن» القادمة من الموقع تلقائيًا</span></div>
      ) : (
        <motion.div className="sr-list" initial="h" animate="s" variants={{ s: { transition: { staggerChildren: 0.03 } } }}>
          {data.rows.map(r => (
            <motion.button key={r.guid} variants={{ h: { opacity: 0, y: 10 }, s: { opacity: 1, y: 0 } }} className={`sr-row ${r.status === 0 ? 'new' : ''}`} onClick={() => setOpen(r)}>
              <span className="sr-av">{r.studentName.trim()[0]}</span>
              <span className="sr-main">
                <b>{r.studentName}{r.existingStudent && <span className="badge info" title={`طالبة مسجلة: ${r.existingStudent}`}><UserCheck size={12} /> مسجلة سابقًا</span>}</b>
                <small>{r.program || 'لم تحدد البرنامج'}{r.city && ` · ${r.city}`}</small>
              </span>
              <span className="sr-phone" dir="ltr">{r.phone}</span>
              <span className={`badge ${TONES[r.status]}`}>{statuses[r.status]}</span>
              <span className="sr-date">{fmt(r.requestDate)}</span>
            </motion.button>
          ))}
        </motion.div>
      )}

      {data && (
        <p className="sr-sync">
          {data.sync.enabled
            ? <>آخر استلام من الموقع: {fmt(data.sync.lastPull) || '—'} · آخر تحديث لبوابة المتدربات: {fmt(data.sync.lastPush) || '—'}</>
            : <>الربط مع الموقع غير مفعل على هذا الخادم (Website:Enabled)</>}
        </p>
      )}

      <AnimatePresence>
        {open && <Detail key={open.guid} row={open} statuses={statuses} canEdit={canEdit} onClose={() => setOpen(null)} onSaved={() => { setOpen(null); qc.invalidateQueries({ queryKey: ['study-requests'] }); qc.invalidateQueries({ queryKey: ['requests', 'counts'] }) }} />}
      </AnimatePresence>
    </div>
  )
}

function Detail({ row, statuses, canEdit, onClose, onSaved }: { row: Row; statuses: string[]; canEdit: boolean; onClose: () => void; onSaved: () => void }) {
  const [status, setStatus] = useState(row.status === 0 && canEdit ? 1 : row.status)
  const [notes, setNotes] = useState(row.followNotes ?? '')
  const save = useMutation({
    mutationFn: () => api<{ message: string }>(`/study-requests/${row.guid}`, { method: 'PUT', json: { status, followNotes: notes } }),
    onSuccess: (r) => { alerts.success(r.message); onSaved() },
  })
  const kv = useMemo(() => [
    ['طريقة الدراسة', MODES.find(m => m.v === row.studyMode)?.label],
    ['البرنامج', row.program],
    ['الجوال', row.phone],
    ['رقم الهوية', row.nationalId],
    ['البريد', row.email],
    ['المدينة', row.city],
    ['تاريخ الطلب', fmt(row.requestDate)],
    ['رقم الطلب', `WSR-${row.id}`],
  ].filter(x => x[1]) as [string, string][], [row])

  return (
    <>
      <motion.div className="sr-ov" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} />
      <motion.aside className="sr-drawer" initial={{ x: '-100%' }} animate={{ x: 0 }} exit={{ x: '-100%' }} transition={{ type: 'spring', damping: 32, stiffness: 320 }}>
        <header>
          <div>
            <h2>{row.studentName}</h2>
            <small>طلب دراسة من الموقع</small>
          </div>
          <button className="btn ghost" onClick={onClose} aria-label="إغلاق"><X size={18} /></button>
        </header>
        <div className="sr-body">
          {row.existingStudent && <div className="sr-note ok"><UserCheck size={16} /> رقم الهوية مسجل لطالبة في النظام: <b>{row.existingStudent}</b></div>}
          <dl className="sr-kv">{kv.map(([k, v]) => <div key={k}><dt>{k}</dt><dd dir={k === 'الجوال' || k === 'رقم الهوية' ? 'ltr' : undefined}>{v}</dd></div>)}</dl>
          {row.notes && <div className="sr-msg"><span>ملاحظة الطالبة</span><p>{row.notes}</p></div>}
          {row.phone && (
            <div className="sr-actions">
              <a className="btn" style={{ background: '#25D366', borderColor: '#25D366' }} target="_blank" rel="noopener noreferrer"
                href={`https://wa.me/${wa(row.phone)}?text=${encodeURIComponent(`مرحبًا ${row.studentName}، معك معهد السمو للتدريب بخصوص طلبك للتسجيل في ${row.program ?? 'برامجنا'}.`)}`}>
                <MessageCircle size={16} /> واتساب
              </a>
              <a className="btn secondary" href={`tel:${row.phone}`}><Phone size={16} /> اتصال</a>
            </div>
          )}
          <div className="sr-sec">
            <label>حالة الطلب</label>
            <div className="sr-status">
              {statuses.map((s, i) => (
                <button key={s} disabled={!canEdit} className={`sr-st ${status === i ? 'on ' + TONES[i] : ''}`} onClick={() => setStatus(i)}>{s}</button>
              ))}
            </div>
          </div>
          <div className="sr-sec">
            <label>ملاحظات المتابعة</label>
            <textarea className="input" rows={4} maxLength={1000} disabled={!canEdit} value={notes} onChange={e => setNotes(e.target.value)} placeholder="مثال: تم التواصل، ستحضر للمعهد يوم الأحد لإكمال التسجيل" />
          </div>
          {row.updatedBy && <p className="muted" style={{ fontSize: 12 }}>آخر تحديث: {row.updatedBy} · {fmt(row.updatedAt)}</p>}
        </div>
        {canEdit && (
          <footer>
            <button className="btn" onClick={() => save.mutate()} disabled={save.isPending}>{save.isPending && <Spinner />} حفظ المتابعة</button>
          </footer>
        )}
      </motion.aside>
    </>
  )
}

const CSS = `
.sr-modes{display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:12px;margin-bottom:16px}
.sr-mode{position:relative;display:flex;align-items:center;gap:14px;padding:16px 18px;border-radius:18px;border:1px solid var(--line);background:var(--surface);text-align:start;cursor:pointer;transition:.2s;overflow:hidden}
.sr-mode:hover{border-color:var(--brand-200)}
.sr-mode.on{border-color:var(--brand);box-shadow:var(--ring)}
.sr-mode-ic{display:grid;place-items:center;width:48px;height:48px;border-radius:14px;background:var(--brand-50);color:var(--brand-ink)}
.sr-mode.on .sr-mode-ic{background:var(--brand);color:#fff}
.sr-mode-t b{display:block;font-size:17px;color:var(--ink)}
.sr-mode-t small{color:var(--muted)} .sr-mode-t em{font-style:normal;color:var(--warn);font-weight:700}
.sr-mode-bar{position:absolute;inset-inline:0;bottom:0;height:3px;background:var(--brand)}
.sr-tools{display:flex;flex-wrap:wrap;align-items:center;gap:10px;margin-bottom:14px}
.sr-search{position:relative;flex:1;min-width:220px}
.sr-search svg{position:absolute;inset-inline-start:12px;top:50%;transform:translateY(-50%);color:var(--muted)}
.sr-search .input{padding-inline-start:36px}
.sr-list{display:grid;gap:8px}
.sr-row{display:grid;grid-template-columns:auto 1fr auto auto auto;align-items:center;gap:14px;padding:12px 16px;border-radius:16px;border:1px solid var(--line);background:var(--surface);text-align:start;cursor:pointer;transition:.15s}
.sr-row:hover{border-color:var(--brand-200);transform:translateY(-1px);box-shadow:var(--sh-2)}
.sr-row.new{border-inline-start:4px solid var(--warn)}
.sr-av{display:grid;place-items:center;width:42px;height:42px;border-radius:50%;background:var(--brand-50);color:var(--brand-ink);font-weight:800}
.sr-main{min-width:0} .sr-main b{display:flex;align-items:center;gap:8px;color:var(--ink)} .sr-main small{display:block;color:var(--muted);white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
.sr-phone{font-variant-numeric:tabular-nums;color:var(--ink-2)} .sr-date{font-size:12px;color:var(--muted);white-space:nowrap}
.sr-empty{display:flex;flex-direction:column;align-items:center;gap:6px;padding:60px 20px;border:1px dashed var(--line);border-radius:18px;color:var(--muted)} .sr-empty b{color:var(--ink);font-size:17px}
.sr-sync{margin-top:14px;font-size:12px;color:var(--muted)}
.sr-ov{position:fixed;inset:0;background:var(--overlay);z-index:60}
.sr-drawer{position:fixed;inset-block:0;left:0;width:min(440px,100%);background:var(--surface);z-index:61;display:flex;flex-direction:column;box-shadow:var(--sh-3)}
.sr-drawer header,.sr-drawer footer{display:flex;align-items:center;justify-content:space-between;gap:10px;padding:16px 20px;border-bottom:1px solid var(--line)}
.sr-drawer footer{border-bottom:0;border-top:1px solid var(--line)} .sr-drawer footer .btn{width:100%}
.sr-drawer h2{margin:0;font-size:20px} .sr-drawer header small{color:var(--muted)}
.sr-body{flex:1;overflow:auto;padding:20px;display:grid;gap:16px;align-content:start}
.sr-kv{display:grid;grid-template-columns:1fr 1fr;gap:10px;margin:0;padding:14px;border-radius:14px;background:var(--bg-2)}
.sr-kv dt{font-size:12px;color:var(--muted)} .sr-kv dd{margin:0;font-weight:700;color:var(--ink);word-break:break-word}
.sr-msg{padding:12px 14px;border-radius:14px;border:1px solid var(--line)} .sr-msg span{font-size:12px;color:var(--muted)} .sr-msg p{margin:4px 0 0;white-space:pre-wrap}
.sr-note{display:flex;align-items:center;gap:8px;padding:10px 12px;border-radius:12px;background:var(--brand-50);color:var(--brand-ink);font-size:14px}
.sr-actions{display:grid;grid-template-columns:1fr 1fr;gap:8px} .sr-actions .btn{justify-content:center}
.sr-sec label{display:block;margin-bottom:6px;font-size:13px;font-weight:700;color:var(--ink-2)}
.sr-status{display:grid;grid-template-columns:1fr 1fr;gap:6px}
.sr-st{padding:9px;border-radius:12px;border:1px solid var(--line);background:var(--surface);font-weight:700;color:var(--ink-2);cursor:pointer}
.sr-st.on{border-color:var(--brand);background:var(--brand-50);color:var(--brand-ink)}
.sr-st:disabled{cursor:not-allowed;opacity:.7}
@media (max-width:720px){.sr-row{grid-template-columns:auto 1fr auto}.sr-phone,.sr-date{display:none}}
`
