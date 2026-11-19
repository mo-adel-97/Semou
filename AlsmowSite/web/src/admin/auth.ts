import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ApiError, api } from '../lib/api'
import type { ResourceSpec, SettingsGroupSpec, User } from '../lib/types'

/** Lets the public site know this browser has a dashboard session, so it shows «وضع التعديل» (see site/SiteTexts). */
const EDITOR_FLAG = 'alsmow.editor'
const flag = (on: boolean) => { try { if (on) localStorage.setItem(EDITOR_FLAG, '1'); else localStorage.removeItem(EDITOR_FLAG) } catch { /* blocked */ } }

export function useMe() {
  return useQuery({
    queryKey: ['me'],
    queryFn: async () => {
      try {
        const u = (await api.get<{ user: User }>('/auth/me')).user
        flag(true)
        return u
      } catch (e) {
        if (e instanceof ApiError && e.status === 401) { flag(false); return null }
        throw e
      }
    },
    staleTime: 5 * 60_000,
    retry: false,
  })
}

export function useLogin() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (v: { email: string; password: string }) => api.post<{ user: User }>('/auth/login', v),
    onSuccess: (r) => { flag(true); qc.setQueryData(['me'], r.user) },
  })
}

export function useLogout() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: () => api.post('/auth/logout'),
    onSettled: () => {
      flag(false)
      qc.setQueryData(['me'], null)
      qc.removeQueries({ predicate: (q) => q.queryKey[0] === 'admin' })
    },
  })
}

export function useSchema() {
  return useQuery({
    queryKey: ['admin', 'schema'],
    queryFn: () => api.get<{ resources: ResourceSpec[]; settings: SettingsGroupSpec[] }>('/admin/schema'),
    staleTime: Infinity,
  })
}
