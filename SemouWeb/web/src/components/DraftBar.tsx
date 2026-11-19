import { useLocation, useNavigate } from 'react-router-dom'
import { Eye } from 'lucide-react'
import { useUiConfig } from '../uiConfig'
import { alerts } from '../lib/alerts'

/**
 * Unsaved edits of الإعدادات العامة stay applied as a live preview while moving around the program;
 * this bar says so (only to the editor: nothing reaches other users before saving).
 */
export function DraftBar() {
  const { draft, clearDraft } = useUiConfig()
  const loc = useLocation()
  const nav = useNavigate()
  if ((!draft.menu && !draft.texts) || loc.pathname === '/s/general-settings') return null
  const what = draft.menu && draft.texts ? 'لهيكلة القائمة والنصوص' : draft.menu ? 'لهيكلة القائمة' : 'للنصوص'
  return (
    <div className="draft-bar" data-no-texts data-picker-ui role="status">
      <Eye size={17} />
      <span>تشاهد معاينة {what} غير محفوظة — لا تظهر لبقية المستخدمين بعد.</span>
      <button className="btn sm" onClick={() => nav('/s/general-settings')}>العودة للحفظ</button>
      <button className="btn ghost sm" onClick={async () => { if (await alerts.confirm({ title: 'تجاهل التعديلات غير المحفوظة؟', danger: true, confirmText: 'تجاهل' })) clearDraft() }}>تجاهل</button>
    </div>
  )
}
