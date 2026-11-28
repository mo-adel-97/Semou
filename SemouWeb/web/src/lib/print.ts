import QRCode from 'qrcode'
import { api } from '../api'
import { alerts } from './alerts'

/* Printable documents (replace the DevExpress reports): tax invoice with the ZATCA QR, registration form. */

const esc = (s: unknown) => String(s ?? '').replace(/[&<>"]/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[ch]!))
const money = (n: unknown) => (Number(n) || 0).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
/** Official documents always use the institute green, whatever UI theme the user picked. */
const BRAND = '#3f7556'

/* ---------- توثيق المستند: verification QR + official stamp on every printed document ---------- */
export type SealType = 'inv' | 'reg' | 'cash' | 'jv' | 'st' | 'fee' | 'rreg' | 'rbill' | 'disc'
interface Seal { code: string; path: string; issuer: string; cancelled: boolean; printedBy: string }

/** Where the QR points: the public site address once deployed (VITE_PUBLIC_URL), else this server. */
const publicBase = () => (import.meta.env.VITE_PUBLIC_URL as string | undefined)?.replace(/\/$/, '') || location.origin

/** The ERP font for print windows (absolute URLs: the popup has no base of its own). */
const FONT_FACE_CSS = [["Light",300],["Regular",400],["Medium",500],["Bold","600 800"],["Black",900]].map(([n, w]) => `@font-face{font-family:'Thmanyah Sans';src:url(${location.origin}/fonts/thmanyah/thmanyahsans-${n}.woff2) format('woff2');font-weight:${w};font-display:block}`).join('')

/** Shared print CSS: keeps colors when printing + the seal block. */
export const PRINT_BASE_CSS = `
  ${FONT_FACE_CSS}
  *{-webkit-print-color-adjust:exact!important;print-color-adjust:exact!important}
  .seal{display:flex;align-items:center;gap:16px;padding:12px 16px;border:1px solid #e3dcc2;border-radius:16px;background:linear-gradient(135deg,#fffdf6,#fbf7ea);page-break-inside:avoid}
  .seal .vq{width:88px;height:88px;flex:none;border-radius:8px;background:#fff;padding:4px;border:1px solid #eee5c8}
  .seal .txt{flex:1;font-size:11px;color:#5b5440;line-height:1.75}
  .seal .txt b.h{display:block;font-size:13px;color:#6d5a1d;margin-bottom:2px}
  .seal .code{font-family:Consolas,monospace;font-size:13.5px;font-weight:800;letter-spacing:1.5px;color:#1d1d1d;direction:ltr;display:inline-block;background:#fff;border:1px dashed #cdbf8c;border-radius:6px;padding:0 8px}
  .seal svg.stamp{width:100px;height:100px;flex:none;transform:rotate(-10deg)}
  .wm{position:fixed;inset:0;display:grid;place-items:center;pointer-events:none;z-index:0} .wm img{width:48%;opacity:.035;filter:grayscale(1)}
`

const stampSvg = (color: string, cancelled: boolean) => `<svg class="stamp" viewBox="0 0 120 120" xmlns="http://www.w3.org/2000/svg">
  <defs><path id="sc" d="M60,60 m-44,0 a44,44 0 1,1 88,0 a44,44 0 1,1 -88,0"/></defs>
  <circle cx="60" cy="60" r="56" fill="none" stroke="${color}" stroke-width="3"/><circle cx="60" cy="60" r="34" fill="none" stroke="${color}" stroke-width="1.5"/>
  <text font-family="Thmanyah Sans" font-size="10.5" font-weight="800" fill="${color}"><textPath href="#sc" startOffset="2%">معهد السمو المتخصص العالي للتدريب • مستند رسمي •</textPath></text>
  <text x="60" y="56" text-anchor="middle" font-family="Thmanyah Sans" font-size="13" font-weight="800" fill="${color}">${cancelled ? 'ملغي' : 'موثّق'}</text>
  <text x="60" y="72" text-anchor="middle" font-family="Thmanyah Sans" font-size="8.5" fill="${color}">إلكترونياً</text></svg>`

/** Fetches the server seal and returns the HTML block (QR to the public verify page + code + stamp). */
export async function sealHtml(type: SealType, guid: string) {
  const s = await api<Seal>(`/docs/seal/${type}/${guid}`)
  const url = publicBase() + s.path
  const qr = await QRCode.toDataURL(url, { errorCorrectionLevel: 'M', margin: 1, width: 240 })
  const at = new Date().toLocaleString('ar-EG', { dateStyle: 'medium', timeStyle: 'short' })
  const host = publicBase().replace(/^https?:\/\//, '')
  return `<div class="wm"><img src="${location.origin}/logo.png"></div>
  <div class="seal">
    <img class="vq" src="${qr}" alt="">
    <div class="txt"><b class="h">مستند رسمي موثّق إلكترونياً من ${esc(s.issuer)}</b>
      امسح الرمز للتحقق من صحة المستند، أو ادخل رمز التحقق على <span dir="ltr">${esc(host)}/verify</span><br>
      رمز التحقق <span class="code">${esc(s.code)}</span><br>
      طُبع بواسطة ${esc(s.printedBy)} • ${esc(at)}</div>
    ${stampSvg(s.cancelled ? '#b91c1c' : '#7a6320', s.cancelled)}
  </div>`
}

/* ---------- Document shell: A4 page, letterhead, footer with the seal ---------- */
const DOC_CSS = `
  @page{size:A4;margin:12mm}
  *{box-sizing:border-box} html,body{margin:0}
  body{font-family:'Thmanyah Sans',sans-serif;color:#1c2321;font-size:12.5px;background:#fff}
  .page{position:relative;z-index:1;min-height:calc(297mm - 24mm);display:flex;flex-direction:column;gap:16px}
  @media screen{body{background:#eef1ef;padding:24px} .page{width:210mm;margin:auto;background:#fff;padding:12mm;min-height:297mm;box-shadow:0 10px 40px rgba(0,0,0,.12);border-radius:6px}}
  .lh{display:flex;align-items:center;gap:14px;padding-bottom:14px;border-bottom:1px solid #e4ebe7;position:relative}
  .lh:after{content:"";position:absolute;bottom:-1px;inset-inline-start:0;width:120px;height:3px;border-radius:3px;background:${BRAND}}
  .lh img{height:64px} .lh .co{flex:1} .lh h1{margin:0;font-size:17px;font-weight:800;color:#14231c}
  .lh .co div{color:#6b7570;font-size:11px;line-height:1.7}
  .badge-doc{text-align:center;min-width:170px;background:${BRAND};color:#fff;border-radius:14px;padding:9px 16px}
  .badge-doc b{display:block;font-size:17px;font-weight:800;line-height:1.4} .badge-doc small{opacity:.85;font-size:10.5px}
  .badge-doc .no{margin-top:4px;background:rgba(255,255,255,.16);border-radius:8px;padding:1px 10px;font-weight:800;font-size:13px;direction:ltr;display:inline-block}
  .cards{display:grid;grid-template-columns:1fr 1fr;gap:12px}
  .card{border:1px solid #e4ebe7;border-radius:14px;overflow:hidden}
  .card h3{margin:0;padding:7px 14px;font-size:11.5px;font-weight:800;color:${BRAND};background:#f3f7f5;border-bottom:1px solid #e4ebe7}
  .kv{display:grid;grid-template-columns:auto 1fr;gap:5px 14px;padding:10px 14px}
  .kv span{color:#7a847f;font-size:11.5px} .kv b{font-weight:700}
  .hero{padding:10px 14px 6px} .hero .big{font-size:16px;font-weight:800;color:#14231c} .hero .sub{color:#6b7570;font-size:11.5px;margin-top:2px}
  .chips{display:flex;flex-wrap:wrap;gap:6px;padding:0 14px 12px} .chips span{background:#f3f7f5;border-radius:999px;padding:2px 10px;font-size:11px;color:#3d4a44}
  table{width:100%;border-collapse:separate;border-spacing:0;border:1px solid #e4ebe7;border-radius:14px;overflow:hidden}
  th{background:${BRAND};color:#fff;padding:8px 10px;text-align:start;font-size:11.5px;font-weight:700}
  td{padding:9px 10px;border-top:1px solid #edf1ef} tbody tr:nth-child(even) td{background:#fafcfb}
  .n{text-align:left;direction:ltr;font-variant-numeric:tabular-nums;white-space:nowrap} .muted{color:#7a847f}
  .row{display:flex;gap:12px;align-items:stretch}
  .tot{margin-inline-start:auto;min-width:290px;border:1px solid #e4ebe7;border-radius:14px;overflow:hidden}
  .tot div{display:flex;justify-content:space-between;padding:7px 14px;border-bottom:1px solid #edf1ef}
  .tot div:last-child{border:0} .tot .g{background:${BRAND};color:#fff;font-weight:800;font-size:14.5px;padding:10px 14px}
  .pay{flex:1;display:grid;grid-template-columns:repeat(3,1fr);border:1px solid #e4ebe7;border-radius:14px;overflow:hidden}
  .pay div{padding:10px 14px;display:grid;align-content:center;border-inline-end:1px solid #edf1ef} .pay div:last-child{border:0}
  .pay span{color:#7a847f;font-size:11px} .pay b{font-size:15px} .pay .ok b{color:#15803d} .pay .due b{color:#b42318}
  .zatca{display:flex;align-items:center;gap:12px;border:1px solid #e4ebe7;border-radius:14px;padding:10px 14px}
  .zatca img{width:112px;height:112px} .zatca small{display:block;color:#7a847f;font-size:10.5px;line-height:1.7} .zatca b{font-size:12px}
  .words{padding:8px 14px;background:#f3f7f5;border-radius:12px;font-weight:700}
  .terms{color:#5c6762;font-size:11px;line-height:1.9;margin:0;padding-inline-start:18px}
  .foot{margin-top:auto;display:grid;gap:14px}
  .sig{display:flex} .sig div{min-width:220px;text-align:center;color:#6b7570;font-size:11.5px;border-top:1px solid #b9c3be;padding-top:6px;margin-top:34px}
  .void{position:fixed;top:38%;left:0;right:0;text-align:center;font-size:110px;color:rgba(180,35,24,.12);transform:rotate(-20deg);font-weight:800;z-index:2}
`

function openPrint(title: string, page: string) {
  const w = window.open('', '_blank', 'width=960,height=1000')
  if (!w) { alerts.error('المتصفح منع فتح نافذة الطباعة، اسمح بالنوافذ المنبثقة'); return }
  w.document.write(`<!doctype html><html dir="rtl" lang="ar"><head><meta charset="utf-8"><title>${esc(title)}</title>
  <style>${PRINT_BASE_CSS}${DOC_CSS}</style></head><body><div class="page">${page}</div>
  <script>document.fonts.ready.then(()=>setTimeout(()=>window.print(),250))<\/script></body></html>`)
  w.document.close()
}

const letterhead = (o: { name: string; address?: string | null; vat?: string | null; cr?: string | null; tel?: string | null; title: string; sub?: string; no: string }) => `
  <div class="lh"><img src="${location.origin}/logo.png" alt="">
    <div class="co"><h1>${esc(o.name)}</h1>
      <div>${esc(o.address ?? '')}</div>
      <div>${o.vat ? `الرقم الضريبي <b dir="ltr">${esc(o.vat)}</b>` : ''}${o.cr ? ` &nbsp;•&nbsp; السجل التجاري <b dir="ltr">${esc(o.cr)}</b>` : ''}${o.tel ? ` &nbsp;•&nbsp; هاتف <span dir="ltr">${esc(o.tel)}</span>` : ''}</div></div>
    <div class="badge-doc"><b>${esc(o.title)}</b>${o.sub ? `<small>${esc(o.sub)}</small><br>` : ''}<span class="no">No. ${esc(o.no)}</span></div>
  </div>`

interface TaxInvoice {
  header: { companyName: string; address: string; vatNo: string; crNo: string; phone: string; notes: string | null; cancelled: boolean; invoiceNo: string; date: string; seller: string
    studentName: string; studentTel: string; nationalId: string; total: number; tax: number; subTotal: number; amountText: string }
  lines: { code: string; name: string; qty: number; cost: number; taxRate: number; tax: number; total: number }[]
  qr: string
}

/** فاتورة ضريبية مبسطة (TaxInvoiceReport + PRINTTAXINVOICE). */
export async function printTaxInvoice(billGuid: string) {
  try {
    const [d, seal] = await Promise.all([api<TaxInvoice>(`/invoices/${billGuid}/print`), sealHtml('inv', billGuid)])
    const qr = await QRCode.toDataURL(d.qr, { errorCorrectionLevel: 'M', margin: 1, width: 300 })
    const h = d.header
    const rows = d.lines.map((l, i) => `<tr><td class="muted">${i + 1}</td><td><b>${esc(l.name)}</b></td><td class="n">${esc(l.qty)}</td><td class="n">${money(l.cost)}</td><td class="n">${esc(l.taxRate)}%</td><td class="n">${money(l.tax)}</td><td class="n"><b>${money(l.total)}</b></td></tr>`).join('')
    openPrint(`فاتورة ${h.invoiceNo}`, `
      ${h.cancelled ? '<div class="void">ملغاة</div>' : ''}
      ${letterhead({ name: h.companyName, address: h.address, vat: h.vatNo, cr: h.crNo, tel: h.phone, title: 'فاتورة ضريبية مبسطة', sub: 'Simplified Tax Invoice', no: h.invoiceNo })}
      <div class="cards">
        <div class="card"><h3>بيانات العميل</h3><div class="kv"><span>الاسم</span><b>${esc(h.studentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(h.nationalId)}</b><span>الجوال</span><b dir="ltr">${esc(h.studentTel)}</b></div></div>
        <div class="card"><h3>بيانات الفاتورة</h3><div class="kv"><span>رقم الفاتورة</span><b>${esc(h.invoiceNo)}</b><span>التاريخ</span><b dir="ltr">${esc(h.date)}</b><span>البائع</span><b>${esc(h.seller)}</b></div></div>
      </div>
      <table><thead><tr><th style="width:34px">#</th><th>البيان</th><th>الكمية</th><th>السعر</th><th>الضريبة %</th><th>الضريبة</th><th>الإجمالي</th></tr></thead><tbody>${rows}</tbody></table>
      <div class="row">
        <div class="zatca"><img src="${qr}" alt=""><div><b>رمز هيئة الزكاة والضريبة والجمارك</b><small>رمز الفوترة الإلكترونية الإلزامي<br>يُقرأ بتطبيق "تحقق" التابع للهيئة</small></div></div>
        <div class="tot"><div><span>الإجمالي قبل الضريبة</span><b class="n">${money(h.total)}</b></div><div><span>ضريبة القيمة المضافة</span><b class="n">${money(h.tax)}</b></div>
          <div class="g"><span>الإجمالي شامل الضريبة</span><span class="n">${money(h.subTotal)} ر.س</span></div></div>
      </div>
      <div class="words">${esc(String(h.amountText ?? '').trim())}</div>
      <ul class="terms"><li>المبالغ المدفوعة رسوم دراسية غير مستردة.</li>${h.notes ? `<li>${esc(h.notes)}</li>` : ''}</ul>
      <div class="foot">${seal}</div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

interface RegPrint {
  header: { RegType: number; ChkContract: boolean; Notes: string; DocName: string; BrName: string; BrAddress: string; BrVatId: string; BrTel: string; BrFax: string
    BatchName: string | null; LevelName: string | null; CourseName: string | null; DateStart: string | null; DateEnd: string | null; IsPay: boolean; PayCode: string | null; PaidAmount: number }
  student: { name: string; tel: string; nationalId: string }
  code: string; date: string; seller: string
  totals: { total: number; tax: number; subTotal: number }
  lines: { name: string; cost: number; tax: number; total: number; taxRate: number }[]
  fees: { name: string; cost: number; tax: number; total: number }[]
}

/** استمارة التسجيل (StudentRegDoc). */
export async function printRegistration(regGuid: string) {
  try {
    const [d, seal] = await Promise.all([api<RegPrint>(`/registration/${regGuid}/print`), sealHtml('reg', regGuid)])
    const h = d.header
    const isCourse = h.RegType === 1
    const program = d.lines.map(l => l.name).join(' + ') || h.CourseName || ''
    const day = (v: string | null) => String(v ?? '').slice(0, 10)
    const rows = [...d.lines.map(l => ({ ...l, kind: isCourse ? 'دورة' : 'دبلوم' })), ...d.fees.map(f => ({ ...f, kind: 'رسوم' }))]
      .map((l, i) => `<tr><td class="muted">${i + 1}</td><td><b>${esc(l.name)}</b></td><td class="muted">${l.kind}</td><td class="n">${money(l.cost)}</td><td class="n">${money(l.tax)}</td><td class="n"><b>${money(l.total)}</b></td></tr>`).join('')
    const paid = Number(h.PaidAmount) || 0
    const left = Math.max(0, (Number(d.totals.subTotal) || 0) - paid)
    openPrint(`استمارة ${d.code}`, `
      ${letterhead({ name: h.BrName, address: h.BrAddress, vat: h.BrVatId, tel: h.BrTel, title: 'استمارة تسجيل', sub: h.DocName, no: d.code })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(d.student.name)}</b><span>رقم الهوية</span><b dir="ltr">${esc(d.student.nationalId)}</b><span>الجوال</span><b dir="ltr">${esc(d.student.tel)}</b></div></div>
        <div class="card"><h3>${isCourse ? 'الدورة' : 'البرنامج التدريبي'}</h3>
          <div class="hero"><div class="big">${esc(program)}</div><div class="sub">تاريخ التسجيل <span dir="ltr">${esc(d.date)}</span></div></div>
          <div class="chips">${isCourse ? `${h.CourseName ? `<span>كود الدورة: ${esc(h.CourseName)}</span>` : ''}<span>من ${esc(day(h.DateStart))} إلى ${esc(day(h.DateEnd))}</span>` : `<span>الدفعة: ${esc(h.BatchName)}</span><span>المستوى: ${esc(h.LevelName)}</span>`}<span>مسؤول التسجيل: ${esc(d.seller)}</span></div>
        </div>
      </div>
      <table><thead><tr><th style="width:34px">#</th><th>البيان</th><th>النوع</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead><tbody>${rows}</tbody></table>
      <div class="row">
        <div class="pay">
          <div><span>قيمة الاستمارة</span><b class="n">${money(d.totals.subTotal)}</b></div>
          <div class="ok"><span>المدفوع${h.PayCode ? ` (فاتورة ${esc(h.PayCode)})` : ''}</span><b class="n">${money(paid)}</b></div>
          <div class="due"><span>المتبقي</span><b class="n">${money(left)}</b></div>
        </div>
        <div class="tot"><div><span>الإجمالي قبل الضريبة</span><b class="n">${money(d.totals.total)}</b></div><div><span>الضريبة</span><b class="n">${money(d.totals.tax)}</b></div>
          <div class="g"><span>إجمالي الاستمارة</span><span class="n">${money(d.totals.subTotal)} ر.س</span></div></div>
      </div>
      <ul class="terms">
        <li>${h.ChkContract ? 'وقّع المتدرب اتفاقية التدريب.' : 'لم يوقّع المتدرب اتفاقية التدريب بعد، ومهلة التوقيع 30 يوماً من تاريخ التسجيل.'}</li>
        <li>المبالغ المدفوعة رسوم دراسية غير مستردة، ويُستكمل سداد المتبقي حسب نظام المعهد.</li>
        ${h.Notes ? `<li>${esc(h.Notes)}</li>` : ''}
      </ul>
      <div class="foot">
        <div class="sig"><div>توقيع المتدرب</div></div>
        ${seal}
      </div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- طلب الالتحاق (before the registration form exists: sent to the student) ---------- */
interface RegOrderPrint {
  order: { code: number; orderDate: string; status: number; regType: number; studentName: string; nationalId: string | null; studentTel: string | null; createdBy: string | null; docCode: string | null }
  statusName: string
  snapshot: { book: string | null; branchName: string | null; batch: string | null; level: string | null; course: string | null; seller: string | null; plan?: string | null
    items: { name: string; type: number; cost: number; tax: number; total: number }[]; total: number; tax: number; subTotal: number
    hasPayment: boolean; payLines: { name: string; total: number }[]; paySub: number; cash: string | null }
  notes: string | null; contract: boolean; payRef: string | null; payDate: string | null
  branch: { name: string | null; address: string | null; vat: string | null; tel: string | null } | null
  plan: { name: string; months: number; price: number; down: number; installment: number } | null
  student: { academyId: string | null } | null
}

/** طلب الالتحاق: the request as sent (programme, package, amounts, down payment) — printable while it waits for review. */
export async function printRegOrder(guid: string) {
  try {
    const d = await api<RegOrderPrint>(`/requests/registration/${guid}/print`)
    const o = d.order, s = d.snapshot, isCourse = o.regType === 1
    const day = (v: string | null) => (v ? new Date(v).toLocaleDateString('en-GB') : '')
    const program = s.items.filter(x => x.type === 0).map(x => x.name).join(' + ') || s.course || ''
    const rows = s.items.map((l, i) => `<tr><td class="muted">${i + 1}</td><td><b>${esc(l.name)}</b></td><td class="muted">${l.type === 1 ? 'رسوم' : isCourse ? 'دورة' : 'دبلوم'}</td><td class="n">${money(l.cost)}</td><td class="n">${money(l.tax)}</td><td class="n"><b>${money(l.total)}</b></td></tr>`).join('')
    const p = d.plan
    const planCard = p ? `<div class="card"><h3>باقة التسجيل</h3>
        <div class="hero"><div class="big">${esc(p.name)}</div><div class="sub">${p.months > 0 ? `دفعة مقدمة ثم ${p.months} قسطاً شهرياً` : 'سداد كامل عند التسجيل'}</div></div>
        <div class="kv"><span>سعر الباقة</span><b class="n">${money(p.price)} ر.س</b>${p.months > 0 ? `<span>الدفعة المقدمة</span><b class="n">${money(p.down)} ر.س</b><span>القسط الشهري</span><b class="n">${money(p.installment)} ر.س × ${p.months}</b>` : ''}</div></div>` : ''
    const paid = s.hasPayment ? s.paySub : 0
    const waiting = o.status === 0 || o.status === 3
    openPrint(`طلب التحاق ${o.code}`, `
      ${o.status === 2 ? '<div class="void">مرفوض</div>' : ''}
      ${letterhead({ name: d.branch?.name ?? s.branchName ?? '', address: d.branch?.address, vat: d.branch?.vat, tel: d.branch?.tel, title: 'طلب التحاق', sub: d.statusName, no: String(o.code) })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(o.studentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(o.nationalId)}</b><span>الجوال</span><b dir="ltr">${esc(o.studentTel)}</b>${d.student?.academyId ? `<span>الكود الأكاديمي</span><b dir="ltr">${esc(d.student.academyId)}</b>` : ''}</div></div>
        <div class="card"><h3>${isCourse ? 'الدورة' : 'البرنامج التدريبي'}</h3>
          <div class="hero"><div class="big">${esc(program)}</div><div class="sub">تاريخ الطلب <span dir="ltr">${esc(day(o.orderDate))}</span></div></div>
          <div class="chips">${isCourse ? (s.course ? `<span>${esc(s.course)}</span>` : '') : `${s.batch ? `<span>الدفعة: ${esc(s.batch)}</span>` : ''}${s.level ? `<span>المستوى: ${esc(s.level)}</span>` : ''}`}${s.seller ? `<span>مسؤول التسجيل: ${esc(s.seller)}</span>` : ''}</div>
        </div>
        ${planCard}
      </div>
      <table><thead><tr><th style="width:34px">#</th><th>البيان</th><th>النوع</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead><tbody>${rows}</tbody></table>
      <div class="row">
        <div class="pay">
          <div><span>قيمة الطلب</span><b class="n">${money(s.subTotal)}</b></div>
          ${s.hasPayment ? `<div class="ok"><span>الدفعة المقدمة${d.payRef ? ` (مرجع ${esc(d.payRef)})` : ''}</span><b class="n">${money(paid)}</b></div>` : ''}
          <div class="due"><span>المتبقي بعد الدفعة المقدمة</span><b class="n">${money(Math.max(0, s.subTotal - paid))}</b></div>
        </div>
        <div class="tot"><div><span>الإجمالي قبل الضريبة</span><b class="n">${money(s.total)}</b></div><div><span>الضريبة</span><b class="n">${money(s.tax)}</b></div>
          <div class="g"><span>إجمالي الطلب</span><span class="n">${money(s.subTotal)} ر.س</span></div></div>
      </div>
      <ul class="terms">
        ${waiting ? '<li>هذا طلب التحاق قيد المراجعة، وتصدر استمارة التسجيل الرسمية وفاتورة الدفعة المقدمة بعد اعتماده.</li>' : o.status === 1 && o.docCode ? `<li>تم اعتماد الطلب وصدرت استمارة التسجيل رقم ${esc(o.docCode)}.</li>` : ''}
        ${isCourse ? '' : `<li>${d.contract ? 'وقّع المتدرب اتفاقية التدريب.' : 'لم يوقّع المتدرب اتفاقية التدريب بعد، ومهلة التوقيع 30 يوماً من تاريخ التسجيل.'}</li>`}
        ${p && p.months > 0 ? `<li>تُسدَّد الأقساط شهرياً بقيمة ${money(p.installment)} ر.س ابتداءً من الشهر التالي للتسجيل.</li>` : ''}
        <li>المبالغ المدفوعة رسوم دراسية غير مستردة، ويُستكمل سداد المتبقي حسب نظام المعهد.</li>
        ${d.notes ? `<li>${esc(d.notes)}</li>` : ''}
      </ul>
      <div class="foot">
        <div class="sig" style="justify-content:space-between"><div>توقيع المتدرب</div><div>مسؤول التسجيل${o.createdBy ? `: ${esc(o.createdBy)}` : ''}</div></div>
      </div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- كشف حساب الطالب ---------- */
export interface StatementData {
  student: { accountGuid: string; code: string; studentName: string; nationalId: string | null; studentTel: string | null; acadmyId: string | null }
  rows: { date: string; typeName: string | null; docNo: string | null; maden: number; daen: number; counted: boolean; balance: number; notes: string | null }[]
  totals: { maden: number; daen: number; balance: number }
}

export async function printStatement(d: StatementData, filter?: string) {
  try {
    const seal = await sealHtml('st', d.student.accountGuid)
    const s = d.student
    const day = (v: string) => new Date(v).toLocaleDateString('en-GB')
    const rows = d.rows.map((r, i) => `<tr${r.counted ? '' : ' class="muted"'}><td class="muted">${i + 1}</td><td class="n">${day(r.date)}</td><td>${esc(r.typeName)}${r.docNo ? ` <span class="muted">#${esc(r.docNo)}</span>` : ''}</td>
      <td>${esc(r.notes)}</td><td class="n">${r.maden ? money(r.maden) : ''}</td><td class="n">${r.daen ? money(r.daen) : ''}</td><td class="n"><b>${money(r.balance)}</b></td></tr>`).join('')
    const b = d.totals.balance
    openPrint(`كشف حساب ${s.studentName}`, `
      ${letterhead({ name: 'معهد السمو المتخصص العالي للتدريب', title: 'كشف حساب', sub: 'Student Statement', no: s.code })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(s.studentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(s.nationalId)}</b><span>الجوال</span><b dir="ltr">${esc(s.studentTel)}</b></div></div>
        <div class="card"><h3>ملخص الحساب</h3><div class="kv"><span>إجمالي المستحق</span><b class="n" style="text-align:start">${money(d.totals.maden)}</b><span>إجمالي المسدد</span><b class="n" style="text-align:start">${money(d.totals.daen)}</b>
          <span>الرصيد</span><b style="color:${b > 0 ? '#b42318' : '#15803d'}">${money(Math.abs(b))} ر.س ${b > 0 ? '(مستحق على المتدرب)' : b < 0 ? '(رصيد للمتدرب)' : '(الحساب متزن)'}</b></div></div>
      </div>
      ${filter ? `<div class="words">تصفية بالبيان: ${esc(filter)}</div>` : ''}
      <table><thead><tr><th style="width:30px">#</th><th>التاريخ</th><th>المستند</th><th>البيان</th><th>مدين</th><th>دائن</th><th>الرصيد</th></tr></thead>
        <tbody>${rows || '<tr><td colspan="7" class="muted" style="text-align:center;padding:20px">لا توجد حركات</td></tr>'}</tbody></table>
      <div class="row"><div class="tot"><div><span>إجمالي المدين</span><b class="n">${money(d.totals.maden)}</b></div><div><span>إجمالي الدائن</span><b class="n">${money(d.totals.daen)}</b></div>
        <div class="g"><span>الرصيد</span><span class="n">${money(b)} ر.س</span></div></div></div>
      <ul class="terms"><li>الرصيد الموجب مستحق على المتدرب، والسالب رصيد له لدى المعهد.</li><li>هذا الكشف موثّق طالما لم تتم حركات جديدة على الحساب بعد تاريخ طباعته.</li></ul>
      <div class="foot">${seal}</div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- استمارة الرسوم ---------- */
interface FeePrint {
  head: { Code: string; Date: string; BookName: string; SellerName: string | null; StudentName: string; NationalId: string; StudentTel: string | null
    Total: number; Tax: number; SubTotal: number; Notes: string | null; IsUse: boolean; BranchName: string | null }
  lines: { code: string; name: string; cost: number; tax: number; total: number }[]
}

export async function printFeeForm(guid: string) {
  try {
    const [d, seal] = await Promise.all([api<FeePrint>(`/fee-forms/${guid}`), sealHtml('fee', guid)])
    const h = d.head
    const rows = d.lines.map((l, i) => `<tr><td class="muted">${i + 1}</td><td><b>${esc(l.name)}</b></td><td class="n">${money(l.cost)}</td><td class="n">${money(l.tax)}</td><td class="n"><b>${money(l.total)}</b></td></tr>`).join('')
    openPrint(`استمارة رسوم ${h.Code}`, `
      ${h.IsUse ? '' : '<div class="void">ملغاة</div>'}
      ${letterhead({ name: h.BranchName ?? 'معهد السمو المتخصص العالي للتدريب', title: 'استمارة رسوم', sub: h.BookName, no: h.Code })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(h.StudentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(h.NationalId)}</b><span>الجوال</span><b dir="ltr">${esc(h.StudentTel)}</b></div></div>
        <div class="card"><h3>بيانات الاستمارة</h3><div class="kv"><span>رقم الاستمارة</span><b>${esc(h.Code)}</b><span>التاريخ</span><b dir="ltr">${esc(new Date(h.Date).toLocaleDateString('en-GB'))}</b><span>مندوب البيع</span><b>${esc(h.SellerName)}</b></div></div>
      </div>
      <table><thead><tr><th style="width:34px">#</th><th>الرسوم</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead><tbody>${rows}</tbody></table>
      <div class="row"><div class="tot"><div><span>الإجمالي قبل الضريبة</span><b class="n">${money(h.Total)}</b></div><div><span>الضريبة</span><b class="n">${money(h.Tax)}</b></div>
        <div class="g"><span>إجمالي الرسوم المستحقة</span><span class="n">${money(h.SubTotal)} ر.س</span></div></div></div>
      <ul class="terms"><li>الرسوم الموضحة أعلاه مستحقة على المتدرب ويتم سدادها لدى المعهد.</li>${h.Notes ? `<li>${esc(h.Notes)}</li>` : ''}</ul>
      <div class="foot"><div class="sig"><div>توقيع المتدرب</div></div>${seal}</div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- المرتجعات ---------- */
interface ReturnPrint {
  head: { Code: string; Date: string; BookName: string; SellerName: string | null; SourceCode: string | null; StudentName: string; NationalId: string; StudentTel: string | null
    Total: number; Tax: number; SubTotal: number; Notes: string | null; IsUse: boolean; BranchName: string | null }
  lines: { name: string; cost: number; tax: number; total: number }[]
}

export async function printReturn(kind: 'reg' | 'bill', guid: string) {
  try {
    const [d, seal] = await Promise.all([api<ReturnPrint>(`/returns/${kind}/${guid}`), sealHtml(kind === 'reg' ? 'rreg' : 'rbill', guid)])
    const h = d.head
    const title = kind === 'reg' ? 'مرتجع تسجيل' : 'إشعار دائن (مرتجع مبيعات)'
    const rows = d.lines.map((l, i) => `<tr><td class="muted">${i + 1}</td><td><b>${esc(l.name)}</b></td><td class="n">${money(l.cost)}</td><td class="n">${money(l.tax)}</td><td class="n"><b>${money(l.total)}</b></td></tr>`).join('')
    openPrint(`${title} ${h.Code}`, `
      ${h.IsUse ? '' : '<div class="void">ملغى</div>'}
      ${letterhead({ name: h.BranchName ?? 'معهد السمو المتخصص العالي للتدريب', title, sub: h.BookName, no: h.Code })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(h.StudentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(h.NationalId)}</b><span>الجوال</span><b dir="ltr">${esc(h.StudentTel)}</b></div></div>
        <div class="card"><h3>بيانات المرتجع</h3><div class="kv"><span>رقم المرتجع</span><b>${esc(h.Code)}</b><span>التاريخ</span><b dir="ltr">${esc(new Date(h.Date).toLocaleDateString('en-GB'))}</b>
          <span>${kind === 'reg' ? 'الاستمارة الأصلية' : 'الفاتورة الأصلية'}</span><b>${esc(h.SourceCode ?? '—')}</b></div></div>
      </div>
      <table><thead><tr><th style="width:34px">#</th><th>البند</th><th>قبل الضريبة</th><th>الضريبة</th><th>الإجمالي</th></tr></thead><tbody>${rows}</tbody></table>
      <div class="row"><div class="tot"><div><span>قبل الضريبة</span><b class="n">${money(h.Total)}</b></div><div><span>الضريبة</span><b class="n">${money(h.Tax)}</b></div>
        <div class="g"><span>إجمالي المرتجع</span><span class="n">${money(h.SubTotal)} ر.س</span></div></div></div>
      ${h.Notes ? `<div class="words">السبب: ${esc(h.Notes)}</div>` : ''}
      <div class="foot"><div class="sig"><div>توقيع المتدرب</div></div>${seal}</div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- سند الخصم ---------- */
interface DiscountPrint {
  Code: string; Date: string; BranchName: string | null; TypeName: string | null; RegCode: string | null; Amount: number; Notes: string | null; IsUse: boolean
  StudentName: string; NationalId: string; StudentTel: string | null
}

export async function printDiscount(guid: string) {
  try {
    const [d, seal] = await Promise.all([api<DiscountPrint>(`/discounts/${guid}`), sealHtml('disc', guid)])
    const words = await api<{ text: string }>(`/vouchers/lookups/tafkeet?amount=${d.Amount}`).then(r => r.text).catch(() => '')
    openPrint(`سند خصم ${d.Code}`, `
      ${d.IsUse ? '' : '<div class="void">ملغى</div>'}
      ${letterhead({ name: 'معهد السمو المتخصص العالي للتدريب', address: d.BranchName, title: 'سند خصم', sub: d.TypeName ?? undefined, no: d.Code })}
      <div class="cards">
        <div class="card"><h3>بيانات المتدرب</h3><div class="kv"><span>الاسم</span><b>${esc(d.StudentName)}</b><span>رقم الهوية</span><b dir="ltr">${esc(d.NationalId)}</b><span>الجوال</span><b dir="ltr">${esc(d.StudentTel)}</b></div></div>
        <div class="card"><h3>بيانات السند</h3><div class="kv"><span>رقم السند</span><b>${esc(d.Code)}</b><span>التاريخ</span><b dir="ltr">${esc(new Date(d.Date).toLocaleDateString('en-GB'))}</b>
          <span>نوع الخصم</span><b>${esc(d.TypeName)}</b>${d.RegCode ? `<span>على الاستمارة</span><b>${esc(d.RegCode)}</b>` : ''}</div></div>
      </div>
      <div class="row"><div class="tot" style="margin-inline-start:0;flex:1"><div class="g"><span>قيمة الخصم</span><span class="n">${money(d.Amount)} ر.س</span></div></div></div>
      ${words ? `<div class="words">${esc(words.trim())}</div>` : ''}
      <ul class="terms"><li>البيان: ${esc(d.Notes)}</li><li>يُخصم المبلغ أعلاه من المستحق على المتدرب لدى المعهد.</li></ul>
      <div class="foot">${seal}</div>`)
  } catch (e) { alerts.error((e as Error).message) }
}

/* ---------- Reports: printable table with the institute letterhead + CSV export ---------- */
export interface ReportCol { label: string; num?: boolean }
export function printReport(title: string, sub: string, cols: ReportCol[], rows: (string | number | null | undefined)[][], foot?: (string | number)[], extra?: string) {
  const cell = (v: unknown, num?: boolean) => num && typeof v === 'number' ? money(v) : esc(v ?? '')
  const head = cols.map(c => `<th${c.num ? ' class="n"' : ''}>${esc(c.label)}</th>`).join('')
  const body = rows.map(r => `<tr>${r.map((v, i) => `<td${cols[i]?.num ? ' class="n"' : ''}>${cell(v, cols[i]?.num)}</td>`).join('')}</tr>`).join('')
  const tfoot = foot ? `<tfoot><tr>${foot.map((v, i) => `<td${cols[i]?.num ? ' class="n"' : ''} style="font-weight:800;background:#f3f7f5">${cell(v, cols[i]?.num)}</td>`).join('')}</tr></tfoot>` : ''
  openPrint(title, `
    ${letterhead({ name: 'معهد السمو المتخصص العالي للتدريب', title, sub, no: new Date().toLocaleDateString('en-GB') })}
    ${extra ?? ''}
    <table style="font-size:11.5px"><thead><tr>${head}</tr></thead><tbody>${body || `<tr><td colspan="${cols.length}" class="muted" style="text-align:center;padding:18px">لا توجد بيانات</td></tr>`}</tbody>${tfoot}</table>
    <div class="foot"><div class="terms">تقرير مستخرج من نظام معهد السمو بتاريخ ${esc(new Date().toLocaleString('ar-EG'))}</div></div>`)
}

/** Excel-friendly CSV (UTF-8 BOM so Arabic opens correctly). */
export function exportCsv(name: string, cols: ReportCol[], rows: (string | number | null | undefined)[][]) {
  const q = (v: unknown) => { const s = String(v ?? ''); return /[",\n]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s }
  const csv = [cols.map(c => q(c.label)).join(','), ...rows.map(r => r.map(q).join(','))].join('\r\n')
  const a = document.createElement('a')
  a.href = URL.createObjectURL(new Blob(['﻿' + csv], { type: 'text/csv;charset=utf-8' }))
  a.download = `${name}.csv`
  a.click()
  setTimeout(() => URL.revokeObjectURL(a.href), 1000)
}
