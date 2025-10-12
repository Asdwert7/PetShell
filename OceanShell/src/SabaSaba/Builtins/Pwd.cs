using System;
using System.Collections.Generic;
using LilyFS;

namespace SabaSaba.Builtins
{
    public sealed class Pwd : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            if (!VfsManager.IsReady)
            {
                Console.WriteLine("/");
                return 0;
            }
            Console.WriteLine(VfsManager.Pwd());
            return 0;
        }
    }
}