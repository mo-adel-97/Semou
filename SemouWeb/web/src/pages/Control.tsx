import { useEffect, useMemo, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { BadgeCheck, BadgePercent, CheckCircle2, ClipboardList, Hourglass, ListChecks, Plus, Printer, Save, Search, Tag, Trash2, Users, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { printDiscount, printRegistration } from '../lib/print'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch } from '../components/ui'
import { ReportGrid, type RCol } from '../components/ReportGrid'
import { DiscountForm, type DiscountTarget } from './DiscountForm'

const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const day = (v: string) => new Date(v).toLocaleDateString('ar-EG', { year: 'numeric', month: 'short', day: 'numeric' })
const KIND = ['دبلوم', 'دورة', 'رسوم']

/* ======================= قائمة الأسعار ======================= */
interface PLRow { Guid: string; Code: number; ListName: string; Notes: string | null; IsUse: boolean; ChkOfferPrice: boolean; Items: number; Books: number }
interface PItem { guid: string; code: string; name: string; diplomType: number; taxRate: number }
interface PLine { diplomGuid: string; code: string; name: string; diplomType: number; taxRate: number; cost: string; total: string }
const r3 = (n: number) => Math.round(n * 1000) / 1000

export function PriceLists({ group }: { group: string }) {
  const { can } = useAuth()
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const list = useQuery({ queryKey: ['price-lists'], queryFn: () => api<PLRow[]>('/control/price-lists'), enabled: can('addpricelist', 'find') })
  const rows = list.data ?? []
  const columns: GridColumn<PLRow>[] = [
    { accessorKey: 'Code', header: 'الكود', meta: { width: 70 }, cell: x => <span className="code-chip">{x.getValue() as number}</span> },
    { accessorKey: 'ListName', header: 'اسم القائمة', cell: x => <b>{x.getValue() as string}</b> },
    { accessorKey: 'Items', header: 'عدد البنود', enableGlobalFilter: false, cell: x => <span className="num-strong">{x.getValue() as number}</span> },
    { accessorKey: 'Books', header: 'الدفاتر المرتبطة', enableGlobalFilter: false, cell: x => <span className="num-strong">{x.getValue() as number}</span> },
    { accessorKey: 'ChkOfferPrice', header: 'عرض الأسعار', enableGlobalFilter: false, cell: x => x.getValue() ? <span className="badge ok">مخصصة لعرض الأسعار</span> : '—' },
    { accessorKey: 'Notes', header: 'ملاحظات', cell: x => (x.getValue() as string) || '—' },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} /> },
  ]
  return (
    <>
      <PageHead title="قائمة الأسعار" subtitle="أسعار الدبلومات والدورات والرسوم والضريبة المطبقة عليها" group={group}
        actions={can('addpricelist', 'add') && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> قائمة جديدة</button>} />
      <div className="stats">
        <Stat icon={<Tag size={20} />} value={rows.length} label="قوائم الأسعار" />
        <Stat icon={<CheckCircle2 size={20} />} value={rows.filter(r => r.IsUse).length} label="قوائم فعالة" delay={0.05} />
        <Stat icon={<ListChecks size={20} />} value={rows.reduce((s, r) => s + r.Items, 0)} label="إجمالي البنود" delay={0.1} />
      </div>
      {!can('addpricelist', 'find') ? <div className="alert error">لا تملك صلاحية استعراض البيانات</div>
        : <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} onRowClick={r => setEditing({ guid: r.Guid })} searchPlaceholder="ابحث باسم القائمة..." />}
      <AnimatePresence>{editing && <PriceListCard key={editing.guid ?? 'new'} guid={editing.guid} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

function PriceListCard({ guid, onClose }: { guid: string | null; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const ro = guid ? !can('addpricelist', 'edit') : !can('addpricelist', 'add')
  const [name, setName] = useState('')
  const [notes, setNotes] = useState('')
  const [isUse, setIsUse] = useState(true)
  const [offer, setOffer] = useState(false)
  const [lines, setLines] = useState<PLine[]>([])
  const [q, setQ] = useState('')
  const [kind, setKind] = useState<number | null>(null)
  const [dirty, setDirty] = useState(false)
  const items = useQuery({ queryKey: ['price-items'], queryFn: () => api<PItem[]>('/control/price-items'), staleTime: 5 * 60_000 })
  const details = useQuery({ queryKey: ['price-lists', guid], enabled: !!guid, staleTime: 0,
    queryFn: () => api<{ head: { ListName: string; Notes: string | null; IsUse: boolean; ChkOfferPrice: boolean; Code: number }; lines: { diplomGuid: string; code: string; name: string; diplomType: number; taxRate: number; cost: number; total: number }[] }>(`/control/price-lists/${guid}`) })
  useEffect(() => {
    const d = details.data
    if (!d) return
    setName(d.head.ListName); setNotes(d.head.Notes ?? ''); setIsUse(d.head.IsUse); setOffer(d.head.ChkOfferPrice)
    setLines(d.lines.map(l => ({ ...l, cost: String(l.cost), total: String(l.total) })))
  }, [details.data])
  const touch = () => setDirty(true)
  const available = useMemo(() => (items.data ?? []).filter(i => !lines.some(l => l.diplomGuid === i.guid) && (kind == null || i.diplomType === kind) && (!q || i.name.includes(q) || i.code.includes(q))), [items.data, lines, q, kind])
  const add = (i: PItem) => { setLines(l => [...l, { diplomGuid: i.guid, code: i.code, name: i.name, diplomType: i.diplomType, taxRate: i.taxRate, cost: '0', total: '0' }]); touch() }
  // DiplomGrid_CellEndEdit: editing the price before VAT updates the total and vice-versa.
  const edit = (g: string, field: 'cost' | 'total', v: string) => {
    v = v.replace(/[^\d.]/g, '')
    setLines(ls => ls.map(l => {
      if (l.diplomGuid !== g) return l
      const n = Number(v) || 0, r = l.taxRate / 100
      return field === 'cost' ? { ...l, cost: v, total: String(r3(n + n * r)) } : { ...l, total: v, cost: String(r ? r3(n / (1 + r)) : n) }
    })); touch()
  }
  const save = useMutation({
    mutationFn: (body: object) => guid ? api<{ message: string }>(`/control/price-lists/${guid}`, { method: 'PUT', json: body }) : api<{ message: string }>('/control/price-lists', { method: 'POST', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['price-lists'] }); notify.success(r.message); onClose() },
  })
  const submit = async () => {
    if (!name.trim()) return alerts.error('يرجى إدخال اسم قائمة الأسعار')
    if (!lines.length) return alerts.error('يرجى إضافة الدبلومات / الدورات إلى القائمة')
    let reason: string | null = null
    if (guid) { reason = await alerts.reason(); if (!reason) return }
    save.mutate({ listName: name, notes, isUse, chkOfferPrice: offer, reason, lines: lines.map(l => ({ diplomGuid: l.diplomGuid, cost: Number(l.cost) || 0, total: Number(l.total) || 0 })) })
  }
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }
  return (
    <Drawer size="lg" onClose={requestClose} icon={<Tag size={22} />} title={guid ? `قائمة الأسعار: ${details.data?.head.ListName ?? ''}` : 'قائمة أسعار جديدة'}
      subtitle="اضغط على أي بند من القائمة اليمنى لإضافته، ثم أدخل السعر قبل الضريبة أو شاملاً لها"
      footer={<>
        {!ro && <button className="btn" onClick={submit} disabled={save.isPending}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>}</button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق</button>
        <span className="hint">{ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : `${lines.length} بند`}</span>
      </>}>
      {guid && details.isPending ? <div className="center-fill" style={{ minHeight: 280 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 16 }}>
          <div className="grid cols-2">
            <Field label="اسم القائمة" required><input className="input" maxLength={100} value={name} disabled={ro} onChange={e => { setName(e.target.value); touch() }} /></Field>
            <Field label="ملاحظات"><input className="input" maxLength={250} value={notes} disabled={ro} onChange={e => { setNotes(e.target.value); touch() }} /></Field>
            <Switch label={isUse ? 'القائمة فعالة' : 'القائمة موقوفة'} checked={isUse} disabled={ro} onChange={v => { setIsUse(v); touch() }} />
            <Switch label="قائمة مخصصة لعرض الأسعار" checked={offer} disabled={ro} onChange={v => { setOffer(v); touch() }} />
          </div>
          <div className="pl-split">
            {!ro && (
              <div className="pl-pool">
                <div className="input-wrap"><Search size={15} /><input className="input" placeholder="ابحث عن بند..." value={q} onChange={e => setQ(e.target.value)} /></div>
                <Segmented value={kind} onChange={setKind} options={[{ value: null, label: 'الكل' }, { value: 0, label: 'دبلومات' }, { value: 1, label: 'دورات' }, { value: 2, label: 'رسوم' }]} />
                <div className="pl-items">
                  {items.isPending ? <Spinner /> : available.length === 0 ? <div className="empty"><b>لا توجد بنود أخرى</b></div> : available.map(i => (
                    <button key={i.guid} type="button" className="pl-item" onClick={() => add(i)}>
                      <Plus size={14} /><span><b>{i.name}</b><small>كود {i.code} • {KIND[i.diplomType] ?? ''}{i.taxRate ? ` • ضريبة ${i.taxRate}%` : ''}</small></span>
                    </button>
                  ))}
                </div>
              </div>
            )}
            <div className="card" style={{ overflow: 'auto', flex: 2 }}>
              <table className="dt ledger">
                <thead><tr><th>البند</th><th className="n">السعر قبل الضريبة</th><th className="n">الضريبة</th><th className="n">الإجمالي</th><th /></tr></thead>
                <tbody>
                  {lines.length === 0 && <tr><td colSpan={5}><div className="empty"><b>لم تتم إضافة بنود بعد</b></div></td></tr>}
                  {lines.map(l => (
                    <tr key={l.diplomGuid}>
                      <td><b>{l.name}</b><div className="muted small">{KIND[l.diplomType] ?? ''} • كود {l.code}{l.taxRate ? ` • ${l.taxRate}%` : ' • بدون ضريبة'}</div></td>
                      <td className="n"><input className="input price" dir="ltr" inputMode="decimal" style={{ width: 120 }} disabled={ro} value={l.cost} onChange={e => edit(l.diplomGuid, 'cost', e.target.value)} /></td>
                      <td className="n">{money(r3((Number(l.total) || 0) - (Number(l.cost) || 0)))}</td>
                      <td className="n"><input className="input price" dir="ltr" inputMode="decimal" style={{ width: 120 }} disabled={ro} value={l.total} onChange={e => edit(l.diplomGuid, 'total', e.target.value)} /></td>
                      <td>{!ro && <button className="btn ghost sm icon" title="حذف البند" onClick={() => { setLines(ls => ls.filter(x => x.diplomGuid !== l.diplomGuid)); touch() }}><Trash2 size={15} /></button>}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </Drawer>
  )
}

/* ======================= ضبط أعداد الدفعات ======================= */
interface SeatLine { diplomGuid: string; code: string; name: string; count: number; registered: number }

export function BatchSeats({ group }: { group: string }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [branch, setBranch] = useState('')
  const [batch, setBatch] = useState('')
  const [counts, setCounts] = useState<Record<string, string>>({})
  const batches = useQuery({ queryKey: ['batch-list', branch], queryFn: () => api<{ guid: string; name: string; code: string; capacity: number }[]>(`/control/batches${qs({ branch: branch || null })}`), placeholderData: keepPreviousData })
  const seats = useQuery({ queryKey: ['batch-seats', branch, batch], enabled: !!branch && !!batch, staleTime: 0,
    queryFn: () => api<{ capacity: number; lines: SeatLine[] }>(`/control/batch-seats${qs({ branch, batch })}`) })
  useEffect(() => { if (seats.data) setCounts(Object.fromEntries(seats.data.lines.map(l => [l.diplomGuid, String(l.count)]))) }, [seats.data])
  const capacity = seats.data?.capacity ?? 0
  const used = Object.values(counts).reduce((s, v) => s + (Number(v) || 0), 0)
  const left = capacity - used
  const save = useMutation({
    mutationFn: (body: object) => api<{ message: string }>('/control/batch-seats', { method: 'PUT', json: body }),
    onSuccess: r => { notify.success(r.message); qc.invalidateQueries({ queryKey: ['batch-seats'] }) },
  })
  const submit = async () => {
    if (left < 0) return alerts.error(`مجموع المقاعد الموزعة (${used}) أكبر من عدد مقاعد الدفعة في الفرع (${capacity})`)
    const reason = await alerts.reason('سبب تعديل توزيع المقاعد')
    if (!reason) return
    save.mutate({ branchGuid: branch, batchGuid: batch, reason, lines: Object.entries(counts).map(([diplomGuid, v]) => ({ diplomGuid, count: Number(v) || 0 })) })
  }
  const canEdit = can('addbatchcount', 'edit')
  return (
    <>
      <PageHead title="ضبط أعداد الدفعات" subtitle="توزيع مقاعد كل دفعة في الفرع على الدبلومات. الدبلوم الذي ليس له مقاعد لا يمكن التسجيل فيه." group={group}
        actions={branch && batch && canEdit && <button className="btn" onClick={submit} disabled={save.isPending || seats.isPending}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ التوزيع</>}</button>} />
      <div className="card" style={{ padding: 16 }}>
        <div className="grid cols-2">
          <Field label="الفرع" required>
            <select className="input" value={branch} onChange={e => { setBranch(e.target.value); setBatch('') }}>
              <option value="">— اختر —</option>
              {branches.data?.filter(b => b.isUse).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
            </select>
          </Field>
          <Field label="الدفعة" required hint={branch ? undefined : 'اختر الفرع أولاً'}>
            <select className="input" value={batch} disabled={!branch} onChange={e => setBatch(e.target.value)}>
              <option value="">— اختر —</option>
              {batches.data?.map(b => <option key={b.guid} value={b.guid}>{b.name} {b.capacity ? `(${b.capacity} مقعد)` : '(لم تحدد مقاعدها)'}</option>)}
            </select>
          </Field>
        </div>
      </div>
      {branch && batch && (seats.isPending ? <div className="center-fill" style={{ minHeight: 200 }}><Spinner size={28} /></div> : <>
        <div className="stats" style={{ marginTop: 16 }}>
          <Stat icon={<Users size={20} />} value={capacity} label="مقاعد الدفعة في الفرع" />
          <Stat icon={<ClipboardList size={20} />} value={used} label="الموزعة على الدبلومات" delay={0.05} />
          <Stat icon={left < 0 ? <XCircle size={20} /> : <CheckCircle2 size={20} />} value={left} label="غير موزعة" tone={left < 0 ? 'danger' : undefined} delay={0.1} />
        </div>
        {capacity === 0 && <div className="alert warn">لم يتم تحديد عدد مقاعد هذه الدفعة في الفرع. يمكن تحديده من شاشة "الدفعات".</div>}
        <div className="card" style={{ overflow: 'auto', marginTop: 12 }}>
          <table className="dt ledger">
            <thead><tr><th>الدبلوم</th><th className="n">المسجلون</th><th className="n">عدد المقاعد</th><th>الإشغال</th></tr></thead>
            <tbody>
              {seats.data?.lines.map(l => {
                const n = Number(counts[l.diplomGuid]) || 0
                const pct = n ? Math.min(100, (l.registered / n) * 100) : l.registered ? 100 : 0
                return (
                  <tr key={l.diplomGuid}>
                    <td><b>{l.name}</b><div className="muted small">كود {l.code}</div></td>
                    <td className="n">{l.registered}</td>
                    <td className="n"><input className="input price" dir="ltr" inputMode="numeric" style={{ width: 100 }} disabled={!canEdit} value={counts[l.diplomGuid] ?? '0'}
                      onChange={e => setCounts(c => ({ ...c, [l.diplomGuid]: e.target.value.replace(/\D/g, '') }))} /></td>
                    <td style={{ minWidth: 160 }}>
                      <span className="seat-bar" style={{ display: 'block' }}><i style={{ width: `${pct}%`, background: l.registered > n ? 'var(--danger)' : undefined }} /></span>
                      <small className="muted">{n ? `${Math.max(0, n - l.registered)} متاح` : 'غير متاح للتسجيل'}</small>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </>)}
    </>
  )
}

/* ======================= قائمة الطلاب الجدد ======================= */
interface NewRow { regDocGuid: string; code: string; regDate: string; studentName: string; tel: string; nationalId: string; batchName: string | null; levelName: string | null
  courseName: string | null; sellerName: string | null; diploms: { diplomGuid: string; diplomName: string }[] }
const esc = (t: string) => t.replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))

export function NewStudents({ group }: { group: string }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const [branch, setBranch] = useState('')
  const [regType, setRegType] = useState<0 | 1>(0)
  const [name, setName] = useState('')
  useEffect(() => { if (!branch && branches.data?.length) setBranch(branches.data.find(b => b.isUse)?.guid ?? '') }, [branches.data, branch])
  const list = useQuery({ queryKey: ['new-students', branch, regType, name], enabled: !!branch, placeholderData: keepPreviousData,
    queryFn: () => api<NewRow[]>(`/control/new-students${qs({ branch, regType, name: name.trim() || null })}`) })
  const accept = useMutation({
    mutationFn: ({ guid, body }: { guid: string; body: object }) => api<{ message: string }>(`/reception/regdocs/${guid}/accept`, { method: 'POST', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['new-students'] }); qc.invalidateQueries({ queryKey: ['reception'] }); alerts.success('تم قبول التسجيل', r.message) },
  })
  const onAccept = async (r: NewRow) => {
    if (!can('newstudenthome', 'add')) { alerts.error('لا تملك صلاحية قبول التسجيل'); return }
    const multi = r.diploms.length > 1
    const v = await alerts.form<{ diplomGuid: string | null; attachNotes: string; contractNotes: string }>({
      title: `قبول تسجيل ${r.studentName}`, confirmText: 'قبول وفتح الملف التدريبي',
      html: `<div class="sm-form">
        ${multi ? `<label class="sm-lbl">الاستمارة تحتوي على أكثر من دبلوم، اختر الدبلوم الذي سيدرسه الطالب</label>
          <select id="acc-dip" class="input"><option value="">— اختر —</option>${r.diploms.map(d => `<option value="${d.diplomGuid}">${esc(d.diplomName)}</option>`).join('')}</select>` : ''}
        <label class="sm-chk"><input type="checkbox" id="acc-att"> مستندات التسجيل مكتملة ومراجَعة</label>
        <input id="acc-att-n" class="input" maxlength="250" placeholder="ملاحظات المستندات (اختياري)">
        <label class="sm-chk"><input type="checkbox" id="acc-con"> تمت مراجعة اتفاقية التدريب</label>
        <input id="acc-con-n" class="input" maxlength="250" placeholder="ملاحظات الاتفاقية (اختياري)">
      </div>`,
      read: el => {
        const val = (id: string) => el.querySelector(`#${id}`) as HTMLInputElement | null
        if (multi && !val('acc-dip')?.value) return 'يرجى اختيار الدبلوم'
        if (!val('acc-att')?.checked) return 'يرجى تحديد حالة مستندات التسجيل'
        if (!val('acc-con')?.checked) return 'يرجى تحديد حالة اتفاقية التدريب'
        return { diplomGuid: multi ? val('acc-dip')!.value : null, attachNotes: val('acc-att-n')!.value, contractNotes: val('acc-con-n')!.value }
      },
    })
    if (v) accept.mutate({ guid: r.regDocGuid, body: { diplomGuid: v.diplomGuid, attachOk: true, attachNotes: v.attachNotes, contractOk: true, contractNotes: v.contractNotes } })
  }
  const rows = list.data ?? []
  type R = NewRow
  const program = (r: R) => r.diploms.map(d => d.diplomName).join(' + ')
  const cols: RCol<R>[] = [
    { key: 'code', label: 'الاستمارة', width: 90, render: r => <span className="code-chip">#{r.code}</span> },
    { key: 'regDate', label: 'التاريخ', width: 100, value: r => new Date(r.regDate).toLocaleDateString('en-GB') },
    { key: 'studentName', label: 'اسم الطالب', width: 230, render: r => <b>{r.studentName}</b> },
    { key: 'nationalId', label: 'رقم الهوية', width: 120 },
    { key: 'tel', label: 'الجوال', width: 120 },
    { key: 'program', label: regType === 1 ? 'الدورة' : 'الدبلوم', width: 280, value: program,
      render: r => <span>{program(r)}{r.diploms.length > 1 && regType === 0 && <span className="badge warn" style={{ marginInlineStart: 6 }}>أكثر من دبلوم</span>}</span> },
    ...(regType === 1
      ? [{ key: 'courseName', label: 'كود الدورة', width: 140 } as RCol<R>]
      : [{ key: 'batchName', label: 'الدفعة', width: 120 } as RCol<R>, { key: 'levelName', label: 'المستوى', width: 110 } as RCol<R>]),
    { key: 'sellerName', label: 'المندوب', width: 150 },
    { key: 'actions', label: 'الإجراءات', width: 230, foot: false, value: () => '',
      render: r => <span className="rg-actions">
        <button className="btn ghost sm" onClick={e => { e.stopPropagation(); printRegistration(r.regDocGuid) }}><Printer size={14} /> طباعة</button>
        {can('newstudenthome', 'add') && <button className="btn sm" disabled={accept.isPending} onClick={e => { e.stopPropagation(); onAccept(r) }}><BadgeCheck size={14} /> قبول التسجيل</button>}
      </span> },
  ]
  return (
    <>
      <PageHead title="قائمة الطلاب الجدد" subtitle="استمارات التسجيل التي تنتظر قبول المشرف لفتح الملف التدريبي للطالب" group={group} />
      <div className="rb">
        <label className="rb-f" style={{ minWidth: 260 }}><small>الفرع</small><select className="input" value={branch} onChange={e => setBranch(e.target.value)}>
          {branches.data?.filter(b => b.isUse).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
        </select></label>
        <label className="rb-f"><small>نوع التسجيل</small><Segmented value={regType} onChange={setRegType} options={[{ value: 0, label: 'دبلومات' }, { value: 1, label: 'دورات' }]} /></label>
        <label className="rb-f" style={{ minWidth: 240 }}><small>اسم الطالب</small><input className="input" placeholder="ابحث باسم الطالب..." value={name} onChange={e => setName(e.target.value)} /></label>
        <span className="badge warn rb-run" style={{ fontSize: 14, padding: '8px 14px', minWidth: 0 }}><Hourglass size={14} /> {rows.length} بانتظار القبول</span>
      </div>
      {list.isPending ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div> : (
        <ReportGrid columns={cols} rows={rows} rowKey={r => r.regDocGuid} footLabel={`${rows.length} استمارة`} emptyText="لا توجد استمارات بانتظار القبول في هذا الفرع"
          toolbar={<button className="btn ghost sm" onClick={() => list.refetch()}>تحديث</button>} />
      )}
    </>
  )
}

/* ======================= سندات الخصم ======================= */
interface DiscRow { Guid: string; Code: string; Date: string; StudentGuid: string; StudentName: string; NationalId: string; TypeName: string | null; BranchName: string | null; Amount: number; Notes: string | null; IsUse: boolean }

export function DiscountDocs({ group }: { group: string }) {
  const { can } = useAuth()
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [open, setOpen] = useState<DiscountTarget | null>(null)
  const list = useQuery({ queryKey: ['discount', 'list', from, to], enabled: can('discount', 'find'), placeholderData: keepPreviousData,
    queryFn: () => api<DiscRow[]>(`/control/discounts${qs({ from: from || null, to: to || null })}`) })
  const rows = list.data ?? []
  const active = rows.filter(r => r.IsUse)
  const columns: GridColumn<DiscRow>[] = [
    { accessorKey: 'Code', header: 'رقم السند', meta: { width: 90 }, cell: x => <span className="code-chip">{x.getValue() as string}</span> },
    { accessorKey: 'Date', header: 'التاريخ', enableGlobalFilter: false, cell: x => day(x.getValue() as string) },
    { accessorKey: 'StudentName', header: 'الطالب', cell: x => <b>{x.getValue() as string}</b> },
    { accessorKey: 'NationalId', header: 'رقم الهوية', cell: x => <span dir="ltr">{x.getValue() as string}</span> },
    { accessorKey: 'TypeName', header: 'نوع الخصم' },
    { accessorKey: 'BranchName', header: 'الفرع' },
    { accessorKey: 'Amount', header: 'القيمة', enableGlobalFilter: false, cell: x => <b className="num-strong">{money(x.getValue() as number)}</b> },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} offText="ملغى" /> },
    { id: 'print', header: '', enableGlobalFilter: false, cell: x => <button className="btn ghost sm icon" title="طباعة" onClick={e => { e.stopPropagation(); printDiscount(x.row.original.Guid) }}><Printer size={15} /></button> },
  ]
  const newDoc = async () => {
    const nid = await alerts.form<{ nid: string }>({
      title: 'سند خصم جديد', confirmText: 'متابعة',
      html: '<div class="sm-form"><label class="sm-lbl">رقم هوية الطالب</label><input id="nid" class="input" dir="ltr" maxlength="10" inputmode="numeric" placeholder="10 أرقام"></div>',
      read: el => { const v = (el.querySelector('#nid') as HTMLInputElement).value.trim(); return /^\d{10}$/.test(v) ? { nid: v } : 'يرجى إدخال رقم هوية صحيح من 10 أرقام' },
    })
    if (!nid) return
    try {
      const r = await api<{ StudentGuid: string; StudentName: string; NationalId: string }[]>(`/reception/search${qs({ q: nid.nid, by: 'nid' })}`)
      if (!r.length) { alerts.error('لا يوجد طالب مسجل بهذا الرقم'); return }
      setOpen({ studentGuid: r[0].StudentGuid, studentName: r[0].StudentName, nationalId: r[0].NationalId, guid: null })
    } catch (e) { alerts.error((e as Error).message) }
  }
  return (
    <>
      <PageHead title="سندات الخصم" subtitle="الخصومات الممنوحة للطلاب وقيودها المحاسبية" group={group}
        actions={can('discount', 'add') && <button className="btn" onClick={newDoc}><Plus size={17} /> سند خصم جديد</button>} />
      <div className="stats">
        <Stat icon={<BadgePercent size={20} />} value={active.length} label="سندات فعالة" />
        <Stat icon={<Tag size={20} />} value={money(active.reduce((s, r) => s + r.Amount, 0))} label="إجمالي الخصومات (ر.س)" delay={0.05} />
        <Stat icon={<XCircle size={20} />} value={rows.length - active.length} label="سندات ملغاة" tone="danger" delay={0.1} />
      </div>
      {!can('discount', 'find') ? <div className="alert error">لا تملك صلاحية استعراض البيانات</div>
        : <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} searchPlaceholder="ابحث برقم السند أو اسم الطالب..."
            onRowClick={r => setOpen({ studentGuid: r.StudentGuid, studentName: r.StudentName, nationalId: r.NationalId, guid: r.Guid })}
            toolbar={<div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
              <span className="muted">من</span><input className="input" type="date" value={from} onChange={e => setFrom(e.target.value)} />
              <span className="muted">إلى</span><input className="input" type="date" value={to} onChange={e => setTo(e.target.value)} />
            </div>} />}
      <AnimatePresence>{open && <DiscountForm key={open.guid ?? 'new'} target={open} onClose={() => setOpen(null)} />}</AnimatePresence>
    </>
  )
}
