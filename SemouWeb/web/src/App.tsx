import { lazy, Suspense, type ComponentType } from 'react'
import { Navigate, Route, Routes, useLocation, useParams } from 'react-router-dom'
import { AnimatePresence, motion } from 'motion/react'
import { ShieldAlert } from 'lucide-react'
import { useAuth } from './auth'
import { findItem } from './menu'
import { useAllowedMenu } from './useAllowedMenu'
import { useUiConfig } from './uiConfig'
import { Shell, PageHead } from './components/Shell'
import { Spinner } from './components/ui'
const VerifyDoc = lazy(() => import('./pages/VerifyDoc').then(m => ({ default: m.VerifyDoc })))
import { Login } from './pages/Login'
import { Home } from './pages/Home'
import { ComingSoon } from './pages/ComingSoon'

const ChartTree = lazy(() => import('./pages/ChartTree').then(m => ({ default: m.ChartTree })))
const Reception = lazy(() => import('./pages/Reception').then(m => ({ default: m.Reception })))
const Transfers = lazy(() => import('./pages/Transfers').then(m => ({ default: m.Transfers })))
const Journal = lazy(() => import('./pages/Journal').then(m => ({ default: m.Journal })))
const Vouchers = lazy(() => import('./pages/Vouchers').then(m => ({ default: m.Vouchers })))
const CashBoxes = lazy(() => import('./pages/CashBoxes').then(m => ({ default: m.CashBoxes })))
const AccountStatement = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.AccountStatement })))
const GeneralJournal = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.GeneralJournal })))
const TrialBalance = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.TrialBalance })))
const IncomeStatement = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.IncomeStatement })))
const CashHistory = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.CashHistory })))
const StudyRequests = lazy(() => import('./pages/StudyRequests').then(m => ({ default: m.StudyRequests })))
const AdmissionRequests = lazy(() => import('./pages/Requests').then(m => ({ default: m.AdmissionRequests })))
const MyRequests = lazy(() => import('./pages/Requests').then(m => ({ default: m.MyRequests })))
const PaymentRequests = lazy(() => import('./pages/Requests').then(m => ({ default: m.PaymentRequests })))
const RegistrationsReport = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.RegistrationsReport })))
const SalesInvoicesReport = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.SalesInvoicesReport })))
const TaxReport = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.TaxReport })))
const TaxReturnReport = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.TaxReturnReport })))
const MarketersReport = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.MarketersReport })))
const PaymentFollow = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.PaymentFollow })))
const CustomerBalances = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.CustomerBalances })))
const ExpenseAnalysis = lazy(() => import('./pages/SalesReports').then(m => ({ default: m.ExpenseAnalysis })))
const OpeningBalances = lazy(() => import('./pages/AccountReports').then(m => ({ default: m.OpeningBalances })))
const DiplomaStudents = lazy(() => import('./pages/StudentLists').then(m => ({ default: m.DiplomaStudents })))
const CourseStudents = lazy(() => import('./pages/StudentLists').then(m => ({ default: m.CourseStudents })))
const PriceLists = lazy(() => import('./pages/Control').then(m => ({ default: m.PriceLists })))
const BatchSeats = lazy(() => import('./pages/Control').then(m => ({ default: m.BatchSeats })))
const NewStudents = lazy(() => import('./pages/Control').then(m => ({ default: m.NewStudents })))
const DiscountDocs = lazy(() => import('./pages/Control').then(m => ({ default: m.DiscountDocs })))
const DocBooks = lazy(() => import('./pages/DocBooks').then(m => ({ default: m.DocBooks })))
const GeneralSettings = lazy(() => import('./pages/Settings').then(m => ({ default: m.GeneralSettings })))
const Pricing = lazy(() => import('./pages/Pricing').then(m => ({ default: m.Pricing })))
const CrudScreen = lazy(() => import('./pages/CrudScreen').then(m => ({ default: m.CrudScreen })))

/**
 * Ported screens, code-split (each screen loads only when opened).
 * To add a screen: add it here and set ported: true in menu.ts.
 */
const SCREENS: Record<string, ComponentType> = {
  branches: lazy(() => import('./pages/Branches').then(m => ({ default: m.Branches }))),
  users: lazy(() => import('./pages/Users').then(m => ({ default: m.Users }))),
  'change-password': lazy(() => import('./pages/ChangePassword').then(m => ({ default: m.ChangePassword }))),
}

const PageLoader = () => <div className="center-fill" style={{ minHeight: '50vh' }}><Spinner size={28} /></div>

function Screen() {
  const { key = '' } = useParams()
  const allowed = useAllowedMenu({ includeHidden: true })
  const { menu } = useUiConfig()
  // The screen as arranged in الإعدادات العامة (its title and group), else as the program defines it.
  const item = menu.flatMap(g => g.items.filter(i => !i.href).map(i => ({ ...i, group: g }))).find(i => i.key === key) ?? findItem(key)
  if (!item) return <Navigate to="/" replace />

  // Same as the desktop: a screen you can't view is not reachable, even by typing its URL.
  if (!allowed.some(g => g.items.some(i => i.key === key))) {
    return (
      <>
        <PageHead title={item.title} group={item.group.title} />
        <div className="card soon">
          <div className="ic danger"><ShieldAlert size={38} /></div>
          <h2>لا تملك صلاحية فتح هذه الشاشة</h2>
          <p>يرجى التواصل مع مدير النظام لمنحك الصلاحية.</p>
        </div>
      </>
    )
  }
  // Accounting: one tree screen serves both the card and the chart menu items (same as the desktop pair).
  const g = item.group.title
  if (key === 'accounts' || key === 'chart-of-accounts') return <Suspense fallback={<PageLoader />}><ChartTree key="accounts" chart="accounts" group={g} /></Suspense>
  if (key === 'cost-centers' || key === 'cost-center-tree') return <Suspense fallback={<PageLoader />}><ChartTree key="cost-centers" chart="cost-centers" group={g} /></Suspense>
  if (key === 'cash-boxes' || key === 'banks') return <Suspense fallback={<PageLoader />}><CashBoxes key={key} kind={key} group={g} /></Suspense>
  if (key === 'receipt') return <Suspense fallback={<PageLoader />}><Vouchers key="receipt" kind="receipt" group={g} /></Suspense>
  if (key === 'payment') return <Suspense fallback={<PageLoader />}><Vouchers key="payment" kind="payment" group={g} /></Suspense>
  if (key === 'journal-entry') return <Suspense fallback={<PageLoader />}><Journal key="daily" kind="daily" group={g} /></Suspense>
  if (key === 'opening-entry') return <Suspense fallback={<PageLoader />}><Journal key="opening" kind="opening" group={g} /></Suspense>
  if (key === 'cash-transfer') return <Suspense fallback={<PageLoader />}><Transfers group={g} /></Suspense>
  if (key === 'diploma-students') return <Suspense fallback={<PageLoader />}><DiplomaStudents group={g} /></Suspense>
  if (key === 'course-students') return <Suspense fallback={<PageLoader />}><CourseStudents group={g} /></Suspense>
  if (key === 'account-statement') return <Suspense fallback={<PageLoader />}><AccountStatement group={g} /></Suspense>
  if (key === 'general-journal') return <Suspense fallback={<PageLoader />}><GeneralJournal group={g} /></Suspense>
  if (key === 'trial-balance') return <Suspense fallback={<PageLoader />}><TrialBalance group={g} /></Suspense>
  if (key === 'income-statement') return <Suspense fallback={<PageLoader />}><IncomeStatement group={g} /></Suspense>
  if (key === 'cash-history') return <Suspense fallback={<PageLoader />}><CashHistory group={g} /></Suspense>
  if (key === 'opening-balances') return <Suspense fallback={<PageLoader />}><OpeningBalances group={g} /></Suspense>
  if (key === 'study-requests') return <Suspense fallback={<PageLoader />}><StudyRequests group={g} /></Suspense>
  if (key === 'admission-requests') return <Suspense fallback={<PageLoader />}><AdmissionRequests group={g} /></Suspense>
  if (key === 'my-requests') return <Suspense fallback={<PageLoader />}><MyRequests group={g} /></Suspense>
  if (key === 'payment-requests') return <Suspense fallback={<PageLoader />}><PaymentRequests group={g} /></Suspense>
  if (key === 'registrations-report') return <Suspense fallback={<PageLoader />}><RegistrationsReport group={g} /></Suspense>
  if (key === 'sales-report') return <Suspense fallback={<PageLoader />}><SalesInvoicesReport group={g} /></Suspense>
  if (key === 'tax-report') return <Suspense fallback={<PageLoader />}><TaxReport group={g} /></Suspense>
  if (key === 'tax-return-report') return <Suspense fallback={<PageLoader />}><TaxReturnReport group={g} /></Suspense>
  if (key === 'marketers-report') return <Suspense fallback={<PageLoader />}><MarketersReport group={g} /></Suspense>
  if (key === 'payment-follow') return <Suspense fallback={<PageLoader />}><PaymentFollow group={g} /></Suspense>
  if (key === 'customer-balances') return <Suspense fallback={<PageLoader />}><CustomerBalances group={g} /></Suspense>
  if (key === 'expense-analysis') return <Suspense fallback={<PageLoader />}><ExpenseAnalysis group={g} /></Suspense>
  if (key === 'price-list') return <Suspense fallback={<PageLoader />}><PriceLists group={g} /></Suspense>
  if (key === 'batch-count') return <Suspense fallback={<PageLoader />}><BatchSeats group={g} /></Suspense>
  if (key === 'new-students') return <Suspense fallback={<PageLoader />}><NewStudents group={g} /></Suspense>
  if (key === 'discount-doc') return <Suspense fallback={<PageLoader />}><DiscountDocs group={g} /></Suspense>
  if (key === 'price-plans') return <Suspense fallback={<PageLoader />}><Pricing group={g} /></Suspense>
  if (key === 'reg-settings') return <Suspense fallback={<PageLoader />}><DocBooks group={g} /></Suspense>
  if (key === 'general-settings') return <Suspense fallback={<PageLoader />}><GeneralSettings group={g} /></Suspense>
  if (key === 'reception') return <Suspense fallback={<PageLoader />}><Reception group={g} /></Suspense>
  if (item.engine) return <Suspense fallback={<PageLoader />}><CrudScreen key={item.engine} screen={item.engine} group={item.group.title} /></Suspense>
  const C = SCREENS[key]
  return C ? <Suspense fallback={<PageLoader />}><C /></Suspense> : <ComingSoon item={item} group={item.group.title} />
}

export default function App() {
  const { me, loading } = useAuth()
  const loc = useLocation()

  // Public page (scanned QR on a printed document): no login needed.
  if (loc.pathname.startsWith('/verify/')) return <Suspense fallback={<PageLoader />}><VerifyDoc /></Suspense>
  if (loading) return <div className="center-fill"><Spinner size={30} /></div>
  if (!me) return <Login />

  return (
    <Shell>
      <AnimatePresence mode="wait">
        <motion.div key={loc.pathname} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -6 }} transition={{ duration: 0.2, ease: 'easeOut' }}>
          <Routes location={loc}>
            <Route path="/" element={<Home />} />
            <Route path="/s/:key" element={<Screen />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </motion.div>
      </AnimatePresence>
    </Shell>
  )
}
