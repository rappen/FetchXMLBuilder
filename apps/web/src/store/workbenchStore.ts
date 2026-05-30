import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import {
  type AppPreferences,
  type DataverseConnectionProfile,
  clearLegacyConnectionProfiles,
  createWebStorageProvider,
  defaultPreferences,
  readLegacyConnectionProfiles,
} from "@fetchxmlbuilder/storage";
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

interface WorkbenchState {
  fetchXml: string;
  outputTab: OutputTab;
  activeModule: AppModule;
  activePane: WorkbenchPane;
  sidebarCollapsed: boolean;
  storageHydrated: boolean;
  orgUrl: string;
  clientId: string;
  tenantId: string;
  connectionProfiles: DataverseConnectionProfile[];
  activeConnectionProfileId: string;
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
  setSidebarCollapsed: (sidebarCollapsed: boolean) => void;
  setConnectionField: (
    field: "orgUrl" | "clientId" | "tenantId",
    value: string,
  ) => void;
  hydrateStoredState: () => Promise<void>;
  savePreferencesPatch: (update: Partial<AppPreferences>) => Promise<void>;
  loadCachedMetadata: (orgUrl: string, entityName?: string) => Promise<void>;
  upsertConnectionProfile: (
    profile: Omit<DataverseConnectionProfile, "updatedAt"> & {
      updatedAt?: string;
    },
  ) => void;
  deleteConnectionProfile: (profileId: string) => void;
  setConnectionProfileTestResult: (
    profileId: string,
    status: "success" | "error",
    message: string,
  ) => void;
  useConnectionProfile: (profileId: string) => void;
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

const storageProvider = createWebStorageProvider();

export const useWorkbenchStore = create<WorkbenchState>((set, get) => ({
  fetchXml: sampleFetchXml,
  outputTab: defaultPreferences.outputTab,
  activeModule: defaultPreferences.activeModule,
  activePane: defaultPreferences.activePane,
  sidebarCollapsed: defaultPreferences.sidebarCollapsed,
  storageHydrated: false,
  orgUrl: defaultPreferences.orgUrl,
  clientId: defaultPreferences.clientId,
  tenantId: defaultPreferences.tenantId,
  connectionProfiles: [],
  activeConnectionProfileId: defaultPreferences.activeConnectionProfileId,
  resultRows: [],
  connectionStatus: "local",
  connectionError: "",
  userName: "",
  metadataEntities: [],
  metadataAttributesByEntity: {},
  loadingAttributeEntity: "",
  setFetchXml: (fetchXml) => set({ fetchXml }),
  setOutputTab: (outputTab) => {
    set({ outputTab });
    void get().savePreferencesPatch({ outputTab });
  },
  setActiveModule: (activeModule) => {
    set({ activeModule });
    void get().savePreferencesPatch({ activeModule });
  },
  setActivePane: (activePane) => {
    set({ activePane });
    void get().savePreferencesPatch({ activePane });
  },
  setSidebarCollapsed: (sidebarCollapsed) => {
    set({ sidebarCollapsed });
    void get().savePreferencesPatch({ sidebarCollapsed });
  },
  setConnectionField: (field, value) => {
    set({ [field]: value });
    void get().savePreferencesPatch({ [field]: value });
  },
  hydrateStoredState: async () => {
    try {
      const preferences = await storageProvider.preferences.load();
      const legacyProfiles = readLegacyConnectionProfiles();
      const connectionProfiles = legacyProfiles.length
        ? await storageProvider.connectionProfiles.migrateFromLegacyProfiles(
            legacyProfiles,
          )
        : await storageProvider.connectionProfiles.list();
      if (legacyProfiles.length) clearLegacyConnectionProfiles();

      const activeConnectionProfile = connectionProfiles.find(
        (profile) => profile.id === preferences.activeConnectionProfileId,
      );

      set({
        outputTab: preferences.outputTab,
        activeModule: preferences.activeModule,
        activePane: preferences.activePane,
        sidebarCollapsed: preferences.sidebarCollapsed,
        orgUrl: activeConnectionProfile?.orgUrl ?? preferences.orgUrl,
        clientId: activeConnectionProfile?.clientId ?? preferences.clientId,
        tenantId: activeConnectionProfile?.tenantId ?? preferences.tenantId,
        connectionProfiles,
        activeConnectionProfileId: activeConnectionProfile
          ? activeConnectionProfile.id
          : "",
        storageHydrated: true,
      });
    } catch {
      set({ storageHydrated: true });
    }
  },
  savePreferencesPatch: async (update) => {
    await ignoreStorageErrors(storageProvider.preferences.patch(update));
  },
  loadCachedMetadata: async (orgUrl, entityName) => {
    try {
      const [cachedEntities, cachedAttributes] = await Promise.all([
        storageProvider.metadataCache.getEntities(orgUrl),
        entityName
          ? storageProvider.metadataCache.getAttributes(orgUrl, entityName)
          : Promise.resolve(null),
      ]);
      set((state) => ({
        ...(cachedEntities
          ? { metadataEntities: cachedEntities.entities }
          : {}),
        ...(cachedAttributes
          ? {
              metadataAttributesByEntity: {
                ...state.metadataAttributesByEntity,
                [entityName ?? cachedAttributes.entityName]:
                  cachedAttributes.attributes,
              },
            }
          : {}),
      }));
    } catch {
      return;
    }
  },
  upsertConnectionProfile: (profile) =>
    set((state) => {
      const savedProfile = {
        ...profile,
        updatedAt: profile.updatedAt ?? new Date().toISOString(),
      };
      const exists = state.connectionProfiles.some(
        (existingProfile) => existingProfile.id === profile.id,
      );
      const connectionProfiles = exists
        ? state.connectionProfiles.map((existingProfile) =>
            existingProfile.id === profile.id ? savedProfile : existingProfile,
          )
        : [savedProfile, ...state.connectionProfiles];
      void ignoreStorageErrors(
        storageProvider.connectionProfiles.upsert(savedProfile),
      );
      void get().savePreferencesPatch({
        activeConnectionProfileId: profile.id,
        orgUrl: savedProfile.orgUrl,
        clientId: savedProfile.clientId,
        tenantId: savedProfile.tenantId,
      });
      return {
        connectionProfiles,
        activeConnectionProfileId: profile.id,
        orgUrl: savedProfile.orgUrl,
        clientId: savedProfile.clientId,
        tenantId: savedProfile.tenantId,
      };
    }),
  deleteConnectionProfile: (profileId) =>
    set((state) => {
      const connectionProfiles = state.connectionProfiles.filter(
        (profile) => profile.id !== profileId,
      );
      const activeConnectionProfileId =
        state.activeConnectionProfileId === profileId
          ? ""
          : state.activeConnectionProfileId;
      void ignoreStorageErrors(
        storageProvider.connectionProfiles.delete(profileId),
      );
      void get().savePreferencesPatch({ activeConnectionProfileId });
      return {
        connectionProfiles,
        activeConnectionProfileId,
      };
    }),
  setConnectionProfileTestResult: (profileId, status, message) =>
    set((state) => {
      const connectionProfiles = state.connectionProfiles.map((profile) =>
        profile.id === profileId
          ? {
              ...profile,
              lastTestedAt: new Date().toISOString(),
              lastTestStatus: status,
              lastTestMessage: message,
              updatedAt: new Date().toISOString(),
            }
          : profile,
      );
      const updatedProfile = connectionProfiles.find(
        (profile) => profile.id === profileId,
      );
      if (updatedProfile) {
        void ignoreStorageErrors(
          storageProvider.connectionProfiles.upsert(updatedProfile),
        );
      }
      return { connectionProfiles };
    }),
  useConnectionProfile: (profileId) =>
    set((state) => {
      const profile = state.connectionProfiles.find(
        (savedProfile) => savedProfile.id === profileId,
      );
      if (!profile) return {};
      void get().savePreferencesPatch({
        activeConnectionProfileId: profile.id,
        orgUrl: profile.orgUrl,
        clientId: profile.clientId,
        tenantId: profile.tenantId,
      });
      return {
        activeConnectionProfileId: profile.id,
        orgUrl: profile.orgUrl,
        clientId: profile.clientId,
        tenantId: profile.tenantId,
      };
    }),
  setResultRows: (resultRows) => set({ resultRows }),
  setConnectionStatus: (connectionStatus, connectionError = "") =>
    set({ connectionStatus, connectionError }),
  setConnectedUser: (userName) => set({ userName }),
  setMetadataEntities: (metadataEntities) =>
    set((state) => {
      if (state.orgUrl) {
        void ignoreStorageErrors(
          storageProvider.metadataCache.saveEntities(
            state.orgUrl,
            metadataEntities,
          ),
        );
      }
      return { metadataEntities };
    }),
  setEntityAttributes: (entityName, attributes) =>
    set((state) => {
      if (state.orgUrl) {
        void ignoreStorageErrors(
          storageProvider.metadataCache.saveAttributes(
            state.orgUrl,
            entityName,
            attributes,
          ),
        );
      }
      return {
        metadataAttributesByEntity: {
          ...state.metadataAttributesByEntity,
          [entityName]: attributes,
        },
      };
    }),
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

async function ignoreStorageErrors(operation: Promise<unknown>) {
  try {
    await operation;
  } catch {
    return;
  }
}
