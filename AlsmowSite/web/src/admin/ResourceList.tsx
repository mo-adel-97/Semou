import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Eye, EyeOff, GripVertical, ImageOff, Pencil, Plus, Search, Trash2 } from 'lucide-react'
import { Reorder } from 'motion/react'
import { useEffect, useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { Skeleton } from '../components/ui'
import { confirmDanger, toast } from '../lib/alerts'
import { api, qs } from '../lib/api'
import { cx, fmtDate } from '../lib/format'
import type { FieldSpec, ResourceSpec, Row } from '../lib/types'
import { PageTitle } from './AdminApp'
import { useSchema } from './auth'

export default function ResourceList() {
  const { res = '' } = useParams()
  const schema = useSchema()
  const spec = schema.data?.resources.find((r) => r.name === res)
  if (schema.isLoading) return <Skeleton className="h-96" />
  if (!spec) return <p className="p-10 text-center text-muted">نوع المحتوى غير موجود</p>
  return <List key={spec.name} spec={spec} />
}

function List({ spec }: { spec: ResourceSpec }) {
  const qc = useQueryClient()
  const [q, setQ] = useState('')
  const [debounced, setDebounced] = useState('')
  const [filters, setFilters] = useState<Record<string, string>>({})
  useEffect(() => {
    const t = setTimeout(() => setDebounced(q), 250)
    return () => clearTimeout(t)
  }, [q])

  const key = ['admin', 'content', spec.name, debounced, filters]
  const { data, isLoading } = useQuery({
    queryKey: key,
    queryFn: () => api.get<Row[]>(`/admin/content/${spec.name}${qs({ q: debounced, ...filters })}`),
  })
  const [rows, setRows] = useState<Row[]>([])
  useEffect(() => setRows(data ?? []), [data])

  const sortable = spec.fields.some((f) => f.key === 'sort') && !debounced && !Object.values(filters).some(Boolean)
  const imageField = spec.fields.find((f) => f.type === 'image' && f.column)
  const columns = spec.fields.filter((f) => f.column && f.type !== 'image' && f.key !== spec.title && f.type !== 'bool')
  const hasPublished = spec.fields.some((f) => f.key === 'is_published')

  const invalidate = () => {
    qc.invalidateQueries({ queryKey: ['admin', 'content', spec.name] })
    qc.invalidateQueries({ queryKey: ['admin', 'dashboard'] })
  }

  const reorder = useMutation({
    mutationFn: (ids: number[]) => api.post(`/admin/content/${spec.name}/reorder`, { ids }),
    onSuccess: () => toast.success('تم حفظ الترتيب'),
    onError: (e) => { toast.error(e.message); invalidate() },
  })
  const toggle = useMutation({
    mutationFn: (r: Row) => api.put(`/admin/content/${spec.name}/${r.id}`, { is_published: !r.is_published }),
    onMutate: (r) => setRows((cur) => cur.map((x) => (x.id === r.id ? { ...x, is_published: !r.is_published } : x))),
    onSuccess: (_d, r) => toast.success(r.is_published ? 'تم إخفاء العنصر من الموقع' : 'تم نشر العنصر'),
    onError: (e) => { toast.error(e.message); invalidate() },
  })
  const remove = useMutation({
    mutationFn: (id: number) => api.del(`/admin/content/${spec.name}/${id}`),
    onSuccess: () => { toast.success('تم الحذف'); invalidate() },
    onError: (e) => toast.error(e.message),
  })

  const [dragged, setDragged] = useState(false)
  const typeFilter = spec.filters.map((k) => spec.fields.find((f) => f.key === k)!).filter(Boolean)

  const empty = useMemo(() => !isLoading && rows.length === 0, [isLoading, rows])

  return (
    <>
      <PageTitle
        title={spec.label}
        text={`${rows.length} ${rows.length === 1 ? spec.singular : 'عنصر'}${sortable && rows.length > 1 ? ' · اسحب العناصر لتغيير ترتيب ظهورها' : ''}`}
        actions={<Link to={`/admin/c/${spec.name}/new`} className="btn-primary !px-5 !py-2.5 text-sm"><Plus className="h-4 w-4" /> إضافة {spec.singular}</Link>}
      />

      <div className="mb-4 flex flex-col gap-3 sm:flex-row">
        <label className="relative flex-1">
          <Search className="pointer-events-none absolute top-1/2 right-3.5 h-4 w-4 -translate-y-1/2 text-muted" />
          <input className="input ps-10" placeholder="بحث..." value={q} onChange={(e) => setQ(e.target.value)} />
        </label>
        {typeFilter.map((f) => (
          <div key={f.key} className="flex flex-wrap gap-1 rounded-xl border border-line bg-surface p-1">
            {[{ value: '', label: 'الكل' }, ...(f.options ?? [])].map((o) => (
              <button
                key={o.value}
                onClick={() => setFilters({ ...filters, [f.key]: o.value })}
                className={cx('rounded-lg px-3 py-1.5 text-sm font-semibold transition', (filters[f.key] ?? '') === o.value ? 'bg-primary text-white' : 'text-ink-2 hover:bg-sand')}
              >
                {o.label}
              </button>
            ))}
          </div>
        ))}
      </div>

      <div className="card overflow-hidden">
        {isLoading ? (
          <div className="space-y-2 p-4">{Array.from({ length: 5 }, (_, i) => <Skeleton key={i} className="h-16" />)}</div>
        ) : empty ? (
          <div className="flex flex-col items-center gap-3 px-6 py-16 text-center">
            <p className="text-lg font-bold">لا يوجد محتوى {debounced && 'مطابق للبحث'}</p>
            <Link to={`/admin/c/${spec.name}/new`} className="btn-primary !px-5 !py-2.5 text-sm"><Plus className="h-4 w-4" /> إضافة {spec.singular}</Link>
          </div>
        ) : (
          <Reorder.Group
            axis="y"
            values={rows}
            onReorder={(next) => { setRows(next); setDragged(true) }}
            className="divide-y divide-line"
          >
            {rows.map((r) => (
              <Reorder.Item
                key={r.id}
                value={r}
                dragListener={sortable}
                onDragEnd={() => { if (dragged) { reorder.mutate(rows.map((x) => x.id)); setDragged(false) } }}
                className="flex items-center gap-3 bg-surface px-3 py-3 md:px-4"
              >
                {sortable && <GripVertical className="h-5 w-5 shrink-0 cursor-grab text-muted/60 active:cursor-grabbing" />}
                {imageField && (
                  r[imageField.key] ? (
                    <img src={r[imageField.key] as string} alt="" className="h-12 w-16 shrink-0 rounded-lg bg-sand object-cover" loading="lazy" draggable={false} />
                  ) : (
                    <span className="grid h-12 w-16 shrink-0 place-items-center rounded-lg bg-sand text-muted"><ImageOff className="h-4 w-4" /></span>
                  )
                )}
                <div className="min-w-0 flex-1">
                  <Link to={`/admin/c/${spec.name}/${r.id}`} className="block truncate font-bold text-ink hover:text-brand-700">{String(r[spec.title] ?? '')}</Link>
                  <div className="mt-0.5 flex flex-wrap gap-x-3 gap-y-1 text-sm text-muted">
                    {columns.map((c) => <Cell key={c.key} f={c} v={r[c.key]} />)}
                    {r.is_featured === true && <span className="text-gold-600">★ في الرئيسية</span>}
                  </div>
                </div>
                {hasPublished && (
                  <button
                    onClick={() => toggle.mutate(r)}
                    className={cx('hidden shrink-0 items-center gap-1.5 rounded-full px-3 py-1.5 text-xs font-bold transition sm:flex', r.is_published ? 'bg-brand-50 text-brand-700 hover:bg-brand-100' : 'bg-sand text-muted hover:bg-line')}
                    title={r.is_published ? 'اضغط للإخفاء' : 'اضغط للنشر'}
                  >
                    {r.is_published ? <Eye className="h-3.5 w-3.5" /> : <EyeOff className="h-3.5 w-3.5" />}
                    {r.is_published ? 'منشور' : 'مخفي'}
                  </button>
                )}
                <Link to={`/admin/c/${spec.name}/${r.id}`} className="rounded-lg p-2 text-ink-2 hover:bg-sand" aria-label="تعديل"><Pencil className="h-4 w-4" /></Link>
                <button
                  onClick={async () => { if (await confirmDanger(`حذف «${String(r[spec.title])}»؟`, 'لا يمكن التراجع عن الحذف.')) remove.mutate(r.id) }}
                  className="rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600"
                  aria-label="حذف"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              </Reorder.Item>
            ))}
          </Reorder.Group>
        )}
      </div>
    </>
  )
}

function Cell({ f, v }: { f: FieldSpec; v: unknown }) {
  if (v === null || v === undefined || v === '') return null
  if (f.type === 'enum') return <span className="chip bg-brand-50 text-brand-700">{f.options?.find((o) => o.value === v)?.label ?? String(v)}</span>
  if (f.type === 'date') return <span>{fmtDate(String(v))}</span>
  if (f.key === 'hours') return <span>{String(v)} ساعة</span>
  if (f.key === 'rating') return <span className="text-gold-600">{'★'.repeat(Number(v))}</span>
  return <span className="truncate">{String(v)}</span>
}
