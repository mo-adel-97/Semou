import { useEffect, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Controller, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AnimatePresence } from 'motion/react'
import { ClipboardList, Landmark, Plus, Save, Search, Vault, CheckCircle2, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/* EncodingCashBox (cash-boxes) and EncodingBank (banks). Saving a new one also creates its ledger account. */
export type CashKind = 'cash-boxes' | 'banks'
const CFG = {
  'cash-boxes': { title: 'الخزائن', singular: 'خزينة', form: 'addcashbox', icon: Vault, subtitle: 'الخزائن النقدية وحساباتها المحاسبية' },
  banks: { title: 'البنوك', singular: 'بنك', form: 'addbank', icon: Landmark, subtitle: 'الحسابات البنكية وحساباتها المحاسبية' },
} as const

interface Row { Code: string; Guid: string; CashName: string; AccountNumber: string | null; IsUse: boolean; UseInSales: boolean; IsPublic: boolean; CenterName: string | null; AccountCode: string | null }
const NO_ROWS: Row[] = []
const yesNo = (v: boolean) => <span className={`badge ${v ? 'ok' : 'muted'}`}>{v ? 'نعم' : 'لا'}</span>

export function CashBoxes({ kind, group }: { kind: CashKind; group: string }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const [status, setStatus] = useState<'all' | 'on' | 'off'>('all')
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const canFind = can(c.form, 'find'), canAdd = can(c.form, 'add')

  const list = useQuery({
    queryKey: ['cash', kind, 'list', status],
    queryFn: () => api<Row[]>(`/accounting/${kind}${qs({ isUse: status === 'all' ? null : status === 'on' })}`),
    enabled: canFind, placeholderData: keepPreviousData,
  })
  const rows = list.data ?? NO_ROWS
  const columns: GridColumn<Row>[] = [
    { accessorKey: 'Code', header: 'الكود', meta: { width: 80 }, cell: x => <span className="code-chip">{x.getValue() as string}</span> },
    { accessorKey: 'CashName', header: `اسم ال${c.singular}`, cell: x => <b>{x.getValue() as string}</b> },
    ...(kind === 'banks' ? [{ accessorKey: 'AccountNumber', header: 'رقم الحساب البنكي', cell: (x: { getValue: () => unknown }) => <span dir="ltr">{String(x.getValue() ?? '')}</span> } as GridColumn<Row>] : []),
    { accessorKey: 'AccountCode', header: 'الحساب المحاسبي' },
    { accessorKey: 'CenterName', header: 'مركز التكلفة', cell: x => (x.getValue() as string) ?? '—' },
    { accessorKey: 'UseInSales', header: 'فواتير المبيعات', enableGlobalFilter: false, cell: x => yesNo(x.getValue() as boolean) },
    { accessorKey: 'IsPublic', header: 'يومية الفرع', enableGlobalFilter: false, cell: x => yesNo(x.getValue() as boolean) },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} /> },
  ]

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  const Icon = c.icon
  return (
    <>
      <PageHead title={c.title} subtitle={c.subtitle} group={group}
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> {c.singular} جديد <kbd>F1</kbd></button>} />
      {!canFind ? <div className="card soon"><div className="ic"><Search size={36} /></div><h2>لا تملك صلاحية استعراض البيانات</h2></div> : (
        <>
          <div className="stats">
            <Stat icon={<Icon size={22} />} value={list.isPending ? '…' : rows.length} label={`إجمالي ${c.title}`} />
            <Stat icon={<CheckCircle2 size={22} />} value={list.isPending ? '…' : rows.filter(r => r.IsUse).length} label="فعال" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : rows.filter(r => !r.IsUse).length} label="موقوف" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} searchPlaceholder={`ابحث في ${c.title}...`}
            onRowClick={r => setEditing({ guid: r.Guid })}
            toolbar={<Segmented value={status} onChange={setStatus} options={[{ value: 'all', label: 'الكل' }, { value: 'on', label: 'فعال' }, { value: 'off', label: 'موقوف' }]} />} />
        </>
      )}
      <AnimatePresence>{editing && <CashCard key={editing.guid ?? 'new'} kind={kind} guid={editing.guid} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

function CashCard({ kind, guid: initialGuid, onClose }: { kind: CashKind; guid: string | null; onClose: () => void }) {
  const c = CFG[kind]
  const { can } = useAuth()
  const qc = useQueryClient()
  const [created, setCreated] = useState<{ code: string; guid: string } | null>(null)
  const [pick, setPick] = useState(false)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid
  const canSave = isNew ? can(c.form, 'add') : can(c.form, 'edit')
  const ro = !canSave

  const schema = z.object({
    CashName: z.string().trim().min(1, kind === 'banks' ? 'يرجى إدخال اسم البنك' : 'يرجى إدخال اسم الخزينة').max(150),
    AccountNumber: kind === 'banks' ? z.string().trim().min(1, 'يرجى إدخال الحساب البنكي').max(100) : z.string(),
    CostCenterGuid: kind === 'banks' ? z.string().min(1, 'يرجى اختيار مركز التكلفة الافتراضي') : z.string(),
    CenterName: z.string(),
    IsUse: z.boolean(), UseInSales: z.boolean(), IsPublic: z.boolean(),
  })
  type F = z.infer<typeof schema>
  const { register, handleSubmit, control, reset, setValue, formState: { errors, isDirty } } = useForm<F>({
    resolver: zodResolver(schema), defaultValues: { CashName: '', AccountNumber: '', CostCenterGuid: '', CenterName: '', IsUse: true, UseInSales: false, IsPublic: false },
  })
  const parent = useQuery({ queryKey: ['cash', kind, 'parent'], queryFn: () => api<{ parent: { Name: string; Code: string } | null }>(`/accounting/${kind}/parent`), staleTime: 60_000 })
  const details = useQuery({
    queryKey: ['cash', kind, 'one', initialGuid], queryFn: () => api<Record<string, unknown>>(`/accounting/${kind}/item/${initialGuid}`), enabled: !!initialGuid, staleTime: 0,
  })
  useEffect(() => {
    const d = details.data
    if (!d) return
    reset({
      CashName: String(d.CashName ?? ''), AccountNumber: String(d.AccountNumber ?? ''),
      CostCenterGuid: d.CostCenterGuid && d.CostCenterGuid !== '00000000-0000-0000-0000-000000000000' ? String(d.CostCenterGuid) : '',
      CenterName: String(d.CenterName ?? ''), IsUse: !!d.IsUse, UseInSales: !!d.UseInSales, IsPublic: !!d.IsPublic,
    })
  }, [details.data, reset])

  const save = useMutation({
    mutationFn: async (v: F) => {
      const body = { cashName: v.CashName, accountNumber: v.AccountNumber, costCenterGuid: v.CostCenterGuid || null, isUse: v.IsUse, useInSales: v.UseInSales, isPublic: v.IsPublic }
      if (isNew) return api<{ message: string; code: string; guid: string }>(`/accounting/${kind}/item`, { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/accounting/${kind}/item/${guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async (r, v) => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid })
      reset(v)
      qc.invalidateQueries({ queryKey: ['cash', kind] })
      qc.invalidateQueries({ queryKey: ['chart', 'accounts'] }) // the ledger account appears in the chart
      await alerts.success(r.message, isNew ? `الكود: ${r.code}` : undefined)
    },
  })
  const submit = handleSubmit(v => save.mutate(v), errs => alerts.error(String(Object.values(errs)[0]?.message ?? 'يرجى مراجعة البيانات')))
  const requestClose = async () => {
    if (isDirty && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !pick && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const d = details.data
  const loading = !!initialGuid && details.isPending
  const Icon = c.icon
  return (
    <Drawer onClose={requestClose} icon={<Icon size={22} />} title={isNew ? `${c.singular} جديد` : String(d?.CashName ?? '')}
      subtitle={isNew ? 'سيتم إنشاء الحساب المحاسبي تلقائياً عند الحفظ' : `الكود ${created?.code ?? d?.Code ?? ''}`}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{ro ? 'عرض فقط' : isDirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid">{Array.from({ length: 6 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <form className="grid cols-2" onSubmit={e => { e.preventDefault(); submit() }} noValidate>
          <div className="section-label span-all">البيانات الأساسية</div>
          <Field label={`اسم ال${c.singular}`} required error={errors.CashName?.message} className="span-2"><input className={`input ${errors.CashName ? 'invalid' : ''}`} autoFocus readOnly={ro} {...register('CashName')} /></Field>
          {kind === 'banks' && <Field label="رقم الحساب البنكي" required error={errors.AccountNumber?.message} className="span-2"><input className={`input ${errors.AccountNumber ? 'invalid' : ''}`} dir="ltr" readOnly={ro} {...register('AccountNumber')} /></Field>}
          <Field label="مركز التكلفة الافتراضي" required={kind === 'banks'} error={errors.CostCenterGuid?.message} className="span-2">
            <div className="input-wrap">
              <ClipboardList size={17} />
              <input className={`input ${errors.CostCenterGuid ? 'invalid' : ''}`} readOnly placeholder="اختر مركز التكلفة" {...register('CenterName')} onClick={() => !ro && setPick(true)} style={{ cursor: ro ? 'default' : 'pointer' }} />
              {!ro && <button type="button" className="btn secondary sm trail" onClick={() => setPick(true)}><Search size={14} /> اختيار</button>}
            </div>
          </Field>
          <div className="section-label span-all">الحساب المحاسبي</div>
          <Field label="الحساب الرئيسي" hint="من إعدادات الحسابات"><input className="input" readOnly value={isNew ? (parent.data?.parent ? `${parent.data.parent.Code} - ${parent.data.parent.Name}` : 'غير محدد في إعدادات الحسابات') : String(d?.ParentName ?? '—')} /></Field>
          <Field label="كود الحساب"><input className="input" readOnly value={isNew ? 'تلقائي' : String(d?.AccountCode ?? '—')} /></Field>
          <div className="section-label span-all">الاستخدام</div>
          <div className="span-all grid" style={{ gap: 14 }}>
            <Controller control={control} name="UseInSales" render={({ field }) => <Switch label="تستخدم في فواتير المبيعات" checked={field.value} disabled={ro} onChange={field.onChange} />} />
            <Controller control={control} name="IsPublic" render={({ field }) => <Switch label="تستخدم في يومية الفرع" checked={field.value} disabled={ro} onChange={field.onChange} />} />
            <Controller control={control} name="IsUse" render={({ field }) => <Switch label="فعال" checked={field.value} disabled={ro} onChange={field.onChange} />} />
          </div>
        </form>
      )}
      <AnimatePresence>
        {pick && <CostCenterPicker onClose={() => setPick(false)} onPick={x => { setValue('CostCenterGuid', x.guid, { shouldDirty: true, shouldValidate: true }); setValue('CenterName', x.name, { shouldDirty: true }); setPick(false) }} />}
      </AnimatePresence>
    </Drawer>
  )
}
