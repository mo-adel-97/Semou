import { Loader2 } from 'lucide-react'
import { MotionConfig } from 'motion/react'
import { lazy, Suspense, useEffect } from 'react'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import { useSite } from './lib/queries'
import Layout from './site/Layout'
import About from './site/pages/About'
import { Blog, PostPage } from './site/pages/Blog'
import { Contact, Register } from './site/pages/Contact'
import Home from './site/pages/Home'
import NotFound from './site/pages/NotFound'
import Page from './site/pages/Page'
import ProgramDetail from './site/pages/ProgramDetail'
import Programs from './site/pages/Programs'
import Portal from './site/pages/Portal'

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
      { path: 'about', element: <About /> },
      { path: 'programs', element: <Programs /> },
      { path: 'programs/type/:type', element: <Programs /> },
      { path: 'programs/:slug', element: <ProgramDetail /> },
      { path: 'blog', element: <Blog /> },
      { path: 'blog/:slug', element: <PostPage /> },
      { path: 'p/:slug', element: <Page /> },
      { path: 'contact', element: <Contact /> },
      { path: 'register', element: <Register /> },
      { path: 'portal', element: <Portal /> },
      { path: '*', element: <NotFound /> },
    ],
  },
])

/** The browser-tab icon (and the iPhone home-screen icon) follow the logo chosen in the dashboard. */
function SiteIcon() {
  const logo = useSite().data?.settings.general.logo
  useEffect(() => {
    if (!logo) return
    for (const rel of ['icon', 'apple-touch-icon']) {
      let l = document.head.querySelector<HTMLLinkElement>(`link[rel="${rel}"]`)
      if (!l) { l = document.createElement('link'); l.rel = rel; document.head.appendChild(l) }
      l.removeAttribute('type')
      l.href = logo
    }
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
