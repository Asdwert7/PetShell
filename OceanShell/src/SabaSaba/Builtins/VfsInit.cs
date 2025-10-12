using System;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class VfsInit : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (args.Count > 1)
            {
                Console.WriteLine("Usage: vfs-init [json_path]");
                return 2;
            }

            if (args.Count == 0)
            {
                VfsManager.ResetToDefault();
                return 0;
            }

            var path = args[0];
            VfsManager.Load(path);
            return 0;
        }
    }
}