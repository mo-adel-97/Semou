import type { LucideIcon } from 'lucide-react'
import {
  Activity, Archive, Award, Ban, Banknote, BarChart3, BadgeCheck, Bell, Book, BookOpen, Bot, Boxes, Brain, Briefcase, Building, Building2, Bus,
  Calculator, Calendar, CalendarDays, Camera, ChartColumn, ChartLine, ChartPie, CircleCheck, CircleDollarSign, CircleHelp, CircleX, Clipboard,
  ClipboardCheck, ClipboardList, Clock, Cog, Coins, Contact, Cpu, CreditCard, Crown, Database, DoorOpen, Download, ExternalLink, Factory,
  FileCheck, FilePen, FilePlus, FileSpreadsheet, FileText, Files, Fingerprint, Flag, Folder, FolderOpen, Gavel, Gem, Gift, Globe,
  GraduationCap, Grid3x3, HandCoins, HandHelping, Handshake, Headset, HeartPulse, History, Home, Hospital, Hourglass, IdCard, Image, Inbox, Info,
  Key, KeyRound, Landmark, Laptop, Layers, LayoutDashboard, LayoutGrid, LayoutList, Leaf, Library, Lightbulb, Link, ListChecks, ListOrdered,
  Lock, Mail, Map as MapIcon, MapPin, Medal, Megaphone, MessageSquare, Monitor, Network, Newspaper, NotebookPen, NotebookText, Package, Palette,
  PenLine, Percent, Phone, PiggyBank, Presentation, Printer, Puzzle, QrCode, Receipt, ReceiptText, Repeat, Rocket, Scale, School, Search,
  Send, Server, Settings, Settings2, Share2, ShieldCheck, ShoppingBag, ShoppingCart, SlidersHorizontal, Smartphone, Sparkles, SquareKanban,
  Stamp, Star, Stethoscope, Store, Table2, Tag, Tags, Target, Timer, TrendingUp, TriangleAlert, Trophy, Truck, UploadCloud, User, UserCheck,
  UserCog, UserPlus, Users, UsersRound, Wallet, WandSparkles, Warehouse, Workflow, Wrench, Zap,
} from 'lucide-react'

/** One icon of the library: the component and Arabic words to find it by. */
export interface IconDef { name: string; icon: LucideIcon; words: string }

const def = (icon: LucideIcon, name: string, words: string): IconDef => ({ icon, name, words })

/**
 * The icons the sidebar can use (chosen in الإعدادات العامة), grouped for the picker.
 * Saved settings store the NAME, so an icon must never be renamed or removed from this list.
 */
export const ICON_GROUPS: { title: string; icons: IconDef[] }[] = [
  { title: 'عام', icons: [
    def(LayoutDashboard, 'LayoutDashboard', 'لوحة الرئيسية'), def(Home, 'Home', 'بيت رئيسية'), def(LayoutGrid, 'LayoutGrid', 'شبكة أقسام'),
    def(LayoutList, 'LayoutList', 'قائمة'), def(SquareKanban, 'SquareKanban', 'لوحة مهام'), def(Layers, 'Layers', 'طبقات'), def(Grid3x3, 'Grid3x3', 'جدول شبكة'),
    def(Star, 'Star', 'نجمة مفضلة'), def(Sparkles, 'Sparkles', 'جديد مميز'), def(Zap, 'Zap', 'سريع'), def(Rocket, 'Rocket', 'انطلاق'),
    def(Flag, 'Flag', 'علم'), def(Target, 'Target', 'هدف'), def(Lightbulb, 'Lightbulb', 'فكرة'), def(Puzzle, 'Puzzle', 'إضافات'),
    def(Search, 'Search', 'بحث'), def(Info, 'Info', 'معلومات'), def(CircleHelp, 'CircleHelp', 'مساعدة'), def(Bell, 'Bell', 'تنبيهات'),
  ]},
  { title: 'المؤسسة والإدارة', icons: [
    def(Building2, 'Building2', 'مبنى شركة ملف'), def(Building, 'Building', 'مبنى'), def(Landmark, 'Landmark', 'إدارة فرع بنك'), def(School, 'School', 'مدرسة معهد'),
    def(Factory, 'Factory', 'مصنع'), def(Warehouse, 'Warehouse', 'مستودع'), def(Store, 'Store', 'متجر'), def(DoorOpen, 'DoorOpen', 'قاعة باب'),
    def(Briefcase, 'Briefcase', 'عمل حقيبة'), def(Network, 'Network', 'شبكة فروع'), def(Workflow, 'Workflow', 'سير عمل'), def(Crown, 'Crown', 'إدارة عليا'),
    def(Settings, 'Settings', 'إعدادات'), def(Settings2, 'Settings2', 'ضبط'), def(SlidersHorizontal, 'SlidersHorizontal', 'تحكم'), def(Cog, 'Cog', 'ترس'), def(Wrench, 'Wrench', 'أدوات صيانة'),
  ]},
  { title: 'الطلاب والتعليم', icons: [
    def(GraduationCap, 'GraduationCap', 'طلاب تخرج شئون'), def(BookOpen, 'BookOpen', 'كتاب مقرر'), def(Book, 'Book', 'كتاب'), def(Library, 'Library', 'مكتبة'),
    def(NotebookText, 'NotebookText', 'دفتر'), def(NotebookPen, 'NotebookPen', 'تسجيل دفتر'), def(Presentation, 'Presentation', 'دورة عرض تدريب'), def(Award, 'Award', 'شهادة'),
    def(Trophy, 'Trophy', 'كأس'), def(Medal, 'Medal', 'ميدالية'), def(Brain, 'Brain', 'اختبار عقل'), def(ClipboardCheck, 'ClipboardCheck', 'اختبارات'),
    def(Stethoscope, 'Stethoscope', 'طبي'), def(HeartPulse, 'HeartPulse', 'صحة'), def(Hospital, 'Hospital', 'مستشفى'),
  ]},
  { title: 'الأشخاص', icons: [
    def(Users, 'Users', 'مستخدمين طلاب أشخاص'), def(UsersRound, 'UsersRound', 'مجموعة'), def(User, 'User', 'شخص'), def(UserPlus, 'UserPlus', 'إضافة طالب تسجيل'),
    def(UserCheck, 'UserCheck', 'قبول'), def(UserCog, 'UserCog', 'صلاحيات'), def(Contact, 'Contact', 'جهة اتصال'), def(IdCard, 'IdCard', 'هوية بطاقة'),
    def(Headset, 'Headset', 'خدمة عملاء'), def(Handshake, 'Handshake', 'اتفاقية شراكة'), def(HandHelping, 'HandHelping', 'دعم'), def(Bot, 'Bot', 'آلي'),
  ]},
  { title: 'المالية والمبيعات', icons: [
    def(Calculator, 'Calculator', 'حسابات'), def(Wallet, 'Wallet', 'محفظة سداد'), def(Banknote, 'Banknote', 'نقدية'), def(Coins, 'Coins', 'عملات'),
    def(CircleDollarSign, 'CircleDollarSign', 'مال'), def(CreditCard, 'CreditCard', 'بطاقة دفع'), def(PiggyBank, 'PiggyBank', 'ادخار'), def(HandCoins, 'HandCoins', 'تحصيل'),
    def(Receipt, 'Receipt', 'فاتورة سند'), def(ReceiptText, 'ReceiptText', 'إيصال'), def(Percent, 'Percent', 'خصم ضريبة'), def(Scale, 'Scale', 'ميزان'),
    def(ShoppingBag, 'ShoppingBag', 'مبيعات'), def(ShoppingCart, 'ShoppingCart', 'سلة'), def(Tag, 'Tag', 'سعر'), def(Tags, 'Tags', 'أسعار'), def(Gift, 'Gift', 'عرض هدية'),
  ]},
  { title: 'التقارير والمستندات', icons: [
    def(BarChart3, 'BarChart3', 'تقارير'), def(ChartColumn, 'ChartColumn', 'أعمدة'), def(ChartLine, 'ChartLine', 'خط'), def(ChartPie, 'ChartPie', 'دائرة نسب'),
    def(TrendingUp, 'TrendingUp', 'نمو'), def(Activity, 'Activity', 'نشاط'), def(FileText, 'FileText', 'مستند'), def(Files, 'Files', 'ملفات'),
    def(FileSpreadsheet, 'FileSpreadsheet', 'إكسل جدول'), def(FileCheck, 'FileCheck', 'معتمد'), def(FilePlus, 'FilePlus', 'جديد'), def(FilePen, 'FilePen', 'تعديل'),
    def(ClipboardList, 'ClipboardList', 'قائمة مهام'), def(Clipboard, 'Clipboard', 'حافظة'), def(ListChecks, 'ListChecks', 'متابعة'), def(ListOrdered, 'ListOrdered', 'ترتيب'),
    def(Table2, 'Table2', 'جدول'), def(Folder, 'Folder', 'مجلد'), def(FolderOpen, 'FolderOpen', 'مجلد مفتوح'), def(Archive, 'Archive', 'أرشيف'), def(Inbox, 'Inbox', 'طلبات وارد'),
    def(Printer, 'Printer', 'طباعة'), def(Stamp, 'Stamp', 'ختم'), def(Gavel, 'Gavel', 'قانون'), def(PenLine, 'PenLine', 'توقيع'), def(Newspaper, 'Newspaper', 'أخبار'),
  ]},
  { title: 'الوقت والتواصل', icons: [
    def(Calendar, 'Calendar', 'تقويم'), def(CalendarDays, 'CalendarDays', 'جدول مواعيد'), def(Clock, 'Clock', 'ساعة مواعيد'), def(Timer, 'Timer', 'مؤقت'),
    def(Hourglass, 'Hourglass', 'انتظار'), def(History, 'History', 'سجل'), def(Repeat, 'Repeat', 'تكرار'), def(Mail, 'Mail', 'بريد'), def(Send, 'Send', 'إرسال'),
    def(MessageSquare, 'MessageSquare', 'رسائل'), def(Phone, 'Phone', 'هاتف'), def(Smartphone, 'Smartphone', 'جوال'), def(Megaphone, 'Megaphone', 'إعلان'), def(Share2, 'Share2', 'مشاركة'),
  ]},
  { title: 'تقنية وأمان', icons: [
    def(ShieldCheck, 'ShieldCheck', 'أمان حماية'), def(Lock, 'Lock', 'قفل'), def(Key, 'Key', 'مفتاح'), def(KeyRound, 'KeyRound', 'كلمة مرور'), def(Fingerprint, 'Fingerprint', 'بصمة'),
    def(QrCode, 'QrCode', 'رمز'), def(Database, 'Database', 'قاعدة بيانات'), def(Server, 'Server', 'خادم'), def(Cpu, 'Cpu', 'معالج'), def(Monitor, 'Monitor', 'شاشة'),
    def(Laptop, 'Laptop', 'حاسب'), def(Link, 'Link', 'رابط'), def(ExternalLink, 'ExternalLink', 'رابط خارجي'), def(Globe, 'Globe', 'موقع إنترنت'),
    def(Download, 'Download', 'تنزيل'), def(UploadCloud, 'UploadCloud', 'رفع'), def(Image, 'Image', 'صورة'), def(Camera, 'Camera', 'كاميرا'), def(Palette, 'Palette', 'ألوان'),
  ]},
  { title: 'أخرى', icons: [
    def(MapPin, 'MapPin', 'موقع'), def(MapIcon, 'Map', 'خريطة'), def(Truck, 'Truck', 'نقل'), def(Bus, 'Bus', 'حافلة'), def(Package, 'Package', 'شحنة'), def(Boxes, 'Boxes', 'مخزون'),
    def(Gem, 'Gem', 'جوهرة'), def(WandSparkles, 'WandSparkles', 'سحر'), def(Leaf, 'Leaf', 'ورقة'), def(BadgeCheck, 'BadgeCheck', 'توثيق'), def(CircleCheck, 'CircleCheck', 'تم'),
    def(CircleX, 'CircleX', 'إلغاء'), def(TriangleAlert, 'TriangleAlert', 'تحذير'), def(Ban, 'Ban', 'منع'),
  ]},
]

const BY_NAME: Record<string, LucideIcon> = Object.fromEntries(ICON_GROUPS.flatMap(g => g.icons.map(i => [i.name, i.icon])))
const NAME_OF = new Map<LucideIcon, string>(ICON_GROUPS.flatMap(g => g.icons.map(i => [i.icon, i.name] as const)))

export const iconByName = (name?: string | null): LucideIcon | undefined => (name ? BY_NAME[name] : undefined)
export const nameOfIcon = (icon?: LucideIcon): string | undefined => (icon ? NAME_OF.get(icon) : undefined)
