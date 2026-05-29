import { formatFetchXml } from "../formatter/formatFetchXml";
import type {
  FetchConditionSelection,
  FetchLinkEntitySelection,
  FetchQueryModel,
  XmlElementNode,
} from "../models/fetchXml";
import { escapeXml, parseFetchXml } from "../parser/parseFetchXml";

export const emptyFetchQueryModel: FetchQueryModel = {
  entity: "account",
  top: "50",
  attributes: [{ name: "name" }],
  conditions: [],
  orders: [],
  links: [],
};

export function readFetchQueryModel(xml: string): FetchQueryModel {
  const document = parseFetchXml(xml);
  const entity = document.root.children.find(
    (child) => child.type === "element" && child.name === "entity",
  );
  if (!entity || entity.type !== "element") return emptyFetchQueryModel;

  const attributes = directChildren(entity, "attribute")
    .map((node) => node.attributes.name)
    .flatMap((name) => (name ? [{ name }] : []));

  const orders = directChildren(entity, "order")
    .filter((node) => node.attributes.attribute)
    .map((node) => ({
      attribute: node.attributes.attribute ?? "",
      descending: node.attributes.descending === "true",
    }));

  const conditions = directChildren(entity, "filter").flatMap((filter) =>
    directChildren(filter, "condition")
      .filter((condition) => condition.attributes.attribute)
      .map((condition, index) => conditionFromNode(condition, index)),
  );

  const links = directChildren(entity, "link-entity").map((node, index) =>
    linkFromNode(node, index),
  );

  return {
    entity: entity.attributes.name ?? "account",
    top: document.root.attributes.top ?? "",
    attributes,
    conditions,
    orders,
    links,
  };
}

export function writeFetchQueryModel(model: FetchQueryModel): string {
  const lines = [`<fetch${model.top ? ` top="${escapeXml(model.top)}"` : ""}>`];
  lines.push(`  <entity name="${escapeXml(model.entity)}">`);

  for (const attribute of model.attributes) {
    if (attribute.name) {
      lines.push(`    <attribute name="${escapeXml(attribute.name)}" />`);
    }
  }

  for (const order of model.orders) {
    if (order.attribute) {
      lines.push(
        `    <order attribute="${escapeXml(order.attribute)}"${order.descending ? ' descending="true"' : ""} />`,
      );
    }
  }

  if (model.conditions.length > 0) {
    lines.push('    <filter type="and">');
    for (const condition of model.conditions) {
      if (!condition.attribute || !condition.operator) continue;
      const value = condition.value
        ? ` value="${escapeXml(condition.value)}"`
        : "";
      lines.push(
        `      <condition attribute="${escapeXml(condition.attribute)}" operator="${escapeXml(condition.operator)}"${value} />`,
      );
    }
    lines.push("    </filter>");
  }

  for (const link of model.links) {
    if (!link.name || !link.from || !link.to) continue;
    lines.push(
      `    <link-entity name="${escapeXml(link.name)}" from="${escapeXml(link.from)}" to="${escapeXml(link.to)}" link-type="${link.linkType}"${link.alias ? ` alias="${escapeXml(link.alias)}"` : ""}>`,
    );
    for (const attribute of link.attributes) {
      if (attribute.name) {
        lines.push(`      <attribute name="${escapeXml(attribute.name)}" />`);
      }
    }
    lines.push("    </link-entity>");
  }

  lines.push("  </entity>");
  lines.push("</fetch>");
  return formatFetchXml(lines.join("\n"));
}

function directChildren(node: XmlElementNode, name: string) {
  return node.children.filter(
    (child): child is XmlElementNode =>
      child.type === "element" && child.name === name,
  );
}

function conditionFromNode(
  node: XmlElementNode,
  index: number,
): FetchConditionSelection {
  return {
    id: `condition-${index + 1}`,
    attribute: node.attributes.attribute ?? "",
    operator: node.attributes.operator ?? "eq",
    value: node.attributes.value ?? readFirstValue(node),
  };
}

function linkFromNode(
  node: XmlElementNode,
  index: number,
): FetchLinkEntitySelection {
  return {
    id: `link-${index + 1}`,
    name: node.attributes.name ?? "",
    from: node.attributes.from ?? "",
    to: node.attributes.to ?? "",
    alias: node.attributes.alias ?? "",
    linkType: node.attributes["link-type"] === "outer" ? "outer" : "inner",
    attributes: directChildren(node, "attribute")
      .map((attribute) => attribute.attributes.name)
      .flatMap((name) => (name ? [{ name }] : [])),
  };
}

function readFirstValue(node: XmlElementNode) {
  const value = directChildren(node, "value").at(0);
  const text = value?.children.find((child) => child.type === "text");
  return text?.type === "text" ? text.text : "";
}
