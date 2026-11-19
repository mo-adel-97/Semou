import Image from '@tiptap/extension-image'
import TextAlign from '@tiptap/extension-text-align'
import { EditorContent, useEditor, useEditorState, type Editor } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import {
  AlignCenter, AlignJustify, AlignLeft, AlignRight, Bold, Heading2, Heading3, ImagePlus, Italic, Link2, List, ListOrdered,
  Loader2, Minus, Quote, Redo2, Strikethrough, Underline, Undo2, Unlink,
} from 'lucide-react'
import { useEffect, useRef, type ReactNode } from 'react'
import Swal from 'sweetalert2'
import { cx } from '../lib/format'
import { useUpload } from './fields'

export default function RichEditor({ value, onChange, invalid }: { value: string; onChange: (html: string) => void; invalid?: boolean }) {
  const editor = useEditor({
    extensions: [
      StarterKit.configure({
        heading: { levels: [2, 3, 4] },
        link: { openOnClick: false, autolink: true, HTMLAttributes: { rel: 'noopener noreferrer' } },
      }),
      Image,
      TextAlign.configure({ types: ['heading', 'paragraph'], defaultAlignment: 'right' }),
    ],
    content: value || '',
    editorProps: {
      attributes: { class: 'prose-ar min-h-72 px-5 py-4 focus:outline-none', dir: 'rtl' },
    },
    onUpdate: ({ editor }) => onChange(editor.isEmpty ? '' : editor.getHTML()),
    immediatelyRender: true,
  })

  // External value replaced (e.g. record loaded after mount)
  useEffect(() => {
    // StrictMode mounts twice: never touch an editor instance that was already destroyed.
    if (!editor || editor.isDestroyed || editor.isFocused) return
    const current = editor.isEmpty ? '' : editor.getHTML()
    if ((value || '') !== current) editor.commands.setContent(value || '', { emitUpdate: false })
  }, [editor, value])

  if (!editor || editor.isDestroyed) return null
  return (
    <div className={cx('overflow-hidden rounded-2xl border bg-surface transition focus-within:border-brand-500 focus-within:ring-4 focus-within:ring-brand-500/15', invalid ? 'border-red-400' : 'border-line')}>
      <Toolbar editor={editor} />
      <EditorContent editor={editor} />
    </div>
  )
}

function Toolbar({ editor }: { editor: Editor }) {
  const upload = useUpload()
  const file = useRef<HTMLInputElement>(null)
  const s = useEditorState({
    editor,
    selector: ({ editor: e }) => ({
      bold: e.isActive('bold'), italic: e.isActive('italic'), underline: e.isActive('underline'), strike: e.isActive('strike'),
      h2: e.isActive('heading', { level: 2 }), h3: e.isActive('heading', { level: 3 }),
      ul: e.isActive('bulletList'), ol: e.isActive('orderedList'), quote: e.isActive('blockquote'), link: e.isActive('link'),
      right: e.isActive({ textAlign: 'right' }), center: e.isActive({ textAlign: 'center' }), left: e.isActive({ textAlign: 'left' }), justify: e.isActive({ textAlign: 'justify' }),
      undo: e.can().undo(), redo: e.can().redo(),
    }),
  })
  const c = () => editor.chain().focus()

  const setLink = async () => {
    const prev = editor.getAttributes('link').href as string | undefined
    const r = await Swal.fire({ title: 'إضافة رابط', input: 'url', inputValue: prev ?? 'https://', showCancelButton: true, confirmButtonText: 'حفظ', cancelButtonText: 'إلغاء', inputAttributes: { dir: 'ltr' } })
    if (!r.isConfirmed) return
    if (!r.value) c().unsetLink().run()
    else c().extendMarkRange('link').setLink({ href: r.value, target: /^https?:/.test(r.value) && !r.value.includes(location.host) ? '_blank' : null }).run()
  }

  const B = ({ on, active, disabled, title, children }: { on: () => void; active?: boolean; disabled?: boolean; title: string; children: ReactNode }) => (
    <button
      type="button"
      onMouseDown={(e) => e.preventDefault()}
      onClick={on}
      disabled={disabled}
      title={title}
      aria-label={title}
      aria-pressed={active}
      className={cx('grid h-8 w-8 place-items-center rounded-lg transition disabled:opacity-30', active ? 'bg-primary text-white' : 'text-ink-2 hover:bg-sand')}
    >
      {children}
    </button>
  )
  const Sep = () => <span className="mx-1 h-5 w-px bg-line" />
  const i = 'h-4 w-4'

  return (
    <div className="sticky top-[57px] z-10 flex flex-wrap items-center gap-0.5 border-b border-line bg-sand p-1.5">
      <B on={() => c().toggleHeading({ level: 2 }).run()} active={s.h2} title="عنوان رئيسي"><Heading2 className={i} /></B>
      <B on={() => c().toggleHeading({ level: 3 }).run()} active={s.h3} title="عنوان فرعي"><Heading3 className={i} /></B>
      <Sep />
      <B on={() => c().toggleBold().run()} active={s.bold} title="عريض"><Bold className={i} /></B>
      <B on={() => c().toggleItalic().run()} active={s.italic} title="مائل"><Italic className={i} /></B>
      <B on={() => c().toggleUnderline().run()} active={s.underline} title="تسطير"><Underline className={i} /></B>
      <B on={() => c().toggleStrike().run()} active={s.strike} title="يتوسطه خط"><Strikethrough className={i} /></B>
      <Sep />
      <B on={() => c().toggleBulletList().run()} active={s.ul} title="قائمة نقطية"><List className={i} /></B>
      <B on={() => c().toggleOrderedList().run()} active={s.ol} title="قائمة مرقمة"><ListOrdered className={i} /></B>
      <B on={() => c().toggleBlockquote().run()} active={s.quote} title="اقتباس"><Quote className={i} /></B>
      <B on={() => c().setHorizontalRule().run()} title="خط فاصل"><Minus className={i} /></B>
      <Sep />
      <B on={() => c().setTextAlign('right').run()} active={s.right} title="محاذاة لليمين"><AlignRight className={i} /></B>
      <B on={() => c().setTextAlign('center').run()} active={s.center} title="توسيط"><AlignCenter className={i} /></B>
      <B on={() => c().setTextAlign('left').run()} active={s.left} title="محاذاة لليسار"><AlignLeft className={i} /></B>
      <B on={() => c().setTextAlign('justify').run()} active={s.justify} title="ضبط"><AlignJustify className={i} /></B>
      <Sep />
      <B on={setLink} active={s.link} title="رابط"><Link2 className={i} /></B>
      {s.link && <B on={() => c().unsetLink().run()} title="إزالة الرابط"><Unlink className={i} /></B>}
      <B on={() => file.current?.click()} title="إدراج صورة">{upload.isPending ? <Loader2 className={cx(i, 'animate-spin')} /> : <ImagePlus className={i} />}</B>
      <span className="flex-1" />
      <B on={() => c().undo().run()} disabled={!s.undo} title="تراجع"><Undo2 className={i} /></B>
      <B on={() => c().redo().run()} disabled={!s.redo} title="إعادة"><Redo2 className={i} /></B>
      <input
        ref={file}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        className="hidden"
        onChange={(e) => {
          const f = e.target.files?.[0]
          e.target.value = ''
          if (f) upload.mutate(f, { onSuccess: (r) => c().setImage({ src: r.url, alt: '' }).run() })
        }}
      />
    </div>
  )
}
