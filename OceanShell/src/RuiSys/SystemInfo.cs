using System;
using System.IO;

namespace RuiSys;

/// <summary>
/// Обертки над системной средой: пользователь, хост, пути.
/// </summary>
public static class SystemInfo
{
    /// <summary>
    /// Достает имя пользователя
    /// </summary>
    public static string GetUser()
    {
        return Environment.UserName;
    }
    /// <summary>
    /// Достает имя машины(локальный без днс)
    /// </summary>
    public static string GetHost()
    {
        return Environment.MachineName;
    }
    /// <summary>
    /// Метод вернет домашнюю директорию
    /// </summary>
    public static string GetHomeDirectory()
    {
        // через api.net берем путь до папки /Users/asdwertinc
        string homedir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return homedir;
    }
    public static string GetCurrentWorkingDirectory() // Аналог PWD
    {
        return Directory.GetCurrentDirectory();
    }
    public static string SetCurrentWorkingDirectory(string path) // Аналог PWD
    {
        Directory.SetCurrentDirectory(path);
        return Directory.GetCurrentDirectory();
    }
}

