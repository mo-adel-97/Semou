import { useEffect, useMemo, useRef, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Controller, useForm, type Control, type UseFormRegister, type FieldErrors } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AnimatePresence, motion } from 'motion/react'
import { CheckCircle2, Database, FileStack, Plus, Save, Search, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { useBranches, useLookup } from '../lib/lookups'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/* ============================================================================
   Metadata-driven screen. The server (Features/Screens/ScreenRegistry.cs) describes
   fields, validation, columns and sub-grids; this component renders list + card.
   Rules are enforced again on the server, so the UI can never bypass them.
   ============================================================================ */

type FieldType = 'Text' | 'TextArea' | 'Int' | 'Decimal' | 'Bool' | 'Select' | 'Combo' | 'Lookup'
interface FieldMeta {
  name: string; label: string; type: FieldType; required: string | null; nonZero: boolean; maxLength: number | null
  options: string[] | null; suggestions: string[] | null; lookup: string | null; section: string | null; span: number
  suffix: string | null; hint: string | null; lockOnEdit: boolean; ltr: boolean; default: unknown
}
interface ColumnMeta { name: string; label: string; kind: 'Text' | 'Code' | 'Status' | 'Option' | 'Number' | 'Lookup'; optionsFrom: string | null }
interface ChildMeta { name: string; label: string; kind: 'BranchSet' | 'BranchNumbers'; valueLabel: string | null; required: string | null }
interface ScreenMeta {
  key: string; form: string; title: string; singular: string; subtitle: string | null; nameField: string; scopeField: string | null
  fields: FieldMeta[]; columns: ColumnMeta[]; children: ChildMeta[]
}
type Row = Record<string, unknown> & { Guid: string; Code: string; IsUse: boolean | null }
interface Details { code: string; guid: string; values: Record<string, unknown>; children: Record<string, unknown> }
type Status = 'all' | 'on' | 'off'
type Values = Record<string, unknown>
type ChildState = Record<string, string[] | Record<string, string>>

const NO_ROWS: Row[] = []
const fmt = (n: number) => n.toLocaleString('ar-EG')

/* ---------- Zod schema built from the same metadata the server validates with ---------- */
function buildSchema(fields: FieldMeta[], isNew: boolean) {
  const shape: Record<string, z.ZodType> = {}
  for (const f of fields) {
    const req = f.required ?? undefined
    switch (f.type) {
      case 'Bool': shape[f.name] = z.boolean(); break
      case 'Int':
      case 'Decimal': {
        let s = z.string().trim()
          .refine(v => v === '' || !isNaN(Number(v)), `${f.label}: قيمة غير صحيحة`)
          .refine(v => v === '' || Number(v) >= 0, `${f.label} لا يمكن أن يكون بالسالب`)
        if (req) s = s.refine(v => v !== '' && (!f.nonZero || Number(v) !== 0), req)
        if (f.type === 'Int') s = s.refine(v => v === '' || Number.isInteger(Number(v)), `${f.label}: رقم صحيح فقط`)
        shape[f.name] = s
        break
      }
      case 'Select': shape[f.name] = req ? z.number().int().min(0, req) : z.number().int(); break
      case 'Lookup': shape[f.name] = req && !(f.lockOnEdit && !isNew) ? z.string().min(1, req) : z.string(); break
      default: {
        let s = z.string().trim()
        if (f.maxLength && f.maxLength < 100000) s = s.max(f.maxLength, `${f.label}: الحد الأقصى ${f.maxLength} حرف`)
        shape[f.name] = req ? s.min(1, req) : s
      }
    }
  }
  return z.object(shape)
}

function emptyValues(fields: FieldMeta[]): Values {
  const v: Values = {}
  for (const f of fields) {
    if (f.type === 'Bool') v[f.name] = f.default ?? false
    else if (f.type === 'Select') v[f.name] = typeof f.default === 'number' ? f.default : -1
    else if (f.type === 'Int' || f.type === 'Decimal') v[f.name] = f.default != null ? String(f.default) : ''
    else v[f.name] = f.default != null ? String(f.default) : ''
  }
  return v
}

function fromServer(fields: FieldMeta[], src: Values): Values {
  const v = emptyValues(fields)
  for (const f of fields) {
    const x = src[f.name]
    if (x === null || x === undefined) continue
    if (f.type === 'Bool') v[f.name] = !!x
    else if (f.type === 'Select') v[f.name] = typeof x === 'number' ? x : Number(x)
    else v[f.name] = String(x)
  }
  return v
}

function toServer(fields: FieldMeta[], v: Values) {
  const out: Values = {}
  for (const f of fields) {
    const x = v[f.name]
    if (f.type === 'Int' || f.type === 'Decimal') out[f.name] = x === '' ? null : Number(x)
    else if (f.type === 'Lookup') out[f.name] = x || null
    else out[f.name] = x
  }
  return out
}

/* ============================== The screen ============================== */
export function CrudScreen({ screen, group }: { screen: string; group: string }) {
  const { can } = useAuth()
  const meta = useQuery({ queryKey: ['screens', screen, 'meta'], queryFn: () => api<ScreenMeta>(`/screens/${screen}/meta`), staleTime: Infinity })
  const m = meta.data

  if (meta.isPending) return <><PageHead title="…" group={group} /><div className="card card-pad"><div className="sk" style={{ height: 300 }} /></div></>
  if (!m) return <><PageHead title="خطأ" group={group} /><div className="card soon"><h2>تعذر تحميل الشاشة</h2><p>{meta.error?.message}</p></div></>
  return <ScreenBody m={m} group={group} canFind={can(m.form, 'find')} canAdd={can(m.form, 'add')} />
}

function ScreenBody({ m, group, canFind, canAdd }: { m: ScreenMeta; group: string; canFind: boolean; canAdd: boolean }) {
  const [status, setStatus] = useState<Status>('all')
  const [scope, setScope] = useState('')
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const branches = useBranches()

  const list = useQuery({
    queryKey: ['screens', m.key, 'list', status, scope],
    queryFn: () => api<Row[]>(`/screens/${m.key}${qs({ isUse: status === 'all' ? null : status === 'on', scope: scope || null })}`),
    enabled: canFind,
    placeholderData: keepPreviousData,
  })
  const rows = list.data ?? NO_ROWS
  const active = rows.filter(r => r.IsUse).length

  const columns = useMemo<GridColumn<Row>[]>(() => m.columns.map(c => {
    const opts = c.optionsFrom ? m.fields.find(f => f.name === c.optionsFrom)?.options ?? [] : []
    const base: GridColumn<Row> = { id: c.name, header: c.label, accessorFn: r => r[c.name] as string }
    switch (c.kind) {
      case 'Code': return { ...base, sortFn: 'alphanumeric', meta: { width: 90 }, cell: x => <span className="code-chip">{String(x.getValue() ?? '')}</span> }
      case 'Status': return { ...base, enableGlobalFilter: false, meta: { width: 110 }, cell: x => <StatusBadge on={x.getValue() as boolean} /> }
      case 'Option': return { ...base, accessorFn: r => opts[r[c.name] as number] ?? '—' }
      case 'Number': return { ...base, cell: x => <span style={{ fontVariantNumeric: 'tabular-nums' }}>{x.getValue() == null ? '—' : Number(x.getValue()).toLocaleString('ar-EG')}</span> }
      default: return { ...base, cell: x => (c.name === m.nameField ? <b>{String(x.getValue() ?? '')}</b> : String(x.getValue() ?? '—')) }
    }
  }), [m])

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  return (
    <>
      <PageHead title={m.title} subtitle={m.subtitle ?? undefined} group={group}
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> {m.singular} جديد <kbd>F1</kbd></button>} />

      {!canFind ? (
        <div className="card soon">
          <div className="ic"><Search size={36} /></div>
          <h2>لا تملك صلاحية استعراض البيانات</h2>
          <p>{canAdd ? `يمكنك إضافة ${m.singular} جديد فقط حسب صلاحياتك.` : 'يرجى التواصل مع مدير النظام.'}</p>
        </div>
      ) : (
        <>
          <div className="stats">
            <Stat icon={<Database size={22} />} value={list.isPending ? '…' : fmt(rows.length)} label={`إجمالي ${m.title}`} />
            <Stat icon={<CheckCircle2 size={22} />} value={list.isPending ? '…' : fmt(active)} label="فعال" delay={0.05} />
            <Stat icon={<XCircle size={22} />} tone="danger" value={list.isPending ? '…' : fmt(rows.length - active)} label="موقوف" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid}
            searchPlaceholder={`ابحث في ${m.title}...`} onRowClick={r => setEditing({ guid: r.Guid })}
            toolbar={<>
              <Segmented<Status> value={status} onChange={setStatus} options={[{ value: 'all', label: 'الكل' }, { value: 'on', label: 'فعال' }, { value: 'off', label: 'موقوف' }]} />
              {m.scopeField && (
                <select className="input" style={{ width: 'auto', minWidth: 200 }} value={scope} onChange={e => setScope(e.target.value)} aria-label="الفرع">
                  <option value="">كل الفروع</option>
                  {branches.data?.map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
                </select>
              )}
              {list.isFetching && !list.isPending && <Spinner size={16} />}
            </>} />
        </>
      )}

      <AnimatePresence>
        {editing && <RecordCard key={editing.guid ?? 'new'} m={m} guid={editing.guid} defaultScope={scope} onClose={() => setEditing(null)} />}
      </AnimatePresence>
    </>
  )
}

/* ============================== Record card ============================== */
function RecordCard({ m, guid: initialGuid, defaultScope, onClose }: { m: ScreenMeta; guid: string | null; defaultScope: string; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const [created, setCreated] = useState<{ code: string; guid: string } | null>(null)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid
  const canSave = isNew ? can(m.form, 'add') : can(m.form, 'edit')
  const ro = !canSave

  const details = useQuery({
    queryKey: ['screens', m.key, 'one', initialGuid],
    queryFn: () => api<Details>(`/screens/${m.key}/${initialGuid}`),
    enabled: !!initialGuid,
    staleTime: 0,
  })

  const schema = useMemo(() => buildSchema(m.fields, isNew), [m.fields, isNew])
  const initial = useMemo(() => {
    const v = emptyValues(m.fields)
    if (m.scopeField && defaultScope) v[m.scopeField] = defaultScope
    return v
  }, [m, defaultScope])
  const { register, handleSubmit, control, reset, formState: { errors, isDirty } } = useForm<Values>({ resolver: zodResolver(schema), defaultValues: initial })

  // Sub-grids live outside RHF; track their dirty state separately.
  const [children, setChildren] = useState<ChildState>(() => Object.fromEntries(m.children.map(c => [c.name, c.kind === 'BranchSet' ? [] : {}])))
  const childBaseline = useRef(JSON.stringify(children))
  const childDirty = JSON.stringify(children) !== childBaseline.current

  useEffect(() => {
    if (!details.data) return
    reset(fromServer(m.fields, details.data.values))
    const ch: ChildState = {}
    for (const c of m.children) {
      const src = details.data.children[c.name]
      ch[c.name] = c.kind === 'BranchSet'
        ? ((src as string[]) ?? [])
        : Object.fromEntries(((src as { branchGuid: string; value: number }[]) ?? []).map(r => [r.branchGuid, String(r.value)]))
    }
    childBaseline.current = JSON.stringify(ch)
    setChildren(ch)
  }, [details.data, m, reset])

  const closeRef = useRef(onClose)
  useEffect(() => { closeRef.current = onClose })
  useEffect(() => { if (details.isError) closeRef.current() }, [details.isError])

  const save = useMutation({
    mutationFn: async (v: Values) => {
      const body = {
        values: toServer(m.fields, v),
        children: Object.fromEntries(m.children.map(c => [c.name, c.kind === 'BranchSet'
          ? children[c.name]
          : Object.entries(children[c.name] as Record<string, string>).filter(([, x]) => x !== '').map(([branchGuid, x]) => ({ branchGuid, value: Number(x) }))])),
      }
      for (const c of m.children) if (c.required && (body.children[c.name] as unknown[]).length === 0) throw Object.assign(new Error(c.required), { status: 400 })
      if (isNew) return api<{ message: string; code: string; guid: string }>(`/screens/${m.key}`, { method: 'POST', json: body })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/screens/${m.key}/${guid}`, { method: 'PUT', json: { ...body, reason } })
    },
    onSuccess: async (r, v) => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid })
      reset(v)
      childBaseline.current = JSON.stringify(children)
      qc.invalidateQueries({ queryKey: ['screens', m.key] })
      await alerts.success(r.message, isNew ? `الكود: ${r.code}` : undefined)
    },
  })

  const submit = handleSubmit(v => save.mutate(v), errs => {
    const first = m.fields.map(f => errs[f.name]?.message).find(Boolean)
    alerts.error(String(first ?? 'يرجى مراجعة البيانات المدخلة'))
  })

  const dirty = isDirty || childDirty
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const loading = !!initialGuid && details.isPending
  const code = created?.code ?? details.data?.code
  const title = isNew ? `${m.singular} جديد` : String(details.data?.values[m.nameField] ?? m.singular)

  // Group fields into sections (desktop group boxes).
  const sections = useMemo(() => {
    const out: { title: string | null; fields: FieldMeta[] }[] = []
    for (const f of m.fields) {
      if (f.section || out.length === 0) out.push({ title: f.section, fields: [] })
      out[out.length - 1].fields.push(f)
    }
    return out
  }, [m.fields])

  return (
    <Drawer onClose={requestClose} icon={<FileStack size={22} />} title={title}
      subtitle={isNew ? 'أدخل البيانات ثم احفظ' : `الكود ${code ?? ''}`}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{ro ? `عرض فقط: لا تملك صلاحية ${isNew ? 'الإضافة' : 'التعديل'}` : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? (
        <div className="grid cols-2">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="field"><div className="sk" style={{ width: 90 }} /><div className="sk" style={{ height: 42, borderRadius: 12 }} /></div>)}</div>
      ) : (
        <form className="grid cols-2" onSubmit={e => { e.preventDefault(); submit() }} noValidate>
          <Field label="الكود" hint={isNew ? 'يُولَّد تلقائياً عند الحفظ' : undefined}><input className="input" readOnly value={code ?? 'تلقائي'} /></Field>
          <div />
          {sections.map((s, si) => (
            <SectionBlock key={si} title={s.title}>
              {s.fields.map((f, fi) => (
                <FieldInput key={f.name} f={f} register={register} control={control} errors={errors}
                  readOnly={ro || (f.lockOnEdit && !isNew)} autoFocus={si === 0 && fi === 0 && !ro} />
              ))}
            </SectionBlock>
          ))}
          {m.children.map(c => (
            <ChildEditor key={c.name} c={c} value={children[c.name]} readOnly={ro}
              onChange={v => setChildren(prev => ({ ...prev, [c.name]: v }))} />
          ))}
        </form>
      )}
    </Drawer>
  )
}

function SectionBlock({ title, children }: { title: string | null; children: React.ReactNode }) {
  return <>{title && <div className="section-label span-all">{title}</div>}{children}</>
}

/* ---------- One input, chosen by field type ---------- */
function FieldInput({ f, register, control, errors, readOnly, autoFocus }: {
  f: FieldMeta; register: UseFormRegister<Values>; control: Control<Values>; errors: FieldErrors<Values>; readOnly: boolean; autoFocus: boolean
}) {
  const err = errors[f.name]?.message as string | undefined
  const cls = `input ${err ? 'invalid' : ''}`
  const span = f.span === 2 || f.type === 'TextArea' ? 'span-2' : ''
  const lookup = useLookup(f.type === 'Lookup' ? f.lookup : null)

  if (f.type === 'Bool') {
    return (
      <div className={`field ${span}`} style={{ justifyContent: 'center' }}>
        <Controller control={control} name={f.name} render={({ field }) => <Switch label={f.label} checked={!!field.value} disabled={readOnly} onChange={field.onChange} />} />
      </div>
    )
  }

  let input: React.ReactNode
  switch (f.type) {
    case 'TextArea':
      input = <textarea className={cls} readOnly={readOnly} autoFocus={autoFocus} maxLength={f.maxLength && f.maxLength < 100000 ? f.maxLength : undefined} {...register(f.name)} />
      break
    case 'Int':
    case 'Decimal':
      input = <input className={cls} readOnly={readOnly} autoFocus={autoFocus} inputMode={f.type === 'Int' ? 'numeric' : 'decimal'} dir="ltr" {...register(f.name)} />
      break
    case 'Select':
      input = (
        <Controller control={control} name={f.name} render={({ field }) => (
          <select className={cls} disabled={readOnly} autoFocus={autoFocus} value={String(field.value ?? -1)} onChange={e => field.onChange(Number(e.target.value))}>
            <option value="-1" disabled>اختر...</option>
            {f.options?.map((o, i) => <option key={i} value={i}>{o}</option>)}
          </select>
        )} />
      )
      break
    case 'Lookup':
      input = (
        <Controller control={control} name={f.name} render={({ field }) => (
          <select className={cls} disabled={readOnly || lookup.loading} autoFocus={autoFocus} value={String(field.value ?? '')} onChange={e => field.onChange(e.target.value)}>
            <option value="">{lookup.loading ? 'جاري التحميل...' : 'اختر...'}</option>
            {lookup.items.filter(i => i.active || i.value === field.value).map(i => <option key={i.value} value={i.value}>{i.label}{i.active ? '' : ' (موقوف)'}</option>)}
          </select>
        )} />
      )
      break
    case 'Combo':
      input = <>
        <input className={cls} readOnly={readOnly} autoFocus={autoFocus} list={`dl-${f.name}`} maxLength={f.maxLength ?? undefined} {...register(f.name)} />
        <datalist id={`dl-${f.name}`}>{f.suggestions?.map(s => <option key={s} value={s} />)}</datalist>
      </>
      break
    default:
      input = <input className={cls} readOnly={readOnly} autoFocus={autoFocus} dir={f.ltr ? 'ltr' : undefined} maxLength={f.maxLength ?? undefined} {...register(f.name)} />
  }

  return (
    <Field label={f.label} required={!!f.required} error={err} hint={f.lockOnEdit ? 'لا يمكن تغييره بعد الحفظ' : f.hint ?? undefined} className={span}>
      {f.suffix ? <div className="input-suffix">{input}<span>{f.suffix}</span></div> : input}
    </Field>
  )
}

/* ---------- Branch sub-grids ---------- */
function ChildEditor({ c, value, readOnly, onChange }: {
  c: ChildMeta; value: string[] | Record<string, string>; readOnly: boolean; onChange: (v: string[] | Record<string, string>) => void
}) {
  const branches = useBranches()
  const list = (branches.data ?? [])

  if (c.kind === 'BranchSet') {
    const set = new Set(value as string[])
    const shown = list.filter(b => b.isUse || set.has(b.guid))
    return (
      <div className="span-all">
        <div className="section-label">{c.label}{c.required && <span className="req" style={{ color: 'var(--danger)' }}>*</span>}</div>
        <div className="chips" style={{ marginTop: 14 }}>
          {branches.isPending && <Spinner />}
          {shown.map(b => {
            const on = set.has(b.guid)
            return (
              <motion.button type="button" key={b.guid} whileTap={{ scale: 0.95 }} className={`chip ${on ? 'on' : ''}`} disabled={readOnly}
                onClick={() => onChange(on ? (value as string[]).filter(g => g !== b.guid) : [...(value as string[]), b.guid])}>
                <span className="tick">{on ? '✓' : ''}</span>{b.name}
              </motion.button>
            )
          })}
        </div>
        <div className="hint muted" style={{ fontSize: 12, marginTop: 6 }}>تم اختيار {set.size} من {shown.length}</div>
      </div>
    )
  }

  const map = value as Record<string, string>
  const shown = list.filter(b => b.isUse || map[b.guid] !== undefined)
  return (
    <div className="span-all">
      <div className="section-label">{c.label}</div>
      <div className="mini-table" style={{ marginTop: 14 }}>
        <div className="mt-head"><span>الفرع</span><span>{c.valueLabel}</span></div>
        {branches.isPending && <div style={{ padding: 16 }}><Spinner /></div>}
        {shown.map(b => (
          <div className="mt-row" key={b.guid}>
            <span>{b.name}</span>
            <input className="input" dir="ltr" inputMode="numeric" placeholder="—" readOnly={readOnly} value={map[b.guid] ?? ''}
              onChange={e => {
                const v = e.target.value.replace(/[^\d.]/g, '')
                const next = { ...map }
                if (v === '') delete next[b.guid]; else next[b.guid] = v
                onChange(next)
              }} />
          </div>
        ))}
      </div>
      <div className="hint muted" style={{ fontSize: 12, marginTop: 6 }}>اترك الحقل فارغاً للفروع غير المشمولة</div>
    </div>
  )
}
