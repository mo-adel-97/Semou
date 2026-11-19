import { useMemo } from 'react'
import { useAuth } from './auth'
import type { MenuGroup } from './menu'
import { useUiConfig } from './uiConfig'

/**
 * The menu as the desktop HomePage would show it for the logged-in user, arranged by الإعدادات العامة.
 * A screen stays bound to the desktop permissions of its original group even when it is moved to another one.
 * includeHidden: also the groups / screens hidden in the settings (used to decide if a screen may be opened).
 */
export function useAllowedMenu(opts?: { includeHidden?: boolean }): MenuGroup[] {
  const { me, can, menu: menuRight } = useAuth()
  const { menu } = useUiConfig()
  const all = !!opts?.includeHidden
  return useMemo(() => menu
    .filter(g => all || !g.hidden)
    .map(g => ({ ...g, items: g.items.filter(i => (all || !i.hidden) && (i.originMenus ?? []).every(menuRight) && (i.perm === null || can(i.perm, 'view'))) }))
    .filter(g => g.items.length > 0),
  // eslint-disable-next-line react-hooks/exhaustive-deps
  [me, menu, all])
}
