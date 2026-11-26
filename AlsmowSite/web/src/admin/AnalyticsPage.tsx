import { useQuery } from '@tanstack/react-query'
import {
  Activity, ArrowDownRight, ArrowUpRight, CalendarRange, Clock, Eye, Filter, Globe2, Layers, LogIn, MousePointerClick,
  RefreshCw, Repeat, Smartphone, Table2, UserPlus, Users,
} from 'lucide-react'
import { motion } from 'motion/react'
import { useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { api, qs } from '../lib/api'
import { cx } from '../lib/format'
import { PageTitle } from './AdminApp'

/* =====================================================================
   التحليلات: self-hosted visit analytics (api/lib/analytics.php).
   Charts are hand-built SVG/HTML following the data-viz method:
   categorical slots in fixed order (--viz-1 visitors, --viz-2 views, --viz-3 third),
   one sequential hue for magnitude (heat map), thin marks, hover tooltips, a table view.
   ===================================================================== */

type Row = { k: string; n: number; d?: number; title?: string }
type Totals = { visitors: number; views: number; sessions: number; bounce: number; perSession: number; avgDuration: number; newShare: number }
interface Report {
  range: { from: string; to: string; days: number; hourly: boolean; prevFrom: string; prevTo: string }
  totals: Totals
  prev: Totals
  series: { t: string; views: number; visitors: number }[]
  live: { total: number; pages: Row[] }
  pages: Row[]
  entries: Row[]
  sources: Row[]
  referrers: Row[]
  campaigns: Row[]
  devices: Row[]
  browsers: Row[]
  os: Row[]
  langs: Row[]
  countries: Row[]
  events: Row[]
  heat: number[][]
  funnel: { sessions: number; program: number; register: number; submitted: number; registrations: number }
  chat: { unanswered: number }
}

const n = (v: number) => v.toLocaleString('ar-EG')
const pct = (v: number) => `${v.toLocaleString('ar-EG', { maximumFractionDigits: 1 })}٪`
function dur(s: number) {
  if (!s) return '٠ث'
  const m = Math.floor(s / 60)
  const r = s % 60
  return m ? `${n(m)}د ${n(r)}ث` : `${n(r)}ث`
}
const iso = (d: Date) => new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const daysAgo = (k: number) => { const d = new Date(); d.setDate(d.getDate() - k); return iso(d) }

const SOURCE: Record<string, string> = { direct: 'زيارات مباشرة', search: 'محركات البحث', social: 'التواصل الاجتماعي', referral: 'مواقع أخرى', campaign: 'حملات (UTM)' }
const DEVICE: Record<string, string> = { mobile: 'الجوال', desktop: 'الكمبيوتر', tablet: 'التابلت' }
const EVENT: Record<string, string> = {
  chat_open: 'فتح المساعد الذكي', chat_message: 'أسئلة للمساعد', register_click: 'نقرات «سجّل الآن»', register_submit: 'طلبات تسجيل مُرسلة',
  contact_submit: 'رسائل تواصل', whatsapp_click: 'نقرات واتساب', phone_click: 'نقرات الاتصال', email_click: 'نقرات البريد', portal_click: 'دخول البوابة',
  program_view: 'مشاهدة صفحة برنامج', theme_change: 'تغيير ألوان الموقع', share: 'مشاركة',
}
const DAYS = ['الأحد', 'الاثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت']
const PRESETS = [
  { id: 'today', label: 'اليوم', from: () => daysAgo(0) },
  { id: '7', label: '٧ أيام', from: () => daysAgo(6) },
  { id: '30', label: '٣٠ يومًا', from: () => daysAgo(29) },
  { id: '90', label: '٩٠ يومًا', from: () => daysAgo(89) },
  { id: '365', label: 'سنة', from: () => daysAgo(364) },
] as const

export default function AnalyticsPage() {
  const [preset, setPreset] = useState<string>('30')
  const [from, setFrom] = useState(daysAgo(29))
  const [to, setTo] = useState(daysAgo(0))
  const q = useQuery({
    queryKey: ['admin', 'analytics', from, to],
    queryFn: () => api.get<Report>(`/admin/analytics${qs({ from, to })}`),
    refetchInterval: 30_000, // live visitors
    placeholderData: (p) => p,
  })
  const r = q.data
  const pick = (id: string) => { const p = PRESETS.find((x) => x.id === id)!; setPreset(id); setFrom(p.from()); setTo(daysAgo(0)) }

  return (
    <div className="viz space-y-6">
      <PageTitle title="التحليلات" text="زيارات الموقع ومصادرها وسلوك الزوار، بدون ملفات تعريف ارتباط وبدون جهات خارجية" />

      {/* filters: one row above the charts */}
      <div className="card flex flex-wrap items-center gap-2 p-3">
        <CalendarRange className="ms-1 h-5 w-5 text-muted" />
        {PRESETS.map((p) => (
          <button key={p.id} onClick={() => pick(p.id)} className={cx('rounded-full px-3.5 py-1.5 text-sm font-semibold transition', preset === p.id ? 'bg-primary text-white' : 'bg-bg text-ink-2 hover:bg-sand')}>{p.label}</button>
        ))}
        <span className="mx-1 h-6 w-px bg-line" />
        <label className="flex items-center gap-1.5 text-sm text-muted">من <input type="date" className="input !h-9 !w-auto !py-1 text-sm" value={from} max={to} onChange={(e) => { setFrom(e.target.value); setPreset('') }} /></label>
        <label className="flex items-center gap-1.5 text-sm text-muted">إلى <input type="date" className="input !h-9 !w-auto !py-1 text-sm" value={to} min={from} max={daysAgo(0)} onChange={(e) => { setTo(e.target.value); setPreset('') }} /></label>
        <span className="flex-1" />
        {r && <span className="text-xs text-muted">مقارنة بـ {r.range.prevFrom} ← {r.range.prevTo}</span>}
        <button onClick={() => q.refetch()} className="grid h-9 w-9 place-items-center rounded-xl text-muted hover:bg-bg hover:text-ink" title="تحديث" aria-label="تحديث"><RefreshCw className={cx('h-4 w-4', q.isFetching && 'animate-spin')} /></button>
      </div>

      {q.isError && <div className="card p-6 text-center text-red-600">{(q.error as Error).message}</div>}
      {!r ? <SkeletonGrid /> : (
        <>
          <div className="grid grid-cols-2 gap-3 md:gap-4 xl:grid-cols-4">
            <Live live={r.live} />
            <Kpi icon={Users} label="الزوار" value={r.totals.visitors} prev={r.prev.visitors} fmt={n} />
            <Kpi icon={Eye} label="مشاهدات الصفحات" value={r.totals.views} prev={r.prev.views} fmt={n} />
            <Kpi icon={Layers} label="الجلسات" value={r.totals.sessions} prev={r.prev.sessions} fmt={n} />
            <Kpi icon={Clock} label="متوسط مدة الجلسة" value={r.totals.avgDuration} prev={r.prev.avgDuration} fmt={dur} />
            <Kpi icon={LogIn} label="معدل الارتداد" value={r.totals.bounce} prev={r.prev.bounce} fmt={pct} lowerIsBetter hint="جلسات بصفحة واحدة فقط" />
            <Kpi icon={Repeat} label="صفحات لكل جلسة" value={r.totals.perSession} prev={r.prev.perSession} fmt={(v) => v.toLocaleString('ar-EG', { maximumFractionDigits: 2 })} />
            <Kpi icon={UserPlus} label="زوار جدد" value={r.totals.newShare} prev={r.prev.newShare} fmt={pct} neutral hint="نسبة الجلسات لزوار يدخلون لأول مرة" />
          </div>

          <Panel title="الزوار والمشاهدات" icon={Activity} sub={r.range.hourly ? 'بالساعة' : 'باليوم'} wide>
            <TrendChart data={r.series} hourly={r.range.hourly} />
          </Panel>

          <div className="grid gap-6 xl:grid-cols-2">
            <Panel title="الصفحات الأكثر زيارة" icon={Eye} sub="المشاهدات · متوسط وقت القراءة">
              <Bars rows={r.pages.map((p) => ({ ...p, label: p.title || p.k, note: p.d ? dur(p.d) : undefined, sub: p.title ? p.k : undefined }))} />
            </Panel>
            <Panel title="مصادر الزيارات" icon={Globe2} sub="من أين جاء الزوار">
              <Bars rows={r.sources.map((s) => ({ ...s, label: SOURCE[s.k] ?? s.k }))} />
              {r.referrers.length > 0 && <><Sub>المواقع المحيلة</Sub><Bars rows={r.referrers.map((s) => ({ ...s, label: s.k }))} /></>}
              {r.campaigns.length > 0 && <><Sub>الحملات</Sub><Bars rows={r.campaigns.map((s) => ({ ...s, label: s.k }))} /></>}
            </Panel>
            <Panel title="الأجهزة" icon={Smartphone}>
              <Segmented rows={r.devices.map((d) => ({ k: d.k, label: DEVICE[d.k] ?? d.k, n: d.n }))} />
              <div className="mt-6 grid gap-6 sm:grid-cols-2">
                <div><Sub first>المتصفحات</Sub><Bars rows={r.browsers.map((s) => ({ ...s, label: s.k }))} compact /></div>
                <div><Sub first>أنظمة التشغيل</Sub><Bars rows={r.os.map((s) => ({ ...s, label: s.k }))} compact /></div>
              </div>
            </Panel>
            <Panel title="رحلة التسجيل" icon={Filter} sub="من الزيارة إلى طلب التسجيل">
              <Funnel f={r.funnel} />
            </Panel>
            <Panel title="أوقات الذروة" icon={Clock} sub="المشاهدات حسب اليوم والساعة (بتوقيت السعودية)" wide>
              <Heat grid={r.heat} />
            </Panel>
            <Panel title="تفاعل الزوار" icon={MousePointerClick} sub="نقرات وأحداث مهمة">
              <Bars rows={r.events.map((e) => ({ ...e, label: EVENT[e.k] ?? e.k }))} empty="لا توجد أحداث في هذه الفترة" />
              {r.chat.unanswered > 0 && <p className="mt-3 rounded-xl bg-gold-400/10 px-3 py-2 text-sm text-ink-2">المساعد الذكي لديه <b>{n(r.chat.unanswered)}</b> سؤالًا بلا إجابة: أضيفي إجاباتها من «إعدادات الموقع ← المساعد الذكي».</p>}
            </Panel>
            <Panel title="صفحات الدخول" icon={LogIn} sub="أول صفحة يفتحها الزائر">
              <Bars rows={r.entries.map((p) => ({ ...p, label: p.k }))} />
              {(r.langs.length > 0 || r.countries.length > 0) && (
                <div className="mt-6 grid gap-6 sm:grid-cols-2">
                  {r.langs.length > 0 && <div><Sub first>لغة المتصفح</Sub><Bars rows={r.langs.map((s) => ({ ...s, label: s.k }))} compact /></div>}
                  {r.countries.length > 0 && <div><Sub first>الدول</Sub><Bars rows={r.countries.map((s) => ({ ...s, label: s.k }))} compact /></div>}
                </div>
              )}
            </Panel>
          </div>
        </>
      )}
    </div>
  )
}

/* ---------------- building blocks ---------------- */

function SkeletonGrid() {
  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {Array.from({ length: 8 }).map((_, i) => <div key={i} className="card h-32 animate-pulse bg-surface" />)}
    </div>
  )
}

function Panel({ title, icon: Icon, sub, children, wide }: { title: string; icon: typeof Users; sub?: string; children: ReactNode; wide?: boolean }) {
  return (
    <motion.section initial={{ opacity: 0, y: 12 }} whileInView={{ opacity: 1, y: 0 }} viewport={{ once: true, margin: '-40px' }} transition={{ duration: 0.4 }} className={cx('card p-5 md:p-6', wide && 'xl:col-span-2')}>
      <header className="mb-4 flex flex-wrap items-baseline gap-x-3 gap-y-1">
        <h2 className="flex items-center gap-2 text-base font-extrabold"><Icon className="h-[18px] w-[18px] text-muted" />{title}</h2>
        {sub && <span className="text-xs text-muted">{sub}</span>}
      </header>
      {children}
    </motion.section>
  )
}

const Sub = ({ children, first }: { children: ReactNode; first?: boolean }) => <h3 className={cx('mb-2 text-xs font-bold tracking-wide text-muted', !first && 'mt-6')}>{children}</h3>

function Kpi({ icon: Icon, label, value, prev, fmt, lowerIsBetter, neutral, hint }: {
  icon: typeof Users; label: string; value: number; prev: number; fmt: (v: number) => string; lowerIsBetter?: boolean; neutral?: boolean; hint?: string
}) {
  const change = prev > 0 ? ((value - prev) / prev) * 100 : value > 0 ? 100 : 0
  const up = change > 0
  const good = neutral ? null : lowerIsBetter ? !up : up
  return (
    <div className="card p-4 md:p-5" title={hint}>
      <div className="flex items-center gap-2 text-sm font-semibold text-muted"><Icon className="h-4 w-4" />{label}</div>
      <div className="mt-2 font-display text-[1.55rem] leading-none font-black text-ink tabular-nums md:text-[2rem]">{fmt(value)}</div>
      <div className="mt-3 flex items-center gap-1.5 text-xs">
        {Math.abs(change) < 0.05 ? (
          <span className="text-muted">بلا تغيير عن الفترة السابقة</span>
        ) : (
          <>
            <span className={cx('inline-flex items-center gap-0.5 rounded-full px-1.5 py-0.5 font-bold', good === null ? 'bg-bg text-ink-2' : good ? 'viz-good' : 'viz-bad')}>
              {up ? <ArrowUpRight className="h-3.5 w-3.5" /> : <ArrowDownRight className="h-3.5 w-3.5" />}
              {pct(Math.abs(change))}
            </span>
            <span className="hidden text-muted sm:inline">{up ? 'ارتفاع' : 'انخفاض'} عن الفترة السابقة ({fmt(prev)})</span>
          </>
        )}
      </div>
    </div>
  )
}

function Live({ live }: { live: Report['live'] }) {
  return (
    <div className="card relative col-span-2 overflow-hidden p-4 sm:col-span-1 md:p-5">
      <div className="flex items-center gap-2 text-sm font-semibold text-muted">
        <span className="relative flex h-2.5 w-2.5"><span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-emerald-400 opacity-60" /><span className="relative inline-flex h-2.5 w-2.5 rounded-full bg-emerald-500" /></span>
        على الموقع الآن
      </div>
      <div className="mt-2 font-display text-[2rem] leading-none font-black text-ink tabular-nums">{n(live.total)}</div>
      <div className="mt-3 space-y-1 text-xs text-muted">
        {live.pages.length ? live.pages.slice(0, 3).map((p) => (
          <div key={p.k} className="flex justify-between gap-2"><span className="truncate" dir="ltr">{p.k}</span><span className="tabular-nums">{n(p.n)}</span></div>
        )) : <span>آخر ٥ دقائق · يتحدث تلقائيًا</span>}
      </div>
    </div>
  )
}

/** Horizontal bars, one series (slot 1). Value text in ink, never the series color. */
function Bars({ rows, compact, empty = 'لا توجد بيانات بعد' }: { rows: (Row & { label: string; note?: string; sub?: string })[]; compact?: boolean; empty?: string }) {
  const max = Math.max(1, ...rows.map((r) => r.n))
  const total = rows.reduce((s, r) => s + r.n, 0) || 1
  if (!rows.length) return <p className="py-6 text-center text-sm text-muted">{empty}</p>
  return (
    <ul className={cx(compact ? 'space-y-2' : 'space-y-3')}>
      {rows.map((r, i) => (
        <motion.li key={r.k + i} initial={{ opacity: 0 }} whileInView={{ opacity: 1 }} viewport={{ once: true }} transition={{ delay: i * 0.03 }} className="group" title={`${r.label}: ${n(r.n)} (${pct((r.n / total) * 100)})`}>
          <div className="mb-1 flex items-baseline gap-2 text-sm">
            <span className="min-w-0 flex-1 truncate font-semibold text-ink-2">{r.label}{r.sub && <span className="ms-2 text-xs font-normal text-muted" dir="ltr">{r.sub}</span>}</span>
            {r.note && <span className="shrink-0 text-xs text-muted">{r.note}</span>}
            <span className="shrink-0 font-bold text-ink tabular-nums">{n(r.n)}</span>
            <span className="w-11 shrink-0 text-left text-xs text-muted tabular-nums">{pct((r.n / total) * 100)}</span>
          </div>
          <div className="viz-track">
            <motion.div className="viz-bar" initial={{ width: 0 }} whileInView={{ width: `${(r.n / max) * 100}%` }} viewport={{ once: true }} transition={{ duration: 0.7, delay: i * 0.03, ease: [0.22, 1, 0.36, 1] }} />
          </div>
        </motion.li>
      ))}
    </ul>
  )
}

/** Part-to-whole in one thin segmented bar (≤3 parts, slots 1–3), legend + direct values. */
function Segmented({ rows }: { rows: { k: string; label: string; n: number }[] }) {
  const total = rows.reduce((s, r) => s + r.n, 0)
  if (!total) return <p className="py-6 text-center text-sm text-muted">لا توجد بيانات بعد</p>
  return (
    <div>
      <div className="flex h-7 gap-[2px] overflow-hidden rounded-[4px]">
        {rows.map((r, i) => (
          <motion.div key={r.k} title={`${r.label}: ${n(r.n)}`} initial={{ flexGrow: 0 }} whileInView={{ flexGrow: r.n }} viewport={{ once: true }} transition={{ duration: 0.8 }} className={`viz-seg-${i + 1}`} style={{ flexBasis: 0 }} />
        ))}
      </div>
      <ul className="mt-3 flex flex-wrap gap-x-5 gap-y-2">
        {rows.map((r, i) => (
          <li key={r.k} className="flex items-center gap-2 text-sm">
            <span className={`h-3 w-3 rounded-[3px] viz-seg-${i + 1}`} />
            <span className="font-semibold text-ink-2">{r.label}</span>
            <span className="font-bold text-ink tabular-nums">{pct((r.n / total) * 100)}</span>
            <span className="text-xs text-muted tabular-nums">({n(r.n)})</span>
          </li>
        ))}
      </ul>
    </div>
  )
}

/** Two series on one scale (both counts): visitors (slot 1) and page views (slot 2); crosshair tooltip; table view. */
function TrendChart({ data, hourly }: { data: Report['series']; hourly: boolean }) {
  const box = useRef<HTMLDivElement>(null)
  const [w, setW] = useState(800)
  const [hover, setHover] = useState<number | null>(null)
  const [table, setTable] = useState(false)
  useEffect(() => {
    const el = box.current
    if (!el) return
    const ro = new ResizeObserver(() => setW(el.clientWidth))
    ro.observe(el)
    setW(el.clientWidth)
    return () => ro.disconnect()
  }, [])
  const H = 260
  const pad = { l: 40, r: 14, t: 14, b: 30 }
  const max = Math.max(4, ...data.map((d) => Math.max(d.views, d.visitors)))
  const nice = niceMax(max)
  const x = (i: number) => pad.l + (data.length <= 1 ? 0 : (i / (data.length - 1)) * (w - pad.l - pad.r))
  const y = (v: number) => pad.t + (1 - v / nice) * (H - pad.t - pad.b)
  const path = (key: 'views' | 'visitors') => data.map((d, i) => `${i ? 'L' : 'M'}${x(i).toFixed(1)},${y(d[key]).toFixed(1)}`).join('')
  const area = `${path('visitors')}L${x(data.length - 1)},${y(0)}L${x(0)},${y(0)}Z`
  const ticks = [0, 0.25, 0.5, 0.75, 1].map((f) => Math.round(nice * f))
  const every = Math.max(1, Math.ceil(data.length / Math.max(2, Math.floor(w / 90))))
  const label = (t: string) => (hourly ? t.slice(11, 16) : `${+t.slice(8, 10)}/${+t.slice(5, 7)}`)
  const totalV = data.reduce((s, d) => s + d.views, 0)

  const onMove = (e: React.PointerEvent) => {
    const rect = (e.currentTarget as SVGElement).getBoundingClientRect()
    const px = e.clientX - rect.left
    const i = Math.round(((px - pad.l) / (w - pad.l - pad.r)) * (data.length - 1))
    setHover(Math.max(0, Math.min(data.length - 1, i)))
  }

  return (
    <div>
      <div className="mb-3 flex flex-wrap items-center gap-4 text-sm">
        <span className="flex items-center gap-2"><span className="h-[3px] w-5 rounded-full viz-seg-1" /><span className="font-semibold text-ink-2">الزوار</span></span>
        <span className="flex items-center gap-2"><span className="h-[3px] w-5 rounded-full viz-seg-2" /><span className="font-semibold text-ink-2">المشاهدات</span></span>
        <span className="flex-1" />
        <button onClick={() => setTable(!table)} className="flex items-center gap-1.5 rounded-lg px-2 py-1 text-xs font-semibold text-muted hover:bg-bg hover:text-ink"><Table2 className="h-4 w-4" />{table ? 'عرض كرسم' : 'عرض كجدول'}</button>
      </div>
      {table ? (
        <div className="max-h-80 overflow-auto rounded-xl border border-line">
          <table className="w-full text-sm">
            <thead className="sticky top-0 bg-bg text-muted"><tr><th className="p-2 text-right">{hourly ? 'الساعة' : 'اليوم'}</th><th className="p-2 text-right">الزوار</th><th className="p-2 text-right">المشاهدات</th></tr></thead>
            <tbody>{[...data].reverse().map((d) => <tr key={d.t} className="border-t border-line"><td className="p-2" dir="ltr">{d.t}</td><td className="p-2 tabular-nums">{n(d.visitors)}</td><td className="p-2 tabular-nums">{n(d.views)}</td></tr>)}</tbody>
          </table>
        </div>
      ) : (
        <div ref={box} className="relative" dir="ltr">
          {totalV === 0 && <div className="pointer-events-none absolute inset-0 z-10 grid place-items-center text-sm text-muted">لا توجد زيارات في هذه الفترة بعد</div>}
          <svg width={w} height={H} className="block touch-none select-none" onPointerMove={onMove} onPointerLeave={() => setHover(null)} role="img" aria-label="الزوار والمشاهدات عبر الزمن">
            {ticks.map((t) => (
              <g key={t}>
                <line x1={pad.l} x2={w - pad.r} y1={y(t)} y2={y(t)} className="viz-grid" />
                <text x={pad.l - 8} y={y(t) + 4} textAnchor="end" className="viz-axis">{t.toLocaleString('ar-EG')}</text>
              </g>
            ))}
            {data.map((d, i) => i % every === 0 && <text key={d.t} x={x(i)} y={H - 8} textAnchor="middle" className="viz-axis">{label(d.t)}</text>)}
            <motion.path d={area} className="viz-area-1" initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.6 }} />
            <motion.path d={path('views')} className="viz-line-2" initial={{ pathLength: 0 }} animate={{ pathLength: 1 }} transition={{ duration: 1, ease: 'easeOut' }} />
            <motion.path d={path('visitors')} className="viz-line-1" initial={{ pathLength: 0 }} animate={{ pathLength: 1 }} transition={{ duration: 1, ease: 'easeOut' }} />
            {hover !== null && (
              <g>
                <line x1={x(hover)} x2={x(hover)} y1={pad.t} y2={H - pad.b} className="viz-cross" />
                <circle cx={x(hover)} cy={y(data[hover].views)} r={4.5} className="viz-dot-2" />
                <circle cx={x(hover)} cy={y(data[hover].visitors)} r={4.5} className="viz-dot-1" />
              </g>
            )}
          </svg>
          {hover !== null && (
            <div className="viz-tip" style={{ left: Math.min(Math.max(x(hover), 70), w - 70), top: 6 }} dir="rtl">
              <div className="mb-1 text-xs text-muted" dir="ltr">{data[hover].t}</div>
              <div className="flex items-center gap-2"><span className="h-2.5 w-2.5 rounded-full viz-seg-1" />الزوار <b className="ms-auto tabular-nums">{n(data[hover].visitors)}</b></div>
              <div className="flex items-center gap-2"><span className="h-2.5 w-2.5 rounded-full viz-seg-2" />المشاهدات <b className="ms-auto tabular-nums">{n(data[hover].views)}</b></div>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

function niceMax(v: number) {
  const p = Math.pow(10, Math.floor(Math.log10(v)))
  for (const m of [1, 2, 2.5, 5, 10]) if (m * p >= v) return m * p
  return 10 * p
}

/** Weekday × hour; one hue light→dark (sequential). */
function Heat({ grid }: { grid: number[][] }) {
  const max = Math.max(1, ...grid.flat())
  const [tip, setTip] = useState<{ d: number; h: number } | null>(null)
  const empty = grid.flat().every((v) => v === 0)
  return (
    <div className="overflow-x-auto">
      <div className="min-w-[640px]">
        <div className="grid grid-cols-[72px_repeat(24,1fr)] gap-[2px]" dir="ltr">
          <span />
          {Array.from({ length: 24 }, (_, h) => <span key={h} className="pb-1 text-center text-[10px] text-muted tabular-nums">{h % 3 === 0 ? h : ''}</span>)}
          {grid.map((row, d) => (
            <FragmentRow key={d}>
              <span className="pe-2 text-right text-xs font-semibold text-muted" dir="rtl">{DAYS[d]}</span>
              {row.map((v, h) => (
                <span
                  key={h}
                  onPointerEnter={() => setTip({ d, h })}
                  onPointerLeave={() => setTip(null)}
                  className={cx('h-6 rounded-[4px] transition-transform hover:scale-110', tip?.d === d && tip?.h === h && 'ring-2 ring-ink/40')}
                  style={{ background: v ? `color-mix(in oklab, var(--viz-1) ${12 + (v / max) * 88}%, var(--surface))` : 'var(--viz-empty)' }}
                  title={`${DAYS[d]} ${h}:00 — ${v.toLocaleString('ar-EG')} مشاهدة`}
                />
              ))}
            </FragmentRow>
          ))}
        </div>
        <div className="mt-3 flex items-center justify-between text-xs text-muted">
          <span>{tip ? `${DAYS[tip.d]} الساعة ${tip.h}:00 — ${n(grid[tip.d][tip.h])} مشاهدة` : empty ? 'لا توجد زيارات بعد' : 'مرّري على أي خانة لمعرفة عدد المشاهدات'}</span>
          <span className="flex items-center gap-1.5">أقل<span className="h-3 w-16 rounded-[3px]" style={{ background: 'linear-gradient(90deg, color-mix(in oklab, var(--viz-1) 12%, var(--surface)), var(--viz-1))' }} />أكثر</span>
        </div>
      </div>
    </div>
  )
}
const FragmentRow = ({ children }: { children: ReactNode }) => <>{children}</>

function Funnel({ f }: { f: Report['funnel'] }) {
  const steps = [
    { k: 'sessions', label: 'زيارات (جلسات)', v: f.sessions },
    { k: 'program', label: 'شاهدت صفحة برنامج', v: f.program },
    { k: 'register', label: 'وصلت لصفحة التسجيل', v: f.register },
    { k: 'submitted', label: 'أرسلت طلب التسجيل', v: f.submitted },
  ]
  const top = Math.max(1, f.sessions)
  return (
    <div className="space-y-3">
      {steps.map((s, i) => {
        const prev = i ? steps[i - 1].v : s.v
        return (
          <div key={s.k}>
            <div className="mb-1 flex items-baseline gap-2 text-sm">
              <span className="flex-1 font-semibold text-ink-2">{s.label}</span>
              <span className="font-bold text-ink tabular-nums">{n(s.v)}</span>
              {i > 0 && <span className="w-24 text-left text-xs text-muted">{prev ? `${pct((s.v / prev) * 100)} من السابقة` : '—'}</span>}
            </div>
            <div className="viz-track"><motion.div className="viz-bar" initial={{ width: 0 }} whileInView={{ width: `${(s.v / top) * 100}%` }} viewport={{ once: true }} transition={{ duration: 0.8, delay: i * 0.1 }} /></div>
          </div>
        )
      })}
      <p className="pt-1 text-xs text-muted">طلبات التسجيل المستلمة في «الرسائل والتسجيلات» خلال الفترة: <b className="text-ink">{n(f.registrations)}</b></p>
    </div>
  )
}

export function useAnalyticsSummary() {
  const r = useQuery({ queryKey: ['admin', 'analytics', 'summary'], queryFn: () => api.get<Report>(`/admin/analytics${qs({ from: daysAgo(6), to: daysAgo(0) })}`), staleTime: 60_000 })
  return useMemo(() => r.data, [r.data])
}
