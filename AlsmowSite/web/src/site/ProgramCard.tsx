import { ArrowLeft, Check, Clock, Laptop, Timer } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Img, Tilt } from '../components/ui'
import { PROGRAM_MODES, PROGRAM_TYPES, type Program } from '../lib/types'

export default function ProgramCard({ p }: { p: Program; index?: number }) {
  return (
    <Tilt className="group h-full rounded-[1.75rem]" max={5}>
      <article className="relative flex h-full flex-col overflow-hidden rounded-[1.75rem] border border-line bg-surface shadow-soft transition-shadow duration-500 group-hover:shadow-lift">
        <Link to={`/programs/${p.slug}`} className="relative block aspect-[16/10] overflow-hidden" tabIndex={-1} aria-hidden="true">
          <Img src={p.image} alt={p.title} className="h-full w-full transition duration-[1.2s] ease-out group-hover:scale-110" />
          <div className="absolute inset-0 bg-gradient-to-t from-deep-950/60 via-deep-950/5 to-transparent" />
          <div className="absolute top-3 right-3 flex flex-wrap gap-1.5">
            <span className="chip glass text-ink">{PROGRAM_TYPES[p.type].label}</span>
            {p.badge && <span className="chip bg-gold-400 text-deep-950 shadow-sm">{p.badge}</span>}
          </div>
          {p.mode === 'online' && (
            <span className="chip absolute right-3 bottom-3 bg-primary/90 text-white backdrop-blur"><Laptop className="h-3.5 w-3.5" /> عن بعد</span>
          )}
        </Link>
        <div className="flex flex-1 flex-col p-6">
          <h3 className="text-xl leading-snug font-bold text-ink">
            <Link to={`/programs/${p.slug}`} className="transition-colors after:absolute after:inset-0 group-hover:text-brand-700">{p.title}</Link>
          </h3>
          {p.summary && <p className="mt-3 line-clamp-2 leading-relaxed text-muted">{p.summary}</p>}
          {p.features.length > 0 && (
            <ul className="mt-4 space-y-2">
              {p.features.slice(0, 3).map((f) => (
                <li key={f} className="flex items-start gap-2 text-sm text-ink-2"><Check className="mt-0.5 h-4 w-4 shrink-0 text-brand-500" />{f}</li>
              ))}
            </ul>
          )}
          <div className="mt-auto flex items-center justify-between gap-3 border-t border-line pt-4 text-sm text-muted">
            <div className="flex flex-wrap items-center gap-4">
              {p.hours ? <span className="flex items-center gap-1.5"><Clock className="h-4 w-4 text-brand-500" />{p.hours} ساعة</span> : null}
              {p.duration && <span className="flex items-center gap-1.5"><Timer className="h-4 w-4 text-brand-500" />{p.duration}</span>}
              {!p.hours && !p.duration && <span>{p.mode ? PROGRAM_MODES[p.mode] : 'التفاصيل والتسجيل'}</span>}
            </div>
            <span className="grid h-10 w-10 place-items-center rounded-full bg-brand-50 text-brand-700 transition duration-500 group-hover:-rotate-45 group-hover:bg-primary group-hover:text-white">
              <ArrowLeft className="h-4 w-4" />
            </span>
          </div>
        </div>
      </article>
    </Tilt>
  )
}
