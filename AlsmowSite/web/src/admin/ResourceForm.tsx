import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ArrowRight, ExternalLink, Loader2, Save, Trash2 } from 'lucide-react'
import { useEffect } from 'react'
import { useForm, type FieldValues } from 'react-hook-form'
import { Link, useBlocker, useNavigate, useParams } from 'react-router-dom'
import { Skeleton } from '../components/ui'
import { confirmDanger, toast } from '../lib/alerts'
import { ApiError, api } from '../lib/api'
import type { FieldSpec, ResourceSpec, Row } from '../lib/types'
import { useSchema } from './auth'
import { Field } from './fields'

// Fields shown in the narrow side column; everything else goes in the main column.
const SIDE: FieldSpec['type'][] = ['image', 'bool', 'enum', 'date', 'int', 'slug']
const PUBLIC_PATH: Record<string, string> = { programs: '/programs/', posts: '/blog/', pages: '/p/' }

export default function ResourceForm() {
  const { res = '', id } = useParams()
  const schema = useSchema()
  const spec = schema.data?.resources.find((r) => r.name === res)
  const record = useQuery({
    queryKey: ['admin', 'record', res, id],
    queryFn: () => api.get<Row>(`/admin/content/${res}/${id}`),
    enabled: !!id && !!spec,
  })
  if (schema.isLoading || (id && record.isLoading)) return <div className="space-y-4"><Skeleton className="h-10 w-64" /><Skeleton className="h-96" /></div>
  if (!spec) return <p className="p-10 text-center text-muted">نوع المحتوى غير موجود</p>
  if (record.error) return <p className="p-10 text-center text-muted">{record.error.message}</p>
  return <Editor key={`${res}-${id ?? 'new'}`} spec={spec} record={record.data} />
}

function defaults(spec: ResourceSpec): FieldValues {
  const v: FieldValues = {}
  for (const f of spec.fields) v[f.key] = f.default ?? (f.type === 'list' ? [] : f.type === 'bool' ? false : f.type === 'int' ? null : '')
  return v
}

function Editor({ spec, record }: { spec: ResourceSpec; record?: Row }) {
  const nav = useNavigate()
  const qc = useQueryClient()
  const isNew = !record
  const form = useForm<FieldValues>({ defaultValues: record ? { ...defaults(spec), ...record } : defaults(spec) })

  const save = useMutation({
    mutationFn: (v: FieldValues) => {
      const body = Object.fromEntries(spec.fields.map((f) => [f.key, v[f.key]]))
      return isNew ? api.post<Row>(`/admin/content/${spec.name}`, body) : api.put<Row>(`/admin/content/${spec.name}/${record!.id}`, body)
    },
    onSuccess: (saved) => {
      toast.success(isNew ? `تمت إضافة ${spec.singular} بنجاح` : 'تم حفظ التعديلات')
      qc.invalidateQueries({ queryKey: ['admin', 'content', spec.name] })
      qc.invalidateQueries({ queryKey: ['admin', 'dashboard'] })
      qc.setQueryData(['admin', 'record', spec.name, String(saved.id)], saved)
      form.reset({ ...defaults(spec), ...saved })
      if (isNew) nav(`/admin/c/${spec.name}/${saved.id}`, { replace: true })
    },
    onError: (e) => {
      if (e instanceof ApiError) for (const [k, m] of Object.entries(e.errors)) form.setError(k, { message: m })
      toast.error(e.message)
    },
  })
  const remove = useMutation({
    mutationFn: () => api.del(`/admin/content/${spec.name}/${record!.id}`),
    onSuccess: () => {
      form.reset(form.getValues())
      qc.invalidateQueries({ queryKey: ['admin', 'content', spec.name] })
      toast.success('تم الحذف')
      nav(`/admin/c/${spec.name}`)
    },
  })

  // Ctrl+S saves
  useEffect(() => {
    const h = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') {
        e.preventDefault()
        form.handleSubmit((v) => save.mutate(v))()
      }
    }
    window.addEventListener('keydown', h)
    return () => window.removeEventListener('keydown', h)
  }, [form, save])

  // Warn before leaving with unsaved changes
  const dirty = form.formState.isDirty && !save.isPending
  useEffect(() => {
    const h = (e: BeforeUnloadEvent) => { if (dirty) e.preventDefault() }
    window.addEventListener('beforeunload', h)
    return () => window.removeEventListener('beforeunload', h)
  }, [dirty])
  const blocker = useBlocker(({ currentLocation, nextLocation }) => dirty && currentLocation.pathname !== nextLocation.pathname)
  useEffect(() => {
    if (blocker.state !== 'blocked') return
    confirmDanger('لديك تعديلات غير محفوظة', 'هل تريد المغادرة دون حفظ؟', 'مغادرة').then((ok) => (ok ? blocker.proceed() : blocker.reset()))
  }, [blocker])

  const main = spec.fields.filter((f) => !SIDE.includes(f.type))
  const side = spec.fields.filter((f) => SIDE.includes(f.type))
  const errors = form.formState.errors
  const slug = record?.slug as string | undefined
  const publicUrl = slug && PUBLIC_PATH[spec.name] ? `${PUBLIC_PATH[spec.name]}${slug}` : null

  return (
    <form onSubmit={form.handleSubmit((v) => save.mutate(v))} noValidate>
      <div className="sticky top-[57px] z-20 -mx-4 mb-6 flex flex-wrap items-center gap-3 border-b border-line bg-bg/90 px-4 py-3 backdrop-blur md:-mx-8 md:px-8">
        <Link to={`/admin/c/${spec.name}`} className="grid h-10 w-10 place-items-center rounded-xl bg-surface shadow-soft hover:bg-sand" aria-label="رجوع"><ArrowRight className="h-5 w-5" /></Link>
        <div className="min-w-0 flex-1">
          <div className="text-sm text-muted">{spec.label}</div>
          <h1 className="truncate text-xl font-extrabold">{isNew ? `${spec.singular} جديد` : String(record[spec.title] ?? '')}</h1>
        </div>
        {dirty && <span className="hidden text-sm text-gold-600 sm:inline">● تعديلات غير محفوظة</span>}
        {publicUrl && (
          <a href={publicUrl} target="_blank" rel="noopener noreferrer" className="btn-ghost !px-4 !py-2.5 text-sm"><ExternalLink className="h-4 w-4" /> معاينة</a>
        )}
        <button type="submit" disabled={save.isPending} className="btn-primary !px-6 !py-2.5 text-sm">
          {save.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />} حفظ
        </button>
      </div>

      <div className="grid gap-6 xl:grid-cols-[1fr_340px]">
        <div className="card space-y-6 p-5 md:p-7">
          {main.map((f) => <Field key={f.key} f={f} control={form.control} error={errors[f.key]?.message as string | undefined} />)}
        </div>
        <div className="space-y-6">
          <div className="card space-y-5 p-5">
            {side.map((f) => <Field key={f.key} f={f} control={form.control} error={errors[f.key]?.message as string | undefined} />)}
          </div>
          {!isNew && (
            <button
              type="button"
              onClick={async () => { if (await confirmDanger(`حذف هذا ${spec.singular}؟`, 'لا يمكن التراجع عن الحذف.')) remove.mutate() }}
              className="flex w-full items-center justify-center gap-2 rounded-2xl border border-red-200 bg-surface px-4 py-3 text-sm font-bold text-red-600 transition hover:bg-red-500/10"
            >
              <Trash2 className="h-4 w-4" /> حذف {spec.singular}
            </button>
          )}
        </div>
      </div>
    </form>
  )
}
