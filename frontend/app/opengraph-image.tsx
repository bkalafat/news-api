import { ImageResponse } from "next/og";

export const runtime = "edge";
export const alt = "Teknoloji Haberleri";
export const size = {
  width: 1200,
  height: 630,
};
export const contentType = "image/png";

export default async function Image() {
  return new ImageResponse(
    (
      <div
        style={{
          background: "linear-gradient(135deg, #0A0A0A 0%, #1e293b 100%)",
          width: "100%",
          height: "100%",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          fontFamily: "system-ui, sans-serif",
          position: "relative",
        }}
      >
        {/* Background Pattern */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            background:
              "radial-gradient(circle at 30% 50%, rgba(59, 130, 246, 0.15) 0%, transparent 50%)",
          }}
        />

        {/* Icon */}
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            marginBottom: "40px",
          }}
        >
          <div
            style={{
              width: "120px",
              height: "120px",
              background: "rgba(59, 130, 246, 0.2)",
              borderRadius: "24px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              border: "3px solid #3b82f6",
            }}
          >
            <div
              style={{
                width: "80px",
                height: "60px",
                background: "#3b82f6",
                borderRadius: "8px",
                display: "flex",
                flexDirection: "column",
                padding: "12px",
                gap: "8px",
              }}
            >
              <div
                style={{ width: "30px", height: "4px", background: "white", borderRadius: "2px" }}
              />
              <div
                style={{ width: "56px", height: "4px", background: "white", borderRadius: "2px" }}
              />
              <div
                style={{ width: "45px", height: "4px", background: "white", borderRadius: "2px" }}
              />
            </div>
          </div>
        </div>

        {/* Title */}
        <div
          style={{
            fontSize: "72px",
            fontWeight: "bold",
            color: "white",
            marginBottom: "20px",
            textAlign: "center",
          }}
        >
          Teknoloji Haberleri
        </div>

        {/* Subtitle */}
        <div
          style={{
            fontSize: "32px",
            color: "#94a3b8",
            marginBottom: "40px",
            textAlign: "center",
          }}
        >
          BBC ve güvenilir kaynaklardan güncel haberler
        </div>

        {/* Badge */}
        <div
          style={{
            display: "flex",
            alignItems: "center",
            padding: "16px 32px",
            background: "rgba(59, 130, 246, 0.2)",
            border: "2px solid #3b82f6",
            borderRadius: "24px",
            fontSize: "24px",
            fontWeight: "600",
            color: "#3b82f6",
          }}
        >
          Modern • Hızlı • Güvenilir
        </div>
      </div>
    ),
    {
      ...size,
    }
  );
}
