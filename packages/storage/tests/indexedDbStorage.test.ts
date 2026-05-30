import "fake-indexeddb/auto";
import { afterEach, describe, expect, it } from "vitest";
import {
  createWebStorageProvider,
  defaultPreferences,
  migratePreferences,
  readLegacyConnectionProfiles,
  resetWebStorageDatabaseForTests,
} from "../src/index";

describe("web storage provider", () => {
  const legacyStorage = createMemoryStorage();

  afterEach(async () => {
    legacyStorage.clear();
    await resetWebStorageDatabaseForTests();
  });

  it("falls back to default preferences for corrupt data", () => {
    expect(migratePreferences(null)).toEqual(defaultPreferences);
    expect(migratePreferences({ activePane: "nope" })).toEqual({
      ...defaultPreferences,
    });
  });

  it("persists preference patches", async () => {
    const storage = createWebStorageProvider();

    const preferences = await storage.preferences.patch({
      activePane: "metadata",
      outputTab: "odata",
      sidebarCollapsed: true,
    });

    expect(preferences).toMatchObject({
      activePane: "metadata",
      outputTab: "odata",
      sidebarCollapsed: true,
    });
    await expect(storage.preferences.load()).resolves.toMatchObject({
      activePane: "metadata",
      outputTab: "odata",
      sidebarCollapsed: true,
    });
  });

  it("reads legacy connection profiles and migrates them into IndexedDB", async () => {
    legacyStorage.setItem(
      "fetchxmlbuilder.dataverseCredentials.v1",
      JSON.stringify([
        {
          id: "profile-1",
          name: "Production",
          orgUrl: "https://example.crm.dynamics.com",
          clientId: "client-id",
          tenantId: "tenant-id",
          updatedAt: "2026-05-01T00:00:00.000Z",
        },
        { id: 42 },
      ]),
    );
    const legacyProfiles = readLegacyConnectionProfiles(legacyStorage);
    const storage = createWebStorageProvider();

    const profiles =
      await storage.connectionProfiles.migrateFromLegacyProfiles(
        legacyProfiles,
      );

    expect(profiles).toEqual([
      {
        id: "profile-1",
        name: "Production",
        orgUrl: "https://example.crm.dynamics.com",
        clientId: "client-id",
        tenantId: "tenant-id",
        updatedAt: "2026-05-01T00:00:00.000Z",
      },
    ]);
  });

  it("persists metadata entities and attributes by org", async () => {
    const storage = createWebStorageProvider();

    await storage.metadataCache.saveEntities("https://Org.crm.dynamics.com/", [
      {
        logicalName: "account",
        displayName: "Account",
        entitySetName: "accounts",
      },
    ]);
    await storage.metadataCache.saveAttributes(
      "https://org.crm.dynamics.com",
      "account",
      [{ logicalName: "name", displayName: "Name", type: "String" }],
    );

    await expect(
      storage.metadataCache.getEntities("https://org.crm.dynamics.com"),
    ).resolves.toMatchObject({
      orgUrl: "https://org.crm.dynamics.com",
      entities: [
        {
          logicalName: "account",
          displayName: "Account",
          entitySetName: "accounts",
        },
      ],
    });
    await expect(
      storage.metadataCache.getAttributes(
        "https://org.crm.dynamics.com/",
        "account",
      ),
    ).resolves.toMatchObject({
      orgUrl: "https://org.crm.dynamics.com",
      entityName: "account",
      attributes: [
        { logicalName: "name", displayName: "Name", type: "String" },
      ],
    });
  });
});

function createMemoryStorage(): Storage {
  const values = new Map<string, string>();
  return {
    get length() {
      return values.size;
    },
    clear() {
      values.clear();
    },
    getItem(key) {
      return values.get(key) ?? null;
    },
    key(index) {
      return [...values.keys()][index] ?? null;
    },
    removeItem(key) {
      values.delete(key);
    },
    setItem(key, value) {
      values.set(key, value);
    },
  };
}
