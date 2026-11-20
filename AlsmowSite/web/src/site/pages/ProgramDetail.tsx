import { ArrowLeft, Award, Check, Clock, Laptop, Phone, Tag, Timer } from 'lucide-react'
import { Link, useParams } from 'react-router-dom'
import { WhatsAppIcon } from '../../components/BrandIcons'
import { Img, PageHero, Reveal, Seo, Skeleton } from '../../components/ui'
import { waNumber } from '../../lib/format'
import { useProgram, useSite } from '../../lib/queries'
import { PROGRAM_MODES, PROGRAM_TYPES } from '../../lib/types'
import ProgramCard from '../ProgramCard'
import NotFound from './NotFound'

export default function ProgramDetail() {
  const { slug = '' } = useParams()
  const { data, isLoading, error } = useProgram(slug)
  const site = useSite()
  if (error) return <NotFound />
  if (isLoading || !data) {
    return (
      <div className="container-x space-y-6 pt-44 pb-20">
        <Skeleton className="h-12 w-2/3" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }
  const p = data.program
  const c = site.data?.settings.contact
  const wa = waNumber(c?.whatsapp || c?.phone)
  const facts = [
    p.hours ? { icon: Clock, label: 'الساعات المعتمدة', value: `${p.hours} ساعة` } : null,
    p.duration ? { icon: Timer, label: 'المدة', value: p.duration } : null,
    p.mode ? { icon: Laptop, label: 'طريقة الدراسة', value: PROGRAM_MODES[p.mode] } : null,
    p.price ? { icon: Tag, label: 'الرسوم', value: p.price } : null,
  ].filter(Boolean) as { icon: typeof Clock; label: string; value: string }[]

  return (
    <>
      <Seo title={p.title} description={p.summary} image={p.image} />
      <PageHero
        title={p.title}
        text={p.summary}
        crumbs={
          <>
            <Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / <Link to={`/programs/type/${p.type}`} className="transition hover:text-brand-700">{PROGRAM_TYPES[p.type].plural}</Link>
          </>
        }
      >
        <div className="mt-6 flex flex-wrap gap-2">
          <span className="chip bg-brand-50 text-brand-700">{PROGRAM_TYPES[p.type].label}</span>
          {p.badge && <span className="chip bg-gold-400 text-deep-950">{p.badge}</span>}
        </div>
      </PageHero>

      <section className="container-x grid gap-10 py-14 md:py-20 lg:grid-cols-[1fr_380px]">
        <div className="min-w-0">
          <Reveal>
            <Img src={p.image} alt={p.title} eager className="aspect-[16/9] w-full rounded-[2rem] shadow-soft" />
          </Reveal>
          {facts.length > 0 && (
            <div className="mt-8 grid grid-cols-2 gap-4 md:grid-cols-4">
              {facts.map((f) => (
                <div key={f.label} className="card p-5">
                  <f.icon className="h-6 w-6 text-brand-500" />
                  <div className="mt-3 text-sm text-muted">{f.label}</div>
                  <div className="mt-1 font-display text-lg font-bold">{f.value}</div>
                </div>
              ))}
            </div>
          )}
          {p.features.length > 0 && (
            <div className="card mt-8 p-7">
              <h2 className="text-2xl font-extrabold">ماذا ستكتسب؟</h2>
              <ul className="mt-5 grid gap-3 sm:grid-cols-2">
                {p.features.map((f) => (
                  <li key={f} className="flex items-start gap-3">
                    <span className="mt-0.5 grid h-6 w-6 shrink-0 place-items-center rounded-full bg-brand-100 text-brand-700"><Check className="h-4 w-4" /></span>
                    <span className="leading-relaxed text-ink-2">{f}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
          {p.body && <div className="prose-ar mt-10" dangerouslySetInnerHTML={{ __html: p.body }} />}
        </div>

        <aside className="lg:sticky lg:top-28 lg:self-start">
          <div className="card overflow-hidden">
            <div className="bg-deep-800 p-6 text-white">
              <h2 className="text-xl font-bold">سجّل في البرنامج</h2>
              <p className="mt-2 text-sm leading-relaxed text-white/70">اترك بياناتك وسيتواصل معك فريق القبول خلال يوم عمل.</p>
            </div>
            <div className="space-y-3 p-6">
              <Link to={`/register?program=${encodeURIComponent(p.title)}`} className="btn-primary w-full">
                سجّل الآن <ArrowLeft className="h-4 w-4" />
              </Link>
              {wa && (
                <a
                  href={`https://wa.me/${wa}?text=${encodeURIComponent(`السلام عليكم، أرغب في الاستفسار عن: ${p.title}`)}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="btn w-full bg-[#25D366] text-white hover:brightness-95"
                >
                  <WhatsAppIcon className="h-5 w-5" /> استفسري عبر واتساب
                </a>
              )}
              {c?.phone && (
                <a href={`tel:${c.phone}`} className="btn-ghost w-full">
                  <Phone className="h-4 w-4" /> <span dir="ltr">{c.phone}</span>
                </a>
              )}
            </div>
            {p.accreditation && (
              <div className="flex items-center gap-3 border-t border-line bg-gold-400/10 p-5">
                <Award className="h-8 w-8 shrink-0 text-gold-600" />
                <div>
                  <div className="text-xs text-muted">معتمد من</div>
                  <div className="font-display text-sm font-bold">{p.accreditation}</div>
                </div>
              </div>
            )}
          </div>
        </aside>
      </section>

      {data.related.length > 0 && (
        <section className="bg-sand/60 py-16 md:py-20">
          <div className="container-x">
            <h2 className="mb-8 text-3xl font-extrabold">برامج قد تهمك</h2>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {data.related.map((r, i) => <ProgramCard key={r.id} p={r} index={i} />)}
            </div>
          </div>
        </section>
      )}
    </>
  )
}
