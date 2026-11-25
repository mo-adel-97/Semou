import { CalendarDays, Check, ClipboardCheck, GraduationCap, Inbox, LayoutGrid, MousePointerClick, PhoneCall, Rocket, Users, Wallet } from 'lucide-react'
import { AnimatePresence, motion, useMotionValueEvent, useScroll } from 'motion/react'
import { useEffect, useRef, useState, type ReactNode } from 'react'
import BrandLogo from '../../components/BrandLogo'
import { LITE, useLenis } from '../../components/ui'
import { cx } from '../../lib/format'
import { PROGRAM_TYPES, type HomeData, type Program, type ProgramType } from '../../lib/types'
import { AppWindow, Headline, REDUCED, at, useOnScreen, usePalette, useTyping } from './kit'

const EASE = [0.22, 1, 0.36, 1] as const

/* =====================================================================
   «رحلتك معنا» as a story told while scrolling: the section pins, the four steps take turns, and each one
   shows the real screen behind it: the programme catalogue, the registration form, the reception of the
   ERP, and the trainee portal. The stage takes a new identity colour at every step.
   ===================================================================== */

const STEPS = [
  { icon: MousePointerClick, title: 'اختر برنامجك', text: 'تصفّح الدبلومات والدورات المعتمدة واختر ما يناسب طموحك.' },
  { icon: ClipboardCheck, title: 'سجّل اهتمامك', text: 'املأ نموذج التسجيل في أقل من دقيقة من أي جهاز.' },
  { icon: PhoneCall, title: 'تواصل فريق القبول', text: 'يصل طلبك فورًا إلى نظام المعهد، ويتواصل معك فريق القبول لتأكيد التسجيل.' },
  { icon: Rocket, title: 'ابدأ رحلتك', text: 'انطلق في التدريب وتابع برنامجك ومستواك ومدفوعاتك من بوابة المتدربين.' },
]

export default function Story({ data }: { data: HomeData }) {
  const palette = usePalette()
  if (LITE || REDUCED) return <StoryStacked data={data} palette={palette} />
  return <StoryPinned data={data} palette={palette} />
}

function StoryPinned({ data, palette }: { data: HomeData; palette: string[] }) {
  const ref = useRef<HTMLElement>(null)
  const lenis = useLenis()
  const { scrollYProgress } = useScroll({ target: ref, offset: ['start start', 'end end'] })
  const [step, setStep] = useState(0)
  const [prog, setProg] = useState(0)
  useMotionValueEvent(scrollYProgress, 'change', (v) => {
    const n = STEPS.length
    setStep(Math.min(n - 1, Math.max(0, Math.floor(v * n))))
    setProg(Math.min(1, Math.max(0, v * n - Math.floor(v * n))))
  })
  const go = (i: number) => {
    const el = ref.current
    if (!el) return
    const top = el.getBoundingClientRect().top + window.scrollY + (el.offsetHeight - window.innerHeight) * ((i + 0.15) / STEPS.length)
    if (lenis) lenis.scrollTo(top, { duration: 1.2 }); else window.scrollTo({ top, behavior: 'smooth' })
  }
  return (
    <section ref={ref} className="relative" style={{ height: `${STEPS.length * 85 + 40}vh` }}>
      <div className="sticky top-0 flex h-[100svh] items-center overflow-hidden">
        <div className="container-x grid w-full items-center gap-12 pt-16 lg:grid-cols-[0.82fr_1.18fr] xl:gap-20">
          <div>
            <Headline eyebrow="رحلتك معنا" title="من أول نقرة حتى أول يوم تدريب." rest="وكل طلب يصل فورًا لفريقنا." />
            <ol className="mt-8 space-y-1.5">
              {STEPS.map((s, i) => {
                const on = i === step
                return (
                  <li key={s.title}>
                    <button onClick={() => go(i)} className={cx('story-step', on && 'on')} style={{ ['--c' as string]: at(palette, i) }}>
                      <span className="story-step-icon"><s.icon className="h-5 w-5" /></span>
                      <span className="min-w-0 flex-1 text-start">
                        <span className="flex items-baseline gap-3">
                          <span className="font-display text-sm font-bold tabular-nums opacity-60">0{i + 1}</span>
                          <b className="font-display text-lg font-extrabold xl:text-xl">{s.title}</b>
                        </span>
                        <AnimatePresence initial={false}>
                          {on && (
                            <motion.span initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} transition={{ duration: 0.45, ease: EASE }} className="block overflow-hidden">
                              <span className="block pt-2 leading-relaxed text-muted">{s.text}</span>
                            </motion.span>
                          )}
                        </AnimatePresence>
                        <span className="story-step-bar"><span style={{ transform: `scaleX(${i < step ? 1 : on ? prog : 0})` }} /></span>
                      </span>
                    </button>
                  </li>
                )
              })}
            </ol>
          </div>
          <StoryStage step={step} data={data} color={at(palette, step)} />
        </div>
      </div>
    </section>
  )
}

function StoryStage({ step, data, color }: { step: number; data: HomeData; color: string }) {
  return (
    <div className="demo-stage story-stage story-pin" style={{ ['--stage' as string]: color }}>
      <div className="demo-stage-bg" aria-hidden />
      <AnimatePresence mode="wait">
        <motion.div
          key={step}
          initial={{ opacity: 0, y: 40, scale: 0.96, rotateX: 8 }}
          animate={{ opacity: 1, y: 0, scale: 1, rotateX: 0 }}
          exit={{ opacity: 0, y: -30, scale: 0.97, transition: { duration: 0.28 } }}
          transition={{ duration: 0.6, ease: EASE }}
          className="story-shot"
        >
          <Screen step={step} data={data} active />
        </motion.div>
      </AnimatePresence>
    </div>
  )
}

/** Phones / reduced motion: no pinning, each step with its screen under it. */
function StoryStacked({ data, palette }: { data: HomeData; palette: string[] }) {
  return (
    <section className="py-20">
      <div className="container-x">
        <Headline eyebrow="رحلتك معنا" title="من أول نقرة حتى أول يوم تدريب." rest="وكل طلب يصل فورًا لفريقنا." />
        <div className="mt-12 space-y-14">
          {STEPS.map((s, i) => <StackedStep key={s.title} i={i} data={data} color={at(palette, i)} />)}
        </div>
      </div>
    </section>
  )
}
function StackedStep({ i, data, color }: { i: number; data: HomeData; color: string }) {
  const [ref, on] = useOnScreen<HTMLDivElement>('-15% 0px')
  const s = STEPS[i]
  return (
    <div ref={ref}>
      <div className="story-step on" style={{ ['--c' as string]: color }}>
        <span className="story-step-icon"><s.icon className="h-5 w-5" /></span>
        <span className="min-w-0 flex-1">
          <span className="flex items-baseline gap-3"><span className="font-display text-sm font-bold opacity-60">0{i + 1}</span><b className="font-display text-lg font-extrabold">{s.title}</b></span>
          <span className="block pt-2 leading-relaxed text-muted">{s.text}</span>
        </span>
      </div>
      <div className="demo-stage story-stage mt-5" style={{ ['--stage' as string]: color }}>
        <div className="demo-stage-bg" aria-hidden />
        <div className="story-shot"><Screen step={i} data={data} active={on} /></div>
      </div>
    </div>
  )
}

/* =====================================================================
   The four screens. Drawn live from the site's own data (programme names, images, durations, counts).
   ===================================================================== */

export function Screen({ step, data, active }: { step: number; data: HomeData; active: boolean }) {
  if (step === 0) return <CatalogScreen data={data} active={active} />
  if (step === 1) return <FormScreen data={data} active={active} />
  if (step === 2) return <ReceptionScreen data={data} active={active} />
  return <PortalScreen data={data} active={active} />
}

/** a small "after a while" helper for the screens' own little animations */
function useAfter(active: boolean, ms: number) {
  const [on, setOn] = useState(REDUCED)
  useEffect(() => {
    if (REDUCED) return
    if (!active) { setOn(false); return }
    const t = setTimeout(() => setOn(true), ms)
    return () => clearTimeout(t)
  }, [active, ms])
  return on
}

function CatalogScreen({ data, active }: { data: HomeData; active: boolean }) {
  const types = (Object.keys(PROGRAM_TYPES) as ProgramType[]).filter((t) => data.counts[t])
  const list = [...data.programs].sort((a, b) => Number(!!b.image) - Number(!!a.image)).slice(0, 4)
  const hover = useAfter(active, 900)
  return (
    <AppWindow url="alsmow.com/programs">
      <div className="p-[1.1em]">
        <div className="flex flex-wrap gap-[.4em]">
          {types.map((t, i) => (
            <span key={t} className={cx('shot-tab', i === 0 && 'on')}>{PROGRAM_TYPES[t].plural} <b>{data.counts[t]}</b></span>
          ))}
        </div>
        <div className="mt-[1em] grid grid-cols-2 gap-[.75em]">
          {list.map((p, i) => (
            <div key={p.id} className={cx('shot-prog', hover && i === 1 && 'lift')}>
              <div className="shot-prog-img">{p.image ? <img src={p.image} alt="" loading="lazy" /> : null}<span>{PROGRAM_TYPES[p.type].label}</span></div>
              <div className="p-[.65em]">
                <b className="line-clamp-1 font-display text-[.95em] text-ink">{p.title}</b>
                <span className="mt-[.2em] block truncate text-[.78em] text-muted">{[p.duration, p.hours ? `${p.hours} ساعة` : null].filter(Boolean).join(' · ') || ' '}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </AppWindow>
  )
}

function FormScreen({ data, active }: { data: HomeData; active: boolean }) {
  const p = data.programs.find((x) => x.type === 'diploma') ?? data.programs[0]
  const name = useTyping('عبدالله محمد', active, 70)
  const phoneOn = useAfter(active, 1100)
  const phone = useTyping('05x xxx xxxx', phoneOn, 60)
  const sent = useAfter(active, 2700)
  return (
    <AppWindow url="alsmow.com/register">
      <div className="p-[1.3em]">
        <b className="font-display text-[1.2em] text-ink">سجّل اهتمامك</b>
        <span className="mt-[.2em] block text-[.82em] text-muted">نتواصل معك خلال وقت قصير</span>
        <div className="mt-[1em] space-y-[.7em]">
          <Field label="الاسم">{name}<Caret on={active && name.length < 12} /></Field>
          <Field label="رقم الجوال"><span dir="ltr">{phone}</span></Field>
          <Field label="البرنامج" select>{p?.title}</Field>
        </div>
        <div className={cx('shot-submit', sent && 'done')}>
          <AnimatePresence mode="wait" initial={false}>
            {sent
              ? <motion.span key="d" initial={{ opacity: 0, y: 6 }} animate={{ opacity: 1, y: 0 }} className="flex items-center gap-[.4em]"><Check className="h-[1.1em] w-[1.1em]" /> تم إرسال طلبك</motion.span>
              : <motion.span key="s" exit={{ opacity: 0, y: -6 }}>إرسال الطلب</motion.span>}
          </AnimatePresence>
        </div>
      </div>
    </AppWindow>
  )
}
function Field({ label, select, children }: { label: string; select?: boolean; children: ReactNode }) {
  return (
    <div>
      <span className="mb-[.3em] block text-[.78em] font-semibold text-ink-2">{label}</span>
      <div className={cx('shot-field', select && 'select')}>{children}</div>
    </div>
  )
}
const Caret = ({ on }: { on: boolean }) => (on ? <span className="demo-caret" /> : null)

function ReceptionScreen({ data, active }: { data: HomeData; active: boolean }) {
  const progs = data.programs.map((p) => p.title)
  const arrived = useAfter(active, 700)
  const called = useAfter(active, 2300)
  const rows: { p: string; st: string; tone: string; when: string }[] = [
    { p: progs[1] ?? '', st: 'تم التسجيل', tone: 'ok', when: '09:40' },
    { p: progs[2] ?? '', st: 'تم التواصل', tone: 'info', when: '09:12' },
    { p: progs[3] ?? '', st: 'تم التسجيل', tone: 'ok', when: '08:55' },
  ]
  return (
    <AppWindow title="نظام السمو · الاستقبال" className="shot-erp">
      <div className="flex h-full">
        <div className="shot-side" aria-hidden>
          <BrandLogo src={data.settings.general.logo} animate={false} className="h-[2em] w-[2em]" />
          <span className="on"><Inbox /></span><span><Users /></span><span><LayoutGrid /></span><span><Wallet /></span>
        </div>
        <div className="min-w-0 flex-1 p-[1em]">
          <div className="flex items-center justify-between">
            <b className="font-display text-[1.05em] text-ink">طلبات الموقع</b>
            <span className="shot-badge">{arrived ? 1 : 0} جديد</span>
          </div>
          <div className="shot-table mt-[.8em]">
            <div className="shot-tr head"><span>المتقدم</span><span>البرنامج</span><span>الحالة</span></div>
            <AnimatePresence>
              {arrived && (
                <motion.div key="new" initial={{ opacity: 0, height: 0, backgroundColor: 'color-mix(in oklab, var(--stage) 30%, transparent)' }} animate={{ opacity: 1, height: 'auto', backgroundColor: 'color-mix(in oklab, var(--stage) 10%, transparent)' }} transition={{ duration: 0.6, ease: EASE }} className="shot-tr new">
                  <span className="flex items-center gap-[.4em]"><i className="shot-av">ع</i>عبدالله محمد</span>
                  <span className="truncate">{data.programs.find((x) => x.type === 'diploma')?.title ?? progs[0]}</span>
                  <span><AnimatePresence mode="wait" initial={false}>
                    {called
                      ? <motion.i key="c" initial={{ scale: 0.6, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} className="shot-pill info"><PhoneCall className="h-[1em] w-[1em]" /> تم التواصل</motion.i>
                      : <motion.i key="n" exit={{ scale: 0.6, opacity: 0 }} className="shot-pill new">جديد</motion.i>}
                  </AnimatePresence></span>
                </motion.div>
              )}
            </AnimatePresence>
            {rows.map((r, i) => (
              <div key={i} className="shot-tr">
                <span className="flex items-center gap-[.4em]"><i className="shot-av muted" /><i className="shot-skel" /></span>
                <span className="truncate">{r.p}</span>
                <span><i className={cx('shot-pill', r.tone)}>{r.st}</i></span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </AppWindow>
  )
}

function PortalScreen({ data, active }: { data: HomeData; active: boolean }) {
  const p: Program | undefined = data.programs.find((x) => x.type === 'diploma') ?? data.programs[0]
  const go = useAfter(active, 400)
  const paid = 0.62
  const C = 2 * Math.PI * 26
  return (
    <AppWindow url="alsmow.com/portal">
      <div className="p-[1.2em]">
        <div className="flex items-center gap-[.7em]">
          <span className="grid h-[2.6em] w-[2.6em] place-items-center rounded-full bg-[var(--stage)] font-display font-black text-white">ع</span>
          <span>
            <span className="block text-[.8em] text-muted">أهلًا بك</span>
            <b className="font-display text-[1.05em] text-ink">عبدالله محمد</b>
          </span>
        </div>
        <div className="mt-[1em] grid grid-cols-[1.25fr_1fr] gap-[.7em]">
          <div className="shot-tile">
            <span className="flex items-center gap-[.4em] text-[.78em] font-bold text-muted"><GraduationCap className="h-[1.1em] w-[1.1em]" /> برنامجي</span>
            <b className="mt-[.35em] line-clamp-2 block font-display text-[.98em] leading-snug text-ink">{p?.title}</b>
            <span className="mt-[.8em] flex justify-between text-[.76em] text-ink-2"><span>المستوى الثالث</span><b>3 / 5</b></span>
            <span className="shot-bar"><motion.span initial={false} animate={{ scaleX: go ? 0.6 : 0 }} transition={{ duration: 1.2, ease: EASE }} /></span>
          </div>
          <div className="shot-tile items-center text-center">
            <span className="text-[.78em] font-bold text-muted">المدفوعات</span>
            <svg viewBox="0 0 64 64" className="mx-auto mt-[.3em] h-[4.6em] w-[4.6em] -rotate-90">
              <circle cx="32" cy="32" r="26" fill="none" stroke="var(--line)" strokeWidth="7" />
              <motion.circle cx="32" cy="32" r="26" fill="none" stroke="var(--stage)" strokeWidth="7" strokeLinecap="round" strokeDasharray={C} initial={false} animate={{ strokeDashoffset: go ? C * (1 - paid) : C }} transition={{ duration: 1.4, ease: EASE, delay: 0.2 }} />
            </svg>
            <b className="mt-[-2.9em] mb-[1.9em] block font-display text-[.95em] text-ink">{Math.round(paid * 100)}%</b>
          </div>
        </div>
        <div className="shot-tile mt-[.7em] !flex-row items-center gap-[.6em]">
          <span className="grid h-[2.2em] w-[2.2em] shrink-0 place-items-center rounded-[.7em] bg-[color-mix(in_oklab,var(--stage)_14%,transparent)] text-[var(--stage)]"><CalendarDays className="h-[1.1em] w-[1.1em]" /></span>
          <span className="min-w-0 flex-1"><b className="block text-[.86em] text-ink">القسط القادم</b><span className="text-[.76em] text-muted">تذكير قبل موعده</span></span>
          <i className="shot-pill info">قريبًا</i>
        </div>
      </div>
    </AppWindow>
  )
}
