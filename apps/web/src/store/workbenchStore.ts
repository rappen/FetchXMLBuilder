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

export type WorkbenchPane = "editor" | "builder" | "metadata" | "results";
export type ConnectionStatus =
  | "local"
  | "connecting"
  | "connected"
  | "loadingMetadata"
  | "error";

interface WorkbenchState {
  fetchXml: string;
  outputTab: OutputTab;
  activePane: WorkbenchPane;
  orgUrl: string;
  clientId: string;
  tenantId: string;
  resultRows: Record<string, unknown>[];
  connectionStatus: ConnectionStatus;
  connectionError: string;
  userName: string;
  metadataEntities: EntitySummary[];
  metadataAttributesByEntity: Record<string, AttributeSummary[]>;
  loadingAttributeEntity: string;
  setFetchXml: (fetchXml: string) => void;
  setOutputTab: (outputTab: OutputTab) => void;
  setActivePane: (activePane: WorkbenchPane) => void;
  setConnectionField: (
    field: "orgUrl" | "clientId" | "tenantId",
    value: string,
  ) => void;
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
  activePane: "editor",
  orgUrl: "",
  clientId: "",
  tenantId: "common",
  resultRows: [],
  connectionStatus: "local",
  connectionError: "",
  userName: "",
  metadataEntities: [],
  metadataAttributesByEntity: {},
  loadingAttributeEntity: "",
  setFetchXml: (fetchXml) => set({ fetchXml }),
  setOutputTab: (outputTab) => set({ outputTab }),
  setActivePane: (activePane) => set({ activePane }),
  setConnectionField: (field, value) => set({ [field]: value }),
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
