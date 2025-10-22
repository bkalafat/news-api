'use client';

import { useTranslations } from "next-intl";
import { Newspaper, Github, Mail } from "lucide-react";
import Link from "next/link";

export function Footer() {
  const t = useTranslations();
  const currentYear = new Date().getFullYear();

  return (
    <footer className="w-full border-t bg-muted/50">
      <div className="container mx-auto px-4 py-12">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Brand */}
          <div className="space-y-4">
            <div className="flex items-center space-x-2">
              <Newspaper className="h-6 w-6" />
              <span className="font-bold text-lg">{t('app.title')}</span>
            </div>
            <p className="text-sm text-muted-foreground">
              {t('app.tagline')}
            </p>
          </div>

          {/* Links */}
          <div className="space-y-4">
            <h3 className="font-semibold">{t('navigation.categories')}</h3>
            <ul className="space-y-2 text-sm text-muted-foreground">
              <li>
                <Link href="/category/technology" className="hover:text-primary transition-colors">
                  {t('categories.technology')}
                </Link>
              </li>
              <li>
                <Link href="/category/world" className="hover:text-primary transition-colors">
                  {t('categories.world')}
                </Link>
              </li>
              <li>
                <Link href="/category/business" className="hover:text-primary transition-colors">
                  {t('categories.business')}
                </Link>
              </li>
              <li>
                <Link href="/category/science" className="hover:text-primary transition-colors">
                  {t('categories.science')}
                </Link>
              </li>
            </ul>
          </div>

          {/* Contact */}
          <div className="space-y-4">
            <h3 className="font-semibold">İletişim</h3>
            <div className="flex space-x-4">
              <a
                href="https://github.com/bkalafat/news-api"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="GitHub"
              >
                <Github className="h-5 w-5" />
              </a>
              <a
                href="mailto:info@example.com"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="Email"
              >
                <Mail className="h-5 w-5" />
              </a>
            </div>
          </div>
        </div>

        <div className="mt-8 pt-8 border-t">
          <p className="text-center text-sm text-muted-foreground">
            © {currentYear} Teknoloji Haberleri. Tüm hakları saklıdır.
          </p>
        </div>
      </div>
    </footer>
  );
}
