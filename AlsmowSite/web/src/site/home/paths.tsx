import { Award, Check, GraduationCap, Sparkles, TrendingUp } from 'lucide-react'
import { motion } from 'motion/react'
import type { ProgramType } from '../../lib/types'

/* =====================================================================
   The three paths, each told by what it hands the visitor, in the hero's language (a coloured stage with white
   UI on it): a diploma ends with an accredited certificate, a qualifying course walks you step by step to the job
   market, a development course grows the skills you already have. No pictures, nothing to fill in later.
   ===================================================================== */

const EASE = [0.22, 1, 0.36, 1] as const
const view = { once: true, margin: '-60px' } as const

export default function PathScene({ type, color }: { type: ProgramType; color: string }) {
  return (
    <div className="path-scene" style={{ ['--stage' as string]: color }}>
      <div className="demo-stage-bg" aria-hidden />
      {type === 'diploma' ? <Certificate /> : type === 'qualifying' ? <Steps /> : <Growth />}
    </div>
  )
}

/** the diploma: the certificate itself, stamped */
function Certificate() {
  return (
    <motion.div className="ps-card ps-cert" initial={{ opacity: 0, y: 26, rotate: -4 }} whileInView={{ opacity: 1, y: 0, rotate: -2.5 }} viewport={view} transition={{ duration: 0.8, ease: EASE }}>
      <div className="ps-cert-head">
        <span className="ps-ic"><GraduationCap /></span>
        <span className="min-w-0">
          <b>شهادة دبلوم</b>
          <small>معتمدة رسمياً · بختم المعهد</small>
        </span>
      </div>
      <i className="ps-line w-[78%]" /><i className="ps-line w-[56%]" />
      <div className="ps-cert-foot">
        <svg viewBox="0 0 120 30" className="ps-sign" aria-hidden><path d="M4 22c10-14 16-16 18-8s-6 12 2 6 14-16 20-10-4 12 6 8 16-12 24-8 10 6 18 2 14-6 24-4" /></svg>
        <motion.span className="ps-stamp" initial={{ opacity: 0, scale: 1.9, rotate: -24 }} whileInView={{ opacity: 1, scale: 1, rotate: -12 }} viewport={view} transition={{ delay: 0.9, type: 'spring', stiffness: 320, damping: 16 }}>
          <Award /> معتمد
        </motion.span>
      </div>
    </motion.div>
  )
}

/** a qualifying course: a short, guided path that ends in the job market */
function Steps() {
  const steps = ['الأساسيات', 'التطبيق العملي', 'جاهز لسوق العمل']
  return (
    <motion.div className="ps-card ps-steps" initial={{ opacity: 0, y: 26 }} whileInView={{ opacity: 1, y: 0 }} viewport={view} transition={{ duration: 0.8, ease: EASE }}>
      <div className="ps-steps-rail" aria-hidden><motion.i initial={{ scaleY: 0 }} whileInView={{ scaleY: 1 }} viewport={view} transition={{ duration: 1.4, delay: 0.4, ease: EASE }} /></div>
      {steps.map((s, i) => (
        <motion.div key={s} className={i === steps.length - 1 ? 'ps-step now' : 'ps-step'} initial={{ opacity: 0, x: -14 }} whileInView={{ opacity: 1, x: 0 }} viewport={view} transition={{ duration: 0.55, delay: 0.35 + i * 0.4, ease: EASE }}>
          <span className="ps-dot">{i < steps.length - 1 ? <Check /> : <i />}</span>
          <b>{s}</b>
          <small>{i < steps.length - 1 ? 'مكتملة' : 'المرحلة التالية'}</small>
        </motion.div>
      ))}
    </motion.div>
  )
}

/** a development course: what you can already do, grown */
function Growth() {
  const skills = [{ k: 'التواصل', a: 46, b: 88 }, { k: 'القيادة', a: 34, b: 74 }, { k: 'الأدوات الرقمية', a: 52, b: 92 }]
  return (
    <motion.div className="ps-card ps-growth" initial={{ opacity: 0, y: 26 }} whileInView={{ opacity: 1, y: 0 }} viewport={view} transition={{ duration: 0.8, ease: EASE }}>
      <div className="ps-growth-head">
        <b>مهاراتك</b>
        <motion.span className="ps-up" initial={{ opacity: 0, scale: 0.6 }} whileInView={{ opacity: 1, scale: 1 }} viewport={view} transition={{ delay: 1.3, type: 'spring', stiffness: 300, damping: 18 }}><TrendingUp /> تطوّر</motion.span>
      </div>
      {skills.map((s, i) => (
        <div key={s.k} className="ps-skill">
          <span>{s.k}</span>
          <span className="ps-bar">
            <i className="ps-bar-was" style={{ width: `${s.a}%` }} />
            <motion.i className="ps-bar-now" initial={{ width: `${s.a}%` }} whileInView={{ width: `${s.b}%` }} viewport={view} transition={{ duration: 1.1, delay: 0.5 + i * 0.18, ease: EASE }} />
          </span>
        </div>
      ))}
      <motion.span className="ps-new" initial={{ opacity: 0, y: 10 }} whileInView={{ opacity: 1, y: 0 }} viewport={view} transition={{ delay: 1.6, duration: 0.5, ease: EASE }}><Sparkles /> مهارة جديدة</motion.span>
    </motion.div>
  )
}
