namespace SabaSaba
{
    /// <summary>
    /// Интерфейс для всех встроенных команд.
    /// Возвращает код завершения (0 = успех).
    /// </summary>
    public interface ICommand
    {
        int Execute(IReadOnlyList<string> args);
    }
}