using System;
using System.Collections.Generic;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class Cd : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (!VfsManager.IsReady)
            {
                Console.WriteLine("cd: VFS не инициализирована (используйте vfs-init)");
                return 2;
            }

            string path = args.Count == 0 ? "~" : args[0]; // без аргументов → домашний (~ → /home)
            if (args.Count > 1)
            {
                Console.WriteLine("Usage: cd [path]");
                return 2;
            }

            if (!VfsManager.ChangeDir(path, out var err))
            {
                Console.WriteLine(err);
                return 2;
            }
            return 0;
        }
    }
}