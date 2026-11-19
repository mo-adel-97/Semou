import { useMutation, useQueryClient } from '@tanstack/react-query'
import { EyeOff, Loader2, PencilLine, Plus, Replace, Save, Search, Trash2, Undo2 } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { confirmDanger, toast } from '../lib/alerts'
import { api } from '../lib/api'
import { cx } from '../lib/format'
import { ADD_STYLES, upsertRule, type TextRule } from '../lib/textOverrides'
import type { Settings } from '../lib/types'

/** «نصوص الموقع»: every text changed, hidden or added on the site, in one list to review, edit or revert. */
export default function TextsManager({ rules: saved }: { rules: TextRule[] }) {
  const qc = useQueryClient()
  const [rules, setRules] = useState<TextRule[]>(saved)
  const [q, setQ] = useState('')
  const [kind, setKind] = useState<'all' | 'edit' | 'hide' | 'add'>('all')
  useEffect(() => setRules(saved), [saved])
  const dirty = JSON.stringify(rules) !== JSON.stringify(saved)

  const save = useMutation({
    mutationFn: () => api.put<{ rules: TextRule[] }>('/admin/settings/texts', { rules }),
    onSuccess: (r) => {
      qc.setQueryData<Settings>(['admin', 'settings'], (old) => (old ? { ...old, texts: r } : old))
      qc.invalidateQueries({ queryKey: ['site'] })
      toast.success('تم حفظ نصوص الموقع')
    },
    onError: (e) => toast.error((e as Error).message),
  })

  const kindOf = (r: TextRule) => (r.add ? 'add' : r.remove ? 'hide' : 'edit')
  const counts = useMemo(() => ({ all: rules.length, edit: rules.filter((r) => kindOf(r) === 'edit').length, hide: rules.filter((r) => kindOf(r) === 'hide').length, add: rules.filter((r) => kindOf(r) === 'add').length }), [rules])
  const shown = rules.filter((r) => (kind === 'all' || kindOf(r) === kind) && (!q.trim() || `${r.from} ${r.to}`.includes(q.trim())))

  const setTo = (r: TextRule, to: string) => setRules((x) => x.map((y) => (y === r ? { ...y, to } : y)))
  const remove = (r: TextRule) => setRules((x) => x.filter((y) => y !== r))
  const addManual = () => setRules((x) => upsertRule(x, { from: 'النص كما يظهر في الموقع', to: 'النص الجديد', mode: 'exact' }))

  return (
    <div className="card overflow-hidden">
      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-line px-6 py-4">
        <div>
          <h2 className="text-lg font-bold">نصوص الموقع</h2>
          <p className="text-sm text-muted">أي كلمة أو زر أو عنوان في الموقع يتغيّر من هنا أو بالضغط عليه في الموقع نفسه، بدون أي تعديل في الكود.</p>
        </div>
        <div className="flex flex-wrap gap-2">
          {dirty && <button onClick={() => setRules(saved)} className="btn-ghost !px-4 !py-2 text-sm"><Undo2 className="h-4 w-4" /> تراجع</button>}
          <button onClick={() => save.mutate()} disabled={!dirty || save.isPending} className="btn-primary !px-5 !py-2 text-sm">
            {save.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />} حفظ
          </button>
        </div>
      </div>

      <div className="p-6">
        <a href="/?edit" target="_blank" rel="noopener" className="group mb-6 flex items-center gap-4 rounded-2xl bg-deep-950 p-5 text-white transition hover:bg-deep-900">
          <span className="grid h-12 w-12 shrink-0 place-items-center rounded-2xl bg-white/10"><PencilLine className="h-6 w-6 text-gold-300" /></span>
          <span className="min-w-0 flex-1">
            <b className="block text-base">تعديل الموقع مباشرة</b>
            <span className="block text-sm text-white/65">يفتح الموقع في «وضع التعديل»: اضغط على أي نص لتغييره أو إخفائه أو إضافة نص بجواره، ثم «حفظ للجميع».</span>
          </span>
          <span className="hidden rounded-full bg-gold-400 px-4 py-2 text-sm font-bold text-deep-950 sm:block">افتح الموقع</span>
        </a>

        <div className="mb-4 flex flex-wrap items-center gap-2">
          <div className="relative min-w-0 flex-1 sm:max-w-xs">
            <Search className="pointer-events-none absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2 text-muted" />
            <input className="input !pr-9" placeholder="بحث في النصوص…" value={q} onChange={(e) => setQ(e.target.value)} />
          </div>
          {([['all', 'الكل'], ['edit', 'معدّلة'], ['hide', 'مخفية'], ['add', 'مضافة']] as const).map(([k, l]) => (
            <button key={k} onClick={() => setKind(k)} className={cx('rounded-full px-3.5 py-1.5 text-sm font-semibold transition', kind === k ? 'bg-primary text-white' : 'bg-bg text-ink-2 hover:bg-sand')}>
              {l} <span className="opacity-70">({counts[k].toLocaleString('ar-EG')})</span>
            </button>
          ))}
          <button onClick={addManual} className="btn-ghost ms-auto !px-3.5 !py-1.5 text-sm"><Plus className="h-4 w-4" /> قاعدة يدوية</button>
        </div>

        {shown.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-line p-10 text-center text-muted">
            {rules.length === 0 ? 'لم يُعدَّل أي نص بعد. افتح «تعديل الموقع مباشرة» واضغط على أي نص.' : 'لا توجد نصوص مطابقة'}
          </div>
        ) : (
          <ul className="divide-y divide-line overflow-hidden rounded-2xl border border-line">
            {shown.map((r, i) => {
              const k = kindOf(r)
              const manual = !r.add && !r.remove
              return (
                <li key={`${k}|${r.path ?? ''}|${r.from}|${i}`} className="grid gap-3 p-4 md:grid-cols-[1fr_1fr_auto] md:items-start">
                  <div className="min-w-0">
                    <div className="mb-1 flex flex-wrap items-center gap-1.5 text-xs">
                      {k === 'edit' && <span className="inline-flex items-center gap-1 rounded-full bg-brand-50 px-2 py-0.5 font-bold text-brand-700"><Replace className="h-3 w-3" /> {r.mode === 'contains' ? 'استبدال عبارة' : 'تغيير'}</span>}
                      {k === 'hide' && <span className="inline-flex items-center gap-1 rounded-full bg-red-500/10 px-2 py-0.5 font-bold text-red-600"><EyeOff className="h-3 w-3" /> {r.whole ? 'إخفاء السطر' : 'إخفاء النص'}</span>}
                      {k === 'add' && <span className="inline-flex items-center gap-1 rounded-full bg-gold-400/15 px-2 py-0.5 font-bold text-gold-700 dark:text-gold-300"><Plus className="h-3 w-3" /> {r.add === 'before' ? 'قبل' : 'بعد'} السطر · {ADD_STYLES[r.style ?? 'text']}</span>}
                      {r.add && <span className="text-muted">{r.path ? `في صفحة ${r.path}` : 'كل الصفحات'}</span>}
                    </div>
                    {manual && rules.length > 0 ? (
                      <input className="input !h-10 text-sm" value={r.from} onChange={(e) => setRules((x) => x.map((y) => (y === r ? { ...y, from: e.target.value } : y)))} aria-label="النص الأصلي" />
                    ) : (
                      <div className={cx('text-sm font-semibold', k === 'hide' ? 'text-muted line-through' : 'text-ink-2')}>{r.from}</div>
                    )}
                  </div>
                  <div className="min-w-0">
                    {k === 'hide' ? (
                      <span className="text-sm text-muted">مخفي من الموقع</span>
                    ) : (
                      <textarea className="input min-h-10 resize-y text-sm" rows={1} value={r.to} onChange={(e) => setTo(r, e.target.value)} aria-label="النص الجديد" />
                    )}
                  </div>
                  <button
                    onClick={async () => { if (await confirmDanger('إرجاع هذا النص كما كان؟', r.from, 'نعم، أرجعه')) remove(r) }}
                    className="justify-self-end rounded-xl p-2 text-muted transition hover:bg-red-500/10 hover:text-red-600"
                    title={k === 'add' ? 'حذف النص المضاف' : 'إرجاع النص الأصلي'}
                  >
                    {k === 'add' ? <Trash2 className="h-4 w-4" /> : <Undo2 className="h-4 w-4" />}
                  </button>
                </li>
              )
            })}
          </ul>
        )}
      </div>
    </div>
  )
}
