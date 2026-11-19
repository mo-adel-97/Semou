// Builds src/generated/texts.json: every Arabic text written in the source code (web screens + server messages),
// so الإعدادات العامة can list and search all of them. Runs before each build (npm run build).
//   t: the text   k: 'e' = a whole text on its own, 'p' = part of a sentence built with values (numbers, names...)
//   f: files it appears in   s: 'web' | 'api'
import { readdirSync, readFileSync, writeFileSync, mkdirSync, statSync } from 'node:fs'
import { join, relative, basename } from 'node:path'
import { fileURLToPath } from 'node:url'

const WEB = fileURLToPath(new URL('..', import.meta.url))
const API = join(WEB, '..', 'api')
const AR = /[؀-ۿ]/
const out = new Map()

function add(text, partial, file, side) {
  // HTML built in strings (print pages, dialogs): each text between the tags is shown on its own.
  if (/<[a-z/!][^>]*>/i.test(text)) { text.split(/<[^>]*>/).forEach(x => add(x, partial, file, side)); return }
  // pieces of a tag cut by a ${value}: keep only the text
  const t = text.replace(/^[^<>]*?=["'][^>]*>/, '').replace(/^\s*>/, '').replace(/<[^>]*$/, '').replace(/\s+/g, ' ').trim()
  if (!t || !AR.test(t) || t.length > 400) return
  const e = out.get(t) ?? { t, k: partial ? 'p' : 'e', f: new Set(), s: side }
  if (!partial) e.k = 'e'
  e.f.add(file)
  out.set(t, e)
}

function walk(dir, exts, skip) {
  const files = []
  for (const n of readdirSync(dir)) {
    const p = join(dir, n)
    if (skip.some(s => p.includes(s))) continue
    if (statSync(p).isDirectory()) files.push(...walk(p, exts, skip))
    else if (exts.some(x => n.endsWith(x))) files.push(p)
  }
  return files
}

/* ---------------- TypeScript / TSX: strings, template literals, JSX text ---------------- */
function scanTs(src, file) {
  const n = src.length
  const emitRun = run => run.split(/[;=()[\],&|?:!]+/).forEach(x => add(x, false, file, 'web'))
  function code(i, stopAtBrace) {
    let depth = 0, run = ''
    while (i < n) {
      const c = src[i], d = src[i + 1]
      if (c === '/' && d === '/') { emitRun(run); run = ''; while (i < n && src[i] !== '\n') i++; continue }
      if (c === '/' && d === '*') { emitRun(run); run = ''; const e = src.indexOf('*/', i + 2); i = e < 0 ? n : e + 2; continue }
      if (c === '"' || c === "'") {
        emitRun(run); run = ''
        let j = i + 1, s = ''
        while (j < n && src[j] !== c && src[j] !== '\n') { if (src[j] === '\\') { s += src[j + 1]; j += 2 } else s += src[j++] }
        add(s, false, file, 'web'); i = j + 1; continue
      }
      if (c === '`') { emitRun(run); run = ''; i = template(i + 1); continue }
      if (c === '{') { emitRun(run); run = ''; depth++; i++; continue }
      if (c === '}') {
        emitRun(run); run = ''
        if (depth === 0 && stopAtBrace) return i + 1
        depth--; i++; continue
      }
      if (c === '<' || c === '>' || c === '\n') { emitRun(run); run = ''; i++; continue }
      run += c; i++
    }
    emitRun(run)
    return i
  }
  function template(i) {
    const parts = []; let s = '', expr = false
    while (i < n && src[i] !== '`') {
      if (src[i] === '\\') { s += src[i + 1]; i += 2; continue }
      if (src[i] === '$' && src[i + 1] === '{') { parts.push(s); s = ''; expr = true; i = code(i + 2, true); continue }
      s += src[i++]
    }
    parts.push(s)
    parts.forEach(p => add(p, expr, file, 'web'))
    return i + 1
  }
  code(0, false)
}

/* ---------------- C#: normal, verbatim, interpolated and raw strings ---------------- */
function scanCs(src, file) {
  const noComments = src.replace(/^\s*\/\/.*$/gm, '').replace(/\/\*[\s\S]*?\*\//g, '')
  const re = /(\$?)"""([\s\S]*?)"""|(\$@|@\$|\$|@)?"((?:[^"\\\n]|\\.|"")*)"/g
  for (const m of noComments.matchAll(re)) {
    const interpolated = (m[1] ?? '').includes('$') || (m[3] ?? '').includes('$')
    const body = m[2] ?? m[4] ?? ''
    if (!AR.test(body)) continue
    if (m[2] !== undefined) {
      // raw strings are mostly SQL: keep only the quoted Arabic literals inside them
      for (const q of body.matchAll(/N?'([^']*)'/g)) add(q[1], false, file, 'api')
      continue
    }
    if (interpolated) body.split(/\{[^{}]*\}/).forEach(p => add(p.replace(/\\n|\\r/g, ' '), true, file, 'api'))
    else add(body.replace(/\\n|\\r/g, ' '), false, file, 'api')
  }
}

for (const f of walk(join(WEB, 'src'), ['.ts', '.tsx'], ['generated', 'icons.ts', 'vite-env']))
  scanTs(readFileSync(f, 'utf8'), basename(f))
try {
  for (const f of walk(API, ['.cs'], ['bin', 'obj', 'wwwroot']))
    scanCs(readFileSync(f, 'utf8'), relative(API, f).replace(/\\/g, '/').split('/').pop())
} catch { /* the server folder is optional */ }

const list = [...out.values()].map(e => ({ t: e.t, k: e.k, f: [...e.f].sort(), s: e.s })).sort((a, b) => a.t.localeCompare(b.t, 'ar'))
mkdirSync(join(WEB, 'src', 'generated'), { recursive: true })
writeFileSync(join(WEB, 'src', 'generated', 'texts.json'), JSON.stringify(list))
console.log(`texts catalog: ${list.length} texts (${list.filter(x => x.s === 'web').length} interface, ${list.filter(x => x.s === 'api').length} server)`)
