using System;

namespace LilyFS
{
    public static class VfsManager
    {
        public static VfsDir? Current { get; private set; }

        public static void Load(string path)
        {
            try
            {
                Current = VfsLoader.LoadFromJson(path);
                Console.WriteLine($"VFS успешно загружена: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки VFS: {ex.Message}");
                Current = null;
            }
        }

        public static void ResetToDefault()
        {
            Current = new VfsDir { Name = "/" };
            Console.WriteLine("VFS сброшена до состояния по умолчанию (пустая).");
        }
    }
}