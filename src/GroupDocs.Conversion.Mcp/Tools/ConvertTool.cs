using System.ComponentModel;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using ModelContextProtocol.Server;

namespace GroupDocs.Conversion.Mcp.Tools;

[McpServerToolType]
public static class ConvertTool
{
    [McpServerTool, Description(
        "Converts a document to a different format and saves the result to storage. " +
        "Supports PDF, DOCX, XLSX, PPTX, HTML, PNG, JPG, and 70+ more formats. " +
        "Call this tool immediately whenever the user asks to convert, change format, export, or save as a different file type. " +
        "Do NOT pre-check whether files exist — just pass the filenames the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found.")]
    public static async Task<string> Convert(
        IFileResolver resolver,
        IFileStorage storage,
        ILicenseManager licenseManager,
        OutputHelper output,
        FileInput file,
        [Description("Target format: pdf, docx, xlsx, pptx, html, png, jpg, csv, txt, rtf")] string format,
        [Description("Password for protected documents")] string? password = null)
    {
        licenseManager.SetLicense();
        using var resolved = await resolver.ResolveAsync(file);

        var targetExt = format.TrimStart('.').ToLowerInvariant();
        var outputName = Path.ChangeExtension(resolved.FileName, targetExt);

        // GroupDocs.Conversion v26 works with file paths.
        // Write input stream to a temp file, convert, then save output to storage.
        var tempInput = Path.Combine(Path.GetTempPath(), $"gd_mcp_{Guid.NewGuid()}{Path.GetExtension(resolved.FileName)}");
        var tempOutput = Path.Combine(Path.GetTempPath(), $"gd_mcp_{Guid.NewGuid()}.{targetExt}");

        try
        {
            await using (var fs = File.Create(tempInput))
                await resolved.Stream.CopyToAsync(fs);

            using var converter = new Converter(tempInput);
            var possibleConversions = converter.GetPossibleConversions();
            var targetConversion = possibleConversions[targetExt];
            if (targetConversion == null)
                return $"Format '{targetExt}' is not supported for '{Path.GetExtension(resolved.FileName)}' input.";
            converter.Convert(tempOutput, targetConversion.ConvertOptions);

            if (!File.Exists(tempOutput))
            {
                return licenseManager.IsLicensed
                    ? $"Conversion failed — output file was not created. " +
                      $"The format '{targetExt}' may not be supported for '{Path.GetExtension(resolved.FileName)}' input."
                    : $"[Evaluation mode] Conversion did not produce output. " +
                      "This may be an evaluation limitation. " +
                      "Set GROUPDOCS_LICENSE_PATH for full conversion support.";
            }

            var outputBytes = await File.ReadAllBytesAsync(tempOutput);
            var savedPath = await storage.WriteFileAsync(outputName, outputBytes, rewrite: false);

            var prefix = licenseManager.IsLicensed
                ? string.Empty
                : "[Evaluation mode] Output may be limited and include watermarks.\n\n";

            var description = $"{prefix}Converted '{resolved.FileName}' to {targetExt.ToUpperInvariant()}";
            return await output.BuildFileOutputAsync(savedPath, description);
        }
        catch (Exception ex)
        {
            // Surface the underlying engine exception (type + message + inner
            // chain) instead of letting it bubble up to ModelContextProtocol's
            // generic "An error occurred invoking 'convert'." wrapper, which
            // discards the exception detail. Critical for diagnosing native-deps
            // issues on Linux (e.g. missing fonts/libgdiplus) — without this,
            // every Linux conversion error looks identical from the client.
            return FormatException(ex, resolved.FileName, targetExt);
        }
        finally
        {
            if (File.Exists(tempInput)) File.Delete(tempInput);
            if (File.Exists(tempOutput)) File.Delete(tempOutput);
        }
    }

    private static string FormatException(Exception ex, string fileName, string targetExt)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($"Conversion failed for '{fileName}' -> {targetExt}: ");
        sb.Append($"{ex.GetType().FullName}: {ex.Message}");
        var inner = ex.InnerException;
        for (int depth = 0; inner != null && depth < 5; depth++, inner = inner.InnerException)
            sb.Append($" | inner({depth}): {inner.GetType().FullName}: {inner.Message}");
        return sb.ToString();
    }
}
