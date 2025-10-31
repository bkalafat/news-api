"use client";

import { useTranslations } from "next-intl";
import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export function HeroSection() {
  const t = useTranslations("home.hero");

  return (
    <section className="from-primary/10 via-background to-background relative bg-gradient-to-br">
      <div className="container mx-auto px-4 py-24 md:py-32">
        <div className="mx-auto max-w-3xl space-y-8 text-center">
          <h1 className="text-4xl font-bold tracking-tight md:text-6xl">{t("title")}</h1>
          <p className="text-muted-foreground text-xl md:text-2xl">{t("subtitle")}</p>
          <div className="flex justify-center">
            <Button asChild size="lg" className="group">
              <Link href="#latest">
                {t("cta")}
                <ArrowRight className="ml-2 h-4 w-4 transition-transform group-hover:translate-x-1" />
              </Link>
            </Button>
          </div>
        </div>
      </div>

      {/* Decorative elements */}
      <div className="absolute inset-0 -z-10 overflow-hidden">
        <div className="from-primary/5 absolute -top-1/2 -right-1/2 h-full w-full rounded-full bg-gradient-to-br to-transparent blur-3xl" />
        <div className="from-primary/5 absolute -bottom-1/2 -left-1/2 h-full w-full rounded-full bg-gradient-to-tr to-transparent blur-3xl" />
      </div>
    </section>
  );
}
