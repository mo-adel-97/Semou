import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { motion } from 'motion/react'
import { Cloud, Eye, EyeOff, Lock, ShieldCheck, User, Zap } from 'lucide-react'
import { useAuth } from '../auth'
import { ApiError } from '../api'
import { alerts } from '../lib/alerts'
import { Field, Spinner } from '../components/ui'
import { ThemePicker } from '../components/ThemePicker'
import { TestBanner } from '../components/Shell'

const schema = z.object({
  userName: z.string().trim().min(1, 'يرجى إدخال اسم المستخدم'),
  password: z.string().min(1, 'يرجى إدخال كلمة المرور'),
})
type LoginForm = z.infer<typeof schema>

export function Login() {
  const { login } = useAuth()
  const [show, setShow] = useState(false)
  const { register, handleSubmit, setFocus, formState: { errors, isSubmitting } } = useForm<LoginForm>({
    resolver: zodResolver(schema),
    defaultValues: { userName: '', password: '' },
  })

  const onSubmit = async (v: LoginForm) => {
    try {
      await login(v.userName, v.password)
    } catch (e) {
      const err = e as ApiError
      await alerts.error(err.message, err.status === 403 ? 'المستخدم موقوف' : 'تعذر تسجيل الدخول')
      setFocus(err.status === 401 ? 'password' : 'userName')
    }
  }

  return (
    <div className="auth">
      <section className="auth-form">
        <div style={{ position: 'absolute', top: 0, insetInline: 0 }}><TestBanner /></div>
        <div className="theme-corner"><ThemePicker /></div>
        <motion.div className="auth-card" initial={{ opacity: 0, y: 16 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.4, ease: 'easeOut' }}>
          <img className="logo-big" src="/logo.png" alt="معهد السمو" />
          <h2>أهلاً بعودتك</h2>
          <p className="sub">سجّل دخولك لمتابعة العمل على نظام معهد السمو</p>
          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <Field label="اسم المستخدم" error={errors.userName?.message}>
              <div className="input-wrap">
                <User size={17} />
                <input className={`input ${errors.userName ? 'invalid' : ''}`} autoFocus autoComplete="username" {...register('userName')} />
              </div>
            </Field>
            <Field label="كلمة المرور" error={errors.password?.message}>
              <div className="input-wrap">
                <Lock size={17} />
                <input className={`input ${errors.password ? 'invalid' : ''}`} type={show ? 'text' : 'password'} autoComplete="current-password" {...register('password')} />
                <button type="button" className="btn ghost sm icon trail" onClick={() => setShow(s => !s)} aria-label="إظهار كلمة المرور">
                  {show ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>
            </Field>
            <button className="btn" disabled={isSubmitting}>{isSubmitting ? <Spinner /> : 'تسجيل الدخول'}</button>
          </form>
        </motion.div>
      </section>
      <section className="auth-art">
        <motion.span className="ring" style={{ width: 520, height: 520, top: -180, left: -160 }} animate={{ rotate: 360 }} transition={{ duration: 120, repeat: Infinity, ease: 'linear' }} />
        <span className="ring" style={{ width: 340, height: 340, top: -90, left: -70 }} />
        <span className="ring" style={{ width: 300, height: 300, bottom: -120, right: -80 }} />
        <div className="brand">
          <span className="logo"><img src="/logo.png" alt="" /></span>
          <span><b>معهد السمو المتخصص العالي للتدريب</b><small>حفر الباطن · المملكة العربية السعودية</small></span>
        </div>
        <motion.div className="copy" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.15, duration: 0.5 }}>
          <h1>كل إدارة المعهد<br />في مكان واحد</h1>
          <p>التسجيل، الحسابات، الجداول، خدمة العملاء والتقارير، من أي جهاز وفي أي وقت وبنفس صلاحياتك.</p>
          <div className="feats">
            <span><Cloud size={15} /> من أي مكان</span>
            <span><ShieldCheck size={15} /> صلاحيات دقيقة</span>
            <span><Zap size={15} /> سريع وخفيف</span>
          </div>
          <div className="foot">جميع الحقوق محفوظة © معهد السمو</div>
        </motion.div>
      </section>
    </div>
  )
}
