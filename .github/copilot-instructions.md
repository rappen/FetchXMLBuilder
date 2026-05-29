# Copilot Instructions

This repository is the TypeScript web port of FetchXML Builder.

- Treat `apps/web`, `packages/core`, and `packages/dataverse` as the active source tree.
- Keep FetchXML parsing, formatting, validation, query modeling, and converters in `packages/core`.
- Keep Dataverse connectivity and API integration code in `packages/dataverse`.
- Keep React UI components, styling, and workbench state in `apps/web`.
- Prefer concise, practical changes over broad rewrites.
- Preserve package boundaries unless a feature clearly needs a shared API.
- Add or update Vitest coverage when changing shared FetchXML behavior.
- Do not reintroduce WinForms, XrmToolBox, NuGet, or submodule dependencies.
- Do not commit generated output such as `dist`, `node_modules`, `bin`, `obj`, or `.tsbuildinfo`.
