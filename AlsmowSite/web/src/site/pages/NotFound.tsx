import { Home } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Seo } from '../../components/ui'

export default function NotFound() {
  return (
    <section className="container-x flex min-h-[70vh] flex-col items-center justify-center pt-36 pb-20 text-center">
      <Seo title="الصفحة غير موجودة" />
      <div className="font-display text-[8rem] leading-none font-black text-brand-100">404</div>
      <h1 className="mt-4 text-3xl font-extrabold">الصفحة غير موجودة</h1>
      <p className="mt-3 max-w-md text-muted">ربما تم نقل الصفحة أو حذفها. يمكنك العودة للرئيسية أو تصفح البرامج.</p>
      <div className="mt-8 flex gap-3">
        <Link to="/" className="btn-primary"><Home className="h-4 w-4" /> الرئيسية</Link>
        <Link to="/programs" className="btn-ghost">البرامج</Link>
      </div>
    </section>
  )
}
