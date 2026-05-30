import { formatFetchXml } from "../formatter/formatFetchXml";
import type {
  FetchConditionSelection,
  FetchFilterGroup,
  FetchLinkEntitySelection,
  FetchQueryModel,
  XmlElementNode,
} from "../models/fetchXml";
import { escapeXml, parseFetchXml } from "../parser/parseFetchXml";

export const emptyFetchQueryModel: FetchQueryModel = {
  entity: "account",
  top: "50",
  distinct: false,
  returnTotalRecordCount: false,
  orderByRawValue: false,
  count: "",
  page: "",
  pagingCookie: "",
  filterType: "and",
  attributes: [{ name: "name" }],
  conditions: [],
  filters: [],
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

  const filter = directChildren(entity, "filter").at(0);
  const conditions = filter
    ? directChildren(filter, "condition")
        .filter((condition) => condition.attributes.attribute)
        .map((condition, index) => conditionFromNode(condition, index))
    : [];
  const filters = filter
    ? directChildren(filter, "filter").map((childFilter, index) =>
        filterFromNode(childFilter, `${index + 1}`),
      )
    : [];

  const links = directChildren(entity, "link-entity").map((node, index) =>
    linkFromNode(node, `${index + 1}`),
  );

  return {
    entity: entity.attributes.name ?? "account",
    top: document.root.attributes.top ?? "",
    distinct: document.root.attributes.distinct === "true",
    returnTotalRecordCount:
      document.root.attributes.returntotalrecordcount === "true",
    orderByRawValue: document.root.attributes.useraworderby === "true",
    count: document.root.attributes.count ?? "",
    page: document.root.attributes.page ?? "",
    pagingCookie: document.root.attributes["paging-cookie"] ?? "",
    filterType: filter?.attributes.type === "or" ? "or" : "and",
    attributes,
    conditions,
    filters,
    orders,
    links,
  };
}

export function writeFetchQueryModel(model: FetchQueryModel): string {
  const fetchAttributes = [
    model.top ? `top="${escapeXml(model.top)}"` : "",
    model.distinct ? 'distinct="true"' : "",
    model.returnTotalRecordCount ? 'returntotalrecordcount="true"' : "",
    model.orderByRawValue ? 'useraworderby="true"' : "",
    model.count ? `count="${escapeXml(model.count)}"` : "",
    model.page ? `page="${escapeXml(model.page)}"` : "",
    model.pagingCookie
      ? `paging-cookie="${escapeXml(model.pagingCookie)}"`
      : "",
  ].filter(Boolean);
  const lines = [
    `<fetch${fetchAttributes.length ? ` ${fetchAttributes.join(" ")}` : ""}>`,
  ];
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

  if (model.conditions.length > 0 || model.filters.length > 0) {
    writeFilterGroup(
      lines,
      {
        id: "root-filter",
        type: model.filterType ?? "and",
        conditions: model.conditions,
        filters: model.filters,
      },
      4,
    );
  }

  for (const link of model.links) writeLinkEntity(lines, link, 4);

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

function filterFromNode(node: XmlElementNode, path: string): FetchFilterGroup {
  return {
    id: `filter-${path}`,
    type: node.attributes.type === "or" ? "or" : "and",
    conditions: directChildren(node, "condition")
      .filter((condition) => condition.attributes.attribute)
      .map((condition, index) =>
        conditionFromNode(
          condition,
          Number(`${path.replace(/\D/g, "")}${index + 1}`),
        ),
      ),
    filters: directChildren(node, "filter").map((filter, index) =>
      filterFromNode(filter, `${path}-${index + 1}`),
    ),
  };
}

function linkFromNode(
  node: XmlElementNode,
  path: string,
): FetchLinkEntitySelection {
  return {
    id: `link-${path}`,
    name: node.attributes.name ?? "",
    from: node.attributes.from ?? "",
    to: node.attributes.to ?? "",
    alias: node.attributes.alias ?? "",
    linkType: node.attributes["link-type"] === "outer" ? "outer" : "inner",
    attributes: directChildren(node, "attribute")
      .map((attribute) => attribute.attributes.name)
      .flatMap((name) => (name ? [{ name }] : [])),
    orders: directChildren(node, "order")
      .filter((order) => order.attributes.attribute)
      .map((order) => ({
        attribute: order.attributes.attribute ?? "",
        descending: order.attributes.descending === "true",
      })),
    filterType:
      directChildren(node, "filter").at(0)?.attributes.type === "or"
        ? "or"
        : "and",
    conditions: directChildren(node, "filter")
      .slice(0, 1)
      .flatMap((filter, filterIndex) =>
        directChildren(filter, "condition")
          .filter((condition) => condition.attributes.attribute)
          .map((condition, conditionIndex) =>
            conditionFromNode(
              condition,
              Number(
                `${path.replace(/\D/g, "")}${filterIndex}${conditionIndex}`,
              ),
            ),
          ),
      ),
    filters:
      directChildren(node, "filter")
        .at(0)
        ?.children.filter(
          (child): child is XmlElementNode =>
            child.type === "element" && child.name === "filter",
        )
        .map((filter, index) =>
          filterFromNode(filter, `${path}-${index + 1}`),
        ) ?? [],
    links: directChildren(node, "link-entity").map((child, index) =>
      linkFromNode(child, `${path}-${index + 1}`),
    ),
  };
}

function writeLinkEntity(
  lines: string[],
  link: FetchLinkEntitySelection,
  indentSize: number,
) {
  if (!link.name || !link.from || !link.to) return;

  const indent = " ".repeat(indentSize);
  const childIndent = " ".repeat(indentSize + 2);

  lines.push(
    `${indent}<link-entity name="${escapeXml(link.name)}" from="${escapeXml(link.from)}" to="${escapeXml(link.to)}" link-type="${link.linkType}"${link.alias ? ` alias="${escapeXml(link.alias)}"` : ""}>`,
  );
  for (const attribute of link.attributes) {
    if (attribute.name) {
      lines.push(
        `${childIndent}<attribute name="${escapeXml(attribute.name)}" />`,
      );
    }
  }
  for (const order of link.orders ?? []) {
    if (order.attribute) {
      lines.push(
        `${childIndent}<order attribute="${escapeXml(order.attribute)}"${order.descending ? ' descending="true"' : ""} />`,
      );
    }
  }
  if ((link.conditions ?? []).length > 0 || (link.filters ?? []).length > 0) {
    writeFilterGroup(
      lines,
      {
        id: `${link.id}-filter`,
        type: link.filterType ?? "and",
        conditions: link.conditions ?? [],
        filters: link.filters ?? [],
      },
      indentSize + 2,
    );
  }
  for (const childLink of link.links ?? []) {
    writeLinkEntity(lines, childLink, indentSize + 2);
  }
  lines.push(`${indent}</link-entity>`);
}

function writeFilterGroup(
  lines: string[],
  filter: Pick<FetchFilterGroup, "type" | "conditions" | "filters" | "id">,
  indentSize: number,
) {
  const indent = " ".repeat(indentSize);
  const childIndent = " ".repeat(indentSize + 2);

  lines.push(`${indent}<filter type="${filter.type ?? "and"}">`);
  for (const condition of filter.conditions ?? []) {
    if (!condition.attribute || !condition.operator) continue;
    const value = condition.value
      ? ` value="${escapeXml(condition.value)}"`
      : "";
    lines.push(
      `${childIndent}<condition attribute="${escapeXml(condition.attribute)}" operator="${escapeXml(condition.operator)}"${value} />`,
    );
  }
  for (const childFilter of filter.filters ?? []) {
    writeFilterGroup(lines, childFilter, indentSize + 2);
  }
  lines.push(`${indent}</filter>`);
}

function readFirstValue(node: XmlElementNode) {
  const value = directChildren(node, "value").at(0);
  const text = value?.children.find((child) => child.type === "text");
  return text?.type === "text" ? text.text : "";
}
