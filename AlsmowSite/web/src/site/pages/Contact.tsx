import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { CheckCircle2, Clock, Loader2, Mail, MapPin, Phone, Send, Building2 } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import type { ComponentType } from 'react'
import { useForm } from 'react-hook-form'
import { Link, useSearchParams } from 'react-router-dom'
import { z } from 'zod'
import { SocialLinks, WhatsAppIcon } from '../../components/BrandIcons'
import { PageHero, Reveal, Seo } from '../../components/ui'
import { ApiError, api } from '../../lib/api'
import { cx, waNumber } from '../../lib/format'
import { Sections } from '../../lib/layout'
import { usePrograms, useSite } from '../../lib/queries'
import { track } from '../../lib/track'
import { PROGRAM_TYPES } from '../../lib/types'

const schema = z.object({
  name: z.string().trim().min(3, 'يرجى كتابة الاسم الكامل').max(120),
  phone: z.string().trim().regex(/^[0-9+\s-]{8,20}$/, 'رقم الجوال غير صحيح'),
  email: z.union([z.literal(''), z.email('البريد الإلكتروني غير صحيح')]),
  program: z.string().optional(),
  study_mode: z.enum(['', 'onsite', 'online']).optional(),
  national_id: z.union([z.literal(''), z.string().regex(/^[12]\d{9}$/, 'رقم الهوية أو الإقامة يتكون من 10 أرقام')]).optional(),
  city: z.string().max(80).optional(),
  subject: z.string().max(200).optional(),
  body: z.string().max(3000).optional(),
  website: z.string().optional(), // honeypot
})
type FormValues = z.infer<typeof schema>

function MessageForm({ kind }: { kind: 'contact' | 'register' }) {
  const [sp] = useSearchParams()
  const programs = usePrograms()
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: '', phone: '', email: '', program: sp.get('program') ?? '', subject: '', body: '', website: '',
      study_mode: (sp.get('mode') as 'onsite' | 'online' | null) ?? '', national_id: '', city: '',
    },
  })
  const send = useMutation({
    mutationFn: (v: FormValues) => {
      if (kind === 'register' && !v.study_mode) {
        form.setError('study_mode', { message: 'اختر طريقة الدراسة' })
        return Promise.reject(new ApiError(422, 'اختر طريقة الدراسة', { study_mode: 'اختر طريقة الدراسة' }))
      }
      return api.post('/public/messages', { ...v, kind })
    },
    onSuccess: () => track(kind === 'register' ? 'register_submit' : 'contact_submit'),
    onError: (e) => {
      if (e instanceof ApiError) for (const [k, m] of Object.entries(e.errors)) form.setError(k as keyof FormValues, { message: m })
    },
  })
  const err = form.formState.errors

  if (send.isSuccess) {
    return (
      <motion.div initial={{ opacity: 0, scale: 0.96 }} animate={{ opacity: 1, scale: 1 }} className="flex flex-col items-center px-6 py-16 text-center">
        <motion.span initial={{ scale: 0 }} animate={{ scale: 1 }} transition={{ type: 'spring', delay: 0.1 }} className="grid h-20 w-20 place-items-center rounded-full bg-brand-100 text-brand-700">
          <CheckCircle2 className="h-10 w-10" />
        </motion.span>
        <h3 className="mt-6 text-2xl font-extrabold">{kind === 'register' ? 'تم استلام طلبك بنجاح' : 'تم إرسال رسالتك'}</h3>
        <p className="mt-3 max-w-sm leading-relaxed text-muted">شكرًا لتواصلك مع معهد السمو، سيتواصل معك فريقنا في أقرب وقت.</p>
        <Link to="/programs" className="btn-ghost mt-8">تصفّح البرامج</Link>
      </motion.div>
    )
  }

  const field = (name: keyof FormValues) => ({ ...form.register(name), 'aria-invalid': !!err[name] || undefined })
  const Err = ({ name }: { name: keyof FormValues }) => (
    <AnimatePresence>
      {err[name] && (
        <motion.p initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }} className="mt-1.5 text-sm text-red-600">
          {err[name]?.message}
        </motion.p>
      )}
    </AnimatePresence>
  )

  return (
    <form onSubmit={form.handleSubmit((v) => send.mutate(v))} className="grid gap-5 p-6 md:grid-cols-2 md:p-8" noValidate>
      <div>
        <label className="label" htmlFor="f-name">الاسم الكامل *</label>
        <input id="f-name" className="input !py-3" autoComplete="name" {...field('name')} />
        <Err name="name" />
      </div>
      <div>
        <label className="label" htmlFor="f-phone">رقم الجوال *</label>
        <input id="f-phone" className="input !py-3 text-left" dir="ltr" inputMode="tel" autoComplete="tel" placeholder="05XXXXXXXX" {...field('phone')} />
        <Err name="phone" />
      </div>
      <div className={cx(kind === 'contact' && 'md:col-span-2')}>
        <label className="label" htmlFor="f-email">البريد الإلكتروني</label>
        <input id="f-email" className="input !py-3 text-left" dir="ltr" type="email" autoComplete="email" {...field('email')} />
        <Err name="email" />
      </div>
      {kind === 'register' && (
        <>
          <div className="md:col-span-2">
            <span className="label">طريقة الدراسة *</span>
            <div className="grid grid-cols-2 gap-3">
              {([
                { v: 'onsite', t: 'حضوري', d: 'في مقر المعهد' },
                { v: 'online', t: 'عن بعد', d: 'من أي مكان' },
              ] as const).map((o) => (
                <label
                  key={o.v}
                  className={cx(
                    'relative cursor-pointer rounded-2xl border-2 p-4 transition',
                    form.watch('study_mode') === o.v ? 'border-brand-500 bg-brand-50 shadow-soft' : 'border-line bg-surface hover:border-brand-300',
                  )}
                >
                  <input type="radio" value={o.v} className="sr-only" {...form.register('study_mode')} />
                  <div className="font-display text-lg font-bold text-ink">{o.t}</div>
                  <div className="text-sm text-muted">{o.d}</div>
                  {form.watch('study_mode') === o.v && <motion.span layoutId="mode-dot" className="absolute top-4 left-4 h-3 w-3 rounded-full bg-brand-500 ring-4 ring-brand-500/20" />}
                </label>
              ))}
            </div>
            <Err name="study_mode" />
          </div>
          <div>
            <label className="label" htmlFor="f-nid">رقم الهوية / الإقامة</label>
            <input id="f-nid" className="input !py-3 text-left" dir="ltr" inputMode="numeric" maxLength={10} placeholder="1XXXXXXXXX" {...field('national_id')} />
            <Err name="national_id" />
          </div>
          <div>
            <label className="label" htmlFor="f-city">المدينة</label>
            <input id="f-city" className="input !py-3" placeholder="حفر الباطن" {...field('city')} />
          </div>
        </>
      )}
      {kind === 'register' ? (
        <div className="md:col-span-2">
          <label className="label" htmlFor="f-program">البرنامج المطلوب</label>
          <select id="f-program" className="input !py-3" {...field('program')}>
            <option value="">لم أحدد بعد</option>
            {Object.entries(PROGRAM_TYPES).map(([t, info]) => {
              const items = programs.data?.filter((p) => p.type === t) ?? []
              return items.length ? (
                <optgroup key={t} label={info.plural}>
                  {items.map((p) => <option key={p.id} value={p.title}>{p.title}</option>)}
                </optgroup>
              ) : null
            })}
            {sp.get('program') && !programs.data?.some((p) => p.title === sp.get('program')) && <option value={sp.get('program')!}>{sp.get('program')}</option>}
          </select>
        </div>
      ) : (
        <div className="md:col-span-2">
          <label className="label" htmlFor="f-subject">الموضوع</label>
          <input id="f-subject" className="input !py-3" {...field('subject')} />
        </div>
      )}
      <div className="md:col-span-2">
        <label className="label" htmlFor="f-body">{kind === 'register' ? 'ملاحظات (اختياري)' : 'رسالتك *'}</label>
        <textarea id="f-body" rows={5} className="input !py-3" {...field('body')} />
        <Err name="body" />
      </div>
      <input type="text" tabIndex={-1} autoComplete="off" className="hidden" aria-hidden="true" {...form.register('website')} />
      <div className="md:col-span-2">
        {send.error && !(send.error instanceof ApiError && Object.keys(send.error.errors).length) && (
          <p className="mb-4 rounded-xl bg-red-500/10 px-4 py-3 text-sm font-semibold text-red-600 dark:text-red-300">{send.error.message}</p>
        )}
        <button type="submit" disabled={send.isPending} className="btn-primary w-full !py-4 text-base md:w-auto md:!px-10">
          {send.isPending ? <Loader2 className="h-5 w-5 animate-spin" /> : <Send className="h-5 w-5" />}
          {kind === 'register' ? 'إرسال طلب التسجيل' : 'إرسال الرسالة'}
        </button>
      </div>
    </form>
  )
}

function ContactCards() {
  const { data } = useSite()
  const c = data?.settings.contact
  const wa = waNumber(c?.whatsapp || c?.phone)
  const items = [
    c?.phone && { icon: Phone, label: 'اتصل بنا', value: c.phone, href: `tel:${c.phone}`, ltr: true },
    wa && { icon: WhatsAppIcon, label: 'واتساب', value: 'راسلنا مباشرة', href: `https://wa.me/${wa}` },
    c?.email && { icon: Mail, label: 'البريد الإلكتروني', value: c.email, href: `mailto:${c.email}` },
    Array.isArray(c?.branches) && c.branches.some((b) => b.trim()) && { icon: Building2, label: 'الفروع وطرق الدراسة', value: c.branches.filter((b) => b.trim()).join(String.fromCharCode(10)) },
    c?.address && { icon: MapPin, label: 'العنوان', value: c.address },
    c?.hours && { icon: Clock, label: 'أوقات العمل', value: c.hours },
  ].filter(Boolean) as { icon: ComponentType<{ className?: string }>; label: string; value: string; href?: string; ltr?: boolean }[]
  return (
    <div className="space-y-4">
      {items.map((it, i) => {
        const inner = (
          <>
            <span className="grid h-12 w-12 shrink-0 place-items-center rounded-2xl bg-brand-100 text-brand-700 transition group-hover:bg-primary group-hover:text-white">
              <it.icon className="h-5 w-5" />
            </span>
            <div className="min-w-0">
              <div className="text-sm text-muted">{it.label}</div>
              <div className="font-display font-bold break-words whitespace-pre-line text-ink" dir={it.ltr ? 'ltr' : undefined}>{it.value}</div>
            </div>
          </>
        )
        return (
          <Reveal key={it.label} delay={i * 0.05}>
            {it.href ? (
              <a href={it.href} target={it.href.startsWith('http') ? '_blank' : undefined} rel="noopener noreferrer" className="group card flex items-center gap-4 p-5 transition hover:shadow-lift">{inner}</a>
            ) : (
              <div className="card flex items-center gap-4 p-5">{inner}</div>
            )}
          </Reveal>
        )
      })}
      {data?.settings.social && (
        <SocialLinks social={data.settings.social} className="pt-2" itemClass="bg-surface text-brand-700 shadow-soft hover:bg-primary hover:text-white" />
      )}
    </div>
  )
}

export function Contact() {
  const { data } = useSite()
  const map = data?.settings.contact.map_url
  return (
    <>
      <Seo title="تواصل معنا" description="تواصل مع معهد السمو للتدريب" />
      <PageHero title="تواصل معنا" text="فريقنا جاهز للإجابة عن استفساراتك ومساعدتك في اختيار البرنامج المناسب." crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / تواصل معنا</>} />
      <Sections settings={data?.settings} page="contact" sections={{ form: (
      <section className="container-x grid gap-8 py-14 md:py-20 lg:grid-cols-[360px_1fr]">
        <ContactCards />
        <Reveal className="card overflow-hidden">
          <div className="border-b border-line px-6 py-5 md:px-8">
            <h2 className="text-2xl font-extrabold">أرسل رسالة</h2>
          </div>
          <MessageForm kind="contact" />
        </Reveal>
      </section>
      ), map: map ? (
        <section className="container-x pb-20">
          <iframe src={map} title="موقع المعهد على الخريطة" loading="lazy" referrerPolicy="no-referrer-when-downgrade" className="h-96 w-full rounded-[2rem] border-0 shadow-soft" />
        </section>
      ) : null }} />
    </>
  )
}

export function Register() {
  return (
    <>
      <Seo title="التسجيل" description="سجّل اهتمامك في برامج معهد السمو للتدريب" />
      <PageHero title="سجّل اهتمامك الآن" text="املأ النموذج وسيتواصل معك فريق القبول لإكمال إجراءات التسجيل." crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / التسجيل</>} />
      <section className="container-x grid gap-8 py-14 md:py-20 lg:grid-cols-[1fr_360px]">
        <Reveal className="card overflow-hidden">
          <div className="border-b border-line px-6 py-5 md:px-8">
            <h2 className="text-2xl font-extrabold">بيانات التسجيل</h2>
            <p className="mt-1 text-muted">الحقول المعلّمة بـ * مطلوبة</p>
          </div>
          <MessageForm kind="register" />
        </Reveal>
        <ContactCards />
      </section>
    </>
  )
}
