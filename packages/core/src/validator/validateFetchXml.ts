import type { FetchXmlDocument, ValidationIssue } from "../models/fetchXml";
import { parseFetchXml, walkElements } from "../parser/parseFetchXml";

const conditionOperatorsWithoutValue = new Set([
  "null",
  "not-null",
  "today",
  "yesterday",
  "tomorrow",
  "last-seven-days",
  "next-seven-days",
]);

export function validateFetchXml(xml: string): ValidationIssue[] {
  const issues: ValidationIssue[] = [];
  let document: FetchXmlDocument;

  try {
    document = parseFetchXml(xml);
  } catch (error) {
    return [
      {
        severity: "error",
        message: error instanceof Error ? error.message : "Invalid XML.",
        path: "/",
      },
    ];
  }

  if (document.root.name !== "fetch") {
    issues.push({
      severity: "error",
      message: "Root element must be <fetch>.",
      path: `/${document.root.name}`,
    });
  }

  let entityCount = 0;
  walkElements(document.root, (node, path) => {
    if (node.name === "entity") {
      entityCount++;
      if (!node.attributes.name) {
        issues.push({
          severity: "error",
          message: "Entity requires a name attribute.",
          path,
        });
      }
    }

    if (node.name === "attribute" && !node.attributes.name) {
      issues.push({
        severity: "error",
        message: "Attribute requires a name attribute.",
        path,
      });
    }

    if (node.name === "condition") {
      if (!node.attributes.attribute) {
        issues.push({
          severity: "error",
          message: "Condition requires an attribute.",
          path,
        });
      }
      if (!node.attributes.operator) {
        issues.push({
          severity: "error",
          message: "Condition requires an operator.",
          path,
        });
      }
      const hasNestedValue = node.children.some(
        (child) => child.type === "element" && child.name === "value",
      );
      const operator = node.attributes.operator ?? "";
      if (
        !conditionOperatorsWithoutValue.has(operator) &&
        !node.attributes.value &&
        !hasNestedValue
      ) {
        issues.push({
          severity: "warning",
          message: "Condition has no literal value.",
          path,
        });
      }
    }
  });

  if (entityCount === 0) {
    issues.push({
      severity: "error",
      message: "FetchXML must include at least one entity.",
      path: "/fetch",
    });
  }

  return issues;
}
