import { ArrowLeft, Award, BadgeCheck, BookOpenCheck, Gem, Handshake, HeartHandshake, Lightbulb, MapPin, Rocket, ShieldCheck, Sparkles, Target, TrendingUp, Users, type LucideIcon } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Img, Reveal, useParallax } from '../../components/ui'
import { cx } from '../../lib/format'
import type { HomeData } from '../../lib/types'
import { Headline, REDUCED, at, useOnScreen, usePalette } from './kit'

/* =====================================================================
   «من نحن» as a bento: the place (photo + the body that supervises it), the story (summary),
   the vision and mission taking turns on a coloured card, and the values as a ruled row, each with its icon in the site's icon tile.
   Every word and image comes from the dashboard (settings › عن المعهد).
   ===================================================================== */

export default function About({ data }: { data: HomeData }) {
  const a = data.settings.about
  const palette = usePalette()
  const [ref, y] = useParallax(36)
  const sup = data.accreditations.find((x) => x.logo)
  // where we teach: the dashboard branches (contact › الفروع), e.g. «حضوري: …», «قريبًا: …», «عن بُعد: …»
  const where = (data.settings.contact.branches?.length ? data.settings.contact.branches : data.settings.hero.highlights ?? []).filter(Boolean)
  const sides = [
    a.vision && { title: 'رؤيتنا', text: a.vision },
    a.mission && { title: 'رسالتنا', text: a.mission },
  ].filter(Boolean) as { title: string; text: string }[]
  if (!a.summary) return null
  return (
    <section className="relative py-24 md:py-32">
      <div className="container-x">
        <div className="flex flex-col items-start gap-6 md:flex-row md:items-end md:justify-between">
          <Headline eyebrow="من نحن" title={a.title || ''} rest={data.settings.general.tagline} />
          <Reveal><Link to="/about" className="btn-ghost group shrink-0">اعرف المزيد عنا <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" /></Link></Reveal>
        </div>

        <div className="mt-12 grid gap-4 lg:grid-cols-12">
          {/* the place */}
          <Reveal className="lg:col-span-7 lg:row-span-2">
            <div ref={ref} className="about-photo">
              <motion.div style={{ y }} className="absolute inset-[-7%_0]"><Img src={a.image} alt={a.title || ''} className="h-full w-full" /></motion.div>
              <div className="about-photo-veil" />
              {where.length > 0 && <Where items={where} />}
              {sup && (
                <div className="about-sup">
                  <span className="about-sup-logo"><img src={sup.logo!} alt="" /></span>
                  <span className="min-w-0">
                    <span className="block text-[12.5px] font-semibold text-white/70">تحت إشراف</span>
                    <b className="block font-display text-[15px] leading-snug text-white">{sup.name}</b>
                  </span>
                </div>
              )}
            </div>
          </Reveal>

          {/* the story */}
          <Reveal delay={0.06} className="lg:col-span-5">
            <div className="about-story">
              <p>{a.summary}</p>
              <Link to="/about" className="mt-5 inline-flex items-center gap-2 font-display font-bold text-brand-700 hover:gap-3 transition-all">اقرأ القصة كاملة <ArrowLeft className="h-4 w-4" /></Link>
            </div>
          </Reveal>

          {/* vision ↔ mission */}
          {sides.length > 0 && (
            <Reveal delay={0.12} className="lg:col-span-5">
              <Sides sides={sides} color={at(palette, 0)} />
            </Reveal>
          )}

          {/* values */}
          {!!a.values?.length && (
            <div className="pgrid grid grid-cols-2 lg:col-span-12" style={{ ['--cols' as string]: Math.min(a.values.length, 4) }}>
              {a.values.map((v, i) => (
                <Reveal key={v} delay={i * 0.06} className="pgrid-cell !h-auto !justify-start gap-4 !p-5 md:!p-6">
                  <ValueIcon value={v} color={at(palette, i + 1)} />
                  <span>
                    <span className="block font-display text-xs font-bold text-muted tabular-nums">0{i + 1}</span>
                    <b className="font-display text-lg font-extrabold text-ink md:text-xl">{v}</b>
                  </span>
                </Reveal>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  )
}

/** The vision and the mission take turns (every few seconds, or on a tap). */
function Sides({ sides, color }: { sides: { title: string; text: string }[]; color: string }) {
  const [i, setI] = useState(0)
  const [ref, on] = useOnScreen<HTMLDivElement>()
  const [hold, setHold] = useState(false)
  useEffect(() => {
    if (!on || hold || REDUCED || sides.length < 2) return
    const t = setTimeout(() => setI((x) => (x + 1) % sides.length), 6500)
    return () => clearTimeout(t)
  }, [i, on, hold, sides.length])
  const s = sides[i]
  return (
    <div ref={ref} className="about-sides" style={{ ['--stage' as string]: color }} onMouseEnter={() => setHold(true)} onMouseLeave={() => setHold(false)}>
      <div className="demo-stage-bg" aria-hidden />
      <div className="relative flex gap-1.5">
        {sides.map((x, k) => (
          <button key={x.title} onClick={() => setI(k)} className={cx('about-tab', k === i && 'on')}>
            {x.title}
            {k === i && !hold && !REDUCED && sides.length > 1 && <motion.i key={'p' + i} initial={{ scaleX: 0 }} animate={on ? { scaleX: 1 } : {}} transition={{ duration: 6.5, ease: 'linear' }} />}
          </button>
        ))}
      </div>
      <AnimatePresence mode="wait">
        <motion.p key={i} initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }} transition={{ duration: 0.45, ease: [0.22, 1, 0.36, 1] }} className="relative mt-6 font-display text-[clamp(1.15rem,1rem+.5vw,1.45rem)] leading-[1.75] font-bold text-white">
          {s.text}
        </motion.p>
      </AnimatePresence>
    </div>
  )
}

/** The places, one after another: «حضوري: حفر الباطن» → «قريبًا: …» → «عن بُعد: …». */
function Where({ items }: { items: string[] }) {
  const [i, setI] = useState(0)
  useEffect(() => {
    if (REDUCED || items.length < 2) return
    const t = setTimeout(() => setI((x) => (x + 1) % items.length), 3200)
    return () => clearTimeout(t)
  }, [i, items.length])
  const [k, ...rest] = items[i].split(/[:：]\s*/)
  const v = rest.join(': ')
  return (
    <span className="about-where" aria-live="polite">
      <MapPin className="h-4 w-4 shrink-0" />
      <AnimatePresence mode="wait">
        <motion.span key={i} initial={{ opacity: 0, y: 6 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -6 }} transition={{ duration: 0.3 }}>
          {v ? <><b>{k}</b> {v}</> : k}
        </motion.span>
      </AnimatePresence>
      {items.length > 1 && <span className="about-where-dots" aria-hidden>{items.map((_, d) => <i key={d} className={d === i ? 'on' : ''} />)}</span>}
    </span>
  )
}

/* a value's icon (the values are written in the dashboard, so the icon follows the word; anything else gets a gem) */
const VALUE_ICONS: [RegExp, LucideIcon][] = [
  [/جود|اتقان|إتقان|تميز|تميّز/, Award], [/تمكين|قدر|مهار/, Rocket], [/التزام|انضباط|مسؤول/, ShieldCheck],
  [/تطوير|تطور|تحسين|نمو/, TrendingUp], [/ابتكار|إبداع|ابداع/, Lightbulb], [/شراك|تعاون/, Handshake],
  [/نزاه|شفاف|أمان|امان|مصداق/, BadgeCheck], [/احترام|إنسان|انسان|رعاي/, HeartHandshake], [/فريق|عمل جماعي/, Users],
  [/تعلم|تعليم|معرف/, BookOpenCheck], [/هدف|رؤي|طموح/, Target], [/ريادة|رياد/, Sparkles],
]
function ValueIcon({ value, color }: { value: string; color: string }) {
  const Icon = VALUE_ICONS.find(([re]) => re.test(value))?.[1] ?? Gem
  return (
    <span
      className="grid h-12 w-12 shrink-0 place-items-center rounded-2xl md:h-14 md:w-14"
      style={{ background: `color-mix(in oklab, ${color} 14%, var(--surface))`, color: `color-mix(in oklab, ${color} 80%, var(--ink))` }}
      aria-hidden
    >
      <Icon className="h-6 w-6" />
    </span>
  )
}
