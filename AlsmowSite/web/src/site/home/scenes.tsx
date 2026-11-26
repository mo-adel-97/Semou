import { BadgeCheck, Bell, CalendarDays, Check, CheckCircle2, Droplets, FileSignature, HeartPulse, Mail, Sparkles, Stethoscope, TrendingUp } from 'lucide-react'
import { motion } from 'motion/react'
import type { ReactNode } from 'react'
import type { FieldKey } from '../../lib/fields'
import { useTyping } from './kit'

/* =====================================================================
   One live «real world» scene per programme field (مجال البرنامج): what the graduate will actually work on,
   playing by itself inside an app window on the programmes stage, in the spirit of the hero's demo.
   The window and all colours follow the stage colour (--stage). The sample data is illustrative.
   ===================================================================== */

const EASE = [0.22, 1, 0.36, 1] as const
const up = (delay: number) => ({ initial: { opacity: 0, y: 10 }, animate: { opacity: 1, y: 0 }, transition: { duration: 0.5, ease: EASE, delay } })
const pop = (delay: number) => ({ initial: { opacity: 0, scale: 0.6 }, animate: { opacity: 1, scale: 1 }, transition: { type: 'spring' as const, stiffness: 320, damping: 18, delay } })

export default function Scene({ field }: { field: FieldKey }) {
  const S = SCENES[field] ?? Business
  return <div className="sc"><S /></div>
}

const SCENES: Record<FieldKey, () => ReactNode> = {
  hr: Hr, nursing: Nursing, health: Health, ai: Ai, law: Law, office: Office, computer: Computer, data: Data,
  language: Language, accounting: Accounting, marketing: Marketing, business: Business,
}

/* ---------- shared bits ---------- */
function Head({ title, pill }: { title: string; pill?: ReactNode }) {
  return <motion.div {...up(0)} className="sc-head"><b>{title}</b>{pill && <span className="sc-pill">{pill}</span>}</motion.div>
}
function Bar({ value, delay = 0.3 }: { value: number; delay?: number }) {
  return <span className="sc-bar"><motion.i initial={{ width: 0 }} animate={{ width: `${value}%` }} transition={{ duration: 1.1, ease: EASE, delay }} /></span>
}
function Done({ children, delay }: { children: ReactNode; delay: number }) {
  return <motion.span {...pop(delay)} className="sc-done"><Check className="h-[1em] w-[1em]" />{children}</motion.span>
}

/* ---------- الموارد البشرية: candidates shortlisted, one hired ---------- */
function Hr() {
  const rows = [['ن', 'نورة العتيبي', 92], ['س', 'سارة القحطاني', 84], ['ر', 'ريم الشمري', 77]] as const
  return (
    <>
      <Head title="المتقدمون لوظيفة أخصائي موارد بشرية" pill="24 طلب" />
      {rows.map(([a, n, v], i) => (
        <motion.div key={n} {...up(0.15 + i * 0.12)} className="sc-row">
          <span className="sc-av">{a}</span>
          <span className="min-w-0 flex-1"><b className="sc-name">{n}</b><span className="sc-sub">مطابقة {v}%</span><Bar value={v} delay={0.4 + i * 0.12} /></span>
          {i === 0 ? <Done delay={1.6}>تم القبول</Done> : <span className="sc-ghost">مقابلة</span>}
        </motion.div>
      ))}
      <motion.div {...up(0.7)} className="sc-stats"><span><b>96%</b>الحضور اليوم</span><span><b>✓</b>مسير الرواتب</span><span><b>3</b>عقود جديدة</span></motion.div>
    </>
  )
}

/* ---------- التمريض: a patient monitor ---------- */
function Nursing() {
  return (
    <>
      <Head title="المريض: أحمد س. · سرير 4" pill={<><i className="sc-live" /> مباشر</>} />
      <motion.div {...up(0.1)} className="sc-monitor">
        <svg viewBox="0 0 300 60" preserveAspectRatio="none" aria-hidden>
          <path className="sc-ecg" d="M0 34 H60 L70 34 L76 10 L84 54 L90 34 H140 L150 34 L156 12 L164 52 L170 34 H220 L230 34 L236 8 L244 56 L250 34 H300" />
        </svg>
      </motion.div>
      <div className="sc-tiles">
        <motion.span {...up(0.25)}><HeartPulse className="sc-beat" /><b>78</b>نبض/د</motion.span>
        <motion.span {...up(0.35)}><Stethoscope /><b>120/80</b>الضغط</motion.span>
        <motion.span {...up(0.45)}><Droplets /><b>98%</b>الأكسجين</motion.span>
      </div>
      {['جرعة الدواء · 10:00', 'قياس العلامات الحيوية', 'تحديث ملف المريض'].map((t, i) => (
        <motion.div key={t} {...up(0.6 + i * 0.12)} className="sc-check"><motion.span {...pop(1.2 + i * 0.45)} className="sc-tick"><Check /></motion.span>{t}</motion.div>
      ))}
      <motion.div {...up(0.9)} className="sc-stats"><span><b>18</b>مرضى القسم</span><span><b>24</b>مهام الوردية</span><span><b>21</b>مكتملة</span></motion.div>
    </>
  )
}

/* ---------- الإدارة الصحية: the hospital today ---------- */
function Health() {
  const deps = [['الطوارئ', 82], ['الباطنية', 64], ['الأطفال', 47]] as const
  return (
    <>
      <Head title="إشغال الأقسام" pill="اليوم" />
      {deps.map(([n, v], i) => (
        <motion.div key={n} {...up(0.1 + i * 0.1)} className="sc-line"><span className="sc-name w-[5.5em]">{n}</span><Bar value={v} delay={0.35 + i * 0.12} /><b className="sc-num">{v}%</b></motion.div>
      ))}
      <motion.div {...up(0.5)} className="sc-head mt-[.3em]"><b>المواعيد القادمة</b></motion.div>
      {[['09:30', 'عيادة القلب · د. خالد'], ['10:15', 'الأشعة · مريض جديد']].map(([t, n], i) => (
        <motion.div key={t} {...up(0.6 + i * 0.1)} className="sc-row"><span className="sc-time">{t}</span><span className="sc-name flex-1">{n}</span>{i === 0 && <Done delay={1.5}>تم التأكيد</Done>}</motion.div>
      ))}
      <motion.div {...up(0.8)} className="sc-stats"><span><b>4.8★</b>رضا المرضى</span><span><b>12د</b>متوسط الانتظار</span></motion.div>
    </>
  )
}

/* ---------- الذكاء الاصطناعي: ask, it answers with a chart ---------- */
function Ai() {
  const q = useTyping('حلّل مبيعات هذا الربع', true, 55)
  return (
    <>
      <div className="sc-bubble me">{q}</div>
      <motion.div {...up(1.5)} className="sc-bubble bot">
        <span className="sc-ai"><Sparkles /> التحليل جاهز</span>
        <div className="sc-chart">{[38, 52, 46, 71, 88].map((h, i) => <motion.i key={i} initial={{ height: 0 }} animate={{ height: `${h}%` }} transition={{ duration: 0.8, ease: EASE, delay: 1.8 + i * 0.1 }} />)}</div>
        <motion.span {...up(2.5)} className="sc-sub"><TrendingUp className="sc-inline" /> ارتفعت المبيعات <b>23%</b> عن الربع الماضي</motion.span>
      </motion.div>
    </>
  )
}

/* ---------- القانون: a contract gets signed and stamped ---------- */
function Law() {
  return (
    <div className="sc-doc">
      <motion.b {...up(0)} className="sc-doc-title">عقد عمل</motion.b>
      {['البند الأول: مدة العقد', 'البند الثاني: الأجر والمزايا', 'البند الثالث: السرية'].map((t, i) => (
        <motion.div key={t} {...up(0.15 + i * 0.12)}><b className="sc-name">{t}</b><span className="sc-lines"><i /><i /></span></motion.div>
      ))}
      <div className="sc-sign">
        <span className="sc-sub">توقيع الطرف الثاني</span>
        <svg viewBox="0 0 160 40" aria-hidden><path className="sc-signature" d="M5 28 C20 5, 30 40, 45 20 S70 8, 80 26 S105 34, 118 14 S140 22, 155 18" /></svg>
      </div>
      <motion.span initial={{ opacity: 0, scale: 2, rotate: -30 }} animate={{ opacity: 1, scale: 1, rotate: -14 }} transition={{ delay: 2.2, type: 'spring', stiffness: 260, damping: 14 }} className="sc-stamp"><FileSignature /> معتمد</motion.span>
    </div>
  )
}

/* ---------- الإدارة المكتبية: the week, meetings, the minutes sent ---------- */
function Office() {
  const days = ['أحد', 'إثن', 'ثلا', 'أرب', 'خمي']
  return (
    <>
      <Head title="جدول المدير · هذا الأسبوع" pill={<><CalendarDays className="sc-inline" /> 5 مواعيد</>} />
      <motion.div {...up(0.1)} className="sc-week">{days.map((d, i) => <span key={d} className={i === 2 ? 'on' : undefined}><small>{d}</small><b>{12 + i}</b></span>)}</motion.div>
      {[['09:00', 'اجتماع الإدارة'], ['11:30', 'استقبال وفد الشركة'], ['01:00', 'مراجعة الخطابات']].map(([t, n], i) => (
        <motion.div key={t} {...up(0.3 + i * 0.12)} className="sc-row"><span className="sc-time">{t}</span><span className="sc-name flex-1">{n}</span>{i === 0 && <Done delay={1.4}>تم</Done>}</motion.div>
      ))}
      <motion.div {...up(1.9)} className="sc-toast"><Mail /> تم إرسال محضر الاجتماع للجميع</motion.div>
    </>
  )
}

/* ---------- الحاسب الآلي: a spreadsheet fills and sums itself ---------- */
function Computer() {
  const rows = [['يناير', '12,400'], ['فبراير', '15,800'], ['مارس', '18,250']]
  return (
    <div className="sc-sheet" dir="ltr">
      <div className="sc-fx"><b>fx</b><motion.span {...up(1.4)}>=SUM(B2:B4)</motion.span></div>
      <div className="sc-grid">
        <span className="h" /><span className="h">A</span><span className="h">B</span>
        <span className="n">1</span><span className="th">الشهر</span><span className="th">المبيعات</span>
        {rows.map(([m, v], i) => [
          <span key={m + 'n'} className="n">{i + 2}</span>,
          <motion.span key={m} {...up(0.2 + i * 0.25)}>{m}</motion.span>,
          <motion.span key={m + 'v'} {...up(0.3 + i * 0.25)}>{v}</motion.span>,
        ])}
        <span className="n">5</span><span className="th">الإجمالي</span>
        <motion.span {...pop(1.8)} className="sum">46,450</motion.span>
      </div>
    </div>
  )
}

/* ---------- إدخال البيانات: a form typed in, saved into the table ---------- */
function Data() {
  const name = useTyping('منيرة الدوسري', true, 70)
  const id = useTyping('1098 4421 07', name.length >= 13, 70)
  return (
    <>
      <Head title="سجل عميل جديد" pill="نموذج 12" />
      <motion.label {...up(0.1)} className="sc-field"><small>الاسم</small><span>{name}<i className="sc-caret" /></span></motion.label>
      <motion.label {...up(0.2)} className="sc-field"><small>رقم الهوية</small><span dir="ltr">{id}</span></motion.label>
      <motion.div {...up(2.6)} className="sc-toast"><CheckCircle2 /> تم الحفظ في قاعدة البيانات</motion.div>
      <motion.div {...up(2.9)} className="sc-row"><span className="sc-av">م</span><span className="sc-name flex-1">منيرة الدوسري</span><span className="sc-ghost">#1284</span></motion.div>
    </>
  )
}

/* ---------- اللغات: a conversation, and the level going up ---------- */
function Language() {
  return (
    <>
      <motion.div {...up(0.1)} className="sc-bubble bot" dir="ltr">Good morning! How can I help you?</motion.div>
      <motion.div {...up(0.9)} className="sc-bubble me" dir="ltr">I&apos;d like to book a meeting room.</motion.div>
      <motion.div {...up(1.6)} className="sc-bubble bot" dir="ltr">Sure, for what time? <Bell className="sc-inline" /></motion.div>
      <motion.div {...up(2.1)} className="sc-line"><span className="sc-name">مستواك</span><Bar value={78} delay={2.3} /><b className="sc-num" dir="ltr">B2 → C1</b></motion.div>
    </>
  )
}

/* ---------- المحاسبة: an invoice and the balance ---------- */
function Accounting() {
  return (
    <>
      <Head title="فاتورة رقم 2041" pill="ضريبية" />
      {[['خدمات استشارية', '8,000'], ['ضريبة القيمة المضافة 15%', '1,200']].map(([n, v], i) => (
        <motion.div key={n} {...up(0.15 + i * 0.12)} className="sc-row"><span className="sc-name flex-1">{n}</span><b className="sc-num">{v}</b></motion.div>
      ))}
      <motion.div {...up(0.5)} className="sc-row sc-total"><span className="flex-1">الإجمالي</span><b>9,200 ر.س</b></motion.div>
      <motion.div {...up(0.8)} className="sc-chart sm">{[40, 55, 48, 66, 80, 74].map((h, i) => <motion.i key={i} initial={{ height: 0 }} animate={{ height: `${h}%` }} transition={{ duration: 0.8, ease: EASE, delay: 1 + i * 0.08 }} />)}</motion.div>
      <Done delay={1.9}>تم ترحيل القيد</Done>
    </>
  )
}

/* ---------- التسويق: a campaign live ---------- */
function Marketing() {
  return (
    <>
      <Head title="حملة التسجيل · سناب وإنستقرام" pill={<><i className="sc-live" /> مباشر</>} />
      <motion.div {...up(0.15)} className="sc-stats big"><span><b>48K</b>وصول</span><span><b>3.2K</b>نقرة</span><span><b>214</b>تسجيل</span></motion.div>
      <motion.div {...up(0.35)} className="sc-graph">
        <svg viewBox="0 0 300 80" preserveAspectRatio="none" aria-hidden><path className="sc-graph-line" d="M0 70 C40 66 60 60 90 52 S140 50 170 36 S230 24 300 8" /></svg>
      </motion.div>
      <motion.div {...up(1.8)} className="sc-toast"><TrendingUp /> نسبة التحويل ارتفعت إلى 6.7%</motion.div>
    </>
  )
}

/* ---------- إدارة الأعمال (default): the performance board ---------- */
function Business() {
  return (
    <>
      <Head title="مؤشرات الأداء · الربع الثالث" pill="محدّث" />
      <motion.div {...up(0.15)} className="sc-stats big"><span><b>92%</b>إنجاز المهام</span><span><b>+18%</b>الإيرادات</span><span><b>4.7★</b>رضا العملاء</span></motion.div>
      <motion.div {...up(0.35)} className="sc-chart">{[30, 45, 40, 62, 58, 80].map((h, i) => <motion.i key={i} initial={{ height: 0 }} animate={{ height: `${h}%` }} transition={{ duration: 0.8, ease: EASE, delay: 0.5 + i * 0.08 }} />)}</motion.div>
      <motion.div {...up(1.4)} className="sc-toast"><BadgeCheck /> تم اعتماد خطة الربع القادم</motion.div>
    </>
  )
}

