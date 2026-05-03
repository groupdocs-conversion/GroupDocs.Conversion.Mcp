# GroupDocs.Conversion MCP Server

MCP server that exposes [GroupDocs.Conversion](https://products.groupdocs.com/conversion) as AI-callable tools
for Claude, Cursor, GitHub Copilot, and other MCP agents.

## Installation

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

**Run directly with `dnx` (recommended — no install step):**

```bash
dnx GroupDocs.Conversion.Mcp --yes
```

Pulls the latest stable release on every invocation. To pin to a specific
version (recommended for shared configs and CI), append `@<version>`:

```bash
dnx GroupDocs.Conversion.Mcp@26.5.1 --yes
```

**Or install as a global dotnet tool:**

```bash
dotnet tool install -g GroupDocs.Conversion.Mcp
groupdocs-conversion-mcp
```

**Or run via Docker:**

```bash
docker run --rm -i \
  -v $(pwd)/documents:/data \
  ghcr.io/groupdocs-conversion/conversion-net-mcp:latest
```

## Available MCP Tools

| Tool | Description |
|---|---|
| `Convert` | Converts a document to a different format (PDF, DOCX, XLSX, PPTX, HTML, PNG, JPG, and 70+ more) and saves the result to storage |
| `GetSupportedFormats` | Lists every target format the source document can be converted to, with primary/secondary indicators |
| `GetDocumentInfo` | Returns file type, page count, and basic properties (author, title, dates, password-protected) for a source document |

## Configuration

| Variable | Description | Default |
|---|---|---|
| `GROUPDOCS_MCP_STORAGE_PATH` | Base folder for input and output files | current directory |
| `GROUPDOCS_MCP_OUTPUT_PATH` | *(Optional)* separate folder for output files | `GROUPDOCS_MCP_STORAGE_PATH` |
| `GROUPDOCS_LICENSE_PATH` | Path to GroupDocs license file | (evaluation mode) |

## Usage with Claude Desktop

```json
{
  "mcpServers": {
    "groupdocs-conversion": {
      "type": "stdio",
      "command": "dnx",
      "args": ["GroupDocs.Conversion.Mcp", "--yes"],
      "env": {
        "GROUPDOCS_MCP_STORAGE_PATH": "/path/to/documents"
      }
    }
  }
}
```

> To pin to a specific version, replace `"GroupDocs.Conversion.Mcp"` with
> `"GroupDocs.Conversion.Mcp@26.5.1"` in `args`. Pinning is recommended for
> shared / committed configs to avoid surprise upgrades.

## Usage with VS Code / GitHub Copilot

NuGet.org generates a ready-to-use `mcp.json` snippet on the [package page](https://www.nuget.org/packages/GroupDocs.Conversion.Mcp).
Copy it directly into your `.vscode/mcp.json`.

Alternatively, add manually to `.vscode/mcp.json`:

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "storage_path",
      "description": "Base folder for input and output files.",
      "password": false
    }
  ],
  "servers": {
    "groupdocs-conversion": {
      "type": "stdio",
      "command": "dnx",
      "args": ["GroupDocs.Conversion.Mcp", "--yes"],
      "env": {
        "GROUPDOCS_MCP_STORAGE_PATH": "${input:storage_path}"
      }
    }
  }
}
```

> Same pinning rule as above — swap `"GroupDocs.Conversion.Mcp"` for
> `"GroupDocs.Conversion.Mcp@26.5.1"` to lock to a specific release.

## Usage with Docker Compose

```bash
cd docker
docker compose up
```

Edit `docker/docker-compose.yml` to point volumes at your local documents folder.

## Documentation & guides

Step-by-step deployment guides and a published-package integration test suite
live in the companion repo
[**GroupDocs.Conversion.Mcp.Tests**](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests):

- [Install from NuGet](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/01-install-from-nuget.md) — `dnx`, global tool, pinned vs always-latest
- [Run via Docker](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/02-run-via-docker.md)
- [Verify on the MCP registry](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/03-verify-mcp-registry.md)
- [Use with Claude Desktop](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/04-use-with-claude-desktop.md)
- [Use with VS Code / GitHub Copilot](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/05-use-with-vscode-copilot.md)
- [Run the integration tests](https://github.com/groupdocs-conversion/GroupDocs.Conversion.Mcp.Tests/blob/master/how-to/06-run-integration-tests.md)

That repo also exercises every advertised tool against the **published** NuGet
artifact on Linux, macOS, and Windows in CI — so the snippets above are
verified end-to-end on every release.

## License

MIT — see [LICENSE](LICENSE)

<!-- mcp-name: io.github.groupdocs-conversion/groupdocs-conversion-mcp -->
