"use client";

import Link from "next/link";
import { useTranslations } from "next-intl";
import { Newspaper, Menu } from "lucide-react";
import { Button } from "@/components/ui/button";
import { SearchBar } from "@/components/search/search-bar";
import { ThemeToggle } from "@/components/theme-toggle";
import { useState } from "react";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";

export function Header() {
  const t = useTranslations();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <header className="glass sticky top-0 z-50 w-full border-b border-border/50 shadow-sm">
      <div className="container mx-auto px-4">
        <div className="flex h-16 items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2 group">
            <Newspaper className="h-6 w-6 text-primary transition-transform group-hover:scale-110" />
            <span className="hidden text-xl font-bold sm:inline-block bg-gradient-to-r from-primary to-primary/70 bg-clip-text text-transparent">{t("app.title")}</span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden items-center space-x-6 md:flex">
            <Link href="/" className="hover:text-primary text-sm font-medium transition-colors">
              {t("navigation.home")}
            </Link>
            <Link
              href="/categories"
              className="hover:text-primary text-sm font-medium transition-colors"
            >
              {t("navigation.categories")}
            </Link>
            <Link
              href="/about"
              className="hover:text-primary text-sm font-medium transition-colors"
            >
              {t("navigation.about")}
            </Link>
          </nav>

          {/* Search Bar */}
          <SearchBar className="hidden max-w-md flex-1 md:flex" />

          {/* Theme Toggle */}
          <ThemeToggle />

          {/* Mobile menu */}
          <Sheet open={mobileMenuOpen} onOpenChange={setMobileMenuOpen}>
            <SheetTrigger asChild className="md:hidden">
              <Button variant="ghost" size="icon" aria-label="Menu">
                <Menu className="h-5 w-5" />
              </Button>
            </SheetTrigger>
            <SheetContent side="right" className="w-[300px] sm:w-[400px]">
              <nav className="mt-8 flex flex-col space-y-4">
                <Link
                  href="/"
                  onClick={() => setMobileMenuOpen(false)}
                  className="hover:text-primary text-lg font-medium transition-colors"
                >
                  {t("navigation.home")}
                </Link>
                <Link
                  href="/categories"
                  onClick={() => setMobileMenuOpen(false)}
                  className="hover:text-primary text-lg font-medium transition-colors"
                >
                  {t("navigation.categories")}
                </Link>
                <Link
                  href="/about"
                  onClick={() => setMobileMenuOpen(false)}
                  className="hover:text-primary text-lg font-medium transition-colors"
                >
                  {t("navigation.about")}
                </Link>
                <div className="border-t pt-4">
                  <h3 className="mb-3 font-semibold">{t("navigation.categories")}</h3>
                  <div className="flex flex-col space-y-2">
                    <Link
                      href="/category/popular"
                      onClick={() => setMobileMenuOpen(false)}
                      className="hover:text-primary text-sm transition-colors"
                    >
                      {t("categories.popular")}
                    </Link>
                    <Link
                      href="/category/artificialintelligence"
                      onClick={() => setMobileMenuOpen(false)}
                      className="hover:text-primary text-sm transition-colors"
                    >
                      {t("categories.artificialintelligence")}
                    </Link>
                    <Link
                      href="/category/githubcopilot"
                      onClick={() => setMobileMenuOpen(false)}
                      className="hover:text-primary text-sm transition-colors"
                    >
                      {t("categories.githubcopilot")}
                    </Link>
                    <Link
                      href="/category/openai"
                      onClick={() => setMobileMenuOpen(false)}
                      className="hover:text-primary text-sm transition-colors"
                    >
                      {t("categories.openai")}
                    </Link>
                    <Link
                      href="/category/claudeai"
                      onClick={() => setMobileMenuOpen(false)}
                      className="hover:text-primary text-sm transition-colors"
                    >
                      {t("categories.claudeai")}
                    </Link>
                  </div>
                </div>
              </nav>
            </SheetContent>
          </Sheet>

          {/* Mobile Search Button */}
          <SearchBar className="md:hidden" />
        </div>
      </div>
    </header>
  );
}
