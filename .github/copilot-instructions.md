# Copilot Instructions

## General Guidelines

- Use https://fetchxmlbuilder.com as the FetchXML Builder website and documentation reference for user-facing guidance and documentation improvements.
- Keep responses concise, actionable, easy to scan, and free from unnecessary historical context.
- Prefer simple, low-bloat solutions. Avoid new wrapper types or abstractions unless they provide clear value.
- Favor user-helpful, configurable designs. Put configuration at the appropriate abstraction level and avoid configuration that only mirrors fixed implementation details.
- Prefer one configuration value with optional placeholders over separate template and fallback values when they serve the same purpose. Replacing an absent placeholder must be safe.
- Preserve backward compatibility for older released FetchXML Builder versions. The platform cannot force updates, so online settings, configuration schemas, protocols, and services must safely support clients that do not recognize new fields or features.
- Keep existing online-settings model entries when adding dynamic model discovery. Older releases depend on the static model lists, and newer releases can use them as known-model overrides or fallbacks.
- Do not create or save task-specific memories. Only record durable, broadly applicable preferences or instructions that are explicitly requested or clearly stated.

## AI Chat and Provider Integration

- Keep provider-specific protocols and adapters isolated from the AI Chat core. Minimize dependencies.
- Keep GPT and Claude routing under the same Foundry provider; distinguish them by model rather than creating separate providers.
- In AI settings, `Free` means FXB-hosted/shared access that is free to the user. It does not refer to a vendor's own free tier. Dynamic discovery excludes only FXB-free providers.
- Show token/price information for selectable AI models and provide an opt-in checkbox for preview or experimental models.
- For dynamic model discovery, use the configured static model list as a compatibility catalog and as an exact model-URL override.
- Keep provider URLs broad and user-facing, such as `https://anthropic.com` or `https://openai.com`.
- Keep model documentation URLs specific to each model when available.
- Keep model documentation root URLs and optional `{model}` URL patterns in provider configuration. Do not hard-code per-model documentation mappings in code.
- Keep model-discovery API endpoints configurable when they can change independently of discovery behavior. Keep genuinely protocol-specific request behavior in the implementation.

## AI Prompts and Tools

- Use Dataverse terminology in AI-facing text: table/entity, column/attribute, and choice/option set. Do not expose internal class names.
- Use short, stable, behavior-focused AI tool names and descriptions. Do not couple prompts or documentation to internal C# method names.
- Allow metadata tools to resolve multiple tables, columns, or relationships in one call when the AI already knows it needs several. Continue to support repeated calls when needed.
- For stateless metadata matching, prioritize the user's configured logical-name/publisher prefix from User Flavors, such as `xyz_` or `new_`. Use neutral prefixes in examples; do not use `rapp_`.
- Use online-file-driven prompt templates with generic placeholder replacement. Extract prompt/template-loading logic when it becomes large enough to clutter its owner.
- Prefer a clean templating API over verbose `KeyValuePair` construction when safety remains clear.
- Type-based metadata matching must work generally across Dataverse types, not only date-like types.

## Code Style

- Follow existing project conventions and keep naming consistent.
- Prefer short, human-friendly names. Use `Match` rather than `Resolve` for metadata behavior.
- Use more technical AI communication names when human-friendly names would hide an important behavioral distinction.
- Name methods precisely: use `DownloadText*` for methods returning text, not `DownloadFile*`.
- Keep explicit sync/async naming symmetry. For example, pair `PromptSync` with `PromptAsync`; do not use an ambiguous plain `Prompt`.
- Generate C# compatible with .NET Framework 4.8 and the project's current language version.
- Always use normal `if (...)` condition parentheses.
- Avoid init-only properties; use setters or constructors compatible with this codebase.
- Put event wiring for designer-owned WinForms controls in the `.Designer.cs` file, not the form constructor.
- For WinForms layout changes, minimize flicker and update size, padding, or layout values only when they actually change.

## Project Maintenance

- Keep project documentation aligned with implemented behavior and current specifications.
- Move a file to its parent/root folder when its folder would otherwise contain only that one file.