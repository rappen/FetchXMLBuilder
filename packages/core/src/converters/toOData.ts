import type { XmlElementNode } from "../models/fetchXml";
import { parseFetchXml, walkElements } from "../parser/parseFetchXml";

const operatorMap: Record<string, string> = {
  eq: "eq",
  ne: "ne",
  neq: "ne",
  lt: "lt",
  le: "le",
  gt: "gt",
  ge: "ge",
};

export function toODataUrl(xml: string) {
  const document = parseFetchXml(xml);
  const entity = document.root.children.find(
    (node) => node.type === "element" && node.name === "entity",
  );
  if (!entity || entity.type !== "element" || !entity.attributes.name) {
    throw new Error("OData conversion requires a root entity name.");
  }

  const selects: string[] = [];
  const filters: string[] = [];
  const orders: string[] = [];

  for (const child of entity.children) {
    if (child.type !== "element") continue;
    if (child.name === "attribute" && child.attributes.name) {
      selects.push(child.attributes.name);
    }
    if (child.name === "order" && child.attributes.attribute) {
      orders.push(
        `${child.attributes.attribute} ${child.attributes.descending === "true" ? "desc" : "asc"}`,
      );
    }
    if (child.name === "filter") {
      filters.push(...filterToOData(child));
    }
  }

  const query = new URLSearchParams();
  if (selects.length) query.set("$select", selects.join(","));
  if (filters.length) query.set("$filter", filters.join(" and "));
  if (orders.length) query.set("$orderby", orders.join(","));

  const qs = Array.from(query.entries())
    .map(([key, value]) => `${key}=${encodeODataQueryValue(value)}`)
    .join("&");
  return `/${entity.attributes.name}${qs ? `?${qs}` : ""}`;
}

function filterToOData(filter: XmlElementNode) {
  const conditions: string[] = [];
  walkElements(filter, (node) => {
    if (
      node.name === "condition" &&
      node.attributes.attribute &&
      node.attributes.operator
    ) {
      const condition = conditionToOData(
        node.attributes.attribute,
        node.attributes.operator,
        node.attributes.value,
      );
      if (condition) conditions.push(condition);
    }
  });
  return conditions;
}

function conditionToOData(attribute: string, operator: string, value?: string) {
  if (operator === "null") return `${attribute} eq null`;
  if (operator === "not-null") return `${attribute} ne null`;
  if (!value) return undefined;
  if (operator === "like")
    return `contains(${attribute}, ${formatValue(value.replace(/%/g, ""))})`;
  const mapped = operatorMap[operator];
  if (!mapped) return undefined;
  return `${attribute} ${mapped} ${formatValue(value)}`;
}

function formatValue(value: string) {
  if (/^(true|false)$/i.test(value)) return value.toLowerCase();
  if (/^-?\d+(\.\d+)?$/.test(value)) return value;
  if (
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
      value,
    )
  )
    return value;
  return `'${value.replace(/'/g, "''")}'`;
}

function encodeODataQueryValue(value: string) {
  return encodeURI(value).replace(/#/g, "%23").replace(/&/g, "%26");
}
