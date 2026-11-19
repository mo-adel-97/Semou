import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Controller, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AnimatePresence, motion } from 'motion/react'
import { ChevronDown, ChevronsDownUp, ChevronsUpDown, FolderTree, FolderOpen, Folder, FileText, Plus, Save, Search, X } from 'lucide-react'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { PageHead } from '../components/Shell'
import { Field, Spinner, Stat, Switch } from '../components/ui'

/*
 * دليل الحسابات / دليل مراكز التكلفة (AccountHome + EncodingAccount, CostCenterHome + EncodingCostCenter).
 * One interactive tree with an inline card: view, edit, or add a child under any main node.
 */
export type ChartKind = 'accounts' | 'cost-centers'

interface Node { code: string; name: string; guid: string; parentGuid: string | null; kind: number | null; isUse: boolean; type_: number | null; children: number }
interface Details {
  node: { Code: string; Guid: string; Name: string; Kind: number; IsUse: boolean; ParentGuid: string | null; ParentName: string | null; ParentCode: string | null; Type_?: number; Maden?: number; Daen?: number }
  openBalances: { branchGuid: string; maden: number; daen: number; notes: string | null }[]
}

const CFG = {
  accounts: { title: 'دليل الحسابات', singular: 'حساب', form: 'addaccount', tree: 'accountchart', kinds: ['رئيسي', 'فرعي - معاملات'], subtitle: 'شجرة الحسابات والأرصدة الافتتاحية لكل فرع' },
  'cost-centers': { title: 'دليل مراكز التكلفة', singular: 'مركز تكلفة', form: 'addcostcenter', tree: 'centertree', kinds: ['رئيسي', 'فرعي'], subtitle: 'شجرة مراكز التكلفة' },
} as const
const TYPES = ['إيراد', 'مصروف', 'عام']
const kindLabel = (k: ChartKind, n: number | null) => (n === 2 ? 'جذر' : n === 0 ? 'رئيسي' : CFG[k].kinds[1] ? 'فرعي' : '')

type Sel = { mode: 'view'; guid: string } | { mode: 'new'; parent: Node } | null

export function ChartTree({ chart, group }: { chart: ChartKind; group: string }) {
  const c = CFG[chart]
  const { can } = useAuth()
  const tree = useQuery({ queryKey: ['chart', chart, 'tree'], queryFn: () => api<Node[]>(`/accounting/${chart}/tree`) })
  const nodes = useMemo(() => tree.data ?? [], [tree.data])
  const [q, setQ] = useState('')
  const [open, setOpen] = useState<Set<string>>(new Set())
  const [sel, setSel] = useState<Sel>(null)

  const byParent = useMemo(() => {
    const m = new Map<string | null, Node[]>()
    for (const n of nodes) { const k = n.parentGuid ?? null; if (!m.has(k)) m.set(k, []); m.get(k)!.push(n) }
    return m
  }, [nodes])
  const byGuid = useMemo(() => new Map(nodes.map(n => [n.guid, n])), [nodes])

  // Roots open by default.
  useEffect(() => { if (nodes.length && open.size === 0) setOpen(new Set((byParent.get(null) ?? []).map(n => n.guid))) }, [nodes, byParent, open.size])

  // Search: matches + their ancestors.
  const visible = useMemo(() => {
    const needle = q.trim()
    if (!needle) return null
    const keep = new Set<string>()
    for (const n of nodes) if (n.name?.includes(needle) || n.code.startsWith(needle)) {
      let cur: Node | undefined = n
      while (cur) { keep.add(cur.guid); cur = cur.parentGuid ? byGuid.get(cur.parentGuid) : undefined }
    }
    return keep
  }, [q, nodes, byGuid])

  const toggle = (g: string) => setOpen(s => { const n = new Set(s); if (n.has(g)) n.delete(g); else n.add(g); return n })
  const expandAll = () => setOpen(new Set(nodes.filter(n => n.children > 0).map(n => n.guid)))
  const collapseAll = () => setOpen(new Set())

  const mains = nodes.filter(n => n.kind !== 1).length
  const subs = nodes.length - mains
  const selectedGuid = sel?.mode === 'view' ? sel.guid : sel?.mode === 'new' ? sel.parent.guid : null

  const renderLevel = (parent: string | null, depth: number): React.ReactNode =>
    (byParent.get(parent) ?? []).filter(n => !visible || visible.has(n.guid)).map(n => {
      const hasKids = n.children > 0
      const isOpen = visible ? true : open.has(n.guid)
      const Icon = !hasKids && n.kind === 1 ? FileText : isOpen ? FolderOpen : Folder
      return (
        <div key={n.guid}>
          <div className={`tree-row ${selectedGuid === n.guid ? 'sel' : ''} ${n.isUse ? '' : 'off'}`} style={{ paddingInlineStart: 8 + depth * 20 }}
            onClick={() => setSel({ mode: 'view', guid: n.guid })}>
            <button className={`tw ${hasKids ? '' : 'leaf'} ${isOpen ? 'open' : ''}`} onClick={e => { e.stopPropagation(); if (hasKids) toggle(n.guid) }} tabIndex={-1} aria-label="فتح">
              {hasKids && <ChevronDown size={14} />}
            </button>
            <Icon size={16} className={`ti k${n.kind}`} />
            <span className="tc">{n.code}</span>
            <span className="tn">{n.name}</span>
            {n.kind === 1 ? null : <span className="tk">{hasKids ? n.children : ''}</span>}
            {n.kind !== 1 && can(c.form, 'add') && (
              <button className="t-add" title={`إضافة ${c.singular} تحت ${n.name}`} onClick={e => { e.stopPropagation(); setSel({ mode: 'new', parent: n }); setOpen(s => new Set(s).add(n.guid)) }}><Plus size={14} /></button>
            )}
          </div>
          <AnimatePresence initial={false}>
            {hasKids && isOpen && (
              <motion.div initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} transition={{ duration: 0.16 }} style={{ overflow: 'hidden' }}>
                {renderLevel(n.guid, depth + 1)}
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      )
    })

  return (
    <>
      <PageHead title={c.title} subtitle={c.subtitle} group={group} />
      <div className="stats">
        <Stat icon={<FolderTree size={22} />} value={tree.isPending ? '…' : nodes.length} label={`إجمالي ${c.title.replace('دليل ', '')}`} />
        <Stat icon={<Folder size={22} />} value={tree.isPending ? '…' : mains} label="رئيسي" delay={0.05} />
        <Stat icon={<FileText size={22} />} value={tree.isPending ? '…' : subs} label="فرعي" delay={0.1} />
      </div>
      <div className="split">
        <div className="card tree-card">
          <div className="dt-toolbar">
            <div className="input-wrap" style={{ flex: 1 }}><Search size={16} /><input className="input" placeholder="ابحث بالاسم أو الكود..." value={q} onChange={e => setQ(e.target.value)} /></div>
            <button className="btn ghost sm icon" title="فتح الكل" onClick={expandAll}><ChevronsUpDown size={16} /></button>
            <button className="btn ghost sm icon" title="طي الكل" onClick={collapseAll}><ChevronsDownUp size={16} /></button>
          </div>
          <div className="tree">
            {tree.isPending ? Array.from({ length: 10 }).map((_, i) => <div key={i} className="sk" style={{ height: 20, margin: '10px 14px', width: `${40 + (i * 13) % 50}%` }} />)
              : nodes.length === 0 ? <div className="empty"><b>لا توجد بيانات</b></div>
              : renderLevel(null, 0)}
            {visible && visible.size === 0 && <div className="empty"><b>لا توجد نتائج مطابقة</b></div>}
          </div>
        </div>
        <div className="card side-panel">
          <AnimatePresence mode="wait">
            {!sel ? (
              <motion.div key="none" className="soon" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}>
                <div className="ic"><FolderTree size={36} /></div>
                <h2>اختر {c.singular} من الشجرة</h2>
                <p>اضغط على أي {c.singular} لعرض بياناته وتعديلها، أو اضغط <Plus size={13} style={{ verticalAlign: 'middle' }} /> بجانب أي {c.singular} رئيسي لإضافة {c.singular} تحته.</p>
              </motion.div>
            ) : (
              <motion.div key={sel.mode === 'view' ? sel.guid : `new-${sel.parent.guid}`} initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0 }} transition={{ duration: 0.18 }}>
                <NodeCard chart={chart} sel={sel} onClose={() => setSel(null)} onCreated={g => setSel({ mode: 'view', guid: g })} />
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </div>
    </>
  )
}

/* ============================== Card ============================== */
const schema = z.object({
  name: z.string().trim().min(1, 'يرجى إدخال الاسم').max(150, 'الاسم طويل جداً'),
  kind: z.number().int(),
  type_: z.number().int(),
  isUse: z.boolean(),
  maden: z.string(),
  daen: z.string(),
})
type Form = z.infer<typeof schema>
type Bal = Record<string, { maden: string; daen: string; notes: string }>

function NodeCard({ chart, sel, onClose, onCreated }: { chart: ChartKind; sel: NonNullable<Sel>; onClose: () => void; onCreated: (guid: string) => void }) {
  const c = CFG[chart]
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const isNew = sel.mode === 'new'
  const canSave = isNew ? can(c.form, 'add') : can(c.form, 'edit')
  const ro = !canSave

  const details = useQuery({
    queryKey: ['chart', chart, 'one', sel.mode === 'view' ? sel.guid : null],
    queryFn: () => api<Details>(`/accounting/${chart}/${(sel as { guid: string }).guid}`),
    enabled: sel.mode === 'view', staleTime: 0,
  })
  const { register, handleSubmit, control, reset, formState: { errors, isDirty } } = useForm<Form>({
    resolver: zodResolver(schema), defaultValues: { name: '', kind: -1, type_: 2, isUse: true, maden: '0', daen: '0' },
  })
  const [bal, setBal] = useState<Bal>({})
  const [balDirty, setBalDirty] = useState(false)

  useEffect(() => {
    const n = details.data?.node
    if (!n) return
    reset({ name: n.Name ?? '', kind: n.Kind ?? 1, type_: n.Type_ ?? 2, isUse: !!n.IsUse, maden: String(n.Maden ?? 0), daen: String(n.Daen ?? 0) })
    setBal(Object.fromEntries((details.data?.openBalances ?? []).map(b => [b.branchGuid, { maden: String(b.maden ?? 0), daen: String(b.daen ?? 0), notes: b.notes ?? '' }])))
    setBalDirty(false)
  }, [details.data, reset])

  const save = useMutation({
    mutationFn: async (v: Form) => {
      if (isNew && v.kind < 0) throw Object.assign(new Error(chart === 'accounts' ? 'يرجى اختيار نوع الحساب' : 'يرجى اختيار نوع مركز التكلفة'), { status: 400 })
      const body = {
        name: v.name, kind: v.kind, type_: v.type_, isUse: v.isUse, maden: Number(v.maden) || 0, daen: Number(v.daen) || 0,
        parentGuid: isNew ? sel.parent.guid : undefined,
        openBalances: Object.entries(bal).map(([branchGuid, b]) => ({ branchGuid, maden: Number(b.maden) || 0, daen: Number(b.daen) || 0, notes: b.notes })),
      }
      if (isNew) return api<{ message: string; code: string; guid: string }>(`/accounting/${chart}`, { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/accounting/${chart}/${(sel as { guid: string }).guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async (r, v) => {
      if (!r) return
      reset(v); setBalDirty(false)
      await qc.invalidateQueries({ queryKey: ['chart', chart] })
      qc.invalidateQueries({ queryKey: ['lookups'] })
      await alerts.success(r.message, isNew ? `الكود: ${r.code}` : undefined)
      if (isNew) onCreated(r.guid)
    },
  })
  const submit = handleSubmit(v => save.mutate(v), errs => alerts.error(String(Object.values(errs)[0]?.message ?? 'يرجى مراجعة البيانات')))

  const close = async () => {
    if ((isDirty || balDirty) && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  if (!isNew && details.isPending) return <div className="grid" style={{ padding: 24 }}>{Array.from({ length: 6 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div>
  const n = details.data?.node
  const parentLabel = isNew ? `${sel.parent.code} - ${sel.parent.name}` : n?.ParentName ? `${n.ParentCode} - ${n.ParentName}` : '—'

  return (
    <form onSubmit={e => { e.preventDefault(); submit() }} noValidate>
      <div className="panel-head">
        <div>
          <small className="muted">{isNew ? `${c.singular} جديد` : `كود ${n?.Code}`}</small>
          <h3>{isNew ? `إضافة ${c.singular}` : n?.Name}</h3>
        </div>
        <button type="button" className="btn ghost sm icon" onClick={close} aria-label="إغلاق"><X size={18} /></button>
      </div>
      <div className="grid cols-2" style={{ padding: '18px 22px' }}>
        <Field label={chart === 'accounts' ? 'الحساب الرئيسي' : 'المركز الرئيسي'} className="span-2"><input className="input" readOnly value={parentLabel} /></Field>
        <Field label="الاسم" required error={errors.name?.message} className="span-2"><input className={`input ${errors.name ? 'invalid' : ''}`} autoFocus readOnly={ro} {...register('name')} /></Field>
        <Field label={chart === 'accounts' ? 'مستوى الحساب' : 'نوع المركز'} required={isNew} hint={isNew ? undefined : 'لا يمكن تغييره بعد الحفظ'}>
          <Controller control={control} name="kind" render={({ field }) => (
            <select className="input" disabled={!isNew || ro} value={field.value} onChange={e => field.onChange(Number(e.target.value))}>
              <option value={-1} disabled>اختر...</option>
              {field.value === 2 && <option value={2}>جذر</option>}
              {c.kinds.map((k, i) => <option key={i} value={i}>{k}</option>)}
            </select>
          )} />
        </Field>
        {chart === 'accounts' && (
          <Field label="نوع الحساب">
            <Controller control={control} name="type_" render={({ field }) => (
              <select className="input" disabled={ro} value={field.value} onChange={e => field.onChange(Number(e.target.value))}>
                {TYPES.map((t, i) => <option key={i} value={i}>{t}</option>)}
              </select>
            )} />
          </Field>
        )}
        {!isNew && <div className="span-2"><Controller control={control} name="isUse" render={({ field }) => <Switch label="فعال" checked={field.value} disabled={ro} onChange={field.onChange} />} /></div>}

        {chart === 'accounts' && (
          <>
            <div className="section-label span-all">الأرصدة الافتتاحية</div>
            {!isNew && <>
              <Field label="مدين"><div className="input-suffix"><input className="input" dir="ltr" inputMode="decimal" readOnly={ro} {...register('maden')} /><span>ر.س</span></div></Field>
              <Field label="دائن"><div className="input-suffix"><input className="input" dir="ltr" inputMode="decimal" readOnly={ro} {...register('daen')} /><span>ر.س</span></div></Field>
            </>}
            <div className="span-all mini-table bal">
              <div className="mt-head"><span>الفرع</span><span>مدين</span><span>دائن</span></div>
              {(branches.data ?? []).filter(b => b.isUse || bal[b.guid]).map(b => {
                const r = bal[b.guid] ?? { maden: '', daen: '', notes: '' }
                const set = (k: 'maden' | 'daen', v: string) => { setBalDirty(true); setBal(x => ({ ...x, [b.guid]: { ...r, [k]: v.replace(/[^\d.]/g, '') } })) }
                return (
                  <div key={b.guid} className="mt-row">
                    <span>{b.name}</span>
                    <input className="input" dir="ltr" inputMode="decimal" placeholder="0" readOnly={ro} value={r.maden} onChange={e => set('maden', e.target.value)} />
                    <input className="input" dir="ltr" inputMode="decimal" placeholder="0" readOnly={ro} value={r.daen} onChange={e => set('daen', e.target.value)} />
                  </div>
                )
              })}
            </div>
          </>
        )}
      </div>
      <div className="panel-foot">
        {canSave ? <button className="btn" disabled={save.isPending}>{save.isPending ? <Spinner /> : <><Save size={16} /> حفظ</>} <kbd>F2</kbd></button>
          : <span className="muted">عرض فقط: لا تملك صلاحية {isNew ? 'الإضافة' : 'التعديل'}</span>}
        <button type="button" className="btn ghost" onClick={close}>إغلاق</button>
        {(isDirty || balDirty) && <span className="hint"><span className="dirty-dot" /> تعديلات غير محفوظة</span>}
      </div>
    </form>
  )
}

export { kindLabel }
