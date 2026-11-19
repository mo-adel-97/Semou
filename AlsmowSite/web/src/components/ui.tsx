import Lenis from 'lenis'
import {
  animate, motion, useInView, useMotionTemplate, useMotionValue, useReducedMotion, useScroll, useSpring, useTransform,
  type MotionValue,
} from 'motion/react'
import { createContext, useContext, useEffect, useRef, useState, type ReactNode, type RefObject } from 'react'
import { cx } from '../lib/format'

const EASE = [0.22, 1, 0.36, 1] as const

/* ---------------- Smooth scroll (Lenis) ---------------- */

const LenisCtx = createContext<Lenis | null>(null)
export const useLenis = () => useContext(LenisCtx)

export function SmoothScroll({ children }: { children: ReactNode }) {
  const reduce = useReducedMotion()
  const [lenis, setLenis] = useState<Lenis | null>(null)
  useEffect(() => {
    if (reduce) return
    const l = new Lenis({ duration: 1.15, easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t)), smoothWheel: true })
    setLenis(l)
    let raf = 0
    const loop = (t: number) => { l.raf(t); raf = requestAnimationFrame(loop) }
    raf = requestAnimationFrame(loop)
    return () => { cancelAnimationFrame(raf); l.destroy(); setLenis(null) }
  }, [reduce])
  return <LenisCtx.Provider value={lenis}>{children}</LenisCtx.Provider>
}

export function ScrollProgress() {
  const { scrollYProgress } = useScroll()
  const x = useSpring(scrollYProgress, { stiffness: 140, damping: 30, mass: 0.3 })
  return <motion.div style={{ scaleX: x }} className="fixed inset-x-0 top-0 z-[70] h-[3px] origin-right bg-gradient-to-l from-brand-500 via-glow to-gold-400" />
}

/* ---------------- Reveal / stagger ---------------- */

export function Reveal({ children, delay = 0, className, y = 28 }: { children: ReactNode; delay?: number; className?: string; y?: number }) {
  return (
    <motion.div
      className={className}
      initial={{ opacity: 0, y, filter: 'blur(6px)' }}
      whileInView={{ opacity: 1, y: 0, filter: 'blur(0px)' }}
      viewport={{ once: true, margin: '-70px' }}
      transition={{ duration: 0.8, delay, ease: EASE }}
    >
      {children}
    </motion.div>
  )
}

export const stagger = {
  container: { hidden: {}, show: { transition: { staggerChildren: 0.08, delayChildren: 0.05 } } },
  item: { hidden: { opacity: 0, y: 30, filter: 'blur(6px)' }, show: { opacity: 1, y: 0, filter: 'blur(0px)', transition: { duration: 0.7, ease: EASE } } },
}

/** Word-by-word headline reveal. Words in `accent` get the gradient. */
export function SplitText({ text, accent, className, delay = 0, as = 'h2', immediate }: {
  text: string; accent?: string; className?: string; delay?: number; as?: 'h1' | 'h2' | 'h3' | 'p'; immediate?: boolean
}) {
  const Tag = motion[as]
  const accentWords = new Set((accent ?? '').split(/\s+/).filter(Boolean))
  const words = text.split(/\s+/)
  const anim = immediate ? { animate: 'show' } : { whileInView: 'show', viewport: { once: true, margin: '-60px' } }
  return (
    <Tag className={className} initial="hidden" {...anim} variants={{ hidden: {}, show: { transition: { staggerChildren: 0.06, delayChildren: delay } } }} aria-label={text}>
      {words.map((w, i) => (
        <span key={i} className="-mb-[0.2em] inline-block overflow-hidden pt-[0.05em] pb-[0.3em] align-top" aria-hidden="true">
          <motion.span
            className={cx('inline-block', accentWords.has(w) && 'text-gradient')}
            variants={{ hidden: { y: '110%', rotate: 4 }, show: { y: '0%', rotate: 0, transition: { duration: 0.85, ease: EASE } } }}
          >
            {w}
          </motion.span>
          {i < words.length - 1 && ' '}
        </span>
      ))}
    </Tag>
  )
}

/* ---------------- Interactive surfaces ---------------- */

/** 3D tilt that follows the pointer + a soft spotlight. */
export function Tilt({ children, className, max = 7 }: { children: ReactNode; className?: string; max?: number }) {
  const ref = useRef<HTMLDivElement>(null)
  const reduce = useReducedMotion()
  const px = useMotionValue(0.5)
  const py = useMotionValue(0.5)
  const rx = useSpring(useTransform(py, [0, 1], [max, -max]), { stiffness: 200, damping: 20 })
  const ry = useSpring(useTransform(px, [0, 1], [-max, max]), { stiffness: 200, damping: 20 })
  const gx = useTransform(px, (v) => `${v * 100}%`)
  const gy = useTransform(py, (v) => `${v * 100}%`)
  const spot = useMotionTemplate`radial-gradient(420px circle at ${gx} ${gy}, color-mix(in oklab, var(--glow) 22%, transparent), transparent 60%)`
  if (reduce) return <div className={className}>{children}</div>
  return (
    <motion.div
      ref={ref}
      className={cx('relative [transform-style:preserve-3d]', className)}
      style={{ rotateX: rx, rotateY: ry, transformPerspective: 1100 }}
      onPointerMove={(e) => {
        const r = ref.current!.getBoundingClientRect()
        px.set((e.clientX - r.left) / r.width)
        py.set((e.clientY - r.top) / r.height)
      }}
      onPointerLeave={() => { px.set(0.5); py.set(0.5) }}
    >
      {children}
      <motion.div className="pointer-events-none absolute inset-0 rounded-[inherit] opacity-0 transition-opacity duration-300 group-hover:opacity-100" style={{ background: spot }} />
    </motion.div>
  )
}

/** Gently pulls its child toward the cursor. */
export function Magnetic({ children, strength = 0.3 }: { children: ReactNode; strength?: number }) {
  const ref = useRef<HTMLDivElement>(null)
  const x = useSpring(0, { stiffness: 250, damping: 15 })
  const y = useSpring(0, { stiffness: 250, damping: 15 })
  return (
    <motion.div
      ref={ref}
      className="inline-block"
      style={{ x, y }}
      onPointerMove={(e) => {
        if (e.pointerType !== 'mouse') return
        const r = ref.current!.getBoundingClientRect()
        x.set((e.clientX - r.left - r.width / 2) * strength)
        y.set((e.clientY - r.top - r.height / 2) * strength)
      }}
      onPointerLeave={() => { x.set(0); y.set(0) }}
    >
      {children}
    </motion.div>
  )
}

export function useParallax(range = 80): [RefObject<HTMLDivElement | null>, MotionValue<number>] {
  const ref = useRef<HTMLDivElement>(null)
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start end', 'end start'] })
  const y = useTransform(scrollYProgress, [0, 1], [range, -range])
  return [ref, y]
}

/* ---------------- Section heading ---------------- */

export function SectionHeading({ eyebrow, title, accent, text, align = 'center', action, light }: {
  eyebrow?: string; title: string; accent?: string; text?: string; align?: 'center' | 'start'; action?: ReactNode; light?: boolean
}) {
  return (
    <div className={cx('mb-12 flex flex-col gap-5 md:mb-16', align === 'center' ? 'items-center text-center' : 'items-start md:flex-row md:items-end md:justify-between')}>
      <div className={cx('flex flex-col gap-4', align === 'center' && 'items-center')}>
        {eyebrow && (
          <Reveal>
            <span className={cx('eyebrow', light && '!border-white/15 !bg-white/10 !text-gold-300')}>
              <span className="h-1.5 w-1.5 rounded-full bg-current" />
              {eyebrow}
            </span>
          </Reveal>
        )}
        <SplitText text={title} accent={accent} className={cx('max-w-3xl text-[clamp(1.8rem,1.1rem+2.4vw,3rem)] leading-[1.25] font-black tracking-tight', light ? 'text-white' : 'text-ink')} />
        {text && (
          <Reveal delay={0.15}>
            <p className={cx('max-w-2xl text-lg leading-relaxed text-pretty', light ? 'text-white/70' : 'text-muted')}>{text}</p>
          </Reveal>
        )}
      </div>
      {action && <Reveal delay={0.2}>{action}</Reveal>}
    </div>
  )
}

/* ---------------- Small pieces ---------------- */

export function Counter({ value, suffix = '' }: { value: number; suffix?: string }) {
  const ref = useRef<HTMLSpanElement>(null)
  const inView = useInView(ref, { once: true })
  const mv = useMotionValue(0)
  const text = useTransform(mv, (v) => Math.round(v).toLocaleString('en-US'))
  useEffect(() => {
    if (!inView) return
    const c = animate(mv, value, { duration: 2.2, ease: [0.16, 1, 0.3, 1] })
    return () => c.stop()
  }, [inView, mv, value])
  return (
    <span ref={ref} className="tabular-nums">
      <motion.span>{text}</motion.span>
      {suffix}
    </span>
  )
}

export function Skeleton({ className }: { className?: string }) {
  return <div className={cx('relative overflow-hidden rounded-2xl bg-sand before:absolute before:inset-0 before:animate-shine before:bg-gradient-to-l before:from-transparent before:via-surface/60 before:to-transparent', className)} />
}

/** Image that fades in once decoded; branded placeholder when missing. */
export function Img({ src, alt, className, eager }: { src?: string | null; alt: string; className?: string; eager?: boolean }) {
  const [loaded, setLoaded] = useState(false)
  if (!src) {
    return (
      <div className={cx('pattern flex items-center justify-center bg-brand-50', className)}>
        <img src="/logo.jpeg" alt="" className="h-14 w-14 rounded-2xl opacity-40 grayscale" />
      </div>
    )
  }
  return (
    <img
      src={src}
      alt={alt}
      loading={eager ? 'eager' : 'lazy'}
      decoding="async"
      onLoad={() => setLoaded(true)}
      ref={(el) => { if (el?.complete && el.naturalWidth) setLoaded(true) }}
      className={cx('bg-sand object-cover transition-[opacity,filter,transform] duration-700', loaded ? 'opacity-100 blur-0' : 'opacity-0 blur-md', className)}
    />
  )
}

export function Stars({ n }: { n: number }) {
  return (
    <div className="flex gap-0.5" aria-label={`${n} من 5`}>
      {Array.from({ length: 5 }, (_, i) => (
        <svg key={i} viewBox="0 0 20 20" className={cx('h-4 w-4', i < n ? 'fill-gold-400' : 'fill-line')}>
          <path d="M10 1.5l2.6 5.6 6.1.7-4.5 4.2 1.2 6-5.4-3-5.4 3 1.2-6L1.3 7.8l6.1-.7z" />
        </svg>
      ))}
    </div>
  )
}

/** Animated aurora blobs used behind heroes. */
export function Aurora({ className }: { className?: string }) {
  return (
    <div className={cx('pointer-events-none absolute inset-0 overflow-hidden', className)} aria-hidden="true">
      <div className="absolute -top-1/3 -left-1/4 h-[70%] w-[60%] animate-aurora rounded-full bg-brand-300/40 blur-[100px] dark:bg-brand-500/25" />
      <div className="absolute -right-1/4 top-0 h-[60%] w-[55%] animate-aurora rounded-full bg-gold-300/35 blur-[110px] [animation-delay:-6s] dark:bg-gold-500/15" />
      <div className="absolute -bottom-1/3 left-1/4 h-[55%] w-[50%] animate-aurora rounded-full bg-glow/30 blur-[120px] [animation-delay:-12s] dark:bg-glow/10" />
    </div>
  )
}

/** Light page header used on every inner page (keeps the floating nav readable). */
export function PageHero({ title, accent, text, crumbs, children }: { title: string; accent?: string; text?: string | null; crumbs?: ReactNode; children?: ReactNode }) {
  return (
    <section className="relative overflow-hidden pt-36 pb-14 md:pt-44 md:pb-20">
      <Aurora />
      <div className="pattern absolute inset-0 [mask-image:radial-gradient(ellipse_at_top,black,transparent_75%)]" />
      <div className="container-x relative">
        {crumbs && (
          <motion.nav initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="mb-6 inline-flex items-center gap-2 rounded-full border border-line bg-surface/70 px-4 py-1.5 text-sm text-muted backdrop-blur" aria-label="مسار الصفحة">
            {crumbs}
          </motion.nav>
        )}
        <SplitText as="h1" immediate text={title} accent={accent} className="max-w-4xl text-[clamp(2rem,1.2rem+2.8vw,3.5rem)] leading-[1.2] font-black tracking-tight text-ink" />
        {text && (
          <motion.p initial={{ opacity: 0, y: 16 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: 0.3, ease: EASE }} className="mt-6 max-w-2xl text-lg leading-relaxed text-pretty text-muted md:text-xl">
            {text}
          </motion.p>
        )}
        {children}
      </div>
    </section>
  )
}

export function EmptyState({ title, text, icon }: { title: string; text?: string; icon?: ReactNode }) {
  return (
    <div className="card flex flex-col items-center gap-3 px-6 py-16 text-center">
      {icon && <div className="mb-2 rounded-2xl bg-brand-50 p-4 text-brand-600">{icon}</div>}
      <h3 className="text-xl font-bold">{title}</h3>
      {text && <p className="max-w-md text-muted">{text}</p>}
    </div>
  )
}

/** Sets <title> + description (React 19 hoists these into <head>). */
export function Seo({ title, description, image }: { title?: string; description?: string | null; image?: string | null }) {
  const full = title ? `${title} | معهد السمو للتدريب` : 'معهد السمو للتدريب | دبلومات ودورات معتمدة'
  return (
    <>
      <title>{full}</title>
      {description && <meta name="description" content={description} />}
      <meta property="og:title" content={full} />
      {description && <meta property="og:description" content={description} />}
      {image && <meta property="og:image" content={image} />}
    </>
  )
}
