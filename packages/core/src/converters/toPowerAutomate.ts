import { formatFetchXml } from "../formatter/formatFetchXml";
import type {
  ParameterManifestItem,
  PowerAutomateConversionOptions,
  PowerAutomateConversionResult,
  XmlElementNode,
} from "../models/fetchXml";
import { parseFetchXml, walkElements } from "../parser/parseFetchXml";
import { validateFetchXml } from "../validator/validateFetchXml";

export function toPowerAutomateParameters(
  xml: string,
  options: PowerAutomateConversionOptions = {},
): PowerAutomateConversionResult {
  const document = parseFetchXml(xml);
  const parameters: ParameterManifestItem[] = [];
  const usedNames = new Set<string>();

  walkElements(document.root, (node) => {
    if (node.name !== "condition") return;

    const entity = node.attributes.entityname;
    const attribute = node.attributes.attribute;
    if (!attribute) return;

    if (node.attributes.value) {
      const parameter = createParameter(
        attribute,
        node.attributes.value,
        entity,
        options,
        usedNames,
      );
      parameters.push(parameter);
      node.attributes.value = tokenFor(parameter.name);
    }

    for (const child of node.children) {
      if (child.type !== "element" || child.name !== "value") continue;
      const text = child.children.find(
        (valueChild) => valueChild.type === "text",
      );
      if (!text || text.type !== "text") continue;
      const parameter = createParameter(
        attribute,
        text.text,
        entity,
        options,
        usedNames,
      );
      parameters.push(parameter);
      text.text = tokenFor(parameter.name);
    }
  });

  return {
    xml: formatFetchXml(document),
    parameters,
    diagnostics: validateFetchXml(formatFetchXml(document)),
  };
}

function createParameter(
  attribute: string,
  originalValue: string,
  entity: string | undefined,
  options: PowerAutomateConversionOptions,
  usedNames: Set<string>,
): ParameterManifestItem {
  const overrideKey = entity ? `${entity}.${attribute}` : attribute;
  const baseName =
    options.parameterNames?.[overrideKey] ??
    options.parameterNames?.[attribute] ??
    normalizeParameterName(
      options.includeEntityName && entity
        ? `${entity}_${attribute}`
        : attribute,
    );
  const name = uniqueName(normalizeParameterName(baseName), usedNames);
  usedNames.add(name);

  return {
    name,
    ...(entity ? { entity } : {}),
    attribute,
    originalValue,
    inferredType: inferType(originalValue),
  };
}

function tokenFor(name: string) {
  return `@{parameters('${name}')}`;
}

function normalizeParameterName(name: string) {
  const normalized = name
    .trim()
    .replace(/[^A-Za-z0-9_]+/g, "_")
    .replace(/^_+|_+$/g, "");
  return /^[A-Za-z_]/.test(normalized) ? normalized : `p_${normalized}`;
}

function uniqueName(baseName: string, usedNames: Set<string>) {
  let name = baseName || "parameter";
  let suffix = 1;
  while (usedNames.has(name)) {
    suffix++;
    name = `${baseName}${suffix}`;
  }
  return name;
}

function inferType(value: string): ParameterManifestItem["inferredType"] {
  if (/^(true|false)$/i.test(value)) return "boolean";
  if (
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
      value,
    )
  )
    return "guid";
  if (/^-?\d+$/.test(value)) return "integer";
  if (/^-?\d+\.\d+$/.test(value)) return "decimal";
  if (!Number.isNaN(Date.parse(value)) && /\d{4}-\d{2}-\d{2}/.test(value))
    return "datetime";
  return "string";
}

export function collectConditionNodes(xml: string): XmlElementNode[] {
  const nodes: XmlElementNode[] = [];
  walkElements(parseFetchXml(xml).root, (node) => {
    if (node.name === "condition") nodes.push(node);
  });
  return nodes;
}
