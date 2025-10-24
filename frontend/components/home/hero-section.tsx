'use client';

import { useTranslations } from "next-intl";
import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export function HeroSection() {
  const t = useTranslations('home.hero');

  return (
    <section className="relative bg-gradient-to-br from-primary/10 via-background to-background">
      <div className="container mx-auto px-4 py-24 md:py-32">
        <div className="max-w-3xl mx-auto text-center space-y-8">
          <h1 className="text-4xl md:text-6xl font-bold tracking-tight">
            {t('title')}
          </h1>
          <p className="text-xl md:text-2xl text-muted-foreground">
            {t('subtitle')}
          </p>
          <div className="flex justify-center">
            <Button asChild size="lg" className="group">
              <Link href="#latest">
                {t('cta')}
                <ArrowRight className="ml-2 h-4 w-4 transition-transform group-hover:translate-x-1" />
              </Link>
            </Button>
          </div>
        </div>
      </div>
      
      {/* Decorative elements */}
      <div className="absolute inset-0 -z-10 overflow-hidden">
        <div className="absolute -top-1/2 -right-1/2 w-full h-full bg-gradient-to-br from-primary/5 to-transparent rounded-full blur-3xl" />
        <div className="absolute -bottom-1/2 -left-1/2 w-full h-full bg-gradient-to-tr from-primary/5 to-transparent rounded-full blur-3xl" />
      </div>
    </section>
  );
}
