import { useQuery } from '@tanstack/react-query'
import { X } from 'lucide-react'
import { useEffect, useMemo, useRef, useState, type CSSProperties } from 'react'
import { Link } from 'react-router-dom'
import { LITE } from '../components/ui'
import { api, qs } from '../lib/api'
import { useSite } from '../lib/queries'
import { stage } from '../lib/stage'
import type { Program } from '../lib/types'

/**
 * The programmes box: a small wrapped box in the floating column (above WhatsApp). Now and then its lid pops and a few
 * small envelopes fly out one after another and float up the page, swaying; each carries a programme's name and opens
 * its page on a tap; under the pointer an envelope stops so it can be read. Long quiet gaps between waves, and the
 * visitor can close it for the visit. Pure CSS art (no images), theme colours, transform/opacity animations only.
 * Every word comes from the dashboard (Settings › صندوق البرامج) or from the programmes themselves.
 */

const OFF_KEY = 'alsmow.dock.off' // sessionStorage: the visitor closed it for this visit
type Flyer = { key: number; p: Program; px: number; py: number; dx: number; r: number; delay: number }

const rnd = (i: number, n: number) => (Math.sin(i * 97.13 + n * 13.7) + 1) / 2

function shuffle<T>(a: T[]): T[] {
  const b = [...a]
  for (let i = b.length - 1; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [b[i], b[j]] = [b[j], b[i]] }
  return b
}

function allowed() {
  try {
    if (/^\/admin/.test(location.pathname)) return false
    const q = new URLSearchParams(location.search)
    if (q.has('gift')) return true
    if (q.has('edit') || sessionStorage.getItem('alsmow.texts.edit') === '1') return false
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return false
    return sessionStorage.getItem(OFF_KEY) !== '1'
  } catch {
    return false
  }
}

export default function GiftBox() {
  const { data } = useSite()
  const g = data?.settings.gift
  const forced = typeof location !== 'undefined' && new URLSearchParams(location.search).has('gift')
  const on = !!g && (forced || g.mode !== 'off') && !(LITE && g.mobile_hide && !forced)
  const [ok] = useState(allowed)
  const [visible, setVisible] = useState(false)
  const [open, setOpen] = useState(false) // lid up while a wave flies out
  const [flyers, setFlyers] = useState<Flyer[]>([])
  const next = useRef(0) // index of the next programme in the shuffled list
  const serial = useRef(0)
  const timers = useRef<number[]>([])
  const later = (ms: number, f: () => void) => { timers.current.push(window.setTimeout(f, ms)) }
  const clear = () => { timers.current.forEach(clearTimeout); timers.current = [] }

  const source = g?.source && g.source !== 'all' ? g.source : undefined
  const { data: programs } = useQuery({
    queryKey: ['programs', source ?? 'all'],
    queryFn: () => api.get<Program[]>(`/public/programs${qs({ type: source })}`),
    enabled: ok && on,
  })
  const list = useMemo(() => shuffle((programs ?? []).filter((p) => p.title)), [programs])

  const first = (Number(g?.delay) || 6) * 1000
  const every = (Number(g?.every) || 45) * 1000
  const flight = (Number(g?.flight) || 12) * 1000
  const count = Math.max(1, Math.min(Number(g?.count) || 3, LITE ? 1 : 5, list.length || 1)) // phones: one at a time (a calm screen)
  const STAGGER = 900

  // the box appears once the opening doves are gone
  useEffect(() => {
    if (!ok || !on || !list.length || visible) return
    const iv = window.setInterval(() => {
      if (document.documentElement.classList.contains('intro-on')) return
      window.clearInterval(iv)
      setVisible(true)
    }, 400)
    return () => window.clearInterval(iv)
  }, [ok, on, list.length, visible])

  // one wave: the lid pops, `count` envelopes fly out one after another, then the lid closes; a quiet gap; the next wave
  const wave = () => {
    clear()
    if (document.hidden || !stage.claim('dock')) { later(3000, wave); return } // the butterflies may be on stage
    setOpen(true)
    // the envelopes fan out, each in its own direction (the box is at the left edge, so: from straight up to across the page)
    const vw = window.innerWidth, vh = window.innerHeight
    const reach = LITE ? Math.min(vh * 0.26, 220) : Math.min(vh * 0.42, 380) // phones: a short hop, so it opens low (not over the hero faces)
    const fw = LITE ? 128 : 156
    const batch: Flyer[] = Array.from({ length: count }, (_, i) => {
      const k = serial.current++
      const p = list[next.current++ % list.length]
      const f = count > 1 ? i / (count - 1) : 0.5
      const a = (82 - f * 66 + (rnd(k, 1) - 0.5) * 8) * (Math.PI / 180) // 82deg (up) ... 16deg (across)
      const r = reach * (0.85 + rnd(k, 2) * 0.25)
      const px = Math.round(Math.min(Math.cos(a) * r * (LITE ? 1.1 : 1.7), vw - fw - 36))
      const py = Math.round(-Math.sin(a) * r - 40)
      return { key: k, p, px: Math.max(0, px), py, dx: Math.round((rnd(k, 3) - 0.3) * 40), r: (rnd(k, 9) - 0.5) * 18, delay: i * STAGGER }
    })
    setFlyers((f) => [...f, ...batch])
    later(count * STAGGER + 500, () => setOpen(false))
    later(count * STAGGER + flight, () => stage.release('dock'))
    later(count * STAGGER + flight + every, wave) // each envelope removes itself when its flight ends
  }

  useEffect(() => {
    if (!visible) return
    later(first, wave)
    return clear
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [visible])

  const close = () => {
    clear()
    stage.release('dock')
    try { sessionStorage.setItem(OFF_KEY, '1') } catch { /* private mode */ }
    setVisible(false)
    setFlyers([])
  }

  // the celebration each envelope bursts with when it opens
  const burst = useMemo(() => Array.from({ length: LITE ? 10 : 14 }, (_, i) => {
    const a = (i / (LITE ? 10 : 14)) * Math.PI * 2 + rnd(i, 7) * 0.4
    const d = 46 + rnd(i, 8) * 46
    return { x: Math.round(Math.cos(a) * d), y: Math.round(Math.sin(a) * d * 0.8 - 18), r: Math.round((rnd(i, 9) - 0.5) * 720), tone: i % 4, shape: i % 3 }
  }), [])
  const pops = useMemo(() => Array.from({ length: 7 }, (_, i) => {
    const a = Math.PI * (0.15 + (i / 6) * 0.7)
    const d = 30 + rnd(i, 1) * 28
    return { x: -Math.cos(a) * d, y: -Math.sin(a) * d - 8, s: 6 + rnd(i, 2) * 6, tone: i % 3 }
  }), [])

  if (!visible || !list.length) return null

  return (
    <div className={`dock${open ? ' dock-open' : ''}`} data-no-texts data-no-fx style={{ '--fly': `${flight}ms` } as CSSProperties}>
      {flyers.map((f) => (
        <Link
          key={f.key}
          to={`/programs/${f.p.slug}`}
          className="dock-fly"
          onAnimationEnd={(e) => { if (e.target === e.currentTarget) setFlyers((all) => all.filter((x) => x.key !== f.key)) }}
          style={{ '--px': `${f.px}px`, '--py': `${f.py}px`, '--dx': `${f.dx}px`, '--rot': `${f.r}deg`, '--d': `${f.delay}ms` } as CSSProperties}
        >
          <span className="fly-env">
            <span className="fly-back" />
            <span className="fly-paper"><b>{f.p.title}</b></span>
            <span className="fly-front" />
            <span className="fly-flap" />
            <span className="fly-shine" />
            <span className="fly-seal"><svg viewBox="0 0 24 24"><path d="M12 2.5l2.6 6.1 6.6.5-5 4.3 1.6 6.4L12 16.3l-5.8 3.5 1.6-6.4-5-4.3 6.6-.5z" /></svg></span>
            <span className="fly-ring" />
            <span className="fly-burst">{burst.map((c, j) => <i key={j} className={`t${c.tone} s${c.shape}`} style={{ '--bx': `${c.x}px`, '--by': `${c.y}px`, '--br': `${c.r}deg` } as CSSProperties} />)}</span>
            <span className="fly-spark"><i /><i /><i /><i /></span>
          </span>
        </Link>
      ))}

      {/* the box itself: a tap sends a wave right away */}
      <button type="button" className="dock-box" onClick={() => { if (!open) wave() }} aria-label={list[next.current % list.length]?.title}>
        <span className="dock-halo" aria-hidden />
        <span className="dock-rays" aria-hidden />
        <span className="dock-pops" aria-hidden>
          {pops.map((b, i) => <i key={i} className={`t${b.tone}`} style={{ '--x': `${b.x}px`, '--y': `${b.y}px`, '--sz': `${b.s}px` } as CSSProperties} />)}
        </span>
        <span className="gift-body" />
        <span className="gift-lid"><span className="gift-bow"><i /><i /><b /></span></span>
      </button>
      <button type="button" className="dock-close" onClick={close} aria-label="×"><X className="h-3 w-3" /></button>
    </div>
  )
}
