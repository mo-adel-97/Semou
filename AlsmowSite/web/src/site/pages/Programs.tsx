import { Search, SearchX } from 'lucide-react'
import { motion } from 'motion/react'
import { useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { EmptyState, PageHero, Seo, Skeleton } from '../../components/ui'
import { cx } from '../../lib/format'
import { usePrograms, useSite } from '../../lib/queries'
import { PROGRAM_TYPES, type ProgramType } from '../../lib/types'
import ProgramCard from '../ProgramCard'

export default function Programs() {
  const { type } = useParams()
  const t = type && type in PROGRAM_TYPES ? (type as ProgramType) : undefined
  const { data, isLoading } = usePrograms(t)
  const site = useSite()
  const [q, setQ] = useState('')
  const list = useMemo(() => {
    const s = q.trim()
    return (data ?? []).filter((p) => !s || p.title.includes(s) || p.summary?.includes(s))
  }, [data, q])

  const title = t ? PROGRAM_TYPES[t].plural : 'برامجنا التدريبية'
  const text = t ? PROGRAM_TYPES[t].blurb : 'دبلومات ودورات معتمدة صُممت لتواكب احتياجات سوق العمل وتفتح لك آفاقًا مهنية جديدة.'
  const counts = site.data?.counts

  return (
    <>
      <Seo title={title} description={text} />
      <PageHero title={title} text={text} crumbs={<><Link to="/" className="transition hover:text-brand-700">الرئيسية</Link> / البرامج</>}>
        <div className="mt-10 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex flex-wrap gap-2">
            <TabLink to="/programs" active={!t} label="الكل" count={counts ? counts.diploma + counts.qualifying + counts.development : undefined} />
            {(Object.keys(PROGRAM_TYPES) as ProgramType[]).map((k) => (
              <TabLink key={k} to={`/programs/type/${k}`} active={t === k} label={PROGRAM_TYPES[k].plural} count={counts?.[k]} />
            ))}
          </div>
          <label className="relative block w-full lg:w-80">
            <Search className="pointer-events-none absolute top-1/2 right-4 h-5 w-5 -translate-y-1/2 text-muted" />
            <input
              value={q}
              onChange={(e) => setQ(e.target.value)}
              placeholder="ابحث عن برنامج..."
              className="w-full rounded-full border border-line bg-surface py-3 ps-12 pe-4 text-ink shadow-soft placeholder:text-muted focus:border-brand-400 focus:ring-4 focus:ring-brand-500/15 focus:outline-none"
            />
          </label>
        </div>
      </PageHero>
      <section className="container-x py-14 md:py-20">
        {isLoading ? (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 6 }, (_, i) => <Skeleton key={i} className="h-[26rem] rounded-3xl" />)}
          </div>
        ) : list.length ? (
          <motion.div layout className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {list.map((p, i) => <ProgramCard key={p.id} p={p} index={i} />)}
          </motion.div>
        ) : (
          <EmptyState icon={<SearchX className="h-8 w-8" />} title="لا توجد برامج مطابقة" text="جرّب كلمة بحث أخرى أو تصفّح جميع البرامج." />
        )}
      </section>
    </>
  )
}

function TabLink({ to, active, label, count }: { to: string; active: boolean; label: string; count?: number }) {
  return (
    <Link
      to={to}
      className={cx(
        'rounded-full px-5 py-2.5 font-display text-[15px] font-bold transition',
        active ? 'bg-primary text-white shadow-glow' : 'border border-line bg-surface text-ink-2 hover:border-brand-300 hover:text-brand-700',
      )}
    >
      {label}
      {count !== undefined && <span className={cx('ms-2 rounded-full px-2 py-0.5 text-xs', active ? 'bg-white/20' : 'bg-sand')}>{count}</span>}
    </Link>
  )
}
