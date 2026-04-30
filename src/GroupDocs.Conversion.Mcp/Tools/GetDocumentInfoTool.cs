using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using ModelContextProtocol.Server;

namespace GroupDocs.Conversion.Mcp.Tools;

[McpServerToolType]
public static class GetDocumentInfoTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    [McpServerTool, Description(
        "Returns file type, page count, and basic properties (author, title, dates, password-protected) for a source document. " +
        "Call this tool immediately whenever the user asks to inspect a document, get document info, check file type, page count, or properties. " +
        "Do NOT pre-check whether files exist — just pass the filename the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found.")]
    public static async Task<string> GetDocumentInfo(
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
            var info = converter.GetDocumentInfo();

            // info is an IDocumentInfo at compile time but a concrete subtype at runtime
            // (PdfDocumentInfo, WordProcessingDocumentInfo, PresentationDocumentInfo, ...).
            // Serializing against the runtime type picks up the subtype-specific properties
            // (Author, Title, CreationDate, PagesCount, Width, Height, etc.).
            var serialized = JsonSerializer.Serialize(info, info.GetType(), JsonOptions);

            var result = new
            {
                fileName = resolved.FileName,
                infoType = info.GetType().Name,
                info = JsonSerializer.Deserialize<JsonElement>(serialized)
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
