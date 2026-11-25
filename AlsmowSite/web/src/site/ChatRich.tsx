import {
  ArrowLeft, Clock, ClipboardCheck, ExternalLink, GraduationCap, LayoutGrid, Mail, MapPin, MessageCircle, Phone, Timer, UserRound, Wallet,
} from 'lucide-react'
import { motion } from 'motion/react'
import { Fragment, useEffect, useMemo, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { cx } from '../lib/format'
import { useHome } from '../lib/queries'
import type { Program } from '../lib/types'

/* =====================================================================
   The assistant's answers, drawn as an app would draw them (no raw links):
   «label: link» → an action row with its icon · «**key:** value» → an info row (phone / mail tappable)
   a programme's name → a card with its image and duration · «Title:» / «**Title**» → a small heading.
   Text only in, React nodes out (never HTML).
   ===================================================================== */

type Item =
  | { k: 'link'; label: string; href: string }
  | { k: 'fact'; key: string; value: string }
  | { k: 'prog'; p: Program }
  | { k: 'text'; text: string }
type Block = { t: 'h'; text: string } | { t: 'p'; text: string } | { t: 'list'; items: Item[] }

const URL_RE = /https?:\/\/[^\s)]+/
const strip = (s: string) => s.replace(/\*\*/g, '').replace(/[:：]\s*$/, '').trim()

function classify(raw: string, progs: Map<string, Program>): Item {
  const url = raw.match(URL_RE)?.[0]
  if (url) return { k: 'link', label: strip(raw.replace(url, '')) || '', href: url }
  const bold = raw.match(/^\*\*(.+?)[:：]?\*\*[:：]?\s*(.+)$/)
  if (bold) return { k: 'fact', key: bold[1].trim(), value: strip(bold[2]) }
  const p = progs.get(strip(raw))
  if (p) return { k: 'prog', p }
  const kv = raw.match(/^([^:：]{2,22})[:：]\s+(.+)$/)
  if (kv) return { k: 'fact', key: strip(kv[1]), value: strip(kv[2]) }
  return { k: 'text', text: raw }
}

export function parseAnswer(text: string, progs: Map<string, Program>): Block[] {
  const out: Block[] = []
  const push = (it: Item) => {
    const last = out[out.length - 1]
    if (last?.t === 'list') last.items.push(it); else out.push({ t: 'list', items: [it] })
  }
  for (const line of text.split('\n')) {
    const t = line.trim()
    if (!t) continue
    const bullet = /^[-•*]\s+/.test(t)
    const body = t.replace(/^[-•*]\s+/, '')
    if (!bullet && /^\*\*[^*]+\*\*$/.test(t)) { out.push({ t: 'h', text: strip(t) }); continue }
    if (!bullet && /[:：]$/.test(t) && !URL_RE.test(t) && t.length < 60) { out.push({ t: 'h', text: strip(t) }); continue }
    if (bullet || URL_RE.test(t) || progs.has(strip(t))) { push(classify(body, progs)); continue }
    out.push({ t: 'p', text: t })
  }
  return out
}

/* ---------- where a link goes: its icon and a fallback label ---------- */

function target(href: string): { internal: string | null; icon: typeof Phone; label: string } {
  try {
    const u = new URL(href)
    const path = decodeURIComponent(u.pathname)
    const internal = u.host === location.host || /(^|\.)alsmow\.com$/.test(u.host) ? u.pathname + u.search : null
    if (u.host === 'wa.me') return { internal: null, icon: MessageCircle, label: 'واتساب' }
    if (/^\/programs\/.+/.test(path)) return { internal, icon: GraduationCap, label: 'صفحة البرنامج' }
    if (/^\/programs/.test(path)) return { internal, icon: LayoutGrid, label: 'البرامج' }
    if (/^\/register/.test(path)) return { internal, icon: ClipboardCheck, label: 'التسجيل' }
    if (/^\/contact/.test(path)) return { internal, icon: MapPin, label: 'تواصل معنا' }
    if (/^\/portal/.test(path)) return { internal, icon: UserRound, label: 'بوابة المتدربين' }
    return { internal, icon: ExternalLink, label: u.host }
  } catch {
    return { internal: null, icon: ExternalLink, label: href }
  }
}

function factIcon(key: string) {
  if (/هاتف|جوال|اتصال/.test(key)) return Phone
  if (/واتس/.test(key)) return MessageCircle
  if (/بريد|ايميل|إيميل/.test(key)) return Mail
  if (/عنوان|مقر|حضوري|قريب|بعد|بُعد/.test(key)) return MapPin
  if (/وقات|دوام|موعد/.test(key)) return Clock
  if (/رسوم|سعر|تكلفة|خصم/.test(key)) return Wallet
  if (/مدة|ساعات|مستوى/.test(key)) return Timer
  return null
}

/** **bold** inside a line; a stray link becomes a short «فتح ←» */
function Inline({ text }: { text: string }) {
  return (
    <>
      {text.split(/(\*\*[^*]+\*\*|https?:\/\/[^\s)]+)/g).map((part, i) => {
        if (/^\*\*[^*]+\*\*$/.test(part)) return <strong key={i} className="font-bold text-ink">{part.slice(2, -2)}</strong>
        if (/^https?:\/\//.test(part)) {
          const tg = target(part)
          return tg.internal
            ? <Link key={i} to={tg.internal} className="font-semibold text-brand-700">{tg.label} ←</Link>
            : <a key={i} href={part} target="_blank" rel="noopener noreferrer" className="font-semibold text-brand-700">{tg.label} ←</a>
        }
        return <Fragment key={i}>{part}</Fragment>
      })}
    </>
  )
}

function Row({ to, href, children, className }: { to?: string | null; href?: string; children: ReactNode; className: string }) {
  if (to) return <Link to={to} className={className}>{children}</Link>
  if (href) return <a href={href} target={/^https?:/.test(href) ? '_blank' : undefined} rel="noopener noreferrer" className={className}>{children}</a>
  return <div className={className}>{children}</div>
}

function ItemView({ it }: { it: Item }) {
  if (it.k === 'link') {
    const tg = target(it.href)
    return (
      <Row to={tg.internal} href={tg.internal ? undefined : it.href} className="chat-row chat-action group">
        <span className="chat-ico"><tg.icon className="h-4 w-4" /></span>
        <span className="min-w-0 flex-1 font-semibold text-ink">{it.label || tg.label}</span>
        <ArrowLeft className="chat-go h-4 w-4" />
      </Row>
    )
  }
  if (it.k === 'fact') {
    const Icon = factIcon(it.key)
    const phone = /^[+\d][\d\s-]{6,}$/.test(it.value) ? it.value.replace(/[\s-]/g, '') : null
    const mail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(it.value) ? it.value : null
    return (
      <Row href={phone ? `tel:${phone}` : mail ? `mailto:${mail}` : undefined} className={cx('chat-row chat-fact', (phone || mail) && 'group')}>
        <span className="chat-ico">{Icon ? <Icon className="h-4 w-4" /> : <i className="h-1.5 w-1.5 rounded-full bg-current" />}</span>
        <span className="min-w-0 flex-1">
          <span className="block text-[12px] font-semibold text-muted">{it.key}</span>
          <span className="block font-bold text-ink" dir={phone || mail ? 'ltr' : undefined} style={phone || mail ? { textAlign: 'right' } : undefined}>{it.value}</span>
        </span>
        {(phone || mail) && <ArrowLeft className="chat-go h-4 w-4" />}
      </Row>
    )
  }
  if (it.k === 'prog') {
    const p = it.p
    return (
      <Row to={`/programs/${p.slug}`} className="chat-row chat-prog group">
        {p.image ? <img src={p.image} alt="" loading="lazy" /> : <span className="chat-ico"><GraduationCap className="h-4 w-4" /></span>}
        <span className="min-w-0 flex-1">
          <span className="block truncate font-display font-bold text-ink">{p.title}</span>
          <span className="block truncate text-[12px] text-muted">{[p.duration, p.hours ? `${p.hours} ساعة` : null].filter(Boolean).join(' · ') || ' '}</span>
        </span>
        <span className="chat-pill">التفاصيل</span>
      </Row>
    )
  }
  return (
    <div className="chat-row chat-text">
      <span className="mt-[.55em] h-1.5 w-1.5 shrink-0 rounded-full bg-brand-500" />
      <span className="min-w-0 flex-1"><Inline text={it.text} /></span>
    </div>
  )
}

/** The answer, its parts arriving one after another (like a real conversation). */
export default function ChatRich({ text, reveal, onDone }: { text: string; reveal: boolean; onDone: () => void }) {
  const home = useHome()
  const progs = useMemo(() => new Map((home.data?.programs ?? []).map((p) => [p.title.trim(), p])), [home.data])
  const blocks = useMemo(() => parseAnswer(text, progs), [text, progs])
  useEffect(() => {
    if (!reveal) return
    const t = setTimeout(onDone, 250 + blocks.length * 110)
    return () => clearTimeout(t)
  }, [reveal]) // eslint-disable-line react-hooks/exhaustive-deps
  return (
    <div className="space-y-2.5">
      {blocks.map((b, i) => (
        <motion.div
          key={i}
          initial={reveal ? { opacity: 0, y: 8 } : false}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: reveal ? i * 0.11 : 0, duration: 0.35, ease: [0.22, 1, 0.36, 1] }}
        >
          {b.t === 'h' && <div className="chat-h">{b.text}</div>}
          {b.t === 'p' && <p className="leading-relaxed"><Inline text={b.text} /></p>}
          {b.t === 'list' && (
            <div className={cx('chat-list', b.items.every((x) => x.k === 'text') && 'plain')}>
              {b.items.map((it, j) => <ItemView key={j} it={it} />)}
            </div>
          )}
        </motion.div>
      ))}
    </div>
  )
}
