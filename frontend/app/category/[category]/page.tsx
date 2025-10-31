import { Metadata } from "next";
import CategoryPageClient from "./page-client";

interface CategoryPageProps {
  params: Promise<{
    category: string;
  }>;
}

// Category metadata mapping - Reddit subreddit based
const categoryMetadata: Record<string, { title: string; description: string; keywords: string[] }> =
  {
    popular: {
      title: "Popüler Haberler",
      description: "Reddit'ten en popüler teknoloji ve yapay zeka haberleri. Güncel trendler ve viral içerikler.",
      keywords: ["popüler haberler", "reddit", "trending", "viral", "teknoloji"],
    },
    artificialintelligence: {
      title: "Yapay Zeka Haberleri",
      description: "r/ArtificialIntelligence'dan yapay zeka, machine learning ve deep learning haberleri.",
      keywords: ["yapay zeka", "AI", "machine learning", "deep learning", "neural networks"],
    },
    githubcopilot: {
      title: "GitHub Copilot Haberleri",
      description: "r/GithubCopilot'tan GitHub Copilot, AI kod asistanı haberleri ve güncellemeler.",
      keywords: ["github copilot", "AI code assistant", "copilot", "github", "coding AI"],
    },
    mcp: {
      title: "MCP Haberleri",
      description: "r/mcp'den Model Context Protocol haberleri ve gelişmeleri.",
      keywords: ["MCP", "model context protocol", "AI protocols", "context management"],
    },
    openai: {
      title: "OpenAI Haberleri",
      description: "r/OpenAI'dan ChatGPT, GPT-4, DALL-E ve diğer OpenAI ürünleri hakkında haberler.",
      keywords: ["openai", "chatgpt", "gpt-4", "dall-e", "openai api"],
    },
    robotics: {
      title: "Robotik Haberleri",
      description: "r/robotics'ten robotik, otomasyon ve robot teknolojileri haberleri.",
      keywords: ["robotics", "robots", "automation", "robotik sistemler"],
    },
    deepseek: {
      title: "DeepSeek AI Haberleri",
      description: "r/DeepSeek'ten DeepSeek AI haberleri ve güncellemeleri.",
      keywords: ["deepseek", "deepseek ai", "AI research", "deep learning"],
    },
    dotnet: {
      title: ".NET Haberleri",
      description: "r/dotnet'ten .NET, C#, ASP.NET Core ve Microsoft teknolojileri haberleri.",
      keywords: [".net", "dotnet", "c#", "asp.net core", "microsoft"],
    },
    claudeai: {
      title: "Claude AI Haberleri",
      description: "r/ClaudeAI'dan Anthropic Claude AI haberleri ve güncellemeleri.",
      keywords: ["claude ai", "anthropic", "claude", "AI assistant"],
    },
  };

export async function generateMetadata({ params }: CategoryPageProps): Promise<Metadata> {
  const { category } = await params;
  const categoryData = categoryMetadata[category] || categoryMetadata.technology;
  
  const title = categoryData?.title || "Teknoloji Haberleri";
  const description = categoryData?.description || "Son dakika teknoloji haberleri ve güncel gelişmeler.";
  const keywords = categoryData?.keywords || ["teknoloji haberleri"];

  return {
    title: `${title} - Teknoloji Haberleri`,
    description,
    keywords,
    alternates: {
      canonical: `/category/${category}`,
    },
    openGraph: {
      title: `${title} - Teknoloji Haberleri`,
      description,
      type: "website",
      url: `/category/${category}`,
      siteName: "Teknoloji Haberleri",
    },
    twitter: {
      card: "summary_large_image",
      title: `${title} - Teknoloji Haberleri`,
      description,
    },
    robots: {
      index: true,
      follow: true,
      googleBot: {
        index: true,
        follow: true,
        "max-snippet": -1,
        "max-image-preview": "large",
      },
    },
  };
}

export default async function CategoryPage({ params }: CategoryPageProps) {
  const { category } = await params;
  return <CategoryPageClient category={category} />;
}
