import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { KeyRound } from 'lucide-react'
import { api } from '../api'
import { alerts } from '../lib/alerts'
import { Field, Spinner } from '../components/ui'
import { PageHead } from '../components/Shell'

/** Port of ChangeUserPassword. */
const schema = z.object({
  oldPassword: z.string().min(1, 'يرجى إدخال كلمة المرور الحالية'),
  newPassword: z.string().min(1, 'يرجى إدخال كلمة المرور الجديدة').max(50, 'الحد الأقصى 50 حرف'),
  confirm: z.string(),
}).refine(v => v.newPassword === v.confirm, { path: ['confirm'], message: 'كلمة المرور الجديدة وتأكيدها غير متطابقين' })
type Form = z.infer<typeof schema>

export function ChangePassword() {
  const nav = useNavigate()
  const { register, handleSubmit, formState: { errors } } = useForm<Form>({ resolver: zodResolver(schema) })

  const mutation = useMutation({
    mutationFn: (v: Form) => api<{ message: string }>('/auth/change-password', { method: 'POST', json: { oldPassword: v.oldPassword, newPassword: v.newPassword } }),
    onSuccess: async r => { await alerts.success(r.message); nav('/') },
  })

  return (
    <>
      <PageHead title="تغيير كلمة المرور" group="ملف" subtitle="غيّر كلمة المرور الخاصة بحسابك" />
      <form className="card card-pad grid" style={{ maxWidth: 520 }} onSubmit={handleSubmit(v => mutation.mutate(v))} noValidate>
        <Field label="كلمة المرور الحالية" required error={errors.oldPassword?.message}>
          <input className={`input ${errors.oldPassword ? 'invalid' : ''}`} type="password" autoComplete="current-password" autoFocus {...register('oldPassword')} />
        </Field>
        <Field label="كلمة المرور الجديدة" required error={errors.newPassword?.message}>
          <input className={`input ${errors.newPassword ? 'invalid' : ''}`} type="password" autoComplete="new-password" maxLength={50} {...register('newPassword')} />
        </Field>
        <Field label="تأكيد كلمة المرور الجديدة" required error={errors.confirm?.message}>
          <input className={`input ${errors.confirm ? 'invalid' : ''}`} type="password" autoComplete="new-password" maxLength={50} {...register('confirm')} />
        </Field>
        <div className="row">
          <button className="btn" disabled={mutation.isPending}>{mutation.isPending ? <Spinner /> : <><KeyRound size={16} /> حفظ كلمة المرور</>}</button>
          <button type="button" className="btn ghost" onClick={() => nav(-1)}>إلغاء</button>
        </div>
      </form>
    </>
  )
}
