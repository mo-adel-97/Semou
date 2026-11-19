import { useQuery } from '@tanstack/react-query'
import { api, qs } from './api'
import type { HomeData, Post, Program, ProgramType, SiteData } from './types'

export const useSite = () => useQuery({ queryKey: ['site'], queryFn: () => api.get<SiteData>('/public/site'), staleTime: 5 * 60_000 })

export const useHome = () => useQuery({ queryKey: ['home'], queryFn: () => api.get<HomeData>('/public/home'), staleTime: 60_000 })

export const usePrograms = (type?: ProgramType) =>
  useQuery({ queryKey: ['programs', type ?? 'all'], queryFn: () => api.get<Program[]>(`/public/programs${qs({ type })}`) })

export const useProgram = (slug: string) =>
  useQuery({
    queryKey: ['program', slug],
    queryFn: () => api.get<{ program: Program; related: Program[] }>(`/public/programs/${encodeURIComponent(slug)}`),
  })

export const usePosts = (page: number) =>
  useQuery({
    queryKey: ['posts', page],
    queryFn: () => api.get<{ items: Post[]; page: number; pages: number; total: number }>(`/public/posts${qs({ page })}`),
    placeholderData: (prev) => prev,
  })

export const usePost = (slug: string) =>
  useQuery({
    queryKey: ['post', slug],
    queryFn: () => api.get<{ post: Post; more: Post[] }>(`/public/posts/${encodeURIComponent(slug)}`),
  })

export const usePage = (slug: string) =>
  useQuery({
    queryKey: ['page', slug],
    queryFn: () => api.get<{ title: string; slug: string; body: string; updated_at: string }>(`/public/pages/${encodeURIComponent(slug)}`),
  })
