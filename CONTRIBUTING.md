# Contributing

This project is now focused on the TypeScript web port of FetchXML Builder.

## Development

- Keep shared FetchXML behavior in `packages/core`.
- Keep Dataverse-specific service code in `packages/dataverse`.
- Keep browser UI behavior in `apps/web`.
- Prefer small, focused changes that preserve the current workspace boundaries.
- Add tests around parser, formatter, validator, converter, and query-model behavior when changing shared logic.

## Checks

Before opening a PR or handing off a change, run:

```bash
npm test
npm run build
```

Use Biome for formatting and lint checks:

```bash
npm run format
npm run lint
```

## Style

- Prefer readable TypeScript over clever abstractions.
- Keep React components focused on UI state and interaction.
- Avoid putting Dataverse transport details directly inside UI components.
- Do not commit generated `dist`, `node_modules`, `.tsbuildinfo`, `bin`, or `obj` output.
