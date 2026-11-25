import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from 'react'
import { flushSync } from 'react-dom'

/**
 * Theme: mode (light / dark / auto) + ANY brand colour.
 * The whole palette is derived from one CSS variable (--brand) with color-mix, so a visitor can pick a preset
 * or any colour at all (hue slider / colour picker) and every shade, button and chart follows.
 * Changes animate with a circular reveal from the click point (View Transitions) where supported.
 */

export type Mode = 'light' | 'dark' | 'auto'
/**
 * The institute's identity colours (main + sub), the default colour and the secondary («gold») colour all come
 * from the dashboard (settings → general: theme_main / theme_sub / theme_color / theme_secondary).
 * These presets are only the extra «ألوان أخرى»; 'semou' is a fallback used before the settings load.
 */
export const DEFAULT_ACCENT = 'semou'
/** A dashboard colour list row: a = name, b = #hex. */
export type PaletteColor = { a: string; b: string }

/** The dashboard's secondary colour («gold» highlights). Remembered so the next visit paints it at once. */
export function applySecondary(color?: string | null) {
  const el = document.documentElement
  const ok = !!color && /^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(color)
  if (ok) el.style.setProperty('--sub', color!)
  else el.style.removeProperty('--sub')
  try { if (ok) localStorage.setItem('alsmow-theme-sub', color!); else localStorage.removeItem('alsmow-theme-sub') } catch { /* */ }
}
export const ACCENTS = [
  { id: 'semou', label: 'السمو', color: '#3f7a58' },
  { id: 'emerald', label: 'زمردي', color: '#0f9d6c' },
  { id: 'teal', label: 'تركواز', color: '#0e8a8c' },
  { id: 'sky', label: 'سماوي', color: '#0284c7' },
  { id: 'ocean', label: 'محيطي', color: '#2563eb' },
  { id: 'navy', label: 'كحلي', color: '#1e3a8a' },
  { id: 'indigo', label: 'نيلي', color: '#4f46e5' },
  { id: 'royal', label: 'ملكي', color: '#7c3aed' },
  { id: 'lavender', label: 'لافندر', color: '#8b7cf6' },
  { id: 'plum', label: 'برقوقي', color: '#86198f' },
  { id: 'rose', label: 'وردي', color: '#db2777' },
  { id: 'ruby', label: 'ياقوتي', color: '#be123c' },
  { id: 'coral', label: 'مرجاني', color: '#f04e3e' },
  { id: 'sunset', label: 'غروب', color: '#ea580c' },
  { id: 'gold', label: 'ذهبي', color: '#a87a1d' },
  { id: 'olive', label: 'زيتوني', color: '#65762a' },
  { id: 'mocha', label: 'موكا', color: '#8b5e3c' },
  { id: 'graphite', label: 'فحمي', color: '#475569' },
] as const
/** A preset id, or any CSS colour the visitor picked (e.g. "#ff5a7a" or "oklch(0.6 0.15 210)"). */
export type Accent = string

const KEY = 'alsmow-theme'
const DEFAULTS_KEY = 'alsmow-theme-defaults' // the dashboard's defaults, remembered so the first paint already uses them
interface Saved { mode?: Mode; accent?: Accent }

function read(key = KEY): Saved {
  try {
    return JSON.parse(localStorage.getItem(key) || '{}') as Saved
  } catch {
    return {}
  }
}
function write(v: Saved) {
  try {
    localStorage.setItem(KEY, JSON.stringify(v))
  } catch {
    /* private mode: theme just won't persist */
  }
}

export const isPreset = (a: string) => ACCENTS.some((x) => x.id === a)
export const accentColor = (a: Accent) => ACCENTS.find((x) => x.id === a)?.color ?? (CSS.supports('color', a) ? a : '#3f7a58')

const media = () => window.matchMedia('(prefers-color-scheme: dark)')

function apply(mode: Mode, accent: Accent) {
  const el = document.documentElement
  const resolved = mode === 'auto' ? (media().matches ? 'dark' : 'light') : mode
  const color = accentColor(accent)
  el.dataset.mode = resolved
  el.dataset.accent = isPreset(accent) ? accent : 'custom'
  el.style.setProperty('--brand', color)
  el.style.setProperty('--glow', `color-mix(in oklab, ${color} 45%, white)`)
  try { localStorage.setItem('alsmow-theme-color', color) } catch { /* */ }
  document.querySelector('meta[name=theme-color]')?.setAttribute('content', resolved === 'dark' ? '#0b100e' : '#faf8f4')
}

/** Runs a theme change with a circular reveal from (x, y) when the browser supports View Transitions. */
function reveal(change: () => void, at?: { x: number; y: number }) {
  const d = document as Document & { startViewTransition?: (cb: () => void) => { ready: Promise<void> } }
  // phones / tablets / touch: change at once. The circular reveal snapshots the whole page (a View Transition),
  // which is far too heavy for mobile GPUs and made the theme picker and the mobile menu stutter.
  const lite = window.matchMedia('(hover: none), (pointer: coarse), (max-width: 1023px)').matches
  if (!d.startViewTransition || lite || window.matchMedia('(prefers-reduced-motion: reduce)').matches) { change(); return }
  const x = at?.x ?? innerWidth / 2
  const y = at?.y ?? innerHeight / 2
  const r = Math.hypot(Math.max(x, innerWidth - x), Math.max(y, innerHeight - y))
  const t = d.startViewTransition(() => flushSync(change))
  t.ready.then(() => {
    document.documentElement.animate(
      { clipPath: [`circle(0px at ${x}px ${y}px)`, `circle(${r}px at ${x}px ${y}px)`] },
      { duration: 650, easing: 'cubic-bezier(.22,1,.36,1)', pseudoElement: '::view-transition-new(root)' },
    )
  }).catch(() => {})
}

type At = { x: number; y: number }
interface Ctx {
  mode: Mode; accent: Accent; resolved: 'light' | 'dark'
  setMode: (m: Mode, at?: At) => void
  setAccent: (a: Accent, at?: At) => void
  resetAccent: (at?: At) => void
  setDefaults: (a?: string, m?: string) => void
}
const ThemeCtx = createContext<Ctx | null>(null)

export function ThemeProvider({ children }: { children: ReactNode }) {
  const saved = read()
  const defs = read(DEFAULTS_KEY)
  const [mode, setModeState] = useState<Mode>(saved.mode ?? defs.mode ?? 'light')
  const [accent, setAccentState] = useState<Accent>(saved.accent ?? defs.accent ?? DEFAULT_ACCENT)
  const [resolved, setResolved] = useState<'light' | 'dark'>(() => (document.documentElement.dataset.mode === 'dark' ? 'dark' : 'light'))

  useEffect(() => {
    apply(mode, accent)
    setResolved(document.documentElement.dataset.mode === 'dark' ? 'dark' : 'light')
    if (mode !== 'auto') return
    const m = media()
    const h = () => { apply('auto', accent); setResolved(m.matches ? 'dark' : 'light') }
    m.addEventListener('change', h)
    return () => m.removeEventListener('change', h)
  }, [mode, accent])

  const setMode = useCallback((m: Mode, at?: At) => {
    reveal(() => { setModeState(m); apply(m, read().accent ?? accent) }, at)
    write({ ...read(), mode: m })
  }, [accent])
  const setAccent = useCallback((a: Accent, at?: At) => {
    const big = isPreset(a) // the slider fires continuously: only presets get the reveal animation
    if (big) reveal(() => { setAccentState(a); apply(read().mode ?? mode, a) }, at)
    else { setAccentState(a); apply(read().mode ?? mode, a) }
    write({ ...read(), accent: a })
  }, [mode])
  const resetAccent = useCallback((at?: At) => {
    const def = read(DEFAULTS_KEY).accent ?? DEFAULT_ACCENT
    reveal(() => { setAccentState(def); apply(read().mode ?? mode, def) }, at)
    const s = read()
    delete s.accent
    write(s)
  }, [mode])
  // Site-wide defaults from the dashboard apply only when the visitor hasn't chosen their own.
  const setDefaults = useCallback((a?: string, m?: string) => {
    const s = read()
    // a preset id, or a colour code set in the dashboard (e.g. #8b1538)
    const okA = !!a && (isPreset(a) || /^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(a))
    const okM = !!m && ['light', 'dark', 'auto'].includes(m)
    try { localStorage.setItem(DEFAULTS_KEY, JSON.stringify({ ...(okA ? { accent: a } : {}), ...(okM ? { mode: m } : {}) })) } catch { /* */ }
    if (!s.accent && okA) setAccentState(a as Accent)
    if (!s.mode && okM) setModeState(m as Mode)
  }, [])

  return <ThemeCtx.Provider value={{ mode, accent, resolved, setMode, setAccent, resetAccent, setDefaults }}>{children}</ThemeCtx.Provider>
}

export function useTheme() {
  const c = useContext(ThemeCtx)
  if (!c) throw new Error('useTheme outside ThemeProvider')
  return c
}
