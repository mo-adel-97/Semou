import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { KeyRound, Loader2 } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { toast } from '../lib/alerts'
import { ApiError, api } from '../lib/api'
import { PageTitle } from './AdminApp'
import { useMe } from './auth'

const schema = z
  .object({
    current: z.string().min(1, 'أدخل كلمة المرور الحالية'),
    password: z.string().min(10, 'كلمة المرور يجب ألا تقل عن 10 أحرف'),
    confirm: z.string(),
  })
  .refine((v) => v.password === v.confirm, { path: ['confirm'], message: 'كلمتا المرور غير متطابقتين' })

export default function ProfilePage() {
  const me = useMe().data!
  const form = useForm<z.infer<typeof schema>>({ resolver: zodResolver(schema), defaultValues: { current: '', password: '', confirm: '' } })
  const save = useMutation({
    mutationFn: (v: z.infer<typeof schema>) => api.post('/auth/password', { current: v.current, password: v.password }),
    onSuccess: () => { toast.success('تم تغيير كلمة المرور، وتم تسجيل خروج الأجهزة الأخرى'); form.reset() },
    onError: (e) => {
      if (e instanceof ApiError) for (const [k, m] of Object.entries(e.errors)) form.setError(k as 'current', { message: m })
      toast.error(e.message)
    },
  })
  const err = form.formState.errors
  return (
    <>
      <PageTitle title="حسابي" text={`${me.name} · ${me.email}`} />
      <form onSubmit={form.handleSubmit((v) => save.mutate(v))} className="card max-w-xl space-y-5 p-6" noValidate>
        <div className="flex items-center gap-3">
          <span className="grid h-11 w-11 place-items-center rounded-xl bg-brand-100 text-brand-700"><KeyRound className="h-5 w-5" /></span>
          <div>
            <h2 className="font-bold">تغيير كلمة المرور</h2>
            <p className="text-sm text-muted">سيتم تسجيل خروجك من جميع الأجهزة الأخرى</p>
          </div>
        </div>
        {(['current', 'password', 'confirm'] as const).map((k) => (
          <div key={k}>
            <label className="label">{k === 'current' ? 'كلمة المرور الحالية' : k === 'password' ? 'كلمة المرور الجديدة' : 'تأكيد كلمة المرور الجديدة'}</label>
            <input type="password" dir="ltr" className="input text-left" autoComplete={k === 'current' ? 'current-password' : 'new-password'} {...form.register(k)} aria-invalid={!!err[k] || undefined} />
            {err[k] && <p className="mt-1 text-sm text-red-600">{err[k]?.message}</p>}
          </div>
        ))}
        <button type="submit" disabled={save.isPending} className="btn-primary">{save.isPending && <Loader2 className="h-4 w-4 animate-spin" />} حفظ</button>
      </form>
    </>
  )
}
