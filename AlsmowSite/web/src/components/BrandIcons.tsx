import { siFacebook, siInstagram, siSnapchat, siTiktok, siWhatsapp, siX, siYoutube } from 'simple-icons'
import type { Settings } from '../lib/types'
import { cx } from '../lib/format'

function SimpleIcon({ path, className }: { path: string; className?: string }) {
  return (
    <svg viewBox="0 0 24 24" className={className} fill="currentColor" aria-hidden="true">
      <path d={path} />
    </svg>
  )
}

export const WhatsAppIcon = ({ className }: { className?: string }) => <SimpleIcon path={siWhatsapp.path} className={className} />

type SocialKey = keyof Settings['social']

const SOCIAL: { key: SocialKey; label: string; path?: string }[] = [
  { key: 'x', label: 'X', path: siX.path },
  { key: 'instagram', label: 'إنستجرام', path: siInstagram.path },
  { key: 'snapchat', label: 'سناب شات', path: siSnapchat.path },
  { key: 'tiktok', label: 'تيك توك', path: siTiktok.path },
  { key: 'facebook', label: 'فيسبوك', path: siFacebook.path },
  { key: 'youtube', label: 'يوتيوب', path: siYoutube.path },
  { key: 'linkedin', label: 'لينكدإن' },
]

export function SocialLinks({ social, className, itemClass }: { social: Settings['social']; className?: string; itemClass?: string }) {
  const items = SOCIAL.filter((s) => social?.[s.key])
  if (!items.length) return null
  return (
    <div className={cx('flex flex-wrap gap-2', className)}>
      {items.map((s) => (
        <a
          key={s.key}
          href={social[s.key]}
          target="_blank"
          rel="noopener noreferrer"
          aria-label={s.label}
          title={s.label}
          className={cx('grid h-10 w-10 place-items-center rounded-full transition', itemClass)}
        >
          {s.path ? <SimpleIcon path={s.path} className="h-[18px] w-[18px]" /> : <span className="text-sm font-black">in</span>}
        </a>
      ))}
    </div>
  )
}
