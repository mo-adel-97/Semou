import { MutationCache, QueryCache, QueryClient } from '@tanstack/react-query'
import { ApiError } from '../api'
import { alerts, notify } from './alerts'

declare module '@tanstack/react-query' {
  interface Register {
    defaultError: ApiError
    /** meta.silent = handle the error yourself (no automatic alert). */
    queryMeta: { silent?: boolean }
    mutationMeta: { silent?: boolean }
  }
}

/**
 * Server state lives in TanStack Query: caching, background refresh, retries.
 * Errors are shown automatically with SweetAlert: toasts for loading data, dialogs for saving.
 */
export const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (err, query) => {
      if (query.meta?.silent || err.status === 401) return
      notify.error(err.message)
    },
  }),
  mutationCache: new MutationCache({
    onError: (err, _v, _c, mutation) => {
      if (mutation.meta?.silent || err.status === 401) return
      alerts.error(err.message)
    },
  }),
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      refetchOnWindowFocus: true,
      retry: (count, err) => err.status >= 500 && count < 2,
    },
    mutations: { retry: false },
  },
})
