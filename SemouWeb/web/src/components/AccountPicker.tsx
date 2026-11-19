import { useDeferredValue, useState } from 'react'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { Command } from 'cmdk'
import { Search } from 'lucide-react'
import { api, qs } from '../api'
import { Dialog, Spinner } from './ui'

export interface AccountLookup { Guid: string; Code: string; Name: string; AccountType: number | null; Level: number | null }
const TYPES = ['إيراد', 'مصروف', 'عام']

/** AccountList dialog: search accounts by code or name, keyboard friendly. */
export function AccountPicker({ onPick, onClose }: { onPick: (a: AccountLookup) => void; onClose: () => void }) {
  const [q, setQ] = useState('')
  const deferred = useDeferredValue(q)
  const { data, isPending, isFetching } = useQuery({
    queryKey: ['vouchers', 'accounts', deferred],
    queryFn: () => api<AccountLookup[]>(`/vouchers/lookups/accounts${qs({ q: deferred })}`),
    placeholderData: keepPreviousData,
  })
  return (
    <Dialog title="اختيار حساب" subtitle="ابحث بالكود أو الاسم، واستخدم الأسهم و Enter" onClose={onClose} size="lg">
      <Command shouldFilter={false} loop>
        <div className="input-wrap">
          <Search size={17} />
          <Command.Input className="input" autoFocus placeholder="مثال: 4010 أو مصروفات" value={q} onValueChange={setQ} />
          {isFetching && <span className="trail" style={{ insetInlineEnd: 12 }}><Spinner size={15} /></span>}
        </div>
        <Command.List className="lookup-list">
          {isPending && <Command.Loading><Spinner /></Command.Loading>}
          {!isPending && <Command.Empty>لا توجد نتائج</Command.Empty>}
          {data?.map(a => (
            <Command.Item key={a.Guid} value={a.Guid} onSelect={() => onPick(a)}>
              <span className="code-chip">{a.Code}</span>
              <span style={{ flex: 1 }}>{a.Name}</span>
              {a.Level === 0 && <span className="badge warn">رئيسي</span>}
              <span className="badge muted">{TYPES[a.AccountType ?? 2] ?? 'عام'}</span>
            </Command.Item>
          ))}
        </Command.List>
      </Command>
    </Dialog>
  )
}
