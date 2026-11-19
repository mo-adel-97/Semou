import { zodResolver } from '@hookform/resolvers/zod'
import BrandLogo from '../components/BrandLogo'
import { Eye, EyeOff, Loader2, Lock, Mail } from 'lucide-react'
import { motion } from 'motion/react'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useLogin } from './auth'

const schema = z.object({
  email: z.email('البريد الإلكتروني غير صحيح'),
  password: z.string().min(1, 'كلمة المرور مطلوبة'),
})

export default function Login() {
  const login = useLogin()
  const [show, setShow] = useState(false)
  const form = useForm<z.infer<typeof schema>>({ resolver: zodResolver(schema), defaultValues: { email: '', password: '' } })
  const e = form.formState.errors

  return (
    <div className="grid min-h-dvh lg:grid-cols-2">
      <title>تسجيل الدخول | لوحة التحكم</title>
      <div className="relative hidden overflow-hidden bg-deep-900 lg:block">
        <div className="pattern-light absolute inset-0" />
        <div className="absolute -top-32 -right-32 h-[30rem] w-[30rem] rounded-full bg-brand-500/30 blur-3xl" />
        <div className="absolute -bottom-40 -left-20 h-[26rem] w-[26rem] rounded-full bg-gold-400/20 blur-3xl" />
        <div className="relative flex h-full flex-col justify-between p-14 text-white">
          <div className="flex items-center gap-3">
            <BrandLogo className="h-16 w-16" />
            <div className="font-display text-xl font-extrabold">معهد السمو للتدريب</div>
          </div>
          <div>
            <h1 className="max-w-md text-5xl leading-tight font-black">لوحة تحكم المحتوى</h1>
            <p className="mt-5 max-w-md text-lg leading-relaxed text-white/70">
              أديري البرامج والمقالات والصفحات والرسائل من مكان واحد، وكل تعديل يظهر على الموقع فورًا.
            </p>
          </div>
          <p className="text-sm text-white/40">© {new Date().getFullYear()} معهد السمو للتدريب</p>
        </div>
      </div>

      <div className="flex items-center justify-center p-6">
        <motion.form
          initial={{ opacity: 0, y: 16 }}
          animate={{ opacity: 1, y: 0 }}
          onSubmit={form.handleSubmit((v) => login.mutate(v))}
          className="w-full max-w-sm"
          noValidate
        >
          <BrandLogo className="mb-8 h-16 w-16 lg:hidden" />
          <h2 className="text-3xl font-extrabold">أهلًا بعودتك 👋</h2>
          <p className="mt-2 text-muted">سجّل الدخول لإدارة محتوى الموقع</p>

          {login.error && (
            <motion.p initial={{ opacity: 0, y: -6 }} animate={{ opacity: 1, y: 0 }} className="mt-6 rounded-xl bg-red-500/10 px-4 py-3 text-sm font-semibold text-red-600 dark:text-red-300">
              {login.error.message}
            </motion.p>
          )}

          <div className="mt-8 space-y-5">
            <div>
              <label className="label" htmlFor="email">البريد الإلكتروني</label>
              <div className="relative">
                <Mail className="pointer-events-none absolute top-1/2 right-3.5 h-5 w-5 -translate-y-1/2 text-muted" />
                <input id="email" type="email" dir="ltr" autoComplete="username" className="input !py-3 ps-11 text-left" aria-invalid={!!e.email || undefined} {...form.register('email')} />
              </div>
              {e.email && <p className="mt-1.5 text-sm text-red-600">{e.email.message}</p>}
            </div>
            <div>
              <label className="label" htmlFor="password">كلمة المرور</label>
              <div className="relative">
                <Lock className="pointer-events-none absolute top-1/2 right-3.5 h-5 w-5 -translate-y-1/2 text-muted" />
                <input id="password" type={show ? 'text' : 'password'} dir="ltr" autoComplete="current-password" className="input !py-3 ps-11 pe-11 text-left" aria-invalid={!!e.password || undefined} {...form.register('password')} />
                <button type="button" onClick={() => setShow(!show)} className="absolute top-1/2 left-3 -translate-y-1/2 p-1 text-muted hover:text-ink" aria-label={show ? 'إخفاء' : 'إظهار'}>
                  {show ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
              {e.password && <p className="mt-1.5 text-sm text-red-600">{e.password.message}</p>}
            </div>
          </div>
          <button type="submit" disabled={login.isPending} className="btn-primary mt-8 w-full !py-3.5">
            {login.isPending && <Loader2 className="h-5 w-5 animate-spin" />} دخول
          </button>
        </motion.form>
      </div>
    </div>
  )
}
