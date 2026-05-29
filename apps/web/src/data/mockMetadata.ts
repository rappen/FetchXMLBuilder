export interface MockAttribute {
  logicalName: string;
  displayName: string;
  type:
    | "String"
    | "Guid"
    | "DateTime"
    | "Integer"
    | "Boolean"
    | "Money"
    | "Lookup";
}

export interface MockRelationship {
  name: string;
  from: string;
  to: string;
  target: string;
}

export interface MockEntity {
  logicalName: string;
  displayName: string;
  entitySetName: string;
  attributes: MockAttribute[];
  relationships: MockRelationship[];
}

export const mockEntities: MockEntity[] = [
  {
    logicalName: "account",
    displayName: "Account",
    entitySetName: "accounts",
    attributes: [
      { logicalName: "accountid", displayName: "Account", type: "Guid" },
      { logicalName: "name", displayName: "Account Name", type: "String" },
      { logicalName: "createdon", displayName: "Created On", type: "DateTime" },
      { logicalName: "statecode", displayName: "Status", type: "Integer" },
      { logicalName: "revenue", displayName: "Annual Revenue", type: "Money" },
      {
        logicalName: "primarycontactid",
        displayName: "Primary Contact",
        type: "Lookup",
      },
    ],
    relationships: [
      {
        name: "contact_customer_accounts",
        from: "parentcustomerid",
        to: "accountid",
        target: "contact",
      },
    ],
  },
  {
    logicalName: "contact",
    displayName: "Contact",
    entitySetName: "contacts",
    attributes: [
      { logicalName: "contactid", displayName: "Contact", type: "Guid" },
      { logicalName: "fullname", displayName: "Full Name", type: "String" },
      { logicalName: "emailaddress1", displayName: "Email", type: "String" },
      { logicalName: "createdon", displayName: "Created On", type: "DateTime" },
      { logicalName: "statecode", displayName: "Status", type: "Integer" },
      {
        logicalName: "parentcustomerid",
        displayName: "Parent Customer",
        type: "Lookup",
      },
    ],
    relationships: [],
  },
  {
    logicalName: "opportunity",
    displayName: "Opportunity",
    entitySetName: "opportunities",
    attributes: [
      {
        logicalName: "opportunityid",
        displayName: "Opportunity",
        type: "Guid",
      },
      { logicalName: "name", displayName: "Topic", type: "String" },
      {
        logicalName: "estimatedvalue",
        displayName: "Est. Revenue",
        type: "Money",
      },
      {
        logicalName: "closeprobability",
        displayName: "Probability",
        type: "Integer",
      },
      { logicalName: "createdon", displayName: "Created On", type: "DateTime" },
      { logicalName: "statecode", displayName: "Status", type: "Integer" },
    ],
    relationships: [],
  },
];

const fallbackEntity = mockEntities[0] as MockEntity;

export function getEntity(logicalName: string) {
  return (
    mockEntities.find((entity) => entity.logicalName === logicalName) ??
    fallbackEntity
  );
}

export function makeMockRows(entityName: string, attributes: string[]) {
  const visibleAttributes = attributes.length > 0 ? attributes : ["name"];
  return Array.from({ length: 15 }, (_, index) => {
    const row: Record<string, string | number | boolean> = {};
    for (const attribute of visibleAttributes) {
      row[attribute] = mockValue(entityName, attribute, index);
    }
    return row;
  });
}

function mockValue(entityName: string, attribute: string, index: number) {
  if (attribute.endsWith("id")) {
    return `${entityName}-${String(index + 1).padStart(4, "0")}`;
  }
  if (attribute.includes("createdon")) {
    return `2026-05-${String((index % 28) + 1).padStart(2, "0")}`;
  }
  if (attribute.includes("statecode")) {
    return index % 2;
  }
  if (attribute.includes("revenue") || attribute.includes("value")) {
    return 10000 + index * 1250;
  }
  if (attribute.includes("email")) {
    return `person.${index + 1}@example.test`;
  }
  return `${titleCase(attribute)} ${index + 1}`;
}

function titleCase(value: string) {
  return value
    .replace(/id$/i, "")
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/[_-]/g, " ")
    .replace(/\b\w/g, (letter) => letter.toUpperCase())
    .trim();
}
