import { useQuery } from '@tanstack/react-query'
import { api } from '../api'

export interface BranchLookup { code: string; name: string; guid: string; isUse: boolean }

/** Branch list shared by every screen (cached for 5 minutes). */
export function useBranches() {
  return useQuery({
    queryKey: ['lookups', 'branches'],
    queryFn: () => api<BranchLookup[]>('/lookups/branches'),
    staleTime: 5 * 60_000,
  })
}

/** Lookup sources that a screen field can reference by name (FieldDef.Lookup on the server). */
export function useLookup(name: string | null | undefined) {
  const branches = useBranches()
  if (name === 'branches') return { items: (branches.data ?? []).map(b => ({ value: b.guid, label: b.name, code: b.code, active: b.isUse })), loading: branches.isPending }
  return { items: [] as { value: string; label: string; code: string; active: boolean }[], loading: false }
}
