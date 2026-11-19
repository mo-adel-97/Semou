import { useEffect, useMemo, useState } from 'react'
import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { BookMarked, BookOpen, Calculator, CheckCircle2, Plus, Save, Search, Users as UsersIcon, X, XCircle } from 'lucide-react'
import { api, qs } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { useBranches } from '../lib/lookups'
import { DataTable, type GridColumn } from '../components/DataTable'
import { PageHead } from '../components/Shell'
import { AccountPicker } from '../components/AccountPicker'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Check, Drawer, Field, Segmented, Spinner, Stat, StatusBadge, Switch, Tabs } from '../components/ui'

/* إعدادات التسجيل / دفاتر المستندات (EncodingRegSetting). */
const FORM = 'regsetting'
interface Row { Guid: string; Code: number; DocName: string; TypeName: string; UnqName: string; BranchName: string | null; PriceListName: string | null; IsUse: boolean; ChkDaily: boolean; ChkVat: boolean; Users: number }
interface Lookups { types: { guid: string; name: string; unq: string }[]; priceLists: { guid: string; name: string; code: string }[]; users: { guid: string; name: string; userName: string }[]; payBooks: { guid: string; name: string; branchGuid: string }[] }
interface Pick { guid: string; label: string }
interface Form {
  docTypeGuid: string; docName: string; branchGuid: string; priceListGuid: string; isUse: boolean
  chkDaily: boolean; chkMaden: boolean; maden: Pick | null; chkDaen: boolean; daen: Pick | null; chkVat: boolean; vat: Pick | null; cost: Pick | null
  chkOtherFees: boolean; chkStartPay: boolean; chkAmountPay: boolean; amountPay: string; docPayGuid: string
  regType: number | null; invoiceType: number | null; fessType: number | null; users: string[]
}
const EMPTY: Form = {
  docTypeGuid: '', docName: '', branchGuid: '', priceListGuid: '', isUse: true, chkDaily: true, chkMaden: true, maden: null, chkDaen: false, daen: null, chkVat: false, vat: null, cost: null,
  chkOtherFees: false, chkStartPay: false, chkAmountPay: false, amountPay: '0', docPayGuid: '', regType: null, invoiceType: null, fessType: null, users: [],
}
const NO_ROWS: Row[] = []

export function DocBooks({ group }: { group: string }) {
  const { can } = useAuth()
  const [type, setType] = useState('')
  const [editing, setEditing] = useState<{ guid: string | null } | null>(null)
  const lookups = useQuery({ queryKey: ['doc-books', 'lookups'], queryFn: () => api<Lookups>('/doc-books/lookups'), staleTime: 5 * 60_000 })
  const list = useQuery({ queryKey: ['doc-books', 'list', type], queryFn: () => api<Row[]>(`/doc-books${qs({ type: type || null })}`), enabled: can(FORM, 'find'), placeholderData: keepPreviousData })
  const rows = list.data ?? NO_ROWS
  const columns: GridColumn<Row>[] = [
    { accessorKey: 'Code', header: 'الكود', meta: { width: 70 }, cell: x => <span className="code-chip">{x.getValue() as number}</span> },
    { accessorKey: 'DocName', header: 'اسم الدفتر', cell: x => <b>{x.getValue() as string}</b> },
    { accessorKey: 'TypeName', header: 'نوع المستند', cell: x => <span className="badge muted">{x.getValue() as string}</span> },
    { accessorKey: 'BranchName', header: 'الفرع' },
    { accessorKey: 'PriceListName', header: 'قائمة الأسعار', cell: x => (x.getValue() as string) ?? '—' },
    { accessorKey: 'ChkDaily', header: 'قيد محاسبي', enableGlobalFilter: false, cell: x => <span className={`badge ${x.getValue() ? 'ok' : 'muted'}`}>{x.getValue() ? 'نعم' : 'لا'}</span> },
    { accessorKey: 'ChkVat', header: 'الضريبة', enableGlobalFilter: false, cell: x => <span className={`badge ${x.getValue() ? 'ok' : 'muted'}`}>{x.getValue() ? 'شامل' : 'بدون'}</span> },
    { accessorKey: 'Users', header: 'المستخدمون', enableGlobalFilter: false, cell: x => <span className="num-strong">{x.getValue() as number}</span> },
    { accessorKey: 'IsUse', header: 'الحالة', enableGlobalFilter: false, cell: x => <StatusBadge on={x.getValue() as boolean} /> },
  ]
  const active = rows.filter(r => r.IsUse).length
  return (
    <>
      <PageHead title="إعدادات التسجيل" subtitle="دفاتر المستندات: الفرع وقائمة الأسعار والقيد المحاسبي والمستخدمون المسموح لهم بكل دفتر" group={group}
        actions={can(FORM, 'add') && <button className="btn" onClick={() => setEditing({ guid: null })}><Plus size={17} /> دفتر جديد</button>} />
      <div className="stats">
        <Stat icon={<BookMarked size={20} />} value={rows.length} label="عدد الدفاتر" />
        <Stat icon={<CheckCircle2 size={20} />} value={active} label="دفاتر فعالة" delay={0.05} />
        <Stat icon={<XCircle size={20} />} value={rows.length - active} label="دفاتر موقوفة" tone="danger" delay={0.1} />
      </div>
      {!can(FORM, 'find') ? <div className="alert error">لا تملك صلاحية استعراض البيانات</div> : (
        <DataTable data={rows} columns={columns} loading={list.isPending} getRowId={r => r.Guid} onRowClick={r => setEditing({ guid: r.Guid })} searchPlaceholder="ابحث باسم الدفتر أو الفرع..."
          toolbar={<select className="input" style={{ width: 'auto', minWidth: 200 }} value={type} onChange={e => setType(e.target.value)}>
            <option value="">كل أنواع المستندات</option>
            {lookups.data?.types.map(t => <option key={t.guid} value={t.guid}>{t.name}</option>)}
          </select>} />
      )}
      <AnimatePresence>{editing && lookups.data && <BookCard key={editing.guid ?? 'new'} guid={editing.guid} lookups={lookups.data} onClose={() => setEditing(null)} />}</AnimatePresence>
    </>
  )
}

function BookCard({ guid, lookups, onClose }: { guid: string | null; lookups: Lookups; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const branches = useBranches()
  const isNew = !guid
  const ro = isNew ? !can(FORM, 'add') : !can(FORM, 'edit')
  const [tab, setTab] = useState<'basic' | 'posting' | 'users'>('basic')
  const [f, setF] = useState<Form>(EMPTY)
  const [dirty, setDirty] = useState(false)
  const [picker, setPicker] = useState<'maden' | 'daen' | 'vat' | 'cost' | null>(null)
  const [uq, setUq] = useState('')
  const set = <K extends keyof Form>(k: K, v: Form[K]) => { setF(x => ({ ...x, [k]: v })); setDirty(true) }

  const details = useQuery({ queryKey: ['doc-books', 'one', guid], queryFn: () => api<{ head: Record<string, any>; users: string[] }>(`/doc-books/${guid}`), enabled: !!guid, staleTime: 0 })
  useEffect(() => {
    const d = details.data
    if (!d) return
    const h = d.head
    const pick = (g: string | null, name: string | null, code: string | null) => (g && g !== '00000000-0000-0000-0000-000000000000' ? { guid: g, label: `${code ?? ''} - ${name ?? ''}` } : null)
    setF({
      docTypeGuid: h.DocTypeGuid, docName: h.DocName ?? '', branchGuid: h.BranchGuid ?? '', priceListGuid: h.PriceListGuid ?? '', isUse: h.IsUse,
      chkDaily: h.ChkDaily, chkMaden: h.ChkMaden, maden: pick(h.MadenGuid, h.MadenName, h.MadenCode), chkDaen: h.ChkDaen, daen: pick(h.DaenGuid, h.DaenName, h.DaenCode),
      chkVat: h.ChkVat, vat: pick(h.VatAccountGuid, h.VatName, h.VatCode), cost: h.CostCenterGuid && h.CostCenterName ? { guid: h.CostCenterGuid, label: h.CostCenterName } : null,
      chkOtherFees: h.ChkOtherFees, chkStartPay: h.ChkStartPay, chkAmountPay: h.ChkAmountPay, amountPay: String(h.AmountPay ?? 0),
      docPayGuid: h.DocPayGuid && h.DocPayGuid !== '00000000-0000-0000-0000-000000000000' ? h.DocPayGuid : '',
      regType: h.UnqName === 'addregdoc' ? h.RegType : null, invoiceType: h.UnqName === 'salesinvoice' ? h.InvoiceType : null, fessType: h.UnqName === 'regfess' ? h.FessType : null,
      users: d.users,
    })
  }, [details.data])

  const unq = lookups.types.find(t => t.guid === f.docTypeGuid)?.unq ?? details.data?.head.UnqName
  const typeName = lookups.types.find(t => t.guid === f.docTypeGuid)?.name ?? details.data?.head.TypeName
  const payBooks = lookups.payBooks.filter(b => b.branchGuid === f.branchGuid)
  const users = useMemo(() => lookups.users.filter(u => !uq || u.name.includes(uq) || u.userName?.includes(uq)), [lookups.users, uq])

  const save = useMutation({
    mutationFn: (body: object) => guid ? api<{ message: string }>(`/doc-books/${guid}`, { method: 'PUT', json: body }) : api<{ message: string }>('/doc-books', { method: 'POST', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['doc-books'] }); setDirty(false); notify.success(r.message); onClose() },
  })
  const submit = async () => {
    if (!f.docTypeGuid) return alerts.error('يرجى اختيار نوع الدفتر')
    if (!f.docName.trim()) { setTab('basic'); return alerts.error('يرجى إدخال اسم الدفتر') }
    if (!f.branchGuid) { setTab('basic'); return alerts.error('يرجى اختيار الفرع') }
    if (!f.priceListGuid) { setTab('basic'); return alerts.error('يرجى اختيار قائمة الأسعار') }
    if (f.chkDaily) {
      if (!f.chkMaden && !f.maden) { setTab('posting'); return alerts.error('يرجى اختيار الجانب المدين') }
      if (!f.chkDaen && !f.daen) { setTab('posting'); return alerts.error('يرجى اختيار الجانب الدائن') }
      if (f.chkVat && !f.vat) { setTab('posting'); return alerts.error('يرجى اختيار حساب الضريبة') }
      if (!f.cost) { setTab('posting'); return alerts.error('يرجى اختيار مركز التكلفة') }
    }
    let reason: string | null = null
    if (guid) { reason = await alerts.reason(); if (!reason) return }
    save.mutate({
      docTypeGuid: f.docTypeGuid, docName: f.docName, branchGuid: f.branchGuid, priceListGuid: f.priceListGuid, isUse: f.isUse,
      chkDaily: f.chkDaily, chkMaden: f.chkMaden, madenGuid: f.maden?.guid ?? null, chkDaen: f.chkDaen, daenGuid: f.daen?.guid ?? null,
      chkVat: f.chkVat, vatAccountGuid: f.vat?.guid ?? null, costCenterGuid: f.cost?.guid ?? null,
      chkOtherFees: f.chkOtherFees, chkStartPay: f.chkStartPay, chkAmountPay: f.chkAmountPay, amountPay: Number(f.amountPay) || 0, docPayGuid: f.docPayGuid || null,
      regType: f.regType, invoiceType: f.invoiceType, fessType: f.fessType, users: f.users, reason,
    })
  }
  const requestClose = async () => {
    if (dirty && !save.isPending && !(await alerts.confirm({ title: 'توجد تعديلات غير محفوظة', text: 'ستُفقد التعديلات عند الخروج. هل أنت متأكد؟', confirmText: 'خروج بدون حفظ', cancelText: 'رجوع', danger: true }))) return
    onClose()
  }

  const AccountField = ({ label, chk, chkLabel, onChk, value, which }: { label: string; chk?: boolean; chkLabel?: string; onChk?: (v: boolean) => void; value: Pick | null; which: 'maden' | 'daen' | 'vat' }) => (
    <div className="acc-field">
      <div className="acc-head"><b>{label}</b>{onChk && <Switch label={chkLabel!} checked={!!chk} disabled={ro} onChange={onChk} />}</div>
      {chk ? <div className="acc-note">يُستخدم حساب الطالب نفسه في هذا الجانب</div> : (
        <button type="button" className="acc-pick" disabled={ro} onClick={() => setPicker(which)}>
          {value ? <span>{value.label}</span> : <span className="muted">اختر الحساب...</span>}<Search size={15} />
        </button>
      )}
    </div>
  )

  const loading = !!guid && details.isPending
  return (
    <Drawer size="lg" onClose={requestClose} icon={<BookMarked size={22} />} title={isNew ? 'دفتر جديد' : `دفتر: ${details.data?.head.DocName ?? ''}`}
      subtitle={isNew ? 'اختر نوع المستند ثم أكمل بيانات الدفتر' : `${typeName ?? ''} • كود ${details.data?.head.Code ?? ''}`}
      tabs={<Tabs value={tab} onChange={setTab} items={[
        { value: 'basic', label: 'البيانات الأساسية' },
        { value: 'posting', label: 'القيد المحاسبي' },
        { value: 'users', label: 'المستخدمون', badge: f.users.length },
      ]} />}
      footer={<>
        {!ro && <button className="btn" onClick={submit} disabled={save.isPending || loading}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>}</button>}
        <button className="btn ghost" onClick={requestClose}>إغلاق</button>
        <span className="hint">{ro ? 'عرض فقط' : dirty ? <><span className="dirty-dot" /> تعديلات غير محفوظة</> : null}</span>
      </>}>
      {loading ? <div className="center-fill" style={{ minHeight: 280 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 18 }}>
          {tab === 'basic' && <>
            <Field label="نوع المستند" required hint={isNew ? undefined : 'لا يمكن تغيير نوع المستند بعد الحفظ'}>
              <div className="pick-grid">
                {lookups.types.map(t => (
                  <button key={t.guid} type="button" disabled={!isNew || ro} className={`pick-card ${f.docTypeGuid === t.guid ? 'on' : ''}`}
                    onClick={() => { set('docTypeGuid', t.guid); setF(x => ({ ...x, regType: null, invoiceType: null, fessType: null })) }}>
                    <b>{t.name}</b>
                  </button>
                ))}
              </div>
            </Field>
            <div className="grid cols-2">
              <Field label="اسم الدفتر" required><input className="input" maxLength={100} value={f.docName} disabled={ro} onChange={e => set('docName', e.target.value)} /></Field>
              <Field label="الفرع" required>
                <select className="input" value={f.branchGuid} disabled={ro} onChange={e => { set('branchGuid', e.target.value); set('docPayGuid', '') }}>
                  <option value="">— اختر —</option>
                  {branches.data?.filter(b => b.isUse || b.guid === f.branchGuid).map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
                </select>
              </Field>
              <Field label="قائمة الأسعار" required>
                <select className="input" value={f.priceListGuid} disabled={ro} onChange={e => set('priceListGuid', e.target.value)}>
                  <option value="">— اختر —</option>
                  {lookups.priceLists.map(p => <option key={p.guid} value={p.guid}>{p.name}</option>)}
                </select>
              </Field>
              <Field label="الحالة"><Switch label={f.isUse ? 'الدفتر فعال' : 'الدفتر موقوف'} checked={f.isUse} disabled={ro} onChange={v => set('isUse', v)} /></Field>
            </div>
            {unq === 'addregdoc' && <Field label="نوع التسجيل" required><Segmented value={f.regType} onChange={v => set('regType', v)} options={[{ value: 0, label: 'دبلوم' }, { value: 1, label: 'دورة' }]} /></Field>}
            {unq === 'salesinvoice' && <Field label="نوع الفاتورة" required><Segmented value={f.invoiceType} onChange={v => set('invoiceType', v)} options={[{ value: 0, label: 'دفعة مقدمة' }, { value: 1, label: 'قسط شهري' }, { value: 2, label: 'سداد رسوم' }]} /></Field>}
            {unq === 'regfess' && <Field label="نوع الرسوم" required><Segmented value={f.fessType} onChange={v => set('fessType', v)} options={[{ value: 1, label: 'رسوم دراسية' }, { value: 2, label: 'رسوم إدارية' }]} /></Field>}
            {unq === 'addregdoc' && (
              <div className="book-box">
                <h4><BookOpen size={16} /> إعدادات استمارة التسجيل</h4>
                <Switch label="إضافة رسوم الاختبار والشهادة إلى الاستمارة" checked={f.chkOtherFees} disabled={ro} onChange={v => set('chkOtherFees', v)} />
                <Switch label="تحصيل دفعة مقدمة عند التسجيل (إصدار فاتورة وسند قبض)" checked={f.chkStartPay} disabled={ro} onChange={v => { set('chkStartPay', v); if (!v) { set('chkAmountPay', false); set('docPayGuid', '') } }} />
                {f.chkStartPay && <div className="grid cols-2">
                  <Field label="دفتر فاتورة الدفعة المقدمة" required hint={f.branchGuid ? (payBooks.length ? undefined : 'لا يوجد دفتر فاتورة مبيعات في هذا الفرع') : 'اختر الفرع أولاً'}>
                    <select className="input" value={f.docPayGuid} disabled={ro || !f.branchGuid} onChange={e => set('docPayGuid', e.target.value)}>
                      <option value="">— اختر —</option>
                      {payBooks.map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
                    </select>
                  </Field>
                  <Field label="حد أدنى للدفعة المقدمة">
                    <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
                      <Switch label="" checked={f.chkAmountPay} disabled={ro} onChange={v => { set('chkAmountPay', v); if (!v) set('amountPay', '0') }} />
                      <input className="input" dir="ltr" inputMode="decimal" disabled={ro || !f.chkAmountPay} value={f.amountPay} onChange={e => set('amountPay', e.target.value.replace(/[^\d.]/g, ''))} />
                    </div>
                  </Field>
                </div>}
              </div>
            )}
          </>}

          {tab === 'posting' && <>
            <div className="book-box">
              <Switch label="هذا الدفتر يُنشئ قيد يومية تلقائياً" checked={f.chkDaily} disabled={ro} onChange={v => set('chkDaily', v)} />
              {!f.chkDaily && <div className="acc-note">لن يتم إنشاء قيد محاسبي للمستندات المسجلة على هذا الدفتر.</div>}
            </div>
            {f.chkDaily && <>
              <div className="grid cols-2">
                <AccountField label="الجانب المدين" chk={f.chkMaden} chkLabel="حساب الطالب" onChk={v => { set('chkMaden', v); if (v) set('maden', null) }} value={f.maden} which="maden" />
                <AccountField label="الجانب الدائن" chk={f.chkDaen} chkLabel="حساب الطالب" onChk={v => { set('chkDaen', v); if (v) set('daen', null) }} value={f.daen} which="daen" />
              </div>
              <div className="grid cols-2">
                <div className="acc-field">
                  <div className="acc-head"><b>ضريبة القيمة المضافة</b><Switch label="الدفتر شامل الضريبة" checked={f.chkVat} disabled={ro} onChange={v => { set('chkVat', v); if (!v) set('vat', null) }} /></div>
                  {f.chkVat ? <button type="button" className="acc-pick" disabled={ro} onClick={() => setPicker('vat')}>{f.vat ? <span>{f.vat.label}</span> : <span className="muted">اختر حساب الضريبة...</span>}<Search size={15} /></button>
                    : <div className="acc-note">بدون ضريبة</div>}
                </div>
                <div className="acc-field">
                  <div className="acc-head"><b>مركز التكلفة</b></div>
                  <div style={{ display: 'flex', gap: 6 }}>
                    <button type="button" className="acc-pick" style={{ flex: 1 }} disabled={ro} onClick={() => setPicker('cost')}>{f.cost ? <span>{f.cost.label}</span> : <span className="muted">اختر مركز التكلفة...</span>}<Search size={15} /></button>
                    {f.cost && !ro && <button type="button" className="btn ghost icon" onClick={() => set('cost', null)} aria-label="مسح"><X size={16} /></button>}
                  </div>
                </div>
              </div>
              <div className="alert info"><Calculator size={16} /> مثال: في دفتر التسجيل يكون المدين حساب الطالب والدائن حساب الإيراد، وفي دفاتر المرتجعات يكون العكس.</div>
            </>}
          </>}

          {tab === 'users' && <>
            <div style={{ display: 'flex', gap: 10, alignItems: 'center', flexWrap: 'wrap' }}>
              <div className="input-wrap" style={{ flex: 1, minWidth: 220 }}><Search size={15} /><input className="input" placeholder="ابحث باسم المستخدم..." value={uq} onChange={e => setUq(e.target.value)} /></div>
              {!ro && <>
                <button className="btn secondary sm" onClick={() => set('users', Array.from(new Set([...f.users, ...users.map(u => u.guid)])))}>تحديد الكل</button>
                <button className="btn ghost sm" onClick={() => set('users', f.users.filter(g => !users.some(u => u.guid === g)))}>إلغاء التحديد</button>
              </>}
            </div>
            <p className="muted" style={{ margin: 0 }}><UsersIcon size={14} /> المستخدمون المحددون فقط هم من يظهر لهم هذا الدفتر عند التسجيل أو السداد أو عمل المستندات.</p>
            <div className="user-checks">
              {users.map(u => (
                <Check key={u.guid} checked={f.users.includes(u.guid)} disabled={ro} label={`${u.name}${u.userName ? ` (${u.userName})` : ''}`}
                  onChange={v => set('users', v ? [...f.users, u.guid] : f.users.filter(x => x !== u.guid))} />
              ))}
            </div>
          </>}
        </div>
      )}
      <AnimatePresence>
        {(picker === 'maden' || picker === 'daen' || picker === 'vat') && <AccountPicker onClose={() => setPicker(null)} onPick={a => { set(picker, { guid: a.Guid, label: `${a.Code} - ${a.Name}` }); setPicker(null) }} />}
        {picker === 'cost' && <CostCenterPicker onClose={() => setPicker(null)} onPick={x => { set('cost', { guid: x.guid, label: x.name }); setPicker(null) }} />}
      </AnimatePresence>
    </Drawer>
  )
}
