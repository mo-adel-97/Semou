import { useDeferredValue, useState, type ReactNode } from 'react'
import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { motion } from 'motion/react'
import { ArrowDownLeft, ArrowUpRight, CalendarClock, FileText, Printer, ReceiptText, Scale, Search, Undo2, Wallet, X } from 'lucide-react'
import { api } from '../api'
import { Drawer, Spinner, Tabs } from '../components/ui'
import { printDiscount, printFeeForm, printRegistration, printReturn, printStatement, printTaxInvoice, type StatementData } from '../lib/print'

/* كشف حساب الطالب (StusdentStatment): movements with running balance, invoices, forms, returns — each printable. */
interface Statement extends StatementData {
  rows: (StatementData['rows'][number] & { kind: string; journalCode: string | null; costCenter: string | null; docGuid: string; journalGuid: string })[]
  bills: { guid: string; code: string; taxNo: string | null; billDate: string; notes: string | null; total: number; tax: number; subTotal: number; userName: string | null; cancelled: boolean }[]
  returns: { date: string; code: string; notes: string | null; total: number; userName: string | null }[]
  regs: { guid: string; code: string; regDate: string; regType: number; total: number; accepted: boolean; returned: boolean; program: string | null }[]
  plans?: StPlan[]
}
/** The registration package (باقة التسجيل) and its schedule: the down payment (no 0) then the monthly instalments. */
interface StPlan {
  regDocGuid: string; regCode: string; regDate: string; returned: boolean; program: string | null
  planName: string; months: number; price: number; downPayment: number; installment: number; firstDue: string | null
  paid: number; remaining: number; overdue: number
  lines: { no: number; due: string; amount: number; paid: number; status: 'paid' | 'partial' | 'overdue' | 'soon' | 'due' }[]
}
type Tab = 'moves' | 'plans' | 'bills' | 'regs' | 'returns'
const STATUS: Record<StPlan['lines'][number]['status'], { text: string; cls: string }> = {
  paid: { text: 'مسدد', cls: 'ok' }, partial: { text: 'مسدد جزئياً', cls: 'info' }, overdue: { text: 'متأخر', cls: 'off' },
  soon: { text: 'يستحق قريباً', cls: 'warn' }, due: { text: 'قادم', cls: 'muted' },
}

const money = (n: number | null | undefined) => (Number(n) || 0).toLocaleString('ar-EG', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const day = (v: string) => new Date(v).toLocaleDateString('ar-EG', { year: 'numeric', month: 'short', day: 'numeric' })

/** Documents that can be reprinted straight from a movement line. */
const printers: Record<string, (guid: string) => Promise<void>> = {
  addregdoc: printRegistration, salesinvoice: printTaxInvoice, regfess: printFeeForm, discount: printDiscount,
  reregister: g => printReturn('reg', g), repay: g => printReturn('bill', g),
}

export function StudentStatement({ account, name, onClose }: { account: string; name: string; onClose: () => void }) {
  const [tab, setTab] = useState<Tab>('moves')
  const [q, setQ] = useState('')
  const dq = useDeferredValue(q.trim())
  const st = useQuery({
    queryKey: ['reception', 'statement', account, dq],
    queryFn: () => api<Statement>(`/reception/students/${account}/statement${dq ? `?q=${encodeURIComponent(dq)}` : ''}`),
    placeholderData: keepPreviousData, staleTime: 15_000,
  })
  const d = st.data
  const bal = d?.totals.balance ?? 0
  const tabs: { key: Tab; label: string; count?: number }[] = [
    { key: 'moves', label: 'الحركات', count: d?.rows.length },
    ...(d?.plans?.length ? [{ key: 'plans' as Tab, label: 'الباقة والأقساط', count: d.plans.length }] : []),
    { key: 'bills', label: 'الفواتير', count: d?.bills.length },
    { key: 'regs', label: 'الاستمارات', count: d?.regs.length },
    { key: 'returns', label: 'المرتجعات', count: d?.returns.length },
  ]

  return (
    <Drawer size="lg" onClose={onClose} icon={<Wallet size={22} />} title={`كشف حساب: ${name}`}
      subtitle={d ? <>كود الطالب #{d.student.code} • الهوية <span dir="ltr">{d.student.nationalId}</span></> : 'جاري التحميل...'}
      tabs={<Tabs value={tab} onChange={setTab} items={tabs.map(t => ({ value: t.key, label: t.label, badge: t.count }))} />}
      footer={<>
        <button className="btn" disabled={!d} onClick={() => d && printStatement(d, dq || undefined)}><Printer size={16} /> طباعة الكشف</button>
        <button className="btn ghost" onClick={onClose}>إغلاق <kbd>Esc</kbd></button>
      </>}>
      {!d ? <div className="center-fill" style={{ minHeight: 300 }}><Spinner size={28} /></div> : (
        <div className="grid" style={{ gap: 16 }}>
          <div className="st-sum">
            <div><span className="ic due"><ArrowUpRight size={18} /></span><small>إجمالي المستحق (مدين)</small><b>{money(d.totals.maden)}</b></div>
            <div><span className="ic ok"><ArrowDownLeft size={18} /></span><small>إجمالي المسدد (دائن)</small><b>{money(d.totals.daen)}</b></div>
            <div className={`big ${bal > 0 ? 'due' : bal < 0 ? 'credit' : 'zero'}`}><span className="ic"><Scale size={18} /></span><small>الرصيد</small>
              <b>{money(Math.abs(bal))} <em>ر.س</em></b><small>{bal > 0 ? 'مستحق على المتدرب' : bal < 0 ? 'رصيد للمتدرب' : 'الحساب متزن'}</small></div>
          </div>

          {tab === 'moves' && (
            <>
              <div className="input-wrap" style={{ maxWidth: 360 }}>
                <Search size={16} /><input className="input" placeholder="بحث في البيان..." value={q} onChange={e => setQ(e.target.value)} />
                {q && <button className="btn ghost sm icon trail" onClick={() => setQ('')} aria-label="مسح"><X size={15} /></button>}
                {st.isFetching && <span className="trail" style={{ insetInlineEnd: q ? 40 : 10 }}><Spinner size={14} /></span>}
              </div>
              <div className="card" style={{ overflow: 'auto' }}>
                <table className="dt ledger">
                  <thead><tr><th>التاريخ</th><th>المستند</th><th>البيان</th><th className="n">مدين</th><th className="n">دائن</th><th className="n">الرصيد</th><th /></tr></thead>
                  <tbody>
                    {d.rows.length === 0 && <tr><td colSpan={7}><div className="empty"><b>{dq ? 'لا توجد حركات مطابقة للبحث' : 'لا توجد حركات على حساب الطالب بعد'}</b></div></td></tr>}
                    {d.rows.map((r, i) => {
                      const p = printers[r.kind]
                      return (
                        <motion.tr key={`${r.journalGuid}-${i}`} className={r.counted ? '' : 'info-row'} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: Math.min(i, 20) * 0.015 }}>
                          <td className="nowrap">{day(r.date)}</td>
                          <td><b>{r.typeName}</b>{r.docNo && <span className="muted"> #{r.docNo}</span>}{!r.counted && <span className="badge muted" title="تظهر للعلم ولا تدخل في الرصيد">للعلم</span>}
                            <div className="muted small">قيد {r.journalCode}{r.costCenter ? ` • ${r.costCenter}` : ''}</div></td>
                          <td className="notes">{r.notes}</td>
                          <td className="n due">{r.maden ? money(r.maden) : ''}</td>
                          <td className="n ok">{r.daen ? money(r.daen) : ''}</td>
                          <td className="n"><b>{money(r.balance)}</b></td>
                          <td>{p && <button className="btn ghost sm icon" title="طباعة المستند" onClick={() => p(r.docGuid)}><Printer size={15} /></button>}</td>
                        </motion.tr>
                      )
                    })}
                  </tbody>
                </table>
              </div>
            </>
          )}

          {tab === 'plans' && <Plans plans={d.plans ?? []} />}

          {tab === 'bills' && (
            <DocList empty="لا توجد فواتير للطالب" items={d.bills.map(b => ({
              key: b.guid, icon: <ReceiptText size={18} />, title: `فاتورة ضريبية ${b.taxNo ?? ''}`.trim(), sub: `رقم ${b.code} • ${day(b.billDate)}${b.userName ? ` • ${b.userName}` : ''}`,
              note: b.notes, amount: b.subTotal, tag: b.cancelled ? { text: 'ملغاة', cls: 'off' } : undefined, onPrint: () => printTaxInvoice(b.guid),
            }))} />
          )}

          {tab === 'regs' && (
            <DocList empty="لا توجد استمارات للطالب" items={d.regs.map(r => ({
              key: r.guid, icon: <FileText size={18} />, title: `استمارة ${r.regType === 1 ? 'دورة' : 'دبلوم'} #${r.code}`, sub: `${r.program ?? ''} • ${day(r.regDate)}`,
              amount: r.total, onPrint: () => printRegistration(r.guid),
              tag: r.returned ? { text: 'مرتجعة', cls: 'off' } : r.accepted ? { text: 'مقبولة', cls: 'ok' } : { text: 'بانتظار القبول', cls: 'warn' },
            }))} />
          )}

          {tab === 'returns' && (
            <DocList empty="لا توجد مرتجعات للطالب" items={d.returns.map(r => ({
              key: r.code, icon: <Undo2 size={18} />, title: `مرتجع #${r.code}`, sub: `${day(r.date)}${r.userName ? ` • ${r.userName}` : ''}`, note: r.notes, amount: r.total,
            }))} />
          )}
        </div>
      )}
    </Drawer>
  )
}

function Plans({ plans }: { plans: StPlan[] }) {
  if (!plans.length) return <div className="empty"><b>لا توجد باقة تسجيل للطالب</b></div>
  return (
    <div className="grid" style={{ gap: 18 }}>
      {plans.map(p => {
        const pct = p.price ? Math.min(100, (p.paid / p.price) * 100) : 0
        return (
          <div key={p.regDocGuid} className="card card-pad grid" style={{ gap: 14 }}>
            <div className="row" style={{ justifyContent: 'space-between', flexWrap: 'wrap', gap: 8 }}>
              <div><b style={{ fontSize: 17 }}><CalendarClock size={17} style={{ verticalAlign: -3, color: 'var(--brand)' }} /> باقة {p.planName}</b>
                <div className="muted small">{p.program ?? ''} • استمارة #{p.regCode} • {day(p.regDate)}</div></div>
              {p.returned ? <span className="badge off">استمارة مرتجعة</span> : p.remaining <= 0 ? <span className="badge ok">مسددة بالكامل</span>
                : p.overdue > 0 ? <span className="badge off">متأخرات {money(p.overdue)} ر.س</span> : <span className="badge info">منتظمة</span>}
            </div>
            <div className="plan-sum">
              <div><small>سعر الباقة</small><b>{money(p.price)}</b></div>
              <div><small>الدفعة المقدمة</small><b>{money(p.months > 0 ? p.downPayment : p.price)}</b></div>
              {p.months > 0 && <div><small>القسط الشهري</small><b>{money(p.installment)} × {p.months}</b></div>}
              <div><small>المسدد</small><b>{money(p.paid)}</b></div>
              <div className={p.remaining > 0 ? 'bad' : ''}><small>المتبقي</small><b>{money(Math.max(0, p.remaining))}</b></div>
            </div>
            {!p.returned && <div className="plan-progress" title={`${Math.round(pct)}%`}><i style={{ width: `${pct}%` }} /></div>}
            {!p.returned && (
              <div style={{ overflow: 'auto' }}>
                <table className="dt ledger">
                  <thead><tr><th>القسط</th><th>تاريخ الاستحقاق</th><th className="n">المبلغ</th><th className="n">المسدد</th><th className="n">المتبقي</th><th>الحالة</th></tr></thead>
                  <tbody>
                    {p.lines.map(l => (
                      <tr key={l.no} className={`sched-row ${l.status}`}>
                        <td><b>{l.no === 0 ? (p.months > 0 ? 'الدفعة المقدمة' : 'السداد الكامل') : `القسط ${l.no}`}</b></td>
                        <td className="nowrap">{day(l.due)}</td>
                        <td className="n">{money(l.amount)}</td>
                        <td className="n ok">{l.paid ? money(l.paid) : ''}</td>
                        <td className="n due">{l.amount - l.paid > 0 ? money(l.amount - l.paid) : ''}</td>
                        <td><span className={`badge ${STATUS[l.status].cls}`}>{STATUS[l.status].text}</span></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
            <small className="muted">المسدد = فواتير السداد الخاصة بهذا الدبلوم منذ التسجيل، موزّعة على الدفعة المقدمة أولاً ثم الأقساط بالترتيب.</small>
          </div>
        )
      })}
    </div>
  )
}

function DocList({ items, empty }: {
  empty: string
  items: { key: string; icon: ReactNode; title: string; sub: string; note?: string | null; amount: number; tag?: { text: string; cls: string }; onPrint?: () => void }[]
}) {
  if (items.length === 0) return <div className="empty"><b>{empty}</b></div>
  return (
    <div className="doc-list">
      {items.map((x, i) => (
        <motion.div key={x.key} className="doc-item" initial={{ opacity: 0, y: 6 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: Math.min(i, 15) * 0.02 }}>
          <span className="di-ic">{x.icon}</span>
          <div className="di-txt"><b>{x.title} {x.tag && <span className={`badge ${x.tag.cls}`}>{x.tag.text}</span>}</b><small>{x.sub}</small>{x.note && <small className="muted">{x.note}</small>}</div>
          <b className="di-amt">{money(x.amount)} <em>ر.س</em></b>
          {x.onPrint && <button className="btn secondary sm" onClick={x.onPrint}><Printer size={15} /> طباعة</button>}
        </motion.div>
      ))}
    </div>
  )
}
