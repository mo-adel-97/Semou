import { useMemo, useState, type ReactNode } from 'react'
import { motion } from 'motion/react'
import {
  columnFilteringFeature,
  createFilteredRowModel,
  createPaginatedRowModel,
  createSortedRowModel,
  filterFn_includesString,
  globalFilteringFeature,
  rowPaginationFeature,
  rowSortingFeature,
  sortFn_alphanumeric,
  sortFn_basic,
  sortFn_text,
  tableFeatures,
  useTable,
  type ColumnDef,
  type PaginationState,
  type RowData,
  type TableFeatures,
  type CellData,
  type SortingState,
} from '@tanstack/react-table'
import { ArrowDown, ArrowUp, ArrowUpDown, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, FilterX, Inbox, Search, X } from 'lucide-react'
import { ColumnFilterButton, ColumnFilterMenu, distinctValues, EMPTY_VALUE } from './ColumnFilter'

/**
 * TanStack Table v9 setup shared by every list screen: global search, sorting, paging.
 * Defined once at module scope (required by v9 for stable inputs).
 */
export const gridFeatures = tableFeatures({
  columnFilteringFeature,
  globalFilteringFeature,
  rowSortingFeature,
  rowPaginationFeature,
  filteredRowModel: createFilteredRowModel(),
  sortedRowModel: createSortedRowModel(),
  paginatedRowModel: createPaginatedRowModel(),
  filterFns: { includesString: filterFn_includesString },
  sortFns: { alphanumeric: sortFn_alphanumeric, text: sortFn_text, basic: sortFn_basic },
})

export type GridColumn<T extends RowData> = ColumnDef<typeof gridFeatures, T>

declare module '@tanstack/react-table' {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  interface ColumnMeta<TFeatures extends TableFeatures, TData extends RowData, TValue extends CellData = CellData> {
    /** Fixed column width (px or CSS length). */
    width?: number | string
  }
}

const PAGE_SIZES = [10, 15, 25, 50, 100]
const fmt = (n: number) => n.toLocaleString('ar-EG')

export function DataTable<T extends RowData>({ data, columns, loading, getRowId, onRowClick, toolbar, searchPlaceholder = 'بحث...', emptyText = 'لا توجد بيانات', initialPageSize = 15 }: {
  /** Must be stable (query data or a memo); v9 recomputes models when the reference changes. */
  data: T[]
  /** Must be stable (module scope or useMemo). */
  columns: GridColumn<T>[]
  loading?: boolean
  getRowId: (row: T) => string
  onRowClick?: (row: T) => void
  toolbar?: ReactNode
  searchPlaceholder?: string
  emptyText?: string
  initialPageSize?: number
}) {
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [pagination, setPagination] = useState<PaginationState>({ pageIndex: 0, pageSize: initialPageSize })
  const [colFilters, setColFilters] = useState<Record<string, Set<string>>>({})
  const [menu, setMenu] = useState<{ key: string; label: string; el: HTMLElement } | null>(null)
  const keyOf = (c: GridColumn<T>) => (c as { accessorKey?: string }).accessorKey
  const textOf = (row: T, key: string) => {
    const v = (row as Record<string, unknown>)[key]
    if (v == null || v === '') return EMPTY_VALUE
    if (typeof v === 'boolean') return v ? 'نعم' : 'لا'
    if (typeof v === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(v)) return new Date(v).toLocaleDateString('en-GB')
    return String(v)
  }
  const filteredData = useMemo(() => {
    let out = data
    for (const [k, set] of Object.entries(colFilters)) out = out.filter(r => set.has(textOf(r, k)))
    return out
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data, colFilters])
  const menuValues = useMemo(() => {
    if (!menu) return []
    let base = data
    for (const [k, set] of Object.entries(colFilters)) if (k !== menu.key) base = base.filter(r => set.has(textOf(r, k)))
    return distinctValues(base, r => textOf(r, menu.key))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [menu, data, colFilters])
  const activeFilters = Object.keys(colFilters).length

  const table = useTable({
    features: gridFeatures,
    columns,
    data: filteredData,
    getRowId: row => getRowId(row),
    globalFilterFn: 'includesString',
    getColumnCanGlobalFilter: () => true,
    state: { globalFilter, sorting, pagination },
    onGlobalFilterChange: setGlobalFilter,
    onSortingChange: setSorting,
    onPaginationChange: setPagination,
  })

  const total = table.getPrePaginatedRowModel().rows.length
  const rows = table.getRowModel().rows
  const pageCount = table.getPageCount()

  return (
    <div className="card">
      <div className="dt-toolbar">
        <div className="input-wrap">
          <Search size={17} />
          <input className="input" placeholder={searchPlaceholder} value={globalFilter} onChange={e => table.setGlobalFilter(e.target.value)} />
          {globalFilter && <button className="btn ghost sm icon trail" onClick={() => table.setGlobalFilter('')} aria-label="مسح"><X size={15} /></button>}
        </div>
        {toolbar}
        {activeFilters > 0 && <button className="btn ghost sm" onClick={() => setColFilters({})}><FilterX size={15} /> مسح فلاتر الأعمدة ({activeFilters})</button>}
      </div>

      <div className="dt-scroll">
        <table className="dt">
          <thead>
            {table.getHeaderGroups().map(g => (
              <tr key={g.id}>
                {g.headers.map(h => {
                  const canSort = h.column.getCanSort()
                  const dir = h.column.getIsSorted()
                  return (
                    <th key={h.id} style={{ width: h.column.columnDef.meta?.width }} className={canSort ? 'sortable' : ''}
                      onClick={canSort ? h.column.getToggleSortingHandler() : undefined}
                      aria-sort={dir === 'asc' ? 'ascending' : dir === 'desc' ? 'descending' : undefined}>
                      <div className="dt-th">
                        <span className="lbl">{h.isPlaceholder ? null : <table.FlexRender header={h} />}
                          {canSort && <span className="sort">{dir === 'asc' ? <ArrowUp size={13} /> : dir === 'desc' ? <ArrowDown size={13} /> : <ArrowUpDown size={12} />}</span>}</span>
                        {(() => {
                          const k = keyOf(h.column.columnDef as GridColumn<T>)
                          if (!k) return null
                          const label = typeof h.column.columnDef.header === 'string' ? h.column.columnDef.header : k
                          return <ColumnFilterButton active={!!colFilters[k]} onClick={el => setMenu({ key: k, label, el })} />
                        })()}
                      </div>
                    </th>
                  )
                })}
              </tr>
            ))}
          </thead>
          <tbody>
            {loading
              ? Array.from({ length: 7 }).map((_, i) => (
                  <tr key={i}>{columns.map((_c, j) => <td key={j}><div className="sk" style={{ width: `${45 + ((i * 13 + j * 29) % 50)}%` }} /></td>)}</tr>
                ))
              : rows.map((row, i) => (
                  <motion.tr key={row.id} initial={{ opacity: 0, y: 4 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: Math.min(i * 0.015, 0.2), duration: 0.18 }}
                    className={onRowClick ? 'clickable' : ''} onClick={onRowClick ? () => onRowClick(row.original) : undefined}>
                    {row.getAllCells().map(cell => (
                      <td key={cell.id}><table.FlexRender cell={cell} /></td>
                    ))}
                  </motion.tr>
                ))}
          </tbody>
        </table>
        {!loading && rows.length === 0 && (
          <div className="empty">
            <div className="ic"><Inbox size={30} /></div>
            <b>{globalFilter || activeFilters ? 'لا توجد نتائج مطابقة' : emptyText}</b>
            {globalFilter && <span>جرّب كلمة بحث أخرى</span>}
          </div>
        )}
      </div>

      {menu && <ColumnFilterMenu anchor={menu.el} title={menu.label} values={menuValues} selected={colFilters[menu.key] ?? null}
        onApply={set => { setColFilters(f => { const n = { ...f }; if (set) n[menu.key] = set; else delete n[menu.key]; return n }); setPagination(p => ({ ...p, pageIndex: 0 })) }}
        onSort={dir => setSorting([{ id: menu.key, desc: dir === -1 }])} onClose={() => setMenu(null)} />}
      {!loading && total > 0 && (
        <div className="dt-foot">
          <span>{fmt(total)} سجل</span>
          <select className="input" style={{ width: 'auto', height: 32, paddingBlock: 0 }} value={pagination.pageSize}
            onChange={e => table.setPageSize(Number(e.target.value))} aria-label="عدد السجلات في الصفحة">
            {PAGE_SIZES.map(s => <option key={s} value={s}>{s} / صفحة</option>)}
          </select>
          {pageCount > 1 && (
            <div className="pager">
              <button className="btn ghost sm icon" disabled={!table.getCanPreviousPage()} onClick={() => table.firstPage()} aria-label="الأولى"><ChevronsRight size={16} /></button>
              <button className="btn ghost sm icon" disabled={!table.getCanPreviousPage()} onClick={() => table.previousPage()} aria-label="السابق"><ChevronRight size={16} /></button>
              <span>صفحة {fmt(pagination.pageIndex + 1)} من {fmt(pageCount)}</span>
              <button className="btn ghost sm icon" disabled={!table.getCanNextPage()} onClick={() => table.nextPage()} aria-label="التالي"><ChevronLeft size={16} /></button>
              <button className="btn ghost sm icon" disabled={!table.getCanNextPage()} onClick={() => table.lastPage()} aria-label="الأخيرة"><ChevronsLeft size={16} /></button>
            </div>
          )}
        </div>
      )}
    </div>
  )
}
