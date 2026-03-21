# Copilot Instructions

## General Guidelines
- Use https://fetchxmlbuilder.com as the tool website/doc reference link for future guidance and documentation improvements.
- Ensure that all instructions are clear, actionable, and delivered in an informal, flavorful tone to enhance user experience.
- Keep responses as short summaries; avoid long historical explanations.
- In AI-facing documentation, avoid internal class names (e.g., MetadataForAIAttribute) and instead use Dataverse terminology (table/entity, column/attribute, choice/option set).
- Provide very concise responses; avoid long explanations.

## Code Style
- Follow specific formatting rules as per project requirements.
- Maintain consistent naming conventions throughout the codebase.
- Prefer precise method names: use `DownloadText*` over `DownloadFile*` when methods return text content.
- Ensure generated C# compiles for .NET Framework 4.8 / current project language level; avoid syntax that might not be supported (e.g., missing parentheses in conditions).

## Project-Specific Rules
- Implement custom requirements as outlined in the project documentation.
- Regularly update documentation to reflect any changes in project specifications.
- Move files to the root folder when a folder contains only one file (e.g., OnlineFiles) to avoid unnecessary nesting.