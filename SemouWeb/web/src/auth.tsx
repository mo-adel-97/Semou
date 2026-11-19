import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api, setUnauthorizedHandler, tokenStore } from './api'
import { notify } from './lib/alerts'

export type Perm = 'add' | 'edit' | 'delete' | 'print' | 'find' | 'view'
export type FormPerms = Record<Perm, boolean>

export interface Me {
  userGuid: string
  sellerGuid: string | null
  fullName: string
  menus: Record<string, boolean>
  perms: Record<string, FormPerms>
}

interface AuthCtx {
  me: Me | null
  loading: boolean
  login: (userName: string, password: string) => Promise<void>
  logout: () => void
  /** Same as AllFiles.FORSAVE / FORUPDATE / FORFIND / FORVIEW: no row means no permission. */
  can: (form: string, perm: Perm) => boolean
  /** Same as AllFiles.MENU_VIEW__: hidden only when the user has an explicit View_ = 0. */
  menu: (key: string) => boolean
}

const Ctx = createContext<AuthCtx | null>(null)
export const meKey = ['auth', 'me'] as const

export function AuthProvider({ children }: { children: ReactNode }) {
  const qc = useQueryClient()
  // Session is React state (not just localStorage) so login/logout re-render the app immediately.
  const [hasToken, setHasToken] = useState(() => !!tokenStore.get())

  const meQuery = useQuery({
    queryKey: meKey,
    queryFn: () => api<Me>('/auth/me'),
    enabled: hasToken,
    staleTime: 60_000,
    refetchInterval: 5 * 60_000, // permission changes reach the open session within minutes
    meta: { silent: true },
  })

  const logout = useCallback(() => {
    tokenStore.clear()
    setHasToken(false)
    qc.cancelQueries()
    qc.clear()
  }, [qc])

  // Logging out in one browser tab logs out the other tabs too.
  useEffect(() => {
    const onStorage = (e: StorageEvent) => { if (e.key === null || e.key === 'semou.token') setHasToken(!!tokenStore.get()) }
    window.addEventListener('storage', onStorage)
    return () => window.removeEventListener('storage', onStorage)
  }, [])

  useEffect(() => {
    setUnauthorizedHandler(() => {
      logout()
      notify.info('انتهت الجلسة، يرجى تسجيل الدخول مرة أخرى')
    })
  }, [logout])

  const loginMutation = useMutation({
    mutationFn: (v: { userName: string; password: string }) =>
      api<{ token: string }>('/auth/login', { method: 'POST', json: v }),
    meta: { silent: true }, // the login form shows its own error
    onSuccess: async r => {
      tokenStore.set(r.token)
      await qc.fetchQuery({ queryKey: meKey, queryFn: () => api<Me>('/auth/me') })
      setHasToken(true)
    },
  })

  const me = (hasToken ? meQuery.data : null) ?? null

  const value = useMemo<AuthCtx>(() => {
    const perms = Object.fromEntries(Object.entries(me?.perms ?? {}).map(([k, v]) => [k.toLowerCase(), v]))
    const menus = Object.fromEntries(Object.entries(me?.menus ?? {}).map(([k, v]) => [k.toLowerCase(), v]))
    return {
      me,
      loading: hasToken && meQuery.isPending,
      login: async (userName, password) => { await loginMutation.mutateAsync({ userName, password }) },
      logout,
      can: (form, perm) => !!perms[form.toLowerCase()]?.[perm],
      menu: key => menus[key.toLowerCase()] !== false,
    }
  }, [me, hasToken, meQuery.isPending, loginMutation, logout])

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>
}

export function useAuth() {
  const c = useContext(Ctx)
  if (!c) throw new Error('useAuth outside AuthProvider')
  return c
}
