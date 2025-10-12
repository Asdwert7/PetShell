using FinPrompt;
using AquaParse;
using SabaSaba;

namespace GuraShell
{
    public static class Program
    {
        private static AppConfig ParseArgs(string[] args)
        {
            var cfg = new AppConfig();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--vfs-path":
                        if (i + 1 < args.Length) cfg.VfsPath = args[++i];
                        else Console.WriteLine("Ошибка: ожидается путь после --vfs-path");
                        break;
                    case "--script":
                        if (i + 1 < args.Length) cfg.ScriptPath = args[++i];
                        else Console.WriteLine("Ошибка: ожидается путь после --script");
                        break;
                    default:
                        Console.WriteLine($"Предупреждение: неизвестный параметр '{args[i]}'");
                        break;
                }
            }
            return cfg;
        }

        private static void DebugDump(AppConfig cfg)
        {
            Console.WriteLine("=== Debug: параметры запуска ===");
            Console.WriteLine($"--vfs-path : {cfg.VfsPath ?? "(не задан)"}");
            Console.WriteLine($"--script   : {cfg.ScriptPath ?? "(не задан)"}");
            Console.WriteLine("================================");
        }
        public static void Main()
        {
            // Настройка для темы. см в класс FinPrompt
            Theme theme = new Theme
            {
                ConsoleName = "GawrShell",
                ShowConsoleName = true,
                PromptChar = " ~%",
                UseColors = false,
                ShowOnlyLastPathSegment = true
            };

            while (true)
            {
                Console.Write(PromptBuilder.Builder(theme));
                var line = Console.ReadLine();
                if (line is null) break;

                if (line.Trim().Length == 0) continue;

                var parse = Tokenizer.Tokenize(line);
                if (!parse.Success)
                {
                    Console.WriteLine($"Parse error: {parse.ErrorMessage}");
                    continue;
                }

                var tokens = parse.Tokens!;
                int code = CommandRouter.Route(tokens);

                if (tokens[0] == "exit")
                {
                    Console.WriteLine($"Завершение консоли (код {code})");
                    break;
                }
            }

        }
    }
}