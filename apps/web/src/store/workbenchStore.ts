import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import { create } from "zustand";

export type OutputTab =
  | "powerAutomate"
  | "odata"
  | "csharp"
  | "javascript"
  | "validation";

export type AppModule = "workbench" | "credentials";
export type WorkbenchPane = "editor" | "builder" | "metadata" | "results";
export type ConnectionStatus =
  | "local"
  | "connecting"
  | "connected"
  | "loadingMetadata"
  | "error";

export interface DataverseCredential {
  id: string;
  name: string;
  orgUrl: string;
  clientId: string;
  tenantId: string;
  lastTestedAt?: string;
  lastTestStatus?: "success" | "error";
  lastTestMessage?: string;
  updatedAt: string;
}

interface WorkbenchState {
  fetchXml: string;
  outputTab: OutputTab;
  activeModule: AppModule;
  activePane: WorkbenchPane;
  orgUrl: string;
  clientId: string;
  tenantId: string;
  credentials: DataverseCredential[];
  activeCredentialId: string;
  resultRows: Record<string, unknown>[];
  connectionStatus: ConnectionStatus;
  connectionError: string;
  userName: string;
  metadataEntities: EntitySummary[];
  metadataAttributesByEntity: Record<string, AttributeSummary[]>;
  loadingAttributeEntity: string;
  setFetchXml: (fetchXml: string) => void;
  setOutputTab: (outputTab: OutputTab) => void;
  setActiveModule: (activeModule: AppModule) => void;
  setActivePane: (activePane: WorkbenchPane) => void;
  setConnectionField: (
    field: "orgUrl" | "clientId" | "tenantId",
    value: string,
  ) => void;
  upsertCredential: (
    credential: Omit<DataverseCredential, "updatedAt"> & {
      updatedAt?: string;
    },
  ) => void;
  deleteCredential: (credentialId: string) => void;
  setCredentialTestResult: (
    credentialId: string,
    status: "success" | "error",
    message: string,
  ) => void;
  useCredential: (credentialId: string) => void;
  setResultRows: (resultRows: Record<string, unknown>[]) => void;
  setConnectionStatus: (
    connectionStatus: ConnectionStatus,
    connectionError?: string,
  ) => void;
  setConnectedUser: (userName: string) => void;
  setMetadataEntities: (metadataEntities: EntitySummary[]) => void;
  setEntityAttributes: (
    entityName: string,
    attributes: AttributeSummary[],
  ) => void;
  setLoadingAttributeEntity: (entityName: string) => void;
  clearLiveConnection: () => void;
}

export const sampleFetchXml = `<fetch top="50">
  <entity name="account">
    <attribute name="name" />
    <attribute name="accountid" />
    <attribute name="createdon" />
    <order attribute="name" />
    <filter type="and">
      <condition attribute="name" operator="like" value="%Contoso%" />
      <condition attribute="statecode" operator="eq" value="0" />
      <condition attribute="createdon" operator="on-or-after" value="2024-01-01" />
    </filter>
    <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="outer" alias="primarycontact">
      <attribute name="fullname" />
    </link-entity>
  </entity>
</fetch>`;

export const useWorkbenchStore = create<WorkbenchState>((set) => ({
  fetchXml: sampleFetchXml,
  outputTab: "powerAutomate",
  activeModule: "workbench",
  activePane: "editor",
  orgUrl: "",
  clientId: "",
  tenantId: "common",
  credentials: readStoredCredentials(),
  activeCredentialId: "",
  resultRows: [],
  connectionStatus: "local",
  connectionError: "",
  userName: "",
  metadataEntities: [],
  metadataAttributesByEntity: {},
  loadingAttributeEntity: "",
  setFetchXml: (fetchXml) => set({ fetchXml }),
  setOutputTab: (outputTab) => set({ outputTab }),
  setActiveModule: (activeModule) => set({ activeModule }),
  setActivePane: (activePane) => set({ activePane }),
  setConnectionField: (field, value) => set({ [field]: value }),
  upsertCredential: (credential) =>
    set((state) => {
      const savedCredential = {
        ...credential,
        updatedAt: credential.updatedAt ?? new Date().toISOString(),
      };
      const exists = state.credentials.some(
        (existingCredential) => existingCredential.id === credential.id,
      );
      const credentials = exists
        ? state.credentials.map((existingCredential) =>
            existingCredential.id === credential.id
              ? savedCredential
              : existingCredential,
          )
        : [savedCredential, ...state.credentials];
      writeStoredCredentials(credentials);
      return { credentials, activeCredentialId: credential.id };
    }),
  deleteCredential: (credentialId) =>
    set((state) => {
      const credentials = state.credentials.filter(
        (credential) => credential.id !== credentialId,
      );
      writeStoredCredentials(credentials);
      return {
        credentials,
        activeCredentialId:
          state.activeCredentialId === credentialId
            ? ""
            : state.activeCredentialId,
      };
    }),
  setCredentialTestResult: (credentialId, status, message) =>
    set((state) => {
      const credentials = state.credentials.map((credential) =>
        credential.id === credentialId
          ? {
              ...credential,
              lastTestedAt: new Date().toISOString(),
              lastTestStatus: status,
              lastTestMessage: message,
              updatedAt: new Date().toISOString(),
            }
          : credential,
      );
      writeStoredCredentials(credentials);
      return { credentials };
    }),
  useCredential: (credentialId) =>
    set((state) => {
      const credential = state.credentials.find(
        (savedCredential) => savedCredential.id === credentialId,
      );
      if (!credential) return {};
      return {
        activeCredentialId: credential.id,
        orgUrl: credential.orgUrl,
        clientId: credential.clientId,
        tenantId: credential.tenantId,
      };
    }),
  setResultRows: (resultRows) => set({ resultRows }),
  setConnectionStatus: (connectionStatus, connectionError = "") =>
    set({ connectionStatus, connectionError }),
  setConnectedUser: (userName) => set({ userName }),
  setMetadataEntities: (metadataEntities) => set({ metadataEntities }),
  setEntityAttributes: (entityName, attributes) =>
    set((state) => ({
      metadataAttributesByEntity: {
        ...state.metadataAttributesByEntity,
        [entityName]: attributes,
      },
    })),
  setLoadingAttributeEntity: (entityName) =>
    set({ loadingAttributeEntity: entityName }),
  clearLiveConnection: () =>
    set({
      connectionStatus: "local",
      connectionError: "",
      userName: "",
      metadataEntities: [],
      metadataAttributesByEntity: {},
      loadingAttributeEntity: "",
      resultRows: [],
    }),
}));

const credentialsStorageKey = "fetchxmlbuilder.dataverseCredentials.v1";

function readStoredCredentials(): DataverseCredential[] {
  if (typeof globalThis.localStorage === "undefined") return [];
  try {
    const rawCredentials = globalThis.localStorage.getItem(
      credentialsStorageKey,
    );
    if (!rawCredentials) return [];
    const credentials = JSON.parse(rawCredentials) as DataverseCredential[];
    return Array.isArray(credentials) ? credentials : [];
  } catch {
    return [];
  }
}

function writeStoredCredentials(credentials: DataverseCredential[]) {
  if (typeof globalThis.localStorage === "undefined") return;
  globalThis.localStorage.setItem(
    credentialsStorageKey,
    JSON.stringify(credentials),
  );
}
