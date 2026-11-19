import { useLayoutEffect, useMemo, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import { ArrowDownAZ, ArrowUpAZ, Filter, Search } from 'lucide-react'

/**
 * Excel-style column filter: every distinct value of the column with its count, search, select all,
 * multi-select, sort. `selected = null` means "no filter" (all values).
 */
export const EMPTY_VALUE = '(فارغ)'

export function ColumnFilterButton({ active, onClick }: { active: boolean; onClick: (el: HTMLElement) => void }) {
  return (
    <button type="button" className={`cf-btn ${active ? 'on' : ''}`} title="تصفية العمود" onClick={e => { e.stopPropagation(); onClick(e.currentTarget) }}>
      <Filter size={12} />
    </button>
  )
}

export function ColumnFilterMenu({ anchor, title, values, selected, onApply, onSort, onClose }: {
  anchor: HTMLElement
  title: string
  values: { value: string; count: number }[]
  selected: Set<string> | null
  onApply: (s: Set<string> | null) => void
  onSort?: (dir: 1 | -1) => void
  onClose: () => void
}) {
  const [q, setQ] = useState('')
  const [pick, setPick] = useState<Set<string>>(() => new Set(selected ?? values.map(v => v.value)))
  const box = useRef<HTMLDivElement>(null)
  const [pos, setPos] = useState<{ top: number; left: number }>({ top: 0, left: 0 })

  useLayoutEffect(() => {
    const r = anchor.getBoundingClientRect()
    const w = 280, h = box.current?.offsetHeight ?? 420
    let left = r.right - w
    if (left < 8) left = 8
    if (left + w > window.innerWidth - 8) left = window.innerWidth - w - 8
    let top = r.bottom + 6
    if (top + h > window.innerHeight - 8) top = Math.max(8, r.top - h - 6)
    setPos({ top, left })
  }, [anchor])

  const shown = useMemo(() => values.filter(v => !q || v.value.includes(q)), [values, q])
  const allShown = shown.length > 0 && shown.every(v => pick.has(v.value))
  const toggleAll = () => setPick(p => { const n = new Set(p); shown.forEach(v => (allShown ? n.delete(v.value) : n.add(v.value))); return n })
  const apply = () => { onApply(pick.size === values.length ? null : pick); onClose() }

  return createPortal(
    <>
      <div className="cf-away" onClick={onClose} />
      <div ref={box} className="cf-menu" style={{ top: pos.top, left: pos.left }} onKeyDown={e => { if (e.key === 'Escape') onClose(); if (e.key === 'Enter') apply() }}>
        <div className="cf-title">{title}</div>
        {onSort && <div className="cf-sort">
          <button type="button" onClick={() => { onSort(1); onClose() }}><ArrowDownAZ size={15} /> ترتيب تصاعدي</button>
          <button type="button" onClick={() => { onSort(-1); onClose() }}><ArrowUpAZ size={15} /> ترتيب تنازلي</button>
        </div>}
        <div className="input-wrap cf-search"><Search size={14} /><input className="input" autoFocus placeholder="بحث في القيم..." value={q} onChange={e => setQ(e.target.value)} /></div>
        <label className="cf-item cf-all"><input type="checkbox" checked={allShown} onChange={toggleAll} /> <span>تحديد الكل{q ? ' (نتائج البحث)' : ''}</span></label>
        <div className="cf-list">
          {shown.slice(0, 400).map(v => (
            <label key={v.value} className="cf-item">
              <input type="checkbox" checked={pick.has(v.value)} onChange={() => setPick(p => { const n = new Set(p); if (n.has(v.value)) n.delete(v.value); else n.add(v.value); return n })} />
              <span title={v.value}>{v.value}</span><small>{v.count}</small>
            </label>
          ))}
          {shown.length > 400 && <div className="cf-more">يتم عرض أول 400 قيمة، استخدم البحث لتضييق القائمة</div>}
          {shown.length === 0 && <div className="cf-more">لا توجد قيم مطابقة</div>}
        </div>
        <div className="cf-actions">
          <button type="button" className="btn sm" disabled={pick.size === 0} onClick={apply}>تطبيق</button>
          <button type="button" className="btn ghost sm" onClick={() => { onApply(null); onClose() }}>مسح الفلتر</button>
        </div>
      </div>
    </>,
    document.body,
  )
}

/** Distinct values with counts, sorted naturally. */
export function distinctValues<T>(rows: T[], get: (r: T) => string) {
  const m = new Map<string, number>()
  for (const r of rows) { const v = get(r) || EMPTY_VALUE; m.set(v, (m.get(v) ?? 0) + 1) }
  return [...m.entries()].map(([value, count]) => ({ value, count })).sort((a, b) => a.value.localeCompare(b.value, 'ar', { numeric: true }))
}
