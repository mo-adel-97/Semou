import { ArrowLeft, BadgeCheck, Bell, Briefcase, CalendarDays, GraduationCap, Quote, ShieldCheck, Sparkles, Wallet, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Counter, Img, Magnetic, Reveal, SectionHeading, Seo, Skeleton, Stars, stagger } from '../../components/ui'
import { cx, fmtDate } from '../../lib/format'
import { Sections } from '../../lib/layout'
import { useHome } from '../../lib/queries'
import { PROGRAM_TYPES, type Accreditation, type HomeData, type ProgramType } from '../../lib/types'
import ProgramsShowcase from '../home/Programs'
import Hero from '../home/Hero'
import Story, { Screen } from '../home/Story'
import About from '../home/About'
import { DarkStage, Headline, IsoCube, at, usePalette } from '../home/kit'
import PathScene from '../home/paths'
import { PostCard } from './Blog'

const TYPE_ICON: Record<ProgramType, typeof GraduationCap> = { diploma: GraduationCap, qualifying: Briefcase, development: Sparkles }

export default function Home() {
  const { data, isLoading } = useHome()
  if (isLoading || !data) return <HomeSkeleton />
  const seo = data.settings.seo
  return (
    <div className="home2 relative">
      <Seo fullTitle={seo.title} description={seo.description} image={seo.og_image} />
      <div className="home-rails" aria-hidden />
      <Hero data={data} />
      {/* every other section in the order (and visibility) set in the dashboard › ترتيب الصفحات */}
      <Sections settings={data.settings} page="home" sections={{
        ticker: <Ticker data={data} />,
        accreditations: <Accreditations items={data.accreditations} />,
        paths: <Paths data={data} />,
        programs: <ProgramsShowcase data={data} />,
        about: <About data={data} />,
        stats: <Stats data={data} />,
        journey: <Story data={data} />,
        testimonials: <Testimonials data={data} />,
        portal: <PortalPromo data={data} />,
        posts: <LatestPosts data={data} />,
        agreements: <Agreements data={data} />,
        partners: <Partners data={data} />,
      }} />
    </div>
  )
}

/* ================= Ticker: a calm band, each word marked with the next identity colour ================= */

function Ticker({ data }: { data: HomeData }) {
  const palette = usePalette()
  const words = [
    ...(Object.keys(PROGRAM_TYPES) as ProgramType[]).filter((t) => data.counts[t]).map((t) => PROGRAM_TYPES[t].plural),
    ...(data.settings.hero.highlights ?? []),
    ...data.programs.slice(0, 6).map((p) => p.title),
  ]
  if (words.length < 3) return null
  const row = [...words, ...words]
  return (
    <div className="ticker2 relative overflow-hidden border-y border-line py-5">
      <div className="flex w-max animate-marquee gap-12 whitespace-nowrap [animation-duration:110s]">
        {[...row, ...row].map((w, i) => (
          <span key={i} className="flex items-center gap-12 font-display text-lg font-bold text-ink-2 md:text-xl">
            {w}
            <i className="h-2.5 w-2.5 rotate-45 rounded-[3px]" style={{ background: at(palette, i) }} />
          </span>
        ))}
      </div>
    </div>
  )
}

/* ================= Accreditations: a ruled grid, every body in its own colours ================= */

function Accreditations({ items }: { items: Accreditation[] }) {
  const [open, setOpen] = useState<Accreditation | null>(null)
  if (!items.length) return null
  const cols = Math.min(items.length, 4)
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <Headline align="center" eyebrow={<><BadgeCheck className="h-4 w-4" /> اعتماداتنا</>} title="معتمدون من جهات رسمية موثوقة." rest="وشهادات معترف بها." />
        <motion.div
          variants={stagger.container} initial="hidden" whileInView="show" viewport={{ once: true, margin: '-60px' }}
          className="pgrid mt-14 grid grid-cols-2"
          style={{ ['--cols' as string]: cols }}
        >
          {items.map((a) => (
            <motion.button key={a.id} variants={stagger.item} onClick={() => a.certificate && setOpen(a)} className={cx('pgrid-cell group', !a.certificate && 'cursor-default')} title={a.name}>
              <img src={a.logo ?? undefined} alt={a.name} loading="lazy" decoding="async" />
              {a.certificate && <span className="pgrid-cert">عرض الشهادة <ArrowLeft className="h-3.5 w-3.5" /></span>}
            </motion.button>
          ))}
        </motion.div>
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

/* ================= Paths: each told by what it gives (a certificate, a guided path, grown skills) ================= */

/** Arabic number agreement: برنامج واحد · برنامجان · ٣–١٠ برامج · ١١+ برنامجاً */
const countLabel = (k: number) => (k === 1 ? 'برنامج واحد' : k === 2 ? 'برنامجان' : k >= 3 && k <= 10 ? `${k} برامج` : `${k} برنامجاً`)

function Paths({ data }: { data: HomeData }) {
  const palette = usePalette()
  const types = Object.keys(PROGRAM_TYPES) as ProgramType[]
  return (
    <section className="py-20 md:py-28">
      <div className="container-x">
        <Headline eyebrow="اختر مسارك" title="ثلاثة مسارات لمستقبل مهني أقوى." rest="اختر ما يناسب طموحك." />
        <motion.div variants={stagger.container} initial="hidden" whileInView="show" viewport={{ once: true, margin: '-80px' }} className="pgrid mt-14 grid md:grid-cols-3" style={{ ['--cols' as string]: 3 }}>
          {types.map((t, i) => {
            return (
              <motion.div key={t} variants={stagger.item} className="pgrid-cell !block !h-auto !p-0">
                <Link to={`/programs/type/${t}`} className="path-card group">
                  <PathScene type={t} color={at(palette, i)} />
                  <div className="p-6 pt-5 md:p-7 md:pt-6">
                    <div className="flex items-center justify-between gap-3">
                      <h3 className="font-display text-xl font-extrabold text-ink md:text-[1.35rem]">{PROGRAM_TYPES[t].plural}</h3>
                      <span className="path-arrow"><ArrowLeft className="h-4 w-4" /></span>
                    </div>
                    <p className="mt-2 leading-relaxed text-muted">{PROGRAM_TYPES[t].blurb}</p>
                    <span className="mt-4 inline-flex items-center gap-2 font-display text-sm font-bold" style={{ color: `color-mix(in oklab, ${at(palette, i)} 80%, var(--ink))` }}>
                      <i className="h-2 w-2 rounded-full" style={{ background: at(palette, i) }} /> {countLabel(data.counts[t])}
                    </span>
                  </div>
                </Link>
              </motion.div>
            )
          })}
        </motion.div>
        <Reveal className="mt-6">
          <Link to="/register" className="path-cta group">
            <div className="relative">
              <h3 className="text-xl font-extrabold md:text-2xl">لم تحدد برنامجك بعد؟</h3>
              <p className="mt-1 text-white/75">سجّل اهتمامك وسيساعدك فريق القبول في اختيار البرنامج الأنسب لك.</p>
            </div>
            <span className="relative grid h-12 w-12 shrink-0 place-items-center rounded-full bg-white text-deep-950 transition group-hover:scale-110 group-hover:-rotate-12"><ArrowLeft className="h-5 w-5" /></span>
          </Link>
        </Reveal>
      </div>
    </section>
  )
}

/* ================= Stats: big numbers on a ruled strip ================= */

function Stats({ data }: { data: HomeData }) {
  const palette = usePalette()
  const items = [
    ...(data.counts.diploma ? [{ value: data.counts.diploma, label: 'دبلوم معتمد', suffix: '' }] : []),
    ...(data.counts.qualifying + data.counts.development ? [{ value: data.counts.qualifying + data.counts.development, label: 'دورة تدريبية', suffix: '' }] : []),
    ...(data.settings.stats.items ?? []).filter((s) => s.value > 0),
  ].slice(0, 4)
  if (items.length < 2) return null
  return (
    <section className="py-10 md:py-16">
      <div className="container-x">
        <div className="pgrid grid grid-cols-2" style={{ ['--cols' as string]: items.length }}>
          {items.map((s, i) => (
            <Reveal key={s.label} delay={i * 0.08} className="pgrid-cell !block !h-auto !p-7 text-start md:!p-10">
              <span className="block h-1 w-10 rounded-full" style={{ background: at(palette, i) }} />
              <div className="mt-6 font-display text-[clamp(3rem,2rem+4vw,5.5rem)] leading-none font-black tracking-tight text-ink"><Counter value={s.value} suffix={s.suffix} /></div>
              <div className="mt-3 font-display text-base font-bold text-muted md:text-lg">{s.label}</div>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  )
}

/* ================= Portal: a dark stage that opens to the full width, with the real portal inside ================= */

function PortalPromo({ data }: { data: HomeData }) {
  const palette = usePalette()
  const feats = [
    { icon: GraduationCap, t: 'برامجي ومستواي', d: 'الدبلوم أو الدورة، المستوى والدفعة والفرع' },
    { icon: Wallet, t: 'حسابي ومدفوعاتي', d: 'الرسوم والمدفوع والمتبقي وسجل السدادات' },
    { icon: Bell, t: 'مواعيد وتنبيهات', d: 'كل ما يخص رحلتك التدريبية' },
    { icon: ShieldCheck, t: 'آمنة ومحمية', d: 'بياناتك لا تظهر إلا لك' },
  ]
  return (
    <DarkStage className="py-24 md:py-32">
      <div className="container-x grid items-center gap-14 lg:grid-cols-[1fr_1.1fr]">
        <div>
          <Headline light eyebrow="بوابة المتدربين" title="كل تفاصيل رحلتك التدريبية" rest="في مكان واحد، لحظة بلحظة." />
          <Reveal delay={0.1}><p className="mt-6 max-w-md text-lg leading-relaxed text-white/65">ادخل برقم الهوية ورقم الجوال المسجّل لدينا، وتابع برنامجك ومستواك ودفعتك وحسابك ومدفوعاتك.</p></Reveal>
          <div className="mt-10 grid gap-x-6 gap-y-6 sm:grid-cols-2">
            {feats.map((c, i) => (
              <Reveal key={c.t} delay={0.1 + i * 0.06} className="flex gap-3">
                <span className="grid h-10 w-10 shrink-0 place-items-center rounded-xl" style={{ background: `color-mix(in oklab, ${at(palette, i)} 22%, transparent)`, color: `color-mix(in oklab, ${at(palette, i)} 70%, white)` }}><c.icon className="h-5 w-5" /></span>
                <span><b className="block font-display text-[15.5px]">{c.t}</b><span className="text-sm leading-relaxed text-white/55">{c.d}</span></span>
              </Reveal>
            ))}
          </div>
          <Magnetic>
            <Link to="/portal" className="btn-gold group mt-10 !px-8 !py-4">دخول البوابة <ArrowLeft className="h-5 w-5 transition group-hover:-translate-x-1" /></Link>
          </Magnetic>
        </div>
        <Reveal y={50}>
          <div className="demo-stage story-stage" style={{ ['--stage' as string]: at(palette, 1) }}>
            <div className="demo-stage-bg" aria-hidden />
            <div className="story-shot"><Screen step={3} data={data} active /></div>
            <div className="demo-chip demo-chip-pay">
              <span className="demo-chip-icon"><CalendarDays className="h-[1.1em] w-[1.1em]" /></span>
              <span><b className="block font-display text-[.95em] text-ink">تم تسجيل السداد</b><span className="text-[.8em] text-muted">يظهر في حسابك فورًا</span></span>
            </div>
          </div>
        </Reveal>
      </div>
    </DarkStage>
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
        <div className="flex w-max animate-marquee gap-6 py-2 hover:[animation-play-state:paused]">
          {[...loop, ...loop].map((p, i) => {
            // each partner in its own colours, on a white card so dark logos stay readable in dark mode
            const img = <img src={p.logo!} alt={p.name} title={p.name} className="h-12 w-auto max-w-[9rem] object-contain" loading="lazy" decoding="async" />
            const card = 'grid h-20 min-w-[9.5rem] shrink-0 place-items-center rounded-2xl border border-line bg-white px-6 shadow-soft transition hover:-translate-y-1 hover:shadow-lift'
            return p.url ? <a key={i} href={p.url} target="_blank" rel="noopener noreferrer" className={card}>{img}</a> : <div key={i} className={card}>{img}</div>
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

