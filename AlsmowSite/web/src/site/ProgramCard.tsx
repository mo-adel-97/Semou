import { ArrowLeft, BadgeCheck, Clock, Laptop, MapPin, Sparkles, Timer, type LucideIcon } from 'lucide-react'
import { Link } from 'react-router-dom'
import { stretchLabel } from '../lib/format'
import { FIELDS, fieldOf } from '../lib/fields'
import { PROGRAM_MODES, PROGRAM_TYPES, type Program } from '../lib/types'
import { at, usePalette } from './home/kit'

/**
 * A programme in a list (البرامج / برامج قد تهمك): instead of a photo, a small coloured stage like the hero's
 * with the programme's field drawn on it (lib/subjects); under it only the name and the essentials.
 */
export default function ProgramCard({ p, index = 0 }: { p: Program; index?: number }) {
  const palette = usePalette()
  const { icon: Icon } = FIELDS[fieldOf(p)]
  const meta: { icon: LucideIcon; t: string }[] = [
    ...(p.duration ? [{ icon: Timer, t: p.duration }] : []),
    ...(p.hours ? [{ icon: Clock, t: `${p.hours} ساعة` }] : []),
    ...(p.mode ? [{ icon: p.mode === 'online' ? Laptop : MapPin, t: PROGRAM_MODES[p.mode] }] : []),
  ]
  return (
    <Link to={`/programs/${p.slug}`} className="pcard group" style={{ ['--stage' as string]: at(palette, index) }}>
      <div className="pcard-stage">
        <div className="demo-stage-bg" aria-hidden />
        <Icon className="pcard-water" strokeWidth={1} aria-hidden />
        <span className="pcard-type"><i />{stretchLabel(PROGRAM_TYPES[p.type].label)}</span>
        {p.badge && <span className="pcard-badge" title={p.badge}><Sparkles className="h-3.5 w-3.5 shrink-0" /><span className="truncate">{p.badge}</span></span>}
        <span className="pcard-emblem" aria-hidden><Icon strokeWidth={1.6} /></span>
      </div>
      <div className="pcard-body">
        <h3 className="pcard-title">{p.title}</h3>
        {p.accreditation && <span className="pcard-acc"><BadgeCheck className="h-4 w-4 shrink-0" /><span className="truncate">{p.accreditation}</span></span>}
        <div className="pcard-foot">
          <div className="pcard-meta">{meta.length ? meta.map((m) => <span key={m.t}><m.icon />{m.t}</span>) : <span>التفاصيل والتسجيل</span>}</div>
          <span className="pcard-go" aria-hidden><ArrowLeft className="h-[1.1rem] w-[1.1rem]" /></span>
        </div>
      </div>
    </Link>
  )
}
