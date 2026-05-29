# FetchXML Builder Web Port

This repository is the web port of FetchXML Builder for Microsoft Dataverse and the Power Platform.

The active codebase is a TypeScript workspace:

- `apps/web`: React/Vite workbench UI
- `packages/core`: FetchXML parsing, formatting, validation, query modeling, and converters
- `packages/dataverse`: Dataverse integration layer

## Getting Started

Install dependencies:

```bash
npm install
```

Run the web app locally:

```bash
npm run dev
```

Run tests:

```bash
npm test
```

Build all workspaces:

```bash
npm run build
```

## Development Focus

The goal of this port is to make FetchXML Builder available as a modern web application while keeping FetchXML-specific logic reusable across packages. Legacy XrmToolBox and WinForms code has been removed from the active tree so new work can focus on the web experience, Dataverse connectivity, and shared FetchXML functionality.
