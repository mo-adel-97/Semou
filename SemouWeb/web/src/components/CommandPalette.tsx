import { createPortal } from 'react-dom'
import { useNavigate } from 'react-router-dom'
import { Command } from 'cmdk'
import { motion } from 'motion/react'
import { CornerDownLeft, Home, Search } from 'lucide-react'
import { useAllowedMenu } from '../useAllowedMenu'

/** Ctrl+K: jump to any screen the user is allowed to open (cmdk = fuzzy search + keyboard nav). */
export function CommandPalette({ onClose }: { onClose: () => void }) {
  const groups = useAllowedMenu()
  const nav = useNavigate()
  const go = (path: string) => { nav(path); onClose() }

  return createPortal(
    <>
      <motion.div className="overlay" initial={{ opacity: 0 }} animate={{ opacity: 1 }} onClick={onClose} />
      <motion.div className="cmdk" initial={{ opacity: 0, y: -12, scale: 0.98 }} animate={{ opacity: 1, y: 0, scale: 1 }} transition={{ type: 'spring', stiffness: 500, damping: 34 }}>
        <Command label="بحث سريع" loop onKeyDown={e => { if (e.key === 'Escape') onClose() }}>
          <div className="cmdk-input">
            <Search size={20} color="var(--muted)" />
            <Command.Input autoFocus placeholder="ابحث عن شاشة... (مثال: سند قبض، الفروع)" />
            <kbd>Esc</kbd>
          </div>
          <Command.List className="cmdk-list">
            <Command.Empty className="empty"><b>لا توجد نتائج</b></Command.Empty>
            <Command.Item className="cmdk-item" value="الرئيسية home" onSelect={() => go('/')}>
              <span className="gi"><Home size={16} /></span>الرئيسية
            </Command.Item>
            {groups.map(g => {
              const Icon = g.icon
              return (
                <Command.Group key={g.key} heading={g.title} className="cmdk-group-wrap">
                  {g.items.map(i => (
                    <Command.Item key={i.key} className="cmdk-item" value={`${i.title} ${g.title} ${i.form}`} onSelect={() => { if (i.href) { window.open(i.href, i.newTab ? "_blank" : "_self", "noopener"); onClose() } else go(`/s/${i.key}`) }}>
                      <span className="gi"><Icon size={16} /></span>
                      {i.title}
                      {i.ported ? <span className="badge ok tag">جاهزة</span> : <span className="badge muted tag">قيد التحويل</span>}
                    </Command.Item>
                  ))}
                </Command.Group>
              )
            })}
          </Command.List>
        </Command>
        <div className="cmdk-foot">
          <span><kbd>↑</kbd> <kbd>↓</kbd> للتنقل</span>
          <span><CornerDownLeft size={12} /> للفتح</span>
        </div>
      </motion.div>
    </>,
    document.body,
  )
}
