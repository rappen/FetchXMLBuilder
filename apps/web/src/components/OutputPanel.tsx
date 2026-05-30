import {
  formatFetchXml,
  toCSharpFetchXml,
  toJavaScriptFetchXml,
  toODataUrl,
  toPowerAutomateParameters,
  validateFetchXml,
} from "@fetchxmlbuilder/core";
import {
  AlertCircle,
  Braces,
  Code2,
  Copy,
  FileJson2,
  Link,
  ListChecks,
  Wand2,
} from "lucide-react";
import type { OutputTab } from "../store/workbenchStore";

interface OutputPanelProps {
  fetchXml: string;
  outputTab: OutputTab;
  setOutputTab: (tab: OutputTab) => void;
}

export const outputTabs: Array<{
  id: OutputTab;
  label: string;
  icon: typeof Wand2;
}> = [
  { id: "powerAutomate", label: "Power Automate", icon: Wand2 },
  { id: "odata", label: "OData", icon: Link },
  { id: "csharp", label: "C#", icon: Code2 },
  { id: "javascript", label: "JavaScript", icon: Braces },
  { id: "validation", label: "Validation", icon: ListChecks },
];

export function OutputPanel({
  fetchXml,
  outputTab,
  setOutputTab,
}: OutputPanelProps) {
  const output = getOutput(fetchXml, outputTab);

  return (
    <section className="panel output-panel" aria-label="Converter output">
      <div className="panel-toolbar">
        <div className="segmented" role="tablist" aria-label="Outputs">
          {outputTabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <button
                className={tab.id === outputTab ? "active" : ""}
                key={tab.id}
                type="button"
                onClick={() => setOutputTab(tab.id)}
                title={tab.label}
              >
                <Icon size={16} />
                <span>{tab.label}</span>
              </button>
            );
          })}
        </div>
        <button
          className="icon-button"
          type="button"
          title="Copy"
          onClick={() => navigator.clipboard.writeText(output.text)}
        >
          <Copy size={17} />
        </button>
      </div>

      {output.kind === "parameters" ? (
        <div className="parameter-layout">
          <pre>{output.text}</pre>
          <div className="manifest">
            {output.parameters.map((parameter) => (
              <div className="manifest-row" key={parameter.name}>
                <FileJson2 size={15} />
                <span>{parameter.name}</span>
                <small>{parameter.inferredType}</small>
              </div>
            ))}
          </div>
        </div>
      ) : output.kind === "issues" ? (
        <div className="issues">
          {output.issues.length === 0 ? (
            <div className="empty-state">No validation issues</div>
          ) : (
            output.issues.map((issue) => (
              <div
                className={`issue ${issue.severity}`}
                key={`${issue.path}-${issue.message}`}
              >
                <AlertCircle size={16} />
                <span>{issue.message}</span>
                <small>{issue.path}</small>
              </div>
            ))
          )}
        </div>
      ) : (
        <pre>{output.text}</pre>
      )}
    </section>
  );
}

export function getOutput(fetchXml: string, tab: OutputTab) {
  try {
    if (tab === "powerAutomate") {
      const converted = toPowerAutomateParameters(fetchXml);
      return {
        kind: "parameters" as const,
        text: converted.xml,
        parameters: converted.parameters,
      };
    }
    if (tab === "odata")
      return { kind: "text" as const, text: toODataUrl(fetchXml) };
    if (tab === "csharp")
      return { kind: "text" as const, text: toCSharpFetchXml(fetchXml) };
    if (tab === "javascript")
      return { kind: "text" as const, text: toJavaScriptFetchXml(fetchXml) };
    return {
      kind: "issues" as const,
      issues: validateFetchXml(fetchXml),
      text: "",
    };
  } catch (error) {
    return {
      kind: "issues" as const,
      text: "",
      issues: [
        {
          severity: "error" as const,
          message:
            error instanceof Error
              ? error.message
              : "Unable to process FetchXML.",
          path: "/",
        },
      ],
    };
  }
}

export function getFormattedXml(fetchXml: string) {
  return formatFetchXml(fetchXml);
}
