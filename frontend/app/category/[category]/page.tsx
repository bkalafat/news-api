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
    world: {
      title: "Dünya Haberleri",
      description:
        "Dünya gündeminden son dakika haberleri, uluslararası gelişmeler ve küresel olaylar.",
      keywords: ["dünya haberleri", "uluslararası haberler", "küresel gelişmeler", "dünya gündemi"],
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
