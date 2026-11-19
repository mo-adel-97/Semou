import { Component, type ErrorInfo, type ReactNode } from 'react'
import { RotateCcw, TriangleAlert } from 'lucide-react'

/** The browser asked for a screen file of an older build (a new version was published meanwhile). */
export const isStaleChunk = (e: unknown) =>
  /Failed to fetch dynamically imported module|error loading dynamically imported module|Importing a module script failed|Unable to preload CSS/i.test(String((e as Error)?.message ?? e))

/** Reloads the page to get the new version; at most once every 30 seconds so it can never loop. */
export function reloadForNewVersion() {
  const KEY = 'semou.reloaded-for-version'
  const last = Number(sessionStorage.getItem(KEY) ?? 0)
  if (Date.now() - last < 30_000) return false
  sessionStorage.setItem(KEY, String(Date.now()))
  location.reload()
  return true
}

/**
 * Catches a crash inside a screen / drawer so the user sees a clear message (and can retry)
 * instead of a blank white page. `resetKey` clears the error when it changes (e.g. another tab).
 */
export class ErrorBoundary extends Component<{ children: ReactNode; resetKey?: unknown; compact?: boolean }, { error: Error | null; key?: unknown }> {
  state: { error: Error | null; key?: unknown } = { error: null, key: this.props.resetKey }

  static getDerivedStateFromError(error: Error) { return { error } }

  static getDerivedStateFromProps(p: { resetKey?: unknown }, s: { error: Error | null; key?: unknown }) {
    return p.resetKey !== s.key ? { error: null, key: p.resetKey } : null
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    if (isStaleChunk(error) && reloadForNewVersion()) return
    console.error('Screen crashed:', error, info.componentStack)
  }

  render() {
    if (!this.state.error) return this.props.children
    if (isStaleChunk(this.state.error)) return (
      <div className={`crash ${this.props.compact ? 'compact' : ''}`} role="status">
        <span className="ic"><RotateCcw size={30} /></span>
        <h3>تم تحديث النظام إلى إصدار جديد</h3>
        <p>جاري تحميل الإصدار الجديد... إذا لم تُحدَّث الصفحة تلقائياً فاضغط الزر.</p>
        <div className="row-btns"><button className="btn" onClick={() => location.reload()}><RotateCcw size={16} /> تحميل الإصدار الجديد</button></div>
      </div>
    )
    return (
      <div className={`crash ${this.props.compact ? 'compact' : ''}`} role="alert">
        <span className="ic"><TriangleAlert size={30} /></span>
        <h3>حدثت مشكلة في عرض هذا الجزء</h3>
        <p>يعمل باقي النظام بشكل طبيعي ولم تتأثر أي بيانات. يرجى المحاولة مرة أخرى، وإذا تكررت المشكلة فأرسل الرسالة التالية إلى الدعم الفني:</p>
        <code dir="ltr">{this.state.error.message}</code>
        <div className="row-btns">
          <button className="btn" onClick={() => this.setState({ error: null })}><RotateCcw size={16} /> حاول مرة أخرى</button>
          {!this.props.compact && <button className="btn ghost" onClick={() => location.reload()}>إعادة تحميل الصفحة</button>}
        </div>
      </div>
    )
  }
}
