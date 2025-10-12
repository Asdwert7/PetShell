using System;

namespace SabaSaba.Builtins
{
    public sealed class Cd : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            Console.WriteLine("Команда-заглушка: cd");
            if (args.Count > 0)
                Console.WriteLine("Аргументы: " + string.Join(", ", args));
            else
                Console.WriteLine("Аргументы: (нет)");
            return 0;
        }
    }
}