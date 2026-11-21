import {
  ArrowLeft, ArrowUp, BookOpen, Briefcase, ChevronDown, GraduationCap, Mail, MapPin, Menu,
  Phone, Sparkles, UserRound, X,
} from 'lucide-react'
import { AnimatePresence, motion, useMotionValueEvent, useScroll } from 'motion/react'
import { lazy, Suspense, useEffect, useState, type ReactNode } from 'react'
import { Link, NavLink, useLocation, useOutlet } from 'react-router-dom'
import { SocialLinks, WhatsAppIcon } from '../components/BrandIcons'
import { LITE, Magnetic, ScrollProgress, SmoothScroll, useLenis } from '../components/ui'
import { cx, waNumber } from '../lib/format'
import { useSite } from '../lib/queries'
import { useTheme } from '../lib/theme'
import { PROGRAM_TYPES, type ProgramType } from '../lib/types'

import IntroBirds from './IntroBirds'
import GiftBox from './GiftBox'
import SiteTexts from './SiteTexts'
import { startTracking, trackView } from '../lib/track'
import Fx from '../components/Fx'
import RouteCurtain from '../components/RouteCurtain'
import BrandLogo from '../components/BrandLogo'
import { ButterflyOrbit, ButterflyShow } from '../components/Butterflies'

const ChatWidget = lazy(() => import('./ChatWidget'))

const TYPE_ICON: Record<ProgramType, typeof GraduationCap> = { diploma: GraduationCap, qualifying: Briefcase, development: Sparkles }

type NavItem = { label: string; to?: string; href?: string; mega?: 'programs'; children?: { label: string; to: string }[] }

function useNav(): NavItem[] {
  const { data } = useSite()
  const pages = data?.pages ?? []
  const lms = data?.settings.general.lms_url
  return [
    { label: 'الرئيسية', to: '/' },
    { label: 'عن المعهد', to: '/about' },
    { label: 'البرامج', to: '/programs', mega: 'programs' },
    ...(pages.length ? [{ label: 'الإرشاد الأكاديمي', children: pages.map((p) => ({ label: p.title, to: `/p/${p.slug}` })) }] : []),
    { label: 'المدونة', to: '/blog' },
    ...(lms ? [{ label: 'منصة التعليم', href: lms }] : []),
    { label: 'تواصل معنا', to: '/contact' },
  ]
}

function Logo({ light, compact, flutter }: { light?: boolean; compact?: boolean; flutter?: boolean }) {
  const { data } = useSite()
  const g = data?.settings.general
  return (
    <Link to="/" className={cx('group flex items-center gap-3', compact ? 'shrink-0' : 'min-w-0')} aria-label="الصفحة الرئيسية">
      {flutter ? (
        // home page: two small butterflies circle the logo
        <ButterflyOrbit className="shrink-0" rings={[{ r: '165%', t: '8s', s: 22 }, { r: '210%', t: '12s', s: 17, reverse: true }]}>
          <BrandLogo src={g?.logo} className="h-12 w-12 shrink-0 md:h-14 md:w-14" />
        </ButterflyOrbit>
      ) : (
        <BrandLogo src={g?.logo} className={compact ? 'h-12 w-12 shrink-0 md:h-14 md:w-14' : 'h-14 w-14 shrink-0 md:h-16 md:w-16'} />
      )}
      {/* header: name only, on one line (hidden at lg where the menu needs the room); the tagline lives in the footer / mobile menu.
          Full size (mobile menu, footer): the name may wrap and a long tagline is cut at two lines, never past the edge */}
      <div className={cx('min-w-0 leading-tight', compact && 'hidden sm:block lg:hidden xl:block')}>
        <div className={cx('font-display font-extrabold', compact ? 'text-[16px] whitespace-nowrap' : 'text-[17px] text-balance', light ? 'text-white' : 'text-ink')}>{g?.site_name || 'معهد السمو للتدريب'}</div>
        {!compact && <div className={cx('mt-0.5 line-clamp-2 text-xs leading-relaxed', light ? 'text-white/55' : 'text-muted')}>{g?.tagline || 'مستقبلك يبدأ من هنا'}</div>}
      </div>
    </Link>
  )
}

/* ---------------- Theme picker: in components/ThemeStudio.tsx ---------------- */
import { ThemePicker } from '../components/ThemeStudio'
export { ThemePicker }

/* ---------------- Header ---------------- */

function ProgramsMega() {
  const { data } = useSite()
  return (
    <div className="grid w-[40rem] grid-cols-[1fr_15rem] gap-2 rounded-3xl border border-line bg-surface p-2 shadow-lift">
      <div className="space-y-1 p-1">
        {(Object.keys(PROGRAM_TYPES) as ProgramType[]).map((t) => {
          const Icon = TYPE_ICON[t]
          return (
            <Link key={t} to={`/programs/type/${t}`} className="group flex items-start gap-4 rounded-2xl p-3 transition hover:bg-sand">
              <span className="grid h-12 w-12 shrink-0 place-items-center rounded-2xl bg-brand-50 text-brand-700 transition group-hover:scale-110 group-hover:bg-primary group-hover:text-white">
                <Icon className="h-5 w-5" />
              </span>
              <div className="min-w-0 flex-1">
                <div className="flex items-center gap-2 font-display font-bold text-ink">
                  {PROGRAM_TYPES[t].plural}
                  {data && <span className="chip bg-sand !px-2 !py-0 text-muted">{data.counts[t]}</span>}
                </div>
                <div className="mt-0.5 text-sm leading-relaxed text-muted">{PROGRAM_TYPES[t].blurb}</div>
              </div>
            </Link>
          )
        })}
      </div>
      <Link to="/programs" className="group relative flex flex-col justify-end overflow-hidden rounded-2xl bg-deep-900 p-5 text-white">
        <div className="pattern-light absolute inset-0" />
        <div className="absolute -top-10 -left-10 h-40 w-40 rounded-full bg-brand-500/50 blur-3xl transition group-hover:scale-125" />
        <BookOpen className="relative mb-auto h-8 w-8 text-gold-300" />
        <div className="relative mt-10 font-display text-xl font-extrabold">كل البرامج</div>
        <div className="relative mt-1 text-sm text-white/65">استكشف جميع الدبلومات والدورات المعتمدة</div>
        <span className="relative mt-4 inline-flex items-center gap-1 text-sm font-bold text-gold-300">تصفّح الآن <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" /></span>
      </Link>
    </div>
  )
}

function Header() {
  const nav = useNav()
  const { data } = useSite()
  const loc = useLocation()
  const [open, setOpen] = useState(false)
  const [scrolled, setScrolled] = useState(false)
  const [hidden, setHidden] = useState(false)
  const [drop, setDrop] = useState<string | null>(null)
  const { scrollY } = useScroll()
  const lenis = useLenis()
  useMotionValueEvent(scrollY, 'change', (y) => {
    const prev = scrollY.getPrevious() ?? 0
    setScrolled(y > 30)
    setHidden(y > 400 && y > prev && !drop)
  })
  useEffect(() => { setOpen(false); setDrop(null) }, [loc.pathname])
  useEffect(() => {
    if (open) lenis?.stop(); else lenis?.start()
    document.body.style.overflow = open ? 'hidden' : ''
    // while the mobile menu is open, the page behind it holds still (frees the GPU for the menu)
    document.documentElement.classList.toggle('menu-open', open)
  }, [open, lenis])

  const ann = data?.settings.general.announcement
  const annLink = data?.settings.general.announcement_link
  const annCta = data?.settings.general.announcement_cta?.trim() || 'سجّل الآن'
  const registerHref = data?.settings.general.register_url
  const registerText = data?.settings.general.register_text?.trim() || 'سجّل الآن'
  const gen = data?.settings.general
  const portalShow = gen?.portal_hide !== true
  const portalText = gen?.portal_text?.trim() || 'بوابة المتدربين'
  const portalLink = gen?.portal_link?.trim() || '/portal'
  const PortalBtn = ({ className, children }: { className: string; children: ReactNode }) =>
    /^https?:\/\//.test(portalLink)
      ? <a href={portalLink} target="_blank" rel="noopener noreferrer" className={className}>{children}</a>
      : <Link to={portalLink} className={className}>{children}</Link>
  const RegisterBtn = ({ className }: { className: string }) =>
    registerHref ? <a href={registerHref} target="_blank" rel="noopener noreferrer" className={className}>{registerText}</a> : <Link to="/register" className={className}>{registerText}</Link>

  return (
    <>
      <motion.header animate={{ y: hidden ? '-120%' : '0%' }} transition={{ duration: 0.35, ease: [0.22, 1, 0.36, 1] }} className="fixed inset-x-0 top-0 z-50">
        <AnimatePresence initial={false}>
          {ann && !scrolled && (
            <motion.div initial={{ height: 0 }} animate={{ height: 'auto' }} exit={{ height: 0 }} className="overflow-hidden bg-deep-950 text-white">
              <div className="container-x flex items-center justify-center gap-3 py-2 text-sm">
                <span className="relative flex h-2 w-2 shrink-0">
                  <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-gold-400 opacity-75" />
                  <span className="relative inline-flex h-2 w-2 rounded-full bg-gold-400" />
                </span>
                <span className="truncate">{ann}</span>
                {annLink && (/^https?:\/\//.test(annLink)
                  ? <a href={annLink} target="_blank" rel="noopener noreferrer" className="shrink-0 font-bold text-gold-300 underline-offset-4 hover:underline">{annCta} ←</a>
                  : <Link to={annLink} className="shrink-0 font-bold text-gold-300 underline-offset-4 hover:underline">{annCta} ←</Link>)}
              </div>
            </motion.div>
          )}
        </AnimatePresence>
        <div className="container-x pt-3">
          <div className={cx('flex items-center justify-between gap-3 rounded-[1.75rem] px-3 py-2.5 transition-all duration-500 md:px-4', scrolled ? 'glass shadow-lift' : 'border border-transparent')}>
            <Logo compact flutter={loc.pathname === '/' && !data?.settings.butterflies?.hide} />
            <nav className="hidden items-center lg:flex" aria-label="القائمة الرئيسية" onMouseLeave={() => setDrop(null)}>
              {nav.map((item) => {
                const active = item.to ? (item.to === '/' ? loc.pathname === '/' : loc.pathname.startsWith(item.to)) : item.children?.some((c) => loc.pathname === c.to)
                const cls = cx('relative flex items-center gap-1 whitespace-nowrap rounded-full px-2.5 py-2 text-[14.5px] font-semibold transition-colors 2xl:px-4', active ? 'text-brand-700' : 'text-ink-2 hover:text-ink')
                const pill = active && <motion.span layoutId="nav-pill" className="absolute inset-0 -z-10 rounded-full bg-brand-50" transition={{ type: 'spring', stiffness: 380, damping: 30 }} />
                if (item.mega || item.children) {
                  return (
                    <div key={item.label} className="relative" onMouseEnter={() => setDrop(item.label)}>
                      <button className={cls} aria-expanded={drop === item.label} onClick={() => setDrop(drop === item.label ? null : item.label)}>
                        {pill}
                        {item.label}
                        <ChevronDown className={cx('h-4 w-4 transition', drop === item.label && 'rotate-180')} />
                      </button>
                      <AnimatePresence>
                        {drop === item.label && (
                          <motion.div
                            initial={{ opacity: 0, y: 12, scale: 0.98, filter: 'blur(4px)' }}
                            animate={{ opacity: 1, y: 0, scale: 1, filter: 'blur(0px)' }}
                            exit={{ opacity: 0, y: 8, scale: 0.98, filter: 'blur(4px)' }}
                            transition={{ duration: 0.22 }}
                            className="absolute top-full right-0 pt-4"
                          >
                            {item.mega ? <ProgramsMega /> : (
                              <div className="w-72 rounded-3xl border border-line bg-surface p-2 shadow-lift">
                                {item.children!.map((c, i) => (
                                  <motion.div key={c.to} initial={{ opacity: 0, x: 10 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: i * 0.03 }}>
                                    <Link to={c.to} className="flex items-center justify-between rounded-2xl px-4 py-2.5 text-[15px] text-ink-2 transition hover:bg-sand hover:text-ink">
                                      {c.label}
                                      <ArrowLeft className="h-4 w-4 opacity-0 transition group-hover:opacity-100" />
                                    </Link>
                                  </motion.div>
                                ))}
                              </div>
                            )}
                          </motion.div>
                        )}
                      </AnimatePresence>
                    </div>
                  )
                }
                return item.href ? (
                  <a key={item.label} href={item.href} target="_blank" rel="noopener noreferrer" className={cls}>{item.label}</a>
                ) : (
                  <NavLink key={item.label} to={item.to!} className={cls}>{pill}{item.label}</NavLink>
                )
              })}
            </nav>
            <div className="flex shrink-0 items-center gap-2">
              <div className="hidden sm:block"><ThemePicker /></div>
              {portalShow && (
                <PortalBtn className="hidden h-11 items-center gap-2 whitespace-nowrap rounded-2xl border border-line bg-surface/70 px-3 text-sm font-bold text-ink-2 transition hover:border-brand-300 hover:text-brand-700 md:flex 2xl:px-4">
                  <UserRound className="h-4 w-4 shrink-0" /> <span className="md:inline lg:hidden 2xl:inline">{portalText}</span>
                </PortalBtn>
              )}
              <Magnetic><RegisterBtn className="btn-primary hidden !h-11 !px-5 !py-0 whitespace-nowrap sm:inline-flex" /></Magnetic>
              <button onClick={() => setOpen(true)} className="grid h-11 w-11 place-items-center rounded-2xl bg-primary text-white lg:hidden" aria-label="فتح القائمة">
                <Menu className="h-5 w-5" />
              </button>
            </div>
          </div>
        </div>
      </motion.header>

      {/* Mobile menu */}
      <AnimatePresence>
        {open && (
          <motion.div className="fixed inset-0 z-[60] lg:hidden" initial="hidden" animate="show" exit="hidden">
            <motion.div variants={{ hidden: { opacity: 0 }, show: { opacity: 1 } }} className="absolute inset-0 bg-deep-950/60 backdrop-blur-sm" onClick={() => setOpen(false)} />
            <motion.aside
              variants={{ hidden: { x: '100%' }, show: { x: 0 } }}
              transition={{ type: 'spring', damping: 30, stiffness: 280 }}
              className="absolute inset-y-0 right-0 flex w-[90%] max-w-sm flex-col bg-bg shadow-2xl"
              aria-label="قائمة الجوال"
            >
              <div className="flex items-center justify-between gap-3 border-b border-line p-4">
                <Logo />
                <button onClick={() => setOpen(false)} className="grid h-10 w-10 shrink-0 place-items-center rounded-xl bg-surface" aria-label="إغلاق"><X className="h-5 w-5" /></button>
              </div>
              <motion.nav className="flex-1 overflow-y-auto p-4" variants={{ show: { transition: { staggerChildren: 0.05, delayChildren: 0.15 } } }}>
                {nav.map((item) => (
                  <motion.div key={item.label} variants={{ hidden: { opacity: 0, x: 30 }, show: { opacity: 1, x: 0 } }}>
                    {item.mega || item.children ? (
                      <details className="group mb-1">
                        <summary className="flex cursor-pointer list-none items-center justify-between rounded-2xl px-4 py-3 text-lg font-bold text-ink hover:bg-surface">
                          {item.label}
                          <ChevronDown className="h-4 w-4 transition group-open:rotate-180" />
                        </summary>
                        <div className="ms-4 border-s-2 border-brand-200 ps-3">
                          {(item.mega
                            ? [{ label: 'جميع البرامج', to: '/programs' }, ...(Object.keys(PROGRAM_TYPES) as ProgramType[]).map((t) => ({ label: PROGRAM_TYPES[t].plural, to: `/programs/type/${t}` }))]
                            : item.children!
                          ).map((c) => (
                            <Link key={c.to} to={c.to} className="block rounded-xl px-3 py-2.5 text-ink-2 hover:bg-surface">{c.label}</Link>
                          ))}
                        </div>
                      </details>
                    ) : item.href ? (
                      <a href={item.href} target="_blank" rel="noopener noreferrer" className="mb-1 block rounded-2xl px-4 py-3 text-lg font-bold text-ink hover:bg-surface">{item.label}</a>
                    ) : (
                      <NavLink to={item.to!} end={item.to === '/'} className={({ isActive }) => cx('mb-1 block rounded-2xl px-4 py-3 text-lg font-bold', isActive ? 'bg-primary text-white' : 'text-ink hover:bg-surface')}>
                        {item.label}
                      </NavLink>
                    )}
                  </motion.div>
                ))}
              </motion.nav>
              <div className="space-y-3 border-t border-line p-4">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-semibold text-muted">المظهر والألوان</span>
                  <ThemePicker align="left" up />
                </div>
                <div className={cx('grid gap-2', portalShow ? 'grid-cols-2' : 'grid-cols-1')}>
                  {portalShow && <PortalBtn className="btn-ghost !px-3"><UserRound className="h-4 w-4" /> {portalText}</PortalBtn>}
                  <RegisterBtn className="btn-primary !px-3" />
                </div>
              </div>
            </motion.aside>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  )
}

/* ---------------- Footer ---------------- */

function Footer() {
  const { data } = useSite()
  const s = data?.settings
  const c = s?.contact
  return (
    <footer className="grain relative overflow-hidden bg-deep-950 text-white/75">
      <div className="pattern-light absolute inset-0" />
      <div className="absolute -top-40 left-1/4 h-96 w-96 rounded-full bg-brand-500/25 blur-[120px]" />
      <div className="absolute -right-20 bottom-0 h-80 w-80 rounded-full bg-gold-400/10 blur-[100px]" />
      <div className="container-x relative pt-20 pb-8">
        <div className="mb-16 flex flex-col items-start justify-between gap-8 rounded-[2rem] border border-white/10 bg-white/[.04] p-8 backdrop-blur md:flex-row md:items-center md:p-10">
          <div>
            <h2 className="text-3xl font-black text-white md:text-4xl">جاهز تبدأ رحلتك؟</h2>
            <p className="mt-2 text-white/60">فريق القبول بانتظارك لمساعدتك في اختيار البرنامج المناسب.</p>
          </div>
          <div className="flex flex-wrap gap-3">
            <Magnetic><Link to="/register" className="btn-gold !px-7 !py-3.5">سجّل الآن <ArrowLeft className="h-4 w-4" /></Link></Magnetic>
            <Link to="/contact" className="btn border border-white/20 !px-7 !py-3.5 text-white hover:bg-white/10">تواصل معنا</Link>
          </div>
        </div>

        <div className="grid gap-12 md:grid-cols-2 lg:grid-cols-[1.4fr_1fr_1fr_1.3fr]">
          <div>
            <Logo light />
            <p className="mt-5 max-w-sm leading-relaxed text-white/55">{s?.about.summary ? `${s.about.summary.slice(0, 170)}…` : 'مؤسسة تدريبية معتمدة في حفر الباطن.'}</p>
            {s?.social && <SocialLinks social={s.social} className="mt-6" itemClass="bg-white/10 text-white hover:bg-gold-400 hover:text-deep-950 hover:-translate-y-1" />}
          </div>
          <FooterCol title="روابط سريعة" links={[['الرئيسية', '/'], ['عن المعهد', '/about'], ['جميع البرامج', '/programs'], ['المدونة', '/blog'], ['بوابة المتدربين', '/portal'], ['تواصل معنا', '/contact']]} />
          <FooterCol
            title="برامجنا"
            links={[
              ...(Object.keys(PROGRAM_TYPES) as ProgramType[]).map((t) => [PROGRAM_TYPES[t].plural, `/programs/type/${t}`] as [string, string]),
              ...(data?.pages.slice(0, 3).map((p) => [p.title, `/p/${p.slug}`] as [string, string]) ?? []),
            ]}
          />
          <div>
            <h3 className="mb-5 font-display text-lg font-bold text-white">تواصل معنا</h3>
            <ul className="space-y-4">
              {c?.phone && <FooterContact icon={Phone} href={`tel:${c.phone}`}><span dir="ltr">{c.phone}</span></FooterContact>}
              {c?.email && <FooterContact icon={Mail} href={`mailto:${c.email}`}><span className="break-all">{c.email}</span></FooterContact>}
              {c?.address && <FooterContact icon={MapPin}>{c.address}</FooterContact>}
            </ul>
          </div>
        </div>

        <div aria-hidden="true" className="pointer-events-none mt-16 overflow-hidden text-center font-display text-[22vw] leading-[0.8] font-black tracking-tighter text-transparent select-none [-webkit-text-stroke:1px_rgb(255_255_255/0.09)] md:text-[16vw]">
          السمو
        </div>

        <div className="mt-6 flex flex-col items-center justify-between gap-4 border-t border-white/10 pt-6 text-sm text-white/45 md:flex-row">
          <p>جميع الحقوق محفوظة © {new Date().getFullYear()} {s?.general.site_name || 'معهد السمو للتدريب'}</p>
          <div className="flex gap-5">
            {data?.pages.slice(3, 5).map((p) => <Link key={p.slug} to={`/p/${p.slug}`} className="hover:text-white">{p.title}</Link>)}
          </div>
        </div>
      </div>
    </footer>
  )
}

function FooterCol({ title, links }: { title: string; links: [string, string][] }) {
  return (
    <div>
      <h3 className="mb-5 font-display text-lg font-bold text-white">{title}</h3>
      <ul className="space-y-3">
        {links.map(([l, to]) => (
          <li key={to}>
            <Link to={to} className="group inline-flex items-center gap-2 transition hover:text-gold-300">
              <span className="h-px w-0 bg-gold-300 transition-all group-hover:w-3" />
              {l}
            </Link>
          </li>
        ))}
      </ul>
    </div>
  )
}

function FooterContact({ icon: Icon, href, children }: { icon: typeof Phone; href?: string; children: ReactNode }) {
  const inner = (
    <>
      <span className="grid h-10 w-10 shrink-0 place-items-center rounded-xl bg-white/10 transition group-hover:bg-gold-400 group-hover:text-deep-950"><Icon className="h-4 w-4" /></span>
      {children}
    </>
  )
  return <li>{href ? <a href={href} className="group flex items-center gap-3 transition hover:text-gold-300">{inner}</a> : <div className="flex items-center gap-3">{inner}</div>}</li>
}

/* ---------------- Floating actions ---------------- */

function Floating() {
  const { data } = useSite()
  const lenis = useLenis()
  const wa = waNumber(data?.settings.contact.whatsapp || data?.settings.contact.phone)
  const [show, setShow] = useState(false)
  const { scrollY } = useScroll()
  useMotionValueEvent(scrollY, 'change', (y) => setShow(y > 700))
  return (
    <div className="fixed bottom-5 left-5 z-40 flex flex-col items-center gap-3">
      <GiftBox />
      <AnimatePresence>
        {show && (
          <motion.button
            initial={{ opacity: 0, scale: 0.5, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.5, y: 20 }}
            onClick={() => (lenis ? lenis.scrollTo(0) : window.scrollTo({ top: 0, behavior: 'smooth' }))}
            className="grid h-11 w-11 place-items-center rounded-full border border-line bg-surface text-ink shadow-lift"
            aria-label="العودة للأعلى"
          >
            <ArrowUp className="h-5 w-5" />
          </motion.button>
        )}
      </AnimatePresence>
      {wa && (
        <motion.a
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 1, type: 'spring' }}
          whileHover={{ scale: 1.08 }}
          href={`https://wa.me/${wa}`}
          target="_blank"
          rel="noopener noreferrer"
          className="relative grid h-14 w-14 place-items-center rounded-full bg-[#25D366] text-white shadow-[0_12px_30px_-8px_rgba(37,211,102,.7)]"
          aria-label="تواصل عبر واتساب"
        >
          <span className="absolute inset-0 animate-ping rounded-full bg-[#25D366] opacity-25" />
          <WhatsAppIcon className="relative h-7 w-7" />
        </motion.a>
      )}
    </div>
  )
}

/* ---------------- Layout + page transitions ---------------- */

export default function Layout() {
  const { pathname } = useLocation()
  const outlet = useOutlet()
  const { data } = useSite()
  const { setDefaults } = useTheme()
  useEffect(() => { setDefaults(data?.settings.general.theme_color || data?.settings.general.theme_accent, data?.settings.general.theme_mode) }, [data, setDefaults])
  useEffect(() => { startTracking() }, [])
  useEffect(() => { trackView(pathname) }, [pathname])
  // the butterflies' scenes: everything (on/off, words, links, timing) comes from the dashboard
  const bf = data?.settings.butterflies
  const bfOn = !bf?.hide
  const letters = bfOn && !bf?.letters_hide ? (Array.isArray(bf?.letters) ? bf.letters : []).map((x) => ({ text: x.a, to: x.b })) : []
  const talks = bfOn && !bf?.chat_hide ? (Array.isArray(bf?.chat) ? bf.chat : []).filter((x) => x.b).map((x) => ({ q: x.a, a: x.b! })) : []

  return (
    <SmoothScroll>
      <ScrollToTop pathname={pathname} />
      <div className="site-shell flex min-h-dvh flex-col">
        <a href="#main" className="sr-only focus:not-sr-only focus:fixed focus:top-3 focus:right-3 focus:z-[100] focus:rounded-xl focus:bg-surface focus:px-4 focus:py-2">تخطي إلى المحتوى</a>
        <ScrollProgress />
        <Header />
        <AnimatePresence mode="wait" initial={false}>
          <motion.main
            id="main"
            key={pathname}
            className="flex-1"
            // phones: a quick fade only; blurring the whole page every navigation is very heavy on mobile GPUs
            initial={LITE ? { opacity: 0 } : { opacity: 0, y: 24, filter: 'blur(8px)' }}
            animate={LITE ? { opacity: 1 } : { opacity: 1, y: 0, filter: 'blur(0px)' }}
            exit={LITE ? { opacity: 0 } : { opacity: 0, y: -12, filter: 'blur(6px)' }}
            transition={{ duration: LITE ? 0.22 : 0.45, ease: [0.22, 1, 0.36, 1] }}
          >
            {outlet}
          </motion.main>
        </AnimatePresence>
        <Footer />
        <Floating />
        <Suspense fallback={null}><ChatWidget /></Suspense>
      </div>
      <IntroBirds />
      {(letters.length > 0 || talks.length > 0) && (
        <ButterflyShow
          letters={letters}
          talks={talks}
          asker={bf?.chat_asker}
          answerer={bf?.chat_answerer}
          first={Number(bf?.first) || 10}
          every={Number(bf?.every) || 35}
        />
      )}
      <Fx />
      <RouteCurtain />
      <SiteTexts />
    </SmoothScroll>
  )
}

function ScrollToTop({ pathname }: { pathname: string }) {
  const lenis = useLenis()
  useEffect(() => {
    // after the exit animation of the previous page
    const t = setTimeout(() => (lenis ? lenis.scrollTo(0, { immediate: true }) : window.scrollTo(0, 0)), LITE ? 220 : 450)
    return () => clearTimeout(t)
  }, [pathname, lenis])
  return null
}
