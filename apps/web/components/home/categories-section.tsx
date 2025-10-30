'use client';

import { useTranslations } from "next-intl";
import { NewsCategory } from "@/lib/api/types";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import {
  MessageSquare,
  Github,
  Twitter,
  Linkedin,
  Facebook,
  Instagram,
  Music,
  Youtube,
  Cpu,
} from "lucide-react";

const categoryIcons = {
  [NewsCategory.Reddit]: MessageSquare,
  [NewsCategory.GitHub]: Github,
  [NewsCategory.Twitter]: Twitter,
  [NewsCategory.LinkedIn]: Linkedin,
  [NewsCategory.Facebook]: Facebook,
  [NewsCategory.Instagram]: Instagram,
  [NewsCategory.TikTok]: Music,
  [NewsCategory.YouTube]: Youtube,
  [NewsCategory.Technology]: Cpu,
};

export function CategoriesSection() {
  const t = useTranslations('categories');

  const categories = Object.values(NewsCategory);

  return (
    <section className="space-y-6">
      <h2 className="text-3xl font-bold">{t('all')}</h2>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
        {categories.map((category) => {
          const Icon = categoryIcons[category];
          return (
            <Card
              key={category}
              className="hover:shadow-md transition-shadow cursor-pointer group"
            >
              <Link href={`/category/${category}`}>
                <CardHeader className="pb-3">
                  <div className="flex justify-center mb-2">
                    <div className="h-12 w-12 rounded-full bg-primary/10 flex items-center justify-center group-hover:bg-primary/20 transition-colors">
                      <Icon className="h-6 w-6 text-primary" />
                    </div>
                  </div>
                  <CardTitle className="text-center text-sm font-medium">
                    {t(category)}
                  </CardTitle>
                </CardHeader>
              </Link>
            </Card>
          );
        })}
      </div>
    </section>
  );
}
