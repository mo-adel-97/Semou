import { useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Ban, BadgePercent, Pencil, Printer, Save } from 'lucide-react'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { printDiscount } from '../lib/print'
import { Drawer, Field, Spinner } from '../components/ui'

/* سند الخصم (EncodingDiscountDoc): Maden discount account / Daen student. */
export interface DiscountTarget { studentGuid: string; studentName: string; nationalId: string; guid: string | null }
interface DType { guid: string; name: string; accountGuid: string | null; percent: number; isPercent: boolean; forRegDoc: boolean }
interface Prep { types: DType[]; branches: { guid: string; name: string }[]; regDocs: { guid: string; code: string; date: string; total: number }[]; defaultBranch: string | null }
interface Details {
  Guid: string; Code: string; Date: string; BranchGuid: string; BranchName: string | null; TypeGuid: string; TypeName: string | null; RegDocGuid: string | null; RegCode: string | null
  RegTotal: number; Amount: number; Notes: string | null; IsUse: boolean; JournalCode: string | null
}
const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

export function DiscountForm({ target, onClose }: { target: DiscountTarget; onClose: () => void }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const [guid, setGuid] = useState(target.guid)
  const [editing, setEditing] = useState(!target.guid)
  const [branch, setBranch] = useState('')
  const [type, setType] = useState('')
  const [reg, setReg] = useState('')
  const [amount, setAmount] = useState('')
  const [date, setDate] = useState(today())
  const [notes, setNotes] = useState('')

  const prep = useQuery({ queryKey: ['discount-prep', target.studentGuid], queryFn: () => api<Prep>(`/discounts/prepare?student=${target.studentGuid}`), staleTime: 60_000, enabled: editing })
  const details = useQuery({ queryKey: ['discount', guid], queryFn: () => api<Details>(`/discounts/${guid}`), enabled: !!guid, staleTime: 0 })

  useEffect(() => { if (!guid && prep.data?.defaultBranch && !branch) setBranch(prep.data.defaultBranch) }, [prep.data, guid, branch])
  useEffect(() => {
    const d = details.data
    if (!d) return
    setBranch(d.BranchGuid); setType(d.TypeGuid); setReg(d.RegDocGuid && d.RegDocGuid !== EMPTY_GUID ? d.RegDocGuid : ''); setAmount(String(d.Amount)); setDate(d.Date.slice(0, 10)); setNotes(d.Notes ?? '')
  }, [details.data])

  const t = prep.data?.types.find(x => x.guid === type)
  const regDoc = prep.data?.regDocs.find(x => x.guid === reg)
  const computed = t?.forRegDoc && t.isPercent && regDoc ? Math.round(regDoc.total * t.percent) / 100 : null
  const value = computed ?? (Number(amount) || 0)
  const ro = !editing
  const d = details.data

  const save = useMutation({
    mutationFn: (body: object) => guid
      ? api<{ message: string; guid: string }>(`/discounts/${guid}`, { method: 'PUT', json: body })
      : api<{ message: string; guid: string }>('/discounts', { method: 'POST', json: body }),
    onSuccess: async r => {
      qc.invalidateQueries({ queryKey: ['reception'] }); qc.invalidateQueries({ queryKey: ['discount'] })
      setGuid(r.guid); setEditing(false)
      if (await alerts.confirm({ title: r.message, text: 'هل تريد طباعة سند الخصم الآن؟', confirmText: 'طباعة', cancelText: 'لاحقاً', icon: 'success' })) printDiscount(r.guid)
    },
  })
  const submit = async (isUse = true) => {
    if (!branch) return alerts.error('يرجى اختيار الفرع أولاً')
    if (!type) return alerts.error('يرجى اختيار نوع الخصم')
    if (t?.forRegDoc && !reg) return alerts.error('يرجى اختيار استمارة التسجيل')
    if (!notes.trim()) return alerts.error('يرجى إدخال البيان الخاص بعملية الخصم')
    if (value <= 0) return alerts.error('يرجى إدخال قيمة الخصم')
    let reason: string | null = null
    if (guid) { reason = await alerts.reason(isUse ? 'سبب تعديل سند الخصم' : 'سبب إلغاء سند الخصم'); if (!reason) return }
    save.mutate({ studentGuid: target.studentGuid, branchGuid: branch, typeGuid: type, regDocGuid: t?.forRegDoc ? reg : null, amount: value, date, notes, isUse, reason })
  }

  const loading = (!!guid && details.isPending) || (editing && prep.isPending)
  return (
    <Drawer onClose={onClose} icon={<BadgePercent size={22} />} title={guid ? `سند خصم رقم ${d?.Code ?? ''}` : 'سند خصم جديد'}
      subtitle={<>{target.studentName} • <span dir="ltr">{target.nationalId}</span>{d?.JournalCode && <> • قيد رقم {d.JournalCode}</>}</>}
      footer={<>
        {editing && <button className="btn" disabled={save.isPending || loading} onClick={() => submit(true)}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>}</button>}
        {!editing && guid && d?.IsUse && can('discount', 'edit') && <button className="btn secondary" onClick={() => setEditing(true)}><Pencil size={16} /> تعديل</button>}
        {!editing && guid && <button className="btn secondary" onClick={() => printDiscount(guid)}><Printer size={16} /> طباعة</button>}
        {editing && guid && can('discount', 'edit') && <button className="btn danger ghost" disabled={save.isPending} onClick={() => submit(false)}><Ban size={16} /> إلغاء السند</button>}
        <button className="btn ghost" onClick={onClose}>إغلاق</button>
        {d && !d.IsUse && <span className="hint"><span className="badge off">ملغى</span></span>}
      </>}>
      {loading ? <div className="center-fill" style={{ minHeight: 260 }}><Spinner size={28} /></div> : ro && d ? (
        <div className="grid" style={{ gap: 14 }}>
          <div className="fee-info">
            <span><small>نوع الخصم</small><b>{d.TypeName ?? '—'}</b></span>
            <span><small>الفرع</small><b>{d.BranchName ?? '—'}</b></span>
            <span><small>التاريخ</small><b>{new Date(d.Date).toLocaleDateString('ar-EG')}</b></span>
            <span><small>الاستمارة</small><b>{d.RegCode ? `#${d.RegCode}` : '—'}</b></span>
          </div>
          <div className="alert info">{d.Notes}</div>
          <div className="totals"><div className="grand"><small>قيمة الخصم</small><b>{money(d.Amount)}</b></div></div>
        </div>
      ) : (
        <div className="grid" style={{ gap: 16 }}>
          {prep.data?.types.length === 0 && <div className="alert error">لا توجد أنواع خصم معرّفة. يرجى تعريفها أولاً من شاشة "أنواع الخصم" وربط كل نوع بحسابه.</div>}
          <div className="grid cols-2">
            <Field label="الفرع" required>
              <select className="input" value={branch} onChange={e => setBranch(e.target.value)}>
                <option value="">— اختر —</option>
                {prep.data?.branches.map(b => <option key={b.guid} value={b.guid}>{b.name}</option>)}
              </select>
            </Field>
            <Field label="التاريخ" required><input className="input" type="date" value={date} onChange={e => setDate(e.target.value)} /></Field>
          </div>
          <Field label="نوع الخصم" required>
            <div className="pick-grid">
              {prep.data?.types.map(x => (
                <button key={x.guid} type="button" className={`pick-card ${type === x.guid ? 'on' : ''}`} onClick={() => { setType(x.guid); if (!x.forRegDoc) setReg('') }}>
                  <b>{x.name}</b><small>{x.isPercent ? `نسبة ${x.percent}%` : 'مبلغ ثابت'}{x.forRegDoc ? ' • على استمارة تسجيل' : ''}</small>
                </button>
              ))}
            </div>
          </Field>
          {t?.forRegDoc && (
            <Field label="استمارة التسجيل" required>
              <select className="input" value={reg} onChange={e => setReg(e.target.value)}>
                <option value="">— اختر —</option>
                {prep.data?.regDocs.map(r => <option key={r.guid} value={r.guid}>استمارة #{r.code} • {new Date(r.date).toLocaleDateString('ar-EG')} • {money(r.total)} ر.س</option>)}
              </select>
            </Field>
          )}
          <Field label="قيمة الخصم" required hint={computed != null ? `محسوبة تلقائياً: ${t!.percent}% من قيمة الاستمارة` : undefined}>
            <input className="input" dir="ltr" inputMode="decimal" value={computed != null ? String(computed) : amount} disabled={computed != null}
              onChange={e => setAmount(e.target.value.replace(/[^\d.]/g, ''))} />
          </Field>
          <Field label="البيان" required><textarea className="input" maxLength={500} value={notes} onChange={e => setNotes(e.target.value)} /></Field>
          <div className="totals"><div className="grand"><small>قيمة الخصم</small><b>{money(value)}</b></div></div>
          <div className="alert info">يُسجَّل الخصم مديناً على حساب نوع الخصم ودائناً للطالب، فيقل المستحق عليه بنفس القيمة.</div>
        </div>
      )}
    </Drawer>
  )
}
