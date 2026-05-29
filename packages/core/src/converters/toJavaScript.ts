import { formatFetchXml } from "../formatter/formatFetchXml";
import { toPowerAutomateParameters } from "./toPowerAutomate";

export function toJavaScriptFetchXml(xml: string) {
  const formatted = formatFetchXml(xml);
  const lines = formatted.split("\n").map((line) => JSON.stringify(line));
  return `const fetchXml = [\n  ${lines.join(",\n  ")}\n].join("\\n");`;
}

export function toJavaScriptFetchXmlWithParameters(xml: string) {
  const converted = toPowerAutomateParameters(xml);
  const data = Object.fromEntries(
    converted.parameters.map((parameter) => [
      parameter.name,
      parameter.originalValue,
    ]),
  );
  return `const fetchData = ${JSON.stringify(data, null, 2)};\n\nconst fetchXml = ${JSON.stringify(converted.xml, null, 2)};`;
}
