import { Check, Monitor, Moon, Palette, Pipette, RotateCcw, Sun } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useRef, useState, type PointerEvent as RPointerEvent } from 'react'
import { cx } from '../lib/format'
import { ACCENTS, accentColor, isPreset, useTheme, type Mode, type PaletteColor } from '../lib/theme'
import { useSite } from '../lib/queries'
import { track } from '../lib/track'

/**
 * «استوديو الألوان»: light / dark / auto, 18 ready palettes, and ANY colour the visitor wants
 * (a rainbow hue slider with a live preview, the device's colour picker, or a hex code).
 */
export function ThemePicker({ align = 'left', up }: { align?: 'left' | 'right'; up?: boolean }) {
  const t = useTheme()
  const site = useSite()
  const gen = site.data?.settings.general
  // identity colours from the dashboard: picking one sets that exact colour (a custom accent)
  const fromDash = (list?: PaletteColor[]) =>
    (Array.isArray(list) ? list : []).filter((c) => /^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(c?.b ?? '')).map((c) => ({ id: c.b.toLowerCase(), label: c.a, color: c.b }))
  const main = fromDash(gen?.theme_main)
  const sub = fromDash(gen?.theme_sub)
  const presets = ACCENTS.map((a) => ({ id: a.id as string, label: a.label as string, color: a.color as string }))
  const groups = [
    ...(main.length ? [{ key: 'main', title: 'ألوان الهوية الأساسية', items: main }] : []),
    ...(sub.length ? [{ key: 'sub', title: 'ألوان الهوية الفرعية', items: sub }] : []),
    { key: 'more', title: main.length || sub.length ? 'ألوان أخرى' : 'ألوان جاهزة', items: presets },
  ]
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)
  const custom = !isPreset(t.accent)
  const [hue, setHue] = useState(() => hueOf(accentColor(t.accent)))
  const sent = useRef(false)
  useEffect(() => {
    if (!open) return
    const h = (e: PointerEvent) => { if (!ref.current?.contains(e.target as Node)) setOpen(false) }
    document.addEventListener('pointerdown', h)
    return () => document.removeEventListener('pointerdown', h)
  }, [open])
  const at = (e: RPointerEvent | React.MouseEvent) => ({ x: e.clientX, y: e.clientY })
  const used = () => { if (!sent.current) { sent.current = true; track('theme_change') } }
  const modes: { id: Mode; label: string; icon: typeof Sun }[] = [
    { id: 'light', label: 'فاتح', icon: Sun },
    { id: 'dark', label: 'داكن', icon: Moon },
    { id: 'auto', label: 'تلقائي', icon: Monitor },
  ]
  // HSL keeps it a plain sRGB colour (works with the native picker); 42% lightness keeps white text readable
  const fromHue = (h: number) => toHex(`hsl(${Math.round(h)} 58% 42%)`)

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen(!open)}
        className="theme-btn grid h-11 w-11 place-items-center rounded-2xl border border-line bg-surface/70 text-ink-2 transition hover:text-brand-700"
        aria-label="المظهر والألوان"
        aria-expanded={open}
      >
        <Palette className="h-5 w-5" />
      </button>
      <AnimatePresence>
        {open && (
          <motion.div
            initial={{ opacity: 0, y: up ? -10 : 10, scale: 0.96 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: up ? -10 : 10, scale: 0.96 }}
            transition={{ type: 'spring', damping: 26, stiffness: 340 }}
            className={cx('absolute z-50 max-h-[80dvh] w-[min(20rem,calc(100vw-2rem))] overflow-y-auto rounded-3xl border border-line bg-surface p-4 shadow-lift', up ? 'bottom-full mb-3 origin-bottom' : 'top-full mt-3 origin-top', align === 'left' ? 'left-0' : 'right-0')}
            data-no-texts-edit
          >
            <div className="mb-2 text-xs font-bold text-muted">المظهر</div>
            <div className="grid grid-cols-3 gap-1 rounded-2xl bg-sand p-1">
              {modes.map((m) => (
                <button
                  key={m.id}
                  onClick={(e) => { t.setMode(m.id, at(e)); used() }}
                  className={cx('relative flex flex-col items-center gap-1 rounded-xl py-2 text-xs font-semibold transition', t.mode === m.id ? 'text-ink' : 'text-muted hover:text-ink')}
                >
                  {t.mode === m.id && <motion.span layoutId="mode-pill" className="absolute inset-0 rounded-xl bg-surface shadow-soft" />}
                  <m.icon className="relative h-4 w-4" />
                  <span className="relative">{m.label}</span>
                </button>
              ))}
            </div>

            {/* the identity palette (main + sub) comes from the dashboard; the presets follow as «ألوان أخرى» */}
            {groups.map((g, gi) => (
              <div key={g.key}>
                <div className={cx('mb-2 flex items-center justify-between text-xs font-bold text-muted', gi === 0 ? 'mt-4' : 'mt-3')}>
                  <span>{g.title}</span>
                  {gi === 0 && <button onClick={(e) => { t.resetAccent(at(e)); used() }} className="flex items-center gap-1 font-semibold hover:text-ink" title="العودة لألوان المعهد"><RotateCcw className="h-3 w-3" /> الافتراضي</button>}
                </div>
                <div className="grid grid-cols-6 gap-2">
                  {g.items.map((a, i) => {
                    const on = t.accent.toLowerCase() === a.id.toLowerCase()
                    return (
                      <motion.button
                        key={a.id}
                        initial={{ opacity: 0, scale: 0.6 }}
                        animate={{ opacity: 1, scale: 1 }}
                        transition={{ delay: i * 0.015, type: 'spring', stiffness: 500, damping: 26 }}
                        whileHover={{ scale: 1.15, rotate: -6 }}
                        whileTap={{ scale: 0.9 }}
                        onClick={(e) => { t.setAccent(a.id, at(e)); setHue(hueOf(a.color)); used() }}
                        className={cx('grid aspect-square place-items-center rounded-full ring-offset-2 ring-offset-surface', on && 'ring-2')}
                        style={{ background: `conic-gradient(from 220deg, ${a.color}, color-mix(in oklab, ${a.color} 55%, white), ${a.color})`, ['--tw-ring-color' as string]: a.color }}
                        title={a.label}
                        aria-label={a.label}
                      >
                        {on && <Check className="h-3.5 w-3.5 text-white drop-shadow" />}
                      </motion.button>
                    )
                  })}
                </div>
              </div>
            ))}

            <div className="mt-4 mb-2 flex items-center justify-between text-xs font-bold text-muted">
              <span>لونك الخاص</span>
              {custom && <span className="flex items-center gap-1 text-brand-700"><span className="h-2 w-2 rounded-full bg-primary" /> مُفعّل</span>}
            </div>
            <input
              type="range"
              min={0}
              max={360}
              value={hue}
              onChange={(e) => { const h = +e.target.value; setHue(h); t.setAccent(fromHue(h)); used() }}
              className="hue-slider w-full"
              aria-label="اختر درجة اللون"
              style={{ ['--thumb' as string]: fromHue(hue) }}
            />
            <div className="mt-3 flex items-center gap-2">
              <label className="relative grid h-10 w-10 shrink-0 cursor-pointer place-items-center overflow-hidden rounded-xl border border-line" style={{ background: accentColor(t.accent) }} title="اختيار لون من الجهاز">
                <Pipette className="h-4 w-4 text-white drop-shadow" />
                <input type="color" className="absolute inset-0 cursor-pointer opacity-0" value={toHex(accentColor(t.accent))} onChange={(e) => { t.setAccent(e.target.value); setHue(hueOf(e.target.value)); used() }} />
              </label>
              <HexInput value={toHex(accentColor(t.accent))} onPick={(c) => { t.setAccent(c); setHue(hueOf(c)); used() }} />
            </div>
            <div className="mt-3 flex gap-1.5" aria-hidden>
              {[7, 20, 48, 74, 100].map((k) => <span key={k} className="h-2 flex-1 rounded-full" style={{ background: `color-mix(in oklab, var(--brand) ${k}%, var(--surface))` }} />)}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}

function HexInput({ value, onPick }: { value: string; onPick: (c: string) => void }) {
  const [v, setV] = useState(value)
  useEffect(() => setV(value), [value])
  return (
    <input
      className="input !h-10 flex-1 text-left font-mono text-sm uppercase"
      dir="ltr"
      maxLength={7}
      value={v}
      onChange={(e) => {
        const x = e.target.value.startsWith('#') ? e.target.value : '#' + e.target.value
        setV(x)
        if (/^#[0-9a-f]{6}$/i.test(x)) onPick(x)
      }}
      aria-label="كود اللون"
    />
  )
}

/* ---- colour helpers (via the browser, so any CSS colour works) ---- */
function toHex(c: string) {
  if (/^#[0-9a-f]{6}$/i.test(c)) return c.toLowerCase()
  const cv = document.createElement('canvas').getContext('2d')
  if (cv) { cv.fillStyle = '#000'; cv.fillStyle = c; const v = cv.fillStyle as string; if (/^#[0-9a-f]{6}$/i.test(v)) return v }
  return '#3f7a58'
}
function hueOf(c: string) {
  const hex = toHex(c)
  const r = parseInt(hex.slice(1, 3), 16) / 255, g = parseInt(hex.slice(3, 5), 16) / 255, b = parseInt(hex.slice(5, 7), 16) / 255
  const mx = Math.max(r, g, b), mn = Math.min(r, g, b), d = mx - mn
  if (!d) return 150
  let h = mx === r ? ((g - b) / d) % 6 : mx === g ? (b - r) / d + 2 : (r - g) / d + 4
  h *= 60
  return Math.round(h < 0 ? h + 360 : h)
}
