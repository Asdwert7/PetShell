using System;
using System.IO;
using FinPrompt;
using AquaParse;
using SabaSaba;

namespace GuraShell
{
    public static class ScriptRunner
    {
        /// Возвращает код из exit, если в скрипте был exit; иначе null.
        public static int? Run(AppConfig cfg, Theme theme)
        {
            var path = cfg.ScriptPath!;
            if (!File.Exists(path))
            {
                Console.WriteLine($"Ошибка: не найден скрипт '{path}'");
                return null;
            }

            var text = File.ReadAllText(path);

            // 1) удалить блочные комментарии /* ... */
            text = RemoveBlockComments(text);

            // 2) построчно обработать
            foreach (var raw in text.Split('\n'))
            {
                var line = StripLineComment(raw); // убрать //...
                line = line.Trim();
                if (line.Length == 0) continue;

                // имитация ввода
                Console.Write(PromptBuilder.Builder(theme));
                Console.WriteLine(line);

                var parse = Tokenizer.Tokenize(line);
                if (!parse.Success)
                {
                    Console.WriteLine($"Parse error: {parse.ErrorMessage} (pos {parse.ErrorPosition})");
                    continue;
                }

                var tokens = parse.Tokens!;
                int code = CommandRouter.Route(tokens);

                if (tokens.Count > 0 && tokens[0] == "exit")
                    return code; // прерываем выполнение скрипта и приложения
            }

            return null;
        }

        private static string RemoveBlockComments(string src)
        {
            // простой проход: ищем '/*' и '*/' и вырезаем
            // (можно реализовать сканером без регэкспов — надёжнее)
            var result = new System.Text.StringBuilder(src.Length);
            int i = 0;
            while (i < src.Length)
            {
                if (i + 1 < src.Length && src[i] == '/' && src[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < src.Length && !(src[i] == '*' && src[i + 1] == '/')) i++;
                    if (i + 1 < src.Length) i += 2; // пропускаем закрывающий */
                    continue;
                }
                result.Append(src[i]);
                i++;
            }
            return result.ToString();
        }

        private static string StripLineComment(string line)
        {
            // обрезаем //... если они не внутри кавычек
            bool inSingle = false, inDouble = false;
            for (int i = 0; i + 1 < line.Length; i++)
            {
                char c = line[i];
                if (c == '\'' && !inDouble) inSingle = !inSingle;
                else if (c == '"' && !inSingle) inDouble = !inDouble;
                else if (!inSingle && !inDouble && c == '/' && line[i + 1] == '/')
                    return line.Substring(0, i);
            }
            return line;
        }
    }
}