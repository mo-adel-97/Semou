import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Loader2, Pencil, Plus, ShieldCheck, Trash2, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useForm } from 'react-hook-form'
import { useState } from 'react'
import { Skeleton } from '../components/ui'
import { confirmDanger, toast } from '../lib/alerts'
import { ApiError, api } from '../lib/api'
import { cx, timeAgo } from '../lib/format'
import type { User } from '../lib/types'
import { PageTitle } from './AdminApp'
import { useMe } from './auth'
import { Toggle } from './fields'

type AdminUser = User & { is_active: boolean; last_login_at: string | null; created_at: string }
type FormV = { name: string; email: string; role: 'admin' | 'editor'; is_active: boolean; password: string }

export default function UsersPage() {
  const me = useMe().data!
  const qc = useQueryClient()
  const { data, isLoading } = useQuery({ queryKey: ['admin', 'users'], queryFn: () => api.get<AdminUser[]>('/admin/users'), enabled: me.role === 'admin' })
  const [editing, setEditing] = useState<AdminUser | 'new' | null>(null)
  const remove = useMutation({
    mutationFn: (id: number) => api.del(`/admin/users/${id}`),
    onSuccess: () => { toast.success('تم حذف المستخدم'); qc.invalidateQueries({ queryKey: ['admin', 'users'] }) },
    onError: (e) => toast.error(e.message),
  })
  if (me.role !== 'admin') return <p className="p-10 text-center text-muted">هذه الصفحة متاحة للمدير فقط</p>

  return (
    <>
      <PageTitle
        title="المستخدمون"
        text="المدير يتحكم في كل شيء، والمحرر يدير المحتوى والرسائل فقط"
        actions={<button onClick={() => setEditing('new')} className="btn-primary !px-5 !py-2.5 text-sm"><Plus className="h-4 w-4" /> مستخدم جديد</button>}
      />
      <div className="card overflow-hidden">
        {isLoading ? (
          <div className="space-y-2 p-4">{Array.from({ length: 3 }, (_, i) => <Skeleton key={i} className="h-14" />)}</div>
        ) : (
          <ul className="divide-y divide-line">
            {data?.map((u) => (
              <li key={u.id} className="flex items-center gap-4 px-4 py-3.5 md:px-6">
                <span className="grid h-10 w-10 shrink-0 place-items-center rounded-full bg-brand-100 font-display font-bold text-brand-700">{u.name.trim()[0]}</span>
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2">
                    <span className="truncate font-bold">{u.name}</span>
                    {u.role === 'admin' && <span className="chip bg-gold-400/15 text-gold-600"><ShieldCheck className="h-3 w-3" /> مدير</span>}
                    {!u.is_active && <span className="chip bg-red-500/10 text-red-600">موقوف</span>}
                    {u.id === me.id && <span className="chip bg-sand text-muted">أنت</span>}
                  </div>
                  <div className="truncate text-sm text-muted" dir="ltr" style={{ textAlign: 'right' }}>{u.email}</div>
                </div>
                <span className="hidden text-xs text-muted sm:inline">{u.last_login_at ? `آخر دخول ${timeAgo(u.last_login_at)}` : 'لم يسجل الدخول'}</span>
                <button onClick={() => setEditing(u)} className="rounded-lg p-2 text-ink-2 hover:bg-sand" aria-label="تعديل"><Pencil className="h-4 w-4" /></button>
                {u.id !== me.id && (
                  <button onClick={async () => { if (await confirmDanger(`حذف ${u.name}؟`)) remove.mutate(u.id) }} className="rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600" aria-label="حذف">
                    <Trash2 className="h-4 w-4" />
                  </button>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>
      <AnimatePresence>{editing && <UserDrawer user={editing === 'new' ? null : editing} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

function UserDrawer({ user, onClose }: { user: AdminUser | null; onClose: () => void }) {
  const qc = useQueryClient()
  const form = useForm<FormV>({
    defaultValues: { name: user?.name ?? '', email: user?.email ?? '', role: user?.role ?? 'editor', is_active: user?.is_active ?? true, password: '' },
  })
  const save = useMutation({
    mutationFn: (v: FormV) => (user ? api.put(`/admin/users/${user.id}`, v) : api.post('/admin/users', v)),
    onSuccess: () => { toast.success(user ? 'تم حفظ التعديلات' : 'تم إنشاء المستخدم'); qc.invalidateQueries({ queryKey: ['admin', 'users'] }); onClose() },
    onError: (e) => {
      if (e instanceof ApiError) for (const [k, m] of Object.entries(e.errors)) form.setError(k as keyof FormV, { message: m })
      toast.error(e.message)
    },
  })
  const err = form.formState.errors
  return (
    <>
      <motion.div className="fixed inset-0 z-50 bg-black/30" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} />
      <motion.form
        onSubmit={form.handleSubmit((v) => save.mutate(v))}
        initial={{ x: '-100%' }}
        animate={{ x: 0 }}
        exit={{ x: '-100%' }}
        transition={{ type: 'spring', damping: 32, stiffness: 320 }}
        className="fixed inset-y-0 left-0 z-50 flex w-full max-w-md flex-col bg-surface shadow-2xl"
        noValidate
      >
        <div className="flex items-center justify-between border-b border-line px-5 py-4">
          <h2 className="text-lg font-bold">{user ? 'تعديل مستخدم' : 'مستخدم جديد'}</h2>
          <button type="button" onClick={onClose} className="rounded-lg p-2 hover:bg-sand" aria-label="إغلاق"><X className="h-5 w-5" /></button>
        </div>
        <div className="flex-1 space-y-5 overflow-y-auto p-6">
          <div>
            <label className="label">الاسم</label>
            <input className="input" {...form.register('name')} aria-invalid={!!err.name || undefined} />
            {err.name && <p className="mt-1 text-sm text-red-600">{err.name.message}</p>}
          </div>
          <div>
            <label className="label">البريد الإلكتروني</label>
            <input className="input text-left" dir="ltr" type="email" {...form.register('email')} aria-invalid={!!err.email || undefined} />
            {err.email && <p className="mt-1 text-sm text-red-600">{err.email.message}</p>}
          </div>
          <div>
            <label className="label">الصلاحية</label>
            <div className="grid grid-cols-2 gap-2">
              {(['editor', 'admin'] as const).map((r) => (
                <label key={r} className={cx('cursor-pointer rounded-xl border p-3 text-sm transition', form.watch('role') === r ? 'border-brand-500 bg-brand-50' : 'border-line')}>
                  <input type="radio" value={r} className="sr-only" {...form.register('role')} />
                  <div className="font-bold">{r === 'admin' ? 'مدير' : 'محرر'}</div>
                  <div className="mt-0.5 text-xs text-muted">{r === 'admin' ? 'كل الصلاحيات' : 'المحتوى والرسائل'}</div>
                </label>
              ))}
            </div>
          </div>
          <div>
            <label className="label">{user ? 'كلمة مرور جديدة (اتركها فارغة لعدم التغيير)' : 'كلمة المرور'}</label>
            <input className="input text-left" dir="ltr" type="password" autoComplete="new-password" {...form.register('password')} aria-invalid={!!err.password || undefined} />
            {err.password ? <p className="mt-1 text-sm text-red-600">{err.password.message}</p> : <p className="mt-1 text-xs text-muted">10 أحرف على الأقل</p>}
          </div>
          <Toggle checked={form.watch('is_active')} onChange={(v) => form.setValue('is_active', v, { shouldDirty: true })} label="الحساب مفعّل" />
        </div>
        <div className="border-t border-line p-4">
          <button type="submit" disabled={save.isPending} className="btn-primary w-full">{save.isPending && <Loader2 className="h-4 w-4 animate-spin" />} حفظ</button>
        </div>
      </motion.form>
    </>
  )
}
