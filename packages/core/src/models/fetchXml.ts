export type XmlNode = XmlElementNode | XmlTextNode;

export interface XmlElementNode {
  type: "element";
  name: string;
  attributes: Record<string, string>;
  children: XmlNode[];
  selfClosing: boolean;
}

export interface XmlTextNode {
  type: "text";
  text: string;
}

export interface FetchXmlDocument {
  root: XmlElementNode;
}

export interface ValidationIssue {
  severity: "error" | "warning";
  message: string;
  path: string;
}

export interface ConvertedOutput {
  code: string;
  language: string;
  diagnostics: ValidationIssue[];
}

export interface ParameterManifestItem {
  name: string;
  entity?: string;
  attribute: string;
  originalValue: string;
  inferredType:
    | "string"
    | "integer"
    | "decimal"
    | "datetime"
    | "guid"
    | "boolean";
}

export interface PowerAutomateConversionResult {
  xml: string;
  parameters: ParameterManifestItem[];
  diagnostics: ValidationIssue[];
}

export interface PowerAutomateConversionOptions {
  parameterNames?: Record<string, string>;
  includeEntityName?: boolean;
}

export interface FetchAttributeSelection {
  name: string;
}

export interface FetchConditionSelection {
  id: string;
  attribute: string;
  operator: string;
  value: string;
}

export interface FetchOrderSelection {
  attribute: string;
  descending: boolean;
}

export interface FetchLinkEntitySelection {
  id: string;
  name: string;
  from: string;
  to: string;
  alias: string;
  linkType: "inner" | "outer";
  attributes: FetchAttributeSelection[];
}

export interface FetchQueryModel {
  entity: string;
  top: string;
  attributes: FetchAttributeSelection[];
  conditions: FetchConditionSelection[];
  orders: FetchOrderSelection[];
  links: FetchLinkEntitySelection[];
}
