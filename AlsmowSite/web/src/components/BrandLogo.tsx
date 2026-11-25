import { motion } from 'motion/react'
import { useEffect, useState } from 'react'
import { cx } from '../lib/format'
import { useSite } from '../lib/queries'

/**
 * The institute's logo as a badge: a white tile with a brand→gold ring and the whole logo inside.
 * Uploaded logos often carry wide empty margins; the badge measures the drawing itself (once per image) and
 * fills the tile with it, so the logo reads at full size whatever image is chosen in the dashboard.
 */
type Box = { x: number; y: number; w: number; h: number; a: number }
const boxes = new Map<string, Box | null>()

function measure(src: string): Promise<Box | null> {
  return new Promise((done) => {
    const img = new Image()
    img.decoding = 'async'
    img.onload = () => {
      try {
        const S = 160
        const a = img.naturalWidth / img.naturalHeight || 1
        const cw = a >= 1 ? S : Math.round(S * a), ch = a >= 1 ? Math.round(S / a) : S
        const c = document.createElement('canvas')
        c.width = cw; c.height = ch
        const ctx = c.getContext('2d', { willReadFrequently: true })
        if (!ctx) return done(null)
        ctx.drawImage(img, 0, 0, cw, ch)
        const d = ctx.getImageData(0, 0, cw, ch).data
        let x0 = cw, y0 = ch, x1 = -1, y1 = -1
        for (let y = 0; y < ch; y++) for (let x = 0; x < cw; x++) {
          const i = (y * cw + x) * 4
          // ink = not transparent and not (near) white
          if (d[i + 3] > 24 && (d[i] < 236 || d[i + 1] < 236 || d[i + 2] < 236)) { if (x < x0) x0 = x; if (x > x1) x1 = x; if (y < y0) y0 = y; if (y > y1) y1 = y }
        }
        if (x1 < 0) return done(null)
        done({ x: x0 / cw, y: y0 / ch, w: (x1 - x0 + 1) / cw, h: (y1 - y0 + 1) / ch, a })
      } catch { done(null) } // a cross-origin image can't be read: keep the plain fit
    }
    img.onerror = () => done(null)
    img.src = src
  })
}

function useBox(src: string) {
  const [box, setBox] = useState<Box | null | undefined>(() => boxes.get(src))
  useEffect(() => {
    if (!src) return
    if (boxes.has(src)) { setBox(boxes.get(src)); return }
    let live = true
    measure(src).then((b) => { boxes.set(src, b); if (live) setBox(b) })
    return () => { live = false }
  }, [src])
  return box
}

export default function BrandLogo({ src, className, animate = true }: { src?: string | null; className?: string; animate?: boolean }) {
  // no explicit image → the logo chosen in the dashboard (Settings → General), so the admin panel and login follow it too
  const { data } = useSite()
  // until the settings arrive, the tile stays empty (no throw-away download of a placeholder image)
  const img = src || data?.settings.general.logo || (data ? '/logo.png' : '')
  const box = useBox(img)
  // fit the drawing (not the file) into the tile: k = how many tile-widths the whole image spans
  const fit = box ? (() => {
    const k = Math.min(1 / box.w, box.a / box.h)
    return {
      width: `${k * 100}%`,
      left: `${50 - (box.x + box.w / 2) * k * 100}%`,
      top: `${50 - (box.y + box.h / 2) * (k / box.a) * 100}%`,
    }
  })() : null
  return (
    <motion.span
      className={cx('brand-logo', className)}
      initial={animate ? { scale: 0.6, rotate: -90, opacity: 0 } : false}
      animate={{ scale: 1, rotate: 0, opacity: 1 }}
      transition={{ type: 'spring', stiffness: 220, damping: 18 }}
      whileHover={{ scale: 1.06 }}
    >
      <span className="brand-logo-disc">
        {img && (fit
          ? <span className="brand-logo-fit"><img src={img} alt="" draggable={false} decoding="async" style={fit} /></span>
          : <img src={img} alt="" draggable={false} decoding="async" />)}
      </span>
    </motion.span>
  )
}
