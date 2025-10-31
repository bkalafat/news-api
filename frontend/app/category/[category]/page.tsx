import { Metadata } from "next";
import CategoryPageClient from "./page-client";

interface CategoryPageProps {
  params: Promise<{
    category: string;
  }>;
}

// Category metadata mapping
const categoryMetadata: Record<string, { title: string; description: string; keywords: string[] }> =
  {
    technology: {
      title: "Teknoloji Haberleri",
      description:
        "Son dakika teknoloji haberleri, yazılım, donanım, yapay zeka, mobil teknolojiler ve yenilikler. Teknoloji dünyasından güncel gelişmeler.",
      keywords: [
        "teknoloji haberleri",
        "yazılım",
        "donanım",
        "yapay zeka",
        "mobil teknoloji",
        "yenilikler",
      ],
    },
    science: {
      title: "Bilim Haberleri",
      description:
        "Bilim dünyasından son dakika haberleri, araştırmalar, keşifler ve bilimsel gelişmeler.",
      keywords: ["bilim haberleri", "araştırmalar", "keşifler", "bilimsel gelişmeler"],
    },
    business: {
      title: "İş Dünyası Haberleri",
      description:
        "İş dünyası, ekonomi, finans ve girişimcilik haberleri. Şirket haberleri ve piyasa analizleri.",
      keywords: [
        "iş dünyası",
        "ekonomi haberleri",
        "finans",
        "girişimcilik",
        "şirket haberleri",
      ],
    },
    health: {
      title: "Sağlık Haberleri",
      description: "Sağlık, tıp ve wellness alanından güncel haberler ve gelişmeler.",
      keywords: ["sağlık haberleri", "tıp", "wellness", "sağlıklı yaşam"],
    },
    entertainment: {
      title: "Eğlence Haberleri",
      description: "Eğlence dünyasından haberler, film, müzik, oyun ve kültür sanat haberleri.",
      keywords: ["eğlence haberleri", "film", "müzik", "oyun", "kültür sanat"],
    },
    sports: {
      title: "Spor Haberleri",
      description: "Spor dünyasından son dakika haberleri, maç sonuçları ve spor analizleri.",
      keywords: ["spor haberleri", "maç sonuçları", "futbol", "basketbol", "spor analizleri"],
    },
    reddit: {
      title: "Reddit Haberleri",
      description:
        "Reddit'ten teknoloji ve toplum haberleri, trendler ve popüler tartışmalar.",
      keywords: ["reddit", "reddit haberleri", "teknoloji tartışmaları", "toplum"],
    },
    github: {
      title: "GitHub Haberleri",
      description:
        "GitHub'tan geliştirici haberleri, açık kaynak projeleri ve yazılım geliştirme trendleri.",
      keywords: [
        "github",
        "github haberleri",
        "açık kaynak",
        "yazılım geliştirme",
        "geliştirici",
      ],
    },
    twitter: {
      title: "X (Twitter) Haberleri",
      description: "X/Twitter'dan sosyal medya trendleri, viral içerikler ve gündem haberleri.",
      keywords: ["twitter", "x haberleri", "sosyal medya", "trendler", "viral içerik"],
    },
    linkedin: {
      title: "LinkedIn Haberleri",
      description: "LinkedIn'den iş dünyası, kariyer ve profesyonel gelişim haberleri.",
      keywords: ["linkedin", "kariyer haberleri", "iş dünyası", "profesyonel gelişim"],
    },
    facebook: {
      title: "Facebook Haberleri",
      description: "Facebook'tan sosyal medya haberleri, özellikler ve gelişmeler.",
      keywords: ["facebook", "sosyal medya haberleri", "meta", "sosyal ağ"],
    },
    instagram: {
      title: "Instagram Haberleri",
      description: "Instagram'dan görsel içerik trendleri, özellikler ve sosyal medya haberleri.",
      keywords: ["instagram", "görsel içerik", "sosyal medya", "instagram trendleri"],
    },
    tiktok: {
      title: "TikTok Haberleri",
      description: "TikTok'tan video içerik trendleri, viral videolar ve platform haberleri.",
      keywords: ["tiktok", "video içerik", "viral videolar", "tiktok trendleri"],
    },
    youtube: {
      title: "YouTube Haberleri",
      description:
        "YouTube'dan video platformu haberleri, içerik üreticileri ve platform güncellemeleri.",
      keywords: ["youtube", "video platformu", "youtuber", "içerik üreticileri"],
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
