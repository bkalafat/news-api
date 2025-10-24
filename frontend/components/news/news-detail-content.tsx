import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Share2, BookmarkPlus } from "lucide-react";
import { Button } from "@/components/ui/button";

interface News {
  id: string;
  content: string;
  authors: string[];
  subjects: string[];
  keywords: string;
  socialTags: string;
}

interface NewsDetailContentProps {
  news: News;
}

export function NewsDetailContent({ news }: NewsDetailContentProps) {
  // Check if content is HTML
  const isHtmlContent = news.content.trim().startsWith('<');

  return (
    <div className="space-y-8">
      {/* Authors */}
      {news.authors && news.authors.length > 0 && (
        <div className="flex flex-wrap items-center gap-4">
          {news.authors.map((author, index) => (
            <div key={index} className="flex items-center gap-2">
              <Avatar>
                <AvatarFallback className="bg-primary text-primary-foreground">
                  {author.charAt(0).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div>
                <p className="text-sm font-semibold">{author}</p>
                <p className="text-xs text-muted-foreground">Yazar</p>
              </div>
            </div>
          ))}
        </div>
      )}

      <Separator />

      {/* Article Content */}
      <Card>
        <CardContent className="pt-6">
          {isHtmlContent ? (
            // Render HTML content
            <article 
              className="prose prose-lg dark:prose-invert max-w-none prose-headings:font-bold prose-h2:text-2xl prose-h2:mt-8 prose-h2:mb-4 prose-h3:text-xl prose-h3:mt-6 prose-h3:mb-3 prose-p:mb-4 prose-p:leading-relaxed prose-ul:my-4 prose-ol:my-4 prose-li:my-2 prose-img:rounded-lg prose-img:my-6 prose-blockquote:border-l-4 prose-blockquote:border-primary prose-blockquote:pl-4 prose-blockquote:italic prose-blockquote:my-6 prose-table:w-full prose-table:my-6 prose-th:bg-muted prose-th:p-3 prose-td:p-3 prose-td:border"
              dangerouslySetInnerHTML={{ __html: news.content }}
            />
          ) : (
            // Render plain text content
            <article className="prose prose-lg dark:prose-invert max-w-none">
              {news.content.split("\n").filter((p) => p.trim().length > 0).map((paragraph, index) => (
                <p key={index} className="mb-4 leading-relaxed text-foreground">
                  {paragraph}
                </p>
              ))}
            </article>
          )}
        </CardContent>
      </Card>

      {/* Tags/Subjects */}
      {news.subjects && news.subjects.length > 0 && (
        <div>
          <h3 className="text-lg font-semibold mb-3">Konular</h3>
          <div className="flex flex-wrap gap-2">
            {news.subjects.map((subject, index) => (
              <Badge key={index} variant="secondary" className="px-3 py-1">
                {subject}
              </Badge>
            ))}
          </div>
        </div>
      )}

      {/* Keywords */}
      {news.keywords && (
        <div>
          <h3 className="text-lg font-semibold mb-3">Anahtar Kelimeler</h3>
          <div className="flex flex-wrap gap-2">
            {news.keywords.split(",").map((keyword, index) => (
              <Badge key={index} variant="outline" className="px-3 py-1">
                {keyword.trim()}
              </Badge>
            ))}
          </div>
        </div>
      )}

      <Separator />

      {/* Action Buttons */}
      <div className="flex flex-wrap gap-4">
        <Button variant="outline" className="flex items-center gap-2">
          <Share2 className="w-4 h-4" />
          Paylaş
        </Button>
        <Button variant="outline" className="flex items-center gap-2">
          <BookmarkPlus className="w-4 h-4" />
          Kaydet
        </Button>
      </div>

      {/* Social Tags */}
      {news.socialTags && (
        <Card className="bg-muted/50">
          <CardContent className="pt-6">
            <h4 className="text-sm font-semibold mb-2 text-muted-foreground">Sosyal Medyada Paylaş</h4>
            <p className="text-sm text-muted-foreground">{news.socialTags}</p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
