import { useEffect, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Controller, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AnimatePresence } from 'motion/react'
import { Building2, CheckCircle2, ClipboardList, Plus, Save, Search, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'
import { CostCenterPicker } from '../components/CostCenterPicker'

/* Port of the desktop "بطاقة فرع" (EncodingBranch) + NewBranchList. Permission key: addbranch. */
const FORM = 'addbranch'

interface BranchRow {
  code: string; guid: string; brName: string | null; brEName: string | null; isUse: boolean | null
  isRegOn: boolean | null; branchQCode: string | null; brTel: string | null; brVatId: string | null
}
type BranchDetails = Partial<Record<keyof BranchForm, unknown>> & { guid: string; code: string }
interface Saved { message: string; code: string; guid: string }

/* ---------- Query keys & API ---------- */
const keys = {
  list: (status: Status) => ['branches', 'list', status] as const,
  one: (code: string) => ['branches', 'one', code] as const,
  all: ['branches'] as const,
}
type Status = 'all' | 'on' | 'off'

/* ---------- Validation: same rules and messages as VALIDATE_DATA in the desktop form ---------- */
const opt = (max: number) => z.string().trim().max(max, `الحد الأقصى ${max} حرف`)
const schema = z.object({
  brName: z.string().trim().min(1, 'يرجى إدخال اسم الفرع').max(150, 'الحد الأقصى 150 حرف'),
  brEName: opt(150),
  brAddress: opt(150),
  brWebSite: opt(50),
  brEmail: opt(50),
  brTel: opt(50),
  brFax: opt(50),
  brVatId: z.string().trim().min(1, 'يرجى إدخال الرقم الضريبي').max(50, 'الحد الأقصى 50 حرف'),
  vatName: z.string().trim().min(1, 'يرجى إدخال الاسم الضريبي').max(200, 'الحد الأقصى 200 حرف'),
  branchQCode: opt(50),
  costCenterGuid: z.string().nullable().refine(v => !!v, 'يرجى اختيار مركز التكلفة الافتراضي'),
  centerName: z.string(),
  brVatSMain: z.boolean(),
  isUse: z.boolean(),
  isRegOn: z.boolean(),
})
type BranchForm = z.infer<typeof schema>

const EMPTY: BranchForm = {
  brName: '', brEName: '', brAddress: '', brWebSite: '', brEmail: '', brTel: '', brFax: '', brVatId: '',
  vatName: '', branchQCode: '', costCenterGuid: null, centerName: '', brVatSMain: false, isUse: true, isRegOn: true,
}

/* ---------- Grid columns (module scope = stable for TanStack Table v9) ---------- */
const columns: GridColumn<BranchRow>[] = [
  { accessorKey: 'code', header: 'الكود', meta: { width: 90 }, sortFn: 'alphanumeric', cell: c => <span className="code-chip">{c.getValue() as string}</span> },
  { accessorKey: 'brName', header: 'اسم الفرع', cell: c => <b>{c.getValue() as string}</b> },
  { accessorKey: 'brTel', header: 'الهاتف' },
  { accessorKey: 'brVatId', header: 'الرقم الضريبي' },
  { accessorKey: 'isRegOn', header: 'التسجيل', enableGlobalFilter: false, cell: c => <StatusBadge on={c.getValue() as boolean} onText="مفتوح" offText="مغلق" /> },
  { accessorKey: 'isUse', header: 'الحالة', enableGlobalFilter: false, cell: c => <StatusBadge on={c.getValue() as boolean} /> },
]
const NO_ROWS: BranchRow[] = []
const fmt = (n: number) => n.toLocaleString('ar-EG')

export function Branches() {
  const { can } = useAuth()
  const [status, setStatus] = useState<Status>('all')
  const [editing, setEditing] = useState<{ code: string | null } | null>(null)
  const canFind = can(FORM, 'find')
  const canAdd = can(FORM, 'add')

  const list = useQuery({
    queryKey: keys.list(status),
    queryFn: () => api<BranchRow[]>(`/branches${qs({ isUse: status === 'all' ? null : status === 'on' })}`),
    enabled: canFind,
    placeholderData: keepPreviousData,
  })
  const rows = list.data ?? NO_ROWS
  const active = rows.filter(r => r.isUse).length

  // F1 = new record, like the desktop.
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ code: null }) }
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  return (
    <>
      <PageHead title="الفروع" subtitle="بيانات الفروع والإعدادات الضريبية لكل فرع" group="ملف"
        actions={canAdd && <button className="btn" onClick={() => setEditing({ code: null })}><Plus size={17} /> فرع جديد <kbd>F1</kbd></button>} />

      {!canFind ? (
        <div className="card soon">
          <div className="ic"><Search size={36} /></div>
          <h2>لا تملك صلاحية استعراض البيانات</h2>
          <p>{canAdd ? 'يمكنك إضافة فرع جديد فقط حسب صلاحياتك.' : 'يرجى التواصل مع مدير النظام.'}</p>
        </div>
      ) : (
        <>
          <div className="stats">
            <Stat icon={<Building2 size={22} />} value={list.isPending ? '…' : fmt(rows.length)} label="إجمالي الفروع" />
            <Stat icon={<CheckCircle2 size={22} />} value={list.isPending ? '…' : fmt(active)} label="فروع فعالة" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : fmt(rows.length - active)} label="فروع موقوفة" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.guid}
            searchPlaceholder="ابحث بالاسم أو الكود أو الهاتف..."
            onRowClick={r => setEditing({ code: r.code })}
            toolbar={<>
              <Segmented<Status> value={status} onChange={setStatus} options={[
                { value: 'all', label: 'الكل' }, { value: 'on', label: 'فعال' }, { value: 'off', label: 'موقوف' },
              ]} />
              {list.isFetching && !list.isPending && <Spinner size={16} />}
            </>} />
        </>
      )}

      <AnimatePresence>
        {editing && <BranchCard key={editing.code ?? 'new'} code={editing.code} onClose={() => setEditing(null)} />}
      </AnimatePresence>
    </>
  )
}

/* ---------------- The branch card (drawer) ---------------- */
function BranchCard({ code, onClose }: { code: string | null; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const [created, setCreated] = useState<{ code: string; guid: string } | null>(null)
  const [pickCenter, setPickCenter] = useState(false)

  const details = useQuery({
    queryKey: keys.one(code ?? ''),
    queryFn: () => api<BranchDetails>(`/branches/${encodeURIComponent(code!)}`),
    enabled: !!code,
    staleTime: 0,
  })

  const guid = created?.guid ?? details.data?.guid ?? null
  const curCode = created?.code ?? code
  const isNew = !guid
  const canSave = isNew ? can(FORM, 'add') : can(FORM, 'edit')
  const ro = !canSave

  const { register, handleSubmit, control, reset, setValue, formState: { errors, isDirty } } = useForm<BranchForm>({
    resolver: zodResolver(schema),
    defaultValues: EMPTY,
  })

  // Fill the form when the record arrives (nulls from old desktop rows become empty values).
  useEffect(() => {
    if (!details.data) return
    const clean = Object.fromEntries(Object.entries(details.data).filter(([k, v]) => v !== null && k in EMPTY))
    reset({ ...EMPTY, ...clean } as BranchForm)
  }, [details.data, reset])

  useEffect(() => { if (details.isError) onClose() }, [details.isError, onClose])

  const save = useMutation({
    mutationFn: async (v: BranchForm) => {
      const { centerName: _c, ...branch } = v
      void _c
      if (isNew) return api<Saved>('/branches', { method: 'POST', json: branch })
      const reason = await alerts.reason() // UpdateResaon dialog
      if (reason === null) return null
      alerts.loading()
      return api<Saved>(`/branches/${guid}`, { method: 'PUT', json: { branch, reason } })
    },
    onSuccess: async (r, v) => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid })
      reset(v) // clean state = saved values
      qc.invalidateQueries({ queryKey: keys.all })
      await alerts.success(r.message, isNew ? `كود الفرع: ${r.code}` : undefined)
    },
  })

  const onInvalid = () => {
    const first = Object.values(errors)[0]?.message
    alerts.error(first ?? 'يرجى مراجعة البيانات المدخلة')
  }
  const submit = handleSubmit(v => save.mutate(v), () => setTimeout(onInvalid))

  const requestClose = async () => {
    if (isDirty && !save.isPending) {
      const ok = await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true })
      if (!ok) return
    }
    onClose()
  }

  // F2 = save, like the desktop.
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'F2' && canSave && !pickCenter && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() }
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const loading = !!code && details.isPending
  const title = isNew ? 'فرع جديد' : (details.data?.brName as string) || 'بطاقة فرع'
  const input = (name: keyof BranchForm, extra?: React.InputHTMLAttributes<HTMLInputElement>) => (
    <input className={`input ${errors[name] ? 'invalid' : ''}`} readOnly={ro} {...extra} {...register(name)} />
  )

  return (
    <Drawer onClose={requestClose} icon={<Building2 size={22} />} title={title}
      subtitle={isNew ? 'أدخل بيانات الفرع ثم احفظ' : `كود الفرع ${curCode}`}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">
          {ro ? 'عرض فقط: لا تملك صلاحية ' + (isNew ? 'الإضافة' : 'التعديل') : isDirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}
        </span>
      </>}>
      {loading ? (
        <div className="grid cols-2">{Array.from({ length: 10 }).map((_, i) => <div key={i} className="field"><div className="sk" style={{ width: 90 }} /><div className="sk" style={{ height: 42, borderRadius: 12 }} /></div>)}</div>
      ) : (
        <form className="grid cols-2" onSubmit={e => { e.preventDefault(); submit() }} noValidate>
          <div className="section-label span-all">البيانات الأساسية</div>
          <Field label="كود الفرع" hint={isNew ? 'يُولَّد تلقائياً عند الحفظ' : undefined}><input className="input" readOnly value={curCode ?? 'تلقائي'} /></Field>
          <Field label="كود مختصر" error={errors.branchQCode?.message}>{input('branchQCode', { maxLength: 50 })}</Field>
          <Field label="اسم الفرع" required error={errors.brName?.message} className="span-2">{input('brName', { maxLength: 150, autoFocus: true })}</Field>
          <Field label="الاسم الأجنبي" error={errors.brEName?.message} className="span-2">{input('brEName', { maxLength: 150, dir: 'auto' })}</Field>
          <Field label="عنوان الفرع" error={errors.brAddress?.message} className="span-2">{input('brAddress', { maxLength: 150 })}</Field>

          <div className="section-label span-all">التواصل</div>
          <Field label="الهاتف" error={errors.brTel?.message}>{input('brTel', { maxLength: 50, dir: 'ltr', inputMode: 'tel' })}</Field>
          <Field label="البريد الإلكتروني" error={errors.brEmail?.message}>{input('brEmail', { maxLength: 50, dir: 'ltr' })}</Field>
          <Field label="الموقع الإلكتروني" error={errors.brWebSite?.message} className="span-2">{input('brWebSite', { maxLength: 50, dir: 'ltr' })}</Field>

          <div className="section-label span-all">البيانات الضريبية والمحاسبية</div>
          <Field label="الاسم الضريبي" required error={errors.vatName?.message} className="span-2">{input('vatName', { maxLength: 200 })}</Field>
          <Field label="الرقم الضريبي" required error={errors.brVatId?.message}>{input('brVatId', { maxLength: 50, dir: 'ltr', inputMode: 'numeric' })}</Field>
          <Field label="السجل التجاري" error={errors.brFax?.message}>{input('brFax', { maxLength: 50, dir: 'ltr' })}</Field>
          <Field label="مركز التكلفة الافتراضي" required error={errors.costCenterGuid?.message} className="span-2">
            <div className="input-wrap">
              <ClipboardList size={17} />
              <input className={`input ${errors.costCenterGuid ? 'invalid' : ''}`} readOnly placeholder="اختر مركز التكلفة" {...register('centerName')}
                onClick={() => !ro && setPickCenter(true)} style={{ cursor: ro ? 'default' : 'pointer' }} />
              {!ro && <button type="button" className="btn secondary sm trail" onClick={() => setPickCenter(true)}><Search size={14} /> اختيار</button>}
            </div>
          </Field>

          <div className="section-label span-all">الحالة</div>
          <div className="span-all row" style={{ gap: 28, flexWrap: 'wrap' }}>
            <Controller control={control} name="isUse" render={({ field }) => <Switch label="الفرع فعال" checked={field.value} disabled={ro} onChange={field.onChange} />} />
            <Controller control={control} name="isRegOn" render={({ field }) => <Switch label="التسجيل مفتوح" checked={field.value} disabled={ro} onChange={field.onChange} />} />
            <Controller control={control} name="brVatSMain" render={({ field }) => <Switch label="نفس رقم المؤسسة" checked={field.value} disabled={ro} onChange={field.onChange} />} />
          </div>
        </form>
      )}

      <AnimatePresence>
        {pickCenter && (
          <CostCenterPicker onClose={() => setPickCenter(false)} onPick={c => {
            setValue('costCenterGuid', c.guid, { shouldDirty: true, shouldValidate: true })
            setValue('centerName', c.name, { shouldDirty: true })
            setPickCenter(false)
          }} />
        )}
      </AnimatePresence>
    </Drawer>
  )
}
