import { useQuery } from '@tanstack/react-query'
import { ArrowLeft, FilePlus2, GraduationCap, Inbox, Newspaper, Plus, Settings, UserPlus } from 'lucide-react'
import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { Skeleton } from '../components/ui'
import { api } from '../lib/api'
import { cx, fmtNum, timeAgo } from '../lib/format'
import { PROGRAM_TYPES, type ProgramType, type User } from '../lib/types'
import { PageTitle } from './AdminApp'

interface DashboardData {
  user: User
  counts: Record<string, number>
  messages: {
    unread: number
    total: number
    registrations: number
    daily: { date: string; count: number }[]
    latest: { id: number; kind: 'contact' | 'register'; name: string; phone: string; program: string | null; subject: string | null; is_read: number; created_at: string }[]
  }
  programsByType: Record<ProgramType, number>
  activity: { action: string; resource: string; label: string | null; created_at: string; user: string | null }[]
}

const ACTIONS: Record<string, string> = { create: 'أضاف', update: 'عدّل', delete: 'حذف', password: 'غيّر كلمة المرور' }
const RES: Record<string, string> = {
  programs: 'برنامج', posts: 'مقال', pages: 'صفحة', accreditations: 'اعتماد', partners: 'شريك',
  agreements: 'اتفاقية', testimonials: 'رأي', settings: 'إعدادات', users: 'مستخدم', media: 'صورة', messages: 'رسالة',
}

export default function Dashboard() {
  const { data, isLoading } = useQuery({ queryKey: ['admin', 'dashboard'], queryFn: () => api.get<DashboardData>('/admin/dashboard') })
  if (isLoading || !data) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-10 w-72" />
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">{Array.from({ length: 4 }, (_, i) => <Skeleton key={i} className="h-32" />)}</div>
        <Skeleton className="h-72" />
      </div>
    )
  }
  const hour = new Date().getHours()
  const greet = hour < 12 ? 'صباح الخير' : 'مساء الخير'
  const tiles = [
    { label: 'رسائل غير مقروءة', value: data.messages.unread, sub: `من ${fmtNum(data.messages.total)} رسالة`, icon: Inbox, to: '/admin/messages', tone: 'bg-gold-400/15 text-gold-600' },
    { label: 'طلبات التسجيل', value: data.messages.registrations, sub: 'إجمالي الطلبات', icon: UserPlus, to: '/admin/messages', tone: 'bg-brand-100 text-brand-700' },
    { label: 'البرامج', value: data.counts.programs ?? 0, sub: `${data.programsByType.diploma} دبلوم · ${data.programsByType.qualifying + data.programsByType.development} دورة`, icon: GraduationCap, to: '/admin/c/programs', tone: 'bg-brand-100 text-brand-700' },
    { label: 'المقالات', value: data.counts.posts ?? 0, sub: 'في المدونة', icon: Newspaper, to: '/admin/c/posts', tone: 'bg-sand text-ink-2' },
  ]

  return (
    <>
      <PageTitle
        title={`${greet}، ${data.user.name.split(' ')[0]}`}
        text="نظرة سريعة على الموقع اليوم"
        actions={
          <>
            <Link to="/admin/c/programs/new" className="btn-primary !px-4 !py-2.5 text-sm"><Plus className="h-4 w-4" /> برنامج جديد</Link>
            <Link to="/admin/c/posts/new" className="btn-ghost !px-4 !py-2.5 text-sm"><FilePlus2 className="h-4 w-4" /> مقال جديد</Link>
          </>
        }
      />

      <div className="grid grid-cols-2 gap-3 md:gap-4 xl:grid-cols-4">
        {tiles.map((t) => (
          <Link key={t.label} to={t.to} className="card group p-4 transition hover:shadow-lift md:p-5">
            <div className="flex items-start justify-between">
              <span className={cx('grid h-11 w-11 place-items-center rounded-xl', t.tone)}><t.icon className="h-5 w-5" /></span>
              <ArrowLeft className="h-4 w-4 text-muted opacity-0 transition group-hover:opacity-100" />
            </div>
            <div className="mt-3 font-display text-2xl font-black text-ink tabular-nums md:mt-4 md:text-3xl">{fmtNum(t.value)}</div>
            <div className="mt-1 font-semibold text-ink-2">{t.label}</div>
            <div className="mt-0.5 truncate text-xs text-muted md:text-sm">{t.sub}</div>
          </Link>
        ))}
      </div>

      <div className="mt-6 grid gap-6 xl:grid-cols-[1.6fr_1fr]">
        <div className="card p-6">
          <MessagesChart daily={data.messages.daily} />
        </div>
        <div className="card p-6">
          <h2 className="mb-4 text-lg font-bold">آخر النشاطات</h2>
          {data.activity.length ? (
            <ul className="space-y-4">
              {data.activity.map((a, i) => (
                <li key={i} className="flex gap-3">
                  <span className={cx('mt-1.5 h-2 w-2 shrink-0 rounded-full', a.action === 'delete' ? 'bg-red-500' : a.action === 'create' ? 'bg-brand-500' : 'bg-gold-500')} />
                  <div className="min-w-0 text-sm">
                    <p className="text-ink-2">
                      <b className="text-ink">{a.user ?? 'النظام'}</b> {ACTIONS[a.action] ?? a.action} {RES[a.resource] ?? a.resource}
                      {a.label && <> «<span className="font-semibold">{a.label}</span>»</>}
                    </p>
                    <p className="mt-0.5 text-xs text-muted">{timeAgo(a.created_at)}</p>
                  </div>
                </li>
              ))}
            </ul>
          ) : (
            <p className="py-8 text-center text-sm text-muted">لا توجد نشاطات بعد</p>
          )}
        </div>
      </div>

      <div className="mt-6 grid gap-6 xl:grid-cols-[1.6fr_1fr]">
        <div className="card overflow-hidden">
          <div className="flex items-center justify-between border-b border-line px-6 py-4">
            <h2 className="text-lg font-bold">أحدث الرسائل</h2>
            <Link to="/admin/messages" className="text-sm font-semibold text-brand-700 hover:underline">عرض الكل</Link>
          </div>
          {data.messages.latest.length ? (
            <ul className="divide-y divide-line">
              {data.messages.latest.map((m) => (
                <li key={m.id}>
                  <Link to={`/admin/messages?open=${m.id}`} className="flex items-center gap-4 px-6 py-3.5 transition hover:bg-sand/50">
                    <span className="grid h-10 w-10 shrink-0 place-items-center rounded-full bg-brand-50 font-display font-bold text-brand-700">{m.name.trim()[0]}</span>
                    <div className="min-w-0 flex-1">
                      <div className="flex items-center gap-2">
                        <span className={cx('truncate', !m.is_read ? 'font-bold' : 'font-medium')}>{m.name}</span>
                        {!m.is_read && <span className="h-2 w-2 rounded-full bg-gold-500" aria-label="غير مقروءة" />}
                      </div>
                      <div className="truncate text-sm text-muted">{m.kind === 'register' ? `طلب تسجيل${m.program ? `: ${m.program}` : ''}` : m.subject || 'رسالة تواصل'}</div>
                    </div>
                    <span className="shrink-0 text-xs text-muted">{timeAgo(m.created_at)}</span>
                  </Link>
                </li>
              ))}
            </ul>
          ) : (
            <p className="py-12 text-center text-muted">لا توجد رسائل بعد</p>
          )}
        </div>
        <div className="card p-6">
          <h2 className="mb-4 text-lg font-bold">اختصارات</h2>
          <div className="grid grid-cols-2 gap-3">
            {[
              { to: '/admin/settings/hero', label: 'الواجهة الرئيسية', icon: Settings },
              { to: '/admin/settings/contact', label: 'بيانات التواصل', icon: Settings },
              { to: '/admin/c/testimonials/new', label: 'إضافة رأي', icon: Plus },
              { to: '/admin/c/partners/new', label: 'إضافة شريك', icon: Plus },
            ].map((s) => (
              <Link key={s.to} to={s.to} className="flex flex-col items-start gap-3 rounded-2xl border border-line p-4 text-sm font-semibold transition hover:border-brand-300 hover:bg-brand-50">
                <s.icon className="h-5 w-5 text-brand-600" />
                {s.label}
              </Link>
            ))}
          </div>
          <div className="mt-6 space-y-2">
            {(Object.keys(PROGRAM_TYPES) as ProgramType[]).map((t) => {
              const total = Math.max(1, data.programsByType.diploma + data.programsByType.qualifying + data.programsByType.development)
              const n = data.programsByType[t]
              return (
                <div key={t}>
                  <div className="mb-1 flex justify-between text-sm"><span className="text-ink-2">{PROGRAM_TYPES[t].plural}</span><span className="font-bold tabular-nums">{n}</span></div>
                  <div className="h-2 overflow-hidden rounded-full bg-sand"><div className="h-full rounded-full bg-brand-500" style={{ width: `${(n / total) * 100}%` }} /></div>
                </div>
              )
            })}
          </div>
        </div>
      </div>
    </>
  )
}

/** Single-series bar chart: messages per day, last 30 days. */
function MessagesChart({ daily }: { daily: { date: string; count: number }[] }) {
  const days = useMemo(() => {
    const map = new Map(daily.map((d) => [d.date, d.count]))
    const out: { date: string; count: number }[] = []
    const now = new Date()
    for (let i = 29; i >= 0; i--) {
      const d = new Date(now.getFullYear(), now.getMonth(), now.getDate() - i)
      const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
      out.push({ date: key, count: map.get(key) ?? 0 })
    }
    return out
  }, [daily])
  const [hover, setHover] = useState<number | null>(null)
  const max = Math.max(4, ...days.map((d) => d.count))
  const total = days.reduce((s, d) => s + d.count, 0)
  const fmt = new Intl.DateTimeFormat('ar-SA-u-ca-gregory-nu-latn', { day: 'numeric', month: 'short' })
  const ticks = [0, Math.round(max / 2), max]

  return (
    <div>
      <div className="mb-5 flex items-end justify-between">
        <div>
          <h2 className="text-lg font-bold">الرسائل اليومية</h2>
          <p className="text-sm text-muted">آخر 30 يومًا</p>
        </div>
        <div className="text-left">
          <div className="font-display text-2xl font-black tabular-nums">{fmtNum(total)}</div>
          <div className="text-xs text-muted">رسالة وطلب</div>
        </div>
      </div>
      <div className="relative flex gap-3">
        <div className="flex h-48 flex-col justify-between text-xs text-muted tabular-nums">
          {[...ticks].reverse().map((t) => <span key={t}>{t}</span>)}
        </div>
        <div className="relative h-48 flex-1">
          {ticks.map((t) => (
            <div key={t} className="absolute inset-x-0 border-t border-line/70" style={{ bottom: `${(t / max) * 100}%` }} />
          ))}
          {/* dir=ltr so time runs left->right like every calendar axis */}
          <div className="absolute inset-0 flex items-end gap-[2px]" dir="ltr" onMouseLeave={() => setHover(null)}>
            {days.map((d, i) => (
              <div key={d.date} className="relative flex h-full flex-1 items-end" onMouseEnter={() => setHover(i)}>
                <div
                  className={cx('w-full rounded-t-[4px] transition-colors', hover === i ? 'bg-primary' : 'bg-brand-500')}
                  style={{ height: d.count ? `${(d.count / max) * 100}%` : '2px', opacity: d.count ? 1 : 0.25 }}
                />
                {hover === i && (
                  <div className="pointer-events-none absolute bottom-full left-1/2 z-10 mb-2 -translate-x-1/2 rounded-lg bg-ink px-2.5 py-1.5 text-center text-xs whitespace-nowrap text-white shadow-lift" dir="rtl">
                    <div className="font-bold">{d.count} رسالة</div>
                    <div className="text-white/70">{fmt.format(new Date(`${d.date}T00:00:00`))}</div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
      <div className="mt-2 flex justify-between ps-8 text-xs text-muted" dir="ltr">
        <span>{fmt.format(new Date(`${days[0].date}T00:00:00`))}</span>
        <span>{fmt.format(new Date(`${days[29].date}T00:00:00`))}</span>
      </div>
    </div>
  )
}
