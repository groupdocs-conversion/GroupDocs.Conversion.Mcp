using System.ComponentModel;
using System.Text.Json;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using ModelContextProtocol.Server;

namespace GroupDocs.Conversion.Mcp.Tools;

[McpServerToolType]
public static class GetSupportedFormatsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool, Description(
        "Lists every target format the source document can be converted to, with primary/secondary indicators. " +
        "Call this tool immediately whenever the user asks what formats they can convert a file to, what conversions are supported, or which output types are available for a given document. " +
        "Do NOT pre-check whether files exist — just pass the filename the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found.")]
    public static async Task<string> GetSupportedFormats(
        IFileResolver resolver,
        ILicenseManager licenseManager,
        OutputHelper output,
        FileInput file)
    {
        licenseManager.SetLicense();
        using var resolved = await resolver.ResolveAsync(file);

        var tempInput = Path.Combine(Path.GetTempPath(), $"gd_mcp_{Guid.NewGuid()}{Path.GetExtension(resolved.FileName)}");
        try
        {
            await using (var fs = File.Create(tempInput))
                await resolved.Stream.CopyToAsync(fs);

            using var converter = new Converter(tempInput);
            var conversions = converter.GetPossibleConversions();

            var result = new
            {
                fileName = resolved.FileName,
                sourceExtension = conversions.Source.Extension,
                sourceFormat = conversions.Source.FileFormat,
                targets = conversions.All
                    .Select(c => new { format = c.Format.FileFormat, extension = c.Format.Extension, isPrimary = c.IsPrimary })
                    .OrderByDescending(c => c.isPrimary)
                    .ThenBy(c => c.format)
                    .ToList()
            };

            var json = JsonSerializer.Serialize(result, JsonOptions);
            return output.TruncateText(json);
        }
        finally
        {
            if (File.Exists(tempInput)) File.Delete(tempInput);
        }
    }
}
