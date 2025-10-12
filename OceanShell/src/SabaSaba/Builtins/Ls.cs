using System;
using System.Collections.Generic;
using System.Linq;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class Ls : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (!VfsManager.IsReady)
            {
                Console.WriteLine("ls: VFS не инициализирована (используйте vfs-init)");
                return 2;
            }

            string? target = null;
            // простой парсер: поддержим только путь, без сложных флагов (-l, -a можно добавить позже)
            if (args.Count > 1)
            {
                Console.WriteLine("Usage: ls [path]");
                return 2;
            }
            if (args.Count == 1) target = args[0];

            var items = VfsManager.List(target, out var err);
            if (err != null) { Console.WriteLine(err); return 2; }

            // печать: каталоги с '/', файлы просто по имени
            foreach (var n in items.OrderBy(n => n.Name, StringComparer.Ordinal))
            {
                if (n.IsDirectory) Console.WriteLine(n.Name + "/");
                else                Console.WriteLine(n.Name);
            }
            return 0;
        }
    }
}