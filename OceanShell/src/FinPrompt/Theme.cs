namespace FinPrompt;

/// <summary>
/// Внешний вид для приглашения 
/// </summary>

public sealed class Theme
{
    public bool UseColors { get; set; } = false;          // на будущее
    public string PromptChar { get; set; } = "%";          // символ в конце
    public string ConsoleName { get; set; } = "GawrGura";  // бренд консоли
    public bool ShowConsoleName { get; set; } = true;      // показывать ли [GawrGura]
    public bool ShowOnlyLastPathSegment { get; set; } = false; // показывать только последний сегмент пути
}
