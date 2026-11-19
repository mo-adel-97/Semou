import { StrictMode, Suspense, lazy } from 'react'
import { ErrorBoundary, reloadForNewVersion } from './components/ErrorBoundary'
import { VersionWatcher } from './components/VersionWatcher'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { QueryClientProvider } from '@tanstack/react-query'
import { MotionConfig } from 'motion/react'
import './index.css'
import App from './App'
import { AuthProvider } from './auth'
import { UiConfigProvider } from './uiConfig'
import { queryClient } from './lib/queryClient'
import { ThemeProvider, applyInitialTheme } from './theme'

// Optional build tag (set by the release script) so every published build is distinguishable.
if (import.meta.env.VITE_BUILD_TAG) (window as unknown as { __build: string }).__build = import.meta.env.VITE_BUILD_TAG

applyInitialTheme() // before first paint: no flash of the wrong theme

// A new version was published while this tab was open: the old screen files no longer exist.
// Load the new version once instead of showing an error (guarded so it can never loop).
window.addEventListener('vite:preloadError', e => { if (reloadForNewVersion()) e.preventDefault() })

// Developer tool (palm-tree button). Off by default so it never covers the UI;
// enable with VITE_QUERY_DEVTOOLS=true in web/.env.local. Never shipped in production.
const Devtools = import.meta.env.DEV && import.meta.env.VITE_QUERY_DEVTOOLS === 'true'
  ? lazy(() => import('@tanstack/react-query-devtools').then(m => ({ default: m.ReactQueryDevtools })))
  : () => null

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MotionConfig reducedMotion="user">
      <ThemeProvider>
        <QueryClientProvider client={queryClient}>
          <BrowserRouter>
            <AuthProvider>
              <UiConfigProvider>
                <ErrorBoundary><App /></ErrorBoundary>
              </UiConfigProvider>
              <VersionWatcher />
            </AuthProvider>
          </BrowserRouter>
          <Suspense><Devtools buttonPosition="bottom-left" /></Suspense>
        </QueryClientProvider>
      </ThemeProvider>
    </MotionConfig>
  </StrictMode>,
)
