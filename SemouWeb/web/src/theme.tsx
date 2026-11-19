import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState, useSyncExternalStore, type ReactNode } from 'react'

export type Mode = 'light' | 'dark' | 'system'
export type Accent = 'semou' | 'ocean' | 'royal' | 'teal' | 'rose' | 'sunset' | 'gold' | 'graphite'

/** Theme accents. "semou" is the alsmow.com brand green and the default. */
export const ACCENTS: { key: Accent; label: string; color: string }[] = [
  { key: 'semou', label: 'السمو', color: '#4a7f5f' },
  { key: 'ocean', label: 'محيطي', color: '#2563eb' },
  { key: 'royal', label: 'ملكي', color: '#7c3aed' },
  { key: 'teal', label: 'تركواز', color: '#0e8a8c' },
  { key: 'rose', label: 'وردي', color: '#e11d48' },
  { key: 'sunset', label: 'غروب', color: '#ea580c' },
  { key: 'gold', label: 'ذهبي', color: '#b07d1c' },
  { key: 'graphite', label: 'فحمي', color: '#475569' },
]

const KEY = 'semou.theme'
interface Saved { mode: Mode; accent: Accent }

function read(): Saved {
  try {
    const s = JSON.parse(localStorage.getItem(KEY) ?? '') as Partial<Saved>
    return {
      mode: s.mode === 'light' || s.mode === 'dark' || s.mode === 'system' ? s.mode : 'system',
      accent: ACCENTS.some(a => a.key === s.accent) ? s.accent! : 'semou',
    }
  } catch {
    return { mode: 'system', accent: 'semou' }
  }
}

/** Applied before React renders (called from main.tsx) so the page never flashes the wrong theme. */
export function applyInitialTheme() {
  const s = read()
  apply(s.mode === 'system' ? systemMode() : s.mode, s.accent)
}

const mq = () => window.matchMedia('(prefers-color-scheme: dark)')
const systemMode = () => (mq().matches ? 'dark' : 'light')

function apply(mode: 'light' | 'dark', accent: Accent) {
  const html = document.documentElement
  html.dataset.mode = mode
  html.dataset.accent = accent
  const color = ACCENTS.find(a => a.key === accent)!.color
  document.querySelector('meta[name="theme-color"]')?.setAttribute('content', mode === 'dark' ? '#0d1110' : color)
}

interface ThemeCtx {
  mode: Mode
  resolved: 'light' | 'dark'
  accent: Accent
  setMode: (m: Mode) => void
  setAccent: (a: Accent) => void
  toggle: () => void
}
const Ctx = createContext<ThemeCtx | null>(null)

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [saved, setSaved] = useState<Saved>(read)
  const sys = useSyncExternalStore(
    cb => { const m = mq(); m.addEventListener('change', cb); return () => m.removeEventListener('change', cb) },
    systemMode,
  )
  const resolved = saved.mode === 'system' ? sys : saved.mode

  const first = useRef(true)
  useEffect(() => {
    localStorage.setItem(KEY, JSON.stringify(saved))
    // The page is snapshotted once and cross-faded as a single layer (View Transitions), so switching costs the
    // same on a 10-row screen and on a 5,000-row journal. Animating every element's colors was what lagged.
    const html = document.documentElement
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches
    const doc = document as Document & { startViewTransition?: (cb: () => void) => unknown }
    if (first.current || reduce || !doc.startViewTransition || html.dataset.mode === resolved && html.dataset.accent === saved.accent) apply(resolved, saved.accent)
    else doc.startViewTransition(() => apply(resolved, saved.accent))
    first.current = false
  }, [resolved, saved])

  const setMode = useCallback((mode: Mode) => setSaved(s => ({ ...s, mode })), [])
  const setAccent = useCallback((accent: Accent) => setSaved(s => ({ ...s, accent })), [])

  const value = useMemo<ThemeCtx>(() => ({
    mode: saved.mode, accent: saved.accent, resolved, setMode, setAccent,
    toggle: () => setMode(resolved === 'dark' ? 'light' : 'dark'),
  }), [saved, resolved, setMode, setAccent])

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>
}

export function useTheme() {
  const c = useContext(Ctx)
  if (!c) throw new Error('useTheme outside ThemeProvider')
  return c
}
