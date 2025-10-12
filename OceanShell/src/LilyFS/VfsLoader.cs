using System.Text;
using System.Text.Json;

namespace LilyFS
{
    public static class VfsLoader
    {
        public static VfsDir LoadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл VFS не найден: {path}");

            var json = File.ReadAllText(path);
            try
            {
                using var doc = JsonDocument.Parse(json);
                return ParseNode(doc.RootElement) as VfsDir
                       ?? throw new InvalidDataException("Корень VFS должен быть директорией");
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Ошибка формата JSON: {ex.Message}");
            }
        }

        private static VfsNode ParseNode(JsonElement el)
        {
            var type = el.GetProperty("type").GetString();
            var name = el.GetProperty("name").GetString() ?? "";

            if (type == "file")
            {
                var file = new VfsFile { Name = name };
                if (el.TryGetProperty("content", out var c))
                {
                    var base64 = c.GetString() ?? "";
                    file.Content = Convert.FromBase64String(base64);
                }
                return file;
            }

            if (type == "dir")
            {
                var dir = new VfsDir { Name = name };
                if (el.TryGetProperty("children", out var ch))
                {
                    foreach (var child in ch.EnumerateArray())
                    {
                        var node = ParseNode(child);
                        node.Parent = dir;
                        dir.Children.Add(node);
                    }
                }
                return dir;
            }

            throw new InvalidDataException($"Неизвестный тип узла: {type}");
        }
    }
}