'use client';

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
    <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto px-4">
        <div className="flex h-16 items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2">
            <Newspaper className="h-6 w-6" />
            <span className="font-bold text-xl hidden sm:inline-block">
              {t('app.title')}
            </span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center space-x-6">
            <Link
              href="/"
              className="text-sm font-medium transition-colors hover:text-primary"
            >
              {t('navigation.home')}
            </Link>
            <Link
              href="/categories"
              className="text-sm font-medium transition-colors hover:text-primary"
            >
              {t('navigation.categories')}
            </Link>
            <Link
              href="/about"
              className="text-sm font-medium transition-colors hover:text-primary"
            >
              {t('navigation.about')}
            </Link>
          </nav>

          {/* Search Bar */}
          <SearchBar className="flex-1 max-w-md hidden md:flex" />

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
              <nav className="flex flex-col space-y-4 mt-8">
                <Link
                  href="/"
                  onClick={() => setMobileMenuOpen(false)}
                  className="text-lg font-medium transition-colors hover:text-primary"
                >
                  {t('navigation.home')}
                </Link>
                <Link
                  href="/categories"
                  onClick={() => setMobileMenuOpen(false)}
                  className="text-lg font-medium transition-colors hover:text-primary"
                >
                  {t('navigation.categories')}
                </Link>
                <Link
                  href="/about"
                  onClick={() => setMobileMenuOpen(false)}
                  className="text-lg font-medium transition-colors hover:text-primary"
                >
                  {t('navigation.about')}
                </Link>
                <div className="border-t pt-4">
                  <h3 className="font-semibold mb-3">{t('navigation.categories')}</h3>
                  <div className="flex flex-col space-y-2">
                    <Link
                      href="/category/technology"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.technology')}
                    </Link>
                    <Link
                      href="/category/world"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.world')}
                    </Link>
                    <Link
                      href="/category/business"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.business')}
                    </Link>
                    <Link
                      href="/category/science"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.science')}
                    </Link>
                    <Link
                      href="/category/health"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.health')}
                    </Link>
                    <Link
                      href="/category/entertainment"
                      onClick={() => setMobileMenuOpen(false)}
                      className="text-sm transition-colors hover:text-primary"
                    >
                      {t('categories.entertainment')}
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
