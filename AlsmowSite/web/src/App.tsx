import { Loader2 } from 'lucide-react'
import { MotionConfig } from 'motion/react'
import { lazy, Suspense, useEffect, type ReactNode } from 'react'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import { useSite } from './lib/queries'
import Layout from './site/Layout'
import Home from './site/pages/Home'

// Inner pages are separate downloads (the home page only ships what it shows), fetched quietly once the page is idle
// so a click still opens them instantly.
const pages = {
  about: () => import('./site/pages/About'),
  blog: () => import('./site/pages/Blog'),
  contact: () => import('./site/pages/Contact'),
  notFound: () => import('./site/pages/NotFound'),
  page: () => import('./site/pages/Page'),
  program: () => import('./site/pages/ProgramDetail'),
  programs: () => import('./site/pages/Programs'),
  portal: () => import('./site/pages/Portal'),
}
const About = lazy(pages.about)
const Blog = lazy(() => pages.blog().then((m) => ({ default: m.Blog })))
const PostPage = lazy(() => pages.blog().then((m) => ({ default: m.PostPage })))
const Contact = lazy(() => pages.contact().then((m) => ({ default: m.Contact })))
const Register = lazy(() => pages.contact().then((m) => ({ default: m.Register })))
const NotFound = lazy(pages.notFound)
const Page = lazy(pages.page)
const ProgramDetail = lazy(pages.program)
const Programs = lazy(pages.programs)
const Portal = lazy(pages.portal)
if (typeof window !== 'undefined' && !/^\/admin/.test(location.pathname)) {
  const warm = () => Object.values(pages).forEach((load) => load().catch(() => {}))
  window.addEventListener('load', () => ('requestIdleCallback' in window ? requestIdleCallback(warm, { timeout: 6000 }) : setTimeout(warm, 3000)), { once: true })
}
const page = (el: ReactNode) => <Suspense fallback={<div className="min-h-dvh" />}>{el}</Suspense>

// The whole dashboard (editor, forms...) is a separate download, never loaded by site visitors.
const Admin = lazy(() => import('./admin/AdminApp'))

function Fallback() {
  return (
    <div className="grid min-h-dvh place-items-center">
      <Loader2 className="h-8 w-8 animate-spin text-brand-600" />
    </div>
  )
}

const router = createBrowserRouter([
  {
    path: '/admin/*',
    element: (
      <Suspense fallback={<Fallback />}>
        <Admin />
      </Suspense>
    ),
  },
  {
    element: <Layout />,
    children: [
      { index: true, element: <Home /> },
      { path: 'about', element: page(<About />) },
      { path: 'programs', element: page(<Programs />) },
      { path: 'programs/type/:type', element: page(<Programs />) },
      { path: 'programs/:slug', element: page(<ProgramDetail />) },
      { path: 'blog', element: page(<Blog />) },
      { path: 'blog/:slug', element: page(<PostPage />) },
      { path: 'p/:slug', element: page(<Page />) },
      { path: 'contact', element: page(<Contact />) },
      { path: 'register', element: page(<Register />) },
      { path: 'portal', element: page(<Portal />) },
      { path: '*', element: page(<NotFound />) },
    ],
  },
])

/** The browser-tab icon (and the iPhone home-screen icon) follow the logo chosen in the dashboard. */
function SiteIcon() {
  const logo = useSite().data?.settings.general.logo
  useEffect(() => {
    if (!logo) return
    // after the page has settled, so the icon never competes with what the visitor sees first
    const set = () => {
      for (const rel of ['icon', 'apple-touch-icon']) {
        let l = document.head.querySelector<HTMLLinkElement>(`link[rel="${rel}"]`)
        if (!l) { l = document.createElement('link'); l.rel = rel; document.head.appendChild(l) }
        l.removeAttribute('type')
        l.href = logo
      }
    }
    const t = window.setTimeout(set, 4000)
    return () => clearTimeout(t)
  }, [logo])
  return null
}

export default function App() {
  return (
    <MotionConfig reducedMotion="user">
      <SiteIcon />
      <RouterProvider router={router} />
    </MotionConfig>
  )
}
