import { useState } from 'react'
import { AnimatePresence, motion } from 'motion/react'
import { Check, Monitor, Moon, Palette, Sun } from 'lucide-react'
import { ACCENTS, useTheme, type Mode } from '../theme'
import { Segmented } from './ui'

/** Top-bar button: light/dark/system + accent colors, with a live preview. */
export function ThemePicker() {
  const { mode, accent, resolved, setMode, setAccent, toggle } = useTheme()
  const [open, setOpen] = useState(false)

  return (
    <div className="pop-anchor row" style={{ gap: 2 }}>
      <button className="btn ghost icon" onClick={toggle} title={resolved === 'dark' ? 'الوضع الفاتح' : 'الوضع الداكن'} aria-label="تبديل الوضع">
        <AnimatePresence mode="wait" initial={false}>
          <motion.span key={resolved} style={{ display: 'inline-flex' }}
            initial={{ rotate: -90, scale: 0.5, opacity: 0 }} animate={{ rotate: 0, scale: 1, opacity: 1 }} exit={{ rotate: 90, scale: 0.5, opacity: 0 }} transition={{ duration: 0.2 }}>
            {resolved === 'dark' ? <Sun size={19} /> : <Moon size={19} />}
          </motion.span>
        </AnimatePresence>
      </button>
      <button className="btn ghost icon" onClick={() => setOpen(o => !o)} title="الثيمات والألوان" aria-label="الثيمات والألوان">
        <Palette size={19} />
      </button>
      <AnimatePresence>
        {open && (
          <>
            <div className="click-away" onClick={() => setOpen(false)} />
            <motion.div className="menu-pop theme-pop" initial={{ opacity: 0, scale: 0.95, y: -6 }} animate={{ opacity: 1, scale: 1, y: 0 }} exit={{ opacity: 0, scale: 0.95, y: -6 }} transition={{ duration: 0.15 }}>
              <h4>المظهر</h4>
              <Segmented<Mode> value={mode} onChange={setMode} options={[
                { value: 'light', label: 'فاتح' }, { value: 'dark', label: 'داكن' }, { value: 'system', label: 'تلقائي' },
              ]} />
              <div className="row muted" style={{ fontSize: 11.5, margin: '6px 2px 14px', gap: 6 }}>
                {mode === 'system' ? <Monitor size={13} /> : resolved === 'dark' ? <Moon size={13} /> : <Sun size={13} />}
                {mode === 'system' ? `يتبع إعدادات جهازك (${resolved === 'dark' ? 'داكن' : 'فاتح'} الآن)` : resolved === 'dark' ? 'مريح للعين في الإضاءة المنخفضة' : 'واضح ومشرق'}
              </div>
              <h4>لون النظام</h4>
              <div className="swatches">
                {ACCENTS.map(a => (
                  <button key={a.key} className={`swatch ${accent === a.key ? 'on' : ''}`} style={{ color: a.color }} onClick={() => setAccent(a.key)} aria-pressed={accent === a.key}>
                    <i style={{ background: `linear-gradient(135deg, ${a.color}, color-mix(in oklab, ${a.color} 60%, black))` }}>
                      {accent === a.key && <motion.span initial={{ scale: 0 }} animate={{ scale: 1 }}><Check size={17} strokeWidth={3} /></motion.span>}
                    </i>
                    <span style={{ color: 'var(--ink-2)' }}>{a.label}</span>
                  </button>
                ))}
              </div>
              <div className="preview" aria-hidden>
                <div className="ps" />
                <div className="pm"><span /><span /><span style={{ width: '70%' }} /></div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
