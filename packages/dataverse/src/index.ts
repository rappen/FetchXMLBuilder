import {
  type AccountInfo,
  InteractionRequiredAuthError,
  PublicClientApplication,
} from "@azure/msal-browser";

export interface DataverseConnectionConfig {
  organizationUrl: string;
  clientId?: string;
  tenantId?: string;
}

export const DEFAULT_DATAVERSE_CLIENT_ID =
  "51f81489-12ee-4a9e-aaae-a2591f45987d";

export interface EntitySummary {
  logicalName: string;
  displayName: string;
  entitySetName?: string;
}

export interface AttributeSummary {
  logicalName: string;
  displayName: string;
  type: string;
}

export interface RelationshipSummary {
  schemaName: string;
  referencedEntity: string;
  referencedAttribute: string;
  referencingEntity: string;
  referencingAttribute: string;
}

export interface FetchXmlExecutionResult {
  rows: Record<string, unknown>[];
  nextLink?: string;
}

export interface DataverseClient {
  listEntities(): Promise<EntitySummary[]>;
  listAttributes(entityLogicalName: string): Promise<AttributeSummary[]>;
  listRelationships(entityLogicalName: string): Promise<RelationshipSummary[]>;
  executeFetchXml(
    entitySetName: string,
    fetchXml: string,
  ): Promise<FetchXmlExecutionResult>;
}

export interface DataverseSession {
  account: AccountInfo;
  client: DataverseClient;
  msal: PublicClientApplication;
  organizationUrl: string;
}

export function normalizeOrganizationUrl(url: string) {
  const parsed = new URL(url);
  parsed.pathname = "";
  parsed.search = "";
  parsed.hash = "";
  return parsed.toString().replace(/\/$/, "");
}

export function createDataverseClient(
  config: DataverseConnectionConfig,
  getAccessToken: () => Promise<string>,
): DataverseClient {
  const organizationUrl = normalizeOrganizationUrl(config.organizationUrl);

  async function request<T>(path: string) {
    const token = await getAccessToken();
    const response = await fetch(`${organizationUrl}/api/data/v9.2/${path}`, {
      headers: {
        Authorization: `Bearer ${token}`,
        Accept: "application/json",
        "OData-Version": "4.0",
        "OData-MaxVersion": "4.0",
      },
    });

    if (!response.ok) {
      throw new Error(
        `Dataverse request failed (${response.status}): ${await response.text()}`,
      );
    }

    return (await response.json()) as T;
  }

  return {
    async listEntities() {
      const result = await request<{ value: Array<Record<string, unknown>> }>(
        "EntityDefinitions?$select=LogicalName,EntitySetName,DisplayName",
      );
      return result.value.map((entity) => ({
        logicalName: String(entity.LogicalName ?? ""),
        ...(entity.EntitySetName
          ? { entitySetName: String(entity.EntitySetName) }
          : {}),
        displayName:
          readLabel(entity.DisplayName) || String(entity.LogicalName ?? ""),
      }));
    },
    async listAttributes(entityLogicalName) {
      const escapedEntityName = entityLogicalName.replace(/'/g, "''");
      const result = await request<{ value: Array<Record<string, unknown>> }>(
        `EntityDefinitions(LogicalName='${escapedEntityName}')/Attributes?$select=LogicalName,DisplayName,AttributeType`,
      );
      return result.value.map((attribute) => ({
        logicalName: String(attribute.LogicalName ?? ""),
        displayName:
          readLabel(attribute.DisplayName) ||
          String(attribute.LogicalName ?? ""),
        type: String(attribute.AttributeType ?? "Unknown"),
      }));
    },
    async listRelationships(entityLogicalName) {
      const escapedEntityName = entityLogicalName.replace(/'/g, "''");
      const select =
        "$select=SchemaName,ReferencedEntity,ReferencedAttribute,ReferencingEntity,ReferencingAttribute";
      const result = await request<{
        OneToManyRelationships?: Array<Record<string, unknown>>;
        ManyToOneRelationships?: Array<Record<string, unknown>>;
      }>(
        `EntityDefinitions(LogicalName='${escapedEntityName}')?$select=LogicalName&$expand=OneToManyRelationships(${select}),ManyToOneRelationships(${select})`,
      );
      const relationships = [
        ...(result.OneToManyRelationships ?? []),
        ...(result.ManyToOneRelationships ?? []),
      ]
        .map(readRelationship)
        .filter((relationship) => relationship.schemaName);
      return dedupeRelationships(relationships).sort((left, right) =>
        left.schemaName.localeCompare(right.schemaName),
      );
    },
    async executeFetchXml(entitySetName, fetchXml) {
      const encoded = encodeURIComponent(fetchXml);
      const result = await request<{
        value: Record<string, unknown>[];
        "@odata.nextLink"?: string;
      }>(`${entitySetName}?fetchXml=${encoded}`);
      return {
        rows: result.value,
        ...(result["@odata.nextLink"]
          ? { nextLink: result["@odata.nextLink"] }
          : {}),
      };
    },
  };
}

export async function createDataverseSession(
  config: DataverseConnectionConfig,
): Promise<DataverseSession> {
  const organizationUrl = normalizeOrganizationUrl(config.organizationUrl);
  const tenantId = config.tenantId?.trim() || "common";
  const clientId = config.clientId?.trim() || DEFAULT_DATAVERSE_CLIENT_ID;
  const msal = new PublicClientApplication({
    auth: {
      clientId,
      authority: `https://login.microsoftonline.com/${tenantId}`,
      redirectUri: globalThis.location.origin,
    },
    cache: {
      cacheLocation: "sessionStorage",
      storeAuthStateInCookie: false,
    },
  });

  await msal.initialize();

  const scopes = getDataverseScopes(organizationUrl);
  const loginResult = await msal.loginPopup({ scopes });
  msal.setActiveAccount(loginResult.account);

  const client = createDataverseClient({ ...config, organizationUrl }, () =>
    acquireDataverseToken(msal, organizationUrl, loginResult.account),
  );

  return {
    account: loginResult.account,
    client,
    msal,
    organizationUrl,
  };
}

export async function acquireDataverseToken(
  msal: PublicClientApplication,
  organizationUrl: string,
  account = msal.getActiveAccount(),
) {
  if (!account) throw new Error("No active Entra account.");
  const normalizedUrl = normalizeOrganizationUrl(organizationUrl);
  const request = {
    account,
    scopes: getDataverseScopes(normalizedUrl),
  };
  const result = await msal.acquireTokenSilent(request).catch((error) => {
    if (error instanceof InteractionRequiredAuthError) {
      return msal.acquireTokenPopup(request);
    }
    throw error;
  });
  return result.accessToken;
}

export function getDataverseScopes(organizationUrl: string) {
  return [`${normalizeOrganizationUrl(organizationUrl)}/user_impersonation`];
}

function readRelationship(value: Record<string, unknown>): RelationshipSummary {
  return {
    schemaName: String(value.SchemaName ?? ""),
    referencedEntity: String(value.ReferencedEntity ?? ""),
    referencedAttribute: String(value.ReferencedAttribute ?? ""),
    referencingEntity: String(value.ReferencingEntity ?? ""),
    referencingAttribute: String(value.ReferencingAttribute ?? ""),
  };
}

function dedupeRelationships(relationships: RelationshipSummary[]) {
  const seen = new Set<string>();
  return relationships.filter((relationship) => {
    const key = [
      relationship.schemaName,
      relationship.referencedEntity,
      relationship.referencedAttribute,
      relationship.referencingEntity,
      relationship.referencingAttribute,
    ].join(":");
    if (seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

function readLabel(value: unknown) {
  const labels = (
    value as { UserLocalizedLabel?: { Label?: string } } | undefined
  )?.UserLocalizedLabel;
  return labels?.Label ?? "";
}
