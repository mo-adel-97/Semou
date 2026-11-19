import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { createReadStream, statSync } from 'node:fs'
import { extname, resolve, sep } from 'node:path'
import { defineConfig, type Plugin } from 'vite'

// Local dev talks to the real PHP API on the staging server (no local PHP needed).
// The session cookie is SameSite=Strict + path /api, so it works through this proxy.
const API = process.env.ALSMOW_API ?? 'https://new.alsmow.com'
// With a local PHP API (php -S, one request at a time), images would queue behind slow API calls,
// so Vite serves ../uploads itself.
const LOCAL_API = /^https?:\/\/(127\.0\.0\.1|localhost)(:|\/|$)/.test(API)
const UPLOADS = resolve(__dirname, '../uploads')
const TYPES: Record<string, string> = {
  '.webp': 'image/webp', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.png': 'image/png', '.gif': 'image/gif',
  '.svg': 'image/svg+xml', '.avif': 'image/avif', '.pdf': 'application/pdf', '.mp4': 'video/mp4', '.ico': 'image/x-icon',
}

function localUploads(): Plugin {
  return {
    name: 'local-uploads',
    configureServer(server) {
      server.middlewares.use('/uploads', (req, res, next) => {
        const rel = decodeURIComponent((req.url ?? '/').split('?')[0])
        const file = resolve(UPLOADS, '.' + rel)
        if (!file.startsWith(UPLOADS + sep)) return next()
        try {
          const st = statSync(file)
          if (!st.isFile()) return next()
          res.setHeader('Content-Type', TYPES[extname(file).toLowerCase()] ?? 'application/octet-stream')
          res.setHeader('Content-Length', st.size)
          res.setHeader('Cache-Control', 'public, max-age=86400')
          createReadStream(file).pipe(res)
        } catch {
          res.statusCode = 404
          res.end()
        }
      })
    },
  }
}

export default defineConfig({
  plugins: [react(), tailwindcss(), ...(LOCAL_API ? [localUploads()] : [])],
  server: {
    port: 5180,
    // prepare the site's modules as soon as the dev server starts, so the first page load isn't slow
    warmup: { clientFiles: ['./src/main.tsx', './src/App.tsx', './src/site/**/*.tsx', './src/lib/*.ts', './src/lib/*.tsx', './src/components/*.tsx'] },
    proxy: {
      '/api': { target: API, changeOrigin: true, secure: true, cookieDomainRewrite: '' },
      ...(LOCAL_API ? {} : { '/uploads': { target: API, changeOrigin: true, secure: true } }),
    },
  },
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    rolldownOptions: {
      output: {
        codeSplitting: {
          groups: [
            { name: 'react', test: /node_modules[\\/](react|react-dom|scheduler|react-router|react-router-dom)[\\/]/ },
            { name: 'query', test: /node_modules[\\/]@tanstack[\\/]/ },
            { name: 'motion', test: /node_modules[\\/](motion|motion-dom|motion-utils|framer-motion)[\\/]/ },
            { name: 'editor', test: /node_modules[\\/](@tiptap|prosemirror-[^\\/]+|orderedmap|rope-sequence|w3c-keyname|linkifyjs)[\\/]/ },
            { name: 'forms', test: /node_modules[\\/](react-hook-form|zod|@hookform)[\\/]/ },
          ],
        },
      },
    },
  },
})
