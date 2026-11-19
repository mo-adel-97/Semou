import { motion } from 'motion/react'
import { Hammer } from 'lucide-react'
import type { MenuItem } from '../menu'
import { PageHead } from '../components/Shell'

export function ComingSoon({ item, group }: { item: MenuItem; group: string }) {
  return (
    <>
      <PageHead title={item.title} group={group} />
      <div className="card soon">
        <motion.div className="ic" animate={{ rotate: [0, -12, 10, -6, 0] }} transition={{ duration: 1.2, repeat: Infinity, repeatDelay: 2 }}>
          <Hammer size={38} />
        </motion.div>
        <h2>هذه الشاشة قيد التحويل إلى نسخة الويب</h2>
        <p>الصلاحيات مطبقة عليها من الآن، وسيتم نقل الشاشة بنفس منطق البرنامج السابق (<code>{item.form}</code>).</p>
      </div>
    </>
  )
}
