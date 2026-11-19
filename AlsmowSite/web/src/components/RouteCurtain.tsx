import { useEffect, useRef, useState } from 'react'
import { useLocation } from 'react-router-dom'
import { useSite } from '../lib/queries'
import BrandLogo from './BrandLogo'

/**
 * Page change = a curtain in the site's colour sweeps across with the logo, and the new page is revealed behind it.
 * Pure CSS animation (compositor); skipped on the first load, on same-page hash changes and for "reduce motion".
 */
export default function RouteCurtain() {
  const { pathname } = useLocation()
  const { data } = useSite()
  const first = useRef(true)
  const [run, setRun] = useState(0)
  useEffect(() => {
    if (first.current) { first.current = false; return }
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return
    setRun((n) => n + 1)
  }, [pathname])
  if (!run) return null
  return (
    <div key={run} className="route-curtain" aria-hidden data-no-texts onAnimationEnd={(e) => { if (e.target === e.currentTarget) setRun(0) }}>
      <span className="rc-panel rc-a" />
      <span className="rc-panel rc-b" />
      <span className="rc-logo"><BrandLogo src={data?.settings.general.logo} animate={false} className="h-full w-full" /></span>
    </div>
  )
}
