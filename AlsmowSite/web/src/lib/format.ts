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
