import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'
import { Banknote, CalendarClock, CheckCircle2, GraduationCap, Layers, Pencil, Plus, RotateCcw, Save, Search, Wallet, X } from 'lucide-react'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts, notify } from '../lib/alerts'
import { PageHead } from '../components/Shell'
import { Dialog, Field, Spinner, Stat, StatusBadge, Switch } from '../components/ui'

/*
 * ضبط الأسعار والدفعات المقدمة: the registration packages (cash / 10 months / 24 months ...) and, for every diploma,
 * its price and down payment in each package. The registration wizard sells a diploma only through these packages
 * (server-checked); the account statement shows the package and its instalment schedule.
 */
const FORM = 'webpriceplan'
interface Plan { Guid: string; Code: number; Name: string; Months: number; Description: string | null; Sort: number; IsUse: boolean; Used: number }
interface Diplom { Guid: string; Code: string; Name: string; EnglishName: string | null; TaxRate: number; IsUse: boolean }
interface PriceRow { PlanGuid: string; DiplomGuid: string; Price: number; DownPayment: number; IsUse: boolean; UpdatedAt: string; UpdatedBy: string | null }
interface Data { plans: Plan[]; diploms: Diplom[]; prices: PriceRow[] }
/** A cell as edited: strings while typing. */
interface Cell { price: string; down: string; on: boolean }

const money = (n: number) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 0, maximumFractionDigits: 2 })
const key = (plan: string, dip: string) => `${plan}|${dip}`
const num = (v: string) => v.replace(/[^\d.]/g, '')
const monthsLabel = (m: number) => (m <= 0 ? 'كاش • دفعة واحدة' : m === 1 ? 'شهر واحد' : m === 2 ? 'شهران' : m <= 10 ? `${m} أشهر` : `${m} شهراً`)
export const installmentOf = (price: number, down: number, months: number) => (months > 0 ? Math.round(((price - down) / months) * 100) / 100 : 0)

export function Pricing({ group }: { group: string }) {
  const { can } = useAuth()
  const qc = useQueryClient()
  const canEdit = can(FORM, 'edit'), canAdd = can(FORM, 'add')
  const q = useQuery({ queryKey: ['pricing'], queryFn: () => api<Data>('/pricing') })
  const d = q.data
  const [editPlan, setEditPlan] = useState<Plan | 'new' | null>(null)
  const [search, setSearch] = useState('')
  const [showOff, setShowOff] = useState(false)
  const [cells, setCells] = useState<Record<string, Cell>>({})

  // what is saved, as cells
  const saved = useMemo(() => {
    const m: Record<string, Cell> = {}
    for (const p of d?.prices ?? []) m[key(p.PlanGuid, p.DiplomGuid)] = { price: String(p.Price), down: String(p.DownPayment), on: p.IsUse }
    return m
  }, [d])
  useEffect(() => { setCells(saved) }, [saved])
  const blank: Cell = { price: '', down: '', on: false }
  const cellOf = (plan: string, dip: string) => cells[key(plan, dip)] ?? blank
  const dirtyKeys = Object.keys(cells).filter(k => {
    const a = cells[k], b = saved[k] ?? blank
    return a.on !== b.on || (Number(a.price) || 0) !== (Number(b.price) || 0) || (Number(a.down) || 0) !== (Number(b.down) || 0)
  })

  const plans = (d?.plans ?? []).filter(p => p.IsUse || showOff)
  const diploms = (d?.diploms ?? []).filter(x => (x.IsUse || showOff) && (!search || x.Name.includes(search) || x.Code.includes(search)))
  const set = (plan: Plan, dip: string, patch: Partial<Cell>) => setCells(c => {
    const cur = c[key(plan.Guid, dip)] ?? blank
    const next = { ...cur, ...patch }
    if (patch.price !== undefined && patch.on === undefined && next.price !== '' && !cur.on && !cur.price) next.on = true // typing a price offers the package
    if (plan.Months <= 0) next.down = next.price // كاش: the whole price is paid on registration
    return { ...c, [key(plan.Guid, dip)]: next }
  })

  const save = useMutation({
    mutationFn: (body: object) => api<{ message: string }>('/pricing/prices', { method: 'PUT', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['pricing'] }); notify.success(r.message) },
  })
  const submit = async () => {
    const plansByGuid = Object.fromEntries((d?.plans ?? []).map(p => [p.Guid, p]))
    const dipsByGuid = Object.fromEntries((d?.diploms ?? []).map(x => [x.Guid, x]))
    const items = dirtyKeys.map(k => { const [planGuid, diplomGuid] = k.split('|'); const c = cells[k]; return { planGuid, diplomGuid, price: Number(c.price) || 0, downPayment: Number(c.down) || 0, isUse: c.on } })
    for (const x of items) {
      const where = `${dipsByGuid[x.diplomGuid]?.Name} • ${plansByGuid[x.planGuid]?.Name}`
      if (x.isUse && x.price <= 0) return alerts.error(`يرجى إدخال السعر (${where})، أو إيقاف الباقة لهذا الدبلوم`)
      if (x.downPayment > x.price) return alerts.error(`الدفعة المقدمة أكبر من السعر (${where})`)
    }
    const reason = await alerts.reason('سبب تعديل الأسعار')
    if (!reason) return
    save.mutate({ items, reason })
  }

  const priced = (plan: string) => (d?.prices ?? []).filter(p => p.PlanGuid === plan && p.IsUse && p.Price > 0).length
  const activeDips = (d?.diploms ?? []).filter(x => x.IsUse)
  const complete = activeDips.filter(x => (d?.plans ?? []).filter(p => p.IsUse).every(p => { const c = saved[key(p.Guid, x.Guid)]; return c?.on && Number(c.price) > 0 })).length

  return (
    <>
      <PageHead title="ضبط الأسعار والدفعات المقدمة" group={group}
        subtitle="باقات التسجيل وسعر كل دبلوم ودفعته المقدمة في كل باقة. السعر نهائي شامل الضريبة، والقسط الشهري يُحسب تلقائياً"
        actions={canAdd && <button className="btn" onClick={() => setEditPlan('new')}><Plus size={17} /> باقة جديدة</button>} />

      {!d ? <div className="center-fill" style={{ minHeight: 300 }}><Spinner size={28} /></div> : <>
        <div className="stats">
          <Stat icon={<Layers size={20} />} value={d.plans.filter(p => p.IsUse).length} label="باقات فعالة" />
          <Stat icon={<GraduationCap size={20} />} value={activeDips.length} label="دبلومات فعالة" delay={0.05} />
          <Stat icon={<CheckCircle2 size={20} />} value={`${complete} من ${activeDips.length}`} label="دبلومات مسعّرة في كل الباقات" delay={0.1} />
        </div>

        {/* ---------- the packages ---------- */}
        <div className="pp-plans">
          {d.plans.map((p, i) => (
            <motion.button type="button" key={p.Guid} className={`pp-plan ${p.IsUse ? '' : 'off'}`} onClick={() => (canEdit ? setEditPlan(p) : undefined)}
              initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.04 }} title={canEdit ? 'تعديل الباقة' : undefined}>
              <span className="pp-plan-ic">{p.Months <= 0 ? <Banknote size={20} /> : <CalendarClock size={20} />}</span>
              <span className="pp-plan-txt">
                <b>{p.Name} {!p.IsUse && <StatusBadge on={false} />}</b>
                <small>{monthsLabel(p.Months)}</small>
                {p.Description && <small className="muted">{p.Description}</small>}
                <small className="pp-plan-meta">{priced(p.Guid)} دبلوم مسعّر{p.Used ? ` • ${p.Used} تسجيل` : ''}</small>
              </span>
              {canEdit && <Pencil size={15} className="pp-plan-edit" />}
            </motion.button>
          ))}
          {canAdd && <button type="button" className="pp-plan add" onClick={() => setEditPlan('new')}><Plus size={20} /><b>باقة جديدة</b><small>مثلاً: 6 أشهر، أو عرض خاص</small></button>}
        </div>

        {/* ---------- the matrix: diplomas × packages ---------- */}
        <div className="card pp-card">
          <div className="dt-toolbar">
            <div className="input-wrap"><Search size={16} /><input className="input" placeholder="ابحث باسم الدبلوم أو الكود..." value={search} onChange={e => setSearch(e.target.value)} />
              {search && <button className="btn ghost sm icon trail" onClick={() => setSearch('')} aria-label="مسح"><X size={15} /></button>}</div>
            <Switch label="إظهار الموقوف" checked={showOff} onChange={setShowOff} />
            <span className="muted small" style={{ marginInlineStart: 'auto' }}>فعّل الباقة للدبلوم ثم أدخل السعر والدفعة المقدمة</span>
          </div>
          {plans.length === 0 ? <div className="empty"><b>لا توجد باقات فعالة</b><small>أضف باقة أولاً</small></div> : (
            <div className="dt-scroll">
              <table className="pp-table">
                <thead>
                  <tr>
                    <th className="pp-dip">الدبلوم</th>
                    {plans.map(p => <th key={p.Guid}><b>{p.Name}</b><small>{monthsLabel(p.Months)}</small></th>)}
                  </tr>
                </thead>
                <tbody>
                  {diploms.length === 0 && <tr><td colSpan={plans.length + 1}><div className="empty"><b>لا توجد دبلومات مطابقة</b></div></td></tr>}
                  {diploms.map(x => (
                    <tr key={x.Guid} className={x.IsUse ? '' : 'off'}>
                      <td className="pp-dip">
                        <b>{x.Name}</b>
                        <small><span className="code-chip">{x.Code}</span> {x.TaxRate ? `ضريبة ${x.TaxRate}%` : 'معفى من الضريبة'}{!x.IsUse && ' • موقوف'}</small>
                      </td>
                      {plans.map(p => {
                        const c = cellOf(p.Guid, x.Guid)
                        const k = key(p.Guid, x.Guid)
                        const dirty = dirtyKeys.includes(k)
                        const price = Number(c.price) || 0, down = Number(c.down) || 0
                        const inst = installmentOf(price, down, p.Months)
                        return (
                          <td key={p.Guid} className={`pp-cell ${c.on ? 'on' : ''} ${dirty ? 'dirty' : ''}`}>
                            <label className="pp-toggle">
                              <input type="checkbox" checked={c.on} disabled={!canEdit} onChange={e => set(p, x.Guid, { on: e.target.checked })} />
                              <span>{c.on ? 'متاحة' : 'غير متاحة'}</span>
                            </label>
                            <div className="pp-inputs">
                              <label><small>السعر</small>
                                <input className="input" dir="ltr" inputMode="decimal" placeholder="0" value={c.price} disabled={!canEdit} onChange={e => set(p, x.Guid, { price: num(e.target.value) })} /></label>
                              {p.Months > 0 && <label><small>الدفعة المقدمة</small>
                                <input className="input" dir="ltr" inputMode="decimal" placeholder="0" value={c.down} disabled={!canEdit} onChange={e => set(p, x.Guid, { down: num(e.target.value) })} /></label>}
                            </div>
                            <small className={`pp-calc ${down > price ? 'bad' : ''}`}>
                              {!price ? '—' : down > price ? 'الدفعة أكبر من السعر'
                                : p.Months > 0 ? <>قسط <b>{money(inst)}</b> × {p.Months}</> : 'يُسدَّد كاملاً عند التسجيل'}
                            </small>
                          </td>
                        )
                      })}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </>}

      <AnimatePresence>
        {dirtyKeys.length > 0 && canEdit && (
          <motion.div className="pp-savebar" initial={{ y: 40, opacity: 0 }} animate={{ y: 0, opacity: 1 }} exit={{ y: 40, opacity: 0 }}>
            <span><span className="dirty-dot" /> {dirtyKeys.length} تعديل غير محفوظ</span>
            <button className="btn ghost" onClick={() => setCells(saved)} disabled={save.isPending}><RotateCcw size={16} /> تراجع</button>
            <button className="btn" onClick={submit} disabled={save.isPending}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ الأسعار</>}</button>
          </motion.div>
        )}
      </AnimatePresence>
      <AnimatePresence>
        {editPlan && <PlanDialog plan={editPlan === 'new' ? null : editPlan} onClose={() => setEditPlan(null)} />}
      </AnimatePresence>
    </>
  )
}

function PlanDialog({ plan, onClose }: { plan: Plan | null; onClose: () => void }) {
  const qc = useQueryClient()
  const [name, setName] = useState(plan?.Name ?? '')
  const [months, setMonths] = useState(String(plan?.Months ?? ''))
  const [desc, setDesc] = useState(plan?.Description ?? '')
  const [sort, setSort] = useState(String(plan?.Sort ?? ''))
  const [isUse, setIsUse] = useState(plan?.IsUse ?? true)
  const save = useMutation({
    mutationFn: (body: object) => plan ? api<{ message: string }>(`/pricing/plans/${plan.Guid}`, { method: 'PUT', json: body }) : api<{ message: string }>('/pricing/plans', { method: 'POST', json: body }),
    onSuccess: r => { qc.invalidateQueries({ queryKey: ['pricing'] }); notify.success(r.message); onClose() },
  })
  const m = Number(months) || 0
  const submit = async () => {
    if (!name.trim()) return alerts.error('يرجى إدخال اسم الباقة')
    if (months === '' || m < 0 || m > 120) return alerts.error('يرجى إدخال عدد الشهور (0 للكاش، حتى 120 شهراً)')
    let reason: string | null = null
    if (plan) { reason = await alerts.reason('سبب تعديل الباقة'); if (!reason) return }
    save.mutate({ name, months: m, description: desc, sort: Number(sort) || 0, isUse, reason })
  }
  return (
    <Dialog title={plan ? `تعديل باقة: ${plan.Name}` : 'باقة جديدة'} subtitle={plan?.Used ? `مستخدمة في ${plan.Used} تسجيل؛ التعديل لا يغيّر باقات الطلاب المسجلين` : 'بعد الإضافة حدّد سعر كل دبلوم فيها من الجدول'} onClose={onClose}
      footer={<>
        <button className="btn" onClick={submit} disabled={save.isPending}>{save.isPending ? <Spinner /> : <><Save size={17} /> حفظ</>}</button>
        <button className="btn ghost" onClick={onClose}>إلغاء</button>
      </>}>
      <div className="grid cols-2">
        <Field label="اسم الباقة" required className="span-2"><input className="input" maxLength={100} value={name} onChange={e => setName(e.target.value)} placeholder="مثال: 24 شهر" autoFocus /></Field>
        <Field label="عدد الأقساط الشهرية" required hint="0 = كاش (سداد كامل عند التسجيل)"><input className="input" dir="ltr" inputMode="numeric" value={months} onChange={e => setMonths(e.target.value.replace(/\D/g, ''))} /></Field>
        <Field label="الترتيب" hint="ترتيب ظهور الباقة"><input className="input" dir="ltr" inputMode="numeric" value={sort} onChange={e => setSort(e.target.value.replace(/\D/g, ''))} /></Field>
        <Field label="الوصف" className="span-2"><input className="input" maxLength={250} value={desc} onChange={e => setDesc(e.target.value)} placeholder="يظهر للموظف عند التسجيل" /></Field>
        <div className="span-2"><Switch label="الباقة فعالة (تظهر عند التسجيل)" checked={isUse} onChange={setIsUse} /></div>
        <div className="span-2 alert info"><Wallet size={16} /> {m > 0 ? `دفعة مقدمة عند التسجيل ثم ${m === 1 ? 'قسط شهري واحد' : m <= 10 ? `${m} أقساط شهرية` : `${m} قسطاً شهرياً`} متساوية.` : 'كاش: يُسدَّد السعر كاملاً عند التسجيل.'}</div>
      </div>
    </Dialog>
  )
}
