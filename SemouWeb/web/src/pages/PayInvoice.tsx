import { useEffect, useState, useDeferredValue } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { AnimatePresence } from 'motion/react'
import { Check, CreditCard, FileUp, History, ReceiptText, Search, Send } from 'lucide-react'
import { api, ApiError, tokenStore } from '../api'
import { alerts } from '../lib/alerts'
import { CostCenterPicker } from '../components/CostCenterPicker'
import { Drawer, Field, Spinner, Switch } from '../components/ui'

/* فاتورة سداد (EncodingNewPayInvoice): kind 1 = سداد الدبلوم/الدورة, kind 2 = سداد رسوم. */
interface Item { code: string; name: string; diplomGuid: string; diplomType: number; taxRate: number; total: number; source: 'file' | 'regdoc' | 'pricelist' }
interface Prep {
  student: { StudentGuid: string; StudentName: string; NationalId: string; foreign: boolean; DiplomName: string | null }
  book: { guid: string; code: number; docName: string; branchName: string; chkVat: boolean }
  items: Item[]
  lastBill: { bdate: string; Refrance: string; CashName: string; FullName: string } | null
  zeroAmount: boolean
  cashBoxes: { Guid: string; Name: string; CashType: number; CostCenterGuid: string | null; CostName: string | null }[]
}
export interface PayTarget { StudentGuid: string; StudentName: string; NationalId: string; FileId: number; kind: 1 | 2 }

const money = (n: number) => n.toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const r2 = (n: number) => Math.round(n * 100) / 100
const today = () => new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)
const OFFICE = 'صندوق المكتب'

export function PayInvoice({ target, onClose }: { target: PayTarget; onClose: () => void }) {
  const qc = useQueryClient()
  const prep = useQuery({ queryKey: ['invoice', 'prepare', target], queryFn: () => api<Prep>(`/invoices/prepare?student=${target.StudentGuid}&file=${target.FileId}&kind=${target.kind}`), staleTime: 0, retry: false, meta: { silent: true } })
  const [amounts, setAmounts] = useState<Record<string, string>>({})
  const [f, setF] = useState({ date: today(), cash: '', cost: '', costName: '', ref: '', payDate: today(), notes: '', split: false, amount1: '', amount2: '', cash2: '', cost2: '', costName2: '', ref2: '', payDate2: today() })
  const [img1, setImg1] = useState<File | null>(null)
  const [img2, setImg2] = useState<File | null>(null)
  const [pick, setPick] = useState<1 | 2 | null>(null)
  const p = prep.data

  useEffect(() => { if (prep.isError) { alerts.error(prep.error.message); onClose() } }, [prep.isError, prep.error, onClose])

  // Desktop shows the student's last payment before opening (ASKLASTBILLINFO).
  useEffect(() => {
    if (!p?.lastBill) return
    const l = p.lastBill
    alerts.info(`آخر عملية سداد لحساب الطالب\nالتاريخ: ${String(l.bdate).slice(0, 10)}\nرقم المرجع: ${l.Refrance}\nالخزينة/البنك: ${l.CashName}\nالمحصل: ${l.FullName}`, 'آخر سداد')
  }, [p?.lastBill])

  // Fees from the registration form are pre-filled with their value (SHOWREGDOCDIPLOM).
  useEffect(() => { if (p && target.kind === 2) setAmounts(Object.fromEntries(p.items.filter(i => i.source === 'regdoc').map(i => [i.diplomGuid, String(i.total)]))) }, [p, target.kind])

  const set = (x: Partial<typeof f>) => setF(v => ({ ...v, ...x }))
  const lines = (p?.items ?? []).filter(i => amounts[i.diplomGuid] !== undefined).map(i => {
    const t = Number(amounts[i.diplomGuid]) || 0
    const cost = i.taxRate ? Math.round(t / (1 + i.taxRate / 100) * 1000) / 1000 : t
    return { i, total: t, cost, tax: i.taxRate ? Math.round(cost * i.taxRate / 100 * 1000) / 1000 : 0 }
  })
  const sub = r2(lines.reduce((s, l) => s + l.total, 0)), tax = r2(lines.reduce((s, l) => s + l.tax, 0))
  const cash = p?.cashBoxes.find(c => c.Guid === f.cash), cash2 = p?.cashBoxes.find(c => c.Guid === f.cash2)
  const needImg1 = !!cash && cash.Name !== OFFICE, needImg2 = f.split && !!cash2 && cash2.Name !== OFFICE
  const deferred = useDeferredValue(sub)
  const words = useQuery({ queryKey: ['tafkeet', deferred], queryFn: () => api<{ text: string }>(`/vouchers/lookups/tafkeet?amount=${deferred}`), enabled: deferred > 0, staleTime: Infinity })

  const validate = (): string | null => {
    if (!f.cash) return 'يرجى اختيار الخزينة / البنك الخاص بعملية السداد'
    if (!f.cost) return 'يرجى اختيار مركز التكلفة الخاص بعملية السداد'
    if (!f.ref.trim()) return 'يرجى إدخال رقم المرجع الخاص بعملية السداد'
    if (sub === 0 && !p?.zeroAmount) return 'يرجى إدخال المبلغ الخاص بعملية السداد'
    if (!f.payDate) return 'يرجى إدخال تاريخ الحوالة البنكية / السداد'
    if (f.split) {
      if (!f.cash2) return 'يرجى اختيار الخزينة / البنك الثاني'
      if (!f.cost2) return 'يرجى اختيار مركز التكلفة الثاني الخاص بعملية السداد'
      if (!f.ref2.trim()) return 'يرجى إدخال رقم المرجع الثاني الخاص بعملية السداد'
      if (!(Number(f.amount1) > 0)) return 'يرجى إدخال قيمة الحوالة الأولى'
      if (!(Number(f.amount2) > 0)) return 'يرجى إدخال قيمة الحوالة الثانية'
      if (r2(Number(f.amount1) + Number(f.amount2)) !== sub) return 'مبالغ الحوالات لا تساوي إجمالي الفاتورة'
    }
    if (needImg1 && !img1) return 'لا يمكن إتمام عملية السداد بدون إرفاق صورة الحوالة الأولى'
    if (needImg2 && !img2) return 'لا يمكن إتمام عملية السداد بدون إرفاق صورة الحوالة الثانية'
    return null
  }

  const save = useMutation({
    mutationFn: async () => {
      const e = validate(); if (e) throw Object.assign(new Error(e), { status: 400 })
      if (!(await alerts.confirm({ title: 'إرسال طلب السداد؟', text: `طلب سداد بقيمة ${money(sub)} ر.س يُرسل إلى الحسابات للمراجعة مع صورة الحوالة. تصدر الفاتورة الضريبية وسند القبض عند تأكيد الطلب.`, confirmText: 'إرسال الطلب' }))) return null
      alerts.loading('جاري إرسال الطلب...')
      const fd = new FormData()
      fd.append('payload', JSON.stringify({
        studentGuid: target.StudentGuid, fileId: target.FileId, kind: target.kind, date: f.date,
        lines: lines.map(l => ({ diplomGuid: l.i.diplomGuid, total: l.total })),
        cashAccountGuid: f.cash, costCenterGuid: f.cost, ref: f.ref, payDate: f.payDate, notes: f.notes,
        split: f.split, amount1: Number(f.amount1) || 0, amount2: Number(f.amount2) || 0, cashAccountGuid2: f.cash2 || null, costCenterGuid2: f.cost2 || null, ref2: f.ref2, payDate2: f.payDate2,
      }))
      if (img1) fd.append('file1', img1, img1.name)
      if (f.split && img2) fd.append('file2', img2, img2.name)
      const res = await fetch('/api/requests/payment', { method: 'POST', body: fd, headers: { Authorization: `Bearer ${tokenStore.get()}` } })
      const body = await res.json().catch(() => null)
      if (!res.ok) throw new ApiError(res.status, body?.message ?? 'تعذر إرسال طلب السداد')
      return body as { message: string; code: number; guid: string }
    },
    onSuccess: async r => {
      if (!r) return
      qc.invalidateQueries({ queryKey: ['reception'] }); qc.invalidateQueries({ queryKey: ['requests'] })
      await alerts.success(r.message, 'تصدر الفاتورة الضريبية وسند القبض فور تأكيد الطلب من الحسابات.')
      onClose()
    },
  })

  return (
    <Drawer size="lg" onClose={onClose} icon={target.kind === 1 ? <CreditCard size={22} /> : <ReceiptText size={22} />}
      title={`${target.kind === 1 ? 'سداد' : 'سداد رسوم'}: ${target.StudentName}`} subtitle={p ? `${p.book.docName} • ${p.book.branchName}` : 'فاتورة مبيعات'}
      footer={<>
        <button className="btn" onClick={() => save.mutate()} disabled={save.isPending || !p}>{save.isPending ? <Spinner /> : <><Send size={17} /> إرسال طلب السداد</>}</button>
        <button className="btn ghost" onClick={onClose}>إغلاق</button>
        <span className="hint">الإجمالي: <b className="num-strong">{money(sub)}</b> ر.س</span>
      </>}>
      {!p ? <div className="grid">{Array.from({ length: 6 }).map((_, i) => <div key={i} className="sk" style={{ height: 40 }} />)}</div> : (
        <div className="grid cols-2">
          {p.lastBill && <div className="span-all alert info"><History size={16} /> آخر سداد: {String(p.lastBill.bdate).slice(0, 10)} • مرجع {p.lastBill.Refrance} • {p.lastBill.CashName} • {p.lastBill.FullName}</div>}
          <Field label="تاريخ الفاتورة" required><input type="date" className="input" value={f.date} onChange={e => set({ date: e.target.value })} /></Field>
          <Field label="الدبلوم / الدورة"><input className="input" readOnly value={p.student.DiplomName ?? ''} /></Field>

          <div className="span-all">
            <div className="lines-head"><h4>{target.kind === 1 ? 'المبلغ المدفوع' : 'الرسوم'}</h4>{p.student.foreign && <span className="badge warn">أجنبي: ضريبة 15%</span>}</div>
            <div className="item-list">
              {p.items.map(i => {
                const on = amounts[i.diplomGuid] !== undefined || target.kind === 1
                return (
                  <div key={i.diplomGuid} className={`item ${on ? 'on' : ''}`} onClick={() => target.kind === 2 && setAmounts(a => { const n = { ...a }; if (n[i.diplomGuid] !== undefined) delete n[i.diplomGuid]; else n[i.diplomGuid] = i.total ? String(i.total) : ''; return n })}>
                    {target.kind === 2 && <span className="cbx-lg">{on && <Check size={15} />}</span>}
                    <div className="nm"><b>{i.name}</b><small>كود {i.code} • ضريبة {i.taxRate}%{i.source === 'regdoc' ? ' • من استمارة التسجيل' : i.source === 'pricelist' ? ' • من قائمة الرسوم' : ''}</small></div>
                    {on && <input className="input price" dir="ltr" inputMode="decimal" placeholder="0.00" autoFocus={target.kind === 1} value={amounts[i.diplomGuid] ?? ''} onClick={e => e.stopPropagation()}
                      onChange={e => setAmounts(a => ({ ...a, [i.diplomGuid]: e.target.value.replace(/[^\d.]/g, '') }))} />}
                  </div>
                )
              })}
            </div>
          </div>
          <div className="span-all totals">
            <div><small>قبل الضريبة</small><b>{money(r2(sub - tax))}</b></div><div><small>الضريبة</small><b>{money(tax)}</b></div>
            <div className="grand"><small>إجمالي الفاتورة</small><b>{money(sub)}</b></div>
          </div>
          {sub > 0 && <div className="tafkeet span-all">{words.data?.text?.trim() || '…'}</div>}

          <div className="section-label span-all">السداد</div>
          <Field label="الخزينة / البنك" required>
            <select className="input" value={f.cash} onChange={e => { const c = p.cashBoxes.find(x => x.Guid === e.target.value); set({ cash: e.target.value, cost: c?.CostCenterGuid ?? f.cost, costName: c?.CostName ?? f.costName }) }}>
              <option value="">اختر...</option>{p.cashBoxes.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
            </select>
          </Field>
          <Field label="مركز التكلفة" required><button type="button" className={`input pick-input ${f.cost ? '' : 'empty'}`} onClick={() => setPick(1)}>{f.costName || 'اختر...'}<Search size={14} /></button></Field>
          <Field label="رقم المرجع / الحوالة" required><input className="input" dir="ltr" maxLength={50} value={f.ref} onChange={e => set({ ref: e.target.value })} /></Field>
          <Field label="تاريخ السداد / الحوالة" required><input type="date" className="input" value={f.payDate} onChange={e => set({ payDate: e.target.value })} /></Field>
          {needImg1 && <Field label="صورة الحوالة الأولى" required className="span-2">
            <label className={`attach-row ${img1 ? 'done' : ''}`} style={{ cursor: 'pointer' }}><span className="ic"><FileUp size={20} /></span><div className="who"><b>{img1?.name ?? 'اختيار صورة / PDF'}</b><small>تُرفع على خادم الأرشيف بعد حفظ الفاتورة</small></div>
              <input type="file" hidden accept=".pdf,image/*" onChange={e => setImg1(e.target.files?.[0] ?? null)} /></label>
          </Field>}
          <Field label="ملاحظات الفاتورة" className="span-2"><input className="input" maxLength={250} value={f.notes} onChange={e => set({ notes: e.target.value })} /></Field>
          <div className="span-2"><Switch label="تقسيم السداد على بنك / خزينة ثانية" checked={f.split} onChange={v => set({ split: v })} /></div>
          {f.split && <>
            <Field label="مبلغ الحوالة الأولى" required><input className="input" dir="ltr" inputMode="decimal" value={f.amount1} onChange={e => set({ amount1: e.target.value.replace(/[^\d.]/g, '') })} /></Field>
            <Field label="مبلغ الحوالة الثانية" required hint={`يجب أن يساوي المجموع ${money(sub)}`}><input className="input" dir="ltr" inputMode="decimal" value={f.amount2} onChange={e => set({ amount2: e.target.value.replace(/[^\d.]/g, '') })} /></Field>
            <Field label="الخزينة / البنك 2" required>
              <select className="input" value={f.cash2} onChange={e => { const c = p.cashBoxes.find(x => x.Guid === e.target.value); set({ cash2: e.target.value, cost2: c?.CostCenterGuid ?? f.cost2, costName2: c?.CostName ?? f.costName2 }) }}>
                <option value="">اختر...</option>{p.cashBoxes.map(c => <option key={c.Guid} value={c.Guid}>{c.Name}</option>)}
              </select>
            </Field>
            <Field label="مركز التكلفة 2" required><button type="button" className={`input pick-input ${f.cost2 ? '' : 'empty'}`} onClick={() => setPick(2)}>{f.costName2 || 'اختر...'}<Search size={14} /></button></Field>
            <Field label="رقم المرجع 2" required><input className="input" dir="ltr" maxLength={50} value={f.ref2} onChange={e => set({ ref2: e.target.value })} /></Field>
            <Field label="تاريخ الحوالة 2" required><input type="date" className="input" value={f.payDate2} onChange={e => set({ payDate2: e.target.value })} /></Field>
            {needImg2 && <Field label="صورة الحوالة الثانية" required className="span-2">
              <label className={`attach-row ${img2 ? 'done' : ''}`} style={{ cursor: 'pointer' }}><span className="ic"><FileUp size={20} /></span><div className="who"><b>{img2?.name ?? 'اختيار صورة / PDF'}</b></div>
                <input type="file" hidden accept=".pdf,image/*" onChange={e => setImg2(e.target.files?.[0] ?? null)} /></label>
            </Field>}
          </>}
        </div>
      )}
      <AnimatePresence>
        {pick && <CostCenterPicker onClose={() => setPick(null)} onPick={x => { set(pick === 1 ? { cost: x.guid, costName: x.name } : { cost2: x.guid, costName2: x.name }); setPick(null) }} />}
      </AnimatePresence>
    </Drawer>
  )
}
