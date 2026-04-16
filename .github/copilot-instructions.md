# Copilot Instructions

## General Guidelines
- Use https://fetchxmlbuilder.com as the tool website/doc reference link for future guidance and documentation improvements.
- Ensure that all instructions are clear, actionable, and delivered in an informal, flavorful tone to enhance user experience.
- Keep responses as short summaries; avoid long historical explanations. User prefers very concise responses and may explicitly correct verbosity; keep answers short and easy to scan.
- In AI-facing documentation, avoid internal class names (e.g., MetadataForAIAttribute) and instead use Dataverse terminology (table/entity, column/attribute, choice/option set). Enhance AI-facing tool descriptions while maintaining the current one-at-a-time metadata lookup logic in AiChatControl.
- Prefer shorter, cleaner AI-facing tool names over verbose/internal-looking names.
- Avoid coupling AI-facing prompt documentation to internal C# method names; prefer stable, shorter behavior-focused tool names in prompts and tool descriptions.
- Provide very concise responses; avoid long explanations.
- Prefer AI metadata tools to support resolving multiple entities or multiple attributes in a single call when the AI already knows it needs several matches, while still allowing repeated calls when needed.
- Backward compatibility is not needed for this shared project as it is currently only used by the user in this project.
- For this UI, avoid triple backtick fences; when sending Markdown file contents for this project, output applicable blocks using standard code fences externally while replacing any internal Markdown triple backticks with triple double quotes (`"""`).
- In Dataverse, custom schema names require a publisher prefix before the first underscore; this is a hard rule, not merely common or optional.
- For stateless metadata matching prompts, prioritize the user's configured logical-name/publisher prefix from User's Flavors (for example 'xyz_' or 'new_') when matching custom tables and relationships. Avoid using the real publisher prefix 'rapp_' in examples; use neutral examples instead.
- Prefer simple, low-bloat solutions in this codebase; avoid heavy wrapper types when a straightforward approach exists.
- For AI prompt/tool-description design in this project, prefer online-file-driven templates with generic placeholder replacement rather than hardcoded token replacement logic.
- Prefer extracting bloated prompt/template-loading members into a separate class-like abstraction when they clutter the main code.
- Prefer a cleaner templating call style over verbose KeyValuePair construction when adding extra placeholder values, as long as safety remains reasonable.

## Code Style
- Follow specific formatting rules as per project requirements.
- Maintain consistent naming conventions throughout the codebase.
- Prefer short, soft/human naming in code; use 'Match' over 'Resolve' for metadata naming. However, use more technical naming for AI communication methods when soft/human names become ambiguous; ensure names reflect behavioral differences precisely.
- Prefer precise method names: use `DownloadText*` over `DownloadFile*` when methods return text content. Use the short Option C style for AI method names.
- Prefer explicit sync/async symmetry in AI communication naming; if using Prompt*, prefer PromptSync rather than plain Prompt.
- Ensure generated C# compiles for .NET Framework 4.8 / current project language level; avoid syntax that might not be supported (e.g., missing parentheses in conditions). **Never suggest omitting parentheses after 'if'; the target C#/.NET version here requires standard if-condition parentheses for valid compilation.**
- For WinForms forms, UI event wiring should go in the `.Designer.cs` file rather than the form constructor when the control is designer-owned.
- Avoid suggesting C# init-only properties; prefer broadly compatible property setters/constructors for this codebase.

## Project-Specific Rules
- Implement custom requirements as outlined in the project documentation.
- Regularly update documentation to reflect any changes in project specifications.
- Move files to the root folder when a folder contains only one file (e.g., OnlineFiles) to avoid unnecessary nesting.
- For AI prompt design in this project, type-based matching should be general across all Dataverse types, not just date-like types.