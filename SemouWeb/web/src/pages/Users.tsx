import { useEffect, useMemo, useRef, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Controller, useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AnimatePresence } from 'motion/react'
import { BadgeCheck, Briefcase, Copy, Eraser, Eye, EyeOff, KeyRound, Landmark, LayoutGrid, Mail, Phone, Plus, Save, Search, ShieldCheck, UserCheck, UserCog, Users as UsersIcon, UserX } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { MENU } from '../menu'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { Check, Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch, Tabs } from '../components/ui'

/* Port of the desktop "بطاقة مستخدم" (EncodingUser). Permission key: adduser. */
const FORM = 'adduser'

interface UserRow {
  code: number; guid: string; userName: string; fullName: string; staut_: boolean; sellerName: string | null; screens: number
  jobTitle: string | null; employmentType: string | null; workMobile: string | null; personalMobile: string | null
}
interface HrItem { guid: string; name: string; isUse: boolean; ibanCode: string | null }
interface HrLists { jobs: HrItem[]; types: HrItem[]; specs: HrItem[]; banks: HrItem[] }
interface Profile {
  nationalId: string | null; jobTitleGuid: string | null; employmentTypeGuid: string | null; specializationGuid: string | null; personalMobile: string | null; workMobile: string | null
  email: string | null; iban: string | null; bankGuid: string | null; hireDate: string | null; birthDate: string | null; notes: string | null
}
interface FormItem { guid: string; name: string; unqName: string; code: number }
interface MenuItemDb { guid: string; menuName: string; unqName: string }
interface Catalog { forms: FormItem[]; menus: MenuItemDb[]; sellers: { guid: string; name: string; isUse: boolean | null }[]; users: UserRow[]; hr: HrLists }
type PermKey = 'add' | 'edit' | 'delete' | 'print' | 'find' | 'view'
type PermRow = Record<PermKey, boolean>
type Perms = Record<string, PermRow>
interface Details {
  code: string; guid: string
  user: {
    fullName: string; userName: string; staut_: boolean; chkBranch: boolean; branchGuid: string | null; chkAmount: boolean; chkOtherFess: boolean; sellerGuid: string | null
    menus: { menuGuid: string; view: boolean }[]; perms: ({ formGuid: string } & PermRow)[]; branches: string[]; profile: Profile | null
  }
}

const PERM_COLS: { key: PermKey; label: string }[] = [
  { key: 'view', label: 'فتح' }, { key: 'find', label: 'استعراض' }, { key: 'add', label: 'إضافة' },
  { key: 'edit', label: 'تعديل' }, { key: 'delete', label: 'حذف' }, { key: 'print', label: 'طباعة' },
]
const NONE: PermRow = { add: false, edit: false, delete: false, print: false, find: false, view: false }
const ALL: PermRow = { add: true, edit: true, delete: true, print: true, find: true, view: true }
const anyOn = (p?: PermRow) => !!p && PERM_COLS.some(c => p[c.key])

/** Group Form_Name keys under the same sections as the main menu; the rest go to "صلاحيات إضافية". */
function groupForms(forms: FormItem[]) {
  const used = new Set<string>()
  const groups = MENU.map(g => {
    const keys = new Set(g.items.map(i => i.perm).filter(Boolean) as string[])
    const items = forms.filter(f => keys.has(f.unqName?.toLowerCase()))
    items.forEach(f => used.add(f.guid))
    return { key: g.key, title: g.title, icon: g.icon, items }
  }).filter(g => g.items.length)
  const rest = forms.filter(f => !used.has(f.guid))
  if (rest.length) groups.push({ key: 'extra', title: 'صلاحيات إضافية (عمليات الطلاب والمستندات)', icon: ShieldCheck, items: rest })
  return groups
}

const columns: GridColumn<UserRow>[] = [
  { accessorKey: 'code', header: 'الكود', meta: { width: 80 }, cell: c => <span className="code-chip">{c.getValue() as number}</span> },
  { accessorKey: 'fullName', header: 'اسم الموظف', cell: c => <b>{c.getValue() as string}</b> },
  { accessorKey: 'jobTitle', header: 'الوظيفة', cell: c => {
    const r = c.row.original
    return r.jobTitle ? <span className="emp-job">{r.jobTitle}{r.employmentType && <span className="badge muted">{r.employmentType}</span>}</span> : '—'
  } },
  { accessorKey: 'workMobile', header: 'الجوال', cell: c => { const v = (c.getValue() as string) ?? c.row.original.personalMobile; return v ? <span dir="ltr">{v}</span> : '—' } },
  { accessorKey: 'userName', header: 'اسم المستخدم', cell: c => <span dir="auto">{c.getValue() as string}</span> },
  { accessorKey: 'sellerName', header: 'مندوب البيع', cell: c => (c.getValue() as string) ?? '—' },
  { accessorKey: 'screens', header: 'الشاشات المتاحة', cell: c => <span className="badge muted">{c.getValue() as number} شاشة</span> },
  { accessorKey: 'staut_', header: 'الحالة', enableGlobalFilter: false, cell: c => <StatusBadge on={c.getValue() as boolean} /> },
]
const NO_ROWS: UserRow[] = []

export function Users() {
  const { can } = useAuth()
  const [status, setStatus] = useState<'all' | 'on' | 'off'>('all')
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const canFind = can(FORM, 'find')
  const canAdd = can(FORM, 'add')

  const list = useQuery({
    queryKey: ['users', 'list', status],
    queryFn: () => api<UserRow[]>(`/users${qs({ isUse: status === 'all' ? null : status === 'on' })}`),
    enabled: canFind, placeholderData: keepPreviousData,
  })
  const rows = list.data ?? NO_ROWS
  const active = rows.filter(r => r.staut_).length

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F1' && !editing && canAdd) { e.preventDefault(); setEditing({ guid: null }) } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [editing, canAdd])

  return (
    <>
      <PageHead title="المستخدمين والصلاحيات" subtitle="حسابات الدخول وصلاحيات كل مستخدم على الشاشات والقوائم والفروع" group="ملف"
        actions={canAdd && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> مستخدم جديد <kbd>F1</kbd></button>} />
      {!canFind ? (
        <div className="card soon"><div className="ic"><Search size={36} /></div><h2>لا تملك صلاحية استعراض البيانات</h2></div>
      ) : (
        <>
          <div className="stats">
            <Stat icon={<UsersIcon size={22} />} value={list.isPending ? '…' : rows.length} label="إجمالي المستخدمين" />
            <Stat icon={<UserCheck size={22} />} value={list.isPending ? '…' : active} label="مستخدم فعال" delay={0.05} />
            <Stat icon={<UserX size={22} />} tone="danger" value={list.isPending ? '…' : rows.length - active} label="مستخدم موقوف" delay={0.1} />
          </div>
          <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.guid} searchPlaceholder="ابحث باسم الموظف أو اسم المستخدم..."
            onRowClick={r => setEditing({ guid: r.guid })}
            toolbar={<Segmented value={status} onChange={setStatus} options={[{ value: 'all', label: 'الكل' }, { value: 'on', label: 'فعال' }, { value: 'off', label: 'موقوف' }]} />} />
        </>
      )}
      <AnimatePresence>
        {editing && <UserCard key={editing.guid ?? 'new'} guid={editing.guid} onClose={() => setEditing(null)} />}
      </AnimatePresence>
    </>
  )
}

/* ============================== User card ============================== */
const baseSchema = z.object({
  fullName: z.string().trim().min(1, 'يرجى إدخال اسم الموظف').max(150, 'اسم الموظف طويل جداً'),
  userName: z.string().trim().min(1, 'يرجى إدخال اسم المستخدم').max(50, 'اسم المستخدم طويل جداً'),
  password: z.string().max(50, 'كلمة المرور طويلة جداً (الحد الأقصى 50 حرف)'),
  staut_: z.boolean(),
  chkBranch: z.boolean(),
  branchGuid: z.string(),
  chkAmount: z.boolean(),
  chkOtherFess: z.boolean(),
  sellerGuid: z.string().min(1, 'يرجى اختيار مندوب البيع'),
  /* بيانات الموظف (all optional; checked only when typed) */
  nationalId: z.string().refine(v => !v || /^[12]\d{9}$/.test(v), 'رقم الهوية 10 أرقام ويبدأ بـ 1 أو 2'),
  jobTitleGuid: z.string(),
  employmentTypeGuid: z.string(),
  specializationGuid: z.string(),
  personalMobile: z.string().refine(v => !v || /^05\d{8}$/.test(v), 'رقم الجوال 10 أرقام ويبدأ بـ 05'),
  workMobile: z.string().refine(v => !v || /^05\d{8}$/.test(v), 'رقم الجوال 10 أرقام ويبدأ بـ 05'),
  email: z.string().trim().max(150, 'البريد الإلكتروني طويل جداً').refine(v => !v || /^[^@\s]+@[^@\s]+\.[^@\s]{2,}$/.test(v), 'البريد الإلكتروني غير صحيح'),
  iban: z.string().refine(v => !v || ibanOk(v), 'رقم الآيبان غير صحيح'),
  bankGuid: z.string(),
  hireDate: z.string(),
  birthDate: z.string(),
  notes: z.string().max(500, 'الملاحظات طويلة جداً'),
})

/** ISO 13616 mod-97 check (Saudi IBAN = SA + 22 digits). */
function ibanOk(raw: string) {
  const v = raw.replace(/\s/g, '').toUpperCase()
  if (!/^[A-Z]{2}\d{2}[A-Z0-9]{11,30}$/.test(v)) return false
  if (v.startsWith('SA') && !/^SA\d{22}$/.test(v)) return false
  let rem = 0
  for (const ch of v.slice(4) + v.slice(0, 4)) {
    const n = parseInt(ch, 36)
    rem = n >= 10 ? (rem * 100 + n) % 97 : (rem * 10 + n) % 97
  }
  return rem === 1
}
const ibanGroups = (v: string) => v.replace(/(.{4})/g, '$1 ').trim()
const digitsOnly = (v: string, max: number) => v.replace(/\D/g, '').slice(0, max)
const PROFILE_KEYS = ['nationalId', 'jobTitleGuid', 'employmentTypeGuid', 'specializationGuid', 'personalMobile', 'workMobile', 'email', 'iban', 'bankGuid', 'hireDate', 'birthDate', 'notes'] as const
const INFO_KEYS = ['fullName', 'userName', 'password', 'sellerGuid', 'branchGuid']
type Info = z.infer<typeof baseSchema>
const EMPTY: Info = {
  fullName: '', userName: '', password: '', staut_: true, chkBranch: false, branchGuid: '', chkAmount: false, chkOtherFess: false, sellerGuid: '',
  nationalId: '', jobTitleGuid: '', employmentTypeGuid: '', specializationGuid: '', personalMobile: '', workMobile: '', email: '', iban: '', bankGuid: '', hireDate: '', birthDate: '', notes: '',
}
type Tab = 'info' | 'profile' | 'perms' | 'menus' | 'branches'

function UserCard({ guid: initialGuid, onClose }: { guid: string | null; onClose: () => void }) {
  const { can, me } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [tab, setTab] = useState<Tab>('info')
  const [created, setCreated] = useState<{ code: string; guid: string } | null>(null)
  const [showPass, setShowPass] = useState(false)
  const guid = created?.guid ?? initialGuid
  const isNew = !guid
  const canSave = isNew ? can(FORM, 'add') : can(FORM, 'edit')
  const ro = !canSave

  // fresh every time the card opens: a salesman or screen added a moment ago must be in the lists
  const catalog = useQuery({ queryKey: ['users', 'catalog'], queryFn: () => api<Catalog>('/users/catalog'), staleTime: 0, refetchOnMount: 'always' })
  const details = useQuery({ queryKey: ['users', 'one', initialGuid], queryFn: () => api<Details>(`/users/${initialGuid}`), enabled: !!initialGuid, staleTime: 0 })

  const schema = useMemo(() => (isNew ? baseSchema.refine(v => v.password.length > 0, { path: ['password'], message: 'يرجى إدخال كلمة المرور' }) : baseSchema)
    .refine(v => !v.chkBranch || v.branchGuid, { path: ['branchGuid'], message: 'يرجى اختيار الفرع الافتراضي' }), [isNew])
  const { register, handleSubmit, control, reset, setValue, getValues, formState: { errors, isDirty } } = useForm<Info>({ resolver: zodResolver(schema), defaultValues: EMPTY })
  const chkBranch = useWatch({ control, name: 'chkBranch' })
  const iban = useWatch({ control, name: 'iban' })
  const ibanValid = !!iban && ibanOk(iban)
  // A Saudi IBAN carries the bank code (characters 5-6): pick the bank that has that code (شاشة البنوك).
  const ibanCode = iban?.startsWith('SA') && iban.length >= 6 ? iban.slice(4, 6) : null
  const knownBank = ibanCode ? catalog.data?.hr.banks.find(b => b.isUse && b.ibanCode === ibanCode)?.guid : undefined
  useEffect(() => { if (knownBank && !getValues('bankGuid')) setValue('bankGuid', knownBank, { shouldDirty: true }) }, [knownBank, getValues, setValue])
  const reloadCatalog = async () => { await catalog.refetch() }

  // Access state (grids) lives outside RHF.
  const [perms, setPerms] = useState<Perms>({})
  const [menus, setMenus] = useState<Record<string, boolean>>({})
  const [userBranches, setUserBranches] = useState<string[]>([])
  const baseline = useRef('')
  const accessJson = JSON.stringify([perms, menus, userBranches])
  const accessDirty = baseline.current !== '' && accessJson !== baseline.current

  useEffect(() => {
    if (!details.data) {
      if (isNew && catalog.data && baseline.current === '') {
        const m = Object.fromEntries(catalog.data.menus.map(x => [x.guid, false]))
        setMenus(m)
        baseline.current = JSON.stringify([{}, m, []])
      }
      return
    }
    const u = details.data.user
    const pr = Object.fromEntries(PROFILE_KEYS.map(k => [k, u.profile?.[k] ?? '']))
    reset({ ...EMPTY, ...u, ...pr, password: '', branchGuid: u.branchGuid ?? '', sellerGuid: u.sellerGuid ?? '' })
    const p: Perms = Object.fromEntries(u.perms.map(({ formGuid, ...r }) => [formGuid, r]))
    const m = Object.fromEntries((catalog.data?.menus ?? []).map(x => [x.guid, u.menus.find(y => y.menuGuid === x.guid)?.view ?? false]))
    setPerms(p); setMenus(m); setUserBranches(u.branches)
    baseline.current = JSON.stringify([p, m, u.branches])
  }, [details.data, catalog.data, reset, isNew])

  const closeRef = useRef(onClose)
  useEffect(() => { closeRef.current = onClose })
  useEffect(() => { if (details.isError) closeRef.current() }, [details.isError])

  const save = useMutation({
    mutationFn: async (v: Info) => {
      const profile = Object.fromEntries(PROFILE_KEYS.map(k => [k, v[k].trim() || null]))
      const info = Object.fromEntries(Object.entries(v).filter(([k]) => !(PROFILE_KEYS as readonly string[]).includes(k)))
      const user = {
        ...info, profile, branchGuid: v.chkBranch ? v.branchGuid || null : null, sellerGuid: v.sellerGuid || null, password: v.password || null,
        menus: Object.entries(menus).map(([menuGuid, view]) => ({ menuGuid, view })),
        perms: Object.entries(perms).filter(([, r]) => anyOn(r)).map(([formGuid, r]) => ({ formGuid, ...r })),
        branches: userBranches,
      }
      if (isNew) return api<{ message: string; code: string; guid: string }>('/users', { method: 'POST', json: user })
      const reason = await alerts.reason()
      if (reason === null) return null
      alerts.loading()
      return api<{ message: string; code: string; guid: string }>(`/users/${guid}`, { method: 'PUT', json: { user, reason } })
    },
    onSuccess: async (r, v) => {
      if (!r) return
      if (isNew) setCreated({ code: r.code, guid: r.guid })
      reset({ ...v, password: '' })
      baseline.current = accessJson
      qc.invalidateQueries({ queryKey: ['users'] })
      if (guid === me?.userGuid) qc.invalidateQueries({ queryKey: ['auth', 'me'] }) // your own menu updates now
      await alerts.success(r.message, isNew ? `كود المستخدم: ${r.code}` : undefined)
    },
  })

  const submit = handleSubmit(v => save.mutate(v), errs => {
    const first = Object.keys(errs)[0]
    setTab(INFO_KEYS.includes(first) ? 'info' : 'profile')
    alerts.error(String(Object.values(errs)[0]?.message ?? 'يرجى مراجعة البيانات المدخلة'))
  })

  const dirty = isDirty || accessDirty
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'F2' && canSave && !document.querySelector('.swal2-container')) { e.preventDefault(); submit() } }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  })

  const loading = (!!initialGuid && details.isPending) || catalog.isPending
  const code = created?.code ?? details.data?.code
  const permCount = Object.values(perms).filter(p => p.view).length
  const menuCount = Object.values(menus).filter(Boolean).length

  /* --- Desktop buttons: حقوق كاملة / حذف الكل / نسخ من --- */
  const fullControl = async () => {
    if (!catalog.data) return
    if (!(await alerts.confirm({ title: 'منح حقوق كاملة؟', text: 'سيتم منح المستخدم كل الصلاحيات على كل الشاشات والقوائم', confirmText: 'منح الكل' }))) return
    setPerms(Object.fromEntries(catalog.data.forms.map(f => [f.guid, { ...ALL }])))
    setMenus(Object.fromEntries(catalog.data.menus.map(m => [m.guid, true])))
    notify.success('تم منح حقوق كاملة. احفظ لتطبيقها')
  }
  const clearAll = async () => {
    if (!(await alerts.confirm({ title: 'حذف كل الصلاحيات؟', text: 'سيتم سحب كل الصلاحيات والقوائم من المستخدم', confirmText: 'حذف الكل', danger: true }))) return
    setPerms({})
    setMenus(m => Object.fromEntries(Object.keys(m).map(k => [k, false])))
  }
  const copyFrom = async () => {
    const others = (catalog.data?.users ?? []).filter(u => u.guid !== guid)
    const { default: Swal } = await import('sweetalert2')
    const r = await Swal.fire({
      title: 'نسخ الصلاحيات من مستخدم', input: 'select', inputOptions: Object.fromEntries(others.map(u => [u.guid, `${u.fullName} (${u.screens} شاشة)`])),
      inputPlaceholder: 'اختر المستخدم', showCancelButton: true, confirmButtonText: 'نسخ', cancelButtonText: 'إلغاء', buttonsStyling: false,
      customClass: { popup: 'sm-popup', title: 'sm-title', confirmButton: 'btn', cancelButton: 'btn ghost', actions: 'sm-actions', input: 'input' },
      inputValidator: v => (!v ? 'يرجى اختيار مستخدم' : undefined),
    })
    if (!r.isConfirmed || !r.value) return
    alerts.loading('جاري نسخ الصلاحيات...')
    try {
      const src = await api<Details>(`/users/${r.value}`)
      setPerms(Object.fromEntries(src.user.perms.map(({ formGuid, ...x }) => [formGuid, x])))
      setMenus(Object.fromEntries((catalog.data?.menus ?? []).map(x => [x.guid, src.user.menus.find(y => y.menuGuid === x.guid)?.view ?? false])))
      alerts.close()
      notify.success('تم نسخ الصلاحيات. احفظ لتطبيقها')
    } catch (e) { alerts.error((e as Error).message) }
  }

  return (
    <Drawer size="lg" onClose={requestClose} icon={<UserCog size={22} />}
      title={isNew ? 'مستخدم جديد' : details.data?.user.fullName ?? 'بطاقة مستخدم'}
      subtitle={isNew ? 'بيانات الدخول والصلاحيات' : `كود المستخدم ${code ?? ''}`}
      tabs={<Tabs value={tab} onChange={setTab} items={[
        { value: 'info', label: 'البيانات الأساسية' },
        { value: 'profile', label: 'بيانات الموظف' },
        { value: 'perms', label: 'صلاحيات الشاشات', badge: permCount },
        { value: 'menus', label: 'القوائم الرئيسية', badge: menuCount },
        { value: 'branches', label: 'فروع التقارير', badge: userBranches.length },
      ]} />}
      footer={<>
        {canSave && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>} <kbd>F2</kbd></button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق <kbd>Esc</kbd></button>
        <span className="hint">{ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="grid cols-2">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="sk" style={{ height: 42 }} />)}</div> : (
        <>
          {/* -------- Info -------- */}
          <form className="grid cols-2" hidden={tab !== 'info'} onSubmit={e => { e.preventDefault(); submit() }} noValidate>
            <Field label="الكود"><input className="input" readOnly value={code ?? 'تلقائي'} /></Field>
            <div />
            <div className="section-label span-all">بيانات الدخول</div>
            <Field label="اسم الموظف" required error={errors.fullName?.message} className="span-2">
              <input className={`input ${errors.fullName ? 'invalid' : ''}`} autoFocus readOnly={ro} {...register('fullName')} />
            </Field>
            <Field label="اسم المستخدم" required error={errors.userName?.message}>
              <input className={`input ${errors.userName ? 'invalid' : ''}`} dir="auto" autoComplete="off" readOnly={ro} {...register('userName')} />
            </Field>
            <Field label="كلمة المرور" required={isNew} error={errors.password?.message} hint={isNew ? undefined : 'اتركها فارغة للإبقاء على كلمة المرور الحالية'}>
              <div className="input-wrap">
                <KeyRound size={17} />
                <input className={`input ${errors.password ? 'invalid' : ''}`} type={showPass ? 'text' : 'password'} autoComplete="new-password" readOnly={ro}
                  placeholder={isNew ? '' : '••••••••'} {...register('password')} />
                <button type="button" className="btn ghost sm icon trail" onClick={() => setShowPass(s => !s)} aria-label="إظهار">{showPass ? <EyeOff size={16} /> : <Eye size={16} />}</button>
              </div>
            </Field>
            <Field label="مندوب البيع" required error={errors.sellerGuid?.message} className="span-2">
              <select className={`input ${errors.sellerGuid ? 'invalid' : ''}`} disabled={ro} {...register('sellerGuid')}>
                <option value="">اختر المندوب...</option>
                {catalog.data?.sellers.map(s => <option key={s.guid} value={s.guid}>{s.name}{s.isUse === false ? ' (موقوف)' : ''}</option>)}
              </select>
            </Field>
            <div className="section-label span-all">الإعدادات</div>
            <div className="span-all grid" style={{ gap: 14 }}>
              <Controller control={control} name="staut_" render={({ field }) => <Switch label="المستخدم فعال" checked={field.value} disabled={ro || guid === me?.userGuid} onChange={field.onChange} />} />
              <Controller control={control} name="chkAmount" render={({ field }) => <Switch label="إدخال مبالغ صفرية" checked={field.value} disabled={ro} onChange={field.onChange} />} />
              <Controller control={control} name="chkOtherFess" render={({ field }) => <Switch label="إضافة رسوم للطالب في عدم وجود ملف تدريبي" checked={field.value} disabled={ro} onChange={field.onChange} />} />
              <Controller control={control} name="chkBranch" render={({ field }) => <Switch label="فروع مخصصة للتقارير" checked={field.value} disabled={ro} onChange={field.onChange} />} />
            </div>
            {chkBranch && (
              <Field label="الفرع الافتراضي" required error={errors.branchGuid?.message} className="span-2">
                <select className={`input ${errors.branchGuid ? 'invalid' : ''}`} disabled={ro} {...register('branchGuid')}>
                  <option value="">اختر الفرع...</option>
                  {branches.data?.filter(b => b.isUse).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
                </select>
              </Field>
            )}
          </form>

          {/* -------- Employee data (dbo.UserProfile) -------- */}
          <div className="grid cols-2" hidden={tab !== 'profile'}>
            <div className="section-label span-all"><Briefcase size={15} /> البيانات الوظيفية</div>
            <Field label="المسمى الوظيفي">
              <Controller control={control} name="jobTitleGuid" render={({ field }) => (
                <HrSelect items={catalog.data?.hr.jobs} value={field.value} onChange={field.onChange} disabled={ro} screen="job-titles" form="webjobtitle" noun="مسمى وظيفي" onAdded={reloadCatalog} />)} />
            </Field>
            <Field label="الحالة الوظيفية">
              <Controller control={control} name="employmentTypeGuid" render={({ field }) => (
                <HrSelect items={catalog.data?.hr.types} value={field.value} onChange={field.onChange} disabled={ro} screen="employment-types" form="webemptype" noun="حالة وظيفية" onAdded={reloadCatalog} />)} />
            </Field>
            <Field label="التخصص / المؤهل" className="span-2">
              <Controller control={control} name="specializationGuid" render={({ field }) => (
                <HrSelect items={catalog.data?.hr.specs} value={field.value} onChange={field.onChange} disabled={ro} screen="specializations" form="webspecial" noun="تخصص" onAdded={reloadCatalog} />)} />
            </Field>
            <Field label="تاريخ المباشرة"><input className="input" type="date" readOnly={ro} {...register('hireDate')} /></Field>
            <Field label="تاريخ الميلاد"><input className="input" type="date" readOnly={ro} {...register('birthDate')} /></Field>

            <div className="section-label span-all"><BadgeCheck size={15} /> الهوية والتواصل</div>
            <Field label="رقم الهوية / الإقامة" error={errors.nationalId?.message} className="span-2">
              <Controller control={control} name="nationalId" render={({ field }) => (
                <input className={`input ${errors.nationalId ? 'invalid' : ''}`} dir="ltr" inputMode="numeric" maxLength={10} readOnly={ro} value={field.value} onBlur={field.onBlur}
                  onChange={e => field.onChange(digitsOnly(e.target.value, 10))} />)} />
            </Field>
            <Field label="الجوال الشخصي" error={errors.personalMobile?.message}>
              <Controller control={control} name="personalMobile" render={({ field }) => (
                <div className="input-wrap"><Phone size={16} />
                  <input className={`input ${errors.personalMobile ? 'invalid' : ''}`} dir="ltr" inputMode="tel" maxLength={10} placeholder="05xxxxxxxx" readOnly={ro} value={field.value} onBlur={field.onBlur}
                    onChange={e => field.onChange(digitsOnly(e.target.value, 10))} /></div>)} />
            </Field>
            <Field label="جوال العمل" error={errors.workMobile?.message}>
              <Controller control={control} name="workMobile" render={({ field }) => (
                <div className="input-wrap"><Phone size={16} />
                  <input className={`input ${errors.workMobile ? 'invalid' : ''}`} dir="ltr" inputMode="tel" maxLength={10} placeholder="05xxxxxxxx" readOnly={ro} value={field.value} onBlur={field.onBlur}
                    onChange={e => field.onChange(digitsOnly(e.target.value, 10))} /></div>)} />
            </Field>
            <Field label="البريد الإلكتروني" error={errors.email?.message} className="span-2">
              <div className="input-wrap"><Mail size={16} />
                <input className={`input ${errors.email ? 'invalid' : ''}`} dir="ltr" type="email" autoComplete="off" readOnly={ro} {...register('email')} /></div>
            </Field>

            <div className="section-label span-all"><Landmark size={15} /> البيانات البنكية</div>
            <Field label="رقم الآيبان (IBAN)" error={errors.iban?.message} className="span-2"
              hint={iban ? (ibanValid ? '✓ رقم الآيبان صحيح' : 'SA + 22 رقم') : 'يبدأ بـ SA ويتكون من 24 خانة'}>
              <Controller control={control} name="iban" render={({ field }) => (
                <input className={`input iban-input ${errors.iban || (iban.length >= 24 && !ibanValid) ? 'invalid' : ''}`} dir="ltr" autoComplete="off" spellCheck={false}
                  placeholder="SA00 0000 0000 0000 0000 0000" readOnly={ro} value={ibanGroups(field.value)} onBlur={field.onBlur}
                  onChange={e => field.onChange(e.target.value.replace(/[^A-Za-z0-9]/g, '').toUpperCase().slice(0, 34))} />)} />
            </Field>
            <Field label="البنك" className="span-2" hint={knownBank ? 'تم تحديد البنك تلقائياً من رقم الآيبان' : undefined}>
              <Controller control={control} name="bankGuid" render={({ field }) => (
                <HrSelect items={catalog.data?.hr.banks} value={field.value} onChange={field.onChange} disabled={ro} screen="banks" form="webbank" noun="بنك" onAdded={reloadCatalog}
                  extra={ibanCode ? { IbanCode: ibanCode } : undefined} />)} />
            </Field>

            <Field label="ملاحظات" error={errors.notes?.message} className="span-2">
              <textarea className="input" readOnly={ro} maxLength={500} {...register('notes')} />
            </Field>
          </div>

          {/* -------- Screen permissions -------- */}
          {tab === 'perms' && catalog.data && (
            <PermMatrix forms={catalog.data.forms} perms={perms} setPerms={setPerms} readOnly={ro}
              tools={!ro && <>
                <button type="button" className="btn secondary sm" onClick={fullControl}><ShieldCheck size={15} /> حقوق كاملة</button>
                <button type="button" className="btn secondary sm" onClick={copyFrom}><Copy size={15} /> نسخ من مستخدم</button>
                <button type="button" className="btn ghost sm" onClick={clearAll}><Eraser size={15} /> حذف الكل</button>
              </>} />
          )}

          {/* -------- Main menus -------- */}
          {tab === 'menus' && catalog.data && (
            <>
              <p className="muted" style={{ marginTop: 0 }}>القوائم الرئيسية التي تظهر للمستخدم في الشريط الجانبي (مثل تبويبات البرنامج السابق).</p>
              <div className="menu-cards">
                {catalog.data.menus.map(mn => {
                  const on = !!menus[mn.guid]
                  const Icon = MENU.find(g => g.menus.includes(mn.unqName?.toLowerCase()))?.icon ?? LayoutGrid
                  return (
                    <button type="button" key={mn.guid} className={`menu-card ${on ? 'on' : ''}`} disabled={ro} onClick={() => setMenus(m => ({ ...m, [mn.guid]: !on }))}>
                      <span className="ic"><Icon size={19} /></span>{mn.menuName}<Check checked={on} onChange={() => {}} />
                    </button>
                  )
                })}
              </div>
            </>
          )}

          {/* -------- Report branches -------- */}
          {tab === 'branches' && (
            <>
              <p className="muted" style={{ marginTop: 0 }}>الفروع التي يمكن للمستخدم الاطلاع على تقاريرها{!chkBranch && ' (فعّل "فروع مخصصة للتقارير" من البيانات الأساسية)'}.</p>
              <div className="chips">
                {branches.data?.filter(b => b.isUse || userBranches.includes(b.guid)).map(b => {
                  const on = userBranches.includes(b.guid)
                  return (
                    <button type="button" key={b.guid} className={`chip ${on ? 'on' : ''}`} disabled={ro || !chkBranch}
                      onClick={() => setUserBranches(x => (on ? x.filter(g => g !== b.guid) : [...x, b.guid]))}>
                      <span className="tick">{on ? '✓' : ''}</span>{b.name}
                    </button>
                  )
                })}
              </div>
            </>
          )}
        </>
      )}
    </Drawer>
  )
}

/* ============================== HR list select (+ quick add) ============================== */
function HrSelect({ items, value, onChange, disabled, screen, form, noun, onAdded, extra }: {
  items: HrItem[] | undefined; value: string; onChange: (v: string) => void; disabled: boolean
  /** screens engine key and its permission key */
  screen: string; form: string; noun: string; onAdded: () => Promise<void>; extra?: Record<string, unknown>
}) {
  const { can } = useAuth()
  const [busy, setBusy] = useState(false)
  const options = (items ?? []).filter(i => i.isUse || i.guid === value)
  const add = async () => {
    const { default: Swal } = await import('sweetalert2')
    const r = await Swal.fire({
      title: `إضافة ${noun}`, input: 'text', inputPlaceholder: 'الاسم', showCancelButton: true, confirmButtonText: 'إضافة', cancelButtonText: 'إلغاء', buttonsStyling: false,
      customClass: { popup: 'sm-popup', title: 'sm-title', confirmButton: 'btn', cancelButton: 'btn ghost', actions: 'sm-actions', input: 'input' },
      inputValidator: v => (!v.trim() ? 'يرجى إدخال الاسم' : v.trim().length > 150 ? 'الاسم طويل جداً' : (items ?? []).some(i => i.name === v.trim()) ? 'الاسم موجود مسبقاً في القائمة' : undefined),
    })
    if (!r.isConfirmed || !r.value) return
    setBusy(true)
    try {
      const saved = await api<{ guid: string }>(`/screens/${screen}`, { method: 'POST', json: { values: { Name: String(r.value).trim(), Notes: null, IsUse: true, ...extra } } })
      await onAdded()
      onChange(saved.guid)
      notify.success(`تمت إضافة ${noun}`)
    } catch (e) { alerts.error((e as Error).message) } finally { setBusy(false) }
  }
  return (
    <div className="hr-select">
      <select className="input" disabled={disabled} value={value} onChange={e => onChange(e.target.value)}>
        <option value="">اختر...</option>
        {options.map(i => <option key={i.guid} value={i.guid}>{i.name}{i.isUse ? '' : ' (موقوف)'}</option>)}
      </select>
      {!disabled && can(form, 'add') && (
        <button type="button" className="btn secondary icon" onClick={add} disabled={busy} title={`إضافة ${noun} جديد`} aria-label={`إضافة ${noun}`}>{busy ? <Spinner /> : <Plus size={17} />}</button>
      )}
    </div>
  )
}

/* ============================== Permission matrix ============================== */
function PermMatrix({ forms, perms, setPerms, readOnly, tools }: {
  forms: FormItem[]; perms: Perms; setPerms: (fn: (p: Perms) => Perms) => void; readOnly: boolean; tools: React.ReactNode
}) {
  const [q, setQ] = useState('')
  const groups = useMemo(() => groupForms(forms), [forms])
  const visible = useMemo(() => groups
    .map(g => ({ ...g, items: g.items.filter(f => !q || f.name?.includes(q) || f.unqName?.toLowerCase().includes(q.toLowerCase())) }))
    .filter(g => g.items.length), [groups, q])
  const visibleForms = visible.flatMap(g => g.items)

  const setCell = (fg: string, k: PermKey, v: boolean) => setPerms(p => {
    const row = { ...(p[fg] ?? NONE), [k]: v }
    // Any right implies the screen can be opened; removing "open" removes everything.
    if (v && k !== 'view') row.view = true
    if (!v && k === 'view') return { ...p, [fg]: { ...NONE } }
    return { ...p, [fg]: row }
  })
  const setRow = (fg: string, v: boolean) => setPerms(p => ({ ...p, [fg]: v ? { ...ALL } : { ...NONE } }))
  const setMany = (items: FormItem[], k: PermKey | 'all', v: boolean) => setPerms(p => {
    const next = { ...p }
    for (const f of items) {
      const row = { ...(next[f.guid] ?? NONE) }
      if (k === 'all') next[f.guid] = v ? { ...ALL } : { ...NONE }
      else if (!v && k === 'view') next[f.guid] = { ...NONE }
      else { row[k] = v; if (v) row.view = true; next[f.guid] = row }
    }
    return next
  })
  const state = (items: FormItem[], k: PermKey | 'all') => {
    const n = items.filter(f => (k === 'all' ? PERM_COLS.every(c => perms[f.guid]?.[c.key]) : perms[f.guid]?.[k])).length
    return { all: n === items.length && n > 0, some: n > 0 && n < items.length }
  }

  return (
    <>
      <div className="perm-tools">
        <div className="input-wrap"><Search size={16} /><input className="input" placeholder="ابحث عن شاشة..." value={q} onChange={e => setQ(e.target.value)} /></div>
        {tools}
      </div>
      <div className="perm-legend">
        <span><b>فتح</b> ظهور الشاشة في القائمة</span><span><b>استعراض</b> البحث وعرض المستندات المحفوظة</span>
        <span><b>إضافة</b> حفظ مستند جديد</span><span><b>تعديل</b> تعديل أو إلغاء مستند</span><span><b>حذف</b> حذف البيانات</span><span><b>طباعة</b> طباعة المستندات</span>
      </div>
      <div className="perm-table">
        <div className="perm-row head">
          <span style={{ alignItems: 'flex-start' }}>الشاشة</span>
          {PERM_COLS.map(c => { const s = state(visibleForms, c.key); return <span key={c.key}>{c.label}<Check checked={s.all} indeterminate={s.some} disabled={readOnly} label={`${c.label} للكل`} onChange={v => setMany(visibleForms, c.key, v)} /></span> })}
          <span>الكل{(() => { const s = state(visibleForms, 'all'); return <Check checked={s.all} indeterminate={s.some} disabled={readOnly} label="كل الصلاحيات" onChange={v => setMany(visibleForms, 'all', v)} /> })()}</span>
        </div>
        {visible.map(g => {
          const Icon = g.icon
          return (
            <div key={g.key}>
              <div className="perm-row group">
                <span className="gname"><Icon size={16} /> {g.title} <small className="muted">({g.items.length})</small></span>
                {PERM_COLS.map(c => { const s = state(g.items, c.key); return <Check key={c.key} checked={s.all} indeterminate={s.some} disabled={readOnly} label={`${c.label}: ${g.title}`} onChange={v => setMany(g.items, c.key, v)} /> })}
                {(() => { const s = state(g.items, 'all'); return <Check checked={s.all} indeterminate={s.some} disabled={readOnly} label={`الكل: ${g.title}`} onChange={v => setMany(g.items, 'all', v)} /> })()}
              </div>
              {g.items.map(f => {
                const r = perms[f.guid] ?? NONE
                const all = PERM_COLS.every(c => r[c.key])
                return (
                  <div key={f.guid} className={`perm-row pitem ${anyOn(r) ? '' : 'off'}`}>
                    <span className="fname">{f.name}<small>{f.unqName}</small></span>
                    {PERM_COLS.map(c => <Check key={c.key} checked={r[c.key]} disabled={readOnly} label={`${c.label}: ${f.name}`} onChange={v => setCell(f.guid, c.key, v)} />)}
                    <Check checked={all} indeterminate={!all && anyOn(r)} disabled={readOnly} label={`الكل: ${f.name}`} onChange={v => setRow(f.guid, v)} />
                  </div>
                )
              })}
            </div>
          )
        })}
        {visible.length === 0 && <div className="empty"><b>لا توجد شاشات مطابقة</b></div>}
      </div>
    </>
  )
}
