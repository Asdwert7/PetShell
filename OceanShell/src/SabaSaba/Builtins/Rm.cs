using System;
using System.Collections.Generic;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class Rm : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (!VfsManager.IsReady) { Console.WriteLine("rm: VFS не инициализирована"); return 2; }
            if (args.Count != 1)
            {
                Console.WriteLine("Usage: rm <file>");
                return 2;
            }

            var ok = VfsManager.RemoveFile(args[0], out var err);
            if (!ok)
            {
                Console.WriteLine(err);
                return 2;
            }
            return 0;
        }
    }
}