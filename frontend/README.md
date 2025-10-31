# 📰 Teknoloji Haberleri - Frontend

Modern, performanslı ve SEO-dostu Türk teknoloji haberleri web sitesi. Next.js 16, TypeScript, ve TailwindCSS ile geliştirilmiştir.

## 🌟 Özellikler

- ⚡ **Next.js 16** - App Router ile modern React framework
- 🎨 **TailwindCSS v4** - Utility-first CSS framework
- 🔷 **TypeScript** - Type-safe development
- 🎯 **Shadcn/ui** - Beautiful, accessible components
- 🔄 **React Query** - Powerful data fetching & caching
- 🌐 **next-intl** - Turkish localization support
- 🎬 **Framer Motion** - Smooth animations
- 📱 **Responsive Design** - Mobile-first approach
- 🚀 **Performance Optimized** - Image optimization, lazy loading
- 🔍 **SEO Optimized** - Meta tags, sitemap, structured data
- ♿ **Accessible** - WCAG compliant components

## 📋 Prerequisites

- Node.js 18+ (recommended: 20+)
- npm or yarn or pnpm
- News API backend running (see [../README.md](../README.md))

## 🚀 Quick Start

### 1. Install Dependencies

```bash
cd web
npm install
```

### 2. Configure Environment Variables

Copy the example environment file:

```bash
cp .env.example .env.local
```

Update `.env.local` with your settings:

```env
# Backend API URL (default: http://localhost:5000)
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_API_BASE_PATH=/api/news

# Site Configuration
NEXT_PUBLIC_SITE_URL=http://localhost:3000
NEXT_PUBLIC_SITE_NAME=Teknoloji Haberleri
```

### 3. Start the Backend API

Make sure the News API backend is running on `http://localhost:5000`:

```bash
cd ../backend
dotnet run
```

### 4. Run the Development Server

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) to see the website.

## 📁 Project Structure

```
web/
├── app/                          # Next.js App Router pages
│   ├── layout.tsx               # Root layout with providers
│   ├── page.tsx                 # Homepage
│   ├── globals.css              # Global styles
│   ├── robots.ts                # SEO robots.txt
│   ├── sitemap.ts               # SEO sitemap
│   └── category/                # Category pages
├── components/                   # React components
│   ├── layout/                  # Layout components
│   ├── home/                    # Homepage sections
│   ├── news/                    # News card components
│   └── ui/                      # Shadcn UI components
├── lib/                         # Utilities and configurations
│   ├── api/                     # API client and hooks
│   ├── providers/               # React context providers
│   └── utils.ts                # Utility functions
├── messages/                    # Internationalization
│   └── tr.json                 # Turkish translations
├── i18n/                       # i18n configuration
├── public/                     # Static assets
├── .env.local                  # Environment variables
├── next.config.ts              # Next.js configuration
└── package.json                # Dependencies
```

## 🔌 API Integration

The frontend communicates with the News API backend using React Query hooks:

```typescript
import { useAllNews, useNewsByCategory } from "@/lib/api/hooks";

// Get all news
const { data, isLoading, error } = useAllNews();

// Get news by category
const { data: techNews } = useNewsByCategory("technology");
```

## 🔧 Development with Context7 MCP

This project is optimized for use with **Context7 MCP** for up-to-date documentation.

Add `use context7` to your prompts:

```
Create a new component for filtering news. use context7
```

## 🛠️ Available Scripts

```bash
npm run dev              # Start dev server
npm run build           # Build for production
npm run start           # Start production server
npm run lint            # Run ESLint
```

## 📚 Resources

- [Next.js Documentation](https://nextjs.org/docs)
- [React Query Documentation](https://tanstack.com/query/latest)
- [TailwindCSS Documentation](https://tailwindcss.com/docs)
- [Shadcn/ui Documentation](https://ui.shadcn.com/)
- [Context7 Documentation](https://context7.com/)

## 📄 License

Part of the News API monorepo.

---

**Need Help?** Check the [main project README](../README.md).
