import {
  type FetchQueryModel,
  readFetchQueryModel,
  writeFetchQueryModel,
} from "@fetchxmlbuilder/core";
import {
  DEFAULT_DATAVERSE_CLIENT_ID,
  type DataverseConnectionConfig,
  type DataverseSession,
  createDataverseSession,
} from "@fetchxmlbuilder/dataverse";
import type { DataverseConnectionProfile } from "@fetchxmlbuilder/storage";
import {
  AlertCircle,
  Blocks,
  CheckCircle2,
  ChevronsLeft,
  ChevronsRight,
  Database,
  Download,
  FileUp,
  KeyRound,
  LogOut,
  Play,
  PlugZap,
  RotateCcw,
  Sparkles,
  UserRound,
} from "lucide-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { CredentialsManager } from "./components/CredentialsManager";
import { MetadataBrowser } from "./components/MetadataBrowser";
import { OutputPanel, getFormattedXml } from "./components/OutputPanel";
import { QueryBuilderPanel } from "./components/QueryBuilderPanel";
import { ResultGrid } from "./components/ResultGrid";
import { XmlEditor } from "./components/XmlEditor";
import {
  type AppModule,
  type ConnectionStatus,
  type WorkbenchPane,
  sampleFetchXml,
  useWorkbenchStore,
} from "./store/workbenchStore";

const modules: Array<{
  id: AppModule;
  label: string;
  icon: typeof Sparkles;
}> = [
  { id: "workbench", label: "Workbench", icon: Sparkles },
  { id: "credentials", label: "Connections", icon: KeyRound },
];

const panes: Array<{
  id: WorkbenchPane;
  label: string;
  icon: typeof Sparkles;
}> = [
  { id: "editor", label: "Editor", icon: Sparkles },
  { id: "builder", label: "Builder", icon: Blocks },
  { id: "metadata", label: "Metadata", icon: Database },
  { id: "results", label: "Results", icon: Play },
];

export function App() {
  const {
    fetchXml,
    outputTab,
    activeModule,
    activePane,
    sidebarCollapsed,
    orgUrl,
    clientId,
    tenantId,
    connectionProfiles,
    activeConnectionProfileId,
    resultRows,
    connectionStatus,
    connectionError,
    userName,
    metadataEntities,
    metadataAttributesByEntity,
    loadingAttributeEntity,
    setFetchXml,
    setOutputTab,
    setActiveModule,
    setActivePane,
    setSidebarCollapsed,
    hydrateStoredState,
    loadCachedMetadata,
    upsertConnectionProfile,
    deleteConnectionProfile,
    setConnectionProfileTestResult,
    useConnectionProfile,
    setResultRows,
    setConnectionStatus,
    setConnectedUser,
    setMetadataEntities,
    setEntityAttributes,
    setLoadingAttributeEntity,
    clearLiveConnection,
  } = useWorkbenchStore();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const dataverseSessionRef = useRef<DataverseSession | null>(null);
  const [testingCredentialId, setTestingCredentialId] = useState("");
  const [hasLiveSession, setHasLiveSession] = useState(false);
  const [isExecutingQuery, setIsExecutingQuery] = useState(false);
  const [queryError, setQueryError] = useState("");
  const selectedEntity = safeReadModel(fetchXml).entity;
  const activeConnectionProfile = connectionProfiles.find(
    (profile) => profile.id === activeConnectionProfileId,
  );

  useEffect(() => {
    void hydrateStoredState();
  }, [hydrateStoredState]);

  const loadEntityAttributes = useCallback(
    async (
      entityName: string,
      client = dataverseSessionRef.current?.client,
    ) => {
      if (metadataAttributesByEntity[entityName]) {
        return metadataAttributesByEntity[entityName];
      }
      if (!client) return [];
      setLoadingAttributeEntity(entityName);
      try {
        const attributes = await client.listAttributes(entityName);
        const sortedAttributes = attributes
          .filter((attribute) => attribute.logicalName)
          .sort((left, right) =>
            left.logicalName.localeCompare(right.logicalName),
          );
        setEntityAttributes(entityName, sortedAttributes);
        return sortedAttributes;
      } catch (error) {
        setConnectionStatus("error", getErrorMessage(error));
        return [];
      } finally {
        setLoadingAttributeEntity("");
      }
    },
    [
      metadataAttributesByEntity,
      setConnectionStatus,
      setEntityAttributes,
      setLoadingAttributeEntity,
    ],
  );

  useEffect(() => {
    if (connectionStatus !== "connected") return;
    void loadEntityAttributes(selectedEntity);
  }, [connectionStatus, loadEntityAttributes, selectedEntity]);

  async function connectToDataverse() {
    try {
      setConnectionStatus("connecting");
      await loadCachedMetadata(orgUrl, selectedEntity);
      const session = await createDataverseSession({
        organizationUrl: orgUrl,
        clientId: getDataverseClientId(clientId),
        tenantId,
      });
      dataverseSessionRef.current = session;
      setHasLiveSession(true);
      setQueryError("");
      setConnectedUser(session.account.username || session.account.name || "");
      setConnectionStatus("loadingMetadata");

      const entities = await session.client.listEntities();
      setMetadataEntities(
        entities
          .filter((entity) => entity.logicalName)
          .sort((left, right) =>
            left.logicalName.localeCompare(right.logicalName),
          ),
      );
      await loadEntityAttributes(selectedEntity, session.client);
      setConnectionStatus("connected");
      setActivePane("metadata");
    } catch (error) {
      dataverseSessionRef.current = null;
      setHasLiveSession(false);
      setConnectionStatus("error", getErrorMessage(error));
    }
  }

  async function testCredential(profile: DataverseConnectionProfile) {
    setTestingCredentialId(profile.id);
    upsertConnectionProfile(profile);
    try {
      const session = await createDataverseSession(
        getConnectionConfig(profile),
      );
      const entities = await session.client.listEntities();
      setConnectionProfileTestResult(
        profile.id,
        "success",
        `${entities.length} entities loaded`,
      );
    } catch (error) {
      setConnectionProfileTestResult(
        profile.id,
        "error",
        getErrorMessage(error),
      );
    } finally {
      setTestingCredentialId("");
    }
  }

  function useSavedCredential(profileId: string) {
    const profile = connectionProfiles.find(
      (connectionProfile) => connectionProfile.id === profileId,
    );
    useConnectionProfile(profileId);
    if (profile) {
      void loadCachedMetadata(profile.orgUrl, selectedEntity);
    }
    setActiveModule("workbench");
  }

  function disconnectFromDataverse() {
    dataverseSessionRef.current = null;
    setHasLiveSession(false);
    setQueryError("");
    clearLiveConnection();
  }

  async function selectEntity(entityName: string) {
    const attributes = await loadEntityAttributes(entityName);
    const model = safeReadModel(fetchXml);
    setFetchXml(
      writeFetchQueryModel({
        ...model,
        entity: entityName,
        attributes: attributes.slice(0, 3).map((attribute) => ({
          name: attribute.logicalName,
        })),
        conditions: [],
        orders: [],
        links: [],
      }),
    );
  }

  async function executeQuery() {
    const model = safeReadModel(fetchXml);
    setQueryError("");
    if (!dataverseSessionRef.current?.client) {
      setConnectionStatus(
        "error",
        "Connect to Dataverse before executing FetchXML.",
      );
      return;
    }

    const liveEntity = metadataEntities.find(
      (entity) => entity.logicalName === model.entity,
    );
    const entitySetName = liveEntity?.entitySetName;

    if (!entitySetName) {
      setQueryError(
        `Entity "${model.entity}" was not found in loaded Dataverse metadata.`,
      );
      return;
    }

    setIsExecutingQuery(true);
    try {
      const result = await dataverseSessionRef.current.client.executeFetchXml(
        entitySetName,
        fetchXml,
      );
      setResultRows(result.rows);
      setConnectionStatus("connected");
      setActivePane("results");
    } catch (error) {
      setQueryError(getErrorMessage(error));
      setConnectionStatus("connected");
    } finally {
      setIsExecutingQuery(false);
    }
  }

  return (
    <main className="app-shell">
      <header className="topbar">
        <div className="brand">
          <img src="/fxb-icon.png" alt="" />
          <div>
            <h1>FetchXML Builder</h1>
            <span>Web Workbench</span>
          </div>
        </div>
        <div className="topbar-actions">
          <input
            accept=".xml,.fetch,.txt"
            hidden
            ref={fileInputRef}
            type="file"
            onChange={async (event) => {
              const file = event.target.files?.[0];
              if (file) setFetchXml(await file.text());
              event.target.value = "";
            }}
          />
          <button
            type="button"
            title="Import XML"
            onClick={() => fileInputRef.current?.click()}
          >
            <FileUp size={17} />
          </button>
          <button
            type="button"
            title="Format"
            onClick={() => setFetchXml(getFormattedXml(fetchXml))}
          >
            <Sparkles size={17} />
            <span>Format</span>
          </button>
          <button
            type="button"
            title="Reset"
            onClick={() => {
              setFetchXml(sampleFetchXml);
              setResultRows([]);
            }}
          >
            <RotateCcw size={17} />
          </button>
          <button
            type="button"
            title="Export XML"
            onClick={() => download("query.fetch.xml", fetchXml)}
          >
            <Download size={17} />
          </button>
          <button
            type="button"
            className="primary-action"
            title="Execute"
            disabled={!hasLiveSession || isExecutingQuery}
            onClick={executeQuery}
          >
            <Play size={17} />
            <span>{isExecutingQuery ? "Running" : "Execute"}</span>
          </button>
        </div>
      </header>

      <div
        className={sidebarCollapsed ? "app-body sidebar-collapsed" : "app-body"}
      >
        <aside className="app-sidebar" aria-label="Application modules">
          <button
            className="sidebar-toggle"
            type="button"
            title={sidebarCollapsed ? "Expand sidebar" : "Collapse sidebar"}
            onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
          >
            {sidebarCollapsed ? (
              <ChevronsRight size={18} />
            ) : (
              <ChevronsLeft size={18} />
            )}
            <span>Collapse</span>
          </button>
          <nav>
            {modules.map((module) => {
              const Icon = module.icon;
              return (
                <button
                  className={activeModule === module.id ? "active" : ""}
                  key={module.id}
                  title={module.label}
                  type="button"
                  onClick={() => setActiveModule(module.id)}
                >
                  <Icon size={18} />
                  <span>{module.label}</span>
                </button>
              );
            })}
          </nav>
        </aside>

        {activeModule === "workbench" ? (
          <div className="module-body">
            <ConnectionStrip
              activeConnectionProfile={activeConnectionProfile}
              error={connectionError || queryError}
              orgUrl={orgUrl}
              status={connectionStatus}
              userName={userName}
              onConnect={connectToDataverse}
              onDisconnect={disconnectFromDataverse}
              onManageCredentials={() => setActiveModule("credentials")}
            />
            <nav className="pane-tabs" aria-label="Workbench views">
              {panes.map((pane) => {
                const Icon = pane.icon;
                return (
                  <button
                    className={activePane === pane.id ? "active" : ""}
                    key={pane.id}
                    type="button"
                    onClick={() => setActivePane(pane.id)}
                  >
                    <Icon size={16} />
                    <span>{pane.label}</span>
                  </button>
                );
              })}
            </nav>

            <div
              className={
                activePane === "builder"
                  ? "workspace builder-focused"
                  : "workspace"
              }
            >
              <div className="left-stack">
                {activePane === "editor" ? (
                  <section
                    className="panel editor-panel"
                    aria-label="FetchXML editor"
                  >
                    <XmlEditor value={fetchXml} onChange={setFetchXml} />
                  </section>
                ) : null}
                {activePane === "builder" ? (
                  <QueryBuilderPanel
                    attributesByEntity={metadataAttributesByEntity}
                    entities={metadataEntities}
                    fetchXml={fetchXml}
                    loadingAttributeEntity={loadingAttributeEntity}
                    onChange={setFetchXml}
                    onEntitySelected={(entityName) =>
                      void loadEntityAttributes(entityName)
                    }
                  />
                ) : null}
                {activePane === "metadata" ? (
                  <MetadataBrowser
                    attributesByEntity={metadataAttributesByEntity}
                    entities={metadataEntities}
                    loadingAttributeEntity={loadingAttributeEntity}
                    selectedEntity={selectedEntity}
                    onEntitySelected={selectEntity}
                  />
                ) : null}
                {activePane === "results" ? (
                  <ResultGrid
                    canExecute={hasLiveSession}
                    isExecuting={isExecutingQuery}
                    rows={resultRows}
                    onExecute={executeQuery}
                  />
                ) : null}
              </div>
              <OutputPanel
                fetchXml={fetchXml}
                outputTab={outputTab}
                setOutputTab={setOutputTab}
              />
            </div>
          </div>
        ) : (
          <CredentialsManager
            activeConnectionProfileId={activeConnectionProfileId}
            connectionProfiles={connectionProfiles}
            testingCredentialId={testingCredentialId}
            onDelete={deleteConnectionProfile}
            onSave={upsertConnectionProfile}
            onTest={(credential) => void testCredential(credential)}
            onUse={useSavedCredential}
          />
        )}
      </div>
    </main>
  );
}

function ConnectionStrip({
  activeConnectionProfile,
  error,
  orgUrl,
  status,
  userName,
  onConnect,
  onDisconnect,
  onManageCredentials,
}: {
  activeConnectionProfile: DataverseConnectionProfile | undefined;
  error: string;
  orgUrl: string;
  status: ConnectionStatus;
  userName: string;
  onConnect: () => void;
  onDisconnect: () => void;
  onManageCredentials: () => void;
}) {
  const isBusy = status === "connecting" || status === "loadingMetadata";
  const isConnected = status === "connected" || status === "loadingMetadata";
  const hasConnectionConfig = Boolean(orgUrl.trim());
  const StatusIcon = status === "error" ? AlertCircle : CheckCircle2;
  const summary =
    userName ||
    activeConnectionProfile?.name ||
    activeConnectionProfile?.orgUrl ||
    orgUrl ||
    "Not connected";

  return (
    <section
      className={
        status === "error"
          ? "connection-strip error"
          : isConnected
            ? "connection-strip ready"
            : "connection-strip"
      }
      aria-label="Connection status"
    >
      <div className="connection-strip-status">
        <StatusIcon size={16} />
        <span className="status-pill-label">{statusLabel(status)}</span>
      </div>
      <div className="connection-strip-summary">
        <UserRound size={15} />
        <span>{error || summary}</span>
      </div>
      <div className="connection-strip-actions">
        <button type="button" onClick={onManageCredentials}>
          <KeyRound size={15} />
          <span>Connections</span>
        </button>
        {isConnected ? (
          <button type="button" onClick={onDisconnect}>
            <LogOut size={15} />
            <span>Disconnect</span>
          </button>
        ) : (
          <button
            className="primary-action"
            type="button"
            disabled={isBusy || !hasConnectionConfig}
            onClick={onConnect}
          >
            <PlugZap size={15} />
            <span>{isBusy ? "Connecting" : "Connect"}</span>
          </button>
        )}
      </div>
    </section>
  );
}

function statusLabel(status: ConnectionStatus) {
  if (status === "connecting") return "Signing in";
  if (status === "loadingMetadata") return "Loading metadata";
  if (status === "connected") return "Connected";
  if (status === "error") return "Connection error";
  return "Local";
}

function getErrorMessage(error: unknown) {
  return error instanceof Error ? error.message : "Unexpected Dataverse error.";
}

function safeReadModel(fetchXml: string): FetchQueryModel {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return {
      entity: "account",
      top: "50",
      distinct: false,
      filterType: "and",
      attributes: [{ name: "name" }],
      conditions: [],
      orders: [],
      links: [],
    };
  }
}

function download(fileName: string, content: string) {
  const blob = new Blob([content], { type: "application/xml" });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = fileName;
  anchor.click();
  URL.revokeObjectURL(url);
}

function getDataverseClientId(clientId: string) {
  return (
    clientId.trim() ||
    import.meta.env.VITE_DATAVERSE_CLIENT_ID?.trim() ||
    DEFAULT_DATAVERSE_CLIENT_ID
  );
}

function getConnectionConfig(
  credential: DataverseConnectionProfile,
): DataverseConnectionConfig {
  return {
    organizationUrl: credential.orgUrl,
    clientId: getDataverseClientId(credential.clientId),
    tenantId: credential.tenantId,
  };
}
