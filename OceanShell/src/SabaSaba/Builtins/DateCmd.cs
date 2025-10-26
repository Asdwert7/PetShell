using System;
using System.Collections.Generic;
using System.Globalization;

namespace SabaSaba.Builtins
{
    public sealed class DateCmd : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            // Простейший вывод: локальные дата и время
            if (args.Count == 0)
            {
                Console.WriteLine(DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture));
                return 0;
            }
            if (args.Count == 1 && args[0].StartsWith("+"))
            {
                var fmt = args[0].Substring(1);
                // поддержим .NET формат (для простоты), не POSIX спецификаторы
                Console.WriteLine(DateTime.Now.ToString(fmt));
                return 0;
            }

            Console.WriteLine("Usage: date [+'format']");
            return 2;
        }
    }
}