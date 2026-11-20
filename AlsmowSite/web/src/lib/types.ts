export type ProgramType = 'diploma' | 'qualifying' | 'development'
export type ProgramMode = 'onsite' | 'online' | 'hybrid'

export const PROGRAM_TYPES: Record<ProgramType, { label: string; plural: string; path: string; blurb: string }> = {
  diploma: { label: 'دبلوم', plural: 'الدبلومات المعتمدة', path: 'diplomas', blurb: 'برامج دبلوم معتمدة تؤهلك لسوق العمل بشهادة موثوقة' },
  qualifying: { label: 'دورة تأهيلية', plural: 'الدورات التأهيلية', path: 'qualifying', blurb: 'دورات تأهيلية تبني مهاراتك الأساسية خطوة بخطوة' },
  development: { label: 'دورة تطويرية', plural: 'الدورات التطويرية', path: 'development', blurb: 'دورات قصيرة لتطوير مهاراتك ومواكبة الجديد' },
}

export const PROGRAM_MODES: Record<ProgramMode, string> = { onsite: 'حضوري', online: 'عن بعد', hybrid: 'مدمج' }

export interface Program {
  id: number
  type: ProgramType
  title: string
  slug: string
  summary: string | null
  body: string | null
  image: string | null
  hours: number | null
  duration: string | null
  mode: ProgramMode | null
  badge: string | null
  features: string[]
  accreditation: string | null
  price: string | null
  is_featured: boolean
}

export interface Post {
  id: number
  title: string
  slug: string
  excerpt: string | null
  body?: string | null
  cover: string | null
  category: string | null
  published_at: string | null
}

export interface Accreditation { id: number; name: string; logo: string | null; certificate: string | null }
export interface Partner { id: number; name: string; logo: string | null; url: string | null }
export interface Agreement { id: number; title: string; partner: string | null; summary: string | null; image: string | null; signed_at: string | null }
export interface Testimonial { id: number; name: string; role: string | null; text: string; rating: number; avatar: string | null }
export interface StatItem { value: number; label: string; suffix: string }

/** One entry of the assistant's knowledge base (settings › المساعد الذكي). */
export interface KbEntry { q: string; variants: string[]; a: string; link: string | null }

export interface Settings {
  general: Partial<{ site_name: string; tagline: string; logo: string; announcement: string; announcement_link: string; announcement_cta: string; register_text: string; register_url: string; portal_hide: boolean; portal_text: string; portal_link: string; lms_url: string; theme_accent: string; theme_mode: string }>
  hero: Partial<{ eyebrow: string; title: string; highlight: string; subtitle: string; cta_text: string; cta_link: string; secondary_text: string; secondary_link: string; image: string; highlights: string[] }>
  assistant?: Partial<{ disabled: boolean; name: string | null; title: string | null; welcome: string | null; suggestions: string[]; fallback: string | null; kb: KbEntry[] }>
  intro?: Partial<{ mode: 'off' | 'first' | 'session' | 'always'; kicker: string | null; title: string | null; message: string | null; quote: string | null; skip: string | null }>
  about: Partial<{ title: string; summary: string; body: string; image: string; vision: string; mission: string; values: string[] }>
  stats: Partial<{ items: StatItem[] }>
  contact: Partial<{ phone: string; whatsapp: string; email: string; address: string; hours: string; map_url: string }>
  social: Partial<Record<'x' | 'instagram' | 'snapchat' | 'tiktok' | 'facebook' | 'linkedin' | 'youtube', string>>
  seo: Partial<{ title: string; description: string; og_image: string }>
  /** components/Butterflies.tsx: every word they show comes from here. */
  butterflies?: Partial<{
    hide: boolean; letters_hide: boolean; letters: TextPair[]; chat_hide: boolean; chat: TextPair[]
    chat_asker: string | null; chat_answerer: string | null; first: string | null; every: string | null
  }>
  /** Site texts changed without touching the code (see lib/textOverrides.ts). */
  texts?: Partial<{ rules: import('./textOverrides').TextRule[] }>
}

/** A row of a 'pairs' field: a text and a second text (a link, or an answer). */
export interface TextPair { a: string; b: string | null }

export type ProgramCounts = Record<ProgramType, number>

export interface SiteData {
  settings: Settings
  pages: { title: string; slug: string }[]
  counts: ProgramCounts
}

export interface HomeData {
  settings: Settings
  programs: Program[]
  posts: Post[]
  accreditations: Accreditation[]
  partners: Partner[]
  agreements: Agreement[]
  testimonials: Testimonial[]
  counts: ProgramCounts
}

/* ---------- Admin ---------- */

export type FieldType = 'string' | 'text' | 'html' | 'int' | 'bool' | 'enum' | 'date' | 'image' | 'url' | 'list' | 'slug' | 'stats' | 'textrules' | 'kb' | 'pairs'

export interface FieldSpec {
  key: string
  type: FieldType
  label: string
  required?: boolean
  max?: number
  min?: number
  maxItems?: number
  help?: string
  placeholder?: string
  default?: unknown
  column?: boolean
  options?: { value: string; label: string }[]
  /** 'pairs' fields: the two inputs of each row */
  labelA?: string
  labelB?: string
  placeholderA?: string
  placeholderB?: string
}

export interface ResourceSpec {
  name: string
  label: string
  singular: string
  icon: string
  title: string
  filters: string[]
  fields: FieldSpec[]
}

export interface SettingsGroupSpec {
  group: keyof Settings
  label: string
  icon: string
  fields: FieldSpec[]
}

export interface User { id: number; name: string; email: string; role: 'admin' | 'editor' }

export interface Message {
  id: number
  kind: 'contact' | 'register'
  name: string
  phone: string | null
  email: string | null
  program: string | null
  subject: string | null
  body: string | null
  is_read: boolean
  created_at: string
}

export type Row = { id: number } & Record<string, unknown>
