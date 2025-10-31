import { ImageResponse } from "next/og";

export const runtime = "edge";
export const size = {
  width: 180,
  height: 180,
};
export const contentType = "image/png";

export default function Icon() {
  return new ImageResponse(
    (
      <div
        style={{
          width: "100%",
          height: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          background: "#3b82f6",
          borderRadius: "40px",
        }}
      >
        <div
          style={{
            width: "120px",
            height: "90px",
            background: "white",
            borderRadius: "12px",
            display: "flex",
            flexDirection: "column",
            padding: "16px",
            gap: "12px",
          }}
        >
          <div
            style={{ width: "40px", height: "8px", background: "#3b82f6", borderRadius: "4px" }}
          />
          <div
            style={{ width: "88px", height: "8px", background: "#3b82f6", borderRadius: "4px" }}
          />
          <div
            style={{ width: "70px", height: "8px", background: "#3b82f6", borderRadius: "4px" }}
          />
        </div>
      </div>
    ),
    {
      ...size,
    }
  );
}
