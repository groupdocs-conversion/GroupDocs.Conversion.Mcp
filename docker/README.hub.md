# GroupDocs.Conversion MCP Server

Convert PDF, Word, Excel, PowerPoint, HTML, images, and 70+ document formats from Claude,
Cursor, GitHub Copilot, and any other MCP agent — **locally, your files never leave the container**.

## Quick start

```bash
docker run --rm -i \
  -v $(pwd)/documents:/data \
  groupdocs/conversion-net-mcp:latest
```

## Use with Claude Desktop

```json
{
  "mcpServers": {
    "groupdocs-conversion": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "-v", "/path/to/documents:/data", "groupdocs/conversion-net-mcp:latest"]
    }
  }
}
```

## Tools

- **Convert** — convert a document to another format (70+ supported) and save it to storage
- **GetSupportedFormats** — list every target format a source document can be converted to
- **GetDocumentInfo** — file type, page count, and basic properties for a source document

## Tags & environment

- Tags: `latest` + an immutable version tag per release matching NuGet (e.g. `26.7.0`).
  Platforms: `linux/amd64`, `linux/arm64`. Also on GHCR: `ghcr.io/groupdocs-conversion/conversion-net-mcp`.
- `GROUPDOCS_MCP_STORAGE_PATH` (default `/data`), `GROUPDOCS_MCP_OUTPUT_PATH` (optional),
  `GROUPDOCS_LICENSE_PATH` — mount your license and point at it to leave evaluation mode
  (watermarked output, 15-document cap per process).

Full docs, one-click installs for other clients, and licensing details:
**https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp**
