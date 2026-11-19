import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  BadgeCheck, BookOpen, Building2, CalendarDays, CreditCard, GraduationCap, IdCard, Layers, Loader2, LogOut, Phone, Receipt,
  RefreshCw, ShieldCheck, Wallet,
} from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { WhatsAppIcon } from '../../components/BrandIcons'
import { Aurora, Counter, Seo, SplitText, stagger } from '../../components/ui'
import BrandLogo from '../../components/BrandLogo'
import { ApiError, api } from '../../lib/api'
import { cx, fmtDate, fmtDateTime, waNumber } from '../../lib/format'
import { useSite } from '../../lib/queries'

interface PortalStudent {
  name: string
  code: string | null
  national_id: string
  branch: string | null
  programs: { title: string; kind: 'diploma' | 'course'; level: string | null; batch: string | null; branch: string | null; status: string | null; start_date: string | null; reg_code: string | null }[]
  finance: { total: number; discount: number; paid: number; balance: number }
  payments: { date: string | null; amount: number; ref: string | null; method: string | null; note: string | null }[]
  synced_at: string
}

const money = (n: number) => new Intl.NumberFormat('en-US', { maximumFractionDigits: 2 }).format(n)

export default function Portal() {
  const qc = useQueryClient()
  const me = useQuery({
    queryKey: ['portal'],
    queryFn: async () => {
      try {
        return (await api.get<{ student: PortalStudent }>('/portal/me')).student
      } catch (e) {
        if (e instanceof ApiError && e.status === 401) return null
        throw e
      }
    },
    retry: false,
  })
  const logout = useMutation({ mutationFn: () => api.post('/portal/logout'), onSettled: () => qc.setQueryData(['portal'], null) })

  return (
    <>
      <Seo title="بوابة المتدربات" description="تابعي برامجك وحسابك ومدفوعاتك في معهد السمو" />
      <meta name="robots" content="noindex" />
      <section className="relative min-h-[85vh] overflow-hidden pt-32 pb-20 md:pt-40">
        <Aurora />
        <div className="pattern absolute inset-0 [mask-image:radial-gradient(ellipse_at_top,black,transparent_70%)]" />
        <div className="container-x relative">
          {me.isLoading ? (
            <div className="grid place-items-center py-32"><Loader2 className="h-8 w-8 animate-spin text-brand-600" /></div>
          ) : (
            <AnimatePresence mode="wait">
              {me.data ? (
                <motion.div key="dash" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0 }}>
                  <Dashboard s={me.data} onLogout={() => logout.mutate()} onRefresh={() => me.refetch()} refreshing={me.isFetching} />
                </motion.div>
              ) : (
                <motion.div key="login" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -20 }}>
                  <Login onDone={(s) => qc.setQueryData(['portal'], s)} />
                </motion.div>
              )}
            </AnimatePresence>
          )}
        </div>
      </section>
    </>
  )
}

/* ---------------- Login ---------------- */

function Login({ onDone }: { onDone: (s: PortalStudent) => void }) {
  const [nid, setNid] = useState('')
  const [phone, setPhone] = useState('')
  const login = useMutation({
    mutationFn: () => api.post<{ student: PortalStudent }>('/portal/login', { national_id: nid, phone }),
    onSuccess: (r) => onDone(r.student),
  })
  const site = useSite()
  const wa = waNumber(site.data?.settings.contact.whatsapp || site.data?.settings.contact.phone)

  return (
    <div className="grid items-center gap-12 lg:grid-cols-2">
      <div>
        <span className="eyebrow"><ShieldCheck className="h-4 w-4" /> بوابة المتدربات</span>
        <SplitText as="h1" immediate text="أهلًا بكِ في بوابتك الخاصة" accent="بوابتك الخاصة" className="mt-6 text-4xl leading-tight font-black text-ink md:text-6xl" />
        <p className="mt-6 max-w-lg text-lg leading-relaxed text-muted">تابعي برنامجك ومستواك ودفعتك، واطّلعي على حسابك ومدفوعاتك لحظة بلحظة، مباشرة من نظام المعهد.</p>
        <motion.ul variants={stagger.container} initial="hidden" animate="show" className="mt-10 grid max-w-lg gap-4 sm:grid-cols-2">
          {[
            { icon: GraduationCap, t: 'برامجي ومستواي' },
            { icon: Wallet, t: 'الرسوم والمتبقي' },
            { icon: Receipt, t: 'سجل السدادات' },
            { icon: ShieldCheck, t: 'دخول آمن ومحمي' },
          ].map((x) => (
            <motion.li key={x.t} variants={stagger.item} className="flex items-center gap-3 rounded-2xl border border-line bg-surface/70 p-3 backdrop-blur">
              <span className="grid h-10 w-10 place-items-center rounded-xl bg-brand-50 text-brand-700"><x.icon className="h-5 w-5" /></span>
              <span className="font-semibold text-ink-2">{x.t}</span>
            </motion.li>
          ))}
        </motion.ul>
      </div>

      <motion.form
        initial={{ opacity: 0, y: 30, rotate: 1 }}
        animate={{ opacity: 1, y: 0, rotate: 0 }}
        transition={{ delay: 0.2, type: 'spring', damping: 22 }}
        onSubmit={(e) => { e.preventDefault(); login.mutate() }}
        className="relative mx-auto w-full max-w-md overflow-hidden rounded-[2rem] border border-line bg-surface p-8 shadow-lift md:p-10"
        noValidate
      >
        <div className="absolute -top-20 -left-20 h-48 w-48 rounded-full bg-brand-200/50 blur-3xl" />
        <div className="relative">
          <BrandLogo className="h-16 w-16" />
          <h2 className="mt-6 text-2xl font-extrabold">تسجيل الدخول</h2>
          <p className="mt-1 text-sm text-muted">استخدمي البيانات المسجلة لدى المعهد</p>

          <AnimatePresence>
            {login.error && (
              <motion.p initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }} className="mt-5 rounded-xl bg-red-500/10 px-4 py-3 text-sm font-semibold text-red-600 dark:text-red-300">
                {login.error.message}
              </motion.p>
            )}
          </AnimatePresence>

          <div className="mt-7 space-y-5">
            <div>
              <label className="label" htmlFor="p-nid">رقم الهوية / الإقامة</label>
              <div className="relative">
                <IdCard className="pointer-events-none absolute top-1/2 right-3.5 h-5 w-5 -translate-y-1/2 text-muted" />
                <input id="p-nid" value={nid} onChange={(e) => setNid(e.target.value)} inputMode="numeric" autoComplete="username" dir="ltr" maxLength={10} placeholder="1XXXXXXXXX" className="input !py-3.5 ps-11 text-left tracking-widest" />
              </div>
            </div>
            <div>
              <label className="label" htmlFor="p-phone">رقم الجوال المسجّل</label>
              <div className="relative">
                <Phone className="pointer-events-none absolute top-1/2 right-3.5 h-5 w-5 -translate-y-1/2 text-muted" />
                <input id="p-phone" value={phone} onChange={(e) => setPhone(e.target.value)} type="password" inputMode="tel" autoComplete="current-password" dir="ltr" placeholder="05XXXXXXXX" className="input !py-3.5 ps-11 text-left tracking-widest" />
              </div>
              <p className="mt-1.5 text-xs text-muted">رقم الجوال المسجّل لدى المعهد هو كلمة المرور</p>
            </div>
          </div>
          <button type="submit" disabled={login.isPending || nid.length < 8 || phone.length < 9} className="btn-primary mt-8 w-full !py-4 text-base">
            {login.isPending && <Loader2 className="h-5 w-5 animate-spin" />} دخول البوابة
          </button>
          <div className="mt-6 border-t border-line pt-5 text-center text-sm text-muted">
            لم تتمكني من الدخول؟{' '}
            {wa ? <a href={`https://wa.me/${wa}?text=${encodeURIComponent('السلام عليكم، لم أتمكن من الدخول إلى بوابة المتدربات')}`} target="_blank" rel="noopener noreferrer" className="font-bold text-brand-700 hover:underline">تواصلي معنا</a> : <Link to="/contact" className="font-bold text-brand-700 hover:underline">تواصلي معنا</Link>}
          </div>
        </div>
      </motion.form>
    </div>
  )
}

/* ---------------- Dashboard ---------------- */

function Dashboard({ s, onLogout, onRefresh, refreshing }: { s: PortalStudent; onLogout: () => void; onRefresh: () => void; refreshing: boolean }) {
  const f = s.finance
  const net = Math.max(0, f.total - f.discount)
  const pct = net > 0 ? Math.min(100, Math.round((f.paid / net) * 100)) : 0
  const first = s.name.split(' ')[0]

  return (
    <div>
      <div className="flex flex-col justify-between gap-6 md:flex-row md:items-end">
        <div className="flex items-center gap-5">
          <motion.span initial={{ scale: 0, rotate: -20 }} animate={{ scale: 1, rotate: 0 }} transition={{ type: 'spring' }} className="grid h-20 w-20 shrink-0 place-items-center rounded-[1.75rem] bg-gradient-to-br from-brand-400 to-deep-700 font-display text-3xl font-black text-white shadow-glow">
            {s.name.trim()[0]}
          </motion.span>
          <div>
            <p className="text-muted">مرحبًا بعودتك 👋</p>
            <SplitText as="h1" immediate text={s.name} accent={first} className="text-3xl font-black text-ink md:text-4xl" />
            <div className="mt-2 flex flex-wrap gap-2 text-sm">
              {s.code && <span className="chip bg-brand-50 text-brand-700">رقم المتدربة: {s.code}</span>}
              <span className="chip bg-sand text-ink-2" dir="ltr">ID {s.national_id}</span>
              {s.branch && <span className="chip bg-sand text-ink-2"><Building2 className="h-3 w-3" /> {s.branch}</span>}
            </div>
          </div>
        </div>
        <div className="flex gap-2">
          <button onClick={onRefresh} className="btn-ghost !px-4 !py-2.5 text-sm"><RefreshCw className={cx('h-4 w-4', refreshing && 'animate-spin')} /> تحديث</button>
          <button onClick={onLogout} className="btn-ghost !px-4 !py-2.5 text-sm hover:!text-red-600"><LogOut className="h-4 w-4" /> خروج</button>
        </div>
      </div>

      {/* Finance */}
      <motion.div variants={stagger.container} initial="hidden" animate="show" className="mt-10 grid gap-5 lg:grid-cols-[1.1fr_2fr]">
        <motion.div variants={stagger.item} className="grain relative overflow-hidden rounded-[2rem] bg-deep-900 p-7 text-white">
          <div className="pattern-light absolute inset-0" />
          <div className="absolute -top-16 -left-16 h-48 w-48 rounded-full bg-brand-500/40 blur-3xl" />
          <div className="relative flex items-center gap-6">
            <Ring pct={pct} />
            <div>
              <div className="text-sm text-white/60">نسبة السداد</div>
              <div className="font-display text-4xl font-black"><Counter value={pct} suffix="%" /></div>
              <div className="mt-1 text-sm text-white/60">من صافي الرسوم</div>
            </div>
          </div>
          <div className="relative mt-7 rounded-2xl bg-white/10 p-4">
            <div className="text-sm text-white/65">المتبقي عليكِ</div>
            <div className="mt-1 font-display text-3xl font-black text-gold-300"><Counter value={Math.round(f.balance)} /> <span className="text-base font-bold text-white/60">ر.س</span></div>
          </div>
        </motion.div>
        <div className="grid grid-cols-2 gap-5 md:grid-cols-4 lg:grid-cols-2">
          {[
            { label: 'إجمالي الرسوم', value: f.total, icon: Wallet, tone: 'bg-brand-50 text-brand-700' },
            { label: 'الخصومات', value: f.discount, icon: BadgeCheck, tone: 'bg-gold-400/15 text-gold-600' },
            { label: 'المدفوع', value: f.paid, icon: CreditCard, tone: 'bg-emerald-500/10 text-emerald-600' },
            { label: 'المتبقي', value: f.balance, icon: Receipt, tone: 'bg-red-500/10 text-red-600' },
          ].map((c) => (
            <motion.div key={c.label} variants={stagger.item} className="card p-5 transition hover:-translate-y-1 hover:shadow-lift">
              <span className={cx('grid h-11 w-11 place-items-center rounded-xl', c.tone)}><c.icon className="h-5 w-5" /></span>
              <div className="mt-4 text-sm text-muted">{c.label}</div>
              <div className="mt-1 font-display text-2xl font-black text-ink tabular-nums">{money(c.value)} <span className="text-sm font-semibold text-muted">ر.س</span></div>
            </motion.div>
          ))}
        </div>
      </motion.div>

      {/* Programs */}
      <h2 className="mt-14 mb-5 flex items-center gap-2 text-2xl font-extrabold"><BookOpen className="h-6 w-6 text-brand-600" /> برامجي</h2>
      {s.programs.length ? (
        <motion.div variants={stagger.container} initial="hidden" animate="show" className="grid gap-5 md:grid-cols-2">
          {s.programs.map((p, i) => (
            <motion.article key={i} variants={stagger.item} className="card relative overflow-hidden p-6">
              <div className={cx('absolute inset-y-0 right-0 w-1.5', p.kind === 'diploma' ? 'bg-brand-500' : 'bg-gold-400')} />
              <div className="flex items-start justify-between gap-3">
                <div>
                  <span className={cx('chip', p.kind === 'diploma' ? 'bg-brand-50 text-brand-700' : 'bg-gold-400/15 text-gold-600')}>{p.kind === 'diploma' ? 'دبلوم' : 'دورة'}</span>
                  <h3 className="mt-3 text-xl font-bold">{p.title}</h3>
                </div>
                {p.status && <span className="chip shrink-0 bg-emerald-500/10 text-emerald-600">{p.status}</span>}
              </div>
              <dl className="mt-5 grid grid-cols-2 gap-4 text-sm">
                {[
                  { icon: Layers, k: 'المستوى', v: p.level },
                  { icon: CalendarDays, k: 'الدفعة', v: p.batch },
                  { icon: Building2, k: 'الفرع', v: p.branch },
                  { icon: CalendarDays, k: 'تاريخ البداية', v: p.start_date ? fmtDate(p.start_date) : null },
                ].filter((x) => x.v).map((x) => (
                  <div key={x.k} className="flex items-start gap-2">
                    <x.icon className="mt-0.5 h-4 w-4 text-muted" />
                    <div><dt className="text-muted">{x.k}</dt><dd className="font-semibold text-ink">{x.v}</dd></div>
                  </div>
                ))}
              </dl>
              {p.reg_code && <div className="mt-5 border-t border-line pt-3 text-xs text-muted">رقم التسجيل: <span className="font-semibold text-ink-2" dir="ltr">{p.reg_code}</span></div>}
            </motion.article>
          ))}
        </motion.div>
      ) : (
        <p className="card p-8 text-center text-muted">لا توجد برامج مسجلة حاليًا</p>
      )}

      {/* Payments */}
      <h2 className="mt-14 mb-5 flex items-center gap-2 text-2xl font-extrabold"><Receipt className="h-6 w-6 text-brand-600" /> سجل السدادات</h2>
      <div className="card overflow-hidden">
        {s.payments.length ? (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-sand text-muted">
                <tr>{['التاريخ', 'المبلغ', 'طريقة السداد', 'رقم السند', 'البيان'].map((h) => <th key={h} className="px-5 py-3 text-start font-semibold">{h}</th>)}</tr>
              </thead>
              <tbody className="divide-y divide-line">
                {s.payments.map((p, i) => (
                  <motion.tr key={i} initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: 0.3 + i * 0.04 }} className="transition hover:bg-sand/50">
                    <td className="px-5 py-3.5 whitespace-nowrap">{p.date ? fmtDate(p.date) : '—'}</td>
                    <td className="px-5 py-3.5 font-bold whitespace-nowrap text-emerald-600 tabular-nums">{money(p.amount)} ر.س</td>
                    <td className="px-5 py-3.5">{p.method ?? '—'}</td>
                    <td className="px-5 py-3.5 font-mono text-xs" dir="ltr">{p.ref ?? '—'}</td>
                    <td className="px-5 py-3.5 text-muted">{p.note ?? ''}</td>
                  </motion.tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p className="p-8 text-center text-muted">لا توجد سدادات مسجلة بعد</p>
        )}
      </div>
      <p className="mt-6 flex items-center gap-2 text-xs text-muted">
        <RefreshCw className="h-3.5 w-3.5" /> آخر تحديث من نظام المعهد: {fmtDateTime(s.synced_at)} · لأي استفسار عن الحساب تواصلي مع الإدارة المالية
        <WhatsAppIcon className="ms-1 h-3.5 w-3.5 text-[#25D366]" />
      </p>
    </div>
  )
}

function Ring({ pct }: { pct: number }) {
  const r = 42
  const c = 2 * Math.PI * r
  return (
    <svg viewBox="0 0 100 100" className="h-28 w-28 -rotate-90">
      <circle cx="50" cy="50" r={r} fill="none" stroke="rgb(255 255 255 / .12)" strokeWidth="10" />
      <motion.circle
        cx="50" cy="50" r={r} fill="none" stroke="url(#ringGrad)" strokeWidth="10" strokeLinecap="round"
        strokeDasharray={c} initial={{ strokeDashoffset: c }} animate={{ strokeDashoffset: c - (c * pct) / 100 }} transition={{ duration: 1.6, ease: [0.22, 1, 0.36, 1], delay: 0.3 }}
      />
      <defs>
        <linearGradient id="ringGrad" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor="var(--glow)" />
          <stop offset="100%" stopColor="var(--color-gold-400)" />
        </linearGradient>
      </defs>
    </svg>
  )
}
