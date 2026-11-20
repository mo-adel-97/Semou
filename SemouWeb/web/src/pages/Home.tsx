import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { motion } from 'motion/react'
import { CheckCircle2, LayoutGrid, Palette, Search, Sparkles } from 'lucide-react'
import { useAuth } from '../auth'
import { useAllowedMenu } from '../useAllowedMenu'
import { Stat } from '../components/ui'

const openPalette = () => window.dispatchEvent(new KeyboardEvent('keydown', { key: 'k', ctrlKey: true }))

export function Home() {
  const { me } = useAuth()
  const groups = useAllowedMenu()
  const nav = useNavigate()
  const [greeting] = useState(() => (new Date().getHours() < 12 ? 'صباح الخير' : 'مساء الخير'))
  const total = groups.reduce((n, g) => n + g.items.length, 0)
  const ready = groups.reduce((n, g) => n + g.items.filter(i => i.ported).length, 0)

  return (
    <>
      <motion.section className="welcome" initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.35 }}>
        <span className="blob" style={{ width: 280, height: 280, left: -80, top: -120 }} />
        <span className="blob" style={{ width: 160, height: 160, left: 160, bottom: -90 }} />
        <img className="wings" src="/logo.png" alt="" />
        <h1>{greeting}، {me?.fullName?.split(' ')[0]} 👋</h1>
        <p>هذه هي الشاشات المتاحة لك حسب صلاحياتك. استخدم البحث السريع للوصول لأي شاشة في ثانية، وغيّر شكل النظام من زر الألوان <Palette size={14} style={{ verticalAlign: 'middle' }} /> في أعلى الشاشة.</p>
        <div className="row">
          <button className="btn light" onClick={openPalette}><Search size={16} /> بحث سريع <kbd>Ctrl K</kbd></button>
          <button className="btn secondary" onClick={() => nav('/s/branches')}>الفروع</button>
        </div>
      </motion.section>

      <div className="stats">
        <Stat icon={<LayoutGrid size={22} />} value={groups.length} label="أقسام متاحة لك" delay={0.05} />
        <Stat icon={<Sparkles size={22} />} value={total} label="شاشة بصلاحيتك" delay={0.1} />
        <Stat icon={<CheckCircle2 size={22} />} value={ready} label="شاشة جاهزة على الويب" delay={0.15} />
      </div>

      <div className="modules">
        {groups.map((g, gi) => {
          const Icon = g.icon
          return (
            <motion.div key={g.key} className="module" initial={{ opacity: 0, y: 14 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.15 + gi * 0.05, duration: 0.3 }} whileHover={{ y: -3 }}>
              <div className="module-head">
                <span className="ic"><Icon size={22} /></span>
                <div><h3>{g.title}</h3><small>{g.items.length} شاشة</small></div>
              </div>
              <div className="module-links">
                {g.items.map(i => i.href ? (
                  <a key={i.key} href={i.href} target={i.newTab ? '_blank' : undefined} rel="noopener noreferrer" className="ready" title={i.title}>
                    <span className="d" />{i.title}
                  </a>
                ) : (
                  <Link key={i.key} to={`/s/${i.key}`} className={i.ported ? 'ready' : ''} title={i.title}>
                    <span className="d" />{i.title}
                  </Link>
                ))}
              </div>
            </motion.div>
          )
        })}
      </div>
    </>
  )
}
