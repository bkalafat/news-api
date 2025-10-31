"use client";

import { useTranslations } from "next-intl";
import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export function HeroSection() {
  const t = useTranslations("home.hero");

  return (
    <section className="relative overflow-hidden">
      {/* Animated gradient background */}
      <div className="absolute inset-0 -z-10">
        <div className="absolute inset-0 bg-gradient-to-br from-primary/20 via-accent/10 to-background" />
        <div className="absolute top-0 right-0 w-96 h-96 bg-primary/30 rounded-full blur-3xl animate-pulse" />
        <div className="absolute bottom-0 left-0 w-96 h-96 bg-accent/20 rounded-full blur-3xl animate-pulse [animation-delay:1s]" />
      </div>

      <div className="container mx-auto px-4 py-32 md:py-40">
        <div className="mx-auto max-w-4xl space-y-8 text-center">
          <h1 className="gradient-text text-5xl font-extrabold tracking-tight md:text-7xl lg:text-8xl">
            {t("title")}
          </h1>
          <p className="text-muted-foreground text-xl md:text-2xl lg:text-3xl max-w-2xl mx-auto font-light">
            {t("subtitle")}
          </p>
          <div className="flex justify-center pt-4">
            <Button 
              asChild 
              size="lg" 
              className="group bg-gradient-to-r from-primary to-primary/80 hover:from-primary/90 hover:to-primary/70 shadow-lg shadow-primary/25 text-lg px-8 py-6 rounded-xl"
            >
              <Link href="#latest">
                {t("cta")}
                <ArrowRight className="ml-2 h-5 w-5 transition-transform group-hover:translate-x-1" />
              </Link>
            </Button>
          </div>
        </div>
      </div>

      {/* Bottom fade */}
      <div className="absolute bottom-0 inset-x-0 h-24 bg-gradient-to-t from-background to-transparent" />
    </section>
  );
}
