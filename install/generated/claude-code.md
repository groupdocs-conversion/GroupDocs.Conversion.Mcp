# Claude Code

```bash
claude mcp add groupdocs-conversion -- dnx GroupDocs.Conversion.Mcp --yes
```

With storage folder and license:

```bash
claude mcp add groupdocs-conversion -e GROUPDOCS_MCP_STORAGE_PATH=/path/to/documents -e GROUPDOCS_LICENSE_PATH=/path/to/GroupDocs.Total.lic -- dnx GroupDocs.Conversion.Mcp --yes
```

Pin a version by replacing `GroupDocs.Conversion.Mcp` with `GroupDocs.Conversion.Mcp@26.9.0`.
