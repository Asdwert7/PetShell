using System;
using System.Collections.Generic;

namespace LilyFS
{
    public static class VfsManager
    {
        public static VfsDir? Root { get; private set; }
        public static VfsDir? Cwd  { get; private set; }   // текущий виртуальный каталог

        public static void Load(string path)
        {
            try
            {
                Root = VfsLoader.LoadFromJson(path);
                Cwd  = Root; // после загрузки в корень
                Console.WriteLine($"VFS успешно загружена: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки VFS: {ex.Message}");
                Root = null;
                Cwd  = null;
            }
        }

        public static void ResetToDefault()
        {
            Root = new VfsDir { Name = "/" };
            Cwd  = Root;
            Console.WriteLine("VFS сброшена до состояния по умолчанию (пустая).");
        }

        public static bool IsReady => Root != null && Cwd != null;

        // --- ПУТИ ---

        // Нормализация и поиск узла по пути (абс/отн, '.', '..', '~' → /home)
        public static VfsNode? Resolve(string path)
        {
            if (!IsReady) return null;
            if (string.IsNullOrWhiteSpace(path)) return Cwd;

            // поддержка '~' (просто маппим на /home)
            if (path == "~") path = "/home";
            else if (path.StartsWith("~/")) path = "/home" + path.Substring(1);

            // абсолютный ли путь?
            var isAbs = path.StartsWith("/");
            var parts = SplitPath(path);

            // начинаем от корня или от текущей
            VfsDir? cursor = isAbs ? Root! : Cwd!;
            VfsNode current = cursor;

            foreach (var part in parts)
            {
                if (part == "." || part.Length == 0) continue;
                if (part == "..")
                {
                    if (current is VfsDir d)
                    {
                        if (d.Parent != null) { current = d.Parent; }
                        // если Parent == null, мы в корне: остаёмся там (не выходим выше)
                        continue;
                    }
                }

                if (current is not VfsDir dir) return null;
                var next = dir.Find(part);
                if (next == null) return null;
                current = next;
            }

            return current;
        }

        public static bool ChangeDir(string path, out string? error)
        {
            error = null;
            var node = Resolve(path);
            if (node == null) { error = $"cd: нет такого пути: {path}"; return false; }
            if (node is not VfsDir d) { error = $"cd: не каталог: {path}"; return false; }
            Cwd = d;
            return true;
        }

        public static string Pwd()
        {
            if (!IsReady) return "/";
            return GetVirtualPath(Cwd!);
        }

        public static IEnumerable<VfsNode> List(string? path, out string? error)
        {
            error = null;
            VfsNode? node = string.IsNullOrWhiteSpace(path) ? Cwd : Resolve(path!);
            if (node == null) { error = $"ls: нет такого пути: {path}"; return Array.Empty<VfsNode>(); }
            if (node is VfsFile f) return new VfsNode[] { f }; // ls на файл — покажем сам файл
            return ((VfsDir)node).Children;
        }

        // построение пути вида /a/b/c для любого узла
        public static string GetVirtualPath(VfsNode node)
        {
            var stack = new Stack<string>();
            var cur = node;
            while (cur != null)
            {
                stack.Push(cur.Name == "/" ? "" : cur.Name);
                cur = cur.Parent!;
            }
            return "/" + string.Join("/", stack.Where(s => s.Length > 0));
        }

        private static IEnumerable<string> SplitPath(string path)
        {
            return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }
        // Утилита: найти родителя и имя последнего сегмента
        private static (VfsDir? parent, string leaf)? GetParentAndLeaf(string path)
        {
            if (!IsReady) return null;

            // Нормализуем ~, абсолютность и т.п. через Resolve, но нам нужен родитель
            // Разобьём вручную: parentPath + leaf
            if (string.IsNullOrWhiteSpace(path)) return null;

            // ~ и ~/path → /home...
            if (path == "~") path = "/home";
            else if (path.StartsWith("~/")) path = "/home" + path.Substring(1);

            var isAbs = path.StartsWith("/");
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parts.Count == 0) return (Root!, "/"); // попытка удалить корень

            string leaf = parts[^1];
            parts.RemoveAt(parts.Count - 1);

            // вычисляем родителя
            VfsNode current = isAbs ? Root! : Cwd!;
            foreach (var part in parts)
            {
                if (part == ".") continue;
                if (part == "..")
                {
                    if (current is VfsDir d && d.Parent != null) current = d.Parent;
                    continue;
                }
                if (current is not VfsDir dir) return null;
                var next = dir.Find(part);
                if (next == null) return null;
                current = next;
            }

            return current is VfsDir pd ? (pd, leaf) : null;
        }

        // rm: удалить файл. Директории не трогаем (для них rmdir).
        public static bool RemoveFile(string path, out string? error)
        {
            error = null;
            if (!IsReady) { error = "rm: VFS не инициализирована"; return false; }

            var info = GetParentAndLeaf(path);
            if (info is null) { error = $"rm: нет такого пути: {path}"; return false; }
            var (parent, leaf) = info.Value;

            if (parent == null) { error = "rm: нельзя удалить корень /"; return false; }

            var node = parent.Find(leaf);
            if (node == null) { error = $"rm: нет такого файла: {path}"; return false; }
            if (node.IsDirectory) { error = $"rm: это каталог: {path}"; return false; }

            parent.Children.Remove(node);
            return true;
        }

        // rmdir: удалить пустой каталог
        public static bool RemoveDir(string path, out string? error)
        {
            error = null;
            if (!IsReady) { error = "rmdir: VFS не инициализирована"; return false; }

            // запретим удалять корень
            if (path == "/" || string.IsNullOrWhiteSpace(path))
            {
                error = "rmdir: нельзя удалить корневой каталог";
                return false;
            }

            var info = GetParentAndLeaf(path);
            if (info is null) { error = $"rmdir: нет такого пути: {path}"; return false; }
            var (parent, leaf) = info.Value;
            if (parent == null) { error = "rmdir: внутренняя ошибка"; return false; }

            var node = parent.Find(leaf);
            if (node == null) { error = $"rmdir: нет такого каталога: {path}"; return false; }
            if (!node.IsDirectory) { error = $"rmdir: не каталог: {path}"; return false; }

            var dir = (VfsDir)node;
            if (dir.Children.Count > 0) { error = $"rmdir: каталог не пуст: {path}"; return false; }

            parent.Children.Remove(dir);
            // если удалили текущий Cwd — откатимся к родителю
            if (Cwd == dir) Cwd = parent;
            return true;
        }
    }
}