import { useDeferredValue, useState } from 'react'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { Command } from 'cmdk'
import { Search } from 'lucide-react'
import { api, qs } from '../api'
import { Dialog, Spinner } from './ui'

export interface Lookup { code: string; name: string; guid: string }

/* ---------------- CostCenterList dialog (accountType = 1) ---------------- */
export function CostCenterPicker({ onPick, onClose }: { onPick: (c: Lookup) => void; onClose: () => void }) {
  const [q, setQ] = useState('')
  const deferred = useDeferredValue(q)
  const { data, isPending, isFetching } = useQuery({
    queryKey: ['lookups', 'cost-centers', 1, deferred],
    queryFn: () => api<Lookup[]>(`/lookups/cost-centers${qs({ q: deferred, accountType: 1 })}`),
    placeholderData: keepPreviousData,
  })

  return (
    <Dialog title="اختيار مركز التكلفة" subtitle="استخدم الأسهم و Enter للاختيار السريع" onClose={onClose}>
      <Command shouldFilter={false} loop>
        <div className="input-wrap">
          <Search size={17} />
          <Command.Input className="input" autoFocus placeholder="ابحث بالاسم أو الكود" value={q} onValueChange={setQ} />
          {isFetching && <span className="trail" style={{ insetInlineEnd: 12 }}><Spinner size={15} /></span>}
        </div>
        <Command.List className="lookup-list">
          {isPending && <Command.Loading><Spinner /></Command.Loading>}
          {!isPending && <Command.Empty>لا توجد نتائج</Command.Empty>}
          {data?.map(c => (
            <Command.Item key={c.guid} value={c.guid} onSelect={() => onPick(c)}>
              <span className="code-chip">{c.code}</span>{c.name}
            </Command.Item>
          ))}
        </Command.List>
      </Command>
    </Dialog>
  )
}
