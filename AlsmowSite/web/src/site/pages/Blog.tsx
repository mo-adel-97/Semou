import { ArrowLeft, ArrowRight, CalendarDays, Newspaper } from 'lucide-react'
import { motion } from 'motion/react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { EmptyState, Img, PageHero, Reveal, Seo, Skeleton } from '../../components/ui'
import { cx, fmtDate } from '../../lib/format'
import { usePost, usePosts } from '../../lib/queries'
import type { Post } from '../../lib/types'
import NotFound from './NotFound'

export function PostCard({ p, index = 0 }: { p: Post; index?: number }) {
  return (
    <motion.article
      initial={{ opacity: 0, y: 24 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
      transition={{ duration: 0.5, delay: index * 0.07 }}
      className="group card relative flex h-full flex-col overflow-hidden transition duration-300 hover:-translate-y-1 hover:shadow-lift"
    >
      <div className="relative aspect-[16/10] overflow-hidden">
        <Img src={p.cover} alt={p.title} className="h-full w-full transition duration-700 group-hover:scale-105" />
        {p.category && <span className="chip absolute top-3 right-3 bg-white/95 text-brand-700 shadow-sm">{p.category}</span>}
      </div>
      <div className="flex flex-1 flex-col p-6">
        {p.published_at && (
          <div className="flex items-center gap-2 text-sm text-muted"><CalendarDays className="h-4 w-4 text-brand-500" />{fmtDate(p.published_at)}</div>
        )}
        <h3 className="mt-2 text-lg leading-snug font-bold">
          <Link to={`/blog/${p.slug}`} className="after:absolute after:inset-0 group-hover:text-brand-700">{p.title}</Link>
        </h3>
        {p.excerpt && <p className="mt-2 line-clamp-3 leading-relaxed text-muted">{p.excerpt}</p>}
        <span className="mt-auto flex items-center gap-1.5 pt-4 text-sm font-bold text-brand-700">
          اقرئي المزيد <ArrowLeft className="h-4 w-4 transition group-hover:-translate-x-1" />
        </span>
      </div>
    </motion.article>
  )
}

export function Blog() {
  const [sp, setSp] = useSearchParams()
  const page = Math.max(1, Number(sp.get('page')) || 1)
  const { data, isLoading } = usePosts(page)
  return (
    <>
      <Seo title="المدونة" description="مقالات وأخبار معهد السمو للتدريب" />
      <PageHero title="المدونة والأخبار" text="مقالات مفيدة وأحدث أخبار وفعاليات معهد السمو." crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / المدونة</>} />
      <section className="container-x py-14 md:py-20">
        {isLoading ? (
          <div className="grid gap-6 md:grid-cols-3">{Array.from({ length: 6 }, (_, i) => <Skeleton key={i} className="h-96" />)}</div>
        ) : data?.items.length ? (
          <>
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
              {data.items.map((p, i) => <PostCard key={p.id} p={p} index={i % 3} />)}
            </div>
            {data.pages > 1 && (
              <nav className="mt-12 flex items-center justify-center gap-2" aria-label="الصفحات">
                <button disabled={page <= 1} onClick={() => setSp({ page: String(page - 1) })} className="btn-ghost !p-3" aria-label="السابق"><ArrowRight className="h-4 w-4" /></button>
                {Array.from({ length: data.pages }, (_, i) => i + 1).map((n) => (
                  <button
                    key={n}
                    onClick={() => setSp({ page: String(n) })}
                    className={cx('h-11 w-11 rounded-full font-bold transition', n === page ? 'bg-primary text-white' : 'bg-surface text-ink-2 hover:bg-brand-50')}
                  >
                    {n}
                  </button>
                ))}
                <button disabled={page >= data.pages} onClick={() => setSp({ page: String(page + 1) })} className="btn-ghost !p-3" aria-label="التالي"><ArrowLeft className="h-4 w-4" /></button>
              </nav>
            )}
          </>
        ) : (
          <EmptyState icon={<Newspaper className="h-8 w-8" />} title="لا توجد مقالات بعد" text="تابعينا قريبًا لقراءة أحدث المقالات والأخبار." />
        )}
      </section>
    </>
  )
}

export function PostPage() {
  const { slug = '' } = useParams()
  const { data, isLoading, error } = usePost(slug)
  if (error) return <NotFound />
  if (isLoading || !data) return <div className="container-x space-y-5 pt-44 pb-20"><Skeleton className="h-12 w-2/3" /><Skeleton className="h-96" /></div>
  const p = data.post
  return (
    <>
      <Seo title={p.title} description={p.excerpt} image={p.cover} />
      <PageHero title={p.title} crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / <Link to="/blog" className="transition hover:text-brand-700">المدونة</Link></>}>
        <div className="mt-6 flex flex-wrap items-center gap-3 text-muted">
          {p.category && <span className="chip bg-brand-50 text-brand-700">{p.category}</span>}
          {p.published_at && <span className="flex items-center gap-2"><CalendarDays className="h-4 w-4" />{fmtDate(p.published_at)}</span>}
        </div>
      </PageHero>
      <article className="container-x max-w-4xl py-14 md:py-20">
        {p.cover && (
          <Reveal>
            <Img src={p.cover} alt={p.title} eager className="mb-10 aspect-[16/9] w-full rounded-[2rem] shadow-soft" />
          </Reveal>
        )}
        <div className="prose-ar" dangerouslySetInnerHTML={{ __html: p.body ?? '' }} />
      </article>
      {data.more.length > 0 && (
        <section className="bg-sand/60 py-16">
          <div className="container-x">
            <h2 className="mb-8 text-3xl font-extrabold">مقالات أخرى</h2>
            <div className="grid gap-6 md:grid-cols-3">{data.more.map((m, i) => <PostCard key={m.id} p={m} index={i} />)}</div>
          </div>
        </section>
      )}
    </>
  )
}
