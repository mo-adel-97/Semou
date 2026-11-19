import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Bot, ChevronDown, FileUp, HelpCircle, Loader2, MessageSquarePlus, Plus, Search, Send, Trash2, X } from 'lucide-react'
import { AnimatePresence, motion } from 'motion/react'
import { useMemo, useRef, useState } from 'react'
import { confirmDanger, toast } from '../lib/alerts'
import { api } from '../lib/api'
import { cx } from '../lib/format'
import type { KbEntry } from '../lib/types'

/**
 * The assistant's knowledge base: questions + answers written by the institute.
 * The assistant already knows the site's programs, policies and contact details; this adds everything else
 * (fees, requirements, schedules, offers…). Includes a live tester and bulk import.
 */
export default function KbField({ value, onChange, max }: { value: KbEntry[]; onChange: (v: KbEntry[]) => void; max: number }) {
  const [q, setQ] = useState('')
  const [open, setOpen] = useState<number | null>(null)
  const [importing, setImporting] = useState(false)
  const list = value ?? []
  const shown = useMemo(
    () => list.map((e, i) => ({ e, i })).filter(({ e }) => !q.trim() || `${e.q} ${e.a} ${(e.variants ?? []).join(' ')}`.includes(q.trim())),
    [list, q],
  )

  const set = (i: number, patch: Partial<KbEntry>) => onChange(list.map((x, j) => (j === i ? { ...x, ...patch } : x)))
  const add = () => {
    if (list.length >= max) return toast.error('وصلتِ للحد الأقصى من الأسئلة')
    onChange([{ q: '', variants: [], a: '', link: null }, ...list])
    setOpen(0)
    setQ('')
  }
  const remove = async (i: number) => {
    if (await confirmDanger('حذف هذا السؤال؟', list[i].q || undefined)) onChange(list.filter((_, j) => j !== i))
  }

  const addFrom = (question: string) => {
    if (list.length >= max) return toast.error('وصلتِ للحد الأقصى من الأسئلة')
    onChange([{ q: question, variants: [], a: '', link: null }, ...list])
    setOpen(0)
    setQ('')
  }

  return (
    <div className="space-y-4">
      <div className="rounded-2xl bg-brand-50 px-4 py-3 text-sm text-ink-2">
        <b className="text-brand-700">يعرف المساعد تلقائيًا:</b> كل البرامج وتفاصيلها، صفحات السياسات، النبذة، بيانات التواصل والعنوان وأوقات العمل، الإعلان الحالي، الإحصائيات، الشركاء، و<b>أكثر من 40 موضوعًا شائعًا</b> (التقسيط، الشروط، السكن، الشهادات، التوظيف…) ومعلومات عامة عن التدريب في المملكة (التدريب الأهلي، التحقق من الترخيص، هدف، تمهير، دروب) إضافة إلى الدردشة العادية. ما تضيفينه هنا له الأولوية على كل ذلك.
      </div>
      <Unanswered onAdd={addFrom} />
      <Tester kb={list} />

      <div className="flex flex-wrap items-center gap-2">
        <div className="relative min-w-0 flex-1 sm:max-w-xs">
          <Search className="pointer-events-none absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2 text-muted" />
          <input className="input !pr-9" placeholder="بحث في الأسئلة…" value={q} onChange={(e) => setQ(e.target.value)} />
        </div>
        <span className="text-sm text-muted">{list.length.toLocaleString('ar-EG')} سؤال</span>
        <span className="flex-1" />
        <button type="button" onClick={() => setImporting(true)} className="btn-ghost !px-3.5 !py-2 text-sm"><FileUp className="h-4 w-4" /> استيراد</button>
        <button type="button" onClick={add} className="btn-primary !px-4 !py-2 text-sm"><Plus className="h-4 w-4" /> سؤال جديد</button>
      </div>

      {list.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-line p-8 text-center text-sm text-muted">
          لا توجد أسئلة بعد. المساعد يجيب الآن من البرامج والسياسات وبيانات التواصل؛ أضيفي هنا أي معلومة أخرى (الرسوم، الشروط، المواعيد، العروض…) أو استوردي ملفًا كاملًا.
        </div>
      ) : (
        <ul className="space-y-2">
          {shown.map(({ e, i }) => {
            const isOpen = open === i
            return (
              <li key={i} className={cx('overflow-hidden rounded-2xl border transition', isOpen ? 'border-brand-300 bg-surface shadow-soft' : 'border-line bg-surface')}>
                <button type="button" onClick={() => setOpen(isOpen ? null : i)} className="flex w-full items-center gap-3 px-4 py-3 text-right">
                  <MessageSquarePlus className="h-4 w-4 shrink-0 text-brand-600" />
                  <span className="min-w-0 flex-1">
                    <span className={cx('block truncate font-semibold', e.q ? 'text-ink' : 'text-muted')}>{e.q || 'سؤال جديد'}</span>
                    {!isOpen && e.a && <span className="block truncate text-sm text-muted">{e.a}</span>}
                  </span>
                  {(e.variants?.length ?? 0) > 0 && <span className="rounded-full bg-bg px-2 py-0.5 text-xs text-muted">+{e.variants.length} صيغة</span>}
                  <ChevronDown className={cx('h-4 w-4 shrink-0 text-muted transition', isOpen && 'rotate-180')} />
                </button>
                <AnimatePresence initial={false}>
                  {isOpen && (
                    <motion.div initial={{ height: 0 }} animate={{ height: 'auto' }} exit={{ height: 0 }} className="overflow-hidden">
                      <div className="space-y-3 border-t border-line p-4">
                        <div>
                          <label className="label">السؤال</label>
                          <input className="input" maxLength={300} value={e.q} onChange={(ev) => set(i, { q: ev.target.value })} placeholder="مثال: كم رسوم دبلوم إدارة الأعمال؟" autoFocus />
                        </div>
                        <div>
                          <label className="label">صيغ أخرى للسؤال <span className="font-normal text-muted">(كل صيغة في سطر، بالعامية أو الفصحى)</span></label>
                          <textarea
                            className="input min-h-20 resize-y"
                            rows={3}
                            value={(e.variants ?? []).join('\n')}
                            onChange={(ev) => set(i, { variants: ev.target.value.split('\n').map((x) => x.trimStart()).slice(0, 20) })}
                            placeholder={'بكام دبلوم البيزنس؟\nسعر دبلوم ادارة الاعمال'}
                          />
                        </div>
                        <div>
                          <label className="label">الإجابة</label>
                          <textarea className="input min-h-28 resize-y" rows={5} maxLength={3000} value={e.a} onChange={(ev) => set(i, { a: ev.target.value })} placeholder="اكتبي الإجابة كما تريدين أن يقرأها الزائر. للنقاط ابدئي السطر بـ - وللخط العريض **هكذا**." />
                        </div>
                        <div className="grid gap-3 sm:grid-cols-[1fr_auto] sm:items-end">
                          <div>
                            <label className="label">رابط يظهر مع الإجابة <span className="font-normal text-muted">(اختياري)</span></label>
                            <input className="input text-left" dir="ltr" value={e.link ?? ''} onChange={(ev) => set(i, { link: ev.target.value || null })} placeholder="/register أو https://…" />
                          </div>
                          <button type="button" onClick={() => remove(i)} className="btn-ghost !px-4 !py-2.5 text-sm text-red-600 hover:!bg-red-500/10"><Trash2 className="h-4 w-4" /> حذف</button>
                        </div>
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>
              </li>
            )
          })}
        </ul>
      )}
      <AnimatePresence>{importing && <ImportDialog onClose={() => setImporting(false)} onImport={(rows) => { onChange([...rows, ...list].slice(0, max)); setImporting(false); toast.success(`أُضيف ${rows.length.toLocaleString('ar-EG')} سؤال، احفظي لتفعيلها`) }} />}</AnimatePresence>
    </div>
  )
}

/* ---------- questions visitors asked that it could not answer ---------- */
type Miss = { k: string; q: string; n: number; at: string }
function Unanswered({ onAdd }: { onAdd: (q: string) => void }) {
  const qc = useQueryClient()
  const [all, setAll] = useState(false)
  const { data } = useQuery({ queryKey: ['admin', 'assistant', 'unanswered'], queryFn: () => api.get<Miss[]>('/admin/assistant/unanswered') })
  const del = useMutation({
    mutationFn: (k: string | null) => api.del(`/admin/assistant/unanswered${k ? `?k=${encodeURIComponent(k)}` : ''}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin', 'assistant', 'unanswered'] }),
  })
  const items = [...(data ?? [])].sort((a, b) => b.n - a.n)
  if (!items.length) return null
  const shown = all ? items : items.slice(0, 6)
  return (
    <div className="rounded-2xl border border-gold-400/40 bg-gold-400/5 p-4">
      <div className="mb-3 flex flex-wrap items-center gap-2">
        <HelpCircle className="h-5 w-5 text-gold-700 dark:text-gold-300" />
        <b>أسئلة لم يجد لها إجابة</b>
        <span className="rounded-full bg-gold-400/20 px-2 text-xs font-bold text-gold-700 dark:text-gold-300">{items.length.toLocaleString('ar-EG')}</span>
        <span className="text-xs text-muted">أضيفي إجاباتها ليصبح المساعد أذكى</span>
        <span className="flex-1" />
        <button type="button" onClick={async () => { if (await confirmDanger('مسح كل الأسئلة غير المجابة؟', undefined, 'نعم، امسح')) del.mutate(null) }} className="text-xs font-semibold text-muted hover:text-red-600">مسح الكل</button>
      </div>
      <ul className="space-y-1.5">
        {shown.map((m) => (
          <li key={m.k} className="flex items-center gap-2 rounded-xl bg-surface px-3 py-2 text-sm">
            <span className="min-w-0 flex-1 truncate font-semibold text-ink-2">{m.q}</span>
            {m.n > 1 && <span className="shrink-0 text-xs text-muted">سُئل {m.n.toLocaleString('ar-EG')} مرات</span>}
            <button type="button" onClick={() => { onAdd(m.q); del.mutate(m.k) }} className="shrink-0 rounded-lg bg-primary px-2.5 py-1 text-xs font-bold text-white">أضيفي إجابة</button>
            <button type="button" onClick={() => del.mutate(m.k)} className="shrink-0 rounded-lg p-1 text-muted hover:text-red-600" title="تجاهل"><X className="h-4 w-4" /></button>
          </li>
        ))}
      </ul>
      {items.length > 6 && <button type="button" onClick={() => setAll(!all)} className="mt-2 text-xs font-bold text-brand-700">{all ? 'عرض أقل' : `عرض الكل (${items.length.toLocaleString('ar-EG')})`}</button>}
    </div>
  )
}

/* ---------- live tester: uses the list being edited (before saving) ---------- */
function Tester({ kb }: { kb: KbEntry[] }) {
  const [q, setQ] = useState('')
  const [log, setLog] = useState<{ q: string; a: string; ok: boolean }[]>([])
  const ask = useMutation({
    mutationFn: (question: string) =>
      api.post<{ answer: string; suggestions: string[]; confident: boolean }>('/admin/assistant/test', {
        q: question,
        kb,
        history: log.slice(-3).flatMap((x) => [{ role: 'user', content: x.q }, { role: 'assistant', content: x.a }]),
      }),
    onSuccess: (r, question) => { setLog((l) => [...l.slice(-4), { q: question, a: r.answer, ok: r.confident }]); setQ('') },
    onError: (e) => toast.error((e as Error).message),
  })
  return (
    <div className="rounded-2xl border border-line bg-bg p-4">
      <div className="mb-3 flex items-center gap-2 font-bold"><Bot className="h-5 w-5 text-brand-600" /> جرّبي المساعد <span className="text-xs font-normal text-muted">(بالأسئلة الحالية حتى قبل الحفظ)</span></div>
      {log.length > 0 && (
        <div className="mb-3 max-h-72 space-y-2 overflow-y-auto">
          {log.map((x, i) => (
            <div key={i} className="space-y-1.5">
              <div className="w-fit max-w-[90%] rounded-2xl rounded-tr-md bg-primary px-3 py-2 text-sm text-white">{x.q}</div>
              <div className={cx('mr-auto w-fit max-w-[90%] rounded-2xl rounded-tl-md border px-3 py-2 text-sm whitespace-pre-line', x.ok ? 'border-line bg-surface text-ink-2' : 'border-gold-400/50 bg-gold-400/10 text-ink-2')}>
                {!x.ok && <span className="mb-1 block text-xs font-bold text-gold-700 dark:text-gold-300">لم يجد إجابة مؤكدة: أضيفي هذا السؤال للقاعدة</span>}
                {x.a.replace(/\*\*/g, '')}
              </div>
            </div>
          ))}
        </div>
      )}
      <form onSubmit={(e) => { e.preventDefault(); if (q.trim()) ask.mutate(q.trim()) }} className="flex gap-2">
        <input className="input" value={q} onChange={(e) => setQ(e.target.value)} placeholder="اكتبي سؤالًا كما تكتبه الزائرة، مثلًا: بكام دبلوم الموارد البشرية؟" />
        <button type="submit" disabled={ask.isPending || !q.trim()} className="btn-primary !px-4 !py-2.5">{ask.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4" />}</button>
      </form>
    </div>
  )
}

/* ---------- bulk import: pasted text (س: / ج:) or a CSV / Excel-exported file ---------- */
function parseText(text: string): KbEntry[] {
  const out: KbEntry[] = []
  let cur: KbEntry | null = null
  let field: 'q' | 'a' = 'q'
  const flush = () => { if (cur && cur.q.trim() && cur.a.trim()) out.push({ ...cur, q: cur.q.trim(), a: cur.a.trim() }); cur = null }
  for (const raw of text.split(/\r?\n/)) {
    const line = raw.trim()
    const mq = line.match(/^(س|سؤال|Q|السؤال)\s*[:：\-]\s*(.*)$/i)
    const ma = line.match(/^(ج|جواب|الجواب|الإجابة|اجابة|إجابة|A)\s*[:：\-]\s*(.*)$/i)
    if (mq) { flush(); cur = { q: mq[2], variants: [], a: '', link: null }; field = 'q'; continue }
    if (ma && cur) { cur.a = ma[2]; field = 'a'; continue }
    if (!cur) continue
    if (field === 'a') cur.a += (cur.a ? '\n' : '') + raw
    else if (line) cur.variants.push(line)
  }
  flush()
  return out
}

function parseCsv(text: string): KbEntry[] {
  const rows: string[][] = []
  let row: string[] = []
  let cell = ''
  let quoted = false
  const sep = (text.split('\n')[0].match(/;/g)?.length ?? 0) > (text.split('\n')[0].match(/,/g)?.length ?? 0) ? ';' : ','
  for (let i = 0; i < text.length; i++) {
    const c = text[i]
    if (quoted) {
      if (c === '"' && text[i + 1] === '"') { cell += '"'; i++ } else if (c === '"') quoted = false
      else cell += c
    } else if (c === '"') quoted = true
    else if (c === sep) { row.push(cell); cell = '' }
    else if (c === '\n') { row.push(cell.replace(/\r$/, '')); rows.push(row); row = []; cell = '' }
    else cell += c
  }
  if (cell || row.length) { row.push(cell); rows.push(row) }
  const head = (rows[0] ?? []).map((h) => h.trim())
  const col = (names: string[]) => head.findIndex((h) => names.some((n) => h.includes(n)))
  let iq = col(['سؤال', 'السؤال', 'question'])
  let ia = col(['اجاب', 'إجاب', 'جواب', 'answer'])
  const iv = col(['صيغ', 'variants'])
  const il = col(['رابط', 'link'])
  const body = iq >= 0 && ia >= 0 ? rows.slice(1) : rows
  if (iq < 0 || ia < 0) { iq = 0; ia = 1 }
  return body
    .map((r) => ({ q: (r[iq] ?? '').trim(), a: (r[ia] ?? '').trim(), variants: iv >= 0 ? (r[iv] ?? '').split(/[|\n]/).map((x) => x.trim()).filter(Boolean) : [], link: il >= 0 ? (r[il] ?? '').trim() || null : null }))
    .filter((e) => e.q && e.a)
}

function ImportDialog({ onClose, onImport }: { onClose: () => void; onImport: (rows: KbEntry[]) => void }) {
  const [text, setText] = useState('')
  const file = useRef<HTMLInputElement>(null)
  const rows = useMemo(() => (text.includes('س:') || text.includes('سؤال:') || /^Q\s*:/im.test(text) ? parseText(text) : parseCsv(text)), [text])
  return (
    <motion.div className="fixed inset-0 z-[70] grid place-items-center bg-deep-950/50 p-4" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onMouseDown={onClose}>
      <motion.div onMouseDown={(e) => e.stopPropagation()} initial={{ y: 20 }} animate={{ y: 0 }} exit={{ y: 20 }} className="max-h-[90dvh] w-full max-w-2xl overflow-y-auto rounded-3xl bg-surface p-6 shadow-lift">
        <h3 className="text-lg font-extrabold">استيراد أسئلة وأجوبة</h3>
        <p className="mt-1 text-sm text-muted">الصقي النص بهذا الشكل، أو اختاري ملف CSV (من Excel: حفظ باسم ← CSV UTF-8) فيه عمودا «السؤال» و«الإجابة» وعمودان اختياريان «صيغ» (مفصولة بـ |) و«رابط».</p>
        <pre className="mt-3 rounded-xl bg-bg p-3 text-xs leading-6 text-ink-2" dir="rtl">{'س: كم رسوم دبلوم إدارة الأعمال؟\nبكام دبلوم البيزنس؟\nج: رسوم الدبلوم 9000 ريال، ويمكن تقسيطها على 3 دفعات.\n\nس: ما شروط القبول؟\nج: - شهادة الثانوية\n- صورة الهوية'}</pre>
        <textarea className="input mt-3 min-h-44 resize-y" value={text} onChange={(e) => setText(e.target.value)} placeholder="الصقي هنا…" />
        <div className="mt-3 flex flex-wrap items-center gap-2">
          <input
            ref={file}
            type="file"
            accept=".csv,.txt,text/csv,text/plain"
            className="hidden"
            onChange={async (e) => { const f = e.target.files?.[0]; if (f) setText(await f.text()); e.target.value = '' }}
          />
          <button type="button" onClick={() => file.current?.click()} className="btn-ghost !px-4 !py-2 text-sm"><FileUp className="h-4 w-4" /> اختيار ملف</button>
          <span className="flex-1 text-sm text-muted">{text ? `تم التعرف على ${rows.length.toLocaleString('ar-EG')} سؤال` : ''}</span>
          <button type="button" onClick={onClose} className="btn-ghost !px-4 !py-2 text-sm">إلغاء</button>
          <button type="button" disabled={!rows.length} onClick={() => onImport(rows)} className="btn-primary !px-5 !py-2 text-sm">إضافة {rows.length ? rows.length.toLocaleString('ar-EG') : ''}</button>
        </div>
      </motion.div>
    </motion.div>
  )
}
