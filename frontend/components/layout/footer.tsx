"use client";

import { useTranslations } from "next-intl";
import { Newspaper, Github, Mail } from "lucide-react";
import Link from "next/link";

export function Footer() {
  const t = useTranslations();
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-muted/50 w-full border-t">
      <div className="container mx-auto px-4 py-12">
        <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
          {/* Brand */}
          <div className="space-y-4">
            <div className="flex items-center space-x-2">
              <Newspaper className="h-6 w-6" />
              <span className="text-lg font-bold">{t("app.title")}</span>
            </div>
            <p className="text-muted-foreground text-sm">{t("app.tagline")}</p>
          </div>

          {/* Links */}
          <div className="space-y-4">
            <h3 className="font-semibold">{t("navigation.categories")}</h3>
            <ul className="text-muted-foreground space-y-2 text-sm">
              <li>
                <Link href="/category/popular" className="hover:text-primary transition-colors">
                  {t("categories.popular")}
                </Link>
              </li>
              <li>
                <Link href="/category/artificialintelligence" className="hover:text-primary transition-colors">
                  {t("categories.artificialintelligence")}
                </Link>
              </li>
              <li>
                <Link href="/category/openai" className="hover:text-primary transition-colors">
                  {t("categories.openai")}
                </Link>
              </li>
              <li>
                <Link href="/category/claudeai" className="hover:text-primary transition-colors">
                  {t("categories.claudeai")}
                </Link>
              </li>
            </ul>
          </div>

          {/* Contact */}
          <div className="space-y-4">
            <h3 className="font-semibold">İletişim</h3>
            <div className="flex space-x-4">
              <a
                href="https://github.com/bkalafat/newsportal"
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

        <div className="mt-8 border-t pt-8">
          <p className="text-muted-foreground text-center text-sm">
            © {currentYear} Teknoloji Haberleri. Tüm hakları saklıdır.
          </p>
        </div>
      </div>
    </footer>
  );
}
