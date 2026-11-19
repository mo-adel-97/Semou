import { ArrowUp, RotateCcw, Sparkles, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { Fragment, useEffect, useRef, useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { cx } from '../lib/format'
import { useSite } from '../lib/queries'
import { track } from '../lib/track'

/* «تحدث إلى السمو»: answered by the institute's own assistant on the server (api/lib/assistant.php). */

type Msg = { role: 'user' | 'assistant'; content: string; suggestions?: string[] }
const KEY = 'alsmow-chat.v2'
const DEFAULT_SUGGESTIONS = ['ما البرامج المتاحة؟', 'كيف أسجل؟', 'هل الشهادات معتمدة؟', 'أين مقركم وما أوقات العمل؟']

function load(): Msg[] {
  try {
    return JSON.parse(sessionStorage.getItem(KEY) || '[]') as Msg[]
  } catch {
    return []
  }
}

/** Tiny safe markdown: **bold**, links, "- " bullets, line breaks. Produces React nodes, never HTML. */
function Rich({ text }: { text: string }) {
  const inline = (s: string, k: string): ReactNode[] =>
    s.split(/(\*\*[^*]+\*\*|https?:\/\/[^\s)]+)/g).map((part, i) => {
      if (/^\*\*[^*]+\*\*$/.test(part)) return <strong key={k + i} className="font-bold text-ink">{part.slice(2, -2)}</strong>
      if (/^https?:\/\//.test(part)) {
        let internal: string | null = null
        let label = 'فتح الرابط ←'
        try {
          const u = new URL(part)
          if (u.host === location.host) internal = u.pathname + u.search
          if (u.host === 'wa.me') label = 'مراسلتنا على واتساب ←'
        } catch { /* not a URL */ }
        return internal ? (
          <Link key={k + i} to={internal} className="font-semibold text-brand-700 underline underline-offset-2">فتح الصفحة ←</Link>
        ) : (
          <a key={k + i} href={part} target="_blank" rel="noopener noreferrer" className="font-semibold break-all text-brand-700 underline underline-offset-2">{label}</a>
        )
      }
      return <Fragment key={k + i}>{part}</Fragment>
    })
  return (
    <div className="space-y-1.5">
      {text.split('\n').map((l, i) => {
        const t = l.trim()
        if (!t) return null
        if (/^[-•*]\s+/.test(t)) return <div key={i} className="flex gap-2"><span className="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-brand-500" /><span>{inline(t.replace(/^[-•*]\s+/, ''), `l${i}`)}</span></div>
        return <p key={i}>{inline(t, `p${i}`)}</p>
      })}
    </div>
  )
}

/** Reveals an answer word by word (feels like a conversation, costs nothing). */
function useTypewriter(full: string, active: boolean, onDone: () => void) {
  const [n, setN] = useState(active ? 0 : full.length)
  useEffect(() => {
    if (!active) { setN(full.length); return }
    setN(0)
    let i = 0
    const words = full.split(/(\s+)/)
    const t = setInterval(() => {
      i += 2
      const shown = words.slice(0, i).join('')
      setN(shown.length)
      if (i >= words.length) { clearInterval(t); onDone() }
    }, 28)
    return () => clearInterval(t)
  }, [full, active]) // eslint-disable-line react-hooks/exhaustive-deps
  return full.slice(0, n)
}

function Answer({ m, typing, onTyped }: { m: Msg; typing: boolean; onTyped: () => void }) {
  const text = useTypewriter(m.content, typing, onTyped)
  return <Rich text={text} />
}

export default function ChatWidget() {
  const { data } = useSite()
  const cfg = data?.settings.assistant ?? {}
  const name = cfg.name?.trim() || 'السمو'
  const title = cfg.title?.trim() || 'تحدث إلى السمو'
  const welcome = cfg.welcome?.trim() || `أهلًا بكِ 👋 أنا ${name}، مساعدتك في المعهد. اسأليني عن البرامج والدبلومات، التسجيل، الرسوم، المواعيد، أو السياسات.`
  const starters = cfg.suggestions?.length ? cfg.suggestions : DEFAULT_SUGGESTIONS

  const [open, setOpen] = useState(false)
  const [msgs, setMsgs] = useState<Msg[]>(load)
  const [input, setInput] = useState('')
  const [busy, setBusy] = useState(false)
  const [typingAt, setTypingAt] = useState(-1)
  const [error, setError] = useState<string | null>(null)
  const scroller = useRef<HTMLDivElement>(null)
  const abort = useRef<AbortController | null>(null)
  // introduce itself once (label visible for a few seconds), then stay a compact icon
  const [peek, setPeek] = useState(false)
  useEffect(() => {
    const a = setTimeout(() => setPeek(true), 1800)
    const b = setTimeout(() => setPeek(false), 7000)
    return () => { clearTimeout(a); clearTimeout(b) }
  }, [])

  useEffect(() => {
    try { sessionStorage.setItem(KEY, JSON.stringify(msgs.slice(-30))) } catch { /* ignore */ }
  }, [msgs])
  useEffect(() => {
    const el = scroller.current
    if (!el) return
    const t = setInterval(() => el.scrollTo({ top: el.scrollHeight }), 120)
    const stop = setTimeout(() => clearInterval(t), typingAt >= 0 ? 6000 : 300)
    return () => { clearInterval(t); clearTimeout(stop) }
  }, [msgs, typingAt, open])
  useEffect(() => () => abort.current?.abort(), [])

  if (cfg.disabled === true) return null

  const send = async (text: string) => {
    const q = text.trim()
    if (!q || busy) return
    setError(null)
    setInput('')
    track('chat_message')
    const history = msgs.slice(-8).map(({ role, content }) => ({ role, content }))
    const next: Msg[] = [...msgs, { role: 'user', content: q }]
    setMsgs([...next, { role: 'assistant', content: '' }])
    setBusy(true)
    const ctrl = new AbortController()
    abort.current = ctrl
    try {
      const res = await fetch('/api/public/chat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'fetch', Accept: 'application/json' },
        body: JSON.stringify({ q, history }),
        signal: ctrl.signal,
      })
      const j = (await res.json().catch(() => null)) as { answer?: string; suggestions?: string[]; message?: string } | null
      if (!res.ok || !j?.answer) throw new Error(j?.message ?? 'تعذر الوصول إلى المساعد، حاولي مرة أخرى')
      setMsgs([...next, { role: 'assistant', content: j.answer, suggestions: j.suggestions ?? [] }])
      setTypingAt(next.length)
    } catch (e) {
      if ((e as Error).name === 'AbortError') return
      setMsgs(next.slice(0, -1))
      setInput(q)
      setError((e as Error).message)
    } finally {
      setBusy(false)
    }
  }

  const last = msgs[msgs.length - 1]
  const followUps = !busy && typingAt === -1 && last?.role === 'assistant' && last.content ? (last.suggestions ?? []) : []

  return (
    <>
      {/* Launcher */}
      <AnimatePresence>
        {!open && (
          <motion.button
            initial={{ opacity: 0, scale: 0.6, y: 30 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.6, y: 30 }}
            transition={{ type: 'spring', delay: 0.6 }}
            whileHover={{ scale: 1.04 }}
            whileTap={{ scale: 0.96 }}
            onClick={() => { setOpen(true); track('chat_open') }}
            onHoverStart={() => setPeek(true)}
            onHoverEnd={() => setPeek(false)}
            className="fixed right-4 bottom-4 z-40 flex items-center rounded-full bg-deep-900 p-1.5 text-white shadow-[0_18px_40px_-12px_rgba(0,0,0,.5)] sm:right-5 sm:bottom-5"
            aria-label={title}
            title={title}
          >
            <span className="relative grid h-12 w-12 place-items-center rounded-full bg-gradient-to-br from-brand-400 via-brand-500 to-gold-400">
              <span className="absolute inset-0 animate-ping rounded-full bg-brand-400 opacity-30" />
              <Sparkles className="relative h-5 w-5" />
            </span>
            <motion.span
              initial={false}
              animate={{ width: peek ? 'auto' : 0, opacity: peek ? 1 : 0 }}
              transition={{ duration: 0.35, ease: [0.22, 1, 0.36, 1] }}
              className="overflow-hidden text-start leading-tight whitespace-nowrap"
            >
              <span className="block ps-3 pe-4 font-display text-[15px] font-bold">{title}</span>
              <span className="block ps-3 pe-4 text-[11px] text-white/60">مساعدتك في المعهد · متاحة الآن</span>
            </motion.span>
          </motion.button>
        )}
      </AnimatePresence>

      {/* Panel */}
      <AnimatePresence>
        {open && (
          <motion.section
            initial={{ opacity: 0, y: 40, scale: 0.96 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 40, scale: 0.96 }}
            transition={{ type: 'spring', damping: 28, stiffness: 280 }}
            className="fixed inset-x-3 bottom-3 z-[65] flex h-[min(42rem,calc(100dvh-1.5rem))] origin-bottom-right flex-col overflow-hidden rounded-[2rem] border border-line bg-bg shadow-[0_40px_80px_-20px_rgba(0,0,0,.45)] sm:inset-x-auto sm:right-5 sm:bottom-5 sm:w-[25rem]"
            aria-label={title}
            data-lenis-prevent
          >
            <header className="relative overflow-hidden bg-deep-900 px-5 pt-5 pb-6 text-white">
              <div className="pattern-light absolute inset-0" />
              <div className="absolute -top-16 -left-10 h-40 w-40 rounded-full bg-brand-500/50 blur-3xl" />
              <div className="relative flex items-center gap-3">
                <span className="grid h-12 w-12 place-items-center rounded-2xl bg-gradient-to-br from-brand-400 to-gold-400 shadow-lg"><Sparkles className="h-6 w-6" /></span>
                <div className="flex-1">
                  <h2 className="font-display text-lg font-extrabold">{title}</h2>
                  <p className="flex items-center gap-1.5 text-xs text-white/65"><span className="h-2 w-2 rounded-full bg-emerald-400" /> تجيبك من معلومات المعهد مباشرة</p>
                </div>
                {msgs.length > 0 && (
                  <button onClick={() => { abort.current?.abort(); setMsgs([]); setError(null); setTypingAt(-1) }} className="rounded-xl p-2 text-white/70 hover:bg-white/10 hover:text-white" title="محادثة جديدة" aria-label="محادثة جديدة">
                    <RotateCcw className="h-4 w-4" />
                  </button>
                )}
                <button onClick={() => setOpen(false)} className="rounded-xl p-2 text-white/70 hover:bg-white/10 hover:text-white" aria-label="إغلاق"><X className="h-5 w-5" /></button>
              </div>
            </header>

            <div ref={scroller} className="-mt-3 flex-1 space-y-4 overflow-y-auto rounded-t-[1.5rem] bg-bg px-4 pt-5 pb-4">
              {msgs.length === 0 && (
                <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.15 }}>
                  <div className="rounded-3xl rounded-tr-md border border-line bg-surface p-4 text-[15px] leading-relaxed text-ink-2 shadow-soft">{welcome}</div>
                  <Chips items={starters} onPick={send} />
                </motion.div>
              )}
              {msgs.map((m, i) => (
                <motion.div key={i} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className={cx('flex', m.role === 'user' ? 'justify-start' : 'justify-end')}>
                  <div className={cx('max-w-[88%] rounded-3xl px-4 py-3 text-[15px] leading-relaxed', m.role === 'user' ? 'rounded-tr-md bg-primary text-white' : 'rounded-tl-md border border-line bg-surface text-ink-2 shadow-soft')}>
                    {m.role === 'assistant' && !m.content ? (
                      <span className="flex gap-1 py-1.5" aria-label="تكتب الآن">
                        {[0, 1, 2].map((d) => (
                          <motion.span key={d} className="h-2 w-2 rounded-full bg-brand-500" animate={{ y: [0, -5, 0], opacity: [0.4, 1, 0.4] }} transition={{ duration: 0.9, repeat: Infinity, delay: d * 0.15 }} />
                        ))}
                      </span>
                    ) : m.role === 'assistant' ? (
                      <Answer m={m} typing={i === typingAt} onTyped={() => setTypingAt(-1)} />
                    ) : (
                      m.content
                    )}
                  </div>
                </motion.div>
              ))}
              {followUps.length > 0 && <Chips items={followUps} onPick={send} />}
              {error && <p className="rounded-2xl bg-red-50 px-4 py-3 text-sm font-semibold text-red-700 dark:bg-red-500/10 dark:text-red-300">{error}</p>}
            </div>

            <form onSubmit={(e) => { e.preventDefault(); send(input) }} className="border-t border-line bg-surface p-3">
              <div className="flex items-end gap-2 rounded-2xl border border-line bg-bg p-1.5 ps-4 focus-within:border-brand-400 focus-within:ring-4 focus-within:ring-brand-500/10">
                <textarea
                  value={input}
                  onChange={(e) => setInput(e.target.value)}
                  onKeyDown={(e) => { if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); send(input) } }}
                  rows={1}
                  maxLength={400}
                  placeholder="اكتبي سؤالك..."
                  className="max-h-28 min-h-10 flex-1 resize-none bg-transparent py-2 text-[15px] text-ink placeholder:text-muted focus:outline-none"
                />
                <button type="submit" disabled={busy || !input.trim()} className="grid h-10 w-10 shrink-0 place-items-center rounded-xl bg-primary text-white transition disabled:opacity-40" aria-label="إرسال">
                  <ArrowUp className="h-5 w-5" />
                </button>
              </div>
              <p className="mt-2 text-center text-[11px] text-muted">الإجابات من معلومات المعهد المنشورة · للتأكيد تواصلي مع فريق القبول</p>
            </form>
          </motion.section>
        )}
      </AnimatePresence>
    </>
  )
}

function Chips({ items, onPick }: { items: string[]; onPick: (s: string) => void }) {
  return (
    <div className="mt-1 flex flex-wrap justify-end gap-2">
      {items.map((s, i) => (
        <motion.button
          key={s}
          initial={{ opacity: 0, y: 8 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 + i * 0.05 }}
          onClick={() => onPick(s)}
          className="rounded-full border border-brand-200 bg-brand-50 px-3.5 py-2 text-sm font-semibold text-brand-700 transition hover:bg-primary hover:text-white"
        >
          {s}
        </motion.button>
      ))}
    </div>
  )
}
