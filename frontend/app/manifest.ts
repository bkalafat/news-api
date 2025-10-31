import { MetadataRoute } from "next";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "Teknoloji Haberleri - Türkiye'nin Teknoloji Gazetesi",
    short_name: "Teknoloji Haberleri",
    description:
      "Son dakika teknoloji haberleri, güncel gelişmeler, yazılım, donanım, yapay zeka ve teknoloji dünyasından tüm haberler.",
    start_url: "/",
    display: "standalone",
    background_color: "#0A0A0A",
    theme_color: "#3b82f6",
    orientation: "portrait-primary",
    categories: ["news", "technology", "business"],
    lang: "tr",
    dir: "ltr",
    scope: "/",
    icons: [
      {
        src: "/icon-192.svg",
        sizes: "192x192",
        type: "image/svg+xml",
        purpose: "any",
      },
      {
        src: "/icon-512.svg",
        sizes: "512x512",
        type: "image/svg+xml",
        purpose: "maskable",
      },
    ],
  };
}
