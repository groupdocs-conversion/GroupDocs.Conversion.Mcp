# Codex CLI (OpenAI)

```bash
codex mcp add groupdocs-conversion -- dnx GroupDocs.Conversion.Mcp --yes
```

Or add to `~/.codex/config.toml`:

```toml
[mcp_servers.groupdocs-conversion]
command = "dnx"
args = ["GroupDocs.Conversion.Mcp", "--yes"]

[mcp_servers.groupdocs-conversion.env]
GROUPDOCS_MCP_STORAGE_PATH = "/path/to/documents"
GROUPDOCS_MCP_OUTPUT_PATH = "/path/to/documents"
GROUPDOCS_LICENSE_PATH = ""   # empty = evaluation mode; set to your GroupDocs.Total.lic to lift limits
```

Pin a version by replacing `GroupDocs.Conversion.Mcp` with `GroupDocs.Conversion.Mcp@26.9.0`.
