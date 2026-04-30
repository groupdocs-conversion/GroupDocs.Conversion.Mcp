---
id: 001
date: 2026-04-30
version: 26.5.0
type: feature
---

# Initial public release of GroupDocs.Conversion MCP Server

## What changed
- NuGet package `GroupDocs.Conversion.Mcp` published with `McpServer` package type.
- Three MCP tools exposed:
  - `Convert` — convert a document to a different format (PDF, DOCX, XLSX, PPTX, HTML, PNG, JPG, and 70+ more) and save the result to storage.
  - `GetSupportedFormats` — list every target format the source document can be converted to, with primary/secondary indicators.
  - `GetDocumentInfo` — return file type, page count, and basic properties (author, title, dates, password-protected) for a source document.
- Installable via `dnx GroupDocs.Conversion.Mcp@26.5.0 --yes` (.NET 10 SDK required) or `dotnet tool install -g`.
- Docker image published to `ghcr.io/groupdocs-conversion/conversion-net-mcp` and `docker.io/groupdocs/conversion-net-mcp`.
- Environment variables: `GROUPDOCS_MCP_STORAGE_PATH`, optional `GROUPDOCS_MCP_OUTPUT_PATH`, `GROUPDOCS_LICENSE_PATH`.
- Linux native graphics deps wired up: `SkiaSharp.NativeAssets.Linux.NoDependencies` (3.119.2) is referenced because `GroupDocs.Conversion` uses SkiaSharp internally; `libgdiplus` + `libfontconfig1` are installed in the Docker image and the `System.Drawing.EnableUnixSupport` runtime flag is set because Conversion's image-format paths still call `System.Drawing.Common`.

## Why
Second product MCP server in the GroupDocs MCP framework (after Metadata). Exposes
GroupDocs.Conversion for .NET as AI-callable tools for Claude, Cursor,
VS Code / GitHub Copilot, and other MCP-compatible agents.

## Migration / impact
First release — no migration required.
