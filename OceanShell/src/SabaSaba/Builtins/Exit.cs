using System;

namespace SabaSaba.Builtins
{
    public sealed class Exit : ICommand
    {
        public int Execute(IReadOnlyList<string> args)
        {
            // Позже можно обрабатывать exit <code>
            int code = 0;
            if (args.Count > 0 && int.TryParse(args[0], out int parsed))
                code = parsed;

            // Просто возвращаем код (Signal для Shell)
            return code;
        }
    }
}
