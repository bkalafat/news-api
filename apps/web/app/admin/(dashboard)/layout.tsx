import { AdminLayout } from '@/components/admin/admin-layout';

// Disable static generation for admin pages
export const dynamic = 'force-dynamic';

export default function AdminDashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <AdminLayout>{children}</AdminLayout>;
}
