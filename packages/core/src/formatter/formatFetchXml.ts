import type {
  FetchXmlDocument,
  XmlElementNode,
  XmlNode,
} from "../models/fetchXml";
import { escapeXml, parseFetchXml } from "../parser/parseFetchXml";

export function formatFetchXml(
  input: string | FetchXmlDocument,
  indent = "  ",
): string {
  const document = typeof input === "string" ? parseFetchXml(input) : input;
  return formatNode(document.root, 0, indent).join("\n");
}

function formatNode(node: XmlNode, depth: number, indent: string): string[] {
  if (node.type === "text")
    return [`${indent.repeat(depth)}${escapeXml(node.text)}`];

  const attributes = formatAttributes(node);
  if (node.children.length === 0 || node.selfClosing) {
    return [`${indent.repeat(depth)}<${node.name}${attributes} />`];
  }

  const textOnly =
    node.children.length === 1 && node.children[0]?.type === "text";
  if (textOnly) {
    const text =
      node.children[0]?.type === "text" ? escapeXml(node.children[0].text) : "";
    return [
      `${indent.repeat(depth)}<${node.name}${attributes}>${text}</${node.name}>`,
    ];
  }

  return [
    `${indent.repeat(depth)}<${node.name}${attributes}>`,
    ...node.children.flatMap((child) => formatNode(child, depth + 1, indent)),
    `${indent.repeat(depth)}</${node.name}>`,
  ];
}

function formatAttributes(node: XmlElementNode) {
  return Object.entries(node.attributes)
    .map(([name, value]) => ` ${name}="${escapeXml(value)}"`)
    .join("");
}
