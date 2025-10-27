import { ImageResponse } from 'next/og';

export const runtime = 'edge';
export const size = {
  width: 32,
  height: 32,
};
export const contentType = 'image/png';

export default function Icon() {
  return new ImageResponse(
    (
      <div
        style={{
          width: '100%',
          height: '100%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          background: '#3b82f6',
          borderRadius: '6px',
        }}
      >
        <div
          style={{
            width: '24px',
            height: '18px',
            background: 'white',
            borderRadius: '2px',
            display: 'flex',
            flexDirection: 'column',
            padding: '3px',
            gap: '2px',
          }}
        >
          <div style={{ width: '8px', height: '2px', background: '#3b82f6', borderRadius: '1px' }} />
          <div style={{ width: '18px', height: '2px', background: '#3b82f6', borderRadius: '1px' }} />
          <div style={{ width: '14px', height: '2px', background: '#3b82f6', borderRadius: '1px' }} />
        </div>
      </div>
    ),
    {
      ...size,
    }
  );
}
