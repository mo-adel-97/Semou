import { ArrowLeft, ArrowRight, BookOpen, CalendarDays, Clock, Newspaper, Phone } from 'lucide-react'
import { useMemo } from 'react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { WhatsAppIcon } from '../../components/BrandIcons'
import { EmptyState, Img, PageHero, Reveal, Seo, Skeleton } from '../../components/ui'
import { cx, fmtDate, waNumber } from '../../lib/format'
import { FIELDS, fieldOf } from '../../lib/fields'
import { usePost, usePosts, usePrograms, useSite } from '../../lib/queries'
import type { Post } from '../../lib/types'
import { at, usePalette } from '../home/kit'
import ProgramCard from '../ProgramCard'
import NotFound from './NotFound'

/** The article's topic icon: the programme field its title talks about (lib/fields), else an open book. */
function topicIcon(p: Pick<Post, 'title'>) {
  const f = fieldOf({ title: p.title, category: null })
  return f === 'business' ? BookOpen : FIELDS[f].icon
}

/** Minutes to read the article body (Arabic ≈ 180 words a minute). */
function readMinutes(html?: string | null) {
  const words = (html ?? '').replace(/<[^>]+>/g, ' ').split(/\s+/).filter(Boolean).length
  return Math.max(1, Math.round(words / 180))
}

/** An article in a list: the same card as a programme (coloured stage + topic icon), so the blog reads as part of the site. */
export function PostCard({ p, index = 0 }: { p: Post; index?: number }) {
  const palette = usePalette()
  const Icon = topicIcon(p)
  return (
    <Link to={`/blog/${p.slug}`} className="pcard group" style={{ ['--stage' as string]: at(palette, index) }}>
      <div className="pcard-stage">
        <div className="demo-stage-bg" aria-hidden />
        <Icon className="pcard-water" strokeWidth={1} aria-hidden />
        <span className="pcard-type"><i />{p.category || 'مقال'}</span>
        <span className="pcard-emblem" aria-hidden><Icon strokeWidth={1.6} /></span>
      </div>
      <div className="pcard-body">
        <h3 className="pcard-title">{p.title}</h3>
        {p.excerpt && <p className="line-clamp-2 text-[.92rem] leading-relaxed text-muted">{p.excerpt}</p>}
        <div className="pcard-foot">
          <div className="pcard-meta">
            {p.published_at ? <span><CalendarDays />{fmtDate(p.published_at)}</span> : <span>اقرأ المقال</span>}
          </div>
          <span className="pcard-go" aria-hidden><ArrowLeft className="h-[1.1rem] w-[1.1rem]" /></span>
        </div>
      </div>
    </Link>
  )
}

export function Blog() {
  const [sp, setSp] = useSearchParams()
  const page = Math.max(1, Number(sp.get('page')) || 1)
  const { data, isLoading } = usePosts(page)
  return (
    <>
      <Seo title="المدونة" description="أدلة ونصائح ومقالات عن الدبلومات والدورات المعتمدة والتدريب وسوق العمل من معهد السمو للتدريب." />
      <PageHero title="المدونة والأخبار" text="أدلة ونصائح تساعدك في اختيار الدبلوم أو الدورة المناسبة، وأحدث أخبار معهد السمو." crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / المدونة</>} />
      <section className="container-x py-14 md:py-20">
        {isLoading ? (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">{Array.from({ length: 6 }, (_, i) => <Skeleton key={i} className="h-[24rem] rounded-3xl" />)}</div>
        ) : data?.items.length ? (
          <>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {data.items.map((p, i) => <PostCard key={p.id} p={p} index={i} />)}
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
          <EmptyState icon={<Newspaper className="h-8 w-8" />} title="لا توجد مقالات بعد" text="تابعنا قريبًا لقراءة أحدث المقالات والأخبار." />
        )}
      </section>
    </>
  )
}

export function PostPage() {
  const { slug = '' } = useParams()
  const { data, isLoading, error } = usePost(slug)
  const site = useSite()
  const programs = usePrograms()
  const p = data?.post
  // the programmes the article links to, as cards under it (in the order the article mentions them)
  const linked = useMemo(() => {
    const all = programs.data
    if (!p?.body || !all) return []
    const seen = new Set<string>()
    return [...p.body.matchAll(/href="\/programs\/([^"#?]+)"/g)].flatMap((m) => {
      let s = m[1]
      try { s = decodeURIComponent(s) } catch { /* already plain */ }
      const hit = all.find((x) => x.slug === s)
      if (!hit || seen.has(s)) return []
      seen.add(s)
      return [hit]
    }).slice(0, 6)
  }, [p?.body, programs.data])

  if (error) return <NotFound />
  if (isLoading || !data || !p) return <div className="container-x space-y-5 pt-44 pb-20"><Skeleton className="h-12 w-2/3" /><Skeleton className="h-96" /></div>
  const c = site.data?.settings.contact
  const wa = waNumber(c?.whatsapp || c?.phone)

  return (
    <>
      <Seo title={p.title} description={p.excerpt} image={p.cover} />
      <PageHero
        title={p.title}
        text={p.excerpt}
        crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / <Link to="/blog" className="transition hover:text-brand-700">المدونة</Link></>}
      >
        <div className="mt-6 flex flex-wrap items-center gap-2">
          {p.category && <span className="chip bg-brand-50 text-brand-700">{p.category}</span>}
          {p.published_at && <span className="chip bg-surface text-ink-2"><CalendarDays className="h-4 w-4 text-brand-500" />{fmtDate(p.published_at)}</span>}
          <span className="chip bg-surface text-ink-2"><Clock className="h-4 w-4 text-brand-500" />{readMinutes(p.body)} دقائق قراءة</span>
        </div>
      </PageHero>

      <section className="container-x grid gap-10 py-14 md:py-20 lg:grid-cols-[1fr_380px]">
        <article className="min-w-0">
          {p.cover && (
            <Reveal>
              <Img src={p.cover} alt={p.title} eager className="aspect-[16/9] w-full rounded-[2rem] shadow-soft" />
            </Reveal>
          )}
          <div className={cx('card p-7 md:p-10', p.cover && 'mt-8')}>
            <div className="prose-ar" dangerouslySetInnerHTML={{ __html: p.body ?? '' }} />
          </div>
        </article>

        <aside className="lg:sticky lg:top-28 lg:self-start">
          <div className="card overflow-hidden">
            <div className="bg-deep-800 p-6 text-white">
              <h2 className="text-xl font-bold">ابدأ رحلتك التدريبية</h2>
              <p className="mt-2 text-sm leading-relaxed text-white/70">اترك بياناتك وسيتواصل معك فريق القبول خلال يوم عمل.</p>
            </div>
            <div className="space-y-3 p-6">
              <Link to="/register" className="btn-primary w-full">سجّل الآن <ArrowLeft className="h-4 w-4" /></Link>
              {wa && (
                <a
                  href={`https://wa.me/${wa}?text=${encodeURIComponent(`السلام عليكم، قرأت مقال «${p.title}» وأرغب في الاستفسار`)}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="btn w-full bg-[#25D366] text-white hover:brightness-95"
                >
                  <WhatsAppIcon className="h-5 w-5" /> استفسر عبر واتساب
                </a>
              )}
              {c?.phone && (
                <a href={`tel:${c.phone}`} className="btn-ghost w-full"><Phone className="h-4 w-4" /> <span dir="ltr">{c.phone}</span></a>
              )}
            </div>
            <div className="border-t border-line p-5">
              <Link to="/programs" className="flex items-center justify-between font-display font-bold text-brand-700 transition hover:text-brand-800">
                تصفّح جميع البرامج <ArrowLeft className="h-4 w-4" />
              </Link>
            </div>
          </div>
        </aside>
      </section>

      {linked.length > 0 && (
        <section className="pb-4 md:pb-8">
          <div className="container-x">
            <h2 className="mb-8 text-3xl font-extrabold">البرامج المذكورة في المقال</h2>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">{linked.map((r, i) => <ProgramCard key={r.id} p={r} index={i} />)}</div>
          </div>
        </section>
      )}

      {data.more.length > 0 && (
        <section className="mt-12 bg-sand/60 py-16 md:py-20">
          <div className="container-x">
            <h2 className="mb-8 text-3xl font-extrabold">مقالات أخرى</h2>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">{data.more.map((m, i) => <PostCard key={m.id} p={m} index={i + 1} />)}</div>
          </div>
        </section>
      )}
    </>
  )
}
