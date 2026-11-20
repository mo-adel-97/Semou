import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useState, type CSSProperties } from 'react'
import { useLenis } from '../components/ui'
import { useSite } from '../lib/queries'
import BrandLogo from '../components/BrandLogo'

/**
 * Opening scene — "the light of knowledge": two white doves glide in carrying a welcome plaque crowned by a
 * golden medallion (an open book radiating light) while motes of light rise; they hover, then glide away.
 *
 * Smoothness: every moving part is a CSS animation of transform / opacity on its own layer (the wings are
 * separate layers too), so it runs on the compositor and never stutters while the page loads behind it.
 * Every word comes from the dashboard (settings group "intro"); intro.mode = off | first | session | always.
 */

export type IntroMode = 'off' | 'first' | 'session' | 'always'
const KEY = 'alsmow.intro.v1' // sessionStorage: played in this visit
const KEY_EVER = 'alsmow.intro.seen' // localStorage: played once in this browser
const MODE_CACHE = 'alsmow.intro.mode' // last known setting, so the decision is instant on the next load
const FLY = 6.4 // seconds (keep in sync with --fly in index.css)

const isMode = (m: unknown): m is IntroMode => m === 'off' || m === 'first' || m === 'session' || m === 'always'
function cachedMode(): IntroMode {
  try { const m = localStorage.getItem(MODE_CACHE); return isMode(m) ? m : 'session' } catch { return 'session' }
}

function shouldPlay(mode: IntroMode) {
  try {
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return false
    if (/^\/admin/.test(location.pathname)) return false
    const p = new URLSearchParams(location.search)
    if (p.has('intro')) return true
    if (p.has('edit') || sessionStorage.getItem('alsmow.texts.edit') === '1') return false
    if (mode === 'off') return false
    if (mode === 'always') return true
    if (mode === 'first') return localStorage.getItem(KEY_EVER) !== '1'
    return sessionStorage.getItem(KEY) !== '1'
  } catch {
    return false
  }
}

function markPlayed() {
  try { sessionStorage.setItem(KEY, '1'); localStorage.setItem(KEY_EVER, '1') } catch { /* private mode: fine */ }
}

/** A setting that may be "never saved" (use the default) or "saved empty" (show nothing). */
function pick(v: string | null | undefined, fallback: string, saved: boolean): string {
  if (v && v.trim()) return v.trim()
  return saved ? '' : fallback
}

/* ---------------- art ---------------- */

const VB = '0 0 160 110'

/** A white dove gliding left, in three layers: far wing, body, near wing (each wing beats on its own layer). */
function Dove({ className = '', phase = 0 }: { className?: string; phase?: number }) {
  const style = { '--ph': `${-phase}s` } as CSSProperties
  return (
    <div className={`dove ${className}`} style={style} aria-hidden>
      <div className="dove-wing far">
        <svg viewBox={VB}>
          <path d="M78 56 C70 38 74 18 90 2 C94 12 100 18 108 21 C106 28 110 33 117 35 C108 44 98 51 78 56 Z" fill="#d7e0db" />
        </svg>
      </div>
      <svg viewBox={VB} className="dove-body">
        <defs>
          <linearGradient id="dv-b" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0" stopColor="#ffffff" />
            <stop offset=".7" stopColor="#f2f6f4" />
            <stop offset="1" stopColor="#e0e8e4" />
          </linearGradient>
        </defs>
        <path d="M114 60 C124 55 135 53 147 53 C144 58 143 62 145 65 C143 69 145 73 149 78 C137 78 125 74 114 69 Z" fill="url(#dv-b)" stroke="#d3ddd8" strokeWidth=".7" />
        <path d="M121 61 L141 57 M121 65 L143 66 M120 68 L141 75" stroke="#dde5e1" strokeWidth=".8" fill="none" />
        <path d="M28 64 C30 50 46 43 66 45 C88 47 106 53 120 62 C112 72 94 80 72 80 C52 80 34 76 28 64 Z" fill="url(#dv-b)" stroke="#d3ddd8" strokeWidth=".7" />
        <path d="M20 56 C20 47 27 41 35 42 C43 43 48 50 46 58 C44 64 38 68 30 67 C24 66 20 62 20 56 Z" fill="#fff" stroke="#d3ddd8" strokeWidth=".7" />
        <path d="M21 55.5 L11 58.5 L21 61 Z" fill="#d99a3c" />
        <circle cx="29" cy="53.5" r="2.1" fill="#1d2924" />
        <circle cx="29.7" cy="52.8" r=".6" fill="#fff" />
        <path d="M72 78 l-2 8 M78 78 l1 8" stroke="#d99a3c" strokeWidth="2.2" strokeLinecap="round" />
      </svg>
      <div className="dove-wing near">
        <svg viewBox={VB}>
          <defs>
            <linearGradient id="dv-w" x1=".2" y1="1" x2=".6" y2="0">
              <stop offset="0" stopColor="#e9efec" />
              <stop offset=".55" stopColor="#ffffff" />
            </linearGradient>
          </defs>
          <path
            d="M60 60 C50 42 52 18 72 0 C76 9 83 13 92 14 C90 21 95 25 103 26 C101 33 106 37 114 38 C108 45 101 51 94 54 C84 60 72 62 60 60 Z"
            fill="url(#dv-w)"
            stroke="#d3ddd8"
            strokeWidth=".7"
          />
          <path d="M62 54 C60 40 62 24 72 10 M70 56 C70 42 76 30 86 20 M80 57 C82 46 88 38 98 31" stroke="#dbe3df" strokeWidth=".9" fill="none" strokeLinecap="round" />
        </svg>
      </div>
    </div>
  )
}

/** Soft cloud: layered translucent shapes (cheap, no filters). */
function Cloud({ className }: { className: string }) {
  return (
    <svg viewBox="0 0 220 90" className={`intro-cloud ${className}`} aria-hidden>
      <g fill="#fff">
        <path opacity=".35" d="M26 76 C6 76 2 54 22 49 C20 31 42 22 55 33 C62 13 96 8 107 30 C116 19 141 21 145 38 C162 31 184 42 179 57 C199 57 205 76 186 78 Z" />
        <path opacity=".6" d="M32 72 C14 72 11 55 26 51 C24 35 44 28 56 37 C62 20 92 14 103 33 C111 23 134 25 138 41 C154 35 173 45 169 58 C186 58 191 72 177 73 Z" />
        <path opacity=".95" d="M40 68 C26 68 24 56 35 53 C34 41 49 36 58 42 C64 28 88 24 97 38 C104 31 122 33 125 44 C137 40 152 47 149 57 C162 58 164 68 154 69 Z" />
      </g>
    </svg>
  )
}

/** The medallion: an open book radiating light. */
function Emblem() {
  return (
    <div className="pl-emblem" aria-hidden>
      <span className="pl-emblem-glow" />
      <svg viewBox="0 0 64 64">
        <g className="pl-rays" stroke="#fff1bd" strokeWidth="2.2" strokeLinecap="round">
          <path d="M32 7 V15" /><path d="M17 11 L21.5 18" /><path d="M47 11 L42.5 18" /><path d="M7 21 L14.5 25" /><path d="M57 21 L49.5 25" />
        </g>
        <path d="M32 26 C26 22 17 21 10 23 V47 C17 45 26 46 32 50 Z" fill="#fffaf0" />
        <path d="M32 26 C38 22 47 21 54 23 V47 C47 45 38 46 32 50 Z" fill="#fff3d6" />
        <path d="M32 26 V50" stroke="#c9a14f" strokeWidth="1.4" />
        <path d="M15 30 C20 29 25 30 28 31.5 M15 35 C20 34 25 35 28 36.5 M15 40 C20 39 25 40 28 41.5 M36 31.5 C39 30 44 29 49 30 M36 36.5 C39 35 44 34 49 35" stroke="#d8c08a" strokeWidth="1.1" fill="none" strokeLinecap="round" />
      </svg>
    </div>
  )
}

/** Motes of golden light rising (fixed positions: same picture on every load). */
const MOTES = Array.from({ length: 14 }, (_, i) => {
  const r = (n: number) => (Math.sin(i * 97.13 + n * 13.7) + 1) / 2
  return { left: 6 + r(1) * 88, top: 45 + r(2) * 50, size: 4 + r(3) * 5, delay: r(4) * 2.6, dur: 3.8 + r(5) * 2.4 }
})

/* ---------------- scene ---------------- */

export default function IntroBirds() {
  const [show, setShow] = useState(() => shouldPlay(cachedMode()))
  const lenis = useLenis()
  const { data } = useSite()
  const g = data?.settings.general
  const it = data?.settings.intro
  const saved = !!it && Object.keys(it).length > 0 // the group was saved at least once
  const mode = it?.mode
  useEffect(() => {
    if (!isMode(mode)) return
    try { localStorage.setItem(MODE_CACHE, mode) } catch { /* */ }
    if (mode === 'off' && !new URLSearchParams(location.search).has('intro')) setShow(false)
  }, [mode])

  const name = pick(it?.title, '', false) || g?.site_name || 'معهد السمو للتدريب'
  const message = pick(it?.message, '', false) || g?.tagline || 'مستقبلك يبدأ من هنا'
  const kicker = pick(it?.kicker, 'أهلاً بك في', saved)
  const quote = pick(it?.quote, '﴿وَقُل رَّبِّ زِدْنِي عِلْمًا﴾', saved)
  const skip = pick(it?.skip, 'تخطّي', false)
  const logo = g?.logo || '/logo.png'

  useEffect(() => {
    if (!show) return
    markPlayed()
    lenis?.stop()
    const html = document.documentElement
    const prev = html.style.overflow
    html.style.overflow = 'hidden'
    html.classList.add('intro-on') // the page under the opaque sky is not painted meanwhile
    const done = setTimeout(() => setShow(false), FLY * 1000 - 250)
    const key = (e: KeyboardEvent) => { if (e.key === 'Escape' || e.key === 'Enter' || e.key === ' ') setShow(false) }
    window.addEventListener('keydown', key)
    return () => {
      clearTimeout(done)
      window.removeEventListener('keydown', key)
      html.style.overflow = prev
      html.classList.remove('intro-on')
      lenis?.start()
    }
  }, [show, lenis])

  return (
    <AnimatePresence>
      {show && (
        <motion.div
          className="intro-sky"
          role="dialog"
          aria-label={`${name}: ${message}`}
          onClick={() => setShow(false)}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1, transition: { duration: 0.3 } }}
          exit={{ opacity: 0, transition: { duration: 0.8, ease: 'easeInOut' } }}
        >
          <div className="intro-sun" />
          <Cloud className="c1" />
          <Cloud className="c2" />
          <Cloud className="c3" />
          <div className="intro-motes" aria-hidden>
            {MOTES.map((m, i) => (
              <span key={i} style={{ left: `${m.left}%`, top: `${m.top}%`, width: m.size, height: m.size, animationDelay: `${m.delay}s`, animationDuration: `${m.dur}s` }} />
            ))}
          </div>
          <div className="intro-far" aria-hidden>
            {[['15%', 0.2, 0.8], ['21%', 0.8, 0.6], ['76%', 0.4, 0.7], ['11%', 1.5, 0.5]].map(([top, d, s], i) => (
              <svg key={i} viewBox="0 0 40 16" style={{ top: top as string, width: 30 * (s as number), animationDelay: `${d}s` }}>
                <path d="M2 10 Q11 3 20 10 Q29 3 38 10" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" />
              </svg>
            ))}
          </div>

          {/* flight = an X move and a Y move with different easing → one natural arc; then a calm hover */}
          <div className="fl-x">
            <div className="fl-y">
              <div className="fl-bob">
                <div className="intro-flight">
                  <Dove className="d-a" />
                  <Dove className="d-b" phase={0.22} />
                  <span className="pl-rope r-a" />
                  <span className="pl-rope r-b" />
                  <div className="plaque">
                    <Emblem />
                    <div className="pl-frame">
                      {kicker && <div className="pl-kicker">{kicker}</div>}
                      <div className="pl-brand">
                        <BrandLogo src={logo} animate={false} className="pl-logo" />
                        <h1>{name}</h1>
                      </div>
                      <div className="pl-rule"><span /><i /><span /></div>
                      <p className="pl-msg">{message}</p>
                      {quote && <p className="pl-quote">{quote}</p>}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <button type="button" className="intro-skip" onClick={(e) => { e.stopPropagation(); setShow(false) }}>
            {skip}
          </button>
        </motion.div>
      )}
    </AnimatePresence>
  )
}
