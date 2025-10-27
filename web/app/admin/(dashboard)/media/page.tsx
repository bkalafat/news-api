export default function AdminMediaPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Medya Yönetimi</h1>
        <p className="text-slate-600 dark:text-slate-400 mt-1">
          Görseller ve dosyalar
        </p>
      </div>

      <div className="border-2 border-dashed rounded-lg p-12 text-center">
        <p className="text-lg text-slate-500">Yakında gelecek</p>
        <p className="text-sm text-slate-400 mt-2">
          Görsel yükleme ve yönetim özellikleri
        </p>
      </div>
    </div>
  );
}
