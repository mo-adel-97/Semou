import { X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useLenis } from '../components/ui'
import { useSite } from '../lib/queries'

/**
 * نافذة الإعلان: the campaign picture shown once the site opens (after the opening scene), a wide picture on computers
 * and a tall one on phones; a tap opens the registration page (or the link set in the dashboard). Everything — on/off,
 * both pictures, the link, the button text, how often — comes from Settings › نافذة الإعلان.
 */

const KEY = 'alsmow.promo' // sessionStorage / localStorage: which picture was already shown (a new picture shows again)
const DELAY = 900 // after the page (and the opening scene) settles

function seen(id: string, mode: string) {
  try {
    if (new URLSearchParams(location.search).has('promo')) return false
    if (mode === 'first') return localStorage.getItem(KEY) === id
    if (mode === 'always') return false
    return sessionStorage.getItem(KEY) === id
  } catch { return false }
}
function markSeen(id: string) {
  try { sessionStorage.setItem(KEY, id); localStorage.setItem(KEY, id) } catch { /* private mode */ }
}

export default function PromoPopup() {
  const p = useSite().data?.settings.popup
  const nav = useNavigate()
  const lenis = useLenis()
  const [open, setOpen] = useState(false)
  const desktop = p?.image_desktop || p?.image_mobile || ''
  const mobile = p?.image_mobile || p?.image_desktop || ''
  const id = `${desktop}|${mobile}`
  const mode = p?.mode || 'session'
  const link = p?.link || '/register'
  const cta = p?.cta || ''

  useEffect(() => {
    if (!p?.enabled || !desktop) return
    try {
      if (/^\/(admin|register|portal)/.test(location.pathname)) return
      if (new URLSearchParams(location.search).has('edit') || sessionStorage.getItem('alsmow.texts.edit') === '1') return
    } catch { /* */ }
    if (seen(id, mode)) return
    let t = 0
    const html = document.documentElement
    const show = () => { window.clearTimeout(t); t = window.setTimeout(() => { if (!html.classList.contains('intro-on')) { markSeen(id); setOpen(true) } }, DELAY) }
    // wait for the opening scene to finish (it marks <html> while it plays)
    const mo = new MutationObserver(() => { if (!html.classList.contains('intro-on')) { mo.disconnect(); show() } })
    if (html.classList.contains('intro-on')) mo.observe(html, { attributes: true, attributeFilter: ['class'] })
    else show()
    return () => { mo.disconnect(); window.clearTimeout(t) }
  }, [p?.enabled, desktop, id, mode])

  // while open: no page scrolling, Esc closes
  useEffect(() => {
    if (!open) return
    lenis?.stop()
    const html = document.documentElement, prev = html.style.overflow
    html.style.overflow = 'hidden'
    const key = (e: KeyboardEvent) => { if (e.key === 'Escape') setOpen(false) }
    window.addEventListener('keydown', key)
    return () => { window.removeEventListener('keydown', key); html.style.overflow = prev; lenis?.start() }
  }, [open, lenis])

  const go = () => {
    setOpen(false)
    if (/^https?:\/\//i.test(link)) window.open(link, '_blank', 'noopener')
    else nav(link)
  }

  return (
    <AnimatePresence>
      {open && (
        <motion.div className="promo-wrap" role="dialog" aria-modal="true" aria-label={p?.alt || 'إعلان'}
          initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} transition={{ duration: 0.3 }}
          onClick={() => setOpen(false)}>
          <motion.div className="promo-card" onClick={e => e.stopPropagation()}
            initial={{ opacity: 0, y: 28, scale: 0.94 }} animate={{ opacity: 1, y: 0, scale: 1 }} exit={{ opacity: 0, y: 16, scale: 0.97 }}
            transition={{ type: 'spring', stiffness: 260, damping: 26 }}>
            <button type="button" className="promo-close" onClick={() => setOpen(false)} aria-label="إغلاق"><X className="h-5 w-5" /></button>
            <button type="button" className="promo-img" onClick={go} aria-label={p?.alt || cta || 'سجّل الآن'}>
              <picture>
                <source media="(max-width: 767px)" srcSet={mobile} />
                <img src={desktop} alt={p?.alt || ''} decoding="async" />
              </picture>
            </button>
            {cta && <button type="button" className="promo-cta btn-primary" onClick={go}>{cta}</button>}
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  )
}
