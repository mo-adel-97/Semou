import { useState } from 'react'
import { useLocation } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { motion } from 'motion/react'
import { BadgeCheck, Ban, ShieldAlert, ShieldCheck } from 'lucide-react'
import { api } from '../api'
import { Spinner } from '../components/ui'
import { ThemePicker } from '../components/ThemePicker'

/* Public page opened by the QR on any printed document: confirms it was issued by معهد السمو. */
interface Result {
  status: 'valid' | 'cancelled' | 'mismatch' | 'invalid'; issuer: string
  kind?: string; number?: string; date?: string; amount?: number; party?: string | null; partyId?: string | null; branch?: string | null; code?: string
}
const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

export function VerifyDoc() {
  const loc = useLocation()
  const [, , type, guid] = loc.pathname.split('/')
  const [code, setCode] = useState(() => new URLSearchParams(loc.search).get('c') ?? '')
  const [typed, setTyped] = useState(code)
  const q = useQuery({
    queryKey: ['verify', type, guid, code], enabled: !!code,
    queryFn: () => api<Result>(`/verify/${encodeURIComponent(type ?? '')}/${encodeURIComponent(guid ?? '')}?c=${encodeURIComponent(code)}`),
    meta: { silent: true }, retry: false,
  })
  const r = q.data
  const view = !code ? null : q.isPending ? 'loading' : q.isError ? 'error' : r!.status

  return (
    <div className="verify-page">
      <div className="theme-corner"><ThemePicker /></div>
      <motion.div className="verify-card" initial={{ opacity: 0, y: 16 }} animate={{ opacity: 1, y: 0 }}>
        <div className="vf-brand"><img src="/logo.jpeg" alt="" /><div><b>معهد السمو المتخصص العالي للتدريب</b><small>خدمة التحقق من المستندات</small></div></div>

        {view === 'loading' && <div className="vf-state"><Spinner size={30} /><p>جاري التحقق من المستند...</p></div>}

        {(view === 'valid' || view === 'cancelled') && r && (
          <>
            <div className={`vf-state ${view === 'valid' ? 'ok' : 'bad'}`}>
              <span className="vf-ic">{view === 'valid' ? <BadgeCheck size={46} /> : <Ban size={46} />}</span>
              <h1>{view === 'valid' ? 'مستند موثّق وصحيح' : 'مستند ملغي'}</h1>
              <p>{view === 'valid' ? `هذا المستند صادر رسمياً من ${r.issuer} وبياناته مطابقة للسجلات.` : 'هذا المستند صادر من المعهد لكنه ملغي في السجلات ولا يُعتد به.'}</p>
            </div>
            <div className="vf-facts">
              <div><small>نوع المستند</small><b>{r.kind}</b></div>
              <div><small>الرقم</small><b>{r.number}</b></div>
              <div><small>التاريخ</small><b dir="ltr">{r.date}</b></div>
              <div><small>المبلغ</small><b>{money(r.amount ?? 0)} ر.س</b></div>
              {r.party && <div><small>الاسم</small><b>{r.party}</b></div>}
              {r.partyId && <div><small>رقم الهوية</small><b dir="ltr">{r.partyId}</b></div>}
              {r.branch && <div><small>الفرع</small><b>{r.branch}</b></div>}
              <div><small>رمز التحقق</small><b dir="ltr" className="mono">{r.code}</b></div>
            </div>
            <p className="vf-note"><ShieldCheck size={15} /> قارن هذه البيانات بالمستند المطبوع الذي بين يديك، فإن وُجد أي اختلاف في المبلغ أو الاسم فقد تم التلاعب بالمستند المطبوع.</p>
          </>
        )}

        {(view === 'mismatch' || view === 'invalid' || view === 'error') && (
          <div className="vf-state bad">
            <span className="vf-ic"><ShieldAlert size={46} /></span>
            <h1>{view === 'error' ? 'تعذر التحقق الآن' : 'لم يتم التحقق من المستند'}</h1>
            <p>{view === 'error' ? 'حاول مرة أخرى بعد قليل.'
              : view === 'mismatch' ? 'رمز التحقق غير مطابق للمستند في سجلات المعهد. إما أن الرمز مكتوب خطأ، أو أن المستند تم تعديله بعد طباعته. يرجى التواصل مع المعهد.'
              : 'هذا المستند غير موجود في سجلات معهد السمو. لا يُعتد به.'}</p>
          </div>
        )}

        {view !== 'valid' && view !== 'cancelled' && view !== 'loading' && (
          <form className="vf-form" onSubmit={e => { e.preventDefault(); setCode(typed.trim()) }}>
            <label>ادخل رمز التحقق المطبوع أسفل المستند</label>
            <div><input className="input mono" dir="ltr" placeholder="XXXX-XXXX-XXXX" value={typed} onChange={e => setTyped(e.target.value)} maxLength={20} />
              <button className="btn" disabled={!typed.trim()}>تحقق</button></div>
          </form>
        )}
        <div className="vf-foot">alsmow.com • جميع الحقوق محفوظة © معهد السمو</div>
      </motion.div>
    </div>
  )
}
