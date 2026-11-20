import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ChevronDown, ChevronUp, Eye, EyeOff, GripVertical, ImagePlus, Images, Loader2, Plus, Trash2, Upload, X } from 'lucide-react'
import { AnimatePresence, motion, Reorder } from 'motion/react'
import { lazy, Suspense, useRef, useState, type ReactNode } from 'react'
import { Controller, type Control, type FieldValues } from 'react-hook-form'
import { toast } from '../lib/alerts'
import { api } from '../lib/api'
import { cx } from '../lib/format'
import type { FieldSpec, KbEntry, SectionPick, StatItem, TextPair } from '../lib/types'

const RichEditor = lazy(() => import('./RichEditor'))
const KbField = lazy(() => import('./KbField'))

export function FieldShell({ f, error, children }: { f: FieldSpec; error?: string; children: ReactNode }) {
  return (
    <div data-field={f.key} className="field-box">
      {f.type !== 'bool' && (
        <label className="label" htmlFor={`f-${f.key}`}>
          {f.label} {f.required && <span className="text-red-500">*</span>}
        </label>
      )}
      {children}
      {error ? <p className="mt-1.5 text-sm font-medium text-red-600">{error}</p> : f.help ? <p className="mt-1.5 text-xs text-muted">{f.help}</p> : null}
    </div>
  )
}

export function Field({ f, control, error }: { f: FieldSpec; control: Control<FieldValues>; error?: string }) {
  return (
    <Controller
      name={f.key}
      control={control}
      render={({ field }) => {
        const common = { id: `f-${f.key}`, 'aria-invalid': !!error || undefined }
        let input: ReactNode
        switch (f.type) {
          case 'string':
          case 'slug':
            input = <input {...common} className="input" value={(field.value as string) ?? ''} onChange={field.onChange} maxLength={f.max} placeholder={f.placeholder} dir={f.type === 'slug' ? 'auto' : undefined} />
            break
          case 'url':
            input = <input {...common} className="input text-left" dir="ltr" value={(field.value as string) ?? ''} onChange={field.onChange} placeholder={f.placeholder ?? 'https://'} />
            break
          case 'text':
            input = (
              <div className="relative">
                <textarea {...common} className="input min-h-28 resize-y" value={(field.value as string) ?? ''} onChange={field.onChange} maxLength={f.max} placeholder={f.placeholder} rows={4} />
                {f.max && <span className="pointer-events-none absolute bottom-2 left-3 text-xs text-muted tabular-nums">{((field.value as string) ?? '').length}/{f.max}</span>}
              </div>
            )
            break
          case 'int':
            input = (
              <input {...common} className="input tabular-nums" type="number" inputMode="numeric" min={f.min} max={f.max}
                value={field.value === null || field.value === undefined ? '' : String(field.value)}
                onChange={(e) => field.onChange(e.target.value === '' ? null : Number(e.target.value))} />
            )
            break
          case 'date':
            input = <input {...common} className="input" type="date" value={(field.value as string) ?? ''} onChange={(e) => field.onChange(e.target.value || null)} />
            break
          case 'enum':
            input = (
              <select {...common} className="input" value={(field.value as string) ?? ''} onChange={(e) => field.onChange(e.target.value || null)}>
                {!f.required && <option value="">— غير محدد —</option>}
                {f.options?.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}
              </select>
            )
            break
          case 'bool':
            input = <Toggle checked={!!field.value} onChange={field.onChange} label={f.label} />
            break
          case 'image':
            input = <ImageField value={(field.value as string) ?? null} onChange={field.onChange} />
            break
          case 'list':
            input = <ListField value={(field.value as string[]) ?? []} onChange={field.onChange} max={f.maxItems ?? 12} />
            break
          case 'stats':
            input = <StatsField value={(field.value as StatItem[]) ?? []} onChange={field.onChange} max={f.maxItems ?? 6} />
            break
          case 'sections':
            // a group never saved yet gives '' / {} here, not a list
            input = <SectionsField f={f} value={Array.isArray(field.value) ? (field.value as SectionPick[]) : []} onChange={field.onChange} />
            break
          case 'color':
            input = <ColorField value={(field.value as string) ?? ''} onChange={field.onChange} placeholder={f.placeholder} />
            break
          case 'pairs':
            input = <PairsField f={f} value={Array.isArray(field.value) ? (field.value as TextPair[]) : []} onChange={field.onChange} />
            break
          case 'kb':
            input = (
              <Suspense fallback={<Loader2 className="h-5 w-5 animate-spin text-muted" />}>
                <KbField value={(field.value as KbEntry[]) ?? []} onChange={field.onChange} max={f.maxItems ?? 500} />
              </Suspense>
            )
            break
          case 'html':
            input = (
              <Suspense fallback={<div className="grid h-72 place-items-center rounded-2xl border border-line bg-surface"><Loader2 className="h-6 w-6 animate-spin text-brand-500" /></div>}>
                <RichEditor value={(field.value as string) ?? ''} onChange={field.onChange} invalid={!!error} />
              </Suspense>
            )
            break
        }
        return <FieldShell f={f} error={error}>{input}</FieldShell>
      }}
    />
  )
}

export function Toggle({ checked, onChange, label }: { checked: boolean; onChange: (v: boolean) => void; label?: string }) {
  return (
    <label className="flex cursor-pointer items-center justify-between gap-3 rounded-xl border border-line bg-surface px-4 py-3 select-none">
      {label && <span className="text-sm font-semibold text-ink-2">{label}</span>}
      <button
        type="button"
        role="switch"
        aria-checked={checked}
        onClick={() => onChange(!checked)}
        className={cx('relative h-6 w-11 shrink-0 rounded-full transition', checked ? 'bg-brand-600' : 'bg-line')}
      >
        <motion.span layout transition={{ type: 'spring', stiffness: 500, damping: 30 }} className={cx('absolute top-0.5 h-5 w-5 rounded-full bg-surface shadow', checked ? 'left-0.5' : 'right-0.5')} />
      </button>
    </label>
  )
}

/* ---------------- Images ---------------- */

export function useUpload() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (file: File) => api.upload(file),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin', 'media'] }),
    onError: (e) => toast.error(e.message),
  })
}

function ImageField({ value, onChange }: { value: string | null; onChange: (v: string | null) => void }) {
  const upload = useUpload()
  const input = useRef<HTMLInputElement>(null)
  const [picker, setPicker] = useState(false)
  const [drag, setDrag] = useState(false)

  const handle = (file?: File) => {
    if (!file) return
    upload.mutate(file, { onSuccess: (r) => onChange(r.url) })
  }

  return (
    <>
      {value ? (
        <div className="group relative overflow-hidden rounded-2xl border border-line bg-sand">
          <img src={value} alt="" className="max-h-56 w-full object-contain" />
          <div className="absolute inset-0 flex items-center justify-center gap-2 bg-deep-950/50 opacity-0 transition group-hover:opacity-100 focus-within:opacity-100">
            <button type="button" onClick={() => input.current?.click()} className="btn-light !px-3 !py-2 text-sm"><Upload className="h-4 w-4" /> تغيير</button>
            <button type="button" onClick={() => setPicker(true)} className="btn-light !px-3 !py-2 text-sm"><Images className="h-4 w-4" /> المكتبة</button>
            <button type="button" onClick={() => onChange(null)} className="btn !bg-red-600 !px-3 !py-2 text-sm text-white" aria-label="إزالة"><Trash2 className="h-4 w-4" /></button>
          </div>
          {upload.isPending && <div className="absolute inset-0 grid place-items-center bg-white/70"><Loader2 className="h-6 w-6 animate-spin text-brand-600" /></div>}
        </div>
      ) : (
        <div
          onDragOver={(e) => { e.preventDefault(); setDrag(true) }}
          onDragLeave={() => setDrag(false)}
          onDrop={(e) => { e.preventDefault(); setDrag(false); handle(e.dataTransfer.files[0]) }}
          className={cx('flex flex-col items-center gap-3 rounded-2xl border-2 border-dashed px-4 py-8 text-center transition', drag ? 'border-brand-500 bg-brand-50' : 'border-line bg-white')}
        >
          {upload.isPending ? (
            <Loader2 className="h-8 w-8 animate-spin text-brand-500" />
          ) : (
            <span className="grid h-12 w-12 place-items-center rounded-2xl bg-brand-50 text-brand-600"><ImagePlus className="h-6 w-6" /></span>
          )}
          <p className="text-sm text-muted">اسحب صورة هنا أو</p>
          <div className="flex gap-2">
            <button type="button" onClick={() => input.current?.click()} className="btn-primary !px-4 !py-2 text-sm">رفع صورة</button>
            <button type="button" onClick={() => setPicker(true)} className="btn-ghost !px-4 !py-2 text-sm">من المكتبة</button>
          </div>
          <p className="text-xs text-muted">JPG أو PNG أو WebP حتى 8 ميجا · تُضغط تلقائيًا</p>
        </div>
      )}
      <input ref={input} type="file" accept="image/jpeg,image/png,image/webp,image/gif" className="hidden" onChange={(e) => { handle(e.target.files?.[0]); e.target.value = '' }} />
      <MediaPicker open={picker} onClose={() => setPicker(false)} onPick={(url) => { onChange(url); setPicker(false) }} />
    </>
  )
}

export interface MediaItem { id: number; url: string; original_name: string | null; width: number; height: number; size: number; created_at: string }

export function MediaPicker({ open, onClose, onPick }: { open: boolean; onClose: () => void; onPick: (url: string) => void }) {
  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'media', 1],
    queryFn: () => api.get<{ items: MediaItem[] }>('/admin/media?page=1'),
    enabled: open,
  })
  return (
    <AnimatePresence>
      {open && (
        <motion.div className="fixed inset-0 z-[90] flex items-center justify-center bg-black/50 p-4" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose}>
          <motion.div initial={{ scale: 0.96, y: 10 }} animate={{ scale: 1, y: 0 }} exit={{ scale: 0.96 }} onClick={(e) => e.stopPropagation()} className="flex max-h-[85vh] w-full max-w-3xl flex-col overflow-hidden rounded-3xl bg-surface shadow-2xl">
            <div className="flex items-center justify-between border-b border-line px-5 py-4">
              <h3 className="text-lg font-bold">مكتبة الصور</h3>
              <button type="button" onClick={onClose} className="rounded-lg p-2 hover:bg-sand" aria-label="إغلاق"><X className="h-5 w-5" /></button>
            </div>
            <div className="overflow-y-auto p-5">
              {isLoading ? (
                <div className="grid place-items-center py-16"><Loader2 className="h-6 w-6 animate-spin text-brand-500" /></div>
              ) : data?.items.length ? (
                <div className="grid grid-cols-3 gap-3 sm:grid-cols-4">
                  {data.items.map((m) => (
                    <button type="button" key={m.id} onClick={() => onPick(m.url)} className="group aspect-square overflow-hidden rounded-xl border border-line bg-sand ring-brand-500 transition hover:ring-4">
                      <img src={m.url} alt={m.original_name ?? ''} loading="lazy" className="h-full w-full object-cover transition group-hover:scale-105" />
                    </button>
                  ))}
                </div>
              ) : (
                <p className="py-16 text-center text-muted">لا توجد صور بعد، ارفع أول صورة.</p>
              )}
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  )
}

/* ---------------- Lists ---------------- */

type Keyed<T> = { k: string; v: T }
let seq = 0
const key = () => `i${++seq}`

function ListField({ value, onChange, max }: { value: string[]; onChange: (v: string[]) => void; max: number }) {
  const [items, setItems] = useState<Keyed<string>[]>(() => value.map((v) => ({ k: key(), v })))
  const [draft, setDraft] = useState('')
  const commit = (next: Keyed<string>[]) => {
    setItems(next)
    onChange(next.map((x) => x.v).filter((x) => x.trim()))
  }
  const add = () => {
    const t = draft.trim()
    if (!t || items.length >= max) return
    commit([...items, { k: key(), v: t }])
    setDraft('')
  }
  return (
    <div className="space-y-2">
      <Reorder.Group axis="y" values={items} onReorder={commit} className="space-y-2">
        {items.map((it) => (
          <Reorder.Item key={it.k} value={it} className="flex items-center gap-2 rounded-xl border border-line bg-surface p-1.5 ps-2">
            <GripVertical className="h-4 w-4 shrink-0 cursor-grab text-muted active:cursor-grabbing" />
            <input
              className="min-w-0 flex-1 bg-transparent px-1 py-1.5 text-[15px] focus:outline-none"
              value={it.v}
              onChange={(e) => commit(items.map((x) => (x.k === it.k ? { ...x, v: e.target.value } : x)))}
            />
            <button type="button" onClick={() => commit(items.filter((x) => x.k !== it.k))} className="rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600" aria-label="حذف">
              <Trash2 className="h-4 w-4" />
            </button>
          </Reorder.Item>
        ))}
      </Reorder.Group>
      {items.length < max && (
        <div className="flex gap-2">
          <input
            className="input"
            value={draft}
            placeholder="اكتب ثم اضغط Enter"
            onChange={(e) => setDraft(e.target.value)}
            onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); add() } }}
          />
          <button type="button" onClick={add} className="btn-ghost shrink-0 !rounded-xl !px-4" aria-label="إضافة"><Plus className="h-4 w-4" /></button>
        </div>
      )}
    </div>
  )
}

/** A page layout: drag (or the arrows) to reorder the sections, the eye to hide / show one. */
function SectionsField({ f, value, onChange }: { f: FieldSpec; value: SectionPick[]; onChange: (v: SectionPick[]) => void }) {
  const opts = f.options ?? []
  const label = (k: string) => opts.find((o) => o.value === k)?.label ?? k
  // saved order first, then any section not saved yet, in its default place
  const saved = value.filter((s) => opts.some((o) => o.value === s.key))
  const list: SectionPick[] = [...saved, ...opts.filter((o) => !saved.some((s) => s.key === o.value)).map((o) => ({ key: o.value, hidden: false }))]
  const move = (i: number, d: number) => {
    const j = i + d
    if (j < 0 || j >= list.length) return
    const next = [...list]
    ;[next[i], next[j]] = [next[j], next[i]]
    onChange(next)
  }
  return (
    <Reorder.Group axis="y" values={list} onReorder={onChange} className="space-y-2">
      {list.map((s, i) => (
        <Reorder.Item key={s.key} value={s} className={cx('flex cursor-grab items-center gap-2 rounded-2xl border border-line bg-surface p-2.5 select-none active:cursor-grabbing', s.hidden && 'opacity-55')}>
          <GripVertical className="h-4 w-4 shrink-0 text-muted" />
          <span className="grid h-7 w-7 shrink-0 place-items-center rounded-lg bg-brand-50 font-display text-xs font-bold text-brand-700">{i + 1}</span>
          <span className={cx('flex-1 text-sm font-semibold', s.hidden ? 'text-muted line-through' : 'text-ink')}>{label(s.key)}</span>
          <button type="button" onClick={() => move(i, -1)} disabled={i === 0} className="rounded-lg p-1.5 text-muted hover:bg-sand hover:text-ink disabled:opacity-30" aria-label="لأعلى"><ChevronUp className="h-4 w-4" /></button>
          <button type="button" onClick={() => move(i, 1)} disabled={i === list.length - 1} className="rounded-lg p-1.5 text-muted hover:bg-sand hover:text-ink disabled:opacity-30" aria-label="لأسفل"><ChevronDown className="h-4 w-4" /></button>
          <button type="button" onClick={() => onChange(list.map((x) => (x.key === s.key ? { ...x, hidden: !x.hidden } : x)))} className={cx('rounded-lg p-1.5', s.hidden ? 'text-muted hover:bg-sand' : 'text-brand-600 hover:bg-brand-50')} aria-label={s.hidden ? 'إظهار' : 'إخفاء'} title={s.hidden ? 'مخفي، اضغط للإظهار' : 'ظاهر، اضغط للإخفاء'}>
            {s.hidden ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
          </button>
        </Reorder.Item>
      ))}
    </Reorder.Group>
  )
}

function ColorField({ value, onChange, placeholder }: { value: string; onChange: (v: string | null) => void; placeholder?: string }) {
  const valid = /^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(value)
  const full = valid && value.length === 4 ? `#${[...value.slice(1)].map((c) => c + c).join('')}` : value
  return (
    <div className="flex items-center gap-2">
      <label className="relative h-11 w-14 shrink-0 cursor-pointer overflow-hidden rounded-xl border border-line" style={{ background: valid ? value : undefined }} title={placeholder}>
        <input type="color" className="absolute inset-0 h-full w-full cursor-pointer opacity-0" value={valid ? full : '#3f7a58'} onChange={(e) => onChange(e.target.value)} />
        {!valid && <span className="pattern absolute inset-0" />}
      </label>
      <input className="input font-mono" dir="ltr" value={value} onChange={(e) => onChange(e.target.value.trim() || null)} placeholder={placeholder} maxLength={7} />
      {value && <button type="button" onClick={() => onChange(null)} className="rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600" aria-label="مسح"><X className="h-4 w-4" /></button>}
    </div>
  )
}

function PairsField({ f, value, onChange }: { f: FieldSpec; value: TextPair[]; onChange: (v: TextPair[]) => void }) {
  const set = (i: number, patch: Partial<TextPair>) => onChange(value.map((x, j) => (j === i ? { ...x, ...patch } : x)))
  const max = f.maxItems ?? 20
  return (
    <div className="space-y-3">
      {value.map((p, i) => (
        <div key={i} className="grid gap-2 rounded-2xl border border-line bg-surface p-3 sm:grid-cols-[1fr_1fr_auto] sm:items-start">
          <label className="block">
            <span className="mb-1 block text-xs font-semibold text-muted">{f.labelA}</span>
            <textarea className="input min-h-[2.75rem] resize-y" rows={1} value={p.a} onChange={(e) => set(i, { a: e.target.value })} placeholder={f.placeholderA} />
          </label>
          <label className="block">
            <span className="mb-1 block text-xs font-semibold text-muted">{f.labelB}</span>
            <textarea className="input min-h-[2.75rem] resize-y" rows={1} value={p.b ?? ''} onChange={(e) => set(i, { b: e.target.value })} placeholder={f.placeholderB} dir="auto" />
          </label>
          <button type="button" onClick={() => onChange(value.filter((_, j) => j !== i))} className="self-end rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600 sm:mt-6" aria-label="حذف"><Trash2 className="h-4 w-4" /></button>
        </div>
      ))}
      {value.length < max && (
        <button type="button" onClick={() => onChange([...value, { a: '', b: '' }])} className="btn-ghost !rounded-xl !px-4 !py-2 text-sm">
          <Plus className="h-4 w-4" /> إضافة
        </button>
      )}
    </div>
  )
}

function StatsField({ value, onChange, max }: { value: StatItem[]; onChange: (v: StatItem[]) => void; max: number }) {
  const set = (i: number, patch: Partial<StatItem>) => onChange(value.map((x, j) => (j === i ? { ...x, ...patch } : x)))
  return (
    <div className="space-y-3">
      {value.map((s, i) => (
        <div key={i} className="grid grid-cols-[1fr_2fr_auto_auto] items-center gap-2 rounded-2xl border border-line bg-surface p-3">
          <input className="input tabular-nums" type="number" min={0} value={s.value} onChange={(e) => set(i, { value: Number(e.target.value) || 0 })} aria-label="الرقم" />
          <input className="input" value={s.label} onChange={(e) => set(i, { label: e.target.value })} placeholder="مثال: متدربة" aria-label="الوصف" />
          <input className="input w-16 text-center" value={s.suffix} maxLength={4} onChange={(e) => set(i, { suffix: e.target.value })} aria-label="لاحقة" title="لاحقة مثل + أو %" />
          <button type="button" onClick={() => onChange(value.filter((_, j) => j !== i))} className="rounded-lg p-2 text-muted hover:bg-red-500/10 hover:text-red-600" aria-label="حذف"><Trash2 className="h-4 w-4" /></button>
        </div>
      ))}
      {value.length < max && (
        <button type="button" onClick={() => onChange([...value, { value: 0, label: '', suffix: '+' }])} className="btn-ghost !rounded-xl !px-4 !py-2 text-sm">
          <Plus className="h-4 w-4" /> إضافة عداد
        </button>
      )}
      <p className="text-xs text-muted">العدادات بقيمة 0 لا تظهر في الموقع. أعداد الدبلومات والدورات تُحسب تلقائيًا.</p>
    </div>
  )
}
