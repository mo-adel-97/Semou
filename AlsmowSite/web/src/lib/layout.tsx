import { Fragment, type ReactNode } from 'react'
import { ButterflySky } from '../components/Butterflies'
import { LITE } from '../components/ui'
import type { SectionPick, Settings } from './types'

/**
 * Page layouts from the dashboard (settings › layout): the order of a page's sections and which are hidden.
 * Sections the dashboard doesn't know yet (added to the site later) keep their default place at the end,
 * so a saved layout never makes a section disappear by accident.
 */
export type PageKey = 'home' | 'about' | 'program' | 'contact'

export function sectionOrder(settings: Settings | undefined, page: PageKey, defaults: string[]): string[] {
  const raw = settings?.layout?.[page]
  const saved = (Array.isArray(raw) ? raw : []) as SectionPick[]
  const known = saved.filter((s) => defaults.includes(s.key))
  const seen = new Set(known.map((s) => s.key))
  return [...known, ...defaults.filter((k) => !seen.has(k)).map((key) => ({ key, hidden: false }))]
    .filter((s) => !s.hidden)
    .map((s) => s.key)
}

/** Renders the sections of a page in the dashboard's order, each on a living background (see SectionLife). */
export function Sections({ settings, page, sections }: { settings: Settings | undefined; page: PageKey; sections: Record<string, ReactNode> }) {
  const bf = !settings?.butterflies?.hide
  return (
    <>
      {sectionOrder(settings, page, Object.keys(sections)).map((k, i) => (
        <Fragment key={k}>{sections[k] ? <SectionLife index={i} butterflies={bf && i % 2 === 1}>{sections[k]}</SectionLife> : null}</Fragment>
      ))}
    </>
  )
}

/**
 * A section that is never still: soft theme-coloured lights drifting slowly behind it, motes of light rising,
 * and (every other section) a couple of butterflies crossing. Transform/opacity only, on their own layers.
 */
export function SectionLife({ index = 0, butterflies, children }: { index?: number; butterflies?: boolean; children: ReactNode }) {
  return (
    <div className={`sec-live ${index % 2 ? 'alt' : ''}`}>
      <span className="sec-orb a" aria-hidden />
      <span className="sec-orb b" aria-hidden />
      <span className="sec-motes" aria-hidden><i /><i /><i /><i /><i /></span>
      {butterflies && <ButterflySky count={LITE ? 1 : 2} className="sec-bf" />}
      {children}
    </div>
  )
}
