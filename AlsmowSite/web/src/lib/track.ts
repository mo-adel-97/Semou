/**
 * Self-hosted analytics (api/lib/analytics.php). No cookies, no third parties.
 * Sends: a page view on every route, the time actually spent on the page (only while visible),
 * and a few events (chat, register, WhatsApp / phone clicks…). Staff (dashboard users) are not counted.
 */

type Hit = Record<string, string | number | boolean | undefined>

const SID_KEY = 'alsmow.sid'
const UTM_KEY = 'alsmow.utm'

function sid(): string {
  try {
    let s = sessionStorage.getItem(SID_KEY)
    if (!s) {
      s = Math.random().toString(36).slice(2, 10) + Date.now().toString(36).slice(-6)
      sessionStorage.setItem(SID_KEY, s)
    }
    return s
  } catch {
    return 'nostorage' + Math.random().toString(36).slice(2, 8)
  }
}

const staff = () => { try { return localStorage.getItem('alsmow.editor') === '1' } catch { return false } }

function send(hit: Hit) {
  if (staff() || navigator.webdriver) return
  try {
    void fetch('/api/public/track', {
      method: 'POST',
      keepalive: true, // still delivered when the tab closes
      headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'fetch' },
      body: JSON.stringify({ sid: sid(), lang: navigator.language, w: window.innerWidth, ...hit }),
    }).catch(() => {})
  } catch { /* never break the site for analytics */ }
}

/* ---------- page views + time on page ---------- */
let current: { path: string; title: string; active: number; since: number | null } | null = null
let views = 0

function flush() {
  if (!current) return
  const active = current.active + (current.since !== null ? Date.now() - current.since : 0)
  const dur = Math.round(active / 1000)
  if (dur >= 1) send({ kind: 'leave', path: current.path, title: current.title, dur })
  current.active = 0
  current.since = document.visibilityState === 'visible' ? Date.now() : null
}

export function trackView(path: string) {
  if (current?.path === path) return
  flush()
  const entry = views === 0
  views++
  let isNew = false
  try { isNew = !localStorage.getItem('alsmow.seen'); localStorage.setItem('alsmow.seen', '1') } catch { /* */ }
  // campaign tags of the landing URL, kept for the session
  let utm: Hit = {}
  try {
    const q = new URLSearchParams(location.search)
    if (q.get('utm_source')) {
      utm = { utm_source: q.get('utm_source') ?? '', utm_medium: q.get('utm_medium') ?? '', utm_campaign: q.get('utm_campaign') ?? '' }
      sessionStorage.setItem(UTM_KEY, JSON.stringify(utm))
    } else if (entry) utm = JSON.parse(sessionStorage.getItem(UTM_KEY) || '{}') as Hit
  } catch { /* */ }
  // the title is set by the page a moment later
  setTimeout(() => {
    const title = document.title.split('|')[0].trim()
    current = { path, title, active: 0, since: document.visibilityState === 'visible' ? Date.now() : null }
    send({ kind: 'view', path, title, entry, new: isNew && entry, ref: entry ? document.referrer : undefined, ...(entry ? utm : {}) })
    if (path.startsWith('/programs/')) track('program_view')
  }, 350)
}

export function track(name: string) {
  send({ kind: 'event', name, path: location.pathname })
}

let started = false
/** Wires visibility (active time only) and the click events. Call once. */
export function startTracking() {
  if (started || typeof window === 'undefined') return
  started = true
  document.addEventListener('visibilitychange', () => {
    if (!current) return
    if (document.visibilityState === 'hidden') {
      if (current.since !== null) { current.active += Date.now() - current.since; current.since = null }
      flush()
    } else current.since = Date.now()
  })
  window.addEventListener('pagehide', flush)
  // outbound contact clicks
  document.addEventListener('click', (e) => {
    const a = (e.target as Element | null)?.closest?.('a')
    if (!a) return
    const href = a.getAttribute('href') ?? ''
    if (href.startsWith('tel:')) track('phone_click')
    else if (href.startsWith('mailto:')) track('email_click')
    else if (/wa\.me|whatsapp/.test(href)) track('whatsapp_click')
    else if (href === '/register' || href.endsWith('/register')) track('register_click')
    else if (href === '/portal') track('portal_click')
  }, { capture: true, passive: true })
}
