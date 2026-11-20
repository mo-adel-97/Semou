import { Eye, Gem, Target } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Img, PageHero, Reveal, Seo, Skeleton } from '../../components/ui'
import { Sections } from '../../lib/layout'
import { useSite } from '../../lib/queries'

export default function About() {
  const { data, isLoading } = useSite()
  const a = data?.settings.about
  return (
    <>
      <Seo title="عن المعهد" description={a?.summary} image={a?.image} />
      <PageHero title={a?.title || 'عن معهد السمو'} text={a?.summary} crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / عن المعهد</>} />
      {isLoading || !a ? (
        <div className="container-x py-20"><Skeleton className="h-96" /></div>
      ) : (
        <Sections settings={data?.settings} page="about" sections={{
          content: (
          <section className="container-x grid items-start gap-12 py-14 md:py-20 lg:grid-cols-2">
            <Reveal className="lg:sticky lg:top-28">
              <Img src={a.image} alt={a.title || ''} className="aspect-[4/3] w-full rounded-[2rem] shadow-lift" />
            </Reveal>
            <Reveal delay={0.05}>
              {a.body ? (
                <div className="prose-ar" dangerouslySetInnerHTML={{ __html: a.body }} />
              ) : (
                <p className="text-lg leading-[2] text-ink-2">{a.summary}</p>
              )}
            </Reveal>
          </section>
          ),
          values: (
          <section className="bg-sand/60 py-16 md:py-20">
            <div className="container-x grid gap-6 md:grid-cols-3">
              {[
                a.vision && { icon: Eye, title: 'رؤيتنا', text: a.vision, tone: 'bg-brand-100 text-brand-700' },
                a.mission && { icon: Target, title: 'رسالتنا', text: a.mission, tone: 'bg-gold-400/15 text-gold-600' },
                a.values?.length && { icon: Gem, title: 'قيمنا', list: a.values, tone: 'bg-deep-800 text-white' },
              ]
                .filter(Boolean)
                .map((x, i) => {
                  const it = x as { icon: typeof Eye; title: string; text?: string; list?: string[]; tone: string }
                  return (
                    <Reveal key={it.title} delay={i * 0.08} className="card p-8">
                      <span className={`grid h-14 w-14 place-items-center rounded-2xl ${it.tone}`}><it.icon className="h-6 w-6" /></span>
                      <h2 className="mt-5 text-2xl font-extrabold">{it.title}</h2>
                      {it.text && <p className="mt-3 leading-[1.9] text-muted">{it.text}</p>}
                      {it.list && (
                        <div className="mt-4 flex flex-wrap gap-2">
                          {it.list.map((v) => <span key={v} className="chip bg-brand-50 !px-4 !py-2 !text-sm text-brand-700">{v}</span>)}
                        </div>
                      )}
                    </Reveal>
                  )
                })}
            </div>
          </section>
          ),
        }} />
      )}
    </>
  )
}
