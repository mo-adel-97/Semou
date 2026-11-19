import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Copy, Loader2, Trash2, Upload } from 'lucide-react'
import { useRef, useState } from 'react'
import { Skeleton } from '../components/ui'
import { confirmDanger, toast } from '../lib/alerts'
import { api } from '../lib/api'
import { cx, fmtDate } from '../lib/format'
import { PageTitle } from './AdminApp'
import { useMe } from './auth'
import { useUpload, type MediaItem } from './fields'

export default function MediaPage() {
  const qc = useQueryClient()
  const me = useMe().data
  const [page, setPage] = useState(1)
  const { data, isLoading } = useQuery({ queryKey: ['admin', 'media', page], queryFn: () => api.get<{ items: MediaItem[]; pages: number }>(`/admin/media?page=${page}`) })
  const upload = useUpload()
  const input = useRef<HTMLInputElement>(null)
  const [drag, setDrag] = useState(false)
  const remove = useMutation({
    mutationFn: (id: number) => api.del(`/admin/media/${id}`),
    onSuccess: () => { toast.success('تم حذف الصورة'); qc.invalidateQueries({ queryKey: ['admin', 'media'] }) },
    onError: (e) => toast.error(e.message),
  })

  const uploadMany = async (files: FileList | null) => {
    if (!files) return
    for (const f of Array.from(files)) await upload.mutateAsync(f).catch(() => null)
    toast.success('تم رفع الصور')
  }

  return (
    <>
      <PageTitle
        title="مكتبة الصور"
        text="كل الصور تُضغط وتُحوّل لصيغة WebP تلقائيًا لسرعة الموقع"
        actions={
          <button onClick={() => input.current?.click()} disabled={upload.isPending} className="btn-primary !px-5 !py-2.5 text-sm">
            {upload.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Upload className="h-4 w-4" />} رفع صور
          </button>
        }
      />
      <input ref={input} type="file" multiple accept="image/jpeg,image/png,image/webp,image/gif" className="hidden" onChange={(e) => { uploadMany(e.target.files); e.target.value = '' }} />
      <div
        onDragOver={(e) => { e.preventDefault(); setDrag(true) }}
        onDragLeave={() => setDrag(false)}
        onDrop={(e) => { e.preventDefault(); setDrag(false); uploadMany(e.dataTransfer.files) }}
        className={cx('rounded-3xl border-2 border-dashed p-4 transition', drag ? 'border-brand-500 bg-brand-50' : 'border-transparent')}
      >
        {isLoading ? (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">{Array.from({ length: 10 }, (_, i) => <Skeleton key={i} className="aspect-square" />)}</div>
        ) : !data?.items.length ? (
          <div className="py-20 text-center text-muted">اسحب الصور هنا أو اضغط «رفع صور»</div>
        ) : (
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">
            {data.items.map((m) => (
              <div key={m.id} className="group card overflow-hidden">
                <div className="relative aspect-square bg-sand">
                  <img src={m.url} alt={m.original_name ?? ''} loading="lazy" className="h-full w-full object-cover" />
                  <div className="absolute inset-0 flex items-center justify-center gap-2 bg-deep-950/50 opacity-0 transition group-hover:opacity-100">
                    <button
                      onClick={() => { navigator.clipboard.writeText(location.origin + m.url); toast.success('تم نسخ الرابط') }}
                      className="grid h-9 w-9 place-items-center rounded-full bg-surface text-ink" aria-label="نسخ الرابط"
                    >
                      <Copy className="h-4 w-4" />
                    </button>
                    {me?.role === 'admin' && (
                      <button
                        onClick={async () => { if (await confirmDanger('حذف هذه الصورة؟', 'إذا كانت مستخدمة في الموقع ستختفي من مكانها.')) remove.mutate(m.id) }}
                        className="grid h-9 w-9 place-items-center rounded-full bg-red-600 text-white" aria-label="حذف"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                </div>
                <div className="p-2.5 text-xs">
                  <div className="truncate font-semibold text-ink-2">{m.original_name}</div>
                  <div className="mt-0.5 text-muted">{m.width}×{m.height} · {Math.round((m.size ?? 0) / 1024)} KB · {fmtDate(m.created_at)}</div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
      {data && data.pages > 1 && (
        <div className="mt-4 flex items-center justify-center gap-2">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)} className="btn-ghost !px-4 !py-2 text-sm">السابق</button>
          <span className="text-sm text-muted">{page} / {data.pages}</span>
          <button disabled={page >= data.pages} onClick={() => setPage(page + 1)} className="btn-ghost !px-4 !py-2 text-sm">التالي</button>
        </div>
      )}
    </>
  )
}
