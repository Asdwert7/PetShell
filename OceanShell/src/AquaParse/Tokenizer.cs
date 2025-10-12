using System.Text;

namespace AquaParse
{
    public sealed class ParseResult
    {
        public bool Success { get; }
        public IReadOnlyList<string>? Tokens { get; }
        public string? ErrorMessage { get; }
        public int ErrorPosition { get; }

        private ParseResult(bool success, IReadOnlyList<string>? tokens, string? errorMessage, int errorPosition)
        {
            Success = success;
            Tokens = tokens;
            ErrorMessage = errorMessage;
            ErrorPosition = errorPosition;
        }

        public static ParseResult Ok(List<string> tokens) => new ParseResult(true, tokens.AsReadOnly(), null, -1);
        public static ParseResult Error(string message, int pos) => new ParseResult(false, null, message, pos);
    }

    public static class Tokenizer
    {
        // Переносим enum ВНЕ метода, чтобы избежать ошибок компиляции в старых версиях C#
        private enum State { Normal, InSingle, InDouble }

        /// <summary>
        /// Токенизирует входную строку в стиле POSIX-like shell (упрощённо).
        /// Возвращает ParseResult: Success==true и Tokens, либо Success==false с ErrorMessage и ErrorPosition.
        /// Правила:
        /// - Пробелы и табы разделяют токены.
        /// - "..." — двойные кавычки: поддерживается экранирование через '\'.
        /// - '...' — одинарные кавычки: всё берём буквально до следующей одинарной кавычки.
        /// - Обратный слэш '\' в обычном контексте экранирует следующий символ.
        /// - Пустые кавычки дают пустой токен.
        /// </summary>
        public static ParseResult Tokenize(string input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            var tokens = new List<string>();
            var buf = new StringBuilder();
            int i = 0;
            int n = input.Length;

            State state = State.Normal;
            int quoteStartPos = -1;

            while (i < n)
            {
                char ch = input[i];

                if (state == State.Normal)
                {
                    if (ch == ' ' || ch == '\t')
                    {
                        if (buf.Length > 0)
                        {
                            tokens.Add(buf.ToString());
                            buf.Clear();
                        }
                        i++;
                        continue;
                    }

                    if (ch == '"')
                    {
                        state = State.InDouble;
                        quoteStartPos = i;
                        i++;
                        continue;
                    }

                    if (ch == '\'')
                    {
                        state = State.InSingle;
                        quoteStartPos = i;
                        i++;
                        continue;
                    }

                    if (ch == '\\')
                    {
                        i++;
                        if (i < n)
                        {
                            buf.Append(input[i]);
                            i++;
                        }
                        else
                        {
                            buf.Append('\\');
                        }
                        continue;
                    }

                    buf.Append(ch);
                    i++;
                    continue;
                }
                else if (state == State.InSingle)
                {
                    if (ch == '\'')
                    {
                        state = State.Normal;
                        i++;
                        continue;
                    }
                    buf.Append(ch);
                    i++;
                    continue;
                }
                else // State.InDouble
                {
                    if (ch == '"')
                    {
                        state = State.Normal;
                        i++;
                        continue;
                    }

                    if (ch == '\\')
                    {
                        i++;
                        if (i < n)
                        {
                            buf.Append(input[i]);
                            i++;
                        }
                        else
                        {
                            buf.Append('\\');
                        }
                        continue;
                    }

                    buf.Append(ch);
                    i++;
                    continue;
                }
            }

            if (buf.Length > 0)
            {
                tokens.Add(buf.ToString());
            }

            if (state != State.Normal)
            {
                return ParseResult.Error("Отсутствует закрывающая кавычка", quoteStartPos);
            }

            return ParseResult.Ok(tokens);
        }
    }
}