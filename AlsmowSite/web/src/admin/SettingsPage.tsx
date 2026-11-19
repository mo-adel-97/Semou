import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Loader2, Save } from 'lucide-react'
import { useEffect } from 'react'
import { useForm, type FieldValues } from 'react-hook-form'
import { NavLink, useParams } from 'react-router-dom'
import { Skeleton } from '../components/ui'
import { toast } from '../lib/alerts'
import { ApiError, api } from '../lib/api'
import { cx } from '../lib/format'
import type { Settings, SettingsGroupSpec } from '../lib/types'
import { ICONS, PageTitle } from './AdminApp'
import { useSchema } from './auth'
import { Field } from './fields'
import TextsManager from './TextsManager'

export default function SettingsPage() {
  const { group } = useParams()
  const schema = useSchema()
  const settings = useQuery({ queryKey: ['admin', 'settings'], queryFn: () => api.get<Settings>('/admin/settings') })
  const groups = schema.data?.settings ?? []
  const active = groups.find((g) => g.group === group) ?? groups[0]

  return (
    <>
      <PageTitle title="إعدادات الموقع" text="كل ما تغيّره هنا يظهر على الموقع خلال دقيقة" />
      <div className="grid gap-6 lg:grid-cols-[220px_1fr]">
        <nav className="flex gap-1 overflow-x-auto lg:flex-col">
          {groups.map((g) => {
            const Icon = ICONS[g.icon]
            return (
              <NavLink
                key={g.group}
                to={`/admin/settings/${g.group}`}
                className={() => cx('flex shrink-0 items-center gap-3 rounded-xl px-4 py-2.5 text-[15px] font-semibold transition', active?.group === g.group ? 'bg-surface text-brand-700 shadow-soft' : 'text-ink-2 hover:bg-white/60')}
              >
                {Icon && <Icon className="h-4 w-4" />}
                {g.label}
              </NavLink>
            )
          })}
        </nav>
        {!active || settings.isLoading || !settings.data ? (
          <Skeleton className="h-96" />
        ) : active.group === 'texts' ? (
          <TextsManager rules={settings.data.texts?.rules ?? []} />
        ) : (
          <GroupForm key={active.group} spec={active} values={settings.data[active.group] as FieldValues} />
        )}
      </div>
    </>
  )
}

function GroupForm({ spec, values }: { spec: SettingsGroupSpec; values: FieldValues }) {
  const qc = useQueryClient()
  const init = () => Object.fromEntries(spec.fields.map((f) => [f.key, values?.[f.key] ?? f.default ?? (f.type === 'list' || f.type === 'stats' || f.type === 'kb' ? [] : '')]))
  const form = useForm<FieldValues>({ defaultValues: init() })
  useEffect(() => form.reset(init()), [values]) // eslint-disable-line react-hooks/exhaustive-deps

  const save = useMutation({
    mutationFn: (v: FieldValues) => api.put<FieldValues>(`/admin/settings/${spec.group}`, v),
    onSuccess: (saved) => {
      qc.setQueryData<Settings>(['admin', 'settings'], (old) => (old ? { ...old, [spec.group]: saved } : old))
      qc.invalidateQueries({ queryKey: ['site'] })
      qc.invalidateQueries({ queryKey: ['home'] })
      form.reset(saved)
      toast.success('تم حفظ الإعدادات')
    },
    onError: (e) => {
      if (e instanceof ApiError) for (const [k, m] of Object.entries(e.errors)) form.setError(k, { message: m })
      toast.error(e.message)
    },
  })

  return (
    <form onSubmit={form.handleSubmit((v) => save.mutate(v))} className="card overflow-hidden" noValidate>
      <div className="flex items-center justify-between border-b border-line px-6 py-4">
        <h2 className="text-lg font-bold">{spec.label}</h2>
        <button type="submit" disabled={save.isPending || !form.formState.isDirty} className="btn-primary !px-5 !py-2 text-sm">
          {save.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />} حفظ
        </button>
      </div>
      <div className="grid gap-6 p-6 md:grid-cols-2">
        {spec.fields.map((f) => (
          <div key={f.key} className={cx(['html', 'text', 'list', 'stats', 'image', 'kb'].includes(f.type) && 'md:col-span-2')}>
            <Field f={f} control={form.control} error={form.formState.errors[f.key]?.message as string | undefined} />
          </div>
        ))}
      </div>
    </form>
  )
}
