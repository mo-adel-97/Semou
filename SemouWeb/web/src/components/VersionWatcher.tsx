import { useEffect, useRef, useState } from 'react'
import { useLocation } from 'react-router-dom'
import { RefreshCw } from 'lucide-react'

/* The entry file of the build this tab is running (its name changes with every published version). */
const current = (document.querySelector('script[type="module"][src*="/assets/index-"]') as HTMLScriptElement | null)?.getAttribute('src') ?? null

async function latest(): Promise<string | null> {
  try {
    const r = await fetch('/index.html', { cache: 'no-store', headers: { 'Cache-Control': 'no-cache' } })
    if (!r.ok) return null
    return (await r.text()).match(/src="(\/assets\/index-[^"]+\.js)"/)?.[1] ?? null
  } catch { return null }
}

/**
 * Detects that a new version was published while this tab was open, so nobody keeps working on old screen code.
 * The new version loads by itself on the next move between screens (nothing typed is lost); a bar offers it right away.
 */
export function VersionWatcher() {
  const [ready, setReady] = useState(false)
  const loc = useLocation()
  const first = useRef(loc.key)

  useEffect(() => {
    if (!current || import.meta.env.DEV) return
    let stop = false
    const check = async () => { const v = await latest(); if (!stop && v && v !== current) setReady(true) }
    check()
    const t = window.setInterval(check, 60_000)
    const onFocus = () => { if (document.visibilityState === 'visible') check() }
    document.addEventListener('visibilitychange', onFocus); window.addEventListener('focus', onFocus)
    return () => { stop = true; window.clearInterval(t); document.removeEventListener('visibilitychange', onFocus); window.removeEventListener('focus', onFocus) }
  }, [])

  // Moving to another screen with a new version waiting: load it there instead of the old code.
  useEffect(() => {
    if (ready && loc.key !== first.current) window.location.reload()
    first.current = loc.key
  }, [loc.key, ready])

  if (!ready) return null
  return (
    <div className="ver-bar" role="status">
      <RefreshCw size={16} />
      <span>يتوفر إصدار أحدث من النظام، ويُحمَّل تلقائياً عند الانتقال إلى شاشة أخرى.</span>
      <button className="btn sm" onClick={() => window.location.reload()}>تحديث الآن</button>
    </div>
  )
}
