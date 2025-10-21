import { notFound } from 'next/navigation';
import Image from 'next/image';
import Link from 'next/link';
import { ArrowLeft, Calendar, User, Eye, Share2 } from 'lucide-react';
import { ShareButtons } from '@/components/social/share-buttons';

async function getNewsArticle(slug: string) {
  try {
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/news`, {
      cache: 'no-store'
    });
    
    if (!res.ok) return null;
    
    const articles = await res.json();
    
    // Find article by matching URL slug
    const article = articles.find((item: any) => {
      const urlSlug = item.url?.split('/').pop() || '';
      return urlSlug === slug;
    });
    
    return article || null;
  } catch (error) {
    console.error('Error fetching news article:', error);
    return null;
  }
}

export async function generateMetadata({ params }: { params: { slug: string } }) {
  const article = await getNewsArticle(params.slug);
  
  if (!article) {
    return {
      title: 'Haber Bulunamadı'
    };
  }
  
  return {
    title: `${article.caption} | Haberler`,
    description: article.summary,
    openGraph: {
      title: article.caption,
      description: article.summary,
      images: article.imgPath ? [article.imgPath] : [],
    },
    twitter: {
      card: 'summary_large_image',
      title: article.caption,
      description: article.summary,
      images: article.imgPath ? [article.imgPath] : [],
    },
  };
}

export default async function NewsDetailPage({ params }: { params: { slug: string } }) {
  const article = await getNewsArticle(params.slug);
  
  if (!article) {
    notFound();
  }

  const formattedDate = new Date(article.expressDate).toLocaleDateString('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  });

  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto px-4 py-8 max-w-4xl">
        {/* Back Button */}
        <Link 
          href="/" 
          className="inline-flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors mb-6"
        >
          <ArrowLeft className="h-4 w-4" />
          <span>Ana Sayfaya Dön</span>
        </Link>

        {/* Category Badge */}
        <div className="mb-4">
          <Link
            href={`/category/${article.category}`}
            className="inline-block px-3 py-1 bg-primary/10 text-primary rounded-full text-sm font-medium hover:bg-primary/20 transition-colors"
          >
            {article.category === 'technology' && 'Teknoloji'}
            {article.category === 'world' && 'Dünya'}
            {article.category === 'business' && 'Ekonomi'}
            {article.category === 'science' && 'Bilim'}
            {article.category === 'health' && 'Sağlık'}
            {article.category === 'entertainment' && 'Eğlence'}
            {article.category === 'sports' && 'Spor'}
          </Link>
        </div>

        {/* Title */}
        <h1 className="text-4xl md:text-5xl font-bold mb-4 leading-tight">
          {article.caption}
        </h1>

        {/* Summary */}
        <p className="text-xl text-muted-foreground mb-6 leading-relaxed">
          {article.summary}
        </p>

        {/* Meta Information */}
        <div className="flex flex-wrap items-center gap-4 mb-6 pb-6 border-b">
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Calendar className="h-4 w-4" />
            <span>{formattedDate}</span>
          </div>
          
          {article.authors && article.authors.length > 0 && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <User className="h-4 w-4" />
              <span>{article.authors.join(', ')}</span>
            </div>
          )}
          
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Eye className="h-4 w-4" />
            <span>{article.viewCount.toLocaleString('tr-TR')} görüntülenme</span>
          </div>
        </div>

        {/* Featured Image */}
        {article.imgPath && (
          <div className="relative w-full h-[400px] md:h-[500px] mb-8 rounded-lg overflow-hidden">
            <Image
              src={article.imgPath}
              alt={article.imgAlt || article.caption}
              fill
              className="object-cover"
              priority
            />
          </div>
        )}

        {/* Article Content */}
        <div className="prose prose-lg dark:prose-invert max-w-none mb-8">
          <div className="text-lg leading-relaxed whitespace-pre-line">
            {article.content}
          </div>
        </div>

        {/* Keywords/Tags */}
        {article.keywords && (
          <div className="mb-8">
            <h3 className="text-sm font-semibold text-muted-foreground mb-3">Etiketler</h3>
            <div className="flex flex-wrap gap-2">
              {article.keywords.split(',').map((keyword: string, index: number) => (
                <span
                  key={index}
                  className="px-3 py-1 bg-muted rounded-full text-sm"
                >
                  {keyword.trim()}
                </span>
              ))}
            </div>
          </div>
        )}

        {/* Social Share */}
        <div className="border-t pt-6">
          <div className="flex items-center gap-3 mb-3">
            <Share2 className="h-5 w-5 text-muted-foreground" />
            <h3 className="text-lg font-semibold">Haberi Paylaş</h3>
          </div>
          <ShareButtons 
            url={`${process.env.NEXT_PUBLIC_SITE_URL || 'http://localhost:3000'}/news/${params.slug}`}
            title={article.caption}
          />
        </div>

        {/* Subjects */}
        {article.subjects && article.subjects.length > 0 && (
          <div className="mt-8 p-4 bg-muted/50 rounded-lg">
            <h3 className="text-sm font-semibold mb-2">Konular</h3>
            <div className="flex flex-wrap gap-2">
              {article.subjects.map((subject: string, index: number) => (
                <span
                  key={index}
                  className="text-sm text-muted-foreground"
                >
                  {subject}
                </span>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
