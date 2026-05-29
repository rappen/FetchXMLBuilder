import { readFetchQueryModel } from "@fetchxmlbuilder/core";
import { Copy, Download } from "lucide-react";
import { useMemo } from "react";
import { makeMockRows } from "../data/mockMetadata";

interface ResultGridProps {
  fetchXml: string;
  rows: Record<string, unknown>[];
  onRowsChange: (rows: Record<string, unknown>[]) => void;
}

export function ResultGrid({ fetchXml, rows, onRowsChange }: ResultGridProps) {
  const model = useMemo(() => safeReadModel(fetchXml), [fetchXml]);
  const columns = Object.keys(rows[0] ?? {});

  function execute() {
    const attributes = model.attributes.map((attribute) => attribute.name);
    onRowsChange(makeMockRows(model.entity, attributes));
  }

  return (
    <section className="panel side-panel results-panel" aria-label="Results">
      <div className="panel-heading">
        <h2>Results</h2>
        <div className="button-row">
          <button type="button" title="Run" onClick={execute}>
            Run
          </button>
          <button
            className="icon-button"
            type="button"
            title="Copy JSON"
            onClick={() =>
              navigator.clipboard.writeText(JSON.stringify(rows, null, 2))
            }
          >
            <Copy size={16} />
          </button>
          <button
            className="icon-button"
            type="button"
            title="Export CSV"
            onClick={() => download("fetchxml-results.csv", toCsv(rows))}
          >
            <Download size={16} />
          </button>
        </div>
      </div>

      {rows.length === 0 ? (
        <div className="empty-state">No rows</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                {columns.map((column) => (
                  <th key={column}>{column}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {rows.map((row, index) => (
                <tr key={`row-${index + 1}`}>
                  {columns.map((column) => (
                    <td key={column}>{String(row[column] ?? "")}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

function safeReadModel(fetchXml: string) {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return { entity: "account", attributes: [{ name: "name" }] };
  }
}

function toCsv(rows: Record<string, unknown>[]) {
  if (rows.length === 0) return "";
  const columns = Object.keys(rows[0] ?? {});
  return [
    columns.join(","),
    ...rows.map((row) =>
      columns.map((column) => csvValue(String(row[column] ?? ""))).join(","),
    ),
  ].join("\n");
}

function csvValue(value: string) {
  return /[",\n]/.test(value) ? `"${value.replace(/"/g, '""')}"` : value;
}

function download(fileName: string, content: string) {
  const blob = new Blob([content], { type: "text/csv" });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = fileName;
  anchor.click();
  URL.revokeObjectURL(url);
}
