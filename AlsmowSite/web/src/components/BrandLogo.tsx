import { motion } from 'motion/react'
import { cx } from '../lib/format'

/**
 * The institute's logo as a round badge: a white disc with a brand→gold ring, the whole logo inside
 * (wings + name, nothing cropped: the square image sits inside the circle at 76%), a soft entrance,
 * and the ring turns on hover.
 */
export default function BrandLogo({ src, className, animate = true }: { src?: string | null; className?: string; animate?: boolean }) {
  return (
    <motion.span
      className={cx('brand-logo', className)}
      initial={animate ? { scale: 0.6, rotate: -90, opacity: 0 } : false}
      animate={{ scale: 1, rotate: 0, opacity: 1 }}
      transition={{ type: 'spring', stiffness: 220, damping: 18 }}
      whileHover={{ scale: 1.06 }}
    >
      <span className="brand-logo-disc">
        <img src={src || '/logo.jpeg'} alt="" draggable={false} />
      </span>
    </motion.span>
  )
}
