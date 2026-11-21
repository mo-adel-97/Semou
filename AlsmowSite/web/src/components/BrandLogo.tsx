import { motion } from 'motion/react'
import { cx } from '../lib/format'
import { useSite } from '../lib/queries'

/**
 * The institute's logo as a round badge: a white disc with a brand→gold ring, the whole logo inside
 * (wings + name, nothing cropped: the square image sits inside the circle at 76%), a soft entrance,
 * and the ring turns on hover.
 */
export default function BrandLogo({ src, className, animate = true }: { src?: string | null; className?: string; animate?: boolean }) {
  // no explicit image → the logo chosen in the dashboard (Settings → General), so the admin panel and login follow it too
  const { data } = useSite()
  // until the settings arrive, the disc stays empty (no throw-away download of a placeholder image)
  const img = src || data?.settings.general.logo || (data ? '/logo.png' : '')
  return (
    <motion.span
      className={cx('brand-logo', className)}
      initial={animate ? { scale: 0.6, rotate: -90, opacity: 0 } : false}
      animate={{ scale: 1, rotate: 0, opacity: 1 }}
      transition={{ type: 'spring', stiffness: 220, damping: 18 }}
      whileHover={{ scale: 1.06 }}
    >
      <span className="brand-logo-disc">
        {img && <img src={img} alt="" draggable={false} decoding="async" />}
      </span>
    </motion.span>
  )
}
