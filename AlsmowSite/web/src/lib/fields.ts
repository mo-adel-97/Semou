import {
  BarChart3, Bot, Briefcase, Calculator, Database, HeartPulse, Hospital, Languages, Laptop, Megaphone, Scale, Users, type LucideIcon,
} from 'lucide-react'
import type { Program } from './types'

/**
 * مجال البرنامج (set per programme in the dashboard: البرامج › مجال البرنامج). Each field has its icon, the
 * app its live scene plays in, and the default «مجالات العمل» (the programme's own «careers» wins).
 * Programmes saved before the field existed get a guess from their name until it is set.
 */
export type FieldKey = 'hr' | 'nursing' | 'health' | 'ai' | 'law' | 'office' | 'computer' | 'data' | 'language' | 'accounting' | 'marketing' | 'business'

export interface FieldInfo { icon: LucideIcon; app: string; careers: string }

export const FIELDS: Record<FieldKey, FieldInfo> = {
  hr: { icon: Users, app: 'نظام الموارد البشرية · التوظيف', careers: 'أخصائي موارد بشرية · مسؤول توظيف · شؤون موظفين' },
  nursing: { icon: HeartPulse, app: 'متابعة المريض · غرفة 204', careers: 'إدارة التمريض · منسق رعاية · مشرف أقسام' },
  health: { icon: Hospital, app: 'إدارة المستشفى · لوحة اليوم', careers: 'إدارة الخدمات الصحية · منسق مواعيد · جودة' },
  ai: { icon: Bot, app: 'مساعد الذكاء الاصطناعي', careers: 'محلل بيانات · أتمتة الأعمال · دعم تقني' },
  law: { icon: Scale, app: 'العقود · مسودة نهائية', careers: 'باحث قانوني · صياغة العقود · شؤون قانونية' },
  office: { icon: Briefcase, app: 'البريد والمواعيد', careers: 'مدير مكتب · سكرتير تنفيذي · استقبال' },
  computer: { icon: Laptop, app: 'Excel · تقرير المبيعات', careers: 'أعمال مكتبية · إدخال بيانات · دعم فني' },
  data: { icon: Database, app: 'نظام إدخال البيانات', careers: 'مدخل بيانات · أرشفة إلكترونية · خدمة عملاء' },
  language: { icon: Languages, app: 'English · محادثة', careers: 'استقبال · خدمة عملاء · مراسلات' },
  accounting: { icon: Calculator, app: 'الفواتير والقيود', careers: 'محاسب · أمين صندوق · مراجعة' },
  marketing: { icon: Megaphone, app: 'حملة إعلانية · مباشر', careers: 'أخصائي تسويق · مبيعات · إدارة حملات' },
  business: { icon: BarChart3, app: 'لوحة الأداء', careers: 'مشرف إداري · منسق مشاريع · خدمة عملاء' },
}

const GUESS: [RegExp, FieldKey][] = [
  [/مستشفي|صحي|طبي/, 'health'], [/تمريض|ممرض/, 'nursing'], [/ذكاء|اصطناعي|برمج/, 'ai'], [/قانون|محاما|انظمه/, 'law'],
  [/موارد|توظيف/, 'hr'], [/بيانات|معالجه النصوص/, 'data'], [/حاسب|حاسوب|كمبيوتر/, 'computer'], [/مكتب|سكرتار/, 'office'],
  [/انجليزي|لغه|لغات|ترجم/, 'language'], [/محاسب|مالي/, 'accounting'], [/تسويق|مبيعات/, 'marketing'],
]
const norm = (s: string) => s.replace(/[ً-ْـ]/g, '').replace(/[أإآ]/g, 'ا').replace(/ة/g, 'ه').replace(/ى/g, 'ي')

export function fieldOf(p: Pick<Program, 'title' | 'category'>): FieldKey {
  if (p.category && p.category in FIELDS) return p.category as FieldKey
  const t = norm(p.title)
  return GUESS.find(([re]) => re.test(t))?.[1] ?? 'business'
}
