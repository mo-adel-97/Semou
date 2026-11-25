import { useEffect, useRef } from 'react'
import { LITE } from '../../components/ui'
import { cx } from '../../lib/format'
import { REDUCED } from './kit'

/**
 * A living web of points (the «spider web» behind the hero): dots in the identity colours drift slowly,
 * near ones are tied by fine threads, and the threads reach for the mouse. Canvas, paused off screen.
 */
export default function Plexus({ colors, className }: { colors: string[]; className?: string }) {
  const ref = useRef<HTMLCanvasElement>(null)
  useEffect(() => {
    const cv = ref.current
    const ctx = cv?.getContext('2d')
    if (!cv || !ctx) return
    const css = getComputedStyle(document.documentElement)
    const rgb = (c: string) => {
      let v = c.trim()
      const m = v.match(/^var\((--[\w-]+)\)$/)
      if (m) v = css.getPropertyValue(m[1]).trim()
      const h = v.replace('#', '')
      const f = h.length === 3 ? h.split('').map((x) => x + x).join('') : h
      const n = parseInt(f, 16)
      return Number.isNaN(n) ? [31, 57, 108] : [(n >> 16) & 255, (n >> 8) & 255, n & 255]
    }
    const pal = colors.map(rgb)
    const ink = rgb('var(--brand)')
    const dark = document.documentElement.dataset.mode === 'dark'
    const dpr = Math.min(window.devicePixelRatio || 1, 2)
    let W = 0, H = 0
    type N = { x: number; y: number; vx: number; vy: number; r: number; c: number[] }
    let nodes: N[] = []
    const LINK = LITE ? 110 : 150
    const seed = () => {
      const count = Math.min(LITE ? 38 : 95, Math.round((W * H) / (LITE ? 9000 : 13000)))
      nodes = Array.from({ length: count }, (_, i) => ({
        x: Math.random() * W, y: Math.random() * H,
        vx: (Math.random() - 0.5) * 0.28, vy: (Math.random() - 0.5) * 0.28,
        r: 1.4 + Math.random() * 2.2, c: pal[i % pal.length],
      }))
    }
    const size = () => {
      const r = cv.getBoundingClientRect()
      W = r.width; H = r.height
      cv.width = Math.round(W * dpr); cv.height = Math.round(H * dpr)
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0)
      seed()
    }
    size()
    const ro = new ResizeObserver(size)
    ro.observe(cv)
    const mouse = { x: -9999, y: -9999 }
    const move = (e: PointerEvent) => { const r = cv.getBoundingClientRect(); mouse.x = e.clientX - r.left; mouse.y = e.clientY - r.top }
    const leave = () => { mouse.x = -9999; mouse.y = -9999 }
    if (!LITE) { window.addEventListener('pointermove', move, { passive: true }); document.addEventListener('pointerleave', leave) }

    const draw = () => {
      ctx.clearRect(0, 0, W, H)
      for (const n of nodes) {
        if (!REDUCED) {
          n.x += n.vx; n.y += n.vy
          if (n.x < -20) n.x = W + 20; else if (n.x > W + 20) n.x = -20
          if (n.y < -20) n.y = H + 20; else if (n.y > H + 20) n.y = -20
        }
      }
      ctx.lineWidth = 1
      for (let i = 0; i < nodes.length; i++) {
        const a = nodes[i]
        for (let j = i + 1; j < nodes.length; j++) {
          const b = nodes[j]
          const dx = a.x - b.x, dy = a.y - b.y
          const d = Math.hypot(dx, dy)
          if (d < LINK) {
            ctx.strokeStyle = `rgba(${ink[0]},${ink[1]},${ink[2]},${(1 - d / LINK) * (dark ? 0.4 : 0.28)})`
            ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(b.x, b.y); ctx.stroke()
          }
        }
        const md = Math.hypot(a.x - mouse.x, a.y - mouse.y)
        if (md < 190) {
          ctx.strokeStyle = `rgba(${a.c[0]},${a.c[1]},${a.c[2]},${(1 - md / 190) * 0.7})`
          ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(mouse.x, mouse.y); ctx.stroke()
          a.x += (mouse.x - a.x) * 0.002; a.y += (mouse.y - a.y) * 0.002
        }
      }
      for (const n of nodes) {
        ctx.fillStyle = `rgba(${n.c[0]},${n.c[1]},${n.c[2]},.9)`
        ctx.beginPath(); ctx.arc(n.x, n.y, n.r, 0, Math.PI * 2); ctx.fill()
      }
    }

    let raf = 0, on = true
    const loop = () => { draw(); if (on && !REDUCED) raf = requestAnimationFrame(loop) }
    const io = new IntersectionObserver(([e]) => {
      on = e.isIntersecting
      cancelAnimationFrame(raf)
      if (on) loop()
    })
    io.observe(cv)
    loop()
    return () => {
      cancelAnimationFrame(raf); io.disconnect(); ro.disconnect()
      window.removeEventListener('pointermove', move); document.removeEventListener('pointerleave', leave)
    }
  }, [colors.join("|")]) // eslint-disable-line react-hooks/exhaustive-deps
  return <canvas ref={ref} className={cx('pointer-events-none block h-full w-full', className)} aria-hidden />
}
