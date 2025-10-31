"use client";

import { useQuery, useMutation, useQueryClient, UseQueryOptions } from "@tanstack/react-query";
import { newsApi } from "./client";
import { News, CreateNewsDto, UpdateNewsDto, ApiError } from "./types";

/**
 * Query keys for React Query caching
 */
export const newsQueryKeys = {
  all: ["news"] as const,
  lists: () => [...newsQueryKeys.all, "list"] as const,
  list: (filters?: any) => [...newsQueryKeys.lists(), filters] as const,
  details: () => [...newsQueryKeys.all, "detail"] as const,
  detail: (id: string) => [...newsQueryKeys.details(), id] as const,
  categories: () => [...newsQueryKeys.all, "category"] as const,
  category: (category: string) => [...newsQueryKeys.categories(), category] as const,
};

/**
 * Hook to fetch all news
 */
export function useAllNews(
  options?: Omit<UseQueryOptions<News[], ApiError>, "queryKey" | "queryFn">
) {
  return useQuery<News[], ApiError>({
    queryKey: newsQueryKeys.lists(),
    queryFn: () => newsApi.getAllNews(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
}

/**
 * Hook to fetch news by ID
 */
export function useNewsById(
  id: string,
  options?: Omit<UseQueryOptions<News, ApiError>, "queryKey" | "queryFn">
) {
  return useQuery<News, ApiError>({
    queryKey: newsQueryKeys.detail(id),
    queryFn: () => newsApi.getNewsById(id),
    staleTime: 5 * 60 * 1000,
    enabled: !!id,
    ...options,
  });
}

/**
 * Hook to fetch news by category
 */
export function useNewsByCategory(
  category: string,
  options?: Omit<UseQueryOptions<News[], ApiError>, "queryKey" | "queryFn">
) {
  return useQuery<News[], ApiError>({
    queryKey: newsQueryKeys.category(category),
    queryFn: () => newsApi.getNewsByCategory(category),
    staleTime: 5 * 60 * 1000,
    enabled: !!category,
    ...options,
  });
}

/**
 * Hook to create news
 */
export function useCreateNews() {
  const queryClient = useQueryClient();

  return useMutation<News, ApiError, CreateNewsDto>({
    mutationFn: (data: CreateNewsDto) => newsApi.createNews(data),
    onSuccess: () => {
      // Invalidate and refetch all news queries
      queryClient.invalidateQueries({ queryKey: newsQueryKeys.lists() });
    },
  });
}

/**
 * Hook to update news
 */
export function useUpdateNews() {
  const queryClient = useQueryClient();

  return useMutation<News, ApiError, { id: string; data: UpdateNewsDto }>({
    mutationFn: ({ id, data }) => newsApi.updateNews(id, data),
    onSuccess: (updatedNews) => {
      // Update the specific news item in cache
      queryClient.setQueryData(newsQueryKeys.detail(updatedNews.id), updatedNews);
      // Invalidate lists
      queryClient.invalidateQueries({ queryKey: newsQueryKeys.lists() });
    },
  });
}

/**
 * Hook to delete news
 */
export function useDeleteNews() {
  const queryClient = useQueryClient();

  return useMutation<void, ApiError, string>({
    mutationFn: (id: string) => newsApi.deleteNews(id),
    onSuccess: (_, deletedId) => {
      // Remove from cache
      queryClient.removeQueries({ queryKey: newsQueryKeys.detail(deletedId) });
      // Invalidate lists
      queryClient.invalidateQueries({ queryKey: newsQueryKeys.lists() });
    },
  });
}

/**
 * Hook to check API health
 */
export function useApiHealth() {
  return useQuery<{ status: string }, ApiError>({
    queryKey: ["health"],
    queryFn: () => newsApi.getHealth(),
    staleTime: 30 * 1000, // 30 seconds
    retry: 3,
  });
}
