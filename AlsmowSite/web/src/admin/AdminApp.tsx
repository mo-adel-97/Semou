import {
  BadgeCheck, BarChart3, Bird, Bot, Building2, ExternalLink, FileSignature, FileText, GraduationCap, Handshake, Images, Inbox,
  LayoutDashboard, LineChart, Loader2, LogOut, Menu, MessageSquareQuote, Newspaper, PencilLine, Phone, Search, Settings, Share2, Sparkles,
  Type, UserCog, Users, X, type LucideIcon,
} from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { lazy, Suspense, useEffect, useState, type ReactNode } from 'react'
import { Link, NavLink, Route, Routes, useLocation } from 'react-router-dom'
import AdminSearch, { useFieldFocus } from './AdminSearch'
import BrandLogo from '../components/BrandLogo'

const AnalyticsPage = lazy(() => import('./AnalyticsPage'))
import { useQuery } from '@tanstack/react-query'
import { api } from '../lib/api'
import { cx } from '../lib/format'
import { ThemePicker } from '../site/Layout'
import { useLogout, useMe, useSchema } from './auth'
import Dashboard from './Dashboard'
import Login from './Login'
import MediaPage from './MediaPage'
import MessagesPage from './MessagesPage'
import ProfilePage from './ProfilePage'
import ResourceForm from './ResourceForm'
import ResourceList from './ResourceList'
import SettingsPage from './SettingsPage'
import UsersPage from './UsersPage'

export const ICONS: Record<string, LucideIcon> = {
  'graduation-cap': GraduationCap,
  newspaper: Newspaper,
  'file-text': FileText,
  'badge-check': BadgeCheck,
  handshake: Handshake,
  'file-signature': FileSignature,
  'message-square-quote': MessageSquareQuote,
  settings: Settings,
  sparkles: Sparkles,
  'building-2': Building2,
  'bar-chart-3': BarChart3,
  phone: Phone,
  'share-2': Share2,
  search: Search,
  type: Type,
  bird: Bird,
  'layout-dashboard': LayoutDashboard,
  bot: Bot,
}

export default function AdminApp() {
  const me = useMe()
  if (me.isLoading) {
    return (
      <div className="grid min-h-dvh place-items-center">
        <Loader2 className="h-8 w-8 animate-spin text-brand-600" />
      </div>
    )
  }
  if (!me.data) return <Login />
  return (
    <Shell>
      <Routes>
        <Route index element={<Dashboard />} />
        <Route path="c/:res" element={<ResourceList />} />
        <Route path="c/:res/new" element={<ResourceForm />} />
        <Route path="c/:res/:id" element={<ResourceForm />} />
        <Route path="analytics" element={<Suspense fallback={<Loader2 className="mx-auto mt-20 h-6 w-6 animate-spin text-muted" />}><AnalyticsPage /></Suspense>} />
        <Route path="messages" element={<MessagesPage />} />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="settings/:group" element={<SettingsPage />} />
        <Route path="media" element={<MediaPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="*" element={<div className="p-10 text-center text-muted">الصفحة غير موجودة</div>} />
      </Routes>
    </Shell>
  )
}

function useUnread() {
  return useQuery({
    queryKey: ['admin', 'unread'],
    queryFn: async () => (await api.get<{ total: number }>('/admin/messages?status=unread&page=1')).total,
    refetchInterval: 60_000,
  })
}

function Shell({ children }: { children: ReactNode }) {
  useFieldFocus()
  const me = useMe().data!
  const schema = useSchema()
  const logout = useLogout()
  const unread = useUnread().data ?? 0
  const loc = useLocation()
  const [open, setOpen] = useState(false)
  useEffect(() => setOpen(false), [loc.pathname])

  const item = (to: string, label: string, Icon: LucideIcon, badge?: number, end?: boolean) => (
    <NavLink
      key={to}
      to={to}
      end={end}
      className={({ isActive }) =>
        cx(
          'flex items-center gap-3 rounded-xl px-3 py-2.5 text-[15px] font-semibold transition',
          isActive ? 'bg-white/12 text-white shadow-inner' : 'text-white/65 hover:bg-white/6 hover:text-white',
        )
      }
    >
      <Icon className="h-[18px] w-[18px] shrink-0" />
      <span className="flex-1 truncate">{label}</span>
      {!!badge && <span className="rounded-full bg-gold-400 px-2 py-0.5 text-xs font-bold text-deep-950">{badge}</span>}
    </NavLink>
  )

  const sidebar = (
    <div className="flex h-full flex-col bg-deep-950 text-white">
      <div className="flex items-center gap-3 px-5 py-5">
        <BrandLogo className="h-12 w-12" animate={false} />
        <div className="leading-tight">
          <div className="font-display font-extrabold">معهد السمو</div>
          <div className="text-xs text-white/50">لوحة التحكم</div>
        </div>
      </div>
      <nav className="flex-1 space-y-6 overflow-y-auto px-3 pb-6">
        <div className="space-y-1">
          {item('/admin', 'الرئيسية', LayoutDashboard, undefined, true)}
          {item('/admin/analytics', 'التحليلات', LineChart)}
          {item('/admin/messages', 'الرسائل والتسجيلات', Inbox, unread)}
        </div>
        <div>
          <div className="mb-2 px-3 text-xs font-bold tracking-wide text-white/35">المحتوى</div>
          <div className="space-y-1">
            {schema.data?.resources.map((r) => item(`/admin/c/${r.name}`, r.label, ICONS[r.icon] ?? FileText))}
            {item('/admin/media', 'مكتبة الصور', Images)}
          </div>
        </div>
        <div>
          <div className="mb-2 px-3 text-xs font-bold tracking-wide text-white/35">الإعدادات</div>
          <div className="space-y-1">
            {item('/admin/settings', 'إعدادات الموقع', Settings)}
            {me.role === 'admin' && item('/admin/users', 'المستخدمون', Users)}
            {item('/admin/profile', 'حسابي', UserCog)}
          </div>
        </div>
      </nav>
      <div className="border-t border-white/10 p-3">
        <div className="flex items-center gap-3 rounded-xl px-3 py-2">
          <span className="grid h-9 w-9 shrink-0 place-items-center rounded-full bg-gold-400 font-display font-bold text-deep-950">{me.name.trim()[0]}</span>
          <div className="min-w-0 flex-1 leading-tight">
            <div className="truncate text-sm font-bold">{me.name}</div>
            <div className="text-xs text-white/50">{me.role === 'admin' ? 'مدير' : 'محرر'}</div>
          </div>
          <button onClick={() => logout.mutate()} className="rounded-lg p-2 text-white/60 hover:bg-white/10 hover:text-white" title="تسجيل الخروج" aria-label="تسجيل الخروج">
            <LogOut className="h-4 w-4" />
          </button>
        </div>
      </div>
    </div>
  )

  return (
    <div className="min-h-dvh bg-bg">
      <title>لوحة التحكم | معهد السمو</title>
      <meta name="robots" content="noindex" />
      <aside className="fixed inset-y-0 right-0 z-40 hidden w-64 lg:block">{sidebar}</aside>
      <AnimatePresence>
        {open && (
          <>
            <motion.div className="fixed inset-0 z-40 bg-black/40 lg:hidden" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setOpen(false)} />
            <motion.aside
              className="fixed inset-y-0 right-0 z-50 w-72 lg:hidden"
              initial={{ x: '100%' }}
              animate={{ x: 0 }}
              exit={{ x: '100%' }}
              transition={{ type: 'spring', damping: 30, stiffness: 300 }}
            >
              <button onClick={() => setOpen(false)} className="absolute top-5 -left-12 grid h-10 w-10 place-items-center rounded-full bg-surface" aria-label="إغلاق"><X className="h-5 w-5" /></button>
              {sidebar}
            </motion.aside>
          </>
        )}
      </AnimatePresence>
      <div className="lg:pr-64">
        <header className="sticky top-0 z-30 flex items-center justify-between gap-3 border-b border-line bg-bg/85 px-4 py-3 backdrop-blur md:px-8">
          <button onClick={() => setOpen(true)} className="grid h-10 w-10 place-items-center rounded-xl bg-surface shadow-soft lg:hidden" aria-label="القائمة">
            <Menu className="h-5 w-5" />
          </button>
          <div className="flex min-w-0 flex-1 justify-start"><AdminSearch /></div>
          <ThemePicker />
          <a href="/?edit" target="_blank" rel="noopener" className="hidden items-center gap-2 rounded-full bg-deep-950 px-4 py-2 text-sm font-semibold text-white shadow-soft hover:bg-deep-900 md:flex" title="افتح الموقع واضغط على أي نص لتعديله">
            <PencilLine className="h-4 w-4 text-gold-300" /> تعديل الموقع مباشرة
          </a>
          <Link to="/" target="_blank" className="flex items-center gap-2 rounded-full bg-surface px-4 py-2 text-sm font-semibold text-brand-700 shadow-soft hover:bg-brand-50">
            عرض الموقع <ExternalLink className="h-4 w-4" />
          </Link>
        </header>
        <main className="px-4 py-6 md:px-8 md:py-8">
          <motion.div key={loc.pathname} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.25 }}>
            {children}
          </motion.div>
        </main>
      </div>
    </div>
  )
}

export function PageTitle({ title, text, actions }: { title: string; text?: string; actions?: ReactNode }) {
  return (
    <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 className="text-2xl font-extrabold md:text-3xl">{title}</h1>
        {text && <p className="mt-1 text-muted">{text}</p>}
      </div>
      {actions && <div className="flex flex-wrap gap-2">{actions}</div>}
    </div>
  )
}
