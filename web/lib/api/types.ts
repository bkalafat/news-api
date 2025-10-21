/**
 * News API TypeScript Types
 * Matching the News API backend DTOs and entities
 */

export interface News {
  id: string;
  title: string;
  description: string;
  url: string;
  imageUrl?: string;
  publishedAt: Date;
  source: string;
  author?: string;
  category?: string;
  content?: string;
}

export interface CreateNewsDto {
  title: string;
  description: string;
  url: string;
  imageUrl?: string;
  publishedAt?: Date;
  source: string;
  author?: string;
  category?: string;
  content?: string;
}

export interface UpdateNewsDto {
  title?: string;
  description?: string;
  url?: string;
  imageUrl?: string;
  publishedAt?: Date;
  source?: string;
  author?: string;
  category?: string;
  content?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

export interface ApiError {
  message: string;
  statusCode: number;
  errors?: Record<string, string[]>;
}

export enum NewsCategory {
  Technology = 'technology',
  World = 'world',
  Business = 'business',
  Science = 'science',
  Health = 'health',
  Entertainment = 'entertainment',
}

export interface NewsFilters {
  category?: NewsCategory;
  source?: string;
  startDate?: Date;
  endDate?: Date;
  searchQuery?: string;
  page?: number;
  pageSize?: number;
}
