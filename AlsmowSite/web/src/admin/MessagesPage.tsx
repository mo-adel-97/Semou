import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Download, Inbox, Mail, MailOpen, Phone, Search, Trash2, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useState, type ReactNode } from 'react'
import { useSearchParams } from 'react-router-dom'
import { WhatsAppIcon } from '../components/BrandIcons'
import { Skeleton } from '../components/ui'
import { confirmDanger, toast } from '../lib/alerts'
import { api, qs } from '../lib/api'
import { cx, fmtDateTime, timeAgo, waNumber } from '../lib/format'
import type { Message } from '../lib/types'
import { PageTitle } from './AdminApp'
import { useMe } from './auth'

type Tab = '' | 'unread' | 'register' | 'contact'

export default function MessagesPage() {
  const qc = useQueryClient()
  const me = useMe().data
  const [sp, setSp] = useSearchParams()
  const [tab, setTab] = useState<Tab>('')
  const [q, setQ] = useState('')
  const [page, setPage] = useState(1)
  const params = { status: tab === 'unread' ? 'unread' : '', kind: tab === 'register' || tab === 'contact' ? tab : '', q, page }
  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'messages', params],
    queryFn: () => api.get<{ items: Message[]; total: number; pages: number }>(`/admin/messages${qs(params)}`),
    placeholderData: keepPreviousData,
  })
  const openId = Number(sp.get('open')) || null
  const [selected, setSelected] = useState<Message | null>(null)
  useEffect(() => {
    if (openId && data) {
      const m = data.items.find((x) => x.id === openId)
      if (m) setSelected(m)
    }
  }, [openId, data])

  const refresh = () => {
    qc.invalidateQueries({ queryKey: ['admin', 'messages'] })
    qc.invalidateQueries({ queryKey: ['admin', 'unread'] })
    qc.invalidateQueries({ queryKey: ['admin', 'dashboard'] })
  }
  const mark = useMutation({
    mutationFn: ({ id, read }: { id: number; read: boolean }) => api.put(`/admin/messages/${id}`, { is_read: read }),
    onSuccess: refresh,
  })
  const remove = useMutation({
    mutationFn: (id: number) => api.del(`/admin/messages/${id}`),
    onSuccess: () => { toast.success('تم حذف الرسالة'); setSelected(null); refresh() },
    onError: (e) => toast.error(e.message),
  })

  const open = (m: Message) => {
    setSelected(m)
    if (!m.is_read) mark.mutate({ id: m.id, read: true })
  }
  const close = () => {
    setSelected(null)
    if (sp.has('open')) { sp.delete('open'); setSp(sp, { replace: true }) }
  }

  const tabs: { id: Tab; label: string }[] = [
    { id: '', label: 'الكل' },
    { id: 'unread', label: 'غير مقروءة' },
    { id: 'register', label: 'طلبات التسجيل' },
    { id: 'contact', label: 'رسائل التواصل' },
  ]

  return (
    <>
      <PageTitle
        title="الرسائل والتسجيلات"
        text={data ? `${data.total} رسالة` : undefined}
        actions={<a href="/api/admin/messages-export" className="btn-ghost !px-4 !py-2.5 text-sm"><Download className="h-4 w-4" /> تصدير Excel</a>}
      />
      <div className="mb-4 flex flex-col gap-3 md:flex-row">
        <div className="seg-scroll flex gap-1 overflow-x-auto rounded-xl border border-line bg-surface p-1 md:flex-wrap">
          {tabs.map((t) => (
            <button key={t.id} onClick={() => { setTab(t.id); setPage(1) }} className={cx('shrink-0 whitespace-nowrap rounded-lg px-3 py-1.5 text-sm font-semibold transition', tab === t.id ? 'bg-primary text-white' : 'text-ink-2 hover:bg-sand')}>
              {t.label}
            </button>
          ))}
        </div>
        <label className="relative flex-1">
          <Search className="pointer-events-none absolute top-1/2 right-3.5 h-4 w-4 -translate-y-1/2 text-muted" />
          <input className="input ps-10" placeholder="بحث بالاسم أو الجوال أو البرنامج..." value={q} onChange={(e) => { setQ(e.target.value); setPage(1) }} />
        </label>
      </div>

      <div className="card overflow-hidden">
        {isLoading ? (
          <div className="space-y-2 p-4">{Array.from({ length: 6 }, (_, i) => <Skeleton key={i} className="h-14" />)}</div>
        ) : !data?.items.length ? (
          <div className="flex flex-col items-center gap-3 py-20 text-muted"><Inbox className="h-10 w-10" />لا توجد رسائل</div>
        ) : (
          <ul className="divide-y divide-line">
            {data.items.map((m) => (
              <li key={m.id}>
                <button onClick={() => open(m)} className={cx('flex w-full items-center gap-4 px-4 py-3.5 text-start transition hover:bg-sand/50 md:px-6', !m.is_read && 'bg-gold-400/10')}>
                  <span className={cx('grid h-10 w-10 shrink-0 place-items-center rounded-full font-display font-bold', m.kind === 'register' ? 'bg-brand-100 text-brand-700' : 'bg-sand text-ink-2')}>
                    {m.name.trim()[0]}
                  </span>
                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2">
                      <span className={cx('truncate', !m.is_read ? 'font-bold' : 'font-medium')}>{m.name}</span>
                      <span className={cx('chip shrink-0', m.kind === 'register' ? 'bg-brand-50 text-brand-700' : 'bg-sand text-ink-2')}>{m.kind === 'register' ? 'تسجيل' : 'تواصل'}</span>
                    </div>
                    <div className="truncate text-sm text-muted">{m.program || m.subject || m.body || m.phone}</div>
                  </div>
                  <span className="shrink-0 text-xs text-muted">{timeAgo(m.created_at)}</span>
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
      {data && data.pages > 1 && (
        <div className="mt-4 flex items-center justify-center gap-2">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)} className="btn-ghost !px-4 !py-2 text-sm">السابق</button>
          <span className="text-sm text-muted">{page} / {data.pages}</span>
          <button disabled={page >= data.pages} onClick={() => setPage(page + 1)} className="btn-ghost !px-4 !py-2 text-sm">التالي</button>
        </div>
      )}

      <AnimatePresence>
        {selected && (
          <>
            <motion.div className="fixed inset-0 z-50 bg-black/30" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={close} />
            <motion.aside
              initial={{ x: '-100%' }}
              animate={{ x: 0 }}
              exit={{ x: '-100%' }}
              transition={{ type: 'spring', damping: 32, stiffness: 320 }}
              className="fixed inset-y-0 left-0 z-50 flex w-full max-w-md flex-col bg-surface shadow-2xl"
            >
              <div className="flex items-center justify-between border-b border-line px-5 py-4">
                <span className={cx('chip', selected.kind === 'register' ? 'bg-brand-50 text-brand-700' : 'bg-sand text-ink-2')}>{selected.kind === 'register' ? 'طلب تسجيل' : 'رسالة تواصل'}</span>
                <button onClick={close} className="rounded-lg p-2 hover:bg-sand" aria-label="إغلاق"><X className="h-5 w-5" /></button>
              </div>
              <div className="flex-1 space-y-5 overflow-y-auto p-6">
                <div>
                  <h2 className="text-2xl font-extrabold">{selected.name}</h2>
                  <p className="mt-1 text-sm text-muted">{fmtDateTime(selected.created_at)}</p>
                </div>
                <dl className="space-y-3 rounded-2xl bg-sand/60 p-4">
                  {selected.phone && <Row label="الجوال"><span dir="ltr">{selected.phone}</span></Row>}
                  {selected.email && <Row label="البريد">{selected.email}</Row>}
                  {selected.program && <Row label="البرنامج">{selected.program}</Row>}
                  {selected.subject && <Row label="الموضوع">{selected.subject}</Row>}
                </dl>
                {selected.body && <p className="leading-[1.9] whitespace-pre-wrap text-ink-2">{selected.body}</p>}
                <div className="grid grid-cols-2 gap-2">
                  {selected.phone && (
                    <>
                      <a href={`https://wa.me/${waNumber(selected.phone)}?text=${encodeURIComponent(`مرحبًا ${selected.name}، معك معهد السمو للتدريب بخصوص طلبك.`)}`} target="_blank" rel="noopener noreferrer" className="btn bg-[#25D366] !py-2.5 text-sm text-white">
                        <WhatsAppIcon className="h-4 w-4" /> واتساب
                      </a>
                      <a href={`tel:${selected.phone}`} className="btn-ghost !py-2.5 text-sm"><Phone className="h-4 w-4" /> اتصال</a>
                    </>
                  )}
                  {selected.email && <a href={`mailto:${selected.email}`} className="btn-ghost col-span-2 !py-2.5 text-sm"><Mail className="h-4 w-4" /> رد بالبريد</a>}
                </div>
              </div>
              <div className="flex gap-2 border-t border-line p-4">
                <button onClick={() => { mark.mutate({ id: selected.id, read: false }); close() }} className="btn-ghost flex-1 !py-2.5 text-sm"><MailOpen className="h-4 w-4" /> تعليم كغير مقروءة</button>
                {me?.role === 'admin' && (
                  <button onClick={async () => { if (await confirmDanger('حذف هذه الرسالة؟')) remove.mutate(selected.id) }} className="rounded-full border border-red-200 px-4 text-red-600 hover:bg-red-500/10" aria-label="حذف">
                    <Trash2 className="h-4 w-4" />
                  </button>
                )}
              </div>
            </motion.aside>
          </>
        )}
      </AnimatePresence>
    </>
  )
}

function Row({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="flex gap-3 text-sm">
      <dt className="w-20 shrink-0 text-muted">{label}</dt>
      <dd className="font-semibold break-words text-ink">{children}</dd>
    </div>
  )
}
