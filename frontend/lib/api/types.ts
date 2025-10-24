/**
 * News API TypeScript Types
 * Matching the News API backend DTOs and entities
 */

export interface News {
  id: string;
  category: string;
  type: string;
  caption: string;
  slug: string;
  keywords: string;
  socialTags: string;
  summary: string;
  imgPath: string;
  imgAlt: string;
  imageUrl?: string;
  thumbnailUrl?: string;
  imageMetadata?: {
    width: number;
    height: number;
    altText: string;
  };
  content: string;
  subjects: string[];
  authors: string[];
  expressDate: string;
  createDate: string;
  updateDate: string;
  priority: number;
  isActive: boolean;
  url: string;
  viewCount: number;
  isSecondPageNews: boolean;
  
  // Legacy compatibility
  title?: string;
  description?: string;
  publishedAt?: Date;
  source?: string;
  author?: string;
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
