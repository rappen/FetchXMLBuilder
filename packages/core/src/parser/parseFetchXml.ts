import type {
  FetchXmlDocument,
  XmlElementNode,
  XmlNode,
} from "../models/fetchXml";

const xmlDeclarationPattern = /^<\?xml[\s\S]*?\?>\s*/;
const commentPattern = /^<!--[\s\S]*?-->\s*/;
const namePattern = /^[A-Za-z_][\w:.-]*/;

export function parseFetchXml(xml: string): FetchXmlDocument {
  const input = xml.trim().replace(xmlDeclarationPattern, "");
  const stack: XmlElementNode[] = [];
  let root: XmlElementNode | undefined;
  let index = 0;

  while (index < input.length) {
    const rest = input.slice(index);

    if (commentPattern.test(rest)) {
      index += rest.match(commentPattern)?.[0].length ?? 0;
      continue;
    }

    if (rest.startsWith("</")) {
      const closeIndex = input.indexOf(">", index);
      if (closeIndex === -1) throw new Error("Unclosed XML closing tag.");
      const name = input.slice(index + 2, closeIndex).trim();
      const current = stack.pop();
      if (!current || current.name !== name) {
        throw new Error(`Unexpected closing tag </${name}>.`);
      }
      index = closeIndex + 1;
      continue;
    }

    if (rest.startsWith("<")) {
      const parsed = parseOpeningTag(input, index);
      const node: XmlElementNode = {
        type: "element",
        name: parsed.name,
        attributes: parsed.attributes,
        children: [],
        selfClosing: parsed.selfClosing,
      };

      const parent = stack.at(-1);
      if (parent) parent.children.push(node);
      if (!root) root = node;
      if (!parsed.selfClosing) stack.push(node);
      index = parsed.nextIndex;
      continue;
    }

    const nextTag = input.indexOf("<", index);
    const end = nextTag === -1 ? input.length : nextTag;
    const text = input.slice(index, end);
    if (text.trim()) {
      stack.at(-1)?.children.push({ type: "text", text: text.trim() });
    }
    index = end;
  }

  if (!root) throw new Error("FetchXML document is empty.");
  if (stack.length > 0)
    throw new Error(`Unclosed XML tag <${stack.at(-1)?.name}>.`);
  return { root };
}

function parseOpeningTag(input: string, startIndex: number) {
  let index = startIndex + 1;
  const name = input.slice(index).match(namePattern)?.[0];
  if (!name) throw new Error(`Invalid XML tag at offset ${startIndex}.`);
  index += name.length;

  const attributes: Record<string, string> = {};
  let selfClosing = false;

  while (index < input.length) {
    index = skipWhitespace(input, index);
    const char = input[index];

    if (char === ">") {
      index++;
      break;
    }

    if (char === "/" && input[index + 1] === ">") {
      selfClosing = true;
      index += 2;
      break;
    }

    const attributeName = input.slice(index).match(namePattern)?.[0];
    if (!attributeName)
      throw new Error(`Invalid XML attribute near offset ${index}.`);
    index += attributeName.length;
    index = skipWhitespace(input, index);
    if (input[index] !== "=")
      throw new Error(`Expected '=' after attribute ${attributeName}.`);
    index++;
    index = skipWhitespace(input, index);

    const quote = input[index];
    if (quote !== "'" && quote !== '"') {
      throw new Error(`Expected quoted value for attribute ${attributeName}.`);
    }
    index++;
    const valueStart = index;
    const valueEnd = input.indexOf(quote, valueStart);
    if (valueEnd === -1)
      throw new Error(`Unclosed value for attribute ${attributeName}.`);
    attributes[attributeName] = unescapeXml(input.slice(valueStart, valueEnd));
    index = valueEnd + 1;
  }

  return { name, attributes, selfClosing, nextIndex: index };
}

function skipWhitespace(input: string, index: number) {
  let nextIndex = index;
  while (/\s/.test(input[nextIndex] ?? "")) nextIndex++;
  return nextIndex;
}

export function walkElements(
  node: XmlNode,
  visit: (node: XmlElementNode, path: string) => void,
  path = "",
) {
  if (node.type !== "element") return;
  const nextPath = `${path}/${node.name}`;
  visit(node, nextPath);
  for (const child of node.children) {
    walkElements(child, visit, nextPath);
  }
}

export function escapeXml(value: string) {
  return value
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function unescapeXml(value: string) {
  return value
    .replace(/&apos;/g, "'")
    .replace(/&quot;/g, '"')
    .replace(/&gt;/g, ">")
    .replace(/&lt;/g, "<")
    .replace(/&amp;/g, "&");
}
