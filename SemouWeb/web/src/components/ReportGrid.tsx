import { useLayoutEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { useVirtualizer, useWindowVirtualizer } from '@tanstack/react-virtual'
import { ArrowDown, ArrowUp, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, Columns3, FilterX, Maximize2, Minimize2, Rows3, Search, X } from 'lucide-react'
import { ColumnFilterButton, ColumnFilterMenu, distinctValues, EMPTY_VALUE } from './ColumnFilter'

/**
 * Professional report grid:
 *  - always fits the page width (no horizontal scroll): widths are proportions, long text is cut with "…" and shown on hover
 *  - Excel-style filter on every column (distinct values, search, multi-select) + sort
 *  - quick search, column chooser, density, full screen, pagination
 *  - totals footer computed on ALL filtered rows (not just the page)
 */
export interface RCol<T> {
  key: string
  label: string
  /** Relative width (proportion of the row). */
  width: number
  num?: boolean
  /** Whole numbers (counts): no decimals. */
  int?: boolean
  blankZero?: boolean
  tone?: 'due' | 'ok' | 'sign'
  group?: string
  value?: (r: T) => string | number | null | undefined
  render?: (r: T) => ReactNode
  foot?: false | ReactNode
  hidden?: boolean
  /** No column filter (e.g. action buttons). */
  noFilter?: boolean
}

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const SIZES = [25, 50, 100, 0] as const
/** Above this many rows on one page only the visible rows are rendered (virtual scrolling). */
const VIRTUAL_FROM = 120

/** The element that scrolls the grid: the grid body in full screen, the content column, or the window on small screens. */
function scrollParent(el: HTMLElement | null): HTMLElement | null {
  for (let p = el?.parentElement; p; p = p.parentElement) {
    const o = getComputedStyle(p).overflowY
    if ((o === 'auto' || o === 'scroll') && p.scrollHeight > p.clientHeight) return p
  }
  return null
}

export function ReportGrid<T>({ columns, rows, rowKey, rowClass, footLabel = 'الإجمالي', onRowClick, toolbar, emptyText = 'لا توجد بيانات', pageSize: initialSize = 25, tree = false, selection }: {
  columns: RCol<T>[]
  rows: T[]
  rowKey: (r: T, i: number) => string
  rowClass?: (r: T) => string | undefined
  footLabel?: string
  onRowClick?: (r: T) => void
  toolbar?: ReactNode
  emptyText?: string
  pageSize?: number
  /** Tree mode (rows already ordered parent → children): no sorting / column filters. */
  tree?: boolean
  /** Row selection (checkbox column): the keys are rowKey values. */
  selection?: { keys: Set<string>; onChange: (keys: Set<string>) => void }
  /** @deprecated kept for old callers; the grid now grows with the page. */
  height?: string
}) {
  const [q, setQ] = useState('')
  const [sort, setSort] = useState<{ key: string; dir: 1 | -1 } | null>(null)
  const [filters, setFilters] = useState<Record<string, Set<string>>>({})
  const [menu, setMenu] = useState<{ key: string; el: HTMLElement } | null>(null)
  const [hidden, setHidden] = useState<Record<string, boolean>>(() => Object.fromEntries(columns.filter(c => c.hidden).map(c => [c.key, true])))
  const [density, setDensity] = useState<'compact' | 'normal'>('compact')
  const [full, setFull] = useState(false)
  const [chooser, setChooser] = useState(false)
  const [selected, setSelected] = useState<string | null>(null)
  const [size, setSize] = useState<number>(initialSize)
  const [page, setPage] = useState(0)

  const cols = columns.filter(c => !hidden[c.key])
  const val = (c: RCol<T>, r: T) => (c.value ? c.value(r) : (r as Record<string, unknown>)[c.key] as string | number | null | undefined)
  /** Text used by search and column filters (what the user sees). */
  const text = (c: RCol<T>, r: T) => {
    const v = val(c, r)
    if (v == null || v === '') return ''
    if (c.num) { const n = Number(v) || 0; return c.blankZero && !n ? '' : c.int ? n.toLocaleString('ar-EG') : money(n) }
    return String(v)
  }

  const filtered = useMemo(() => {
    let out = rows
    for (const [key, set] of Object.entries(filters)) {
      const c = columns.find(x => x.key === key)
      if (c) out = out.filter(r => set.has(text(c, r) || EMPTY_VALUE))
    }
    const s = q.trim()
    if (s) out = out.filter(r => columns.some(c => text(c, r).includes(s)))
    if (sort) {
      const c = columns.find(x => x.key === sort.key)
      if (c) out = [...out].sort((a, b) => {
        const x = val(c, a), y = val(c, b)
        if (typeof x === 'number' && typeof y === 'number') return (x - y) * sort.dir
        return String(x ?? '').localeCompare(String(y ?? ''), 'ar', { numeric: true }) * sort.dir
      })
    }
    return out
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rows, q, sort, filters, columns])

  const totals = useMemo(() => Object.fromEntries(cols.filter(c => c.num && c.foot !== false).map(c => [c.key, filtered.reduce((s, r) => s + (Number(val(c, r)) || 0), 0)])), [filtered, cols]) // eslint-disable-line react-hooks/exhaustive-deps
  const pages = size ? Math.max(1, Math.ceil(filtered.length / size)) : 1
  const cur = Math.min(page, pages - 1)
  const view = size ? filtered.slice(cur * size, cur * size + size) : filtered
  const weight = cols.reduce((s, c) => s + c.width, 0)
  const activeFilters = Object.keys(filters).length

  const groups: { label: string; span: number }[] = []
  for (const c of cols) {
    const g = c.group ?? ''
    if (groups.length && g && groups[groups.length - 1].label === g) groups[groups.length - 1].span++
    else groups.push({ label: g, span: 1 })
  }
  const hasGroups = cols.some(c => c.group)
  const span = cols.length + (selection ? 1 : 0)

  /* ---- virtual scrolling for big pages (e.g. a year of the general journal = 15,000 lines) ---- */
  const big = view.length > VIRTUAL_FROM
  const bodyRef = useRef<HTMLTableSectionElement>(null)
  const [scroller, setScroller] = useState<{ el: HTMLElement | null; margin: number }>({ el: null, margin: 0 })
  useLayoutEffect(() => {
    if (!big) return
    const el = scrollParent(bodyRef.current)
    const top = bodyRef.current!.getBoundingClientRect().top
    const margin = el ? top - el.getBoundingClientRect().top + el.scrollTop : top + window.scrollY
    setScroller(s => (s.el === el && Math.abs(s.margin - margin) < 1 ? s : { el, margin }))
  })
  const rowH = density === 'compact' ? 36 : 44
  const opts = { count: big ? view.length : 0, estimateSize: () => rowH, overscan: 14, scrollMargin: scroller.margin }
  const elV = useVirtualizer({ ...opts, getScrollElement: () => scroller.el, enabled: big && !!scroller.el })
  const winV = useWindowVirtualizer({ ...opts, enabled: big && !scroller.el })
  const v = scroller.el ? elV : winV
  const items = big ? v.getVirtualItems() : []
  const measure = v.measureElement
  const pad = items.length
    ? { top: items[0].start - scroller.margin, bottom: v.getTotalSize() - (items[items.length - 1].end - scroller.margin) }
    : { top: 0, bottom: big ? view.length * rowH : 0 }

  const cell = (c: RCol<T>, r: T) => (c.render ? c.render(r) : text(c, r))
  const tone = (c: RCol<T>, r: T) => {
    if (!c.tone) return ''
    if (c.tone === 'sign') return (Number(val(c, r)) || 0) < 0 ? 'due' : ''
    return (Number(val(c, r)) || 0) ? c.tone : ''
  }
  const setFilter = (key: string, s: Set<string> | null) => { setFilters(f => { const n = { ...f }; if (s) n[key] = s; else delete n[key]; return n }); setPage(0) }
  const menuCol = menu ? columns.find(c => c.key === menu.key) : undefined
  // Values for a column's menu: computed against the rows filtered by the OTHER columns (like Excel).
  const menuValues = useMemo(() => {
    if (!menuCol) return []
    let base = rows
    for (const [key, set] of Object.entries(filters)) {
      if (key === menuCol.key) continue
      const c = columns.find(x => x.key === key)
      if (c) base = base.filter(r => set.has(text(c, r) || EMPTY_VALUE))
    }
    return distinctValues(base, r => text(menuCol, r))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [menuCol, rows, filters, columns])

  return (
    <div className={`rg ${full ? 'rg-full' : ''} rg-${density}`}>
      <div className="rg-bar">
        <div className="input-wrap rg-search"><Search size={15} /><input className="input" placeholder="بحث في الجدول..." value={q} onChange={e => { setQ(e.target.value); setPage(0) }} />
          {q && <button className="btn ghost sm icon trail" onClick={() => setQ('')} aria-label="مسح"><X size={14} /></button>}</div>
        <span className="rg-count">{filtered.length.toLocaleString('ar-EG')} سطر{filtered.length !== rows.length ? ` من ${rows.length.toLocaleString('ar-EG')}` : ''}</span>
        {activeFilters > 0 && <button className="btn ghost sm rg-clear" onClick={() => { setFilters({}); setPage(0) }}><FilterX size={15} /> مسح فلاتر الأعمدة ({activeFilters})</button>}
        <div className="rg-tools">
          {toolbar}
          <div className="pop-anchor">
            <button className="btn ghost sm" onClick={() => setChooser(v => !v)} title="إظهار / إخفاء الأعمدة"><Columns3 size={16} /> الأعمدة</button>
            {chooser && <>
              <div className="click-away" onClick={() => setChooser(false)} />
              <div className="menu-pop rg-chooser">
                {columns.map(c => (
                  <label key={c.key} className="sm-chk"><input type="checkbox" checked={!hidden[c.key]} onChange={e => setHidden(h => ({ ...h, [c.key]: !e.target.checked }))} /> {c.group ? `${c.group} - ` : ''}{c.label}</label>
                ))}
              </div>
            </>}
          </div>
          <button className="btn ghost sm icon" title={density === 'compact' ? 'صفوف مريحة' : 'صفوف مضغوطة'} onClick={() => setDensity(d => (d === 'compact' ? 'normal' : 'compact'))}><Rows3 size={16} /></button>
          <button className="btn ghost sm icon" title={full ? 'تصغير' : 'ملء الشاشة'} onClick={() => setFull(f => !f)}>{full ? <Minimize2 size={16} /> : <Maximize2 size={16} />}</button>
        </div>
      </div>
      <div className="rg-body">
        <table className="rg-table">
          <colgroup>{selection && <col style={{ width: 40 }} />}{cols.map(c => <col key={c.key} style={{ width: `${(c.width / weight) * 100}%` }} />)}</colgroup>
          <thead>
            {hasGroups && <tr className="rg-groups">{selection && <th />}{groups.map((g, i) => <th key={i} colSpan={g.span} className={g.label ? 'grp' : ''}>{g.label}</th>)}</tr>}
            <tr>
              {selection && (() => {
                const keys = filtered.map((r, i) => rowKey(r, i))
                const on = keys.length > 0 && keys.every(k => selection.keys.has(k))
                const some = !on && keys.some(k => selection.keys.has(k))
                return <th className="rg-sel"><input type="checkbox" checked={on} ref={el => { if (el) el.indeterminate = some }} title="تحديد كل السطور الظاهرة"
                  onChange={() => { const n = new Set(selection.keys); keys.forEach(k => (on ? n.delete(k) : n.add(k))); selection.onChange(n) }} /></th>
              })()}
              {cols.map(c => (
                <th key={c.key} className={c.num ? 'n' : ''} title={c.label}>
                  <div className="rg-th">
                    <span className="lbl" onClick={() => !tree && setSort(s => (s?.key === c.key ? (s.dir === 1 ? { key: c.key, dir: -1 } : null) : { key: c.key, dir: 1 }))}>
                      {c.label}{sort?.key === c.key && (sort.dir === 1 ? <ArrowUp size={12} /> : <ArrowDown size={12} />)}
                    </span>
                    {!c.noFilter && !tree && <ColumnFilterButton active={!!filters[c.key]} onClick={el => setMenu({ key: c.key, el })} />}
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody ref={bodyRef}>
            {view.length === 0 && <tr><td colSpan={span} className="rg-empty">{rows.length ? 'لا توجد نتائج مطابقة للفلاتر' : emptyText}</td></tr>}
            {big && pad.top > 0 && <tr className="rg-pad" aria-hidden><td colSpan={span} style={{ height: pad.top }} /></tr>}
            {(big ? items.map(it => ({ r: view[it.index], i: it.index })) : view.map((r, i) => ({ r, i }))).map(({ r, i }) => {
              const k = rowKey(r, cur * size + i)
              return (
                <tr key={k} data-index={i} ref={big ? measure : undefined} className={`${rowClass?.(r) ?? ''} ${selected === k ? 'sel' : ''} ${selection?.keys.has(k) ? 'checked' : ''}`} onClick={() => { setSelected(k); onRowClick?.(r) }}>
                  {selection && <td className="rg-sel" onClick={e => e.stopPropagation()}><input type="checkbox" checked={selection.keys.has(k)}
                    onChange={() => { const n = new Set(selection.keys); if (n.has(k)) n.delete(k); else n.add(k); selection.onChange(n) }} /></td>}
                  {cols.map(c => { const v = cell(c, r); return <td key={c.key} className={`${c.num ? 'n' : ''} ${tone(c, r)}`} title={typeof v === 'string' ? v : undefined}>{v}</td> })}
                </tr>
              )
            })}
            {big && pad.bottom > 0 && <tr className="rg-pad" aria-hidden><td colSpan={span} style={{ height: pad.bottom }} /></tr>}
          </tbody>
          <tfoot>
            <tr>
              {selection && <td />}
              {cols.map((c, i) => (
                <td key={c.key} className={c.num ? 'n' : ''}>
                  {c.foot !== undefined && c.foot !== false ? c.foot : c.num && c.foot !== false ? (c.int ? (totals[c.key] ?? 0).toLocaleString('ar-EG') : money(totals[c.key] ?? 0)) : i === 0 ? footLabel : ''}
                </td>
              ))}
            </tr>
          </tfoot>
        </table>
      </div>
      <div className="rg-pager">
        <span>عدد السطور في الصفحة</span>
        <select className="input" value={size} onChange={e => { setSize(Number(e.target.value)); setPage(0) }}>
          {SIZES.map(s => <option key={s} value={s}>{s || 'الكل'}</option>)}
        </select>
        {size > 0 && pages > 1 && <div className="pager">
          <button className="btn ghost sm icon" disabled={cur === 0} onClick={() => setPage(0)} aria-label="الأولى"><ChevronsRight size={16} /></button>
          <button className="btn ghost sm icon" disabled={cur === 0} onClick={() => setPage(cur - 1)} aria-label="السابقة"><ChevronRight size={16} /></button>
          <span>صفحة <b>{(cur + 1).toLocaleString('ar-EG')}</b> من {pages.toLocaleString('ar-EG')}</span>
          <button className="btn ghost sm icon" disabled={cur >= pages - 1} onClick={() => setPage(cur + 1)} aria-label="التالية"><ChevronLeft size={16} /></button>
          <button className="btn ghost sm icon" disabled={cur >= pages - 1} onClick={() => setPage(pages - 1)} aria-label="الأخيرة"><ChevronsLeft size={16} /></button>
        </div>}
      </div>
      {menu && menuCol && (
        <ColumnFilterMenu anchor={menu.el} title={menuCol.group ? `${menuCol.group} - ${menuCol.label}` : menuCol.label} values={menuValues} selected={filters[menu.key] ?? null}
          onApply={s => setFilter(menu.key, s)} onSort={dir => setSort({ key: menu.key, dir })} onClose={() => setMenu(null)} />
      )}
    </div>
  )
}
