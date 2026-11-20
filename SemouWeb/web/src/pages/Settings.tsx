import { useDeferredValue, useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { AnimatePresence, motion, Reorder, useDragControls } from 'motion/react'
import {
  ArrowLeftRight, ChevronDown, Eye, EyeOff, FolderPlus, GripVertical, History, LayoutDashboard, Link2, ListTree, Monitor, MousePointerClick,
  PanelRight, Plus, RotateCcw, Save, Search, Server, Settings2, Smile, Trash2, Type, Undo2, Users, X,
} from 'lucide-react'
import { PageHead } from '../components/Shell'
import { Dialog, Segmented, Spinner, Tabs } from '../components/ui'
import { api } from '../api'
import { useAuth } from '../auth'
import { alerts } from '../lib/alerts'
import { ICON_GROUPS, iconByName } from '../lib/icons'
import { ADD_STYLES, type AddStyle, type TextRule } from '../lib/textOverrides'
import { MENU } from '../menu'
import { textPicker, upsertRule } from '../components/TextPicker'
import {
  DEFAULT_BRAND, buildMenu, completeMenuConfig, defaultIconName, defaultMenuConfig, useUiConfig, type MenuConfig, type TextsConfig,
} from '../uiConfig'

type Tab = 'menu' | 'texts' | 'history'
const esc = (s: string) => s.replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]!))
const BASE_ITEMS = new Map(MENU.flatMap(g => g.items.map(i => [i.key, { item: i, group: g }] as const)))
const same = (a: unknown, b: unknown) => JSON.stringify(a) === JSON.stringify(b)
const newKey = (p: string) => `${p}-${Date.now().toString(36)}${Math.random().toString(36).slice(2, 5)}`

/* =============================== Page =============================== */

export function GeneralSettings({ group }: { group: string }) {
  const { can } = useAuth()
  const ui = useUiConfig()
  const editable = can('websettings', 'edit')
  const [tab, setTab] = useState<Tab>('menu')

  const savedMenu = useMemo(() => completeMenuConfig(ui.saved.menu), [ui.saved.menu])
  const menuCfg = ui.draft.menu ?? savedMenu
  const menuDirty = !!ui.draft.menu && !same(ui.draft.menu, savedMenu)
  const savedRules = useMemo(() => ui.saved.texts?.rules ?? [], [ui.saved.texts])
  const rules = ui.draft.texts?.rules ?? savedRules
  const textsDirty = !!ui.draft.texts && !same(ui.draft.texts.rules, savedRules)

  const setMenu = (fn: (c: MenuConfig) => void) => ui.setDraft(d => {
    const next = structuredClone(d.menu ?? savedMenu); fn(next)
    return { ...d, menu: same(next, savedMenu) ? undefined : next }
  })
  const setRules = (fn: (r: TextRule[]) => TextRule[]) => ui.setDraft(d => {
    const next = fn(d.texts?.rules ?? savedRules)
    return { ...d, texts: same(next, savedRules) ? undefined : { v: 1, rules: next } }
  })

  const pending = ([menuDirty && 'menu', textsDirty && 'texts'].filter(Boolean)) as ('menu' | 'texts')[]
  const NAMES = { menu: 'هيكلة القائمة', texts: 'النصوص' } as const

  /** Saves every section with unsaved edits (structure and/or texts) in one go. */
  const save = async () => {
    if (!pending.length) return
    const v = await alerts.form<{ note: string }>({
      title: `حفظ ${pending.map(x => NAMES[x]).join(' و')}`, confirmText: 'حفظ وتطبيق للجميع', icon: 'question',
      html: `<div class="sm-form"><p class="sm-text">تظهر التعديلات لجميع المستخدمين فور الحفظ، وتُحفظ النسخة الحالية في سجل التعديلات ويمكن استرجاعها في أي وقت.</p>
        <label class="sm-lbl">وصف مختصر للتعديل (اختياري)</label><input id="gs-note" class="input" maxlength="200" placeholder="مثال: نقل طلبات السداد إلى قسم المبيعات" /></div>`,
      read: el => ({ note: (el.querySelector('#gs-note') as HTMLInputElement).value.trim() }),
    })
    if (!v) return
    alerts.loading()
    try {
      for (const section of pending) {
        const data = section === 'menu' ? ui.draft.menu : ui.draft.texts
        await api(`/ui-config/${section}`, { method: 'PUT', json: { data, baseVersion: ui.saved.versions[section] ?? null, note: v.note } })
        ui.setDraft(d => ({ ...d, [section]: undefined }))
      }
      await ui.refetch()
      alerts.success('تم حفظ الإعدادات وتطبيقها على جميع المستخدمين')
    } catch (e) { await ui.refetch(); alerts.error((e as Error).message, 'تعذّر الحفظ') }
  }
  const discard = async () => {
    if (await alerts.confirm({ title: 'تجاهل التعديلات غير المحفوظة؟', text: 'تعود الإعدادات إلى آخر نسخة محفوظة.', danger: true, confirmText: 'تجاهل' }))
      ui.clearDraft()
  }

  return (
    <>
      <PageHead title="الإعدادات العامة" group={group} subtitle="تحكّم في هيكلة النظام ومحتواه ونصوصه بالكامل دون أي تعديل برمجي — مع معاينة مباشرة قبل الحفظ" />
      {!ui.loaded && <div className="gs-note warn"><Spinner /> جاري تحميل الإعدادات الحالية...</div>}
      {!editable && <div className="gs-note"><Eye size={16} /> لديك صلاحية الاطلاع فقط؛ التعديل والحفظ يتطلبان صلاحية «تعديل» على الإعدادات العامة.</div>}

      <div className="gs-tabs">
        <Tabs<Tab> value={tab} onChange={setTab} items={[
          { value: 'menu', label: <span className="gs-tab"><ListTree size={16} /> هيكلة القائمة والمحتوى</span>, badge: menuDirty ? '•' : undefined },
          { value: 'texts', label: <span className="gs-tab"><Type size={16} /> النصوص والمسميات</span>, badge: textsDirty ? '•' : rules.length ? rules.length.toLocaleString('ar-EG') : undefined },
          { value: 'history', label: <span className="gs-tab"><History size={16} /> سجل التعديلات</span> },
        ]} />
      </div>

      <AnimatePresence>
        {pending.length > 0 && editable && (
          <motion.div className="gs-savebar" data-no-texts initial={{ opacity: 0, y: -8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }}>
            <span className="dot" />
            <div><b>تعديلات غير محفوظة: {pending.map(x => NAMES[x]).join(' و')}</b><small>تشاهدها الآن كمعاينة مباشرة في النظام كله، ولا تظهر لبقية المستخدمين قبل الحفظ.</small></div>
            <button className="btn ghost sm" onClick={discard}><Undo2 size={15} /> تجاهل</button>
            <button className="btn" onClick={save}><Save size={16} /> حفظ وتطبيق للجميع</button>
          </motion.div>
        )}
      </AnimatePresence>

      {tab === 'menu' && <MenuEditor cfg={menuCfg} setMenu={setMenu} editable={editable} />}
      {tab === 'texts' && <TextsEditor rules={rules} setRules={setRules} editable={editable} />}
      {tab === 'history' && <HistoryTab onPreview={(section, data) => {
        ui.setDraft(d => ({ ...d, [section]: data }))
        setTab(section)
        alerts.success('تم تحميل النسخة كمعاينة', 'راجعها ثم اضغط «حفظ وتطبيق للجميع» لاسترجاعها، أو «تجاهل» للعودة.')
      }} />}
    </>
  )
}

/* =============================== Structure =============================== */

type IconTarget = { kind: 'group' | 'item'; key: string } | null

function MenuEditor({ cfg, setMenu, editable }: { cfg: MenuConfig; setMenu: (fn: (c: MenuConfig) => void) => void; editable: boolean }) {
  const [open, setOpen] = useState<Set<string>>(() => new Set([cfg.groups[0]?.key]))
  const [q, setQ] = useState('')
  const [iconFor, setIconFor] = useState<IconTarget>(null)
  const dq = useDeferredValue(q.trim())

  const toggle = (k: string) => setOpen(s => { const n = new Set(s); if (n.has(k)) n.delete(k); else n.add(k); return n })
  const titleOf = (k: string) => cfg.links[k]?.title ?? cfg.items[k]?.title ?? BASE_ITEMS.get(k)?.item.title ?? k
  const groupTitle = (k: string) => cfg.groups.find(g => g.key === k)?.title || MENU.find(g => g.key === k)?.title || 'قسم جديد'
  const stats = useMemo(() => {
    const items = cfg.groups.flatMap(g => g.items)
    return {
      groups: cfg.groups.length, hiddenGroups: cfg.groups.filter(g => g.hidden).length, items: items.length,
      hidden: items.filter(k => cfg.items[k]?.hidden).length, renamed: Object.values(cfg.items).filter(x => x.title).length + cfg.groups.filter(g => g.title).length,
      links: Object.keys(cfg.links).length,
    }
  }, [cfg])

  const addGroup = async () => {
    const v = await alerts.form<{ title: string }>({
      title: 'قسم جديد في القائمة', confirmText: 'إضافة',
      html: `<div class="sm-form"><label class="sm-lbl">اسم القسم</label><input id="g-t" class="input" maxlength="60" placeholder="مثال: الخدمات الإلكترونية" /></div>`,
      read: el => { const t = (el.querySelector('#g-t') as HTMLInputElement).value.trim(); return t ? { title: t } : 'يرجى كتابة اسم القسم' },
    })
    if (!v) return
    const key = newKey('group')
    setMenu(c => { c.groups.push({ key, title: v.title, icon: 'LayoutGrid', custom: true, items: [] }) })
    setOpen(s => new Set(s).add(key))
  }
  const addLink = async (groupKey?: string) => {
    const opts = cfg.groups.map(g => `<option value="${esc(g.key)}"${g.key === groupKey ? ' selected' : ''}>${esc(groupTitle(g.key))}</option>`).join('')
    const v = await alerts.form<{ title: string; url: string; group: string; newTab: boolean }>({
      title: 'رابط مخصص في القائمة', confirmText: 'إضافة',
      html: `<div class="sm-form" data-no-texts><label class="sm-lbl">عنوان الرابط</label><input id="l-t" class="input" maxlength="60" placeholder="مثال: بوابة الطالب" />
        <label class="sm-lbl">العنوان (URL)</label><input id="l-u" class="input" dir="ltr" placeholder="https://..." />
        <label class="sm-lbl">القسم</label><select id="l-g" class="input">${opts}</select>
        <label class="switch" style="margin-top:6px"><input id="l-n" type="checkbox" checked /> يفتح في تبويب جديد</label></div>`,
      read: el => {
        const title = (el.querySelector('#l-t') as HTMLInputElement).value.trim(), url = (el.querySelector('#l-u') as HTMLInputElement).value.trim()
        if (!title) return 'يرجى كتابة عنوان الرابط'
        if (!/^(https?:\/\/|\/)/i.test(url)) return 'يجب أن يبدأ العنوان بـ https:// أو http:// أو /'
        return { title, url, group: (el.querySelector('#l-g') as HTMLSelectElement).value, newTab: (el.querySelector('#l-n') as HTMLInputElement).checked }
      },
    })
    if (!v) return
    const key = newKey('link')
    setMenu(c => { c.links[key] = { title: v.title, url: v.url, newTab: v.newTab, icon: 'ExternalLink' }; c.groups.find(g => g.key === v.group)?.items.push(key) })
    setOpen(s => new Set(s).add(v.group))
  }
  const editLink = async (key: string) => {
    const l = cfg.links[key]
    const v = await alerts.form<{ url: string; newTab: boolean }>({
      title: `الرابط: ${l.title}`, confirmText: 'حفظ',
      html: `<div class="sm-form" data-no-texts><label class="sm-lbl">العنوان (URL)</label><input id="l-u" class="input" dir="ltr" value="${esc(l.url)}" />
        <label class="switch" style="margin-top:6px"><input id="l-n" type="checkbox"${l.newTab !== false ? ' checked' : ''} /> يفتح في تبويب جديد</label></div>`,
      read: el => {
        const url = (el.querySelector('#l-u') as HTMLInputElement).value.trim()
        return /^(https?:\/\/|\/)/i.test(url) ? { url, newTab: (el.querySelector('#l-n') as HTMLInputElement).checked } : 'يجب أن يبدأ العنوان بـ https:// أو http:// أو /'
      },
    })
    if (v) setMenu(c => { c.links[key].url = v.url; c.links[key].newTab = v.newTab })
  }
  const removeGroup = async (key: string) => {
    const g = cfg.groups.find(x => x.key === key)!
    if (!await alerts.confirm({ title: `حذف القسم «${groupTitle(key)}»؟`, text: g.items.length ? 'تعود الشاشات التي فيه إلى أقسامها الأصلية، وتُحذف الروابط المخصصة.' : undefined, danger: true, confirmText: 'حذف' })) return
    setMenu(c => {
      const gone = c.groups.find(x => x.key === key)!
      c.groups = c.groups.filter(x => x.key !== key)
      for (const k of gone.items) {
        if (c.links[k]) { delete c.links[k]; continue }
        const home = BASE_ITEMS.get(k)?.group.key
        ;(c.groups.find(x => x.key === home) ?? c.groups[0])?.items.push(k)
      }
    })
  }
  const resetAll = async () => {
    if (await alerts.confirm({ title: 'استعادة الهيكلة الافتراضية؟', text: 'تعود جميع الأقسام والشاشات إلى ترتيبها وأسمائها الأصلية وتُحذف الأقسام والروابط المخصصة. (لا يُحفظ شيء قبل ضغط «حفظ»)', danger: true, confirmText: 'استعادة' }))
      setMenu(c => { Object.assign(c, defaultMenuConfig()) })
  }
  const moveItem = (key: string, to: string) => setMenu(c => {
    c.groups.forEach(g => { g.items = g.items.filter(k => k !== key) })
    c.groups.find(g => g.key === to)?.items.push(key)
  })

  const matches = dq ? cfg.groups.flatMap(g => g.items.filter(k => titleOf(k).includes(dq) || (BASE_ITEMS.get(k)?.item.title ?? '').includes(dq)).map(k => ({ k, g: g.key }))) : []
  const lock = !editable

  return (
    <div className="gs-grid">
      <div className="gs-main">
        <div className="gs-stats">
          <StatPill icon={<ListTree size={16} />} v={stats.groups} l="قسم" />
          <StatPill icon={<LayoutDashboard size={16} />} v={stats.items} l="شاشة ورابط" />
          <StatPill icon={<EyeOff size={16} />} v={stats.hidden + stats.hiddenGroups} l="مخفي" />
          <StatPill icon={<Type size={16} />} v={stats.renamed} l="اسم معدّل" />
          <StatPill icon={<Link2 size={16} />} v={stats.links} l="رابط مخصص" />
        </div>

        <section className="card gs-card">
          <header><span className="ic"><PanelRight size={18} /></span><div><h3>رأس القائمة الجانبية</h3><p>الاسم والوصف الظاهران أعلى القائمة وعناوينها الثابتة</p></div></header>
          <div className="gs-brand" data-no-texts>
            {([['name', 'اسم الجهة'], ['tagline', 'الوصف المختصر'], ['home', 'عنوان الصفحة الرئيسية'], ['sections', 'عنوان قائمة الأقسام']] as const).map(([k, label]) => (
              <label key={k} className="field">
                <span>{label}</span>
                <input className="input" disabled={lock} maxLength={60} value={cfg.brand?.[k] ?? ''} placeholder={DEFAULT_BRAND[k]}
                  onChange={e => setMenu(c => { c.brand = { ...c.brand, [k]: e.target.value || undefined } })} />
              </label>
            ))}
          </div>
        </section>

        <section className="card gs-card">
          <header>
            <span className="ic"><ListTree size={18} /></span>
            <div><h3>الأقسام والشاشات</h3><p>اسحب من <GripVertical size={13} className="inl" /> لإعادة الترتيب • انقر على الأيقونة لتغييرها • اكتب الاسم الجديد مباشرة</p></div>
          </header>
          <div className="gs-toolbar" data-no-texts>
            <label className="gs-search"><Search size={16} /><input placeholder="ابحث عن شاشة..." value={q} onChange={e => setQ(e.target.value)} />{q && <button onClick={() => setQ('')}><X size={14} /></button>}</label>
            <button className="btn ghost sm" onClick={() => setOpen(open.size >= cfg.groups.length ? new Set() : new Set(cfg.groups.map(g => g.key)))}>
              <ChevronDown size={15} /> {open.size >= cfg.groups.length ? 'طي الكل' : 'توسيع الكل'}
            </button>
            {editable && <>
              <button className="btn secondary sm" onClick={addGroup}><FolderPlus size={15} /> قسم جديد</button>
              <button className="btn secondary sm" onClick={() => addLink()}><Link2 size={15} /> رابط مخصص</button>
              <button className="btn ghost sm danger-t" onClick={resetAll}><RotateCcw size={15} /> الافتراضي</button>
            </>}
          </div>

          {dq ? (
            <div className="gs-results" data-no-texts>
              {matches.length === 0 ? <p className="muted">لا توجد شاشة بهذا الاسم</p> : matches.map(({ k, g }) => (
                <ItemRow key={k} k={k} cfg={cfg} setMenu={setMenu} lock={lock} groupKey={g} groupTitle={groupTitle} onIcon={() => setIconFor({ kind: 'item', key: k })} onEditLink={editLink} onMove={moveItem} plain />
              ))}
            </div>
          ) : (
            <Reorder.Group as="div" axis="y" className="gs-groups" values={cfg.groups.map(g => g.key)}
              onReorder={keys => setMenu(c => { c.groups = keys.map(k => c.groups.find(g => g.key === k)!) })}>
              {cfg.groups.map(g => (
                <GroupCard key={g.key} gk={g.key} cfg={cfg} setMenu={setMenu} lock={lock} open={open.has(g.key)} onToggle={() => toggle(g.key)}
                  groupTitle={groupTitle} onIcon={t => setIconFor(t)} onRemove={removeGroup} onAddLink={addLink} onEditLink={editLink} onMove={moveItem} />
              ))}
            </Reorder.Group>
          )}
        </section>
      </div>

      <aside className="gs-aside">
        <SidebarPreview cfg={cfg} />
      </aside>

      <AnimatePresence>
        {iconFor && (
          <IconPicker
            current={iconFor.kind === 'group' ? cfg.groups.find(g => g.key === iconFor.key)?.icon ?? defaultIconName(iconFor.key) : cfg.links[iconFor.key]?.icon ?? cfg.items[iconFor.key]?.icon}
            allowNone={iconFor.kind === 'item'}
            title={iconFor.kind === 'group' ? `أيقونة قسم «${groupTitle(iconFor.key)}»` : `أيقونة «${titleOf(iconFor.key)}»`}
            onClose={() => setIconFor(null)}
            onPick={name => {
              const t = iconFor
              setMenu(c => {
                if (t.kind === 'group') { const g = c.groups.find(x => x.key === t.key)!; g.icon = name ?? undefined }
                else if (c.links[t.key]) c.links[t.key].icon = name ?? undefined
                else { c.items[t.key] = { ...c.items[t.key], icon: name ?? undefined }; if (!Object.values(c.items[t.key]).some(Boolean)) delete c.items[t.key] }
              })
              setIconFor(null)
            }} />
        )}
      </AnimatePresence>
    </div>
  )
}

const StatPill = ({ icon, v, l }: { icon: ReactNode; v: number; l: string }) => (
  <div className="gs-stat"><span>{icon}</span><b>{v.toLocaleString('ar-EG')}</b><small>{l}</small></div>
)

interface RowCommon {
  cfg: MenuConfig; setMenu: (fn: (c: MenuConfig) => void) => void; lock: boolean
  groupTitle: (k: string) => string; onEditLink: (k: string) => void; onMove: (k: string, to: string) => void
}

function GroupCard({ gk, open, onToggle, onIcon, onRemove, onAddLink, ...p }: RowCommon & {
  gk: string; open: boolean; onToggle: () => void; onIcon: (t: IconTarget) => void; onRemove: (k: string) => void; onAddLink: (g: string) => void
}) {
  const { cfg, setMenu, lock } = p
  const g = cfg.groups.find(x => x.key === gk)!
  const base = MENU.find(b => b.key === gk)
  const drag = useDragControls()
  const Icon = iconByName(g.icon) ?? base?.icon ?? iconByName('LayoutGrid')!
  const hiddenCount = g.items.filter(k => cfg.items[k]?.hidden).length
  return (
    <Reorder.Item as="div" value={gk} dragListener={false} dragControls={drag} className={`gs-group ${g.hidden ? 'is-hidden' : ''} ${open ? 'open' : ''}`}
      whileDrag={{ scale: 1.01, boxShadow: '0 18px 40px rgba(0,0,0,.18)', zIndex: 5 }}>
      <div className="gs-ghead" data-no-texts>
        {!lock && <span className="grip" onPointerDown={e => { e.preventDefault(); drag.start(e) }} title="اسحب لتغيير الترتيب"><GripVertical size={18} /></span>}
        <button className="gs-icon" disabled={lock} onClick={() => onIcon({ kind: 'group', key: gk })} title="تغيير الأيقونة"><Icon size={19} /></button>
        <input className="gs-title" disabled={lock} maxLength={60} value={g.title ?? ''} placeholder={base?.title ?? 'اسم القسم'}
          onChange={e => setMenu(c => { c.groups.find(x => x.key === gk)!.title = e.target.value || undefined })} />
        {g.custom && <span className="badge info">قسم مخصص</span>}
        {g.title && base && <span className="gs-orig" title="الاسم الأصلي">{base.title}</span>}
        <span className="gs-count">{g.items.length.toLocaleString('ar-EG')}{hiddenCount ? ` • ${hiddenCount.toLocaleString('ar-EG')} مخفي` : ''}</span>
        <button className={`gs-eye ${g.hidden ? 'off' : ''}`} disabled={lock} title={g.hidden ? 'القسم مخفي — انقر لإظهاره' : 'إخفاء القسم من القائمة'}
          onClick={() => setMenu(c => { const x = c.groups.find(y => y.key === gk)!; x.hidden = !x.hidden || undefined })}>{g.hidden ? <EyeOff size={17} /> : <Eye size={17} />}</button>
        {g.custom && !lock && <button className="gs-eye danger" title="حذف القسم" onClick={() => onRemove(gk)}><Trash2 size={16} /></button>}
        <button className="gs-chev" onClick={onToggle} aria-label="عرض الشاشات"><ChevronDown size={18} /></button>
      </div>
      <AnimatePresence initial={false}>
        {open && (
          <motion.div className="gs-gbody" initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} transition={{ duration: 0.2 }}>
            {g.items.length === 0
              ? <div className="gs-empty">لا توجد شاشات في هذا القسم بعد — انقل إليه شاشات من الأقسام الأخرى عبر <ArrowLeftRight size={13} className="inl" /> أو أضف رابطاً.</div>
              : (
                <Reorder.Group as="div" axis="y" values={g.items} className="gs-items"
                  onReorder={keys => setMenu(c => { c.groups.find(x => x.key === gk)!.items = keys })}>
                  {g.items.map(k => <ItemRow key={k} k={k} groupKey={gk} onIcon={() => onIcon({ kind: 'item', key: k })} {...p} />)}
                </Reorder.Group>
              )}
            {!lock && <button className="gs-addlink" onClick={() => onAddLink(gk)}><Plus size={14} /> إضافة رابط مخصص هنا</button>}
          </motion.div>
        )}
      </AnimatePresence>
    </Reorder.Item>
  )
}

function ItemRow({ k, groupKey, onIcon, plain, cfg, setMenu, lock, groupTitle, onEditLink, onMove }: RowCommon & {
  k: string; groupKey: string; onIcon: () => void; plain?: boolean
}) {
  const drag = useDragControls()
  const link = cfg.links[k]
  const base = BASE_ITEMS.get(k)
  const ic = cfg.items[k]
  const Icon = iconByName(link?.icon ?? ic?.icon)
  const hidden = !!ic?.hidden
  const setItem = (patch: Partial<{ title: string; icon: string; hidden: boolean }>) => setMenu(c => {
    const next = { ...c.items[k], ...patch }
    for (const x of Object.keys(next) as (keyof typeof next)[]) if (!next[x]) delete next[x]
    if (Object.keys(next).length) c.items[k] = next; else delete c.items[k]
  })
  const body = (
    <div className={`gs-item ${hidden ? 'is-hidden' : ''}`} data-no-texts>
      {!lock && !plain && <span className="grip" onPointerDown={e => { e.preventDefault(); drag.start(e) }}><GripVertical size={16} /></span>}
      <button className={`gs-icon sm ${Icon ? '' : 'no-ic'}`} disabled={lock} onClick={onIcon} title="أيقونة (اختياري)">{Icon ? <Icon size={16} /> : <Smile size={15} />}</button>
      {link
        ? <input className="gs-title" disabled={lock} maxLength={60} value={link.title} onChange={e => setMenu(c => { c.links[k].title = e.target.value })} />
        : <input className="gs-title" disabled={lock} maxLength={80} value={ic?.title ?? ''} placeholder={base?.item.title} onChange={e => setItem({ title: e.target.value })} />}
      <span className="gs-tags">
        {link ? <span className="badge info"><Link2 size={11} /> رابط</span>
          : base?.item.ported ? <span className="badge ok">جاهزة</span> : <span className="badge muted">قريباً</span>}
        {ic?.title && base && <span className="gs-orig" title="الاسم الأصلي">{base.item.title}</span>}
        {plain && <span className="gs-orig">في: {groupTitle(groupKey)}</span>}
      </span>
      {link && !lock && <button className="gs-eye" title="تعديل العنوان" onClick={() => onEditLink(k)}><Settings2 size={15} /></button>}
      {!lock && (
        <label className="gs-move" title="نقل إلى قسم آخر">
          <ArrowLeftRight size={15} />
          <select value={groupKey} onChange={e => onMove(k, e.target.value)}>
            {cfg.groups.map(g => <option key={g.key} value={g.key}>{groupTitle(g.key)}</option>)}
          </select>
        </label>
      )}
      {link
        ? !lock && <button className="gs-eye danger" title="حذف الرابط" onClick={() => setMenu(c => { delete c.links[k]; c.groups.forEach(g => { g.items = g.items.filter(x => x !== k) }) })}><Trash2 size={15} /></button>
        : <button className={`gs-eye ${hidden ? 'off' : ''}`} disabled={lock} title={hidden ? 'مخفية — انقر لإظهارها' : 'إخفاء من القائمة'} onClick={() => setItem({ hidden: !hidden })}>{hidden ? <EyeOff size={16} /> : <Eye size={16} />}</button>}
    </div>
  )
  if (plain) return body
  return <Reorder.Item as="div" value={k} dragListener={false} dragControls={drag} whileDrag={{ scale: 1.02, boxShadow: '0 12px 28px rgba(0,0,0,.16)', zIndex: 5 }}>{body}</Reorder.Item>
}

/* ---------------- Icon picker ---------------- */

function IconPicker({ current, allowNone, title, onPick, onClose }: { current?: string; allowNone: boolean; title: string; onPick: (n: string | null) => void; onClose: () => void }) {
  const [q, setQ] = useState('')
  const t = q.trim().toLowerCase()
  const groups = ICON_GROUPS.map(g => ({ ...g, icons: g.icons.filter(i => !t || i.words.includes(t) || i.name.toLowerCase().includes(t)) })).filter(g => g.icons.length)
  return (
    <Dialog title={title} subtitle="اختر أيقونة من المكتبة" onClose={onClose} size="lg">
      <div className="ip" data-no-texts>
        <label className="gs-search"><Search size={16} /><input autoFocus placeholder="ابحث بالعربية: تقارير، طلاب، مال..." value={q} onChange={e => setQ(e.target.value)} /></label>
        {allowNone && <button className={`ip-none ${!current ? 'on' : ''}`} onClick={() => onPick(null)}><X size={15} /> بدون أيقونة</button>}
        <div className="ip-scroll">
          {groups.map(g => (
            <div key={g.title} className="ip-group">
              <h4>{g.title}</h4>
              <div className="ip-grid">
                {g.icons.map(i => { const I = i.icon; return (
                  <button key={i.name} className={current === i.name ? 'on' : ''} title={i.words} onClick={() => onPick(i.name)}><I size={21} /><small>{i.words.split(' ')[0]}</small></button>
                ) })}
              </div>
            </div>
          ))}
          {groups.length === 0 && <p className="muted" style={{ textAlign: 'center', padding: 30 }}>لا توجد أيقونة بهذا الاسم</p>}
        </div>
      </div>
    </Dialog>
  )
}

/* ---------------- Live preview ---------------- */

function SidebarPreview({ cfg }: { cfg: MenuConfig }) {
  const { can, menu: menuRight } = useAuth()
  const [who, setWho] = useState<'all' | 'me'>('all')
  const [openG, setOpenG] = useState<string | null>(null)
  const groups = useMemo(() => buildMenu(cfg)
    .filter(g => !g.hidden)
    .map(g => ({ ...g, items: g.items.filter(i => !i.hidden && (who === 'all' || ((i.originMenus ?? []).every(menuRight) && (i.perm === null || can(i.perm, 'view'))))) }))
    .filter(g => g.items.length > 0), [cfg, who, can, menuRight])
  const b = { ...DEFAULT_BRAND, ...Object.fromEntries(Object.entries(cfg.brand ?? {}).filter(([, v]) => v?.trim())) }
  const shown = openG ?? groups[0]?.key
  return (
    <div className="gs-preview">
      <div className="gs-preview-head">
        <span><Monitor size={16} /> معاينة مباشرة</span>
        <Segmented<'all' | 'me'> value={who} onChange={setWho} options={[{ value: 'all', label: 'كل الشاشات' }, { value: 'me', label: 'بصلاحياتي' }]} />
      </div>
      <div className="side gs-side" data-no-texts>
        <div className="side-brand">
          <span className="logo"><img src="/logo.png" alt="" /></span>
          <span className="txt"><b>{b.name}</b><small>{b.tagline}</small></span>
        </div>
        <nav className="side-nav">
          <span className="nav-link"><LayoutDashboard size={19} /><span>{b.home}</span></span>
          <div className="side-label">{b.sections}</div>
          {groups.map(g => {
            const I = g.icon; const o = shown === g.key
            return (
              <div key={g.key}>
                <button className={`nav-group ${o ? 'open' : ''}`} onClick={() => setOpenG(o ? '__none' : g.key)}><I size={19} /><span>{g.title}</span><ChevronDown size={16} className="chev" /></button>
                {o && (
                  <div className="nav-sub"><div style={{ padding: '2px 0 6px' }}>
                    {g.items.map(i => { const Ic = i.icon; return (
                      <a key={i.key} className={Ic ? 'with-ic' : ''} onClick={e => e.preventDefault()} href="#">{Ic && <Ic size={15} className="ic" />}{i.title}{i.href ? <Link2 size={12} className="ext" /> : i.ported && <span className="new">جاهزة</span>}</a>
                    ) })}
                  </div></div>
                )}
              </div>
            )
          })}
          {groups.length === 0 && <p style={{ color: 'rgba(255,255,255,.6)', padding: 12, fontSize: 13 }}>لا توجد أقسام ظاهرة</p>}
        </nav>
      </div>
      <p className="gs-preview-foot"><Users size={14} /> القائمة الحقيقية على يمين الشاشة تتغير معك مباشرة أيضاً. الصلاحيات لا تتأثر: كل مستخدم يرى ما يحق له فقط.</p>
    </div>
  )
}

/* =============================== Texts =============================== */

interface CatalogEntry { t: string; k: 'e' | 'p'; f: string[]; s: 'web' | 'api' }
type TextFilter = 'all' | 'changed' | 'web' | 'api'
const PAGE = 60
const screenName = (path: string) => (path === '/' ? 'الرئيسية' : MENU.flatMap(g => g.items).find(i => `/s/${i.key}` === path)?.title ?? path)

function TextsEditor({ rules, setRules, editable }: { rules: TextRule[]; setRules: (fn: (r: TextRule[]) => TextRule[]) => void; editable: boolean }) {
  const [catalog, setCatalog] = useState<CatalogEntry[] | null>(null)
  const [q, setQ] = useState('')
  const [filter, setFilter] = useState<TextFilter>('all')
  const [limit, setLimit] = useState(PAGE)
  const dq = useDeferredValue(q.trim())
  useEffect(() => { import('../generated/texts.json').then(m => setCatalog(m.default as CatalogEntry[])) }, [])
  useEffect(() => { setLimit(PAGE) }, [dq, filter])

  const edits = useMemo(() => rules.filter(r => !r.add), [rules])
  const adds = useMemo(() => rules.filter(r => r.add), [rules])
  const byFrom = useMemo(() => new Map(edits.map(r => [r.from.trim(), r])), [edits])
  const custom = useMemo(() => (catalog ? edits.filter(r => !catalog.some(c => c.t === r.from.trim())) : []), [edits, catalog])
  const list = useMemo(() => (catalog ?? []).filter(c =>
    (filter === 'all' || (filter === 'changed' ? byFrom.has(c.t) : c.s === filter)) &&
    (!dq || c.t.includes(dq) || byFrom.get(c.t)?.to.includes(dq) || c.f.some(f => f.toLowerCase().includes(dq.toLowerCase())))), [catalog, filter, dq, byFrom])

  const setRule = (from: string, to: string, mode: TextRule['mode'], remove?: boolean, whole?: boolean) =>
    setRules(r => upsertRule(r, { from, to: remove ? '' : to, mode: whole ? 'exact' : mode, remove: remove || undefined, whole: (remove && whole) || undefined }))
  /** A new text placed before / after the line that holds an existing text (the anchor). */
  const editAdded = async (cur?: TextRule) => {
    const styles = (Object.keys(ADD_STYLES) as AddStyle[]).map(k => `<option value="${k}"${(cur?.style ?? 'note') === k ? ' selected' : ''}>${ADD_STYLES[k]}</option>`).join('')
    const v = await alerts.form<TextRule>({
      title: cur ? 'تعديل النص المضاف' : 'إضافة نص جديد في الشاشات', confirmText: cur ? 'حفظ' : 'إضافة', wide: true,
      html: `<div class="sm-form tp-form" data-no-texts>
        <label class="sm-lbl">النص المضاف</label><textarea id="a-t" class="input" rows="3" maxlength="600" placeholder="اكتب النص الذي تريد إضافته...">${esc(cur?.to ?? '')}</textarea>
        <label class="sm-lbl">يظهر بجوار السطر الذي فيه النص</label><input id="a-f" class="input" maxlength="300" value="${esc(cur?.from ?? '')}" placeholder="انسخ نصاً موجوداً في الشاشة، مثال: ابدأ بالبحث عن الطالب" />
        <div class="tp-row">
          <div><label class="sm-lbl">مكانه</label><select id="a-w" class="input"><option value="after"${cur?.add !== 'before' ? ' selected' : ''}>بعد هذا السطر</option><option value="before"${cur?.add === 'before' ? ' selected' : ''}>قبل هذا السطر</option></select></div>
          <div><label class="sm-lbl">شكله</label><select id="a-s" class="input">${styles}</select></div>
        </div>
        <p class="sm-hint">الأسهل: فعّل «وضع النقر» وانقر على السطر في الشاشة نفسها ثم اختر «إضافة نص جديد هنا».</p></div>`,
      read: el => {
        const to = (el.querySelector('#a-t') as HTMLTextAreaElement).value.trim(), from = (el.querySelector('#a-f') as HTMLInputElement).value.trim()
        if (!to) return 'يرجى كتابة النص المضاف'
        if (!from) return 'يرجى تحديد النص الذي يظهر بجواره'
        return { from, to, mode: 'exact', add: (el.querySelector('#a-w') as HTMLSelectElement).value as 'before' | 'after', style: (el.querySelector('#a-s') as HTMLSelectElement).value as AddStyle, path: cur?.path }
      },
    })
    if (!v) return
    setRules(r => upsertRule(cur ? r.filter(x => x !== cur) : r, v))
  }
  const addManual = async () => {
    const v = await alerts.form<TextRule>({
      title: 'قاعدة استبدال يدوية', confirmText: 'إضافة',
      html: `<div class="sm-form" data-no-texts><label class="sm-lbl">النص الحالي كما يظهر في النظام</label><input id="m-f" class="input" maxlength="300" />
        <label class="sm-lbl">يُستبدل بـ</label><input id="m-t" class="input" maxlength="300" />
        <label class="sm-lbl">طريقة الاستبدال</label><select id="m-m" class="input"><option value="exact">مطابقة النص كاملاً</option><option value="contains">أينما وردت العبارة داخل النصوص</option></select></div>`,
      read: el => {
        const from = (el.querySelector('#m-f') as HTMLInputElement).value.trim(), to = (el.querySelector('#m-t') as HTMLInputElement).value.trim()
        if (!from || !to) return 'يرجى كتابة النص الحالي والنص الجديد'
        return { from, to, mode: (el.querySelector('#m-m') as HTMLSelectElement).value as TextRule['mode'] }
      },
    })
    if (v) setRules(r => upsertRule(r, v))
  }

  return (
    <div className="gs-texts">
      <div className="gs-texts-hero">
        <div className="gs-stats">
          <StatPill icon={<Type size={16} />} v={catalog?.length ?? 0} l="نص في البرنامج" />
          <StatPill icon={<Monitor size={16} />} v={catalog?.filter(c => c.s === 'web').length ?? 0} l="في الواجهة" />
          <StatPill icon={<Server size={16} />} v={catalog?.filter(c => c.s === 'api').length ?? 0} l="رسالة من الخادم" />
          <StatPill icon={<Save size={16} />} v={edits.length} l="نص معدّل" />
          <StatPill icon={<Plus size={16} />} v={adds.length} l="نص مضاف" />
        </div>
        {editable && (
          <div className="gs-pick">
            <div><b><MousePointerClick size={17} /> عدّل أي نص بالنقر عليه</b><small>فعّل الوضع ثم تنقّل في أي شاشة وانقر على الكلمة التي تريد تغييرها.</small></div>
            <button className="btn" onClick={() => textPicker.start()}><MousePointerClick size={16} /> تفعيل وضع النقر</button>
          </div>
        )}
      </div>

      <section className="card gs-card">
        <div className="gs-toolbar" data-no-texts>
          <label className="gs-search grow"><Search size={16} /><input placeholder="ابحث في نصوص البرنامج أو اسم الملف..." value={q} onChange={e => setQ(e.target.value)} />{q && <button onClick={() => setQ('')}><X size={14} /></button>}</label>
          <Segmented<TextFilter> value={filter} onChange={setFilter} options={[
            { value: 'all', label: 'الكل' }, { value: 'changed', label: `المعدّلة (${edits.length.toLocaleString('ar-EG')})` }, { value: 'web', label: 'الواجهة' }, { value: 'api', label: 'رسائل الخادم' },
          ]} />
          {editable && <button className="btn sm" onClick={() => editAdded()}><Plus size={15} /> إضافة نص</button>}
          {editable && <button className="btn secondary sm" onClick={addManual}><Type size={15} /> قاعدة يدوية</button>}
        </div>

        {!catalog ? <div className="center-fill" style={{ minHeight: 200 }}><Spinner size={26} /></div> : (
          <div className="gs-tlist" data-no-texts>
            {adds.length > 0 && filter !== 'web' && filter !== 'api' && !dq && (
              <>
                <div className="gs-tsec">النصوص المضافة ({adds.length.toLocaleString('ar-EG')})</div>
                {adds.map(r => (
                  <div key={`${r.add}|${r.from}`} className="gs-trow gs-added">
                    <div className="gs-tfrom">
                      <span className={`tx-add tx-${r.style ?? 'text'} gs-added-prev`}>{r.to}</span>
                      <span className="meta"><span className="badge info"><Plus size={11} /> {r.add === 'before' ? 'قبل' : 'بعد'}</span><span className="file" dir="rtl">«{r.from.length > 60 ? r.from.slice(0, 60) + '…' : r.from}»</span><span className="file" dir="rtl">{ADD_STYLES[r.style ?? 'text']}</span><span className="file" dir="rtl">{r.path ? `في شاشة: ${screenName(r.path)}` : 'في كل الشاشات'}</span></span>
                    </div>
                    {editable && (
                      <div className="gs-tto gs-added-acts">
                        <button className="btn ghost sm" onClick={() => editAdded(r)}><Settings2 size={15} /> تعديل</button>
                        <button className="gs-eye danger" title="حذف النص المضاف" onClick={() => setRules(x => x.filter(y => y !== r))}><Trash2 size={15} /></button>
                      </div>
                    )}
                  </div>
                ))}
              </>
            )}
            {(custom.length > 0 || adds.length > 0) && filter !== 'web' && filter !== 'api' && !dq && custom.length === 0 && <div className="gs-tsec">نصوص البرنامج</div>}
            {custom.length > 0 && filter !== 'web' && filter !== 'api' && !dq && (
              <>
                <div className="gs-tsec">قواعد مخصصة ({custom.length.toLocaleString('ar-EG')})</div>
                {custom.map(r => <TextRow key={'c:' + r.from} from={r.from} rule={r} kind={r.mode === 'contains' ? 'p' : 'e'} files={[]} side="custom" lock={!editable} onSet={setRule} />)}
                <div className="gs-tsec">نصوص البرنامج</div>
              </>
            )}
            {list.slice(0, limit).map(c => <TextRow key={c.t} from={c.t} rule={byFrom.get(c.t)} kind={c.k} files={c.f} side={c.s} lock={!editable} onSet={setRule} />)}
            {list.length === 0 && <p className="muted" style={{ textAlign: 'center', padding: 30 }}>لا توجد نصوص مطابقة</p>}
            {list.length > limit && <button className="btn ghost gs-more" onClick={() => setLimit(l => l + PAGE * 2)}>عرض المزيد ({(list.length - limit).toLocaleString('ar-EG')} نص متبقٍ)</button>}
          </div>
        )}
      </section>
    </div>
  )
}

function TextRow({ from, rule, kind, files, side, lock, onSet }: {
  from: string; rule?: TextRule; kind: 'e' | 'p'; files: string[]; side: 'web' | 'api' | 'custom'; lock: boolean; onSet: (from: string, to: string, mode: TextRule['mode'], remove?: boolean, whole?: boolean) => void
}) {
  const [val, setVal] = useState(rule?.to ?? '')
  const removed = !!rule?.remove
  const mode = rule?.mode ?? (kind === 'p' ? 'contains' : 'exact')
  const t = useRef(0)
  useEffect(() => { setVal(rule?.to ?? '') }, [rule?.to])
  const commit = (v: string, m = mode) => { window.clearTimeout(t.current); onSet(from, v.trim(), m) }
  return (
    <div className={`gs-trow ${rule ? 'changed' : ''} ${removed ? 'removed' : ''}`}>
      <div className="gs-tfrom">
        <span className="txt">{from}</span>
        <span className="meta">
          {side === 'api' ? <span className="badge muted"><Server size={11} /> الخادم</span> : side === 'custom' ? <span className="badge info">مخصص</span> : null}
          {removed && <span className="badge off"><EyeOff size={11} /> {rule?.whole ? 'السطر كاملاً محذوف' : 'محذوف من الواجهة'}</span>}
          {kind === 'p' && <span className="badge warn" title="هذا النص يُركَّب مع أرقام أو أسماء عند العرض">جزء من جملة</span>}
          {files.slice(0, 3).map(f => <span key={f} className="file">{f.replace(/\.(tsx?|cs)$/, '')}</span>)}{files.length > 3 && <span className="file">+{files.length - 3}</span>}
        </span>
      </div>
      <div className="gs-tto">
        <input className="input" disabled={lock || removed} value={removed ? '' : val} placeholder={removed ? 'هذا النص محذوف ولن يظهر في أي شاشة' : 'اكتب النص البديل...'} maxLength={400}
          onChange={e => { const v = e.target.value; setVal(v); window.clearTimeout(t.current); t.current = window.setTimeout(() => commit(v), 450) }}
          onBlur={e => commit(e.target.value)} onKeyDown={e => { if (e.key === 'Enter') commit((e.target as HTMLInputElement).value) }} />
        <select className="input mode" disabled={lock} value={mode} onChange={e => { if (removed) onSet(from, '', e.target.value as TextRule['mode'], true); else if (val.trim()) commit(val, e.target.value as TextRule['mode']) }} title="طريقة الاستبدال">
          <option value="exact">نص كامل</option><option value="contains">داخل الجمل</option>
        </select>
        {!lock && !removed && <button className="gs-eye danger" title="حذف النص من الواجهة" onClick={async () => {
          const v = await alerts.form<{ act: 'text' | 'line' }>({
            title: 'حذف النص من الواجهة', confirmText: 'حذف', icon: 'warning',
            html: `<div class="sm-form tp-form" data-no-texts><div class="tp-orig">${esc(from)}</div>
              <label class="tp-opt"><input type="radio" name="act" value="text" checked /> <span><b>حذف هذا النص فقط</b><small>تبقى بقية الجملة أو الأيقونة المجاورة له إن وُجدت</small></span></label>
              <label class="tp-opt"><input type="radio" name="act" value="line" /> <span><b>حذف السطر / العنصر كاملاً</b><small>يختفي العنصر الذي يحتوي النص بكل ما فيه (مناسب للجمل التي بها أيقونة أو أجزاء أخرى)</small></span></label>
              <p class="sm-hint">يمكنك إرجاعه في أي وقت من زر الإرجاع.</p></div>`,
            read: el => ({ act: (el.querySelector('input[name=act]:checked') as HTMLInputElement).value as 'text' | 'line' }),
          })
          if (!v) return
          window.clearTimeout(t.current); setVal(''); onSet(from, '', mode, true, v.act === 'line')
        }}><Trash2 size={15} /></button>}
        {rule && !lock && <button className="gs-eye" title="إرجاع النص الأصلي" onClick={() => { setVal(''); window.clearTimeout(t.current); onSet(from, '', mode, false) }}><RotateCcw size={15} /></button>}
      </div>
    </div>
  )
}

/* =============================== History =============================== */

interface Version { id: number; savedAt: string; user: string | null; note: string | null; size: number }

function HistoryTab({ onPreview }: { onPreview: (section: 'menu' | 'texts', data: MenuConfig & TextsConfig) => void }) {
  return (
    <div className="gs-history">
      <HistoryList section="menu" title="هيكلة القائمة والمحتوى" icon={<ListTree size={18} />} onPreview={onPreview} />
      <HistoryList section="texts" title="النصوص والمسميات" icon={<Type size={18} />} onPreview={onPreview} />
    </div>
  )
}

function HistoryList({ section, title, icon, onPreview }: { section: 'menu' | 'texts'; title: string; icon: ReactNode; onPreview: (s: 'menu' | 'texts', d: MenuConfig & TextsConfig) => void }) {
  const q = useQuery({ queryKey: ['ui-config', 'history', section], queryFn: () => api<Version[]>(`/ui-config/${section}/history`) })
  const fmt = new Intl.DateTimeFormat('ar-EG', { dateStyle: 'medium', timeStyle: 'short' })
  const load = async (id: number) => {
    try { const r = await api<{ data: MenuConfig & TextsConfig }>(`/ui-config/${section}/history/${id}`); onPreview(section, r.data) }
    catch (e) { alerts.error((e as Error).message) }
  }
  return (
    <section className="card gs-card">
      <header><span className="ic">{icon}</span><div><h3>{title}</h3><p>آخر ٤٠ نسخة محفوظة — حمّل أي نسخة لمعاينتها ثم احفظها لاسترجاعها</p></div></header>
      {q.isLoading ? <div className="center-fill" style={{ minHeight: 120 }}><Spinner /></div>
        : !q.data?.length ? <p className="muted" style={{ padding: '6px 4px 14px' }}>لم تُحفظ أي تعديلات بعد — يعمل النظام بالإعدادات الافتراضية.</p>
        : (
          <ol className="gs-vers">
            {q.data.map((v, n) => (
              <li key={v.id} className={n === 0 ? 'current' : ''}>
                <span className="dot" />
                <div><b>{v.note || (n === 0 ? 'النسخة الحالية' : 'تعديل بدون وصف')}</b><small>{v.user ?? '—'} • {fmt.format(new Date(v.savedAt))}</small></div>
                {n === 0 ? <span className="badge ok">الحالية</span> : <button className="btn ghost sm" onClick={() => load(v.id)}><Eye size={14} /> معاينة واسترجاع</button>}
              </li>
            ))}
          </ol>
        )}
    </section>
  )
}
