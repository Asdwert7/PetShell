using RuiSys;

namespace FinPrompt
{
    /// <summary>
    /// Строит строку приглашения вида:
    /// user@host[GawrGura]:~$ 
    /// </summary>
    public static class PromptBuilder
    {
        public static string Builder(Theme? theme = null)
        {
            if (theme == null)
                theme = new Theme();

            string user = SystemInfo.GetUser();
            string host = SystemInfo.GetHost();
            string cwd = SystemInfo.GetCurrentWorkingDirectory();
            string home = SystemInfo.GetHomeDirectory();

            string displayPath = ShortTildaToHome(cwd, home);

            if (theme.ShowOnlyLastPathSegment)
            {
                displayPath = PathTail(displayPath);
            }

            // Для вывода имени в консоли 
            var tega = (theme.ShowConsoleName && !string.IsNullOrEmpty(theme.ConsoleName))
            ? $"[{theme.ConsoleName}]"
            : "";

            var promptChar = string.IsNullOrEmpty(theme.PromptChar) ? "%" : theme.PromptChar;

            return $"{user}@{host}{tega}:{promptChar} ";

        }
        /// <summary>
        /// возвращает более короткую версию пути, где домашняя директория заменяется на ~.
        /// </summary>
        /// <param name="cwd"> текущий путь (current working directory) </param>
        /// <param name="home"> домашний каталог пользователя </param>
        /// <returns></returns>
        private static string ShortTildaToHome(string cwd, string home)
        {
            if (string.IsNullOrEmpty(cwd)) { return "~"; } // когда путь нуль или "" то верни ~
            if (string.IsNullOrEmpty(home)) { return cwd; } // если нечего заменять то верни как есть

            if (cwd == home) return "~"; // Если совпало то вернет тильду
            var separator = Path.DirectorySeparatorChar;
            if (home.Length > 1 && home.EndsWith(separator.ToString()))
            {
                home = home.TrimEnd(separator);
            }
            if (cwd.StartsWith(home + separator))
            {
                return "~" + cwd.Substring(home.Length);
            }
            return cwd == string.Empty ? "/" : cwd;
        }
        /// <summary>
        /// берёт путь и возвращает его "хвост" — то есть имя последней папки или файла.
        /// </summary>
        /// <param name="path"></param>
        /// <remarks>
        /// из полного пути /Users/alex/Documents/Projects вернет Projects
        /// </remarks>
        /// <returns></returns>
        private static string PathTail(string path)
        {
            if (path == "~") return "~";

            if (string.IsNullOrEmpty(path)) return "/";

            var sep = Path.DirectorySeparatorChar; // разделитель для крос платформы, чтобы на винде работало)))
            if (path.EndsWith(sep) && path != "/")
                path = path.TrimEnd(sep); // Уберет / или \\

            var last = Path.GetFileName(path); // Последний элемент выдернет из конца директории
            return string.IsNullOrEmpty(last) ? path : last;
        }
    }
}