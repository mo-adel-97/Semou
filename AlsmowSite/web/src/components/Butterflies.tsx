import { Mail } from 'lucide-react'
import { useEffect, useRef, useState, type CSSProperties, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { cx } from '../lib/format'

/**
 * Butterflies in the theme's colours (wings = --brand / --glow).
 * Each wing is its own layer turning in real 3D (rotateY with perspective) and the body bobs with every beat,
 * so the flight looks soft and alive; every movement is a transform (smooth on phones). Styles: .bf* in index.css.
 */
export function Butterfly({ className, style }: { className?: string; style?: CSSProperties }) {
  return (
    <span className={cx('bf', className)} style={style} aria-hidden>
      <span className="bf-body-wrap">
        <span className="bf-w bf-wl">
          <svg viewBox="0 0 20 32">
            <path className="bf-up" d="M20 15 C15 3 4 -1 2.5 7 C1.5 13 9 17 20 15 Z" />
            <path className="bf-lo" d="M20 16.5 C12 17 5 22 8 28 C10.5 32 17 26 20 16.5 Z" />
          </svg>
        </span>
        <span className="bf-w bf-wr">
          <svg viewBox="20 0 20 32">
            <path className="bf-up" d="M20 15 C25 3 36 -1 37.5 7 C38.5 13 31 17 20 15 Z" />
            <path className="bf-lo" d="M20 16.5 C28 17 35 22 32 28 C29.5 32 23 26 20 16.5 Z" />
          </svg>
        </span>
        <svg viewBox="0 0 40 32" className="bf-torso">
          <path className="bf-body" d="M20 9 V24" />
          <path className="bf-ant" d="M20 9 C19 6 17.5 4.5 16 4 M20 9 C21 6 22.5 4.5 24 4" />
        </svg>
      </span>
    </span>
  )
}

/** Butterflies circling whatever is inside (a logo): each ring has its own size, speed and direction. */
export function ButterflyOrbit({ children, className, rings = [
  { r: '180%', t: '9s', s: 32 },
  { r: '240%', t: '14s', s: 24, reverse: true },
] }: { children: ReactNode; className?: string; rings?: { r: string; t: string; s: number; reverse?: boolean; delay?: string }[] }) {
  return (
    <span className={cx('bf-orbit-host', className)}>
      {children}
      {rings.map((ring, i) => (
        <span
          key={i}
          className={cx('bf-ring', ring.reverse && 'rev')}
          style={{ '--r': ring.r, '--t': ring.t, animationDelay: ring.delay ?? `${-i * 3}s` } as CSSProperties}
        >
          <span className="bf-upright">
            <Butterfly style={{ '--s': `${ring.s}px`, '--flap': `${0.22 + i * 0.05}s` } as CSSProperties} />
          </span>
        </span>
      ))}
    </span>
  )
}

/** A few butterflies drifting across the section: a steady glide forward, with a separate soft rise-and-fall. */
const SKY = Array.from({ length: 7 }, (_, i) => {
  const r = (n: number) => (Math.sin(i * 41.7 + n * 9.13) + 1) / 2
  return {
    y: `${8 + r(1) * 70}%`, d: `${24 + r(2) * 16}s`, delay: `${-r(3) * 30}s`, s: Math.round(36 + r(4) * 22),
    flap: `${0.2 + r(5) * 0.12}s`, bob: `${2.6 + r(6) * 1.8}s`, amp: `${18 + r(7) * 26}px`,
  }
})
export function ButterflySky({ count = 7, className }: { count?: number; className?: string }) {
  return (
    <div className={cx('bf-sky', className)} aria-hidden>
      {SKY.slice(0, count).map((b, i) => (
        <span key={i} className="bf-fly" style={{ '--y': b.y, '--d': b.d, '--delay': b.delay } as CSSProperties}>
          <span className="bf-bob" style={{ '--bob': b.bob, '--amp': b.amp } as CSSProperties}>
            <Butterfly style={{ '--s': `${b.s}px`, '--flap': b.flap } as CSSProperties} />
          </span>
        </span>
      ))}
    </div>
  )
}

/* ---------------- Scenes: a butterfly carrying a banner, two butterflies talking ---------------- */

export type ButterflyMessage = { text: string; to?: string | null }
export type ButterflyTalk = { q: string; a: string }

type Frame = { x: number; y: number; r?: number }
/** Bakes a flight path into Web Animations keyframes: the browser then plays it on the compositor,
 *  so the flight stays perfectly smooth even while the page is busy. */
function bake(path: (t: number) => Frame, seconds: number): Keyframe[] {
  const n = Math.round(seconds * 30)
  return Array.from({ length: n + 1 }, (_, i) => {
    const p = path((i / n) * seconds)
    return { offset: i / n, transform: `translate3d(${p.x.toFixed(1)}px, ${p.y.toFixed(1)}px, 0) rotate(${(p.r ?? 0).toFixed(1)}deg)` }
  })
}
const easeOut = (x: number) => 1 - Math.pow(1 - x, 3)
const easeIn = (x: number) => x * x * x
const clamp01 = (x: number) => Math.max(0, Math.min(1, x))

/**
 * Every so often, on any page, one of two short scenes plays on its own layer above the page (nothing moves,
 * nothing is covered for long): a butterfly carrying a banner on a thread, or two butterflies talking
 * (a question, then the institute's answer). They take turns and never overlap; they wait while the page is
 * hidden or the opening scene plays, and never play for "reduce motion". All the words come from the settings.
 */
export function ButterflyShow({ letters, talks, asker, answerer, first = 10, every = 35 }: {
  letters: ButterflyMessage[]; talks: ButterflyTalk[]; asker?: string | null; answerer?: string | null; first?: number; every?: number
}) {
  const [scene, setScene] = useState<{ n: number; kind: 'letter' | 'talk'; i: number } | null>(null)
  const data = useRef({ letters, talks })
  data.current = { letters, talks }
  const count = useRef({ n: 0, letter: 0, talk: 0, last: '' as '' | 'letter' | 'talk' })
  const busy = useRef(false)

  useEffect(() => {
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return
    let timer = 0
    const next = () => {
      const { letters, talks } = data.current
      // the conversation opens the show (it's the richer scene), then they alternate
      const kinds = [...(talks.length ? ['talk' as const] : []), ...(letters.length ? ['letter' as const] : [])]
      if (kinds.length && !busy.current && !document.hidden && !document.documentElement.classList.contains('intro-on')) {
        const c = count.current
        const kind = kinds.length > 1 && c.last === kinds[0] ? kinds[1] : kinds[0]
        const i = kind === 'letter' ? c.letter++ % letters.length : c.talk++ % talks.length
        c.last = kind
        busy.current = true
        setScene({ n: c.n++, kind, i })
      }
      timer = window.setTimeout(next, every * 1000)
    }
    timer = window.setTimeout(next, first * 1000)
    return () => clearTimeout(timer)
  }, [first, every])

  if (!scene) return null
  const end = () => { busy.current = false; setScene(null) }
  if (scene.kind === 'letter') {
    const msg = letters[scene.i % Math.max(1, letters.length)]
    return msg ? <LetterScene key={scene.n} msg={msg} onDone={end} /> : null
  }
  const talk = talks[scene.i % Math.max(1, talks.length)]
  return talk ? <TalkScene key={scene.n} talk={talk} asker={asker} answerer={answerer} onDone={end} /> : null
}

/** A butterfly flies in carrying a banner on a thread, hovers so it can be read, then flies off. */
function LetterScene({ msg, onDone }: { msg: ButterflyMessage; onDone: () => void }) {
  const ref = useRef<HTMLDivElement>(null)
  const done = useRef(onDone)
  done.current = onDone
  useEffect(() => {
    const el = ref.current
    if (!el) return
    const W = window.innerWidth, H = window.innerHeight, phone = W < 640
    const hx = phone ? W * 0.5 : W * 0.62
    const hy = H * (phone ? 0.3 : 0.27)
    const IN = 3.2, HOLD = 4.4, OUT = 3, ALL = IN + HOLD + OUT
    const path = (t: number): Frame => {
      const wave = Math.sin(t * 2.2) * 12 + Math.sin(t * 0.8) * 8
      if (t < IN) { const k = easeOut(t / IN); return { x: W + 140 + (hx - W - 140) * k, y: hy + wave, r: (1 - k) * -9 } }
      if (t < IN + HOLD) { const h = t - IN; return { x: hx + Math.sin(h * 1.5) * 16, y: hy + wave + Math.cos(h * 1.5) * 5, r: Math.sin(h * 1.5) * 5 } }
      const k = easeIn(clamp01((t - IN - HOLD) / OUT))
      return { x: hx + (-380 - hx) * k, y: hy + wave - k * H * 0.08, r: -k * 11 }
    }
    const anim = el.animate(bake(path, ALL), { duration: ALL * 1000, easing: 'linear', fill: 'forwards' })
    anim.finished.then(() => done.current(), () => {})
    return () => anim.cancel()
  }, [])
  const body = (
    <>
      <span className="bf-letter-ic"><Mail className="h-3.5 w-3.5" /></span>
      <span>{msg.text}</span>
    </>
  )
  return (
    <div ref={ref} className="bf-messenger" data-no-texts>
      <Butterfly style={{ '--s': '44px', '--flap': '.24s' } as CSSProperties} />
      <span className="bf-thread" aria-hidden />
      <div className="bf-letter-swing">
        {!msg.to ? <span className="bf-letter">{body}</span>
          : /^https?:/.test(msg.to) ? <a href={msg.to} target="_blank" rel="noopener noreferrer" className="bf-letter">{body}</a>
          : <Link to={msg.to} className="bf-letter">{body}</Link>}
      </div>
    </div>
  )
}

/**
 * Two butterflies fly in from both sides and stop face to face, hovering. Then a little conversation, like a chat:
 * "typing…" over the one who asks, her question; "typing…" over the institute's butterfly, its answer.
 * They stay a moment so it can be read, then fly away.
 */
function TalkScene({ talk, asker, answerer, onDone }: { talk: ButterflyTalk; asker?: string | null; answerer?: string | null; onDone: () => void }) {
  const a = useRef<HTMLDivElement>(null)
  const b = useRef<HTMLDivElement>(null)
  const done = useRef(onDone)
  done.current = onDone
  const [geo] = useState(() => {
    const W = window.innerWidth, H = window.innerHeight, phone = W < 640
    const cy = H * (phone ? 0.25 : 0.27)
    const ax = phone ? W * 0.74 : W / 2 + 150 // the one who asks: on the right (read first in Arabic)
    const bx = phone ? W * 0.26 : W / 2 - 150 // the institute: on the left
    return {
      W, H, cy, ax, bx,
      // under each speaker, tail pointing up at her; on wide screens each bubble grows away from the other one
      q: phone ? { top: cy + 36, right: 12 } : { top: cy + 38, right: W - ax - 40 },
      r: phone ? { top: cy + 36 + 96, left: 12 } : { top: cy + 38, right: W - bx - 40 },
    }
  })
  // timeline (seconds)
  const IN = 2.4, MEET = 0.5
  const ASK_TYPING = IN + MEET, ASK = ASK_TYPING + 1.3
  const ANS_TYPING = ASK + 1.6, ANS = ANS_TYPING + 1.5
  const LEAVE = ANS + 5.2, OUT = 2.4, ALL = LEAVE + OUT
  useEffect(() => {
    const { W, H, cy, ax, bx } = geo
    // `face`: how much each one leans towards the other while they talk
    const fly = (from: number, at: number, to: number, face: number, phase: number) => (t: number): Frame => {
      const wave = Math.sin(t * 2.2 + phase) * 7 + Math.sin(t * 0.8 + phase) * 4
      if (t < IN) {
        const k = easeOut(t / IN)
        return { x: from + (at - from) * k, y: cy + wave + (1 - k) * 30, r: (1 - k) * (from > at ? -14 : 14) + k * face }
      }
      if (t < LEAVE) { // face to face, hovering gently in place
        const h = t - IN
        return { x: at + Math.sin(h * 1.1 + phase) * 6, y: cy + wave, r: face + Math.sin(h * 1.4 + phase) * 4 }
      }
      const k = easeIn(clamp01((t - LEAVE) / OUT))
      return { x: at + (to - at) * k, y: cy + wave - k * H * 0.14, r: face * (1 - k) + (to > at ? 14 : -14) * k }
    }
    const opts = { duration: ALL * 1000, easing: 'linear', fill: 'forwards' as const }
    const anims = [
      a.current!.animate(bake(fly(W + 90, ax, W + 140, -16, 0), ALL), opts),
      b.current!.animate(bake(fly(-90, bx, -140, 16, 1.7), ALL), opts),
    ]
    Promise.all(anims.map((x) => x.finished)).then(() => done.current(), () => {})
    return () => anims.forEach((x) => x.cancel())
  }, [geo, ALL, LEAVE])
  const t = (s: number) => `${s}s`
  const end = t(LEAVE - 0.3)
  const answerEnd = 'right' in geo.r && geo.r.right !== undefined && !('left' in geo.r)
  return (
    <div className="bfc" data-no-texts>
      <div ref={a} className="bfc-fly"><Butterfly style={{ '--s': '44px', '--flap': '.24s' } as CSSProperties} /></div>
      <div ref={b} className="bfc-fly inst"><Butterfly style={{ '--s': '50px', '--flap': '.27s' } as CSSProperties} /></div>

      <div className="bfc-typing ask" style={{ ...geo.q, ...({ '--in': t(ASK_TYPING), '--out': t(ASK) } as CSSProperties) }} aria-hidden><i /><i /><i /></div>
      <div className="bfc-bubble ask" style={{ ...geo.q, ...({ '--in': t(ASK), '--out': end } as CSSProperties) }}>
        {asker && <span className="bfc-name">{asker}</span>}
        <span>{talk.q}</span>
      </div>

      <div className={cx('bfc-typing answer', answerEnd && 'tail-end')} style={{ ...geo.r, ...({ '--in': t(ANS_TYPING), '--out': t(ANS) } as CSSProperties) }} aria-hidden><i /><i /><i /></div>
      <div className={cx('bfc-bubble answer', answerEnd && 'tail-end')} style={{ ...geo.r, ...({ '--in': t(ANS), '--out': end } as CSSProperties) }}>
        {answerer && <span className="bfc-name">{answerer}</span>}
        <span>{talk.a}</span>
      </div>
    </div>
  )
}
