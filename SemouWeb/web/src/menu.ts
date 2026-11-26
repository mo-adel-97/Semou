import type { LucideIcon } from 'lucide-react'
import {
  Building2, Calculator, GraduationCap, Landmark, Headset, BarChart3, ShoppingBag,
} from 'lucide-react'

/**
 * The desktop ribbon (HomePage.Designer.vb), page by page and in the same order.
 *  - menu  : MainMenu.UnqName, checked with ShowUserProfile (AllFiles.MENU_VIEW__)
 *  - perm  : Form_Name.UnqName, checked with View_ (AllFiles.FORVIEW). null = always shown, like the desktop.
 *  - form  : the original WinForms class, kept for reference while porting.
 *  - ported: the screen already works in the web version.
 */
export interface MenuItem {
  key: string
  title: string
  perm: string | null
  form: string
  ported?: boolean
  /** Rendered by the metadata engine (CrudScreen) using this server screen key. */
  engine?: string
  /** Live count of requests waiting for review (sidebar badge). */
  badge?: 'reg' | 'pay' | 'study'
  /** Web-only screen (no desktop equivalent). */
  web?: boolean
  /* ---- set by the الإعدادات العامة layer (uiConfig.tsx), never in this file ---- */
  /** Icon chosen for the item. */
  icon?: LucideIcon
  /** Hidden from the sidebar, the search and the home page (the screen still opens for whoever may open it). */
  hidden?: boolean
  /** A custom link (external page) instead of a screen. */
  href?: string
  newTab?: boolean
  /** The MainMenu keys of the group the screen belongs to in the desktop (kept when it is moved to another group). */
  originMenus?: string[]
}

export interface MenuGroup {
  key: string
  title: string
  icon: LucideIcon
  /** All of these MainMenu keys must be visible. */
  menus: string[]
  items: MenuItem[]
  /** Set by الإعدادات العامة. */
  hidden?: boolean
  custom?: boolean
}

export const MENU: MenuGroup[] = [
  {
    key: 'file', title: 'ملف', icon: Building2, menus: ['file'],
    items: [
      { key: 'company-info', title: 'إعدادات النظام', perm: 'companyinfo', form: 'EncodingCompanyInfo' },
      { key: 'branches', title: 'الفروع', perm: 'addbranch', form: 'EncodingBranch', ported: true },
      { key: 'users', title: 'المستخدمين والصلاحيات', perm: 'adduser', form: 'EncodingUser', ported: true },
      { key: 'trainers', title: 'المدربين', perm: 'addtrainer', form: 'EncodingTrainer', ported: true, engine: 'trainers' },
      { key: 'salesmen', title: 'مناديب البيع', perm: 'addsalesman', form: 'EncodingSalesMan', ported: true, engine: 'salesmen' },
      { key: 'account-settings', title: 'إعدادات الحسابات', perm: 'accountsetting', form: 'EncodinAccountSetting' },
      { key: 'discount-types', title: 'أنواع الخصم', perm: 'distype', form: 'EncodingDiscountType', ported: true, engine: 'discount-types' },
      { key: 'services', title: 'الخدمات', perm: 'adddiplom', form: 'EncodingDiplom', ported: true, engine: 'services' },
      { key: 'price-list', title: 'قائمة الأسعار', perm: 'addpricelist', form: 'EncodingPriceList', ported: true },
      { key: 'offers', title: 'العروض', perm: 'pricesale', form: 'EncodingPriceSale' },
      { key: 'reg-settings', title: 'إعدادات التسجيل', perm: 'regsetting', form: 'EncodingRegSetting', ported: true },
      { key: 'reception', title: 'مكتب الاستقبال', perm: 'resdesk', form: 'MainFindStudent', ported: true },
      { key: 'my-requests', title: 'طلباتي', perm: 'webmyorders', form: '', ported: true, web: true },
      { key: 'offer-price', title: 'عرض سعر', perm: 'offerprice', form: 'EncodingOfferPrice' },
      { key: 'batch-seats', title: 'عداد الدفعات', perm: 'batchcount', form: 'BatchSeatsHome' },
      { key: 'discount-basket', title: 'سلة الخصومات', perm: 'discountcar', form: 'DisCountHome' },
      { key: 'sectors', title: 'القطاعات', perm: 'sector', form: 'EncodingCompany', ported: true, engine: 'sectors' },
      { key: 'general-settings', title: 'الإعدادات العامة', perm: 'websettings', form: '', ported: true, web: true },
      { key: 'change-password', title: 'تغيير كلمة المرور', perm: null, form: 'ChangeUserPassword', ported: true },
    ],
  },
  {
    key: 'sales', title: 'المبيعات', icon: ShoppingBag, menus: [],
    items: [
      { key: 'study-requests', title: 'طلبات الدراسة', perm: 'webstudyrequest', form: '', ported: true, web: true, badge: 'study' },
      { key: 'admission-requests', title: 'طلبات الالتحاق', perm: 'webregorder', form: '', ported: true, web: true, badge: 'reg' },
    ],
  },
  {
    key: 'account', title: 'الحسابات العامة', icon: Calculator, menus: ['account'],
    items: [
      { key: 'payment-requests', title: 'طلبات السداد', perm: 'webpayorder', form: '', ported: true, web: true, badge: 'pay' },
      { key: 'accounts', title: 'إضافة حساب', perm: 'addaccount', form: 'EncodingAccount', ported: true },
      { key: 'chart-of-accounts', title: 'دليل الحسابات', perm: 'accountchart', form: 'AccountHome', ported: true },
      { key: 'cost-centers', title: 'إضافة مركز تكلفة', perm: 'addcostcenter', form: 'EncodingCostCenter', ported: true },
      { key: 'cost-center-tree', title: 'دليل مراكز التكلفة', perm: 'centertree', form: 'CostCenterHome', ported: true },
      { key: 'cash-boxes', title: 'الخزائن', perm: 'addcashbox', form: 'EncodingCashBox', ported: true },
      { key: 'banks', title: 'البنوك', perm: 'addbank', form: 'EncodingBank', ported: true },
      { key: 'opening-entry', title: 'قيد افتتاحي', perm: 'opendaily', form: 'EncodingOpenDaily', ported: true },
      { key: 'journal-entry', title: 'قيد يومية', perm: 'daily', form: 'EncodingDaily', ported: true },
      { key: 'receipt', title: 'سند قبض', perm: 'receipt', form: 'EncodingCashDayReceipt', ported: true },
      { key: 'payment', title: 'سند صرف', perm: 'pay', form: 'EncodingCashDayPay', ported: true },
      { key: 'cash-transfer', title: 'تحويل نقدية', perm: 'transmoney', form: 'EncodingTransMoney', ported: true },
      { key: 'discount-doc', title: 'سند خصم', perm: 'discount', form: 'EncodingDiscountDoc', ported: true },
      { key: 'cash-history', title: 'حركة الخزينة - البنك', perm: 'cashhistory', form: 'CashBoxHistory', ported: true },
      { key: 'opening-balances', title: 'الأرصدة الافتتاحية', perm: 'openbalance', form: 'AccountOpenBalnceList', ported: true },
      { key: 'account-statement', title: 'كشف حساب', perm: 'statment', form: 'GeneralAccountStatment', ported: true },
      { key: 'general-journal', title: 'اليومية العامة', perm: 'dailyhome', form: 'DailyHome', ported: true },
      { key: 'trial-balance', title: 'ميزان المراجعة', perm: 'mezan', form: 'TrailHome', ported: true },
      { key: 'income-statement', title: 'قائمة الدخل', perm: 'instatment', form: 'IncomeStatment', ported: true },
    ],
  },
  {
    key: 'control', title: 'شئون الطلاب', icon: GraduationCap, menus: ['control'],
    items: [
      { key: 'batches', title: 'الدفعات', perm: 'addbatch', form: 'EncodingBatch', ported: true, engine: 'batches' },
      { key: 'batch-count', title: 'ضبط أعداد الدفعات', perm: 'addbatchcount', form: 'EncodingBatchCount', ported: true },
      { key: 'levels', title: 'المستويات', perm: 'addlevel', form: 'EncodingLevel', ported: true, engine: 'levels' },
      { key: 'reg-cases', title: 'حالات التسجيل', perm: 'addstaut', form: 'EncodingRegCases', ported: true, engine: 'reg-cases' },
      { key: 'new-course', title: 'فتح دورة جديدة', perm: 'addcourse', form: 'EncodingCourse' },
      { key: 'courses', title: 'قائمة الدورات', perm: 'allcourse', form: 'CourseHome' },
      { key: 'other-fees', title: 'الرسوم الأخرى', perm: 'addfees', form: 'EncodingOtherFees', ported: true, engine: 'other-fees' },
      { key: 'questions', title: 'أسئلة الاختبارات', perm: 'question', form: 'EncodingQuestion' },
      { key: 'exam-settings', title: 'إعداد الاختبارات', perm: 'examsetting', form: 'EncodingTestSetting' },
      { key: 'new-students', title: 'قائمة الطلاب الجدد', perm: 'newstudenthome', form: 'NewStudentHome', ported: true },
      { key: 'diploma-students', title: 'طلاب الدبلومات', perm: 'diplomstudent', form: 'LevelReportForm', ported: true },
      { key: 'course-students', title: 'طلاب الدورات', perm: 'coursestudent', form: 'StudentCourseList', ported: true },
    ],
  },
  {
    // Desktop: this ribbon page is hidden when either "student" or "toolbranch" is denied (HomePage.SHOWPROFILE).
    key: 'toolbranch', title: 'إدارة الفرع', icon: Landmark, menus: ['toolbranch', 'student'],
    items: [
      { key: 'students', title: 'قائمة الطلاب', perm: null, form: 'StudentDashBoard' },
      { key: 'work-hours', title: 'مواعيد العمل', perm: 'worktime', form: 'BranchTimeWork' },
      { key: 'classrooms', title: 'القاعات الدراسية', perm: 'classroom', form: 'EncodingCalssRoom', ported: true, engine: 'classrooms' },
      { key: 'books', title: 'المقررات الدراسية', perm: 'addbook', form: 'EncodingBook', ported: true, engine: 'books' },
      { key: 'study-plans', title: 'الخطط التدريبية', perm: 'addplan', form: 'EncodingStudyPlan' },
      { key: 'classes', title: 'تكوين شعبة', perm: 'addclass', form: 'EncodingClass' },
      { key: 'design-table', title: 'تصميم جدول', perm: 'studytable', form: 'EncodingTabel' },
      { key: 'class-table', title: 'جدول الشعب', perm: 'classtabel', form: 'ClassTabel' },
      { key: 'trainer-table', title: 'جدول مدرب', perm: 'traniertabel', form: 'TranierTabel' },
      { key: 'room-table', title: 'جدول قاعة', perm: 'roomtabel', form: 'RoomTabel' },
      { key: 'payment-order', title: 'أمر صرف نقدية', perm: 'paymentorder', form: 'EncodingPaymentOrder' },
      { key: 'receipt-order', title: 'إقرار استلام نقدية', perm: 'receiptorder', form: 'EncodingReceiptOrder' },
      { key: 'branch-daily', title: 'يومية الفرع', perm: 'branchdaliy', form: 'BranchDailyHome' },
      { key: 'contracts', title: 'اتفاقية التدريب', perm: 'contract', form: 'ContractHome' },
      { key: 'contract-follow', title: 'متابعة الاتفاقيات', perm: 'contractfollow', form: 'ContractReport' },
    ],
  },
  {
    key: 'custservices', title: 'خدمة العملاء', icon: Headset, menus: ['custservices'],
    items: [
      { key: 'old-seller-report', title: 'تقرير مبيعات المناديب', perm: null, form: 'OldSellerReport' },
      { key: 'crm', title: 'مكتب خدمة العملاء', perm: 'crm', form: 'MainCallHome' },
      { key: 'crm-report', title: 'تقرير خدمة العملاء', perm: 'crmreport', form: 'CrmDailyHome' },
    ],
  },
  {
    key: 'report', title: 'التقارير', icon: BarChart3, menus: ['report'],
    items: [
      { key: 'registrations-report', title: 'تقرير التسجيلات', perm: 'reviewdesk', form: 'RegReview', ported: true },
      { key: 'sales-report', title: 'تقرير المبيعات', perm: 'salesreport', form: 'SalesReport', ported: true },
      { key: 'tax-report', title: 'تقرير مبيعات الضرائب', perm: 'taxreport', form: 'TaxHome', ported: true },
      { key: 'tax-return-report', title: 'تقرير مرتجع مبيعات الضرائب', perm: 'taxreport', form: 'ReTaxReport', ported: true },
      { key: 'marketers-report', title: 'تقرير المسوقين', perm: 'salesmanreport', form: 'SalesManReport', ported: true },
      { key: 'payment-follow', title: 'متابعة السداد', perm: 'custbalance', form: 'CustomerHome', ported: true },
      { key: 'customer-balances', title: 'أرصدة العملاء', perm: 'customerbalance', form: 'CustomerBalanceHome', ported: true },
      { key: 'expense-analysis', title: 'تحليل المصروفات', perm: 'expenseanalysis', form: 'ExpenseAnalysisHome', ported: true },
    ],
  },
]

export const ALL_ITEMS = MENU.flatMap(g => g.items.map(i => ({ ...i, group: g })))
export const findItem = (key: string) => ALL_ITEMS.find(i => i.key === key)
