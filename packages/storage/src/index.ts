import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";

export type StoredAppModule = "workbench" | "credentials";
export type StoredWorkbenchPane = "editor" | "builder" | "metadata" | "results";
export type StoredOutputTab =
  | "powerAutomate"
  | "odata"
  | "csharp"
  | "javascript"
  | "validation";

export interface AppPreferences {
  schemaVersion: 1;
  activeModule: StoredAppModule;
  activePane: StoredWorkbenchPane;
  outputTab: StoredOutputTab;
  activeConnectionProfileId: string;
  sidebarCollapsed: boolean;
  orgUrl: string;
  clientId: string;
  tenantId: string;
}

export interface DataverseConnectionProfile {
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

export interface CachedEntities {
  orgUrl: string;
  entities: EntitySummary[];
  updatedAt: string;
}

export interface CachedAttributes {
  orgUrl: string;
  entityName: string;
  attributes: AttributeSummary[];
  updatedAt: string;
}

export interface SchemaCacheEntry {
  cacheKey: string;
  orgUrl: string;
  schemaName: string;
  value: unknown;
  updatedAt: string;
}

export interface CompletionIndexEntry {
  cacheKey: string;
  orgUrl: string;
  value: unknown;
  updatedAt: string;
}

export interface PreferencesStore {
  load(): Promise<AppPreferences>;
  save(preferences: AppPreferences): Promise<void>;
  patch(update: Partial<AppPreferences>): Promise<AppPreferences>;
  reset(): Promise<AppPreferences>;
}

export interface ConnectionProfileStore {
  list(): Promise<DataverseConnectionProfile[]>;
  upsert(profile: DataverseConnectionProfile): Promise<void>;
  delete(profileId: string): Promise<void>;
  migrateFromLegacyProfiles(
    profiles: DataverseConnectionProfile[],
  ): Promise<DataverseConnectionProfile[]>;
}

export interface MetadataCacheStore {
  getEntities(orgUrl: string): Promise<CachedEntities | null>;
  saveEntities(orgUrl: string, entities: EntitySummary[]): Promise<void>;
  getAttributes(
    orgUrl: string,
    entityName: string,
  ): Promise<CachedAttributes | null>;
  saveAttributes(
    orgUrl: string,
    entityName: string,
    attributes: AttributeSummary[],
  ): Promise<void>;
  getSchema(cacheKey: string): Promise<SchemaCacheEntry | null>;
  saveSchema(entry: SchemaCacheEntry): Promise<void>;
  getCompletionIndex(cacheKey: string): Promise<CompletionIndexEntry | null>;
  saveCompletionIndex(entry: CompletionIndexEntry): Promise<void>;
  clearOrg(orgUrl: string): Promise<void>;
}

export interface WebStorageProvider {
  preferences: PreferencesStore;
  connectionProfiles: ConnectionProfileStore;
  metadataCache: MetadataCacheStore;
}

export const defaultPreferences: AppPreferences = {
  schemaVersion: 1,
  activeModule: "workbench",
  activePane: "builder",
  outputTab: "powerAutomate",
  activeConnectionProfileId: "",
  sidebarCollapsed: false,
  orgUrl: "",
  clientId: "",
  tenantId: "common",
};

const databaseName = "fetchxmlbuilder";
const databaseVersion = 1;
const preferencesStoreName = "preferences";
const connectionProfilesStoreName = "connectionProfiles";
const metadataEntitiesStoreName = "metadataEntities";
const metadataAttributesStoreName = "metadataAttributes";
const schemaCacheStoreName = "schemaCache";
const completionIndexesStoreName = "completionIndexes";
const preferencesId = "app";
const legacyConnectionProfilesStorageKey =
  "fetchxmlbuilder.dataverseCredentials.v1";

type StoreName =
  | typeof preferencesStoreName
  | typeof connectionProfilesStoreName
  | typeof metadataEntitiesStoreName
  | typeof metadataAttributesStoreName
  | typeof schemaCacheStoreName
  | typeof completionIndexesStoreName;

let databasePromise: Promise<IDBDatabase> | undefined;

export function createWebStorageProvider(): WebStorageProvider {
  return {
    preferences: new IndexedDbPreferencesStore(),
    connectionProfiles: new IndexedDbConnectionProfileStore(),
    metadataCache: new IndexedDbMetadataCacheStore(),
  };
}

export async function resetWebStorageDatabaseForTests() {
  const database = databasePromise ? await databasePromise : undefined;
  database?.close();
  databasePromise = undefined;
  await new Promise<void>((resolve, reject) => {
    const request = indexedDB.deleteDatabase(databaseName);
    request.onsuccess = () => resolve();
    request.onerror = () =>
      reject(request.error ?? new Error("Could not delete IndexedDB."));
    request.onblocked = () =>
      reject(new Error("Could not delete IndexedDB because it is blocked."));
  });
}

export function readLegacyConnectionProfiles(
  storage = globalThis.localStorage,
): DataverseConnectionProfile[] {
  if (!storage) return [];
  try {
    const rawProfiles = storage.getItem(legacyConnectionProfilesStorageKey);
    if (!rawProfiles) return [];
    const parsed = JSON.parse(rawProfiles);
    if (!Array.isArray(parsed)) return [];
    return parsed.flatMap((profile) => {
      const normalized = normalizeConnectionProfile(profile);
      return normalized ? [normalized] : [];
    });
  } catch {
    return [];
  }
}

export function clearLegacyConnectionProfiles(
  storage = globalThis.localStorage,
) {
  storage?.removeItem(legacyConnectionProfilesStorageKey);
}

export function migratePreferences(value: unknown): AppPreferences {
  if (!isRecord(value)) return { ...defaultPreferences };
  return {
    schemaVersion: 1,
    activeModule: isStoredAppModule(value.activeModule)
      ? value.activeModule
      : defaultPreferences.activeModule,
    activePane: isStoredWorkbenchPane(value.activePane)
      ? value.activePane
      : defaultPreferences.activePane,
    outputTab: isStoredOutputTab(value.outputTab)
      ? value.outputTab
      : defaultPreferences.outputTab,
    activeConnectionProfileId:
      typeof value.activeConnectionProfileId === "string"
        ? value.activeConnectionProfileId
        : typeof value.activeCredentialId === "string"
          ? value.activeCredentialId
          : defaultPreferences.activeConnectionProfileId,
    sidebarCollapsed:
      typeof value.sidebarCollapsed === "boolean"
        ? value.sidebarCollapsed
        : defaultPreferences.sidebarCollapsed,
    orgUrl:
      typeof value.orgUrl === "string"
        ? value.orgUrl
        : defaultPreferences.orgUrl,
    clientId:
      typeof value.clientId === "string"
        ? value.clientId
        : defaultPreferences.clientId,
    tenantId:
      typeof value.tenantId === "string" && value.tenantId.trim()
        ? value.tenantId
        : defaultPreferences.tenantId,
  };
}

export function normalizeConnectionProfile(
  value: unknown,
): DataverseConnectionProfile | null {
  if (!isRecord(value)) return null;
  if (
    typeof value.id !== "string" ||
    typeof value.name !== "string" ||
    typeof value.orgUrl !== "string"
  ) {
    return null;
  }
  return {
    id: value.id,
    name: value.name,
    orgUrl: value.orgUrl,
    clientId: typeof value.clientId === "string" ? value.clientId : "",
    tenantId:
      typeof value.tenantId === "string" && value.tenantId.trim()
        ? value.tenantId
        : "common",
    ...(value.lastTestedAt && typeof value.lastTestedAt === "string"
      ? { lastTestedAt: value.lastTestedAt }
      : {}),
    ...(value.lastTestStatus === "success" || value.lastTestStatus === "error"
      ? { lastTestStatus: value.lastTestStatus }
      : {}),
    ...(value.lastTestMessage && typeof value.lastTestMessage === "string"
      ? { lastTestMessage: value.lastTestMessage }
      : {}),
    updatedAt:
      typeof value.updatedAt === "string"
        ? value.updatedAt
        : new Date().toISOString(),
  };
}

export function normalizeCacheOrgUrl(orgUrl: string) {
  try {
    const parsed = new URL(orgUrl);
    parsed.pathname = "";
    parsed.search = "";
    parsed.hash = "";
    return parsed.toString().replace(/\/$/, "").toLowerCase();
  } catch {
    return orgUrl.trim().replace(/\/$/, "").toLowerCase();
  }
}

export function getMetadataAttributesCacheKey(
  orgUrl: string,
  entityName: string,
) {
  return `${normalizeCacheOrgUrl(orgUrl)}::${entityName.toLowerCase()}`;
}

class IndexedDbPreferencesStore implements PreferencesStore {
  async load() {
    const record = await getValue<{ id: string; value: unknown }>(
      preferencesStoreName,
      preferencesId,
    );
    return migratePreferences(record?.value);
  }

  async save(preferences: AppPreferences) {
    await putValue(preferencesStoreName, {
      id: preferencesId,
      value: migratePreferences(preferences),
    });
  }

  async patch(update: Partial<AppPreferences>) {
    const next = migratePreferences({ ...(await this.load()), ...update });
    await this.save(next);
    return next;
  }

  async reset() {
    await this.save(defaultPreferences);
    return { ...defaultPreferences };
  }
}

class IndexedDbConnectionProfileStore implements ConnectionProfileStore {
  async list() {
    const profiles = await getAllValues<DataverseConnectionProfile>(
      connectionProfilesStoreName,
    );
    return profiles
      .flatMap((profile) => {
        const normalized = normalizeConnectionProfile(profile);
        return normalized ? [normalized] : [];
      })
      .sort((left, right) => right.updatedAt.localeCompare(left.updatedAt));
  }

  async upsert(profile: DataverseConnectionProfile) {
    const normalized = normalizeConnectionProfile(profile);
    if (!normalized) return;
    await putValue(connectionProfilesStoreName, normalized);
  }

  async delete(profileId: string) {
    await deleteValue(connectionProfilesStoreName, profileId);
  }

  async migrateFromLegacyProfiles(profiles: DataverseConnectionProfile[]) {
    const existingProfiles = await this.list();
    const existingIds = new Set(existingProfiles.map((profile) => profile.id));
    for (const profile of profiles) {
      if (!existingIds.has(profile.id)) {
        await this.upsert(profile);
      }
    }
    return this.list();
  }
}

class IndexedDbMetadataCacheStore implements MetadataCacheStore {
  async getEntities(orgUrl: string) {
    return (
      (await getValue<CachedEntities>(
        metadataEntitiesStoreName,
        normalizeCacheOrgUrl(orgUrl),
      )) ?? null
    );
  }

  async saveEntities(orgUrl: string, entities: EntitySummary[]) {
    await putValue(metadataEntitiesStoreName, {
      orgUrl: normalizeCacheOrgUrl(orgUrl),
      entities,
      updatedAt: new Date().toISOString(),
    });
  }

  async getAttributes(orgUrl: string, entityName: string) {
    return (
      (await getValue<CachedAttributes>(
        metadataAttributesStoreName,
        getMetadataAttributesCacheKey(orgUrl, entityName),
      )) ?? null
    );
  }

  async saveAttributes(
    orgUrl: string,
    entityName: string,
    attributes: AttributeSummary[],
  ) {
    await putValue(metadataAttributesStoreName, {
      cacheKey: getMetadataAttributesCacheKey(orgUrl, entityName),
      orgUrl: normalizeCacheOrgUrl(orgUrl),
      entityName,
      attributes,
      updatedAt: new Date().toISOString(),
    });
  }

  async getSchema(cacheKey: string) {
    return (
      (await getValue<SchemaCacheEntry>(schemaCacheStoreName, cacheKey)) ?? null
    );
  }

  async saveSchema(entry: SchemaCacheEntry) {
    await putValue(schemaCacheStoreName, {
      ...entry,
      orgUrl: normalizeCacheOrgUrl(entry.orgUrl),
    });
  }

  async getCompletionIndex(cacheKey: string) {
    return (
      (await getValue<CompletionIndexEntry>(
        completionIndexesStoreName,
        cacheKey,
      )) ?? null
    );
  }

  async saveCompletionIndex(entry: CompletionIndexEntry) {
    await putValue(completionIndexesStoreName, {
      ...entry,
      orgUrl: normalizeCacheOrgUrl(entry.orgUrl),
    });
  }

  async clearOrg(orgUrl: string) {
    const normalizedOrgUrl = normalizeCacheOrgUrl(orgUrl);
    await deleteValue(metadataEntitiesStoreName, normalizedOrgUrl);
    await deleteByIndex(
      metadataAttributesStoreName,
      "orgUrl",
      normalizedOrgUrl,
    );
    await deleteByIndex(schemaCacheStoreName, "orgUrl", normalizedOrgUrl);
    await deleteByIndex(completionIndexesStoreName, "orgUrl", normalizedOrgUrl);
  }
}

async function openDatabase() {
  if (!databasePromise) {
    databasePromise = new Promise((resolve, reject) => {
      const request = indexedDB.open(databaseName, databaseVersion);
      request.onupgradeneeded = () => {
        const database = request.result;
        if (!database.objectStoreNames.contains(preferencesStoreName)) {
          database.createObjectStore(preferencesStoreName, { keyPath: "id" });
        }
        if (!database.objectStoreNames.contains(connectionProfilesStoreName)) {
          database.createObjectStore(connectionProfilesStoreName, {
            keyPath: "id",
          });
        }
        if (!database.objectStoreNames.contains(metadataEntitiesStoreName)) {
          database.createObjectStore(metadataEntitiesStoreName, {
            keyPath: "orgUrl",
          });
        }
        if (!database.objectStoreNames.contains(metadataAttributesStoreName)) {
          const store = database.createObjectStore(
            metadataAttributesStoreName,
            {
              keyPath: "cacheKey",
            },
          );
          store.createIndex("orgUrl", "orgUrl", { unique: false });
        }
        if (!database.objectStoreNames.contains(schemaCacheStoreName)) {
          const store = database.createObjectStore(schemaCacheStoreName, {
            keyPath: "cacheKey",
          });
          store.createIndex("orgUrl", "orgUrl", { unique: false });
        }
        if (!database.objectStoreNames.contains(completionIndexesStoreName)) {
          const store = database.createObjectStore(completionIndexesStoreName, {
            keyPath: "cacheKey",
          });
          store.createIndex("orgUrl", "orgUrl", { unique: false });
        }
      };
      request.onsuccess = () => resolve(request.result);
      request.onerror = () =>
        reject(request.error ?? new Error("Could not open IndexedDB."));
    });
  }
  return databasePromise;
}

async function withStore<T>(
  storeName: StoreName,
  mode: IDBTransactionMode,
  callback: (store: IDBObjectStore) => IDBRequest<T> | undefined,
) {
  const database = await openDatabase();
  return new Promise<T>((resolve, reject) => {
    const transaction = database.transaction(storeName, mode);
    const store = transaction.objectStore(storeName);
    const request = callback(store);
    transaction.oncomplete = () => resolve(request?.result as T);
    transaction.onerror = () =>
      reject(transaction.error ?? new Error(`IndexedDB ${storeName} failed.`));
    transaction.onabort = () =>
      reject(transaction.error ?? new Error(`IndexedDB ${storeName} aborted.`));
  });
}

async function getValue<T>(storeName: StoreName, key: IDBValidKey) {
  return withStore<T | undefined>(storeName, "readonly", (store) =>
    store.get(key),
  );
}

async function getAllValues<T>(storeName: StoreName) {
  return withStore<T[]>(storeName, "readonly", (store) => store.getAll());
}

async function putValue<T extends object>(storeName: StoreName, value: T) {
  await withStore<IDBValidKey>(storeName, "readwrite", (store) =>
    store.put(value),
  );
}

async function deleteValue(storeName: StoreName, key: IDBValidKey) {
  await withStore<undefined>(storeName, "readwrite", (store) =>
    store.delete(key),
  );
}

async function deleteByIndex(
  storeName: StoreName,
  indexName: string,
  key: IDBValidKey,
) {
  const database = await openDatabase();
  await new Promise<void>((resolve, reject) => {
    const transaction = database.transaction(storeName, "readwrite");
    const store = transaction.objectStore(storeName);
    const index = store.index(indexName);
    const request = index.openKeyCursor(IDBKeyRange.only(key));
    request.onsuccess = () => {
      const cursor = request.result;
      if (!cursor) return;
      store.delete(cursor.primaryKey);
      cursor.continue();
    };
    transaction.oncomplete = () => resolve();
    transaction.onerror = () =>
      reject(transaction.error ?? new Error(`IndexedDB ${storeName} failed.`));
    transaction.onabort = () =>
      reject(transaction.error ?? new Error(`IndexedDB ${storeName} aborted.`));
  });
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

function isStoredAppModule(value: unknown): value is StoredAppModule {
  return value === "workbench" || value === "credentials";
}

function isStoredWorkbenchPane(value: unknown): value is StoredWorkbenchPane {
  return (
    value === "editor" ||
    value === "builder" ||
    value === "metadata" ||
    value === "results"
  );
}

function isStoredOutputTab(value: unknown): value is StoredOutputTab {
  return (
    value === "powerAutomate" ||
    value === "odata" ||
    value === "csharp" ||
    value === "javascript" ||
    value === "validation"
  );
}
