import { useState, type ReactNode } from 'react'
import { AnimatePresence } from 'motion/react'
import { AlertTriangle, FileSpreadsheet, Printer, Search, X } from 'lucide-react'
import { useBranches } from '../lib/lookups'
import { exportCsv, printReport, type ReportCol } from '../lib/print'
import { CostCenterPicker } from './CostCenterPicker'
import { Spinner } from './ui'
import type { RCol } from './ReportGrid'

/* Shared building blocks of every report screen: compact filter bar, dates, branch, summary strip, print/Excel. */
export const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
export const count = (n: number) => (Number(n) || 0).toLocaleString('ar-EG')
export const day = (v: string) => new Date(v).toLocaleDateString('en-GB')
export const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
export const monthStart = () => today().slice(0, 8) + '01'
export const yearStart = () => `${new Date().getFullYear()}-01-01`
export type Params = Record<string, string | null>

export function exportOf<T>(cols: RCol<T>[], rows: T[]) {
  const c = cols.filter(x => !x.hidden && x.key !== 'actions')
  const v = (x: RCol<T>, r: T) => (x.value ? x.value(r) : (r as Record<string, unknown>)[x.key] as string | number | null | undefined)
  return { cols: c.map(x => ({ label: x.group ? `${x.group} ${x.label}` : x.label, num: x.num }) as ReportCol), rows: rows.map(r => c.map(x => v(x, r))) }
}
export function Exports<T>({ title, sub, cols, rows, foot }: { title: string; sub: string; cols: RCol<T>[]; rows: T[]; foot?: (string | number)[] }) {
  const e = () => exportOf(cols, rows)
  return <>
    <button className="btn ghost sm" onClick={() => { const x = e(); printReport(title, sub, x.cols, x.rows, foot) }}><Printer size={15} /> طباعة</button>
    <button className="btn ghost sm" onClick={() => { const x = e(); exportCsv(`${title} ${sub}`, x.cols, x.rows) }}><FileSpreadsheet size={15} /> Excel</button>
  </>
}

export function Bar({ children, onRun, loading, disabled }: { children: ReactNode; onRun: () => void; loading?: boolean; disabled?: boolean }) {
  return (
    <div className="rb" onKeyDown={e => { if (e.key === 'Enter' && (e.target as HTMLElement).tagName === 'INPUT') onRun() }}>
      {children}
      <button className="btn rb-run" onClick={onRun} disabled={loading || disabled}>{loading ? <Spinner /> : <><Search size={16} /> عرض</>}</button>
    </div>
  )
}
export const F = ({ label, children, w }: { label: string; children: ReactNode; w?: number }) => <label className="rb-f" style={w ? { minWidth: w } : undefined}><small>{label}</small>{children}</label>
export function Dates({ from, to, setFrom, setTo }: { from: string; to: string; setFrom: (v: string) => void; setTo: (v: string) => void }) {
  return <>
    <F label="من"><input className="input" type="date" value={from} onChange={e => setFrom(e.target.value)} /></F>
    <F label="إلى"><input className="input" type="date" value={to} onChange={e => setTo(e.target.value)} /></F>
  </>
}
export function BranchSelect({ value, onChange, all = 'كل الفروع' }: { value: string; onChange: (v: string) => void; all?: string }) {
  const b = useBranches()
  return <F label="الفرع" w={190}><select className="input" value={value} onChange={e => onChange(e.target.value)}>
    <option value="">{all}</option>{b.data?.map(x => <option key={x.guid} value={x.guid}>{x.name}</option>)}</select></F>
}
export function Select({ label, value, onChange, list, all, w = 180 }: { label: string; value: string; onChange: (v: string) => void; list?: { guid: string; name: string }[]; all: string; w?: number }) {
  return <F label={label} w={w}><select className="input" value={value} onChange={e => onChange(e.target.value)}>
    <option value="">{all}</option>{list?.map(x => <option key={x.guid} value={x.guid}>{x.name}</option>)}</select></F>
}
export function SearchBox({ value, onChange, placeholder = 'الاسم أو الهوية أو الرقم', w = 210 }: { value: string; onChange: (v: string) => void; placeholder?: string; w?: number }) {
  return <F label="بحث" w={w}><input className="input" placeholder={placeholder} value={value} onChange={e => onChange(e.target.value)} /></F>
}
export function Picker({ label, value, placeholder, onOpen, onClear, w = 210 }: { label: string; value: string | null; placeholder: string; onOpen: () => void; onClear?: () => void; w?: number }) {
  return <F label={label} w={w}>
    <div className="rb-pick">
      <button type="button" className="input" onClick={onOpen}>{value ? <span>{value}</span> : <span className="muted">{placeholder}</span>}<Search size={14} /></button>
      {value && onClear && <button type="button" className="btn ghost sm icon" onClick={onClear} aria-label="مسح"><X size={14} /></button>}
    </div>
  </F>
}
export function CostSelect({ value, onChange }: { value: { guid: string; name: string } | null; onChange: (v: { guid: string; name: string } | null) => void }) {
  const [open, setOpen] = useState(false)
  return <>
    <Picker label="مركز التكلفة" value={value?.name ?? null} placeholder="كل المراكز" onOpen={() => setOpen(true)} onClear={() => onChange(null)} />
    <AnimatePresence>{open && <CostCenterPicker onClose={() => setOpen(false)} onPick={x => { onChange({ guid: x.guid, name: x.name }); setOpen(false) }} />}</AnimatePresence>
  </>
}
export const Summary = ({ items }: { items: { label: string; value: ReactNode; tone?: 'due' | 'ok' | 'brand' }[] }) =>
  <div className="rs">{items.map(i => <div key={i.label} className={i.tone ?? ''}><small>{i.label}</small><b>{i.value}</b></div>)}</div>
export const Waiting = ({ loading, text = 'حدد الفلاتر ثم اضغط "عرض"', error }: { loading: boolean; text?: string; error?: string | null }) =>
  loading ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div>
    : error ? <div className="rg-wait err"><AlertTriangle size={28} /><b>{error}</b></div>
    : <div className="rg-wait"><Search size={28} /><b>{text}</b></div>
