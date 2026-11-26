const dateFmt = new Intl.DateTimeFormat('ar-SA-u-ca-gregory-nu-latn', { day: 'numeric', month: 'long', year: 'numeric' })
const dateTimeFmt = new Intl.DateTimeFormat('ar-SA-u-ca-gregory-nu-latn', { day: 'numeric', month: 'short', hour: 'numeric', minute: '2-digit' })
const numFmt = new Intl.NumberFormat('en-US')

function parse(d: string) {
  // "2026-11-15" or "2026-11-15 13:40:00" (server time, Riyadh)
  return new Date(d.length <= 10 ? `${d}T00:00:00` : d.replace(' ', 'T'))
}

export const fmtDate = (d?: string | null) => (d ? dateFmt.format(parse(d)) : '')
export const fmtDateTime = (d?: string | null) => (d ? dateTimeFmt.format(parse(d)) : '')
export const fmtNum = (n: number) => numFmt.format(n)

export function timeAgo(d: string) {
  const diff = (Date.now() - parse(d).getTime()) / 1000
  if (diff < 60) return 'الآن'
  if (diff < 3600) return `منذ ${Math.floor(diff / 60)} دقيقة`
  if (diff < 86400) return `منذ ${Math.floor(diff / 3600)} ساعة`
  if (diff < 86400 * 7) return `منذ ${Math.floor(diff / 86400)} يوم`
  return fmtDate(d)
}

/** "0501234567" -> "966501234567" for wa.me links */
export function waNumber(n?: string | null) {
  if (!n) return ''
  const d = n.replace(/\D+/g, '')
  if (d.startsWith('966')) return d
  if (d.startsWith('05')) return `966${d.slice(1)}`
  if (d.startsWith('5') && d.length === 9) return `966${d}`
  return d
}

export const cx = (...c: (string | false | null | undefined)[]) => c.filter(Boolean).join(' ')

/* ---------- Kashida (تطويل): stretch an Arabic word the calligraphic way, e.g. الاصطناعي → الاصطناعـــي ---------- */

// letters that join the next one (the only places a kashida can go after)
const JOINS_NEXT = new Set('بتثجحخسشصضطظعغفقكلمنهيئـ')
const LETTER = /[ء-ي]/
const MARK = /[ً-ْٰ]/ // harakat stay on their letter

/** Puts `n` tatweel characters before the last letter that can take one (never inside لا). Non-Arabic words come back unchanged. */
export function kashida(word: string, n = 3): string {
  const chars = [...word]
  for (let i = chars.length - 1; i > 0; i--) {
    if (!LETTER.test(chars[i])) continue
    let j = i - 1
    while (j >= 0 && MARK.test(chars[j])) j--
    if (j < 0) break
    const prev = chars[j]
    if (!JOINS_NEXT.has(prev)) continue
    if (prev === 'ل' && 'اأإآ'.includes(chars[i])) continue
    return [...chars.slice(0, i), 'ـ'.repeat(n), ...chars.slice(i)].join('')
  }
  return word
}

/** The word of a sentence that reads best stretched: the longest Arabic one (ties → the last). Returns its index or -1. */
export function kashidaWord(words: string[]): number {
  let best = -1
  words.forEach((w, i) => { const len = w.replace(/[^ء-ي]/g, '').length; if (len >= 3 && kashida(w) !== w && (best < 0 || len >= words[best].replace(/[^ء-ي]/g, '').length)) best = i })
  return best
}

/** Stretches a word but keeps trailing punctuation outside the stretch (موثوقة. → موثوقـــة.). */
export function kashidaKeepPunct(word: string, n = 3) {
  const m = word.match(/^(.*?)([.,،:!؟?]*)$/)
  return m ? kashida(m[1], n) + m[2] : kashida(word, n)
}

/** A short label in the stretched display style: its longest word gets the kashida («دورة تأهيلية» → «دورة تأهيـلية»). */
export function stretchLabel(label: string, n = 2) {
  const w = label.split(' ')
  const k = kashidaWord(w)
  return k < 0 ? label : w.map((x, i) => (i === k ? kashidaKeepPunct(x, n) : x)).join(' ')
}
