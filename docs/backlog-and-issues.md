# Backlog & Known Issues

Running list of ideas, planned work, and known limitations for the
GroupDocs.Conversion MCP server. Grouped by topic. Terse on purpose — each line is
a ticket, not an essay. `[ ]` = open, `[x]` = shipped (kept for context).

**Current surface (26.9.0):** `convert`, `get_supported_formats`, `get_document_info`.

---

## Confirmed defects — external audit, 2026-08-16

Source: black-box test round against `ghcr.io/groupdocs-conversion/conversion-net-mcp:latest`
(26.7.2, licensed), 46 family-wide defects reported and all 46 independently reproduced with
control calls. A later validation round found **zero false positives**.

`S#` = shared core (`GroupDocs.Mcp.Core`) · `M#` = this repo · `P#` = GroupDocs.Conversion library

**Verdict: solid.** No product-library defects found. This is the one server where the
**base64 `fileContent` + `fileName` upload path is proven to work**.

### Shared core — fixed once in `GroupDocs.Mcp.Core`, lands here on the next bump

- [ ] **S1** Passing `fileName` crashes any tool — **High**. Unhandled `ArgumentException` in
      `FileResolver.ResolveAsync`; client sees only `An error occurred invoking '<tool>'`.
      *Proof:* `get_document_info {"file":{"fileName":"03_pages_text.pdf"}}` → opaque error at
      `GetDocumentInfoTool.cs:31`; `filePath` control succeeds.
- [ ] **S2** Missing files return an opaque error — **High**. Stderr held exactly what the client
      needed (`File 'nope.pdf' not found in storage. Available files: …`) and it never left the
      process.
- [ ] **S3** `isError` is set on crashes but not on real failures — **Med**.
- [ ] **S2b** The available-files listing silently stops at 20 entries — **Low**. *First measured
      here:* storage held 36 files, the listing showed exactly 20 with no truncation marker, and
      actively misled testers into thinking a fixture was missing. *Fix:* raise the cap or append
      `…and N more`. Only matters once S2 surfaces the listing — fix them together.

Nothing to do in this repo for S1–S3 beyond re-testing after the Core bump.

### MCP wrapper — this repo

- [ ] Document the `Words: 0, Lines: 0` behaviour in the `get_document_info` description —
      **Info**. Converted DOCX output reports zeros because the tool echoes the statistics the
      producing application wrote, and Aspose output leaves them empty; a hand-authored DOCX that
      was never converted reports the same. **Not a conversion defect** — but it was filed as one
      during the audit, so one sentence prevents repeat triage. **P2**

### Product library — upstream

None found.

---

## Known issues & limitations

- Conversion is licence-gated: without a licence the engine runs in evaluation mode. Read-side
  tools (`get_supported_formats`, `get_document_info`) work unlicensed.
- `get_document_info` echoes producer-written document statistics; empty values mean the producing
  application did not write them, not that extraction failed (see above).
- Note the licensing quirk carried into `GroupDocs.Total.Mcp`: Conversion is licensed there by
  setting the `GROUPDOCS_LIC_PATH` environment variable rather than a `License()` call. **There is
  no env-var equivalent for metered keys**, so the metered path must call
  `new GroupDocs.Conversion.Metered().SetMeteredKey(...)` directly.

---

## Tools & functionality

- [ ] `convert` — expose an output `fileName` parameter instead of relying on the derived name.
      **P2**
- [ ] `convert` — page-range parameter for partial conversion. **P2**
- [ ] `check_format_support(ext)` probe — cheaper for an agent than pulling the full
      `get_supported_formats` payload (74 targets for PDF alone). **P2**

## Testing & CI

- [ ] Add the two mandatory probes: the **`fileName`-only form**, and a **missing file** asserting
      the promised `Available files:` text. Today's oracle passes on the exact defect. **P1**
- [ ] Add a `channel: [dnx, docker]` axis — the current matrix is dnx-only. **P1**
- [ ] Per-tool Linux smoke test in image CI. **P1**
- [ ] Regression test for the 20-entry listing cap once S2/S2b land. **P2**
- [ ] Not covered today: password-protected conversion; content fidelity is verified via page
      count and format only, not visually. **P2**
- [ ] macOS integration leg hangs (family-wide, ~7 products) — `timeout-minutes: 20` is committed
      locally but unpushed here. Push it, and stream the `dnx` child's stderr to an uploaded file
      so the hang can finally be diagnosed. **P1**

## Documentation & discoverability

- [ ] Licensing section covering the metered option once it ships. **P1**
- [ ] Document that base64 input is proven working here — it is the recommended workaround for
      files outside storage. **P2**
- [ ] Refresh the MCP Registry description when the tool set changes.

## Platform & infra (longer-term)

- [ ] Metered licensing (`GROUPDOCS_METERED_PUBLIC_KEY` / `_PRIVATE_KEY`) via
      `GroupDocs.Mcp.Core`, plus the `get_license_status` tool. **P1**
- [ ] HTTP/SSE transport for shared/team deploys (stdio stays default). **P2**
- [ ] Remote storage (URL / S3) via `GroupDocs.Mcp.Core`. **P2**

---

*Evidence: `TEMP_ThirdPartyAnalysis/conversion.md` (per-product findings),
`ALL-PRODUCTS-REPORT.md` (10-product sweep), `VALIDATION-REPORT.md` (why the green suites miss
these). Conventions: any behaviour change ships with a `changelog/NNN-*.md` entry and a CalVer
bump. Integration tests target the published NuGet via `dnx`, so new-tool tests only pass once the
matching version is live.*
