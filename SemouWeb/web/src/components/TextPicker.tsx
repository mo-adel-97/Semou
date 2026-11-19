import { useEffect, useRef, useState, useSyncExternalStore } from 'react'
import { Compass, MousePointerClick, Save, X } from 'lucide-react'
import { alerts } from '../lib/alerts'
import { ADD_STYLES, originalAttr, originalOf, sameRule, type AddStyle, type TextRule } from '../lib/textOverrides'
import { useUiConfig, type TextsConfig } from '../uiConfig'
import { api } from '../api'

/* ------- the picker is switched on from الإعدادات العامة and stays on while moving between screens ------- */
const KEY = 'semou.text-picker'
let active = (() => { try { return sessionStorage.getItem(KEY) === '1' } catch { return false } })()
const subs = new Set<() => void>()
const set = (v: boolean) => { active = v; try { if (v) sessionStorage.setItem(KEY, '1'); else sessionStorage.removeItem(KEY) } catch { /* blocked */ } subs.forEach(f => f()) }
export const textPicker = { start: () => set(true), stop: () => set(false) }
/** Always usable while editing texts: the sidebar (to move between screens) and the picker's own bar / dialogs. */
const FREE = '[data-picker-ui], .swal2-container, .side'
const usePickerActive = () => useSyncExternalStore(f => { subs.add(f); return () => subs.delete(f) }, () => active)

const esc = (s: string) => s.replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]!))

export function upsertRule(rules: TextRule[], r: TextRule): TextRule[] {
  const rest = rules.filter(x => !sameRule(x, r))
  if (r.add) return r.to.trim() ? [...rest, r] : rest
  return r.remove || (r.to.trim() && r.to.trim() !== r.from.trim()) ? [...rest, r] : rest
}

/** What is under the pointer: a text of the page (with its original) or a placeholder / tooltip. */
function hit(x: number, y: number): { original: string; shown: string; rect: DOMRect } | null {
  const el = document.elementFromPoint(x, y)
  if (!el || el.closest(FREE)) return null
  const doc = document as Document & { caretPositionFromPoint?: (x: number, y: number) => { offsetNode: Node } | null; caretRangeFromPoint?: (x: number, y: number) => Range | null }
  const node = doc.caretPositionFromPoint?.(x, y)?.offsetNode ?? doc.caretRangeFromPoint?.(x, y)?.startContainer
  if (node && node.nodeType === Node.TEXT_NODE && (node as Text).data.trim() && el.contains(node)) {
    const r = document.createRange(); r.selectNodeContents(node)
    const rect = r.getBoundingClientRect()
    if (x >= rect.left - 2 && x <= rect.right + 2 && y >= rect.top - 2 && y <= rect.bottom + 2)
      return { original: originalOf(node).trim(), shown: (node as Text).data.trim(), rect }
  }
  // Clicked beside the words but inside an element that holds one text (a heading, a button, a cell): that text.
  const own = [...el.childNodes].filter(c => c.nodeType === Node.TEXT_NODE && (c as Text).data.trim())
  if (own.length === 1) return { original: originalOf(own[0]).trim(), shown: (own[0] as Text).data.trim(), rect: el.getBoundingClientRect() }
  const field = el.closest('input[placeholder], textarea[placeholder]')
  if (field) return { original: (originalAttr(field, 'placeholder') ?? '').trim(), shown: field.getAttribute('placeholder') ?? '', rect: field.getBoundingClientRect() }
  return null
}

/**
 * Click any text of the interface to change it. The changes collect in the settings draft (previewed live
 * everywhere) and are saved with one click from the bar.
 */
export function TextPicker() {
  const on = usePickerActive()
  const { saved, draft, setDraft, refetch } = useUiConfig()
  const [box, setBox] = useState<DOMRect | null>(null)
  const [browsing, setBrowsing] = useState(false)
  const busy = useRef(false)
  const raf = useRef(0)
  const base: TextsConfig = draft.texts ?? saved.texts ?? { v: 1, rules: [] }
  const baseRef = useRef(base); baseRef.current = base
  const pending = draft.texts ? draft.texts.rules.length - (saved.texts?.rules.length ?? 0) : 0
  const changed = !!draft.texts

  useEffect(() => {
    if (!on || browsing) { setBox(null); return }
    document.documentElement.classList.add('picking')
    const move = (e: PointerEvent) => {
      if (busy.current) return
      cancelAnimationFrame(raf.current)
      raf.current = requestAnimationFrame(() => setBox(hit(e.clientX, e.clientY)?.rect ?? null))
    }
    const block = (e: Event) => {
      if ((e as MouseEvent).altKey || busy.current || (e.target as Element)?.closest?.(FREE)) return
      e.preventDefault(); e.stopPropagation()
    }
    const click = async (e: MouseEvent) => {
      if (e.altKey || busy.current || (e.target as Element)?.closest?.(FREE)) return
      e.preventDefault(); e.stopPropagation()
      const h = hit(e.clientX, e.clientY)
      if (!h || !h.original) return
      busy.current = true; setBox(null)
      const rules = baseRef.current.rules
      const cur = rules.find(r => !r.add && r.from.trim() === h.original)
      const added = rules.filter(r => r.add && r.from.trim() === h.original)
      const act0 = cur?.whole ? 'line' : cur?.remove ? 'text' : 'replace'
      const styles = (Object.keys(ADD_STYLES) as AddStyle[]).map(k => `<option value="${k}">${ADD_STYLES[k]}</option>`).join('')
      type Out = { act: 'replace' | 'text' | 'line' | 'add'; to: string; mode: 'exact' | 'contains'; add: 'before' | 'after'; style: AddStyle; text: string }
      const v = await alerts.form<Out>({
        title: 'تعديل الشاشة', confirmText: 'اعتماد', wide: true,
        html: `<div class="sm-form tp-form" data-no-texts data-act="${act0}">
          <label class="sm-lbl">النص الذي اخترته</label><div class="tp-orig">${esc(h.original)}</div>
          <div class="tp-acts">
            <label><input type="radio" name="act" value="replace"${act0 === 'replace' ? ' checked' : ''} /><span>تغيير النص</span></label>
            <label><input type="radio" name="act" value="add" /><span>إضافة نص جديد هنا</span></label>
            <label><input type="radio" name="act" value="text"${act0 === 'text' ? ' checked' : ''} /><span>حذف النص</span></label>
            <label><input type="radio" name="act" value="line"${act0 === 'line' ? ' checked' : ''} /><span>حذف السطر كاملاً</span></label>
          </div>
          <div class="tp-sec tp-s-replace">
            <label class="sm-lbl">النص الجديد</label><input id="tp-to" class="input" maxlength="300" value="${esc(cur && !cur.remove ? cur.to : h.shown)}" />
            <label class="sm-lbl">طريقة الاستبدال</label>
            <select id="tp-mode" class="input"><option value="exact"${cur?.mode !== 'contains' ? ' selected' : ''}>مطابقة النص كاملاً (موصى به)</option><option value="contains"${cur?.mode === 'contains' ? ' selected' : ''}>استبدال هذه العبارة أينما وردت داخل النصوص</option></select>
            <p class="sm-hint">اترك النص الجديد مطابقاً للأصلي أو فارغاً لإرجاعه كما كان.</p>
          </div>
          <div class="tp-sec tp-s-add">
            <label class="sm-lbl">النص المضاف</label><textarea id="tp-new" class="input" rows="3" maxlength="600" placeholder="اكتب النص الذي تريد إضافته..."></textarea>
            <div class="tp-row">
              <div><label class="sm-lbl">مكانه</label><select id="tp-where" class="input"><option value="after">بعد هذا السطر</option><option value="before">قبل هذا السطر</option></select></div>
              <div><label class="sm-lbl">شكله</label><select id="tp-style" class="input">${styles}</select></div>
            </div>
            ${added.length ? `<p class="sm-hint">لهذا السطر نصوص مضافة: ${added.map(a => `«${esc(a.to.slice(0, 40))}» (${a.add === 'before' ? 'قبل' : 'بعد'})`).join('، ')} — اختر نفس المكان لتعديلها، أو امسح النص لحذفها.</p>` : ''}
          </div>
          <div class="tp-sec tp-s-text tp-s-line"><p class="sm-hint">يمكنك إرجاعه في أي وقت من شاشة الإعدادات العامة.</p></div>
        </div>`,
        didOpen: el => {
          const form = el.querySelector('.tp-form') as HTMLElement
          const where = el.querySelector('#tp-where') as HTMLSelectElement, txt = el.querySelector('#tp-new') as HTMLTextAreaElement, sty = el.querySelector('#tp-style') as HTMLSelectElement
          // Switching before/after loads the text already added there, but never wipes what the user typed.
          let auto = ''
          const fill = () => { const a = added.find(x => x.add === where.value); if (txt.value === auto) { txt.value = auto = a?.to ?? '' } if (a?.style) sty.value = a.style }
          sty.value = 'note'
          fill(); where.addEventListener('change', fill)
          el.querySelectorAll<HTMLInputElement>('input[name=act]').forEach(r => r.addEventListener('change', () => { form.dataset.act = r.value; if (r.value === 'add') txt.focus() }))
        },
        read: el => {
          const q = <T,>(sel: string) => el.querySelector(sel) as unknown as T
          const act = q<HTMLInputElement>('input[name=act]:checked').value as Out['act']
          return {
            act, to: q<HTMLInputElement>('#tp-to').value.trim(), mode: q<HTMLSelectElement>('#tp-mode').value as Out['mode'],
            add: q<HTMLSelectElement>('#tp-where').value as Out['add'], style: q<HTMLSelectElement>('#tp-style').value as AddStyle, text: q<HTMLTextAreaElement>('#tp-new').value.trim(),
          }
        },
      })
      busy.current = false
      if (!v) return
      const rule: TextRule = v.act === 'add'
        ? { from: h.original, to: v.text, mode: 'exact', add: v.add, style: v.style, path: location.pathname }
        : { from: h.original, to: v.act === 'replace' ? v.to : '', mode: v.act === 'line' ? 'exact' : v.mode, remove: v.act !== 'replace' || undefined, whole: v.act === 'line' || undefined }
      setDraft(d => ({ ...d, texts: { v: 1, rules: upsertRule((d.texts ?? saved.texts ?? { v: 1, rules: [] }).rules, rule) } }))
    }
    const key = (e: KeyboardEvent) => { if (e.key === 'Escape' && !busy.current) textPicker.stop() }
    window.addEventListener('pointermove', move, true)
    for (const t of ['pointerdown', 'mousedown', 'mouseup', 'pointerup', 'dblclick', 'submit'] as const) window.addEventListener(t, block, true)
    window.addEventListener('click', click, true)
    window.addEventListener('keydown', key, true)
    return () => {
      document.documentElement.classList.remove('picking')
      window.removeEventListener('pointermove', move, true)
      for (const t of ['pointerdown', 'mousedown', 'mouseup', 'pointerup', 'dblclick', 'submit'] as const) window.removeEventListener(t, block, true)
      window.removeEventListener('click', click, true)
      window.removeEventListener('keydown', key, true)
    }
  }, [on, browsing, saved.texts, setDraft])

  const save = async () => {
    if (!draft.texts) return
    try {
      await api('/ui-config/texts', { method: 'PUT', json: { data: draft.texts, baseVersion: saved.versions.texts ?? null, note: 'تعديل نصوص من الشاشة مباشرة' } })
      await refetch(); setDraft(d => ({ ...d, texts: undefined }))
      alerts.success('تم حفظ النصوص', 'ظهرت التعديلات لجميع المستخدمين')
    } catch (e) { alerts.error((e as Error).message, 'تعذّر الحفظ') }
  }

  if (!on) return null
  return (
    <>
      {box && <div className="tp-box" data-picker-ui style={{ top: box.top - 3, left: box.left - 4, width: box.width + 8, height: box.height + 6 }} />}
      <div className="tp-bar" data-picker-ui data-no-texts>
        {browsing ? <Compass size={18} /> : <MousePointerClick size={18} />}
        {browsing
          ? <div><b>وضع التصفح</b><small>استخدم البرنامج وتنقّل عادي، ثم اضغط «متابعة التعديل» عند الشاشة التي تريدها</small></div>
          : <div><b>وضع تعديل النصوص</b><small>انقر على أي نص لتغييره • القائمة الجانبية تعمل للتنقل • «تصفح» لاستخدام الشاشة عادي • Esc للخروج</small></div>}
        <button className={`btn sm ${browsing ? '' : 'secondary'} tp-mode`} onClick={() => setBrowsing(b => !b)}>
          {browsing ? <><MousePointerClick size={15} /> متابعة التعديل</> : <><Compass size={15} /> تصفح</>}
        </button>
        {changed && <span className="tp-count">{pending > 0 ? `${pending.toLocaleString('ar-EG')} تعديل جديد` : 'تعديلات غير محفوظة'}</span>}
        {changed && <button className="btn sm" onClick={save}><Save size={15} /> حفظ للجميع</button>}
        {changed && <button className="btn ghost sm" onClick={() => setDraft(d => ({ ...d, texts: undefined }))}>تجاهل</button>}
        <button className="btn ghost sm icon" title="إنهاء" onClick={() => textPicker.stop()}><X size={16} /></button>
      </div>
    </>
  )
}
