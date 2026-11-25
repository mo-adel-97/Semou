import { ArrowLeft, Check, CheckCircle2, Inbox, Loader2, Send, Sparkles } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useLayoutEffect, useRef, useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import BrandLogo from '../../components/BrandLogo'
import { Counter, Magnetic, SplitText } from '../../components/ui'
import { cx } from '../../lib/format'
import { PROGRAM_TYPES, type HomeData, type Program } from '../../lib/types'
import { AppWindow, Cursor, at, centerIn, useOnScreen, usePalette, useScript, useTyping } from './kit'
import Plexus from './Plexus'

const EASE = [0.22, 1, 0.36, 1] as const

function SmartLink({ to, className, children }: { to?: string; className: string; children: ReactNode }) {
  return to && /^https?:/.test(to) ? (
    <a href={to} target="_blank" rel="noopener noreferrer" className={className}>{children}</a>
  ) : (
    <Link to={to || '/programs'} className={className}>{children}</Link>
  )
}

/* =====================================================================
   Hero: the words on one side; on the other a coloured stage where the institute's real system plays itself:
   a visitor asks the site's assistant, picks one of the real programmes, confirms, and the request lands in
   the reception of «نظام السمو» a second later. The stage takes the next identity colour on every loop.
   ===================================================================== */

export default function Hero({ data }: { data: HomeData }) {
  const h = data.settings.hero
  const title = h.title || data.settings.general.site_name || ''
  const total = data.counts.diploma + data.counts.qualifying + data.counts.development
  const palette = usePalette()
  const accs = data.accreditations.filter((a) => a.logo)
  return (
    <section className="hero2 relative isolate overflow-hidden">
      <div className="hero2-grid absolute inset-0 -z-10" aria-hidden />
      <Plexus colors={palette} className="hero2-web absolute inset-0 -z-10" />
      <div className="hero2-layout container-x">
        <div className="hero2-text">
          {h.eyebrow && (
            <motion.span initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.6, ease: EASE }} className="kicker">
              <span className="relative flex h-2 w-2">
                <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-gold-500 opacity-70" />
                <span className="relative inline-flex h-2 w-2 rounded-full bg-gold-500" />
              </span>
              {h.eyebrow}
            </motion.span>
          )}
          <SplitText as="h1" immediate delay={0.08} text={title} accent={h.highlight} className="hero-title hero2-title font-black tracking-tight text-balance text-ink" />
          {h.subtitle && (
            <motion.p initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.8, delay: 0.35, ease: EASE }} className="hero2-sub max-w-xl text-pretty text-muted">
              {h.subtitle}
            </motion.p>
          )}
          <motion.div initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: 0.5, ease: EASE }} className="hero2-cta grid grid-cols-2 gap-2.5 sm:flex sm:flex-wrap sm:items-center sm:gap-3">
            <Magnetic>
              <SmartLink to={h.cta_link || '/register'} className="btn-primary group w-full !px-3 !py-3.5 sm:w-auto sm:!px-8 xl:!py-4 xl:text-base">
                {h.cta_text} <ArrowLeft className="h-4 w-4 shrink-0 transition group-hover:-translate-x-1 sm:h-5 sm:w-5" />
              </SmartLink>
            </Magnetic>
            {h.secondary_text && (
              <SmartLink to={h.secondary_link || '/programs'} className="btn-ghost w-full !px-3 !py-3.5 sm:w-auto sm:!px-8 xl:!py-4 xl:text-base">{h.secondary_text}</SmartLink>
            )}
          </motion.div>
        </div>

        <motion.div initial={{ opacity: 0, y: 30, scale: 0.97 }} animate={{ opacity: 1, y: 0, scale: 1 }} transition={{ duration: 1, delay: 0.15, ease: EASE }} className="hero2-stage">
          <Stage data={data} total={total} />
        </motion.div>
        {accs.length > 0 && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: 0.7, ease: EASE }} className="hero2-accs">
            <div className="hero2-accs-label">معتمدون من</div>
            <div className="hero-accs">
              {accs.map((a) => <img key={a.id} src={a.logo!} alt={a.name} title={a.name} loading="eager" decoding="async" />)}
            </div>
          </motion.div>
        )}

      </div>
    </section>
  )
}

/* ---------- the stage ---------- */

// how long each phase of the demo lasts (ms)
const SCRIPT = [900, 1500, 1100, 1300, 1100, 900, 900, 3600, 700]
// 0 welcome · 1 the visitor types · 2 «typing…» · 3 programmes listed · 4 cursor → choose · 5 chosen
// 6 cursor → confirm · 7 sent + the request arrives in the ERP · 8 reset

function Stage({ data, total }: { data: HomeData; total: number }) {
  const palette = usePalette()
  const [ref, onScreen] = useOnScreen<HTMLDivElement>('-10% 0px')
  const { phase, loop } = useScript(SCRIPT, onScreen, 5)
  const [pick, setPick] = useState<number | null>(null) // a colour the visitor clicked
  const color = pick !== null ? at(palette, pick) : at(palette, loop)
  const a = data.settings.assistant
  const programs = pickPrograms(data.programs)
  const image = data.settings.hero.image

  return (
    <div ref={ref} className="demo-stage" style={{ ['--stage' as string]: color }}>
      <div className="demo-stage-bg" aria-hidden />

      {/* the institute's people (the hero photo from the dashboard), framed like a print */}
      {image && (
        <div className="demo-photo">
          <img src={image} alt={data.settings.hero.title ?? ''} fetchPriority="high" decoding="async" />
        </div>
      )}

      {/* a live number from the database */}
      {total > 0 && (
        <div className="demo-chip demo-chip-top">
          <span className="demo-chip-icon"><Sparkles className="h-[1.1em] w-[1.1em]" /></span>
          <span>
            <b className="block font-display text-[1.35em] leading-none font-black"><Counter value={total} suffix="+" /></b>
            <span className="text-[.82em] text-muted">برنامج تدريبي معتمد</span>
          </span>
        </div>
      )}

      <Chat phase={phase} loop={loop} name={a?.name || data.settings.general.site_name || ''} programs={programs} logo={data.settings.general.logo} />

      {/* the same request, a second later, inside the institute's ERP */}
      <AnimatePresence>
        {phase === 7 && programs[0] && (
          <motion.div
            key={loop}
            initial={{ opacity: 0, y: -18, scale: 0.9 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: -10, scale: 0.95, transition: { duration: 0.25 } }}
            transition={{ delay: 0.55, type: 'spring', stiffness: 260, damping: 20 }}
            className="demo-toast"
          >
            <span className="demo-toast-icon"><Inbox className="h-[1.1em] w-[1.1em]" /></span>
            <span className="min-w-0">
              <span className="flex items-center gap-1.5 text-[.78em] font-bold text-muted">نظام السمو · الاستقبال <span className="demo-dot" /></span>
              <b className="block truncate font-display text-[.98em] text-ink">طلب تسجيل جديد</b>
              <span className="block truncate text-[.8em] text-ink-2">{programs[0].title}</span>
            </span>
          </motion.div>
        )}
      </AnimatePresence>

      {/* the identity palette: click a colour to paint the stage with it */}
      <div className="demo-swatches" role="group" aria-label="ألوان الهوية">
        {palette.map((c, i) => (
          <button key={c + i} onClick={() => setPick(i)} className={cx(c === color && 'on')} style={{ background: c }} aria-label={c} />
        ))}
      </div>
    </div>
  )
}

/** Diplomas first (the flagship), each with an image if possible. */
function pickPrograms(all: Program[]) {
  const order: Program['type'][] = ['diploma', 'qualifying', 'development']
  return [...all].sort((x, y) => order.indexOf(x.type) - order.indexOf(y.type) || Number(!!y.image) - Number(!!x.image)).slice(0, 3)
}

/* ---------- the assistant conversation ---------- */

function Chat({ phase, loop, name, programs, logo }: { phase: number; loop: number; name: string; programs: Program[]; logo?: string }) {
  const box = useRef<HTMLDivElement>(null)
  const row = useRef<HTMLButtonElement>(null)
  const confirm = useRef<HTMLButtonElement>(null)
  const ask = 'أبغى أسجل في دبلوم معتمد'
  const typed = useTyping(ask, phase >= 1 && phase < 8, 48)
  const [cur, setCur] = useState({ x: 0, y: 0 })
  const [click, setClick] = useState(0)
  const kind = programs[0] ? PROGRAM_TYPES[programs[0].type].plural : ''

  // the cursor: enters from the corner, goes to the first programme, then to «تأكيد»
  useLayoutEffect(() => {
    const b = box.current
    if (!b) return
    if (phase === 4) { const p = centerIn(b, row.current, 0.22, 0.55); if (p) setCur(p) }
    else if (phase === 6) { const p = centerIn(b, confirm.current, 0.5, 0.6); if (p) setCur(p) }
    else if (phase < 4 || phase === 8) setCur({ x: b.offsetWidth * 0.9, y: b.offsetHeight * 0.95 })
  }, [phase])
  useEffect(() => { if (phase === 5 || phase === 7) setClick((c) => c + 1) }, [phase])

  const chosen = phase >= 5 && phase < 8
  return (
    <div className="demo-chat-wrap">
      <AppWindow
        className="demo-chat"
        title={<span className="flex items-center gap-2"><BrandLogo src={logo} animate={false} className="h-[1.45em] w-[1.45em]" /> {name}</span>}
      >
        <div ref={box} className="relative flex h-full flex-col">
          <div className="flex-1 space-y-[.7em] overflow-hidden p-[1em]">
            <Bubble bot>مرحبًا 👋 كيف أقدر أساعدك اليوم؟</Bubble>
            {phase >= 1 && phase < 8 && <Bubble>{typed}{phase === 1 && <span className="demo-caret" />}</Bubble>}
            {phase === 2 && <Bubble bot><span className="demo-typing"><i /><i /><i /></span></Bubble>}
            <AnimatePresence mode="popLayout">
              {phase >= 3 && phase < 7 && (
                <motion.div key={'list' + loop} initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8, transition: { duration: 0.2 } }} transition={{ duration: 0.45, ease: EASE }} className="demo-card">
                  <div className="mb-[.6em] text-[.86em] font-semibold text-ink-2">أبرز {kind} المتاحة الآن:</div>
                  {programs.map((p, i) => (
                    <button key={p.id} ref={i === 0 ? row : undefined} className={cx('demo-row', i === 0 && chosen && 'on')} tabIndex={-1}>
                      {p.image ? <img src={p.image} alt="" loading="lazy" /> : <span className="demo-row-ph" />}
                      <span className="min-w-0 flex-1 text-start">
                        <b className="block truncate">{p.title}</b>
                        <span className="block truncate text-[.82em] text-muted">{[p.duration, p.hours ? `${p.hours} ساعة` : null].filter(Boolean).join(' · ')}</span>
                      </span>
                      <span className="demo-pick">{i === 0 && chosen ? <><Check className="h-[1em] w-[1em]" /> تم</> : 'اختيار'}</span>
                    </button>
                  ))}
                  <button ref={confirm} tabIndex={-1} className={cx('demo-confirm', chosen && 'ready')}>
                    {phase === 6 ? <Loader2 className="h-[1.05em] w-[1.05em] animate-spin" /> : <Send className="h-[1em] w-[1em]" />} تأكيد التسجيل
                  </button>
                </motion.div>
              )}
              {phase === 7 && (
                <motion.div key={'ok' + loop} initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0 }} transition={{ type: 'spring', stiffness: 260, damping: 20 }} className="demo-card demo-done">
                  <CheckCircle2 className="h-[2.2em] w-[2.2em] text-gold-500" />
                  <b className="mt-[.5em] block font-display text-[1.05em] text-ink">تم استلام طلبك</b>
                  <span className="mt-[.2em] block text-[.85em] text-muted">سيتواصل معك فريق القبول قريبًا</span>
                </motion.div>
              )}
            </AnimatePresence>
          </div>
          <div className="demo-input"><span>اكتب سؤالك هنا…</span><Send className="h-[1em] w-[1em]" /></div>
          <Cursor x={cur.x} y={cur.y} click={click} show={phase >= 4 && phase <= 7} />
        </div>
      </AppWindow>
    </div>
  )
}

function Bubble({ bot, children }: { bot?: boolean; children: ReactNode }) {
  return (
    <motion.div initial={{ opacity: 0, y: 8, scale: 0.96 }} animate={{ opacity: 1, y: 0, scale: 1 }} transition={{ duration: 0.35, ease: EASE }} className={cx('demo-bubble', bot ? 'bot' : 'me')}>
      {children}
    </motion.div>
  )
}
