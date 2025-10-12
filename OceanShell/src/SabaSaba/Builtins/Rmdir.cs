using System;
using System.Collections.Generic;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class Rmdir : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (!VfsManager.IsReady) { Console.WriteLine("rmdir: VFS не инициализирована"); return 2; }
            if (args.Count != 1)
            {
                Console.WriteLine("Usage: rmdir <dir>");
                return 2;
            }

            var ok = VfsManager.RemoveDir(args[0], out var err);
            if (!ok)
            {
                Console.WriteLine(err);
                return 2;
            }
            return 0;
        }
    }
}