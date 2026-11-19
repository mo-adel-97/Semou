import { useEffect, useState, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { FlaskConical } from 'lucide-react'
import { api } from '../api'
import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom'
import { AnimatePresence, motion } from 'motion/react'
import { ChevronDown, ExternalLink, KeyRound, Settings2, LayoutDashboard, LogOut, Menu, PanelRightClose, PanelRightOpen, Search } from 'lucide-react'
import { useAuth } from '../auth'
import { useAllowedMenu } from '../useAllowedMenu'
import { alerts } from '../lib/alerts'
import { ErrorBoundary } from './ErrorBoundary'
import { CommandPalette } from './CommandPalette'
import { ThemePicker } from './ThemePicker'
import { useUiConfig } from '../uiConfig'
import { DraftBar } from './DraftBar'
import { TextPicker } from './TextPicker'

const gregorian = new Intl.DateTimeFormat('ar-EG', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
const hijri = new Intl.DateTimeFormat('ar-SA-u-ca-islamic-umalqura', { day: 'numeric', month: 'long', year: 'numeric' })

export const initialsOf = (name?: string | null) =>
  (name ?? '؟').trim().split(/\s+/).slice(0, 2).map(w => w[0]).join('')

export function Shell({ children }: { children: ReactNode }) {
  const { me, logout, can } = useAuth()
  const groups = useAllowedMenu()
  const { brand } = useUiConfig()
  const loc = useLocation()
  const nav = useNavigate()
  const activeKey = loc.pathname.startsWith('/s/') ? loc.pathname.slice(3) : ''
  const activeGroup = groups.find(g => g.items.some(i => i.key === activeKey))?.key

  const [collapsed, setCollapsed] = useState(() => localStorage.getItem('semou.side') === '1')
  const [mobileOpen, setMobileOpen] = useState(false)
  const [manualOpen, setManualOpen] = useState<Record<string, boolean>>({})
  const [palette, setPalette] = useState(false)
  const [userMenu, setUserMenu] = useState(false)
  const [now] = useState(() => new Date())

  // The group holding the current screen is open unless the user closed it.
  const isOpen = (key: string) => manualOpen[key] ?? key === activeGroup

  useEffect(() => { localStorage.setItem('semou.side', collapsed ? '1' : '0') }, [collapsed])
  // The content column is the scroll container: start every screen from the top.
  useEffect(() => { document.querySelector('.main-col')?.scrollTo({ top: 0 }) }, [loc.pathname])

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') { e.preventDefault(); setPalette(p => !p) }
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [])

  const go = (path: string) => { setMobileOpen(false); setUserMenu(false); nav(path) }

  const confirmLogout = async () => {
    setUserMenu(false)
    if (await alerts.confirm({ title: 'تسجيل الخروج؟', text: 'سيتم إنهاء جلستك الحالية على هذا الجهاز', confirmText: 'خروج', danger: true })) logout()
  }

  const initials = initialsOf(me?.fullName)
  // Requests waiting for review: refreshed every 30 seconds (and when the window regains focus).
  const canReview = can('webregorder', 'view') || can('webpayorder', 'view') || can('webstudyrequest', 'view')
  const pending = useQuery({ queryKey: ['requests', 'counts'], enabled: canReview, refetchInterval: 30_000, meta: { silent: true },
    queryFn: async () => {
      const [r, s] = await Promise.all([
        can('webregorder', 'view') || can('webpayorder', 'view') ? api<{ reg: number; pay: number }>('/requests/counts') : Promise.resolve({ reg: 0, pay: 0 }),
        can('webstudyrequest', 'view') ? api<{ count: number }>('/study-requests/count') : Promise.resolve({ count: 0 }),
      ])
      return { ...r, study: s.count }
    } })

  return (
    <div className={`shell ${collapsed ? 'collapsed' : ''} ${mobileOpen ? 'mobile-open' : ''}`}>
      <aside className="side">
        <Link to="/" className="side-brand" onClick={() => setMobileOpen(false)}>
          <span className="logo"><img src="/logo.jpeg" alt="" /></span>
          <span className="txt"><b>{brand.name}</b><small>{brand.tagline}</small></span>
        </Link>
        <nav className="side-nav">
          <NavLink to="/" end className="nav-link" title={brand.home} onClick={() => setMobileOpen(false)}>
            <LayoutDashboard size={19} /><span>{brand.home}</span>
          </NavLink>
          <div className="side-label">{brand.sections}</div>
          {groups.map(g => {
            const Icon = g.icon
            const open = isOpen(g.key)
            return (
              <div key={g.key}>
                <button className={`nav-group ${open ? 'open' : ''} ${activeGroup === g.key ? 'has-active' : ''}`} title={g.title}
                  onClick={() => {
                    if (collapsed) { setCollapsed(false); setManualOpen(o => ({ ...o, [g.key]: true })) }
                    else setManualOpen(o => ({ ...o, [g.key]: !open }))
                  }}>
                  <Icon size={19} /><span>{g.title}</span>
                  {(() => { const n = g.items.reduce((t, i) => t + (i.badge ? pending.data?.[i.badge] ?? 0 : 0), 0); return n > 0 ? <span className="nb g">{n.toLocaleString('ar-EG')}</span> : null })()}
                  <ChevronDown size={16} className="chev" />
                </button>
                <AnimatePresence initial={false}>
                  {open && (
                    <motion.div className="nav-sub" initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} transition={{ duration: 0.2, ease: 'easeOut' }}>
                      <div style={{ padding: '2px 0 6px' }}>
                        {g.items.map(i => {
                          const n = i.badge ? pending.data?.[i.badge] ?? 0 : 0
                          const Ic = i.icon
                          if (i.href) return (
                            <a key={i.key} href={i.href} target={i.newTab ? '_blank' : undefined} rel="noopener noreferrer" title={i.title} className={Ic ? 'with-ic' : ''} onClick={() => setMobileOpen(false)}>
                              {Ic && <Ic size={15} className="ic" />}{i.title}<ExternalLink size={12} className="ext" />
                            </a>
                          )
                          return (
                            <NavLink key={i.key} to={`/s/${i.key}`} title={i.title} className={Ic ? 'with-ic' : ''} onClick={() => setMobileOpen(false)}>
                              {Ic && <Ic size={15} className="ic" />}{i.title}{n > 0 ? <span className="nb" title="بانتظار المراجعة">{n.toLocaleString('ar-EG')}</span> : i.ported && <span className="new">جاهزة</span>}
                            </NavLink>
                          )
                        })}
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>
              </div>
            )
          })}
        </nav>
        <div className="side-foot">
          <div className="side-user">
            <span className="avatar">{initials}</span>
            <span className="who"><b>{me?.fullName}</b><small>متصل الآن</small></span>
          </div>
        </div>
      </aside>

      <div className="main-col">
        <TestBanner />
        <header className="top">
          <button className="btn ghost icon mobile-only" onClick={() => setMobileOpen(o => !o)} aria-label="القائمة"><Menu size={20} /></button>
          <button className="btn ghost icon desktop-only" onClick={() => setCollapsed(c => !c)} aria-label="طي القائمة">
            {collapsed ? <PanelRightOpen size={20} /> : <PanelRightClose size={20} />}
          </button>
          <button className="search-trigger" onClick={() => setPalette(true)}>
            <Search size={17} /> <span className="desktop-only">ابحث عن أي شاشة...</span>
            <kbd className="desktop-only">Ctrl K</kbd>
          </button>
          <div className="top-right">
            <div className="date-pill">
              <b>{gregorian.format(now)}</b>
              <small>{hijri.format(now)}</small>
            </div>
            <ThemePicker />
            <div className="pop-anchor">
              <button className="btn ghost" style={{ paddingInline: 6 }} onClick={() => setUserMenu(m => !m)} aria-label="حسابي">
                <span className="avatar" style={{ width: 34, height: 34 }}>{initials}</span>
                <ChevronDown size={15} />
              </button>
              <AnimatePresence>
                {userMenu && (
                  <>
                    <div className="click-away" onClick={() => setUserMenu(false)} />
                    <motion.div className="menu-pop" initial={{ opacity: 0, scale: 0.95, y: -6 }} animate={{ opacity: 1, scale: 1, y: 0 }} exit={{ opacity: 0, scale: 0.95, y: -6 }} transition={{ duration: 0.14 }}>
                      <div className="head"><b>{me?.fullName}</b><small>مستخدم النظام</small></div>
                      {can('websettings', 'view') && <button className="item" onClick={() => go('/s/general-settings')}><Settings2 size={16} /> الإعدادات العامة</button>}
                      <button className="item" onClick={() => go('/s/change-password')}><KeyRound size={16} /> تغيير كلمة المرور</button>
                      <button className="item danger" onClick={confirmLogout}><LogOut size={16} /> تسجيل خروج</button>
                    </motion.div>
                  </>
                )}
              </AnimatePresence>
            </div>
          </div>
        </header>
        <main className="content"><ErrorBoundary resetKey={loc.pathname}>{children}</ErrorBoundary></main>
      </div>

      <AnimatePresence>
        {mobileOpen && <motion.div className="overlay" style={{ zIndex: 35 }} initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setMobileOpen(false)} />}
      </AnimatePresence>
      {palette && <CommandPalette onClose={() => setPalette(false)} />}
      <DraftBar />
      <TextPicker />
    </div>
  )
}

export function PageHead({ title, subtitle, group, actions }: { title: string; subtitle?: string; group?: string; actions?: ReactNode }) {
  return (
    <div className="page-head">
      <div>
        <div className="crumbs"><Link to="/">الرئيسية</Link>{group && <><span>/</span><span>{group}</span></>}</div>
        <h1>{title}</h1>
        {subtitle && <p>{subtitle}</p>}
      </div>
      {actions && <div className="actions">{actions}</div>}
    </div>
  )
}

/** Big warning strip when the app runs on the test copy of the database. */
export function TestBanner() {
  const info = useQuery({ queryKey: ['app', 'info'], queryFn: () => api<{ testMode: boolean; database: string | null }>('/app/info'), staleTime: Infinity, meta: { silent: true } })
  if (!info.data?.testMode) return null
  return (
    <div className="test-banner" role="status">
      <FlaskConical size={18} />
      <b>نسخة تجريبية</b>
      <span>أنت تعمل على قاعدة البيانات التجريبية ({info.data.database}). أي حفظ هنا لا يؤثر على البيانات الحقيقية، ورفع المرفقات تجريبي فقط.</span>
    </div>
  )
}
