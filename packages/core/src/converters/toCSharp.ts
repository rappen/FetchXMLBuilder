import { formatFetchXml } from "../formatter/formatFetchXml";
import { toPowerAutomateParameters } from "./toPowerAutomate";

export function toCSharpFetchXml(xml: string) {
  const escaped = formatFetchXml(xml).replace(/"/g, '""');
  return `var fetchXml = @"
${escaped}
";`;
}

export function toCSharpFetchXmlWithParameters(xml: string) {
  const converted = toPowerAutomateParameters(xml);
  const data = converted.parameters
    .map(
      (parameter) =>
        `    ${parameter.name} = "${escapeCSharp(parameter.originalValue)}"`,
    )
    .join(",\n");
  const escaped = converted.xml.replace(/"/g, '""');

  return `var fetchData = new
{
${data}
};

var fetchXml = @"
${escaped}
";`;
}

function escapeCSharp(value: string) {
  return value.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
}
