using System;
using System.Collections.Generic;
using System.Linq;
using SabaSaba.Builtins;

namespace SabaSaba
{
    public static class CommandRouter
    {
        public static int Route(IReadOnlyList<string> tokens)
        {
            if (tokens.Count == 0) return 0;

            var name = tokens[0];
            var args = tokens.Count > 1 ? tokens.Skip(1).ToList() : new List<string>();

            ICommand? cmd = name switch
            {
                "exit"     => new Exit(),
                "ls"       => new Ls(),
                "cd"       => new Cd(),
                "pwd"      => new Pwd(),
                "date"     => new DateCmd(),
                "cal"      => new Cal(),
                "vfs-init" => new VfsInit(),
                _          => null
            };

            if (cmd == null)
            {
                Console.WriteLine($"Ошибка: неизвестная команда '{name}'");
                return 127;
            }

            try { return cmd.Execute(args); }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения команды '{name}': {ex.Message}");
                return 1;
            }
        }
    }
}