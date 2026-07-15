using System.Text;

namespace GroupDocs.Conversion.Mcp.Tools;

// Shared descriptive-error formatter for the tool surface. Engine failures are
// surfaced as text (not ModelContextProtocol's opaque "An error occurred
// invoking '<tool>'") so AI agents and integration tests can read the cause —
// critical for diagnosing native-deps issues on Linux (missing fonts /
// libgdiplus). The text always starts with "<op> failed for '<file>'[ <suffix>]: …";
// integration tests match that prefix.
internal static class ToolError
{
    public static string Format(string op, string file, Exception ex, string? subjectSuffix = null)
    {
        var suffix = string.IsNullOrEmpty(subjectSuffix) ? string.Empty : $" {subjectSuffix}";
        var sb = new StringBuilder();
        sb.Append($"{op} failed for '{file}'{suffix}: {ex.GetType().FullName}: {ex.Message}");
        var inner = ex.InnerException;
        for (int d = 0; inner != null && d < 5; d++, inner = inner.InnerException)
        {
            sb.Append($" | inner({d}): {inner.GetType().FullName}: {inner.Message}");
        }
        return sb.ToString();
    }
}
