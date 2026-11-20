import { useEffect } from 'react'

/**
 * Site-wide micro-interactions, all transform/opacity (compositor) and off for "reduce motion":
 * - desktop: a soft brand halo trails the cursor, and cards light up where the cursor is (spotlight);
 * - touch: a ripple of light blooms from the exact tap point, with a tiny haptic tick on Android.
 * Depth parallax follows the mouse on desktop only.
 */
export default function Fx() {
  useEffect(() => {
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return
    const fine = window.matchMedia('(hover: hover) and (pointer: fine)').matches
    const cleanups: (() => void)[] = []

    /* ---- scroll reveal: cards rise in one after another, photos open like a curtain, titles come out of a mist ---- */
    const KINDS: [string, string][] = [
      ['main article, main .card, main li.fx-item', 'fx-card'],
      ['main article img, main figure img, main .prose-ar img', 'fx-img'],
      ['main h2:not([aria-label]), main .eyebrow', 'fx-title'],
    ]
    const io = new IntersectionObserver((entries) => {
      for (const e of entries) {
        if (!e.isIntersecting) continue
        const el = e.target as HTMLElement
        el.classList.add('fx-in')
        io.unobserve(el)
      }
    }, { rootMargin: '0px 0px -8% 0px', threshold: 0.12 })
    const scan = () => {
      for (const [sel, cls] of KINDS) {
        document.querySelectorAll<HTMLElement>(sel).forEach((el) => {
          if (el.dataset.fx || el.closest('.intro-sky, [data-picker-ui], [data-no-fx]')) return
          el.dataset.fx = '1'
          // stagger siblings of the same grid / list
          const sibs = el.parentElement ? [...el.parentElement.children].filter((c) => c.tagName === el.tagName) : []
          el.style.setProperty('--fx-i', String(Math.min(8, Math.max(0, sibs.indexOf(el)))))
          el.classList.add(cls)
          io.observe(el)
        })
      }
    }
    document.documentElement.classList.add('fx-on')
    scan()
    // safety net: anything on screen even partly (end of page, inside a sideways slider…) is revealed anyway
    const sweep = window.setInterval(() => {
      document.querySelectorAll<HTMLElement>('[data-fx]:not(.fx-in)').forEach((el) => {
        const r = el.getBoundingClientRect()
        if (r.bottom > 0 && r.top < innerHeight && r.right > 0 && r.left < innerWidth) { el.classList.add('fx-in'); io.unobserve(el) }
      })
    }, 900)
    cleanups.push(() => clearInterval(sweep))
    let scanT = 0
    const mo = new MutationObserver((records) => {
      // the butterflies' scenes come and go above the page: they are not content, don't re-scan the page for them
      const page = records.some((r) => !(r.target instanceof Element && r.target.closest('[data-no-fx]'))
        && [...r.addedNodes].some((n) => !(n instanceof Element && n.closest('[data-no-fx]'))))
      if (!page) return
      clearTimeout(scanT); scanT = window.setTimeout(scan, 80)
    })
    mo.observe(document.body, { childList: true, subtree: true })
    cleanups.push(() => { io.disconnect(); mo.disconnect(); clearTimeout(scanT); document.documentElement.classList.remove('fx-on') })

    /* ---- tap ripple (all devices, strongest on touch) ---- */
    const layer = document.createElement('div')
    layer.className = 'fx-layer'
    layer.setAttribute('aria-hidden', 'true')
    layer.setAttribute('data-no-texts', '')
    document.body.appendChild(layer)
    const tap = (e: PointerEvent) => {
      const t = e.target as Element | null
      if (!t?.closest || t.closest('input, textarea, select, [contenteditable], .intro-sky, [data-picker-ui]')) return
      const hit = t.closest('a, button, [role=button], article, label, summary')
      if (!hit) return
      const r = document.createElement('span')
      r.className = 'fx-ripple' + (e.pointerType === 'mouse' ? ' mouse' : '')
      r.style.left = `${e.clientX}px`
      r.style.top = `${e.clientY}px`
      layer.appendChild(r)
      setTimeout(() => r.remove(), 700)
      if (e.pointerType === 'touch' && hit.matches('a, button') && 'vibrate' in navigator) {
        try { navigator.vibrate(6) } catch { /* not allowed */ }
      }
    }
    document.addEventListener('pointerdown', tap, { passive: true })
    cleanups.push(() => { document.removeEventListener('pointerdown', tap); layer.remove() })

    /* ---- depth scenes: layers follow the mouse (desktop) or the phone's tilt (no permission prompt; iOS skips) ---- */
    let dx = 0, dy = 0, cx = 0, cy = 0, draf = 0
    const depthLoop = () => {
      cx += (dx - cx) * 0.08
      cy += (dy - cy) * 0.08
      document.querySelectorAll<HTMLElement>('[data-depth-scene]').forEach((s) => {
        s.style.setProperty('--tx', cx.toFixed(3))
        s.style.setProperty('--ty', cy.toFixed(3))
      })
      draf = Math.abs(dx - cx) + Math.abs(dy - cy) > 0.002 ? requestAnimationFrame(depthLoop) : 0
    }
    const aim = (x: number, y: number) => {
      dx = Math.max(-1, Math.min(1, x))
      dy = Math.max(-1, Math.min(1, y))
      if (!draf) draf = requestAnimationFrame(depthLoop)
    }
    if (fine) {
      const mm = (e: PointerEvent) => aim((e.clientX / innerWidth) * 2 - 1, (e.clientY / innerHeight) * 2 - 1)
      document.addEventListener('pointermove', mm, { passive: true })
      cleanups.push(() => document.removeEventListener('pointermove', mm))
    }
    // phones: no tilt-driven parallax. The gyroscope never rests (hand tremor), so it kept a style
    // recalculation running on every frame while scrolling, which was a main source of jank.
    cleanups.push(() => cancelAnimationFrame(draf))

    if (fine) {
      /* ---- cursor halo (lags slightly behind the cursor for a fluid feel) ---- */
      const halo = document.createElement('div')
      halo.className = 'fx-halo'
      halo.setAttribute('aria-hidden', 'true')
      document.body.appendChild(halo)
      let tx = innerWidth / 2, ty = innerHeight / 2, x = tx, y = ty, raf = 0, shown = false
      const loop = () => {
        x += (tx - x) * 0.14
        y += (ty - y) * 0.14
        halo.style.transform = `translate3d(${x - 200}px, ${y - 200}px, 0)`
        raf = Math.abs(tx - x) + Math.abs(ty - y) > 0.5 ? requestAnimationFrame(loop) : 0
      }
      /* ---- spotlight on the card under the cursor ---- */
      let spot: HTMLElement | null = null
      const move = (e: PointerEvent) => {
        tx = e.clientX
        ty = e.clientY
        if (!shown) { shown = true; halo.classList.add('on') }
        if (!raf) raf = requestAnimationFrame(loop)
        const el = (e.target as Element | null)?.closest?.('article, .card, [data-spot]') as HTMLElement | null
        if (spot && spot !== el) spot.removeAttribute('data-spot-on')
        spot = el && !el.closest('[data-picker-ui], .intro-sky') ? el : null
        if (spot) {
          const b = spot.getBoundingClientRect()
          spot.style.setProperty('--mx', `${e.clientX - b.left}px`)
          spot.style.setProperty('--my', `${e.clientY - b.top}px`)
          spot.setAttribute('data-spot-on', '')
        }
      }
      const leave = () => { shown = false; halo.classList.remove('on'); spot?.removeAttribute('data-spot-on'); spot = null }
      document.addEventListener('pointermove', move, { passive: true })
      document.documentElement.addEventListener('pointerleave', leave)
      cleanups.push(() => {
        document.removeEventListener('pointermove', move)
        document.documentElement.removeEventListener('pointerleave', leave)
        cancelAnimationFrame(raf)
        halo.remove()
      })
    }
    return () => cleanups.forEach((f) => f())
  }, [])
  return null
}
