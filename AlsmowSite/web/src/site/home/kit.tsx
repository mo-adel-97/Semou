import { motion, useScroll, useTransform, type MotionValue } from 'motion/react'
import { useEffect, useRef, useState, type ReactNode } from 'react'
import { LITE, Reveal } from '../../components/ui'
import { cx, kashidaKeepPunct, kashidaWord } from '../../lib/format'
import { useSite } from '../../lib/queries'

/* =====================================================================
   Building blocks of the home page's visual language.
   Every colour comes from the dashboard (settings › عام › ألوان الهوية): the identity palette drives the
   coloured stages, cubes and accents; --brand / --sub stay the base of everything else.
   ===================================================================== */

const HEX = /^#([0-9a-f]{3}|[0-9a-f]{6})$/i

/** The dashboard's identity palette: the sub colours first (the lively ones), then the main ones. */
export function usePalette(): string[] {
  const g = useSite().data?.settings.general
  const pick = (l?: { a: string; b: string }[]) => (Array.isArray(l) ? l : []).map((c) => c?.b).filter((b): b is string => !!b && HEX.test(b))
  const sub = pick(g?.theme_sub)
  const main = pick(g?.theme_main)
  const all = sub.length ? sub : main
  return all.length ? all : ['var(--brand)', 'var(--sub)']
}

/** A colour from the palette, cycling. */
export const at = (p: string[], i: number) => p[((i % p.length) + p.length) % p.length]

/** Plays only while on screen (saves the CPU, and the story starts when the visitor actually sees it). */
export function useOnScreen<T extends Element>(margin = '0px') {
  const ref = useRef<T>(null)
  const [on, setOn] = useState(false)
  useEffect(() => {
    const el = ref.current
    if (!el) return
    const io = new IntersectionObserver(([e]) => setOn(e.isIntersecting), { rootMargin: margin })
    io.observe(el)
    return () => io.disconnect()
  }, [margin])
  return [ref, on] as const
}

export const REDUCED = typeof window !== 'undefined' && window.matchMedia('(prefers-reduced-motion: reduce)').matches

/**
 * A scripted timeline: phase 0 → n-1 then loops. `durations[i]` is how long phase i lasts (ms).
 * Paused while `playing` is false; with reduced motion it rests on `rest`.
 */
export function useScript(durations: number[], playing: boolean, rest = durations.length - 1) {
  const [phase, setPhase] = useState(REDUCED ? rest : 0)
  const [loop, setLoop] = useState(0)
  useEffect(() => {
    if (!playing || REDUCED) return
    const t = setTimeout(() => {
      setPhase((p) => {
        const n = (p + 1) % durations.length
        if (n === 0) setLoop((l) => l + 1)
        return n
      })
    }, durations[phase])
    return () => clearTimeout(t)
  }, [phase, playing, durations])
  return { phase, loop }
}

/** Types `text` letter by letter once `go` is true. */
export function useTyping(text: string, go: boolean, speed = 55) {
  const [n, setN] = useState(0)
  useEffect(() => {
    if (!go) { setN(0); return }
    if (REDUCED) { setN(text.length); return }
    let i = 0
    const t = setInterval(() => { i++; setN(i); if (i >= text.length) clearInterval(t) }, speed)
    return () => clearInterval(t)
  }, [text, go, speed])
  return text.slice(0, n)
}

/* ---------- headings: the title in ink, the rest of the sentence in a quieter tone ---------- */

export function Headline({ eyebrow, title, rest, align = 'start', className, light }: {
  eyebrow?: ReactNode; title: string; rest?: string | null; align?: 'start' | 'center'; className?: string; light?: boolean
}) {
  // Arabic display style: the longest word of the title is stretched with a kashida (برامج معتمـــدة)
  const words = title.split(/\s+/)
  const k = kashidaWord(words.map((w) => w.replace(/[.,،:!؟?]+$/, '')))
  const shown = k < 0 ? title : words.map((w, i) => (i === k ? kashidaKeepPunct(w, 2) : w)).join(' ')
  return (
    <div className={cx('flex flex-col gap-5', align === 'center' && 'items-center text-center', className)}>
      {eyebrow && <Reveal><span className={cx('kicker', light && 'kicker-light')}>{eyebrow}</span></Reveal>}
      <Reveal delay={0.05}>
        <h2 className={cx('max-w-3xl text-[clamp(1.7rem,1.15rem+1.75vw,2.7rem)] leading-[1.3] font-extrabold tracking-tight text-balance', light ? 'text-white' : 'text-ink')} aria-label={rest ? `${title} ${rest}` : title}>
          {shown}
          {rest && <> <span className={light ? 'hl-rest-light' : 'hl-rest'}>{rest}</span></>}
        </h2>
      </Reveal>
    </div>
  )
}

/* ---------- words that light up one by one as the page scrolls (desktop) ---------- */

export function ScrollWords({ text, className }: { text: string; className?: string }) {
  const ref = useRef<HTMLParagraphElement>(null)
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start 88%', 'end 55%'] })
  if (LITE || REDUCED) return <Reveal><p className={className}>{text}</p></Reveal>
  const words = text.split(/\s+/).filter(Boolean)
  return (
    <p ref={ref} className={className}>
      {words.map((w, i) => <Word key={i} p={scrollYProgress} from={i / words.length} to={(i + 1) / words.length}>{w}</Word>)}
    </p>
  )
}
function Word({ p, from, to, children }: { p: MotionValue<number>; from: number; to: number; children: string }) {
  const o = useTransform(p, [from, to], [0.16, 1])
  return <><motion.span style={{ opacity: o }}>{children}</motion.span>{' '}</>
}

/* ---------- a dark stage that grows from a card to the full width as it scrolls in ---------- */

export function DarkStage({ children, className, id }: { children: ReactNode; className?: string; id?: string }) {
  const ref = useRef<HTMLElement>(null)
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start end', 'start 25%'] })
  const clip = useTransform(scrollYProgress, [0, 1], ['inset(0% 4% 0% 4% round 2.75rem)', 'inset(0% 0% 0% 0% round 0rem)'])
  return (
    <motion.section ref={ref} id={id} style={LITE || REDUCED ? undefined : { clipPath: clip }} className={cx('stage-dark relative isolate overflow-hidden text-white', className)}>
      <div className="stage-dark-bg absolute inset-0 -z-10" aria-hidden />
      {children}
    </motion.section>
  )
}

/* ---------- an isometric cube in a palette colour, with an icon on its top face ---------- */

export function IsoCube({ color, children, className }: { color: string; children?: ReactNode; className?: string }) {
  return (
    <div className={cx('iso-cube', className)} style={{ ['--c' as string]: color }}>
      <svg viewBox="0 0 120 132" className="h-full w-full overflow-visible" aria-hidden>
        <ellipse className="iso-shadow" cx="60" cy="126" rx="42" ry="7" />
        <g className="iso-body">
          <polygon points="60,8 112,38 60,68 8,38" style={{ fill: 'color-mix(in oklab, var(--c) 62%, white)' }} />
          <polygon points="8,38 60,68 60,124 8,94" style={{ fill: 'var(--c)' }} />
          <polygon points="60,68 112,38 112,94 60,124" style={{ fill: 'color-mix(in oklab, var(--c) 68%, black)' }} />
          <polyline points="8,38 60,68 112,38" fill="none" stroke="white" strokeOpacity=".35" strokeWidth=".8" />
          <line x1="60" y1="68" x2="60" y2="124" stroke="white" strokeOpacity=".18" strokeWidth=".8" />
        </g>
      </svg>
      {children && <span className="iso-face">{children}</span>}
    </div>
  )
}

/* ---------- a fake application window (the product shots are drawn live, never screenshots) ---------- */

export function AppWindow({ title, url, children, className, dark }: { title?: ReactNode; url?: string; children: ReactNode; className?: string; dark?: boolean }) {
  return (
    <div className={cx('app-win', dark && 'app-win-dark', className)}>
      <div className="app-win-bar">
        <span className="flex gap-1.5" aria-hidden><i /><i /><i /></span>
        {url ? <span className="app-win-url" dir="ltr">{url}</span> : <span className="app-win-title">{title}</span>}
        <span className="w-10" />
      </div>
      <div className="app-win-body">{children}</div>
    </div>
  )
}

/** The demo's mouse pointer: glides to (x, y) in its container's pixels, and pulses when it clicks. */
export function Cursor({ x, y, click, show = true }: { x: number; y: number; click?: number; show?: boolean }) {
  return (
    <motion.div
      className="demo-cursor"
      initial={false}
      animate={{ x, y, opacity: show ? 1 : 0 }}
      transition={{ x: { type: 'spring', stiffness: 70, damping: 16 }, y: { type: 'spring', stiffness: 70, damping: 16 }, opacity: { duration: 0.3 } }}
      aria-hidden
    >
      {!!click && <span key={click} className="demo-click" />}
      <svg viewBox="0 0 24 24" className="h-6 w-6 drop-shadow-md"><path d="M5 3l14 7.5-6.2 1.6L10 18.5z" fill="#0b0f17" stroke="white" strokeWidth="1.6" strokeLinejoin="round" /></svg>
    </motion.div>
  )
}

/** Where an element sits inside a container (for the demo cursor). */
export function centerIn(container: HTMLElement | null, el: HTMLElement | null, dx = 0.5, dy = 0.6) {
  if (!container || !el) return null
  const c = container.getBoundingClientRect(), r = el.getBoundingClientRect()
  // the container may be scaled (transforms): convert back to its own pixels
  const s = container.offsetWidth ? c.width / container.offsetWidth : 1
  return { x: (r.left - c.left + r.width * dx) / s, y: (r.top - c.top + r.height * dy) / s }
}
