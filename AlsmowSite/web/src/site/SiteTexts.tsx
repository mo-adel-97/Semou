import { useQuery, useQueryClient } from '@tanstack/react-query'
import { Check, Compass, MousePointerClick, PencilLine, Save, Undo2, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useRef, useState, useSyncExternalStore } from 'react'
import { createPortal } from 'react-dom'
import { useLocation } from 'react-router-dom'
import { confirmDanger, toast } from '../lib/alerts'
import { ApiError, api } from '../lib/api'
import { cx } from '../lib/format'
import { useSite } from '../lib/queries'
import { ADD_STYLES, applyTextRules, originalAttr, originalOf, refreshAdds, upsertRule, type AddStyle, type TextRule } from '../lib/textOverrides'
import type { User } from '../lib/types'

/* =====================================================================
   Every text of the site is editable without touching the code:
   - saved rules (settings group "texts") are applied for every visitor;
   - a signed-in editor gets «وضع التعديل»: click any text on any page to
     change / hide / add next to it, preview live, then «حفظ للجميع».
   ===================================================================== */

/* ---------- draft (unsaved changes), kept across reloads of the tab ---------- */
const DRAFT_KEY = 'alsmow.texts.draft'
const EDIT_KEY = 'alsmow.texts.edit'
export const EDITOR_FLAG = 'alsmow.editor' // set on dashboard login: only then does the site ask "who am I?"

type Store = { draft: TextRule[] | null; editing: boolean }
let store: Store = {
  draft: (() => { try { const s = sessionStorage.getItem(DRAFT_KEY); return s ? (JSON.parse(s) as TextRule[]) : null } catch { return null } })(),
  editing: (() => { try { return sessionStorage.getItem(EDIT_KEY) === '1' } catch { return false } })(),
}
const subs = new Set<() => void>()
function update(p: Partial<Store>) {
  store = { ...store, ...p }
  try {
    if (store.draft) sessionStorage.setItem(DRAFT_KEY, JSON.stringify(store.draft)); else sessionStorage.removeItem(DRAFT_KEY)
    if (store.editing) sessionStorage.setItem(EDIT_KEY, '1'); else sessionStorage.removeItem(EDIT_KEY)
  } catch { /* storage blocked: works for this page view only */ }
  subs.forEach((f) => f())
}
const useStore = () => useSyncExternalStore((f) => { subs.add(f); return () => subs.delete(f) }, () => store)

/** The signed-in dashboard user (only asked when this browser has logged in to the dashboard before). */
function useEditor() {
  const flagged = (() => { try { return localStorage.getItem(EDITOR_FLAG) === '1' } catch { return false } })()
  return useQuery({
    queryKey: ['me'],
    enabled: flagged,
    retry: false,
    staleTime: 5 * 60_000,
    queryFn: async () => {
      try { return (await api.get<{ user: User }>('/auth/me')).user } catch (e) {
        if (e instanceof ApiError && e.status === 401) { try { localStorage.removeItem(EDITOR_FLAG) } catch { /* */ } return null }
        throw e
      }
    },
  })
}

/* ---------- applies the rules everywhere (mounted once in the site layout) ---------- */
export default function SiteTexts() {
  const { data } = useSite()
  const { draft } = useStore()
  const saved = data?.settings.texts?.rules
  const rules = draft ?? saved ?? []
  const { pathname } = useLocation()
  const me = useEditor().data

  useEffect(() => { applyTextRules(rules) }, [rules])
  // added texts belong to a page: re-place them after navigating
  useEffect(() => { const t = setTimeout(refreshAdds, 60); return () => clearTimeout(t) }, [pathname])
  // /?edit opens edit mode directly (the dashboard's «تعديل الموقع مباشرة» button)
  useEffect(() => {
    if (me && new URLSearchParams(location.search).has('edit') && !store.editing) update({ editing: true })
  }, [me])

  if (!me) return null
  return <EditMode saved={saved ?? []} />
}

/* ---------- edit mode ---------- */
const FREE = '[data-picker-ui], .swal2-container'
const esc = (s: string) => s.trim()

type Hit = { original: string; shown: string; rect: DOMRect }
function hit(x: number, y: number): Hit | null {
  const el = document.elementFromPoint(x, y)
  if (!el || el.closest(FREE)) return null
  const doc = document as Document & {
    caretPositionFromPoint?: (x: number, y: number) => { offsetNode: Node } | null
    caretRangeFromPoint?: (x: number, y: number) => Range | null
  }
  const node = doc.caretPositionFromPoint?.(x, y)?.offsetNode ?? doc.caretRangeFromPoint?.(x, y)?.startContainer
  if (node && node.nodeType === Node.TEXT_NODE && (node as Text).data.trim() && el.contains(node) && !(node.parentElement?.closest('[data-tx-add]'))) {
    const r = document.createRange()
    r.selectNodeContents(node)
    const rect = r.getBoundingClientRect()
    if (x >= rect.left - 3 && x <= rect.right + 3 && y >= rect.top - 3 && y <= rect.bottom + 3)
      return { original: esc(originalOf(node)), shown: esc((node as Text).data), rect }
  }
  // beside the words but inside an element with one text (a button, a heading): that text
  const box = el.closest('a, button, h1, h2, h3, h4, h5, h6, p, li, label, span, div') ?? el
  const own = [...box.childNodes].filter((c) => c.nodeType === Node.TEXT_NODE && (c as Text).data.trim())
  if (own.length === 1) return { original: esc(originalOf(own[0])), shown: esc((own[0] as Text).data), rect: box.getBoundingClientRect() }
  const field = el.closest('input[placeholder], textarea[placeholder]')
  if (field) return { original: esc(originalAttr(field, 'placeholder') ?? ''), shown: field.getAttribute('placeholder') ?? '', rect: field.getBoundingClientRect() }
  const img = el.closest('img[alt]')
  if (img && img.getAttribute('alt')?.trim()) return { original: esc(originalAttr(img, 'alt') ?? ''), shown: img.getAttribute('alt') ?? '', rect: img.getBoundingClientRect() }
  return null
}

function EditMode({ saved }: { saved: TextRule[] }) {
  const { draft, editing } = useStore()
  const qc = useQueryClient()
  const [box, setBox] = useState<DOMRect | null>(null)
  const [browsing, setBrowsing] = useState(false)
  const [picked, setPicked] = useState<Hit | null>(null)
  const [saving, setSaving] = useState(false)
  const raf = useRef(0)
  const rules = draft ?? saved
  const changes = draft ? diffCount(saved, draft) : 0

  useEffect(() => {
    if (!editing || browsing || picked) { setBox(null); document.documentElement.classList.remove('tx-picking'); return }
    document.documentElement.classList.add('tx-picking')
    const move = (e: PointerEvent) => {
      cancelAnimationFrame(raf.current)
      raf.current = requestAnimationFrame(() => setBox(hit(e.clientX, e.clientY)?.rect ?? null))
    }
    const block = (e: Event) => {
      if ((e as MouseEvent).altKey || (e.target as Element)?.closest?.(FREE)) return
      e.preventDefault()
      e.stopPropagation()
    }
    const click = (e: MouseEvent) => {
      if (e.altKey || (e.target as Element)?.closest?.(FREE)) return
      e.preventDefault()
      e.stopPropagation()
      const h = hit(e.clientX, e.clientY)
      if (h?.original) { setBox(null); setPicked(h) }
    }
    const key = (e: KeyboardEvent) => { if (e.key === 'Escape') update({ editing: false }) }
    const evs = ['pointerdown', 'mousedown', 'mouseup', 'pointerup', 'dblclick', 'submit', 'touchstart'] as const
    window.addEventListener('pointermove', move, true)
    for (const t of evs) window.addEventListener(t, block, { capture: true, passive: false })
    window.addEventListener('click', click, true)
    window.addEventListener('keydown', key, true)
    return () => {
      document.documentElement.classList.remove('tx-picking')
      window.removeEventListener('pointermove', move, true)
      for (const t of evs) window.removeEventListener(t, block, true)
      window.removeEventListener('click', click, true)
      window.removeEventListener('keydown', key, true)
    }
  }, [editing, browsing, picked])

  // read the store at call time: one dialog may apply two rules in a row
  const apply = (r: TextRule) => update({ draft: upsertRule(store.draft ?? saved, r) })
  const save = async () => {
    if (!draft) return
    setSaving(true)
    try {
      await api.put('/admin/settings/texts', { rules: draft })
      await qc.invalidateQueries({ queryKey: ['site'] })
      update({ draft: null })
      toast.success('تم الحفظ، وظهرت التعديلات لكل الزوار')
    } catch (e) {
      toast.error((e as Error).message || 'تعذّر الحفظ')
    } finally {
      setSaving(false)
    }
  }
  const discard = async () => {
    if (await confirmDanger('تجاهل التعديلات غير المحفوظة؟', 'سيعود الموقع كما هو محفوظ.', 'نعم، تجاهل')) update({ draft: null })
  }

  if (!editing) {
    return (
      <button
        data-picker-ui
        data-no-texts
        onClick={() => update({ editing: true })}
        className="fixed bottom-5 left-1/2 z-[70] flex -translate-x-1/2 items-center gap-2 rounded-full bg-deep-950 px-4 py-2.5 text-sm font-bold text-white shadow-lift ring-1 ring-white/10 transition hover:bg-deep-900"
      >
        <PencilLine className="h-4 w-4 text-gold-300" /> وضع التعديل
        {draft && <span className="rounded-full bg-gold-400 px-2 text-xs text-deep-950">غير محفوظ</span>}
      </button>
    )
  }

  return (
    <>
      {box && <div className="tx-box" data-picker-ui style={{ top: box.top - 4, left: box.left - 5, width: box.width + 10, height: box.height + 8 }} />}
      <div className="tx-bar" data-picker-ui data-no-texts>
        <span className={cx('tx-bar-ic', browsing && 'browse')}>{browsing ? <Compass className="h-4 w-4" /> : <MousePointerClick className="h-4 w-4" />}</span>
        <div className="min-w-0 flex-1 leading-tight">
          <b className="block text-sm">{browsing ? 'وضع التصفح' : 'وضع التعديل'}</b>
          <small className="block truncate text-xs text-white/60">
            {browsing ? 'تنقّل في الموقع عادي، ثم «متابعة التعديل»' : 'اضغط على أي نص لتعديله • «تصفح» للتنقل بين الصفحات'}
          </small>
        </div>
        <button className="tx-btn" onClick={() => setBrowsing((b) => !b)}>
          {browsing ? <><MousePointerClick className="h-4 w-4" /><span>متابعة التعديل</span></> : <><Compass className="h-4 w-4" /><span>تصفح</span></>}
        </button>
        {draft && (
          <>
            <span className="tx-count">{changes ? `${changes.toLocaleString('ar-EG')} تعديل` : 'غير محفوظ'}</span>
            <button className="tx-btn gold" onClick={save} disabled={saving}><Save className="h-4 w-4" /><span>حفظ للجميع</span></button>
            <button className="tx-btn ghost" onClick={discard} title="تجاهل التعديلات"><Undo2 className="h-4 w-4" /></button>
          </>
        )}
        <button className="tx-btn ghost" title="إنهاء وضع التعديل" onClick={() => update({ editing: false })}><X className="h-4 w-4" /></button>
      </div>
      <AnimatePresence>{picked && <EditDialog hit={picked} rules={rules} onClose={() => setPicked(null)} onApply={(r) => { apply(r); setPicked(null) }} />}</AnimatePresence>
    </>
  )
}

function diffCount(a: TextRule[], b: TextRule[]) {
  const ka = new Set(a.map((r) => JSON.stringify(r)))
  const kb = new Set(b.map((r) => JSON.stringify(r)))
  let n = 0
  for (const k of kb) if (!ka.has(k)) n++
  for (const k of ka) if (!kb.has(k)) n++
  return n
}

/* ---------- the dialog for one clicked text ---------- */
type Act = 'replace' | 'add' | 'text' | 'line'
function EditDialog({ hit: h, rules, onClose, onApply }: { hit: Hit; rules: TextRule[]; onClose: () => void; onApply: (r: TextRule) => void }) {
  const cur = rules.find((r) => !r.add && r.from.trim() === h.original)
  const added = rules.filter((r) => r.add && r.from.trim() === h.original && (!r.path || r.path === location.pathname))
  const [act, setAct] = useState<Act>(cur?.whole ? 'line' : cur?.remove ? 'text' : 'replace')
  const [to, setTo] = useState(cur && !cur.remove ? cur.to : h.shown)
  const [mode, setMode] = useState<'exact' | 'contains'>(cur?.mode ?? 'exact')
  const [where, setWhere] = useState<'before' | 'after'>('after')
  const existing = added.find((a) => a.add === where)
  const [text, setText] = useState(existing?.to ?? '')
  const [style, setStyle] = useState<AddStyle>(existing?.style ?? 'note')
  const [allPages, setAllPages] = useState(existing ? !existing.path : false)
  const [touched, setTouched] = useState(false)
  useEffect(() => {
    if (touched) return
    const a = added.find((x) => x.add === where)
    setText(a?.to ?? ''); if (a?.style) setStyle(a.style); setAllPages(a ? !a.path : false)
  }, [where]) // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => {
    const k = (e: KeyboardEvent) => { if (e.key === 'Escape') { e.stopPropagation(); onClose() } }
    window.addEventListener('keydown', k, true)
    return () => window.removeEventListener('keydown', k, true)
  }, [onClose])

  const submit = () => {
    if (act === 'add') {
      // remove the old one if its page scope changed, then add
      const old = added.find((a) => a.add === where)
      if (old && (!!old.path) === allPages) onApply({ ...old, to: '' })
      onApply({ from: h.original, to: text.trim(), mode: 'exact', add: where, style, ...(allPages ? {} : { path: location.pathname }) })
      return
    }
    onApply({ from: h.original, to: act === 'replace' ? to.trim() : '', mode: act === 'line' ? 'exact' : mode, ...(act !== 'replace' ? { remove: true } : {}), ...(act === 'line' ? { whole: true } : {}) })
  }
  const restore = () => onApply({ from: h.original, to: h.original, mode: 'exact' })

  const acts: [Act, string][] = [['replace', 'تغيير النص'], ['add', 'إضافة نص هنا'], ['text', 'إخفاء النص'], ['line', 'إخفاء السطر / الزر']]
  return createPortal(
    <motion.div data-picker-ui data-no-texts className="fixed inset-0 z-[90] flex items-end justify-center bg-deep-950/50 p-0 backdrop-blur-[2px] sm:items-center sm:p-6" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onMouseDown={onClose}>
      <motion.div
        role="dialog"
        aria-label="تعديل النص"
        onMouseDown={(e) => e.stopPropagation()}
        initial={{ y: 40 }}
        animate={{ y: 0 }}
        exit={{ y: 40 }}
        transition={{ type: 'spring', damping: 30, stiffness: 340 }}
        className="max-h-[92dvh] w-full max-w-xl overflow-y-auto rounded-t-3xl bg-surface p-5 shadow-lift sm:rounded-3xl sm:p-6"
      >
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-extrabold">تعديل النص</h2>
          <button onClick={onClose} className="grid h-9 w-9 place-items-center rounded-xl text-muted hover:bg-bg" aria-label="إغلاق"><X className="h-5 w-5" /></button>
        </div>
        <div className="mb-4 rounded-2xl border border-line bg-bg px-4 py-3 text-[15px] font-semibold text-ink-2">{h.original}</div>

        <div className="mb-4 grid grid-cols-2 gap-2 sm:grid-cols-4">
          {acts.map(([k, l]) => (
            <button key={k} onClick={() => setAct(k)} className={cx('rounded-xl border px-2 py-2.5 text-[13px] font-bold transition', act === k ? 'border-brand-500 bg-brand-50 text-brand-700 ring-2 ring-brand-500/15' : 'border-line text-ink-2 hover:bg-bg')}>
              {l}
            </button>
          ))}
        </div>

        {act === 'replace' && (
          <div className="space-y-3">
            <div>
              <label className="label">النص الجديد</label>
              <textarea autoFocus className="input min-h-20 resize-y" rows={2} maxLength={600} value={to} onChange={(e) => setTo(e.target.value)} />
            </div>
            <div>
              <label className="label">أين يتغيّر</label>
              <select className="input" value={mode} onChange={(e) => setMode(e.target.value as 'exact' | 'contains')}>
                <option value="exact">في كل مكان يظهر فيه هذا النص بالضبط (موصى به)</option>
                <option value="contains">هذه العبارة أينما وردت حتى داخل جمل أطول</option>
              </select>
            </div>
          </div>
        )}
        {act === 'add' && (
          <div className="space-y-3">
            <div>
              <label className="label">النص المضاف</label>
              <textarea autoFocus className="input min-h-24 resize-y" rows={3} maxLength={600} placeholder="اكتب النص الذي تريد إضافته…" value={text} onChange={(e) => { setText(e.target.value); setTouched(true) }} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="label">مكانه</label>
                <select className="input" value={where} onChange={(e) => setWhere(e.target.value as 'before' | 'after')}>
                  <option value="after">بعد هذا السطر</option>
                  <option value="before">قبل هذا السطر</option>
                </select>
              </div>
              <div>
                <label className="label">شكله</label>
                <select className="input" value={style} onChange={(e) => setStyle(e.target.value as AddStyle)}>
                  {(Object.keys(ADD_STYLES) as AddStyle[]).map((k) => <option key={k} value={k}>{ADD_STYLES[k]}</option>)}
                </select>
              </div>
            </div>
            <label className="flex cursor-pointer items-center gap-2 text-sm font-semibold text-ink-2">
              <input type="checkbox" className="h-4 w-4 accent-[var(--primary)]" checked={allPages} onChange={(e) => setAllPages(e.target.checked)} />
              يظهر في كل الصفحات التي فيها هذا السطر (وليس هذه الصفحة فقط)
            </label>
            {added.length > 0 && <p className="text-xs text-muted">لهذا السطر نصوص مضافة؛ اختر نفس المكان لتعديلها، أو امسح النص لحذفها.</p>}
          </div>
        )}
        {(act === 'text' || act === 'line') && (
          <p className="rounded-2xl bg-gold-400/10 px-4 py-3 text-sm text-ink-2">
            {act === 'text' ? 'سيختفي هذا النص من كل صفحات الموقع.' : 'سيختفي السطر أو الزر الذي يحمل هذا النص بالكامل من كل صفحات الموقع.'} يمكنك إرجاعه في أي وقت.
          </p>
        )}

        <div className="mt-5 flex flex-wrap items-center gap-2">
          <button onClick={submit} className="btn-primary !px-6 !py-2.5 text-sm"><Check className="h-4 w-4" /> اعتماد</button>
          <button onClick={onClose} className="btn-ghost !px-5 !py-2.5 text-sm">إلغاء</button>
          {cur && act !== 'add' && <button onClick={restore} className="ms-auto text-sm font-bold text-brand-700 hover:underline">إرجاع النص الأصلي</button>}
        </div>
        <p className="mt-3 text-xs text-muted">التعديل يظهر لك فورًا كمعاينة، ولا يراه الزوار إلا بعد «حفظ للجميع».</p>
      </motion.div>
    </motion.div>,
    document.body,
  )
}
