import { useEffect, useRef, type ReactNode } from 'react'
import { ErrorBoundary } from './ErrorBoundary'
import { createPortal } from 'react-dom'
import { motion } from 'motion/react'
import { Loader2, X } from 'lucide-react'

/* ---------------- Escape key + scroll lock for overlays ---------------- */
function useOverlay(onClose: () => void) {
  const ref = useRef(onClose)
  useEffect(() => { ref.current = onClose })
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      // SweetAlert handles its own Escape; don't close the drawer underneath it.
      if (e.key === 'Escape' && !document.querySelector('.swal2-container')) ref.current()
    }
    window.addEventListener('keydown', onKey)
    const prev = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => { window.removeEventListener('keydown', onKey); document.body.style.overflow = prev }
  }, [])
}

const fade = { initial: { opacity: 0 }, animate: { opacity: 1 }, exit: { opacity: 0 }, transition: { duration: 0.15 } }

/* ---------------- Dialog ---------------- */
export function Dialog({ title, subtitle, onClose, children, footer, size }: {
  title: ReactNode; subtitle?: ReactNode; onClose: () => void; children: ReactNode; footer?: ReactNode; size?: 'lg'
}) {
  useOverlay(onClose)
  return createPortal(
    <>
      <motion.div className="overlay" {...fade} onClick={onClose} />
      <div className="dialog-wrap">
        <motion.div className={`dialog ${size ?? ''}`} role="dialog" aria-modal="true"
          initial={{ opacity: 0, scale: 0.96, y: 8 }} animate={{ opacity: 1, scale: 1, y: 0 }} exit={{ opacity: 0, scale: 0.97 }}
          transition={{ type: 'spring', stiffness: 420, damping: 32 }}>
          <div className="dialog-head">
            <div><h3>{title}</h3>{subtitle && <p>{subtitle}</p>}</div>
            <button className="btn ghost sm icon" onClick={onClose} aria-label="إغلاق"><X size={18} /></button>
          </div>
          <div className="dialog-body">{children}</div>
          {footer && <div className="dialog-foot">{footer}</div>}
        </motion.div>
      </div>
    </>,
    document.body,
  )
}

/* ---------------- Drawer (record card) ---------------- */
export function Drawer({ icon, title, subtitle, onClose, children, footer, size, tabs }: {
  icon?: ReactNode; title: ReactNode; subtitle?: ReactNode; onClose: () => void; children: ReactNode; footer?: ReactNode; size?: 'lg' | 'xl'
  /** Optional tab strip rendered under the header. */
  tabs?: ReactNode
}) {
  useOverlay(onClose)
  return createPortal(
    <>
      <motion.div className="overlay" {...fade} onClick={onClose} />
      <motion.aside className={`drawer ${size ?? ''}`} role="dialog" aria-modal="true"
        initial={{ x: '-100%' }} animate={{ x: 0 }} exit={{ x: '-100%' }}
        transition={{ type: 'spring', stiffness: 380, damping: 38 }}>
        <div className="drawer-head">
          {icon && <div className="ic">{icon}</div>}
          <div><h3>{title}</h3>{subtitle && <p>{subtitle}</p>}</div>
          <button className="btn ghost icon close" onClick={onClose} aria-label="إغلاق"><X size={20} /></button>
        </div>
        {tabs}
        <div className="drawer-body"><ErrorBoundary compact>{children}</ErrorBoundary></div>
        {footer && <div className="drawer-foot">{footer}</div>}
      </motion.aside>
    </>,
    document.body,
  )
}

/* ---------------- Form bits ---------------- */
export function Field({ label, required, hint, error, children, className }: {
  label: ReactNode; required?: boolean; hint?: ReactNode; error?: string; children: ReactNode; className?: string
}) {
  return (
    <div className={`field ${className ?? ''}`}>
      <label>{label}{required && <span className="req">*</span>}</label>
      {children}
      {error
        ? <motion.span className="err" initial={{ opacity: 0, y: -3 }} animate={{ opacity: 1, y: 0 }}>{error}</motion.span>
        : hint ? <span className="hint">{hint}</span> : null}
    </div>
  )
}

export function Switch({ checked, onChange, label, disabled }: {
  checked: boolean; onChange: (v: boolean) => void; label: ReactNode; disabled?: boolean
}) {
  return (
    <label className="switch">
      <input type="checkbox" checked={checked} disabled={disabled} onChange={e => onChange(e.target.checked)} />
      {label}
    </label>
  )
}

export function Segmented<T extends string | number | null>({ value, onChange, options }: {
  value: T; onChange: (v: T) => void; options: { value: T; label: string }[]
}) {
  return (
    <div className="seg" role="radiogroup">
      {options.map(o => (
        <button key={String(o.value)} type="button" className={o.value === value ? 'on' : ''}
          role="radio" aria-checked={o.value === value} onClick={() => onChange(o.value)}>
          {o.value === value && <motion.span layoutId={`seg-${options.map(x => x.value).join()}`} className="seg-pill" transition={{ type: 'spring', stiffness: 500, damping: 38 }} />}
          <span style={{ position: 'relative' }}>{o.label}</span>
        </button>
      ))}
    </div>
  )
}

export const Spinner = ({ size = 18 }: { size?: number }) => <Loader2 size={size} className="spin" />

export function StatusBadge({ on, onText = 'فعال', offText = 'موقوف' }: { on: boolean | null | undefined; onText?: string; offText?: string }) {
  return <span className={`badge ${on ? 'ok' : 'off'}`}>{on ? onText : offText}</span>
}

/** Animated stat tile. */
export function Stat({ icon, value, label, tone, delay = 0 }: { icon: ReactNode; value: ReactNode; label: string; tone?: 'danger'; delay?: number }) {
  return (
    <motion.div className="stat" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay, duration: 0.3 }}>
      <span className={`ic ${tone ?? ''}`}>{icon}</span>
      <div><b>{value}</b><span>{label}</span></div>
    </motion.div>
  )
}

/* ---------------- Tabs (animated underline) ---------------- */
export function Tabs<T extends string>({ value, onChange, items }: {
  value: T; onChange: (v: T) => void; items: { value: T; label: ReactNode; badge?: ReactNode }[]
}) {
  return (
    <div className="tabs" role="tablist">
      {items.map(t => (
        <button key={t.value} role="tab" aria-selected={t.value === value} className={t.value === value ? 'on' : ''} onClick={() => onChange(t.value)}>
          {t.label}{t.badge != null && <span className="tab-badge">{t.badge}</span>}
          {t.value === value && <motion.span layoutId="tab-line" className="tab-line" transition={{ type: 'spring', stiffness: 500, damping: 40 }} />}
        </button>
      ))}
    </div>
  )
}

/** Small square checkbox used in permission matrices. */
export function Check({ checked, onChange, disabled, label, indeterminate }: {
  checked: boolean; onChange: (v: boolean) => void; disabled?: boolean; label?: string; indeterminate?: boolean
}) {
  return (
    <button type="button" role="checkbox" aria-checked={indeterminate ? 'mixed' : checked} aria-label={label} title={label} disabled={disabled}
      className={`cbx ${checked ? 'on' : ''} ${indeterminate ? 'mixed' : ''}`} onClick={() => onChange(!checked)}>
      {checked ? '✓' : indeterminate ? '–' : ''}
    </button>
  )
}
