import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { LayoutGrid } from 'lucide-react'
import { api } from './api'
import { useAuth } from './auth'
import { MENU, type MenuGroup, type MenuItem } from './menu'
import { iconByName, nameOfIcon } from './lib/icons'
import { applyTextRules, type TextRule } from './lib/textOverrides'

/* =============================== Shapes saved in dbo.WebUiConfig =============================== */

export interface Brand { name?: string; tagline?: string; home?: string; sections?: string }
export interface GroupCfg { key: string; title?: string; icon?: string; hidden?: boolean; custom?: boolean; items: string[] }
export interface ItemCfg { title?: string; icon?: string; hidden?: boolean }
export interface LinkCfg { title: string; url: string; icon?: string; newTab?: boolean }
export interface MenuConfig { v: 1; brand?: Brand; groups: GroupCfg[]; items: Record<string, ItemCfg>; links: Record<string, LinkCfg> }
export interface TextsConfig { v: 1; rules: TextRule[] }
interface Saved { menu: MenuConfig | null; texts: TextsConfig | null; versions: Record<string, number> }

export const DEFAULT_BRAND: Required<Brand> = { name: 'معهد السمو', tagline: 'نظام الإدارة المتكامل', home: 'الرئيسية', sections: 'الأقسام' }

/** The structure exactly as the program ships it (the starting point of the editor and of "reset"). */
export function defaultMenuConfig(): MenuConfig {
  return { v: 1, brand: {}, groups: MENU.map(g => ({ key: g.key, items: g.items.map(i => i.key) })), items: {}, links: {} }
}

/** Fills a saved structure with groups / screens added to the program after it was saved (at their default place). */
export function completeMenuConfig(cfg: MenuConfig | null): MenuConfig {
  if (!cfg) return defaultMenuConfig()
  const c: MenuConfig = structuredClone({ ...defaultMenuConfig(), ...cfg, items: cfg.items ?? {}, links: cfg.links ?? {}, brand: cfg.brand ?? {} })
  const placed = new Set(c.groups.flatMap(g => g.items))
  MENU.forEach((bg, idx) => {
    const missing = bg.items.map(i => i.key).filter(k => !placed.has(k))
    let g = c.groups.find(x => x.key === bg.key)
    if (!g && missing.length) { g = { key: bg.key, items: [] }; c.groups.splice(Math.min(idx, c.groups.length), 0, g) }
    if (g) g.items.push(...missing)
  })
  // Drop references to screens that no longer exist.
  const known = new Set(MENU.flatMap(g => g.items.map(i => i.key)))
  c.groups = c.groups.filter(g => g.custom || MENU.some(b => b.key === g.key))
  c.groups.forEach(g => { g.items = [...new Set(g.items)].filter(k => known.has(k) || !!c.links[k]) })
  return c
}

/** The sidebar after applying the settings (before the permission filter). */
export function buildMenu(saved: MenuConfig | null): MenuGroup[] {
  const cfg = completeMenuConfig(saved)
  const byItem = new Map(MENU.flatMap(g => g.items.map(i => [i.key, { i, g }] as const)))
  return cfg.groups.map(gc => {
    const bg = MENU.find(b => b.key === gc.key)
    const items: MenuItem[] = gc.items.flatMap((k): MenuItem[] => {
      const link = cfg.links[k]
      if (link) return [{ key: k, title: link.title, perm: null, form: '', href: link.url, newTab: link.newTab ?? true, icon: iconByName(link.icon), web: true, originMenus: [] }]
      const hit = byItem.get(k)
      if (!hit) return []
      const ic = cfg.items[k]
      return [{ ...hit.i, title: ic?.title?.trim() || hit.i.title, icon: iconByName(ic?.icon), hidden: !!ic?.hidden, originMenus: hit.g.menus }]
    })
    return { key: gc.key, title: gc.title?.trim() || bg?.title || 'قسم جديد', icon: iconByName(gc.icon) ?? bg?.icon ?? LayoutGrid, menus: [], items, hidden: !!gc.hidden, custom: !!gc.custom }
  })
}

export const defaultIconName = (groupKey: string) => nameOfIcon(MENU.find(g => g.key === groupKey)?.icon)

/* =============================== Context =============================== */

interface Ctx {
  saved: Saved
  loaded: boolean
  /** Unsaved edits of the settings screen: applied live to the whole interface as a preview. */
  draft: { menu?: MenuConfig; texts?: TextsConfig }
  setDraft: (d: { menu?: MenuConfig; texts?: TextsConfig } | ((d: { menu?: MenuConfig; texts?: TextsConfig }) => { menu?: MenuConfig; texts?: TextsConfig })) => void
  clearDraft: () => void
  menu: MenuGroup[]
  brand: Required<Brand>
  texts: TextRule[]
  refetch: () => Promise<unknown>
}

const CACHE = 'semou.ui-config'
const DRAFT = 'semou.ui-config.draft'
const empty: Saved = { menu: null, texts: null, versions: {} }
const readCache = (): Saved => { try { return JSON.parse(localStorage.getItem(CACHE) ?? 'null') ?? empty } catch { return empty } }

const UiCtx = createContext<Ctx | null>(null)

export function UiConfigProvider({ children }: { children: ReactNode }) {
  const { me } = useAuth()
  const [cached] = useState(readCache)
  const q = useQuery({
    queryKey: ['ui-config'], enabled: !!me, staleTime: 60_000, refetchInterval: 5 * 60_000, meta: { silent: true },
    queryFn: () => api<Saved>('/ui-config'),
  })
  const saved = q.data ?? cached
  useEffect(() => { if (q.data) try { localStorage.setItem(CACHE, JSON.stringify(q.data)) } catch { /* storage full or blocked */ } }, [q.data])

  // The unsaved edits survive a reload (a new version loading, F5) until they are saved or discarded.
  const [draft, setDraftState] = useState<Ctx['draft']>(() => { try { return JSON.parse(sessionStorage.getItem(DRAFT) ?? '{}') } catch { return {} } })
  useEffect(() => {
    try { if (draft.menu || draft.texts) sessionStorage.setItem(DRAFT, JSON.stringify(draft)); else sessionStorage.removeItem(DRAFT) } catch { /* storage blocked */ }
  }, [draft])
  const setDraft = useCallback<Ctx['setDraft']>(d => setDraftState(d), [])
  const clearDraft = useCallback(() => setDraftState({}), [])

  const menuCfg = draft.menu ?? saved.menu
  const textsCfg = draft.texts ?? saved.texts
  const menu = useMemo(() => buildMenu(menuCfg), [menuCfg])
  const brand = useMemo(() => {
    const b = menuCfg?.brand ?? {}
    return { name: b.name?.trim() || DEFAULT_BRAND.name, tagline: b.tagline?.trim() || DEFAULT_BRAND.tagline, home: b.home?.trim() || DEFAULT_BRAND.home, sections: b.sections?.trim() || DEFAULT_BRAND.sections }
  }, [menuCfg])
  const texts = useMemo(() => textsCfg?.rules ?? [], [textsCfg])
  useEffect(() => { applyTextRules(texts) }, [texts])

  const value = useMemo<Ctx>(() => ({ saved, loaded: !!q.data, draft, setDraft, clearDraft, menu, brand, texts, refetch: q.refetch }),
    [saved, q.data, draft, setDraft, clearDraft, menu, brand, texts, q.refetch])
  return <UiCtx.Provider value={value}>{children}</UiCtx.Provider>
}

export function useUiConfig(): Ctx {
  const c = useContext(UiCtx)
  if (!c) throw new Error('useUiConfig outside UiConfigProvider')
  return c
}
