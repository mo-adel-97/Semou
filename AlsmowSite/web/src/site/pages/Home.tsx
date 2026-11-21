import {
  ArrowLeft, BadgeCheck, Briefcase, CalendarDays, ClipboardCheck, Eye, GraduationCap, MousePointerClick, PhoneCall,
  Quote, Rocket, ShieldCheck, Sparkles, Target, UserRound, Wallet, X,
} from 'lucide-react'
import { AnimatePresence, LayoutGroup, motion, useScroll, useTransform } from 'motion/react'
import { useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import {
  Aurora, Counter, Img, LITE, Magnetic, Reveal, SectionHeading, Seo, Skeleton, SplitText, Stars, Tilt, stagger, useParallax,
} from '../../components/ui'
import { Capsule, Cube, Sphere } from '../../components/Shapes3D'
import { ButterflyOrbit, ButterflySky } from '../../components/Butterflies'
import BrandLogo from '../../components/BrandLogo'
import { cx, fmtDate } from '../../lib/format'
import { Sections } from '../../lib/layout'
import { useHome } from '../../lib/queries'
import { PROGRAM_TYPES, type Accreditation, type HomeData, type ProgramType } from '../../lib/types'
import ProgramCard from '../ProgramCard'
import { PostCard } from './Blog'

const EASE = [0.22, 1, 0.36, 1] as const
const TYPE_ICON: Record<ProgramType, typeof GraduationCap> = { diploma: GraduationCap, qualifying: Briefcase, development: Sparkles }

export default function Home() {
  const { data, isLoading } = useHome()
  const wide = useWide()
  if (isLoading || !data) return <HomeSkeleton />
  const seo = data.settings.seo
  return (
    <>
      <Seo fullTitle={seo.title} description={seo.description} image={seo.og_image} />
      {/* desktop keeps the original composition; phones/tablets get the full-screen stage */}
      {wide ? <Hero data={data} /> : <HeroMobile data={data} />}
      {/* every other section in the order (and visibility) set in the dashboard › ترتيب الصفحات */}
      <Sections settings={data.settings} page="home" sections={{
        ticker: <Ticker data={data} />,
        accreditations: <Accreditations items={data.accreditations} />,
        paths: <Paths data={data} />,
        programs: <Programs data={data} />,
        about: <About data={data} />,
        stats: <Stats data={data} />,
        journey: <Journey />,
        testimonials: <Testimonials data={data} />,
        portal: <PortalPromo />,
        posts: <LatestPosts data={data} />,
        agreements: <Agreements data={data} />,
        partners: <Partners data={data} />,
      }} />
    </>
  )
}

function SmartLink({ to, className, children }: { to?: string; className: string; children: ReactNode }) {
  return to && /^https?:/.test(to) ? (
    <a href={to} target="_blank" rel="noopener noreferrer" className={className}>{children}</a>
  ) : (
    <Link to={to || '/programs'} className={className}>{children}</Link>
  )
}

/* ================= Hero ================= */

function Hero({ data }: { data: HomeData }) {
  const h = data.settings.hero
  const title = h.title || 'معهد السمو للتدريب'
  const total = data.counts.diploma + data.counts.qualifying + data.counts.development
  const ref = useRef<HTMLElement>(null)
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start start', 'end start'] })
  const imgY = useTransform(scrollYProgress, [0, 1], [0, 60])
  const textY = useTransform(scrollYProgress, [0, 1], [0, -60])
  const fade = useTransform(scrollYProgress, [0, 0.8], [1, 0])

  return (
    <section ref={ref} className="relative overflow-hidden pt-28 pb-14 sm:pt-32 md:pt-36 lg:flex lg:min-h-[min(100svh,56rem)] lg:items-center lg:pt-28 lg:pb-14">
      <HeroMotes />
      <Aurora />
      {!data.settings.butterflies?.hide && <ButterflySky count={LITE ? 2 : 3} />}
      <div className="pattern absolute inset-0 [mask-image:radial-gradient(ellipse_at_center,black_20%,transparent_70%)]" />

      <div className="container-x relative grid items-center gap-10 sm:gap-14 lg:grid-cols-[1.25fr_1fr] lg:gap-10 xl:gap-16">
        {/* scroll-linked drift is desktop only: on phones it is JS work on every scroll frame */}
        <motion.div style={LITE ? undefined : { y: textY, opacity: fade }}>
          {h.eyebrow && (
            <motion.span initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.6 }} className="eyebrow">
              <span className="relative flex h-2 w-2">
                <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-brand-500 opacity-60" />
                <span className="relative inline-flex h-2 w-2 rounded-full bg-brand-500" />
              </span>
              {h.eyebrow}
            </motion.span>
          )}
          <SplitText
            as="h1"
            immediate
            delay={0.1}
            text={title}
            accent={h.highlight}
            className="hero-title mt-5 text-[clamp(2.1rem,1.2rem+3.2vw,4rem)] leading-[1.2] font-black tracking-tight text-balance text-ink sm:mt-6"
          />
          {h.subtitle && (
            <motion.p
              initial={LITE ? { opacity: 0, y: 12 } : { opacity: 0, y: 20, filter: 'blur(6px)' }}
              animate={LITE ? { opacity: 1, y: 0 } : { opacity: 1, y: 0, filter: 'blur(0px)' }}
              transition={{ duration: 0.8, delay: LITE ? 0.35 : 0.5, ease: EASE }}
              className="mt-4 max-w-xl text-[clamp(1rem,0.9rem+0.4vw,1.2rem)] leading-[1.85] text-pretty text-muted sm:mt-5 sm:leading-[1.9]"
            >
              {h.subtitle}
            </motion.p>
          )}
          {/* phones: the two buttons share one row (equal width); from sm they size to their text */}
          <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: LITE ? 0.45 : 0.65, ease: EASE }} className="mt-7 grid grid-cols-2 gap-2.5 sm:mt-8 sm:flex sm:flex-wrap sm:items-center sm:gap-3">
            <Magnetic>
              <SmartLink to={h.cta_link || '/register'} className="btn-primary group w-full !gap-1.5 !px-3 !py-3.5 text-[14.5px] sm:w-auto sm:!gap-2 sm:!px-7 sm:text-[15px] xl:!px-8 xl:!py-4 xl:text-base">
                <span className="absolute inset-0 -translate-x-full bg-gradient-to-l from-transparent via-white/25 to-transparent transition-transform duration-700 group-hover:translate-x-full" />
                {h.cta_text || 'ابدأ رحلتك الآن'} <ArrowLeft className="h-4 w-4 shrink-0 transition group-hover:-translate-x-1 sm:h-5 sm:w-5" />
              </SmartLink>
            </Magnetic>
            <SmartLink to={h.secondary_link || '/programs'} className="btn-ghost w-full !px-3 !py-3.5 text-[14.5px] sm:w-auto sm:!px-7 sm:text-[15px] xl:!px-8 xl:!py-4 xl:text-base">
              {h.secondary_text || 'استعرض البرامج'}
            </SmartLink>
          </motion.div>
          {!!h.highlights?.length && (
            <motion.ul variants={stagger.container} initial="hidden" animate="show" transition={{ delayChildren: LITE ? 0.5 : 0.8 }} className="mt-7 grid grid-cols-2 gap-x-3 gap-y-2.5 sm:mt-9 sm:flex sm:flex-wrap sm:gap-x-6 sm:gap-y-3">
              {h.highlights.map((x) => (
                <motion.li key={x} variants={stagger.item} className="flex items-center gap-1.5 text-[13.5px] leading-snug font-semibold text-ink-2 sm:gap-2 sm:text-[15px]">
                  <BadgeCheck className="h-[1.15rem] w-[1.15rem] shrink-0 text-brand-500 sm:h-5 sm:w-5" />
                  {x}
                </motion.li>
              ))}
            </motion.ul>
          )}
        </motion.div>

        {/* Visual composition */}
        <motion.div
          initial={{ opacity: 0, scale: 0.92, rotate: -2 }}
          animate={{ opacity: 1, scale: 1, rotate: 0 }}
          transition={{ duration: 1.1, ease: EASE }}
          className="relative mx-auto w-[min(100%,25rem)] sm:w-[min(100%,32rem)] lg:w-[min(100%,calc((100svh-10rem)*1.15),40rem)]"
          data-depth-scene
        >
          {/* rotating badge */}
          <div className="absolute top-[4%] left-[2%] z-20 hidden h-24 w-24 sm:block xl:h-28 xl:w-28">
            <svg viewBox="0 0 100 100" className="h-full w-full animate-spin-slow text-ink-2">
              <defs><path id="circ" d="M50,50 m-38,0 a38,38 0 1,1 76,0 a38,38 0 1,1 -76,0" /></defs>
              <text fontSize="8.4" fontWeight="700" fill="currentColor" direction="ltr" style={{ fontFamily: "system-ui, sans-serif" }}>
                <textPath href="#circ" textLength="236" lengthAdjust="spacingAndGlyphs">AL SUMOU INSTITUTE • CERTIFIED TRAINING • </textPath>
              </text>
            </svg>
            {/* the institute's own logo in the centre, with butterflies circling it */}
            <span className="absolute inset-0 grid place-items-center">
              {data.settings.butterflies?.hide
                ? <BrandLogo src={data.settings.general?.logo} animate={false} className="h-14 w-14 shadow-lift xl:h-16 xl:w-16" />
                : <ButterflyOrbit><BrandLogo src={data.settings.general?.logo} animate={false} className="h-14 w-14 shadow-lift xl:h-16 xl:w-16" /></ButterflyOrbit>}
            </span>
          </div>

          {/* Soft glow + slowly turning dashed ring behind the cut-out (the photo carries its own colour circles) */}
          <div className="depth-1 absolute inset-[8%_6%_4%] rounded-full bg-brand-500/20 blur-3xl" />
          <div className="hd-rays depth-1" aria-hidden />
          <div className="hero-blob depth-1" />
          {/* real 3D shapes (CSS only): float on their own and follow the mouse / phone tilt */}
          <Sphere className="depth-3 top-[6%] right-[8%] [--sz:clamp(38px,9vw,70px)]" />
          <Cube className="depth-3 top-[52%] right-[-2%] !z-[5] [--sz:clamp(34px,7vw,52px)] s3d-hide-sm" />
          <Sphere gold className="depth-2 bottom-[30%] right-[4%] [--sz:clamp(20px,4vw,30px)]" />
          <Capsule className="depth-3 top-[34%] left-[-3%] !z-[5] [--sz:clamp(46px,9vw,74px)] s3d-hide-sm" />
          <div className="absolute bottom-0 left-1/2 h-[14%] w-[80%] -translate-x-1/2 rounded-[50%] bg-deep-950/40 blur-2xl" />
          {/* 6:5 frame, anchored to the top: anything below the person (e.g. a caption baked into an uploaded
              photo) falls outside the frame and is never shown */}
          <div className="depth-2 relative aspect-[6/5]">
            {h.image ? (
              <motion.img
                style={LITE ? undefined : { y: imgY }}
                src={h.image}
                alt={title}
                width={1920}
                height={1600}
                className="hero-photo-soft absolute inset-0 h-full w-full object-cover object-top"
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: LITE ? 0.6 : 1, delay: LITE ? 0.1 : 0.2 }}
                fetchPriority="high"
                decoding="async"
              />
            ) : (
              <div className="absolute inset-0 grid place-items-center">
                <BrandLogo className="h-40 w-40" />
              </div>
            )}
            {h.image && <div className="hd-sheen" aria-hidden />}
          </div>

          <FloatCard className="top-[62%] -right-1 sm:-right-4 xl:-right-10" delay={LITE ? 0.6 : 0.9}>
            <span className="grid h-11 w-11 place-items-center rounded-xl bg-gold-400/15 text-gold-600"><ShieldCheck className="h-6 w-6" /></span>
            <div>
              <div className="font-display text-sm font-extrabold text-ink">شهادات معتمدة</div>
              <div className="text-xs text-muted">التدريب التقني والمهني</div>
            </div>
          </FloatCard>
          {total > 0 && (
            <FloatCard className="bottom-[26%] -left-1 [animation-delay:-3s] sm:bottom-[22%] sm:-left-4 xl:-left-8" delay={LITE ? 0.75 : 1.1}>
              <span className="grid h-11 w-11 place-items-center rounded-xl bg-brand-100 text-brand-700"><GraduationCap className="h-6 w-6" /></span>
              <div>
                <div className="font-display text-2xl leading-none font-black text-brand-700"><Counter value={total} suffix="+" /></div>
                <div className="mt-1 text-xs text-muted">برنامج تدريبي</div>
              </div>
            </FloatCard>
          )}
          <FloatCard className="inset-x-0 -bottom-3 mx-auto w-max !py-2.5 whitespace-nowrap [animation-delay:-5s] sm:inset-x-auto sm:-bottom-4 sm:right-[18%] sm:mx-0" delay={LITE ? 0.9 : 1.3}>
            <span className="relative flex h-2.5 w-2.5">
              <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-emerald-500 opacity-70" />
              <span className="relative inline-flex h-2.5 w-2.5 rounded-full bg-emerald-500" />
            </span>
            <span className="text-sm font-bold text-ink">التسجيل مفتوح الآن</span>
          </FloatCard>
        </motion.div>
      </div>

      <motion.div style={{ opacity: fade }} className="absolute bottom-5 left-1/2 hidden -translate-x-1/2 flex-col items-center gap-2 text-xs text-muted [@media(min-height:820px)]:lg:flex">
        <span className="flex h-10 w-6 justify-center rounded-full border-2 border-line pt-2">
          <motion.span animate={{ y: [0, 10, 0], opacity: [1, 0.2, 1] }} transition={{ duration: 1.8, repeat: Infinity }} className="h-2 w-1 rounded-full bg-brand-500" />
        </span>
        اكتشف المزيد
      </motion.div>
    </section>
  )
}

function FloatCard({ className, delay, children }: { className: string; delay: number; children: ReactNode }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20, scale: 0.9 }}
      animate={{ opacity: 1, y: 0, scale: 1 }}
      transition={{ delay, type: 'spring', stiffness: 200, damping: 18 }}
      className={cx('depth-3 glass absolute z-10 flex animate-float items-center gap-2.5 rounded-2xl p-2.5 text-sm shadow-lift xl:gap-3 xl:p-3.5 [&_.h-11]:h-9 [&_.h-11]:w-9 xl:[&_.h-11]:h-11 xl:[&_.h-11]:w-11', className)}
    >
      {children}
    </motion.div>
  )
}

/* ================= Hero: phones & tablets =================
   A full-screen stage: the portrait on top (lit by slow light rays and an orbit of light), the words below it,
   never over her face. Every colour comes from the visitor's theme (brand + glow), in light and dark mode. */

const WIDE_QUERY = '(min-width: 1024px)'
function useWide() {
  const [wide, setWide] = useState(() => typeof window !== 'undefined' && window.matchMedia(WIDE_QUERY).matches)
  useEffect(() => {
    const m = window.matchMedia(WIDE_QUERY)
    const on = () => setWide(m.matches)
    m.addEventListener('change', on)
    return () => m.removeEventListener('change', on)
  }, [])
  return wide
}

function HeroMobile({ data }: { data: HomeData }) {
  const h = data.settings.hero
  const title = h.title || 'معهد السمو للتدريب'
  const total = data.counts.diploma + data.counts.qualifying + data.counts.development
  return (
    <section className="hm relative isolate flex min-h-[100svh] flex-col justify-end overflow-hidden">
      <div className="hm-stage absolute inset-0 -z-20">
        <div className="hm-glow" />
        {h.image ? (
          // 6:5 frame anchored to the top: anything baked in under the person (a caption…) stays out of view
          <div className="hm-figure aspect-[6/5]">
            <div className="hm-rays" aria-hidden />
            <div className="hm-orbit" aria-hidden><i /></div>
            <img src={h.image} alt={title} width={1920} height={1600} className="hero-photo-soft relative h-full w-full object-cover object-top" fetchPriority="high" decoding="async" />
          </div>
        ) : (
          <div className="absolute inset-x-0 top-[18%] grid place-items-center"><BrandLogo className="h-36 w-36" /></div>
        )}
      </div>
      <div className="hm-veil absolute inset-0 -z-10" />
      <HeroMotes />
      {!data.settings.butterflies?.hide && <ButterflySky count={2} />}

      <div className="hm-words container-x relative pb-24">
        {h.eyebrow && (
          <motion.span initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.6, delay: 0.15, ease: EASE }} className="eyebrow">
            <span className="relative flex h-2 w-2">
              <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-brand-500 opacity-60" />
              <span className="relative inline-flex h-2 w-2 rounded-full bg-brand-500" />
            </span>
            {h.eyebrow}
          </motion.span>
        )}
        <SplitText as="h1" immediate delay={0.2} text={title} accent={h.highlight} className="hero-title mt-5 text-[clamp(2.4rem,1.4rem+5vw,3.6rem)] leading-[1.15] font-black tracking-tight text-balance text-ink" />
        <motion.span initial={{ scaleX: 0 }} animate={{ scaleX: 1 }} transition={{ duration: 1, delay: 0.6, ease: EASE }} className="hm-rule" aria-hidden />
        {h.subtitle && (
          <motion.p initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: 0.45, ease: EASE }} className="mt-5 line-clamp-4 text-[1.02rem] leading-[1.85] text-pretty text-ink-2">
            {h.subtitle}
          </motion.p>
        )}
        <motion.div initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.6, delay: 0.55, ease: EASE }} className="mt-7 grid grid-cols-2 gap-2.5">
          <SmartLink to={h.cta_link || '/register'} className="btn-primary group w-full !gap-1.5 !px-3 !py-3.5 text-[14.5px]">
            {h.cta_text || 'ابدأ رحلتك الآن'} <ArrowLeft className="h-4 w-4 shrink-0 transition group-hover:-translate-x-1" />
          </SmartLink>
          <SmartLink to={h.secondary_link || '/programs'} className="btn-ghost w-full !px-3 !py-3.5 text-[14.5px]">
            {h.secondary_text || 'استعرض البرامج'}
          </SmartLink>
        </motion.div>
        <motion.dl initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.7, delay: 0.7, ease: EASE }} className="hm-facts mt-9 grid grid-cols-3">
          {total > 0 && (
            <div>
              <dt className="font-display text-2xl leading-none font-black text-brand-600"><Counter value={total} suffix="+" /></dt>
              <dd>برنامج تدريبي</dd>
            </div>
          )}
          <div>
            <dt className="flex items-center gap-1.5 font-display text-[15px] leading-none font-extrabold text-ink"><ShieldCheck className="h-4 w-4 text-brand-500" /> معتمد</dt>
            <dd>التدريب التقني والمهني</dd>
          </div>
          <div>
            <dt className="flex items-center gap-2 font-display text-[15px] leading-none font-extrabold text-ink">
              <span className="relative flex h-2.5 w-2.5 shrink-0">
                <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-emerald-500 opacity-70" />
                <span className="relative inline-flex h-2.5 w-2.5 rounded-full bg-emerald-500" />
              </span>
              التسجيل
            </dt>
            <dd>مفتوح الآن</dd>
          </div>
        </motion.dl>
      </div>
    </section>
  )
}

/* ================= Ticker ================= */

function Ticker({ data }: { data: HomeData }) {
  const words = [
    ...(Object.keys(PROGRAM_TYPES) as ProgramType[]).filter((t) => data.counts[t]).map((t) => PROGRAM_TYPES[t].plural),
    ...(data.settings.hero.highlights ?? []),
    ...data.programs.slice(0, 6).map((p) => p.title),
  ]
  if (words.length < 3) return null
  const row = [...words, ...words]
  return (
    <div className="relative -mx-4 -rotate-1 overflow-hidden border-y border-deep-700 bg-deep-900 py-4 text-white">
      {/* a calm pace: easy to read while it glides */}
      <div className="flex w-max animate-marquee gap-10 whitespace-nowrap [animation-duration:95s]">
        {[...row, ...row].map((w, i) => (
          <span key={i} className="flex items-center gap-10 font-display text-xl font-bold md:text-2xl">
            {w}
            <Sparkles className="h-5 w-5 text-gold-400" />
          </span>
        ))}
      </div>
    </div>
  )
}

/* ================= Accreditations ================= */

function Accreditations({ items }: { items: Accreditation[] }) {
  const [open, setOpen] = useState<Accreditation | null>(null)
  if (!items.length) return null
  return (
    <section className="py-16 md:py-24">
      <div className="container-x">
        <Reveal>
          {/* a deep band in the theme colour; every logo is painted in one light tone through its own shape,
              so the row always matches the site's colour, in light and dark mode */}
          <div className="grain relative overflow-hidden rounded-[2.5rem] bg-deep-900 px-5 py-12 text-white sm:px-10 md:py-14">
            <div className="pattern-light absolute inset-0" />
            <div className="absolute -top-24 left-1/2 h-64 w-[70%] -translate-x-1/2 rounded-full bg-brand-500/30 blur-3xl" />
            <div className="relative mb-10 text-center">
              <span className="eyebrow !border-white/15 !bg-white/10 !text-white"><BadgeCheck className="h-4 w-4" /> اعتماداتنا</span>
              <h2 className="mt-4 text-[clamp(1.6rem,1.1rem+1.8vw,2.4rem)] leading-tight font-black">معتمدون من جهات رسمية موثوقة</h2>
            </div>
            <motion.div variants={stagger.container} initial="hidden" whileInView="show" viewport={{ once: true }} className="relative flex flex-wrap justify-center gap-3 sm:gap-4">
              {items.map((a) => (
                <motion.button
                  key={a.id}
                  variants={stagger.item}
                  onClick={() => a.certificate && setOpen(a)}
                  className={cx('acc-item group', !a.certificate && 'cursor-default')}
                  title={a.name}
                >
                  {/* the logo already carries the body's name: shown once, the name stays for screen readers / tooltip */}
                  <span className="acc-logo" style={{ WebkitMaskImage: `url(${a.logo})`, maskImage: `url(${a.logo})` }} role="img" aria-label={a.name} />
                  {a.certificate && <span className="acc-cert">عرض الشهادة</span>}
                </motion.button>
              ))}
            </motion.div>
          </div>
        </Reveal>
      </div>
      <AnimatePresence>
        {open && (
          <motion.div className="fixed inset-0 z-[80] flex items-center justify-center bg-deep-950/85 p-4 backdrop-blur" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setOpen(null)} role="dialog" aria-label={open.name}>
            <motion.div initial={{ scale: 0.85, rotate: -2 }} animate={{ scale: 1, rotate: 0 }} exit={{ scale: 0.9 }} className="relative max-h-[90vh] max-w-3xl" onClick={(e) => e.stopPropagation()}>
              <img src={open.certificate!} alt={open.name} className="max-h-[85vh] w-auto rounded-2xl bg-white shadow-2xl" />
              <button onClick={() => setOpen(null)} className="absolute -top-4 -left-4 grid h-10 w-10 place-items-center rounded-full bg-surface shadow-lift" aria-label="إغلاق"><X className="h-5 w-5" /></button>
              <p className="mt-3 text-center font-semibold text-white">{open.name}</p>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </section>
  )
}

/* ================= Paths (bento) ================= */

function Paths({ data }: { data: HomeData }) {
  const types = Object.keys(PROGRAM_TYPES) as ProgramType[]
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <SectionHeading eyebrow="اختر مسارك" title="ثلاثة مسارات لمستقبل مهني أقوى" accent="مهني أقوى" text="سواء كنت تبحث عن دبلوم معتمد أو دورة لتطوير مهارة محددة، لدينا المسار المناسب لك." />
        <motion.div variants={stagger.container} initial="hidden" whileInView="show" viewport={{ once: true, margin: '-80px' }} className="grid gap-5 md:grid-cols-3 md:grid-rows-2">
          {types.map((t, i) => {
            const Icon = TYPE_ICON[t]
            const big = i === 0
            return (
              <motion.div key={t} variants={stagger.item} className={cx(big && 'md:row-span-2')}>
                <Tilt className="group h-full rounded-[2rem]">
                  <Link
                    to={`/programs/type/${t}`}
                    className={cx(
                      'relative flex h-full flex-col overflow-hidden rounded-[2rem] p-8 transition-shadow duration-500 hover:shadow-lift',
                      big ? 'min-h-[22rem] bg-deep-900 text-white' : 'card min-h-[13rem]',
                    )}
                  >
                    {big && <><div className="pattern-light absolute inset-0" /><div className="absolute -bottom-20 -left-20 h-72 w-72 rounded-full bg-brand-500/40 blur-3xl transition duration-700 group-hover:scale-125" /></>}
                    <div className="relative flex items-start justify-between">
                      <span className={cx('grid h-14 w-14 place-items-center rounded-2xl transition duration-500 group-hover:rotate-[-8deg] group-hover:scale-110', big ? 'bg-white/10 text-gold-300' : 'bg-brand-50 text-brand-700')}>
                        <Icon className="h-7 w-7" />
                      </span>
                      <span className={cx('font-display text-5xl font-black', big ? 'text-white/15' : 'text-brand-100')}>0{i + 1}</span>
                    </div>
                    <div className="relative mt-auto pt-8">
                      <div className={cx('font-display text-sm font-bold', big ? 'text-gold-300' : 'text-brand-600')}>{data.counts[t]} برامج</div>
                      <h3 className={cx('mt-1 leading-snug font-black', big ? 'text-3xl md:text-4xl' : 'text-2xl')}>{PROGRAM_TYPES[t].plural}</h3>
                      <p className={cx('mt-3 leading-relaxed', big ? 'max-w-sm text-white/70' : 'text-muted')}>{PROGRAM_TYPES[t].blurb}</p>
                      <span className={cx('mt-6 inline-flex items-center gap-2 font-bold', big ? 'text-gold-300' : 'text-brand-700')}>
                        استكشف <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1.5" />
                      </span>
                    </div>
                  </Link>
                </Tilt>
              </motion.div>
            )
          })}
          <motion.div variants={stagger.item} className="md:col-span-2">
            <Link to="/register" className="group relative flex h-full min-h-[9rem] items-center justify-between gap-6 overflow-hidden rounded-[2rem] bg-gradient-to-l from-gold-400 to-gold-300 p-8 text-deep-950">
              <div className="absolute inset-0 -translate-x-full bg-gradient-to-l from-transparent via-white/40 to-transparent transition duration-1000 group-hover:translate-x-full" />
              <div className="relative">
                <h3 className="text-2xl font-black md:text-3xl">لم تحدد برنامجك بعد؟</h3>
                <p className="mt-1 font-medium text-deep-900/75">سجّل اهتمامك وسيساعدك فريق القبول في اختيار البرنامج الأنسب لك.</p>
              </div>
              <span className="relative grid h-14 w-14 shrink-0 place-items-center rounded-full bg-deep-950 text-white transition group-hover:scale-110"><ArrowLeft className="h-6 w-6" /></span>
            </Link>
          </motion.div>
        </motion.div>
      </div>
    </section>
  )
}

/* ================= Programs (tabbed) ================= */

function Programs({ data }: { data: HomeData }) {
  const types = (Object.keys(PROGRAM_TYPES) as ProgramType[]).filter((t) => data.programs.some((p) => p.type === t))
  const [tab, setTab] = useState<ProgramType | null>(null)
  const active = tab ?? types[0]
  const list = useMemo(() => data.programs.filter((p) => p.type === active).slice(0, 6), [data.programs, active])
  if (!types.length) return null
  return (
    <section className="relative bg-sand/70 py-20 md:py-28" id="programs">
      <div className="container-x">
        <SectionHeading
          align="start"
          eyebrow="برامجنا التدريبية"
          title="برامج معتمدة تفتح لك أبواب المستقبل"
          accent="أبواب المستقبل"
          action={
            <LayoutGroup>
              <div className="inline-flex flex-wrap gap-1 rounded-full border border-line bg-surface p-1.5 shadow-soft">
                {types.map((t) => (
                  <button key={t} onClick={() => setTab(t)} className={cx('relative rounded-full px-5 py-2.5 font-display text-[15px] font-bold transition-colors', active === t ? 'text-white' : 'text-ink-2 hover:text-ink')}>
                    {active === t && <motion.span layoutId="prog-tab" className="absolute inset-0 rounded-full bg-primary" transition={{ type: 'spring', stiffness: 400, damping: 32 }} />}
                    <span className="relative">{PROGRAM_TYPES[t].plural}</span>
                  </button>
                ))}
              </div>
            </LayoutGroup>
          }
        />
        <AnimatePresence mode="wait">
          <motion.div key={active} initial="hidden" animate="show" exit={{ opacity: 0, y: -20, transition: { duration: 0.2 } }} variants={stagger.container} className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {list.map((p) => <motion.div key={p.id} variants={stagger.item}><ProgramCard p={p} /></motion.div>)}
          </motion.div>
        </AnimatePresence>
        <Reveal className="mt-12 flex justify-center">
          <Link to={`/programs/type/${active}`} className="btn-ghost group">
            عرض كل {PROGRAM_TYPES[active].plural} <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" />
          </Link>
        </Reveal>
      </div>
    </section>
  )
}

/* ================= About ================= */

function About({ data }: { data: HomeData }) {
  const a = data.settings.about
  const [ref, y] = useParallax(50)
  if (!a.summary) return null
  return (
    <section className="relative overflow-hidden py-20 md:py-32">
      <div className="container-x grid items-center gap-16 lg:grid-cols-2">
        <div ref={ref} className="relative order-last mx-auto w-full max-w-lg lg:order-first">
          <motion.div style={{ y }} className="absolute -top-8 -right-8 h-40 w-40 rounded-[2rem] bg-gold-400/20" />
          <Reveal>
            <div className="relative overflow-hidden rounded-[2.5rem] shadow-lift">
              <Img src={a.image} alt={a.title || 'عن المعهد'} className="aspect-[4/5] w-full transition duration-[1.5s] hover:scale-105 sm:aspect-[5/5.5]" />
            </div>
          </Reveal>
          <motion.div style={{ y }} className="glass absolute -bottom-8 -left-4 max-w-[16rem] rounded-3xl p-5 shadow-lift sm:-left-10">
            <ShieldCheck className="h-8 w-8 text-brand-600" />
            <div className="mt-3 font-display text-sm text-muted">تحت إشراف</div>
            <div className="font-display leading-snug font-bold text-ink">المؤسسة العامة للتدريب التقني والمهني</div>
          </motion.div>
        </div>
        <div>
          <SectionHeading align="start" eyebrow="من نحن" title={a.title || 'عن معهد السمو'} accent="السمو" />
          <Reveal><p className="-mt-6 text-lg leading-[2] text-ink-2">{a.summary}</p></Reveal>
          <motion.div variants={stagger.container} initial="hidden" whileInView="show" viewport={{ once: true }} className="mt-10 grid gap-4 sm:grid-cols-2">
            {[
              a.vision && { icon: Eye, title: 'رؤيتنا', text: a.vision, tone: 'bg-brand-100 text-brand-700' },
              a.mission && { icon: Target, title: 'رسالتنا', text: a.mission, tone: 'bg-gold-400/15 text-gold-600' },
            ].filter(Boolean).map((x) => {
              const it = x as { icon: typeof Eye; title: string; text: string; tone: string }
              return (
                <motion.div key={it.title} variants={stagger.item} className="card group p-6 transition duration-500 hover:-translate-y-1 hover:shadow-lift">
                  <span className={cx('grid h-12 w-12 place-items-center rounded-2xl transition duration-500 group-hover:rotate-12', it.tone)}><it.icon className="h-5 w-5" /></span>
                  <h3 className="mt-4 text-lg font-bold">{it.title}</h3>
                  <p className="mt-2 leading-relaxed text-muted">{it.text}</p>
                </motion.div>
              )
            })}
          </motion.div>
          <Reveal delay={0.2} className="mt-10">
            <Link to="/about" className="btn-primary group">اعرف المزيد عنا <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" /></Link>
          </Reveal>
        </div>
      </div>
    </section>
  )
}

/* ================= Stats ================= */

function Stats({ data }: { data: HomeData }) {
  const items = [
    ...(data.counts.diploma ? [{ value: data.counts.diploma, label: 'دبلوم معتمد', suffix: '' }] : []),
    ...(data.counts.qualifying + data.counts.development ? [{ value: data.counts.qualifying + data.counts.development, label: 'دورة تدريبية', suffix: '' }] : []),
    ...(data.settings.stats.items ?? []).filter((s) => s.value > 0),
  ].slice(0, 4)
  if (items.length < 2) return null
  return (
    <section className="py-10 md:py-16">
      <div className="container-x">
        <Reveal>
          <div className="grain relative overflow-hidden rounded-[2.5rem] bg-deep-900 px-6 py-14 md:px-12 md:py-20">
            <div className="pattern-light absolute inset-0" />
            <div className="absolute -top-24 -right-24 h-72 w-72 rounded-full bg-gold-400/25 blur-3xl" />
            <div className="absolute -bottom-24 -left-24 h-72 w-72 rounded-full bg-brand-500/40 blur-3xl" />
            <div className={cx('relative grid gap-y-12 text-center', items.length === 4 ? 'grid-cols-2 lg:grid-cols-4' : items.length === 3 ? 'grid-cols-1 sm:grid-cols-3' : 'grid-cols-2')}>
              {items.map((s, i) => (
                <Reveal key={s.label} delay={i * 0.1} className={cx(i > 0 && 'lg:border-s lg:border-white/10')}>
                  <div className="font-display text-6xl font-black text-white md:text-7xl"><Counter value={s.value} suffix={s.suffix} /></div>
                  <div className="mt-3 font-display text-lg font-semibold text-gold-300">{s.label}</div>
                </Reveal>
              ))}
            </div>
          </div>
        </Reveal>
      </div>
    </section>
  )
}

/* ================= Journey (timeline) ================= */

const STEPS = [
  { icon: MousePointerClick, title: 'اختر برنامجك', text: 'تصفّح الدبلومات والدورات المعتمدة واختر ما يناسب طموحك.' },
  { icon: ClipboardCheck, title: 'سجّل اهتمامك', text: 'املأ نموذج التسجيل في أقل من دقيقة من أي جهاز.' },
  { icon: PhoneCall, title: 'تواصل فريق القبول', text: 'نتواصل معك لتأكيد التسجيل والإجابة عن كل استفساراتك.' },
  { icon: Rocket, title: 'ابدأ رحلتك', text: 'انطلق في التدريب وتابع برنامجك ومدفوعاتك من بوابة المتدربين.' },
]

function Journey() {
  const ref = useRef<HTMLDivElement>(null)
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start 75%', 'end 60%'] })
  const line = useTransform(scrollYProgress, [0, 1], [0, 1])
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <SectionHeading eyebrow="رحلتك معنا" title="أربع خطوات تفصلك عن البداية" accent="البداية" />
        <div ref={ref} className="relative grid gap-10 md:grid-cols-4 md:gap-6">
          <div className="absolute top-8 right-8 left-8 hidden h-0.5 bg-line md:block" />
          <motion.div style={{ scaleX: line }} className="absolute top-8 right-8 left-8 hidden h-0.5 origin-right bg-gradient-to-l from-brand-500 to-gold-400 md:block" />
          {STEPS.map((s, i) => (
            <Reveal key={s.title} delay={i * 0.12} className="relative text-center">
              <div className="relative mx-auto grid h-16 w-16 place-items-center rounded-2xl border border-line bg-surface text-brand-700 shadow-soft">
                <s.icon className="h-7 w-7" />
                <span className="absolute -top-2 -left-2 grid h-7 w-7 place-items-center rounded-full bg-primary font-display text-xs font-bold text-white">{i + 1}</span>
              </div>
              <h3 className="mt-6 text-xl font-bold">{s.title}</h3>
              <p className="mx-auto mt-2 max-w-[16rem] leading-relaxed text-muted">{s.text}</p>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

/* ================= Testimonials ================= */

function Testimonials({ data }: { data: HomeData }) {
  const items = data.testimonials
  if (!items.length) return null
  return (
    <section className="bg-sand/70 py-20 md:py-28">
      <div className="container-x">
        <SectionHeading eyebrow="قصص نجاح" title="ماذا يقول متدربونا" accent="متدربونا" />
        <div className="columns-1 gap-6 md:columns-2 lg:columns-3">
          {items.map((t, i) => (
            <Reveal key={t.id} delay={(i % 3) * 0.08} className="mb-6 break-inside-avoid">
              <figure className="card relative p-7 transition duration-500 hover:-translate-y-1 hover:shadow-lift">
                <Quote className="absolute top-6 left-6 h-10 w-10 text-brand-100" />
                <Stars n={t.rating} />
                <blockquote className="mt-4 leading-[1.9] text-ink-2">{t.text}</blockquote>
                <figcaption className="mt-6 flex items-center gap-3 border-t border-line pt-5">
                  {t.avatar ? <img src={t.avatar} alt="" className="h-11 w-11 rounded-full object-cover" loading="lazy" /> : <span className="grid h-11 w-11 place-items-center rounded-full bg-brand-100 font-display font-bold text-brand-700">{t.name.trim()[0]}</span>}
                  <div>
                    <div className="font-display font-bold text-ink">{t.name}</div>
                    {t.role && <div className="text-sm text-muted">{t.role}</div>}
                  </div>
                </figcaption>
              </figure>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

/* ================= Portal promo ================= */

function PortalPromo() {
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <Reveal>
          <div className="grain relative grid items-center gap-10 overflow-hidden rounded-[2.5rem] bg-deep-950 p-8 text-white md:grid-cols-2 md:p-14">
            <div className="pattern-light absolute inset-0" />
            <div className="absolute -top-32 left-1/3 h-80 w-80 rounded-full bg-brand-500/40 blur-[100px]" />
            <div className="relative">
              <span className="eyebrow !border-white/15 !bg-white/10 !text-gold-300"><UserRound className="h-4 w-4" /> بوابة المتدربين</span>
              <SplitText text="كل تفاصيل رحلتك التدريبية في مكان واحد" accent="مكان واحد" className="mt-6 text-3xl leading-tight font-black md:text-5xl" />
              <p className="mt-5 max-w-md text-lg leading-relaxed text-white/70">ادخل برقم الهوية ورقم الجوال المسجّل لدينا، وتابع برنامجك ومستواك ودفعتك وحسابك ومدفوعاتك لحظة بلحظة.</p>
              <Magnetic>
                <Link to="/portal" className="btn-gold group mt-8 !px-8 !py-4">دخول البوابة <ArrowLeft className="h-5 w-5 transition group-hover:-translate-x-1" /></Link>
              </Magnetic>
            </div>
            <div className="relative grid gap-4 sm:grid-cols-2">
              {[
                { icon: GraduationCap, t: 'برامجي ومستواي', d: 'الدبلوم أو الدورة، المستوى والدفعة والفرع' },
                { icon: Wallet, t: 'حسابي ومدفوعاتي', d: 'الرسوم والمدفوع والمتبقي وسجل السدادات' },
                { icon: CalendarDays, t: 'مواعيد وتنبيهات', d: 'كل ما يخص رحلتك التدريبية' },
                { icon: ShieldCheck, t: 'آمنة ومحمية', d: 'بياناتك لا تظهر إلا لك' },
              ].map((c, i) => (
                <motion.div
                  key={c.t}
                  initial={{ opacity: 0, y: 30, rotate: i % 2 ? 2 : -2 }}
                  whileInView={{ opacity: 1, y: 0, rotate: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: 0.2 + i * 0.1, duration: 0.7, ease: EASE }}
                  className={cx('rounded-3xl border border-white/10 bg-white/[.06] p-5 backdrop-blur', i % 2 === 1 && 'sm:translate-y-8')}
                >
                  <c.icon className="h-7 w-7 text-gold-300" />
                  <div className="mt-4 font-display text-lg font-bold">{c.t}</div>
                  <div className="mt-1 text-sm leading-relaxed text-white/60">{c.d}</div>
                </motion.div>
              ))}
            </div>
          </div>
        </Reveal>
      </div>
    </section>
  )
}

/* ================= Blog / agreements / partners ================= */

function LatestPosts({ data }: { data: HomeData }) {
  if (!data.posts.length) return null
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <SectionHeading align="start" eyebrow="المدونة" title="أحدث المقالات والأخبار" accent="والأخبار" action={<Link to="/blog" className="btn-ghost group shrink-0">عرض الكل <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" /></Link>} />
        <div className="grid gap-6 md:grid-cols-3">{data.posts.map((p, i) => <PostCard key={p.id} p={p} index={i} />)}</div>
      </div>
    </section>
  )
}

function Agreements({ data }: { data: HomeData }) {
  if (!data.agreements.length) return null
  return (
    <section className="bg-sand/70 py-20 md:py-28">
      <div className="container-x">
        <SectionHeading eyebrow="شراكات استراتيجية" title="الاتفاقيات ومذكرات التفاهم" accent="ومذكرات التفاهم" />
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {data.agreements.map((a, i) => (
            <Reveal key={a.id} delay={i * 0.06}>
              <article className="card group h-full overflow-hidden transition duration-500 hover:-translate-y-1 hover:shadow-lift">
                <div className="overflow-hidden"><Img src={a.image} alt={a.title} className="aspect-[16/9] w-full transition duration-700 group-hover:scale-105" /></div>
                <div className="p-6">
                  {a.signed_at && <div className="flex items-center gap-2 text-sm text-muted"><CalendarDays className="h-4 w-4 text-brand-500" />{fmtDate(a.signed_at)}</div>}
                  <h3 className="mt-2 text-lg leading-snug font-bold">{a.title}</h3>
                  {a.summary && <p className="mt-2 line-clamp-3 leading-relaxed text-muted">{a.summary}</p>}
                </div>
              </article>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

function Partners({ data }: { data: HomeData }) {
  const items = data.partners.filter((p) => p.logo)
  if (!items.length) return null
  const loop = items.length < 6 ? [...items, ...items, ...items] : items
  return (
    <section className="py-16">
      <div className="container-x"><h2 className="mb-10 text-center font-display text-2xl font-extrabold">شركاء النجاح</h2></div>
      <div className="relative overflow-hidden [mask-image:linear-gradient(to_left,transparent,black_10%,black_90%,transparent)]">
        <div className="flex w-max animate-marquee gap-16 hover:[animation-play-state:paused]">
          {[...loop, ...loop].map((p, i) => {
            const img = <img src={p.logo!} alt={p.name} title={p.name} className="h-14 w-auto max-w-[10rem] object-contain opacity-60 grayscale transition hover:opacity-100 hover:grayscale-0" loading="lazy" />
            return p.url ? <a key={i} href={p.url} target="_blank" rel="noopener noreferrer" className="shrink-0">{img}</a> : <div key={i} className="shrink-0">{img}</div>
          })}
        </div>
      </div>
    </section>
  )
}

function HomeSkeleton() {
  return (
    <div className="container-x grid gap-10 pt-44 pb-20 lg:grid-cols-2">
      <div className="space-y-5">
        <Skeleton className="h-8 w-56 rounded-full" />
        <Skeleton className="h-20 w-full" />
        <Skeleton className="h-20 w-4/5" />
        <Skeleton className="h-24 w-full" />
        <div className="flex gap-3"><Skeleton className="h-14 w-44 rounded-full" /><Skeleton className="h-14 w-40 rounded-full" /></div>
      </div>
      <Skeleton className="aspect-[4/5] rounded-t-full rounded-b-[3rem]" />
    </div>
  )
}

/** Motes of light drifting up behind the hero (CSS only; fixed positions so it looks the same on every load). */
const MOTES = Array.from({ length: 16 }, (_, i) => {
  const r = (n: number) => (Math.sin(i * 78.233 + n * 12.9898) + 1) / 2
  return { left: `${4 + r(1) * 92}%`, s: `${6 + r(2) * 10}px`, d: `${9 + r(3) * 9}s`, delay: `${-r(4) * 14}s`, x: `${(r(5) - 0.5) * 120}px` }
})
function HeroMotes() {
  return (
    <div className="hero-motes" aria-hidden>
      {MOTES.map((m, i) => (
        <i key={i} style={{ left: m.left, ['--s' as string]: m.s, ['--d' as string]: m.d, ['--delay' as string]: m.delay, ['--x' as string]: m.x }} />
      ))}
    </div>
  )
}
