import { MetadataRoute } from 'next';

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: 'Teknoloji Haberleri - Türkiye\'nin Teknoloji Gazetesi',
    short_name: 'Teknoloji Haberleri',
    description: 'BBC ve güvenilir kaynaklardan son dakika teknoloji haberleri',
    start_url: '/',
    display: 'standalone',
    background_color: '#0A0A0A',
    theme_color: '#3b82f6',
    orientation: 'portrait-primary',
    categories: ['news', 'technology'],
    lang: 'tr',
    dir: 'ltr',
    icons: [
      {
        src: '/icon-192.svg',
        sizes: '192x192',
        type: 'image/svg+xml',
        purpose: 'any',
      },
      {
        src: '/icon-512.svg',
        sizes: '512x512',
        type: 'image/svg+xml',
        purpose: 'any',
      },
    ],
  };
}
