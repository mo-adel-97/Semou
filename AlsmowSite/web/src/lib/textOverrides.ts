/**
 * Site texts edited from the dashboard (or by clicking them on the site in edit mode), applied to whatever is
 * on screen — every page, menu, button, message and tooltip — without touching the source code.
 *
 *  - exact   : a text that is exactly the original (ignoring the spaces around it) is replaced. Safe everywhere.
 *  - contains: every occurrence of the original inside a text is replaced (for phrases inside longer sentences).
 *              Never applied inside form fields or the rich editor.
 *  - remove  : the text disappears; whole: the element holding it (the whole line / button) is hidden.
 *  - add     : a NEW text shown before / after the line that holds `from` (the anchor), on one page or all.
 *
 * The original of every replaced text is remembered, so new rules re-apply cleanly and edit mode can show
 * what the source really says.
 */
export interface TextRule {
  from: string
  to: string
  mode: 'exact' | 'contains'
  remove?: boolean
  whole?: boolean
  add?: 'before' | 'after'
  style?: AddStyle
  /** The page (URL path) an added text belongs to; empty = every page. */
  path?: string
}
export type AddStyle = 'text' | 'title' | 'note' | 'warn'
export const ADD_STYLES: Record<AddStyle, string> = { text: 'نص عادي', title: 'عنوان', note: 'ملاحظة (مربع ملوّن)', warn: 'تنبيه (مربع ذهبي)' }
const ruleKey = (r: Pick<TextRule, 'from' | 'add' | 'path'>) => `${r.add ?? 'edit'}|${r.add ? (r.path ?? '') : ''}|${r.from.trim()}`
export const sameRule = (a: TextRule, b: TextRule) => ruleKey(a) === ruleKey(b)

/** Adds / replaces a rule in a list; an empty replacement (or one equal to the original) removes it. */
export function upsertRule(rules: TextRule[], r: TextRule): TextRule[] {
  const rest = rules.filter((x) => !sameRule(x, r))
  if (r.add) return r.to.trim() ? [...rest, r] : rest
  return r.remove || (r.to.trim() && r.to.trim() !== r.from.trim()) ? [...rest, r] : rest
}

const ATTRS = ['placeholder', 'title', 'aria-label', 'alt'] as const
const SKIP_ALL = '[data-no-texts], script, style, noscript, code, pre'
const SKIP_PARTIAL = 'input, textarea, select, [contenteditable], .ProseMirror'

let exact = new Map<string, string>()
let whole = new Set<string>()
let adds = new Map<string, TextRule[]>()
const hiddenBy = new Map<Element, Text>()
let contains: [string, string][] = []
const original = new WeakMap<Node, string>()
const output = new WeakMap<Node, string>()
const attrOriginal = new WeakMap<Element, Record<string, string>>()
const attrOutput = new WeakMap<Element, Record<string, string>>()
let observer: MutationObserver | null = null
let titleOriginal: string | null = null
let titleOutput: string | null = null

function transform(s: string, partial: boolean): string {
  const t = s.trim()
  if (!t) return s
  const e = exact.get(t)
  if (e !== undefined) return s.replace(t, e)
  if (!partial || contains.length === 0) return s
  let r = s
  for (const [f, to] of contains) if (r.includes(f)) r = r.split(f).join(to)
  return r
}

function textNode(n: Text) {
  const cur = n.data
  if (output.get(n) === cur) return // our own replacement
  const el = n.parentElement
  if (!el || el.closest(SKIP_ALL)) return
  original.set(n, cur)
  if (adds.size) insertAdds(el, cur.trim())
  if (whole.size && whole.has(cur.trim())) {
    // hide the whole line: a button / link / list item / heading holding the text
    const line = el.closest('a, button, li, h1, h2, h3, h4, h5, h6, p, label') ?? el
    line.setAttribute('data-tx-hide', '')
    hiddenBy.set(line, n)
  } else {
    for (const [line, by] of hiddenBy) if (by === n) { line.removeAttribute('data-tx-hide'); hiddenBy.delete(line) }
  }
  const next = transform(cur, !el.closest(SKIP_PARTIAL))
  if (next !== cur) { output.set(n, next); n.data = next } else output.delete(n)
}

/** The added texts of an anchor, placed beside the anchor's line (once, even when the page re-renders). */
function insertAdds(el: Element, anchor: string) {
  const list = adds.get(anchor)
  if (!list || el.closest('nav, header, [data-picker-ui]')) return
  for (const r of list) {
    if (r.path && r.path !== location.pathname) continue
    const id = ruleKey(r)
    const sib = r.add === 'before' ? el.previousElementSibling : el.nextElementSibling
    if (sib instanceof HTMLElement && sib.dataset.txAdd === id) { if (sib.textContent !== r.to) sib.textContent = r.to; continue }
    // Already in this container but no longer beside the line (the page re-drew it): move it back, never add twice.
    const stray = el.parentElement ? [...el.parentElement.children].find((c) => c instanceof HTMLElement && c.dataset.txAdd === id) : undefined
    if (stray) { stray.textContent = r.to; el.insertAdjacentElement(r.add === 'before' ? 'beforebegin' : 'afterend', stray); continue }
    const block = r.style !== 'text' || getComputedStyle(el).display !== 'inline'
    const node = document.createElement(block ? 'div' : 'span')
    node.className = `tx-add tx-${r.style ?? 'text'}`
    node.dataset.txAdd = id
    node.setAttribute('data-no-texts', '')
    node.textContent = r.to
    el.insertAdjacentElement(r.add === 'before' ? 'beforebegin' : 'afterend', node)
  }
}

function attrs(el: Element) {
  if (el.closest(SKIP_ALL)) return
  for (const a of ATTRS) {
    const cur = el.getAttribute(a)
    if (cur == null) continue
    const outs = attrOutput.get(el) ?? {}
    if (outs[a] === cur) continue
    const origs = attrOriginal.get(el) ?? {}
    origs[a] = cur
    attrOriginal.set(el, origs)
    const next = transform(cur, false)
    if (next !== cur) { outs[a] = next; attrOutput.set(el, outs); el.setAttribute(a, next) } else { delete outs[a]; attrOutput.set(el, outs) }
  }
}

function walk(root: Node) {
  if (root.nodeType === Node.TEXT_NODE) { textNode(root as Text); return }
  if (root.nodeType !== Node.ELEMENT_NODE) return
  const w = document.createTreeWalker(root, NodeFilter.SHOW_TEXT | NodeFilter.SHOW_ELEMENT)
  let n: Node | null = root
  while (n) {
    if (n.nodeType === Node.TEXT_NODE) textNode(n as Text)
    else attrs(n as Element)
    n = w.nextNode()
  }
}

function title() {
  const cur = document.title
  if (cur === titleOutput) return
  titleOriginal = cur
  const next = transform(cur, true)
  if (next !== cur) { titleOutput = next; document.title = next } else titleOutput = null
}

/** Puts back every original text (before applying a new set of rules). */
function restoreAll() {
  const w = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT | NodeFilter.SHOW_ELEMENT)
  let n: Node | null = w.currentNode
  while (n) {
    if (n.nodeType === Node.TEXT_NODE) {
      const o = original.get(n)
      if (o !== undefined && output.get(n) === (n as Text).data) { output.delete(n); (n as Text).data = o }
    } else {
      const el = n as Element
      const outs = attrOutput.get(el)
      const origs = attrOriginal.get(el)
      if (outs && origs) for (const a of Object.keys(outs)) if (el.getAttribute(a) === outs[a]) el.setAttribute(a, origs[a])
      attrOutput.delete(el)
    }
    n = w.nextNode()
  }
  hiddenBy.forEach((_, el) => el.removeAttribute('data-tx-hide'))
  hiddenBy.clear()
  document.querySelectorAll('[data-tx-add]').forEach((x) => x.remove())
  if (titleOutput && document.title === titleOutput && titleOriginal) document.title = titleOriginal
  titleOutput = null
}

let lastKey = ''
/** Applies a set of rules to the page now and to everything that appears later. */
export function applyTextRules(rules: TextRule[]) {
  if (typeof document === 'undefined') return
  const key = JSON.stringify(rules)
  if (key === lastKey) return
  lastKey = key
  restoreAll()
  adds = new Map()
  for (const r of rules) if (r.add && r.from.trim() && r.to.trim()) adds.set(r.from.trim(), [...(adds.get(r.from.trim()) ?? []), r])
  const valid = rules
    .filter((r) => !r.add)
    .filter((r) => r.from.trim() && (r.remove || (r.to.trim() && r.from.trim() !== r.to.trim())))
    .map((r) => (r.remove ? { ...r, to: '' } : r))
  exact = new Map(valid.filter((r) => r.mode !== 'contains').map((r) => [r.from.trim(), r.to.trim()]))
  whole = new Set(valid.filter((r) => r.remove && r.whole).map((r) => r.from.trim()))
  // Longer phrases first, so "بوابة المتدربات" wins over "بوابة".
  contains = valid.filter((r) => r.mode === 'contains').map((r) => [r.from, r.to] as [string, string]).sort((a, b) => b[0].length - a[0].length)
  if (exact.size === 0 && contains.length === 0 && adds.size === 0) { observer?.disconnect(); observer = null; return }
  walk(document.body)
  title()
  if (observer) return
  observer = new MutationObserver((list) => {
    for (const m of list) {
      if (m.type === 'characterData') textNode(m.target as Text)
      else if (m.type === 'attributes') attrs(m.target as Element)
      else m.addedNodes.forEach(walk)
    }
    title()
  })
  observer.observe(document.body, { subtree: true, childList: true, characterData: true, attributes: true, attributeFilter: [...ATTRS] })
  const t = document.querySelector('title')
  if (t) observer.observe(t, { childList: true, characterData: true, subtree: true })
}

/** Re-checks added texts after a page change (they are tied to a path). */
export function refreshAdds() {
  if (adds.size) walk(document.body)
}

/** What the source says for a text node currently on screen (its text before any rule). */
export const originalOf = (n: Node): string => original.get(n) ?? (n.nodeType === Node.TEXT_NODE ? (n as Text).data : '')
export const originalAttr = (el: Element, a: string): string | null => attrOriginal.get(el)?.[a] ?? el.getAttribute(a)
