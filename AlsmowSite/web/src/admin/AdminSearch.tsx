import { useQuery } from '@tanstack/react-query'
import { CornerDownLeft, FileText, Loader2, Search, Settings, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useMemo, useRef, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { api, qs } from '../lib/api'
import { cx } from '../lib/format'
import { ICONS } from './AdminApp'
import { useSchema } from './auth'

/* =====================================================================
   Dashboard search: type any text you see on the site, get the exact
   field that holds it (site settings or any content item), open it with
   the text selected.
   ===================================================================== */

interface SettingHit { group: string; groupLabel: string; field: string; label: string; snippet: string; byValue: boolean }
interface ContentHit { res: string; resLabel: string; id: number; title: string; field: string; label: string; snippet: string; published: boolean | null }
interface SearchResult { q: string; settings: SettingHit[]; content: ContentHit[] }

type Item = { key: string; to: string; icon: typeof Search; where: string; title: string; snippet: string; muted?: boolean }

/** Same idea as the server's search_norm, but 1:1 per character so match positions stay valid. */
const MAP: Record<string, string> = { 'أ': 'ا', 'إ': 'ا', 'آ': 'ا', 'ٱ': 'ا', 'ة': 'ه', 'ى': 'ي', 'ؤ': 'و', 'ئ': 'ي' }
const HARAKAT = /[ً-ٰٟـ]/
function normChars(s: string) {
  // returns normalised text + index map back to the original string
  let out = ''
  const idx: number[] = []
  for (let i = 0; i < s.length; i++) {
    const c = s[i]
    if (HARAKAT.test(c)) continue
    out += (MAP[c] ?? c).toLowerCase()
    idx.push(i)
  }
  return { out, idx }
}
function findIn(text: string, q: string): [number, number] | null {
  const nq = normChars(q.trim().replace(/\s+/g, ' ')).out
  if (!nq) return null
  const { out, idx } = normChars(text)
  const p = out.indexOf(nq)
  if (p < 0) return null
  return [idx[p], idx[p + nq.length - 1] + 1]
}

function Highlight({ text, q }: { text: string; q: string }) {
  const m = findIn(text, q)
  if (!m) return <>{text}</>
  return (
    <>
      {text.slice(0, m[0])}
      <mark className="rounded bg-gold-300/60 px-0.5 text-ink dark:bg-gold-400/35">{text.slice(m[0], m[1])}</mark>
      {text.slice(m[1])}
    </>
  )
}

function useDebounced<T>(v: T, ms: number) {
  const [d, setD] = useState(v)
  useEffect(() => { const t = setTimeout(() => setD(v), ms); return () => clearTimeout(t) }, [v, ms])
  return d
}

/* ---------------- header trigger + palette ---------------- */

export default function AdminSearch() {
  const [open, setOpen] = useState(false)
  useEffect(() => {
    const k = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') { e.preventDefault(); setOpen((o) => !o) }
      else if (e.key === '/' && !/^(INPUT|TEXTAREA|SELECT)$/.test((e.target as HTMLElement).tagName) && !(e.target as HTMLElement).isContentEditable) { e.preventDefault(); setOpen(true) }
    }
    window.addEventListener('keydown', k)
    return () => window.removeEventListener('keydown', k)
  }, [])

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="flex h-10 min-w-0 flex-1 items-center gap-2.5 rounded-xl border border-line bg-surface px-3 text-right text-sm text-muted shadow-soft transition hover:border-brand-300 hover:text-ink-2 sm:max-w-md"
        aria-label="البحث في محتوى الموقع"
      >
        <Search className="h-4 w-4 shrink-0" />
        <span className="flex-1 truncate">ابحث عن أي نص في الموقع…</span>
        <kbd className="hidden rounded-md border border-line bg-bg px-1.5 py-0.5 font-sans text-[11px] text-muted sm:inline" dir="ltr">Ctrl K</kbd>
      </button>
      <AnimatePresence>{open && <Palette onClose={() => setOpen(false)} />}</AnimatePresence>
    </>
  )
}

function Palette({ onClose }: { onClose: () => void }) {
  const [q, setQ] = useState('')
  const [active, setActive] = useState(0)
  const dq = useDebounced(q.trim(), 280)
  const nav = useNavigate()
  const schema = useSchema()
  const inputRef = useRef<HTMLInputElement>(null)
  const listRef = useRef<HTMLDivElement>(null)

  const { data, isFetching, isError } = useQuery({
    queryKey: ['admin', 'search', dq],
    queryFn: () => api.get<SearchResult>(`/admin/search${qs({ q: dq })}`),
    enabled: dq.length >= 2,
    staleTime: 30_000,
    placeholderData: (prev) => prev,
  })

  // admin screens themselves ("الرسائل", "الشركاء"...) are searchable too
  const screens = useMemo<Item[]>(() => {
    if (dq.length < 2) return []
    const all: Item[] = [
      ...(schema.data?.resources ?? []).map((r) => ({ key: `scr-${r.name}`, to: `/admin/c/${r.name}`, icon: ICONS[r.icon] ?? FileText, where: 'قسم في لوحة التحكم', title: r.label, snippet: '' })),
      ...(schema.data?.settings ?? []).map((g) => ({ key: `scr-set-${g.group}`, to: `/admin/settings/${g.group}`, icon: ICONS[g.icon] ?? Settings, where: 'إعدادات الموقع', title: g.label, snippet: '' })),
      { key: 'scr-messages', to: '/admin/messages', icon: FileText, where: 'قسم في لوحة التحكم', title: 'الرسائل والتسجيلات', snippet: '' },
      { key: 'scr-media', to: '/admin/media', icon: FileText, where: 'قسم في لوحة التحكم', title: 'مكتبة الصور', snippet: '' },
    ]
    return all.filter((s) => findIn(s.title, dq))
  }, [dq, schema.data])

  const groups = useMemo(() => {
    const out: { title: string; items: Item[] }[] = []
    if (screens.length) out.push({ title: 'الأقسام', items: screens })
    if (data && data.q === dq) {
      const enc = (s: string) => encodeURIComponent(s)
      if (data.settings.length)
        out.push({
          title: 'إعدادات الموقع',
          items: data.settings.map((s) => ({
            key: `s-${s.group}-${s.field}`,
            to: `/admin/settings/${s.group}?f=${s.field}${s.byValue ? `&hl=${enc(dq)}` : ''}`,
            icon: ICONS[(schema.data?.settings ?? []).find((g) => g.group === s.group)?.icon ?? ''] ?? Settings,
            where: `${s.groupLabel} › ${s.label}`,
            title: s.label,
            snippet: s.snippet || (s.byValue ? '' : 'فارغ'),
            muted: !s.byValue,
          })),
        })
      const byRes = new Map<string, ContentHit[]>()
      for (const c of data.content) byRes.set(c.resLabel, [...(byRes.get(c.resLabel) ?? []), c])
      for (const [label, hits] of byRes)
        out.push({
          title: label,
          items: hits.map((c) => ({
            key: `c-${c.res}-${c.id}`,
            to: `/admin/c/${c.res}/${c.id}?f=${c.field}&hl=${enc(dq)}`,
            icon: ICONS[(schema.data?.resources ?? []).find((r) => r.name === c.res)?.icon ?? ''] ?? FileText,
            where: `${label} › ${c.label}${c.published === false ? ' · غير منشور' : ''}`,
            title: c.title,
            snippet: c.field === (schema.data?.resources ?? []).find((r) => r.name === c.res)?.title ? '' : c.snippet,
          })),
        })
    }
    return out
  }, [data, dq, screens, schema.data])

  const flat = useMemo(() => groups.flatMap((g) => g.items), [groups])
  useEffect(() => setActive(0), [dq])
  useEffect(() => { listRef.current?.querySelector(`[data-i="${active}"]`)?.scrollIntoView({ block: 'nearest' }) }, [active])
  useEffect(() => {
    const prev = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => { document.body.style.overflow = prev }
  }, [])

  const go = (it?: Item) => { if (!it) return; onClose(); nav(it.to) }
  const onKey = (e: React.KeyboardEvent) => {
    if (e.key === 'ArrowDown') { e.preventDefault(); setActive((a) => Math.min(flat.length - 1, a + 1)) }
    else if (e.key === 'ArrowUp') { e.preventDefault(); setActive((a) => Math.max(0, a - 1)) }
    else if (e.key === 'Enter') { e.preventDefault(); go(flat[active]) }
    else if (e.key === 'Escape') { e.preventDefault(); onClose() }
  }

  const waiting = dq.length >= 2 && (isFetching || data?.q !== dq)
  const total = (data?.q === dq ? data.settings.length + data.content.length : 0) + screens.length
  let i = -1

  return (
    <motion.div className="fixed inset-0 z-[60] flex items-start justify-center bg-deep-950/45 p-3 pt-[8vh] backdrop-blur-sm sm:p-6 sm:pt-[12vh]" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onMouseDown={onClose}>
      <motion.div
        role="dialog"
        aria-label="البحث في محتوى الموقع"
        className="flex max-h-[80dvh] w-full max-w-2xl flex-col overflow-hidden rounded-2xl border border-line bg-surface shadow-lift"
        initial={{ y: -12, scale: 0.98 }}
        animate={{ y: 0, scale: 1 }}
        exit={{ y: -8, scale: 0.98, opacity: 0 }}
        transition={{ duration: 0.18 }}
        onMouseDown={(e) => e.stopPropagation()}
      >
        <div className="flex items-center gap-3 border-b border-line px-4">
          {waiting ? <Loader2 className="h-5 w-5 shrink-0 animate-spin text-brand-600" /> : <Search className="h-5 w-5 shrink-0 text-muted" />}
          <input
            ref={inputRef}
            autoFocus
            value={q}
            onChange={(e) => setQ(e.target.value)}
            onKeyDown={onKey}
            placeholder="اكتب أي نص تراه في الموقع: عنوان، جملة، رقم هاتف…"
            className="h-14 min-w-0 flex-1 bg-transparent text-[15px] outline-none placeholder:text-muted"
            aria-controls="admin-search-list"
          />
          {q && <button onClick={() => { setQ(''); inputRef.current?.focus() }} className="rounded-lg p-1.5 text-muted hover:bg-bg hover:text-ink" aria-label="مسح"><X className="h-4 w-4" /></button>}
          <button onClick={onClose} className="hidden rounded-md border border-line px-1.5 py-0.5 text-[11px] text-muted sm:block">Esc</button>
        </div>

        <div ref={listRef} id="admin-search-list" className="flex-1 overflow-y-auto overscroll-contain p-2">
          {dq.length < 2 ? (
            <Hint />
          ) : isError ? (
            <p className="p-8 text-center text-sm text-red-600">تعذّر البحث، حاول مرة أخرى</p>
          ) : !waiting && total === 0 ? (
            <div className="p-8 text-center">
              <p className="font-bold text-ink-2">لا توجد نتائج لـ «{dq}»</p>
              <p className="mt-1 text-sm text-muted">جرّب كلمة أقصر أو جزءًا من الجملة. بعض النصوص الثابتة في تصميم الموقع لا تُعدَّل من اللوحة.</p>
            </div>
          ) : (
            groups.map((g) => (
              <div key={g.title} className="mb-2">
                <div className="sticky top-0 z-10 bg-surface/95 px-3 py-1.5 text-xs font-bold text-muted backdrop-blur">{g.title} <span className="font-normal">({g.items.length.toLocaleString('ar-EG')})</span></div>
                {g.items.map((it) => {
                  i++
                  const n = i
                  const Icon = it.icon
                  return (
                    <button
                      key={it.key}
                      data-i={n}
                      onMouseMove={() => setActive(n)}
                      onClick={() => go(it)}
                      className={cx('flex w-full items-start gap-3 rounded-xl px-3 py-2.5 text-right transition', active === n ? 'bg-brand-50 ring-1 ring-brand-200' : 'hover:bg-bg')}
                    >
                      <span className={cx('mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg', active === n ? 'bg-brand-600 text-white' : 'bg-bg text-ink-2')}><Icon className="h-4 w-4" /></span>
                      <span className="min-w-0 flex-1">
                        <span className="block truncate font-semibold text-ink"><Highlight text={it.title} q={dq} /></span>
                        {it.snippet && <span className={cx('mt-0.5 block text-sm leading-relaxed line-clamp-2', it.muted ? 'text-muted/70 italic' : 'text-ink-2')}><Highlight text={it.snippet} q={dq} /></span>}
                        <span className="mt-1 block truncate text-xs text-muted">{it.where}</span>
                      </span>
                      {active === n && <CornerDownLeft className="mt-2 hidden h-4 w-4 shrink-0 text-brand-600 sm:block" />}
                    </button>
                  )
                })}
              </div>
            ))
          )}
        </div>

        <div className="hidden items-center gap-4 border-t border-line bg-bg/60 px-4 py-2 text-xs text-muted sm:flex">
          <span><kbd className="font-sans">↑ ↓</kbd> للتنقل</span>
          <span><kbd className="font-sans">Enter</kbd> للفتح</span>
          <span className="flex-1" />
          {total > 0 && !waiting && <span>{total.toLocaleString('ar-EG')} نتيجة</span>}
        </div>
      </motion.div>
    </motion.div>
  )
}

function Hint() {
  return (
    <div className="p-6 text-sm text-muted">
      <p className="font-bold text-ink-2">ابحث عن أي نص لتعرف مكان تعديله</p>
      <ul className="mt-2 list-disc space-y-1 pr-5">
        <li>يبحث في كل إعدادات الموقع، والبرامج، والمقالات، والصفحات، وباقي المحتوى (بما فيه النص الكامل).</li>
        <li>لا يهم شكل الهمزة أو التاء المربوطة: «الاداره» تجد «الإدارة».</li>
        <li>اضغط على النتيجة لتفتح الخانة نفسها والنص محدَّد بداخلها.</li>
      </ul>
    </div>
  )
}

/* ---------------- land on the field: ?f=<field>&hl=<text> ---------------- */

/** After opening a result: scroll to the field, flash it and select the matched text inside it. */
export function useFieldFocus() {
  const loc = useLocation()
  useEffect(() => {
    const p = new URLSearchParams(loc.search)
    const f = p.get('f')
    if (!f) return
    const hl = p.get('hl') ?? ''
    let tries = 0
    let timer = 0
    const attempt = () => {
      const box = document.querySelector<HTMLElement>(`[data-field="${CSS.escape(f)}"]`)
      // wait until the form has its data (rich editors render a moment later)
      const ready = box && (!hl || (box.querySelector('input,textarea') as HTMLInputElement | null)?.value || box.querySelector('[contenteditable]')?.textContent)
      if (!box || (!ready && tries < 25)) { if (tries++ < 40) timer = window.setTimeout(attempt, 150); return }
      box.scrollIntoView({ block: 'center', behavior: 'smooth' })
      box.classList.remove('field-flash'); void box.offsetWidth; box.classList.add('field-flash')
      window.setTimeout(() => box.classList.remove('field-flash'), 2600)
      window.setTimeout(() => selectText(box, hl), 380)
    }
    timer = window.setTimeout(attempt, 120)
    return () => window.clearTimeout(timer)
  }, [loc.pathname, loc.search])
}

function selectText(box: HTMLElement, hl: string) {
  const input = box.querySelector<HTMLInputElement | HTMLTextAreaElement>('input:not([type=checkbox]):not([type=file]),textarea')
  const editable = box.querySelector<HTMLElement>('[contenteditable="true"]')
  if (editable && hl) {
    // walk the editor's text nodes to find the match (it may sit inside one text node)
    const walker = document.createTreeWalker(editable, NodeFilter.SHOW_TEXT)
    for (let n = walker.nextNode(); n; n = walker.nextNode()) {
      const m = findIn(n.textContent ?? '', hl)
      if (!m) continue
      const r = document.createRange()
      r.setStart(n, m[0]); r.setEnd(n, m[1])
      editable.focus({ preventScroll: true })
      const sel = window.getSelection(); sel?.removeAllRanges(); sel?.addRange(r)
      ;(n.parentElement as HTMLElement | null)?.scrollIntoView({ block: 'center', behavior: 'smooth' })
      return
    }
    editable.focus({ preventScroll: true })
    return
  }
  if (input) {
    input.focus({ preventScroll: true })
    const m = hl ? findIn(input.value, hl) : null
    if (m) {
      input.setSelectionRange(m[0], m[1])
      if (input instanceof HTMLTextAreaElement) {
        // bring the selected line into view inside a long textarea
        const lineH = parseFloat(getComputedStyle(input).lineHeight) || 24
        const line = input.value.slice(0, m[0]).split('\n').length - 1
        input.scrollTop = Math.max(0, line * lineH - input.clientHeight / 2)
      }
    }
  }
}
