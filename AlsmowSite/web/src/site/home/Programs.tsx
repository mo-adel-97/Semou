import { ArrowLeft, Briefcase, Clock, MapPin, Timer } from 'lucide-react'
import { AnimatePresence, LayoutGroup, motion, useMotionValueEvent, useScroll, type Variants } from 'motion/react'
import { useEffect, useLayoutEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { cx, stretchLabel } from '../../lib/format'
import { FIELDS, fieldOf } from '../../lib/fields'
import { PROGRAM_MODES, PROGRAM_TYPES, type HomeData, type Program, type ProgramType } from '../../lib/types'
import { useLenis } from '../../components/ui'
import { AppWindow, Headline, REDUCED, at, useOnScreen, usePalette } from './kit'
import Scene from './scenes'

const EASE = [0.22, 1, 0.36, 1] as const
/** a programme turning in: from below when moving forward, from above when going back */
const TURN: Variants = {
  enter: (d: number) => ({ opacity: 0, y: d * 56, scale: 0.96 }),
  center: { opacity: 1, y: 0, scale: 1, transition: { duration: 0.55, ease: [0.22, 1, 0.36, 1] as const } },
  exit: (d: number) => ({ opacity: 0, y: d * -40, scale: 0.97, transition: { duration: 0.28, ease: [0.55, 0, 0.75, 0.2] as const } }),
}
const AUTO_MS = 6500 // each programme stays on the stage this long while the visitor just watches

/** Diplomas first; inside a type the dashboard's featured ones first. */
function ordered(list: Program[]) {
  return [...list.filter((p) => p.is_featured), ...list.filter((p) => !p.is_featured)]
}

/* =====================================================================
   Programmes showcase, in the hero's language. The section pins while the visitor scrolls through it: every
   scroll step turns to the next programme (instead of scrolling past them), and when the visitor stops, it keeps
   turning by itself. On one side the list, on the other a coloured stage: the field's live scene (what the
   graduate will work on), the programme card and its essentials. On a phone the list becomes a strip.
   ===================================================================== */

export default function ProgramsShowcase({ data }: { data: HomeData }) {
  const palette = usePalette()
  const types = (Object.keys(PROGRAM_TYPES) as ProgramType[]).filter((t) => data.programs.some((p) => p.type === t))
  const [tab, setTab] = useState<ProgramType | null>(null)
  const active = tab ?? types[0]
  const list = useMemo(() => ordered(data.programs.filter((p) => p.type === active)).slice(0, 6), [data.programs, active])
  const n = list.length
  const [sel, setSel] = useState(0)
  const [dir, setDir] = useState(1)
  const [tick, setTick] = useState(0)
  const [hover, setHover] = useState(false)
  const [scrolled, setScrolled] = useState(false) // the visitor already turned a page by scrolling (hides the hint)
  const lastInput = useRef(0)
  const [ref, onScreen] = useOnScreen<HTMLElement>('-15% 0px')
  // the section keeps ONE size and stays pinned whatever the tab, so moving between tabs never moves the page
  const most = Math.max(...types.map((t) => Math.min(6, data.programs.filter((p) => p.type === t).length)), 1)
  const pin = !REDUCED && most > 1

  // scroll → programme. While the section is pinned, ONE gesture (a wheel notch, a trackpad swipe, a finger swipe)
  // turns exactly one programme; past the last one (or before the first) the page simply carries on. The track keeps
  // a matching scroll position per programme, so the scrollbar, the keyboard and a fast fling still land right.
  const track = useRef<HTMLDivElement>(null)
  const { scrollYProgress } = useScroll({ target: track, offset: ['start start', 'end end'] })
  const step = useRef(0)
  const lenis = useLenis()
  const selRef = useRef(0)
  selRef.current = sel
  const turn = (s: number) => {
    lastInput.current = Date.now()
    if (s === selRef.current) return
    setScrolled(true)
    setDir(s > selRef.current ? 1 : -1)
    selRef.current = s
    setSel(s)
  }
  // a scroll that moves the page itself (scrollbar, keys, a fling) lands on the programme of that position
  useMotionValueEvent(scrollYProgress, 'change', (v) => {
    if (!pin) return
    const s = Math.min(n - 1, Math.max(0, Math.round(v * (n - 1))))
    if (s !== step.current) { step.current = s; turn(s) }
  })
  useEffect(() => {
    const el = track.current
    if (!pin || !el) return
    let lastFlip = 0, lastWheel = 0, touchY: number | null = null, touchDone = false
    const box = () => { const r = el.getBoundingClientRect(); return { top: scrollY + r.top, range: r.height - innerHeight, r } }
    const pinned = () => { const { r } = box(); return r.top <= 4 && r.bottom >= innerHeight - 4 }
    /** may the stage turn one more programme this way? (otherwise the page carries on) */
    const engaged = (down: boolean) => pinned() && (down ? selRef.current < n - 1 : selRef.current > 0)
    const at = (s: number) => { const { top, range } = box(); step.current = s; return n > 1 ? top + (range * s) / (n - 1) : top }
    const jump = (y: number) => { if (lenis) lenis.scrollTo(y, { immediate: true, force: true }); else window.scrollTo({ top: y, behavior: 'instant' }) }
    const flip = (down: boolean) => {
      const s = selRef.current + (down ? 1 : -1)
      lastFlip = Date.now()
      jump(at(s))
      turn(s)
    }
    // captured on the window before the smooth-scroll library sees it, so the page does not glide on meanwhile
    const onWheel = (e: WheelEvent) => {
      if (e.ctrlKey || Math.abs(e.deltaY) < Math.abs(e.deltaX) || !e.deltaY) return
      const down = e.deltaY > 0
      if (!engaged(down)) return
      e.preventDefault(); e.stopPropagation()
      const now = Date.now(), fresh = now - lastWheel > 140 // a new gesture (a trackpad's inertia is one long stream)
      lastWheel = now
      if (now - lastFlip > (fresh ? 320 : 1000)) flip(down)
    }
    const onStart = (e: TouchEvent) => { touchY = e.touches[0].clientY; touchDone = false }
    const onMove = (e: TouchEvent) => {
      if (touchY === null) return
      const dy = touchY - e.touches[0].clientY
      if (Math.abs(dy) < 6) return
      const down = dy > 0
      if (!touchDone && !engaged(down)) { touchY = null; return } // let the page scroll natively
      e.preventDefault(); e.stopPropagation()
      if (!touchDone && Math.abs(dy) > 30) { touchDone = true; flip(down) }
    }
    const onEnd = () => { touchY = null }
    // a gliding scroll that reaches the section stops on its first (or, coming back up, its last) programme
    let prev = el.getBoundingClientRect().top
    const onScroll = () => {
      const { range, r } = box()
      if (Date.now() - lastFlip > 200 && ((prev > 0 && r.top <= 0 && r.top > -range) || (prev < -range && r.top >= -range && r.top < 0))) {
        lastFlip = Date.now(); jump(at(selRef.current)) // stay on the programme already on stage
      }
      prev = el.getBoundingClientRect().top
    }
    const opts = { passive: false, capture: true }
    window.addEventListener('wheel', onWheel, opts)
    window.addEventListener('touchstart', onStart, { passive: true, capture: true })
    window.addEventListener('touchmove', onMove, opts)
    window.addEventListener('touchend', onEnd, true)
    window.addEventListener('scroll', onScroll, { passive: true })
    return () => {
      window.removeEventListener('wheel', onWheel, true)
      window.removeEventListener('touchstart', onStart, true)
      window.removeEventListener('touchmove', onMove, true)
      window.removeEventListener('touchend', onEnd, true)
      window.removeEventListener('scroll', onScroll)
    }
  }, [pin, n, lenis])

  // …and on its own while the visitor just watches
  const playing = onScreen && !hover && !REDUCED && n > 1
  useEffect(() => {
    if (!playing) return
    const t = setTimeout(() => {
      if (Date.now() - lastInput.current < AUTO_MS) { setTick((k) => k + 1); return }
      setDir(1); setSel((s) => (s + 1) % n)
    }, AUTO_MS)
    return () => clearTimeout(t)
  }, [playing, sel, tick, n])

  // a new tab starts from its first programme, and the page stays on the section: the track's length changes with
  // the number of programmes (a single course does not pin at all), which would otherwise drop the visitor further down
  const keep = useRef(false)
  useLayoutEffect(() => {
    setSel(0); selRef.current = 0; step.current = 0
    const el = track.current
    if (!keep.current || !el) return
    keep.current = false
    const r = el.getBoundingClientRect()
    if (r.top < 0) { const y = scrollY + r.top; if (lenis) lenis.scrollTo(y, { immediate: true, force: true }); else window.scrollTo({ top: y, behavior: 'instant' }) }
  }, [active])

  // the phone strip follows the programme on stage
  const strip = useRef<HTMLDivElement>(null)
  useEffect(() => {
    const box = strip.current
    const el = box?.children[sel] as HTMLElement | undefined
    if (box && el) box.scrollTo({ left: el.offsetLeft - (box.clientWidth - el.offsetWidth) / 2, behavior: 'smooth' })
  }, [sel, active])

  if (!types.length) return null
  const cur = Math.min(sel, n - 1)
  const p = list[cur]
  const color = at(palette, cur)
  const choose = (i: number) => { if (i === cur) return; lastInput.current = Date.now(); setDir(i > cur ? 1 : -1); setSel(i) }
  const meta = (x: Program) => [x.duration, x.hours ? `${x.hours} ساعة` : null, x.mode ? PROGRAM_MODES[x.mode] : null].filter(Boolean).join(' · ') || 'التفاصيل والتسجيل'

  return (
    <section ref={ref} className="pshow relative pt-20 md:pt-28" id="programs">
      <div className="container-x">
        <Headline eyebrow="برامجنا التدريبية" title="برامج معتمدة" rest="تفتح لك أبواب المستقبل." />
      </div>

      <div ref={track} className={cx('pshow-track', pin && 'pinned')} style={{ ['--n' as string]: most }}>
        <div className="pshow-sticky">
          <div className="container-x w-full">
            <div className="pshow-bar">
              <LayoutGroup>
                <div className="pshow-tabs" role="tablist">
                  {types.map((t) => (
                    <button key={t} role="tab" aria-selected={active === t} onClick={() => { keep.current = true; setTab(t) }} className={cx('pshow-tab', active === t && 'on')}>
                      {active === t && <motion.span layoutId="pshow-tab" className="pshow-tab-bg" transition={{ type: 'spring', stiffness: 400, damping: 32 }} />}
                      <span className="relative">{PROGRAM_TYPES[t].plural}</span>
                      <span className="pshow-tab-n relative">{data.counts[t]}</span>
                    </button>
                  ))}
                </div>
              </LayoutGroup>
              {n > 1 && (
                <div className="pshow-pager" aria-hidden>
                  <b>{String(cur + 1).padStart(2, '0')}</b><span>/ {String(n).padStart(2, '0')}</span>
                  <span className="pshow-dots">{list.map((x, i) => <i key={x.id} className={i === cur ? 'on' : undefined} style={i === cur ? { background: color } : undefined} />)}</span>
                </div>
              )}
            </div>

            <div className="pshow-grid" onPointerEnter={(e) => e.pointerType === 'mouse' && setHover(true)} onPointerLeave={() => setHover(false)}>
              {/* the list (desktop) */}
              <div className="pshow-list" role="group" aria-label={PROGRAM_TYPES[active].plural}>
                {list.map((x, i) => {
                  const S = FIELDS[fieldOf(x)].icon
                  const on = i === cur
                  return (
                    <button key={x.id} aria-pressed={on} onClick={() => choose(i)} className={cx('pshow-row', on && 'on')} style={{ ['--c' as string]: at(palette, i) }}>
                      {on && <motion.span layoutId="pshow-row" className="pshow-row-bg" transition={{ type: 'spring', stiffness: 380, damping: 34 }} />}
                      <span className="pshow-row-icon"><S /></span>
                      <span className="relative min-w-0 flex-1 text-start">
                        <b className="pshow-row-title">{x.title}</b>
                        <span className="pshow-row-meta">{meta(x)}</span>
                      </span>
                      <ArrowLeft className="pshow-row-arrow" />
                      {on && playing && <span key={`${active}-${sel}-${tick}`} className="pshow-row-progress" style={{ animationDuration: `${AUTO_MS}ms` }} />}
                    </button>
                  )
                })}
                <Link to={`/programs/type/${active}`} className="pshow-all group">
                  عرض كل {PROGRAM_TYPES[active].plural} <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" />
                </Link>
              </div>

              {/* the stage */}
              <div className="pshow-stage-wrap">
                {p && <Stage key={active} p={p} color={color} dir={dir} />}
                <AnimatePresence>
                  {pin && n > 1 && !scrolled && (
                    <motion.span className="pshow-hint" initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: 8 }} transition={{ delay: 1.2 }}>
                      <i className="pshow-hint-mouse"><i /></i> مرّر لاستعراض {PROGRAM_TYPES[active].plural}
                    </motion.span>
                  )}
                </AnimatePresence>
              </div>

              {/* the strip (phone) */}
              <div ref={strip} className="pshow-strip" role="group" aria-label={PROGRAM_TYPES[active].plural}>
                {list.map((x, i) => {
                  const S = FIELDS[fieldOf(x)].icon
                  return (
                    <button key={x.id} aria-pressed={i === cur} onClick={() => choose(i)} className={cx('pshow-chipbtn', i === cur && 'on')} style={{ ['--c' as string]: at(palette, i) }}>
                      <S /><span>{x.title}</span>
                    </button>
                  )
                })}
                <Link to={`/programs/type/${active}`} className="pshow-chipbtn all">عرض الكل <ArrowLeft /></Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

/* ---------- the coloured stage: the field's live scene + the programme card + the essentials ---------- */

function Stage({ p, color, dir }: { p: Program; color: string; dir: number }) {
  const field = fieldOf(p)
  const { icon: Icon, app, careers } = FIELDS[field]
  return (
    <div className="demo-stage pshow-stage" style={{ ['--stage' as string]: color }}>
      <div className="demo-stage-bg" aria-hidden />
      <AnimatePresence mode="popLayout" initial={false} custom={dir}>
        <motion.div key={p.id} className="pshow-scene" custom={dir} variants={TURN} initial="enter" animate="center" exit="exit">
          {/* what the graduate will work on, playing live */}
          <motion.div className="pshow-app" initial={{ opacity: 0, y: 18 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.55, ease: EASE }}>
            <AppWindow title={<span className="flex items-center gap-[.45em]"><Icon className="h-[1.15em] w-[1.15em]" style={{ color: 'var(--stage)' }} />{app}</span>}>
              <Scene field={field} />
            </AppWindow>
          </motion.div>

          {/* the programme */}
          <motion.div className="pshow-card" initial={{ opacity: 0, y: 30 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5, ease: EASE, delay: 0.08 }}>
            <span className="pshow-type"><i />{stretchLabel(PROGRAM_TYPES[p.type].label)}{p.badge && <em title={p.badge}>{p.badge}</em>}</span>
            <h3 className="pshow-title">{p.title}</h3>
            <div className="pshow-actions">
              <Link to={`/programs/${p.slug}`} className="pshow-btn">اكتشف البرنامج <ArrowLeft className="h-[1.1em] w-[1.1em]" /></Link>
              <Link to={`/register?program=${encodeURIComponent(p.title)}`} className="pshow-btn ghost">سجّل الآن</Link>
            </div>
          </motion.div>

          {p.duration && <Fact className="pshow-chip-a" delay={0.2} icon={<Timer />} label="مدة البرنامج" value={p.duration} />}
          {p.hours ? <Fact className="pshow-chip-b" delay={0.26} icon={<Clock />} label="ساعات تدريبية" value={`${p.hours} ساعة`} />
            : p.mode ? <Fact className="pshow-chip-b" delay={0.26} icon={<MapPin />} label="نمط الدراسة" value={PROGRAM_MODES[p.mode]} /> : null}
          <motion.div className="demo-chip pshow-chip-c" initial={{ opacity: 0, y: 14, scale: 0.9 }} animate={{ opacity: 1, y: 0, scale: 1 }} transition={{ type: 'spring', stiffness: 260, damping: 22, delay: 0.32 }}>
            <span className="demo-chip-icon"><Briefcase /></span>
            <span className="min-w-0">
              <span className="block text-[.78em] font-semibold text-muted">مجالات العمل</span>
              <b className="pshow-careers">{p.careers || careers}</b>
            </span>
          </motion.div>
        </motion.div>
      </AnimatePresence>
    </div>
  )
}

function Fact({ className, icon, label, value, delay }: { className: string; icon: ReactNode; label: string; value: string; delay: number }) {
  return (
    <motion.div className={cx('demo-chip', className)} initial={{ opacity: 0, y: 14, scale: 0.9 }} animate={{ opacity: 1, y: 0, scale: 1 }} transition={{ type: 'spring', stiffness: 260, damping: 22, delay }}>
      <span className="demo-chip-icon">{icon}</span>
      <span className="min-w-0">
        <span className="block text-[.78em] font-semibold text-muted">{label}</span>
        <b className="block truncate font-display text-[1.02em] text-ink">{value}</b>
      </span>
    </motion.div>
  )
}
