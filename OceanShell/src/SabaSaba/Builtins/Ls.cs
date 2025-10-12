using System;

namespace SabaSaba.Builtins
{
    public sealed class Ls : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            Console.WriteLine("Команда-заглушка: ls");
            if (args.Count > 0)
                Console.WriteLine("Аргументы: " + string.Join(", ", args));
            else
                Console.WriteLine("Аргументы: (нет)");
            return 0;
        }
    }
}