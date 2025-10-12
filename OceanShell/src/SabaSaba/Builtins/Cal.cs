using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SabaSaba.Builtins
{
    public sealed class Cal : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            // cal            -> текущий месяц
            // cal 10 2025    -> конкретный месяц и год
            int month, year;
            var now = DateTime.Now;

            if (args.Count == 0) { month = now.Month; year = now.Year; }
            else if (args.Count == 2 && int.TryParse(args[0], out month) && int.TryParse(args[1], out year))
            {
                if (month < 1 || month > 12 || year < 1 || year > 9999) { Console.WriteLine("cal: неверные значения месяца/года"); return 2; }
            }
            else
            {
                Console.WriteLine("Usage: cal [month 1-12] [year]");
                return 2;
            }

            PrintCalendar(month, year);
            return 0;
        }

        private static void PrintCalendar(int month, int year)
        {
            var sb = new StringBuilder();
            var culture = CultureInfo.InvariantCulture;
            var first = new DateTime(year, month, 1);
            var days = DateTime.DaysInMonth(year, month);
            var title = $"{culture.DateTimeFormat.GetMonthName(month)} {year}";
            sb.AppendLine("     " + title);
            sb.AppendLine("Mo Tu We Th Fr Sa Su"); // недельная шапка (понедельник - первый день)

            // Сдвиг: индекс дня недели, где Monday=1 ... Sunday=0/7
            int offset = ((int)first.DayOfWeek == 0) ? 7 : (int)first.DayOfWeek; // Sunday -> 7
            int col = 1;

            // печатаем пустые клетки до первого дня
            for (int i = 1; i < offset; i++) { sb.Append("   "); col++; }

            // печать дней
            for (int d = 1; d <= days; d++)
            {
                sb.Append(d < 10 ? $" {d}" : $"{d}");
                if (col < 7) { sb.Append(' '); col++; }
                else { sb.AppendLine(); col = 1; }
                if (col == 1 && d != days) sb.Append(""); // перенос строки уже сделан
            }

            if (col != 1) sb.AppendLine(); // завершающая строка
            Console.Write(sb.ToString());
        }
    }
}