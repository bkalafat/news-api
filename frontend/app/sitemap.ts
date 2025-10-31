import { MetadataRoute } from "next";
import { NewsCategory } from "@/lib/api/types";

interface News {
  id: string;
  slug: string;
  updateDate: string;
  expressDate: string;
  priority: number;
  category: string;
}

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const baseUrl = process.env.NEXT_PUBLIC_SITE_URL || "http://localhost:3000";

  // Static pages
  const staticPages: MetadataRoute.Sitemap = [
    {
      url: baseUrl,
      lastModified: new Date(),
      changeFrequency: "hourly",
      priority: 1.0,
    },
    {
      url: `${baseUrl}/about`,
      lastModified: new Date(),
      changeFrequency: "monthly",
      priority: 0.5,
    },
    {
      url: `${baseUrl}/categories`,
      lastModified: new Date(),
      changeFrequency: "weekly",
      priority: 0.7,
    },
    {
      url: `${baseUrl}/search`,
      lastModified: new Date(),
      changeFrequency: "daily",
      priority: 0.6,
    },
  ];

  // Category pages
  const categoryPages: MetadataRoute.Sitemap = Object.values(NewsCategory).map((category) => ({
    url: `${baseUrl}/category/${category}`,
    lastModified: new Date(),
    changeFrequency: "hourly" as const,
    priority: 0.8,
  }));

  // Fetch news articles for dynamic sitemap
  let newsPages: MetadataRoute.Sitemap = [];
  try {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
    const response = await fetch(`${apiUrl}/api/NewsArticle?pageSize=1000`, {
      next: { revalidate: 3600 }, // Cache for 1 hour
    });

    if (response.ok) {
      const news: News[] = await response.json();
      newsPages = news.map((item) => {
        // Calculate priority based on article priority and recency
        const daysSincePublish = Math.floor(
          (Date.now() - new Date(item.expressDate).getTime()) / (1000 * 60 * 60 * 24)
        );
        let priority = 0.7;
        if (item.priority >= 80) priority = 0.9;
        else if (item.priority >= 60) priority = 0.8;
        else if (daysSincePublish < 7) priority = 0.8; // Recent articles get higher priority

        return {
          url: `${baseUrl}/news/${item.slug}`,
          lastModified: new Date(item.updateDate || item.expressDate),
          changeFrequency: daysSincePublish < 1 ? ("hourly" as const) : ("daily" as const),
          priority: Math.min(priority, 0.9),
        };
      });
    }
  } catch (error) {
    console.error("Error fetching news for sitemap:", error);
  }

  return [...staticPages, ...categoryPages, ...newsPages];
}
