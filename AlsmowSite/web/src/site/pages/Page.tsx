import { Link, useParams } from 'react-router-dom'
import { PageHero, Seo, Skeleton } from '../../components/ui'
import { fmtDate } from '../../lib/format'
import { usePage, useSite } from '../../lib/queries'
import { cx } from '../../lib/format'
import NotFound from './NotFound'

export default function Page() {
  const { slug = '' } = useParams()
  const { data, isLoading, error } = usePage(slug)
  const site = useSite()
  if (error) return <NotFound />
  return (
    <>
      <Seo title={data?.title} />
      <PageHero title={data?.title ?? '...'} crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / الإرشاد الأكاديمي</>} />
      <section className="container-x grid gap-10 py-14 md:py-20 lg:grid-cols-[1fr_300px]">
        <div className="min-w-0">
          {isLoading || !data ? (
            <div className="space-y-4"><Skeleton className="h-6 w-full" /><Skeleton className="h-6 w-5/6" /><Skeleton className="h-6 w-4/6" /></div>
          ) : (
            <>
              <div className="prose-ar" dangerouslySetInnerHTML={{ __html: data.body ?? '' }} />
              <p className="mt-12 border-t border-line pt-5 text-sm text-muted">آخر تحديث: {fmtDate(data.updated_at)}</p>
            </>
          )}
        </div>
        <aside className="lg:sticky lg:top-28 lg:self-start">
          <div className="card p-3">
            <div className="px-3 py-2 font-display text-sm font-bold text-muted">الإرشاد الأكاديمي</div>
            {site.data?.pages.map((p) => (
              <Link
                key={p.slug}
                to={`/p/${p.slug}`}
                className={cx('block rounded-xl px-3 py-2.5 text-[15px] transition', p.slug === slug ? 'bg-primary font-bold text-white' : 'text-ink-2 hover:bg-sand')}
              >
                {p.title}
              </Link>
            ))}
          </div>
        </aside>
      </section>
    </>
  )
}
