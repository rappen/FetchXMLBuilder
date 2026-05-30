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
  Copy,
  Database,
  Download,
  FileJson2,
  FileUp,
  KeyRound,
  LogOut,
  Play,
  PlugZap,
  RefreshCw,
  RotateCcw,
  Sparkles,
  Wand2,
  X,
} from "lucide-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { CredentialsManager } from "./components/CredentialsManager";
import { MetadataBrowser } from "./components/MetadataBrowser";
import {
  getFormattedXml,
  getOutput,
  outputTabs,
} from "./components/OutputPanel";
import { QueryBuilderPanel } from "./components/QueryBuilderPanel";
import { ResultGrid } from "./components/ResultGrid";
import { XmlEditor } from "./components/XmlEditor";
import {
  type AppModule,
  type ConnectionStatus,
  type OutputTab,
  type WorkbenchPane,
  sampleFetchXml,
  useWorkbenchStore,
} from "./store/workbenchStore";

const sidebarItems: Array<
  | {
      type: "pane";
      id: WorkbenchPane;
      label: string;
      icon: typeof Sparkles;
    }
  | {
      type: "module";
      id: AppModule;
      label: string;
      icon: typeof Sparkles;
    }
> = [
  { type: "pane", id: "builder", label: "Builder", icon: Blocks },
  { type: "pane", id: "editor", label: "Editor", icon: Sparkles },
  { type: "pane", id: "metadata", label: "Metadata", icon: Database },
  { type: "pane", id: "results", label: "Results", icon: Play },
  { type: "module", id: "credentials", label: "Connections", icon: KeyRound },
];

export function App() {
  const {
    fetchXml,
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
    metadataUpdatedAt,
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
  const [isTransformMenuOpen, setIsTransformMenuOpen] = useState(false);
  const [transformDialogTab, setTransformDialogTab] =
    useState<OutputTab | null>(null);
  const [builderWarningCount, setBuilderWarningCount] = useState(0);
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
    if (builderWarningCount > 0) {
      setQueryError("Resolve builder warnings before executing FetchXML.");
      return;
    }
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

  async function reloadMetadata() {
    const client = dataverseSessionRef.current?.client;
    if (!client) {
      setConnectionStatus(
        "error",
        "Connect to Dataverse before reloading metadata.",
      );
      return;
    }

    setConnectionStatus("loadingMetadata");
    try {
      const entities = await client.listEntities();
      setMetadataEntities(
        entities
          .filter((entity) => entity.logicalName)
          .sort((left, right) =>
            left.logicalName.localeCompare(right.logicalName),
          ),
      );
      await loadEntityAttributes(selectedEntity, client);
      setConnectionStatus("connected");
    } catch (error) {
      setConnectionStatus("error", getErrorMessage(error));
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
          <div className="topbar-menu">
            <button
              type="button"
              title="Transform"
              onClick={() => setIsTransformMenuOpen(!isTransformMenuOpen)}
            >
              <Wand2 size={17} />
              <span>Transform</span>
            </button>
            {isTransformMenuOpen ? (
              <div className="transform-menu" role="menu">
                {outputTabs.map((tab) => {
                  const Icon = tab.icon;
                  return (
                    <button
                      key={tab.id}
                      type="button"
                      role="menuitem"
                      onClick={() => {
                        setOutputTab(tab.id);
                        setTransformDialogTab(tab.id);
                        setIsTransformMenuOpen(false);
                      }}
                    >
                      <Icon size={16} />
                      <span>{tab.label}</span>
                    </button>
                  );
                })}
              </div>
            ) : null}
          </div>
          <button
            type="button"
            title="Reload metadata"
            disabled={!hasLiveSession || connectionStatus === "loadingMetadata"}
            onClick={() => void reloadMetadata()}
          >
            <RefreshCw size={17} />
            <span>Reload</span>
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
            title={
              builderWarningCount
                ? "Resolve builder warnings before executing"
                : "Execute"
            }
            disabled={
              !hasLiveSession || isExecutingQuery || builderWarningCount > 0
            }
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
            {sidebarItems.map((item) => {
              const Icon = item.icon;
              const isActive =
                item.type === "module"
                  ? activeModule === item.id
                  : activeModule === "workbench" && activePane === item.id;
              return (
                <button
                  className={isActive ? "active" : ""}
                  key={`${item.type}-${item.id}`}
                  title={item.label}
                  type="button"
                  onClick={() => {
                    if (item.type === "module") {
                      setActiveModule(item.id);
                      return;
                    }
                    setActiveModule("workbench");
                    setActivePane(item.id);
                  }}
                >
                  <Icon size={18} />
                  <span>{item.label}</span>
                </button>
              );
            })}
          </nav>
        </aside>

        {activeModule === "workbench" ? (
          <div className="module-body">
            <div className="workspace">
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
                    onWarningsChange={setBuilderWarningCount}
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
                    canExecute={hasLiveSession && builderWarningCount === 0}
                    isExecuting={isExecutingQuery}
                    rows={resultRows}
                    onExecute={executeQuery}
                  />
                ) : null}
              </div>
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
      <ConnectionStrip
        activeConnectionProfile={activeConnectionProfile}
        entityCount={metadataEntities.length}
        error={connectionError || queryError}
        metadataUpdatedAt={metadataUpdatedAt}
        orgUrl={orgUrl}
        status={connectionStatus}
        userName={userName}
        onConnect={connectToDataverse}
        onDisconnect={disconnectFromDataverse}
        onManageCredentials={() => setActiveModule("credentials")}
      />
      {transformDialogTab ? (
        <TransformDialog
          fetchXml={fetchXml}
          selectedTab={transformDialogTab}
          onClose={() => setTransformDialogTab(null)}
          onSelect={(tab) => {
            setOutputTab(tab);
            setTransformDialogTab(tab);
          }}
        />
      ) : null}
    </main>
  );
}

function TransformDialog({
  fetchXml,
  selectedTab,
  onClose,
  onSelect,
}: {
  fetchXml: string;
  selectedTab: OutputTab;
  onClose: () => void;
  onSelect: (tab: OutputTab) => void;
}) {
  const selected = outputTabs.find((tab) => tab.id === selectedTab);
  const output = getOutput(fetchXml, selectedTab);
  const copyText =
    output.kind === "issues"
      ? output.issues
          .map((issue) => `${issue.severity}: ${issue.message} (${issue.path})`)
          .join("\n")
      : output.text;

  return (
    <div className="modal-backdrop" role="presentation">
      <dialog
        className="transform-dialog"
        aria-label="Transformation result"
        open
      >
        <div className="panel-heading builder-heading">
          <div>
            <h2>{selected?.label ?? "Transformation"}</h2>
            <span>Generated from the current FetchXML</span>
          </div>
          <div className="dialog-actions">
            <button
              className="icon-button"
              type="button"
              title="Copy"
              onClick={() => navigator.clipboard.writeText(copyText)}
            >
              <Copy size={17} />
            </button>
            <button
              className="icon-button"
              type="button"
              title="Close"
              onClick={onClose}
            >
              <X size={17} />
            </button>
          </div>
        </div>
        <div className="panel-toolbar">
          <div
            className="segmented"
            role="tablist"
            aria-label="Transformations"
          >
            {outputTabs.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  className={tab.id === selectedTab ? "active" : ""}
                  key={tab.id}
                  type="button"
                  title={tab.label}
                  onClick={() => onSelect(tab.id)}
                >
                  <Icon size={16} />
                  <span>{tab.label}</span>
                </button>
              );
            })}
          </div>
        </div>
        {output.kind === "parameters" ? (
          <div className="parameter-layout">
            <pre>{output.text}</pre>
            <div className="manifest">
              {output.parameters.map((parameter) => (
                <div className="manifest-row" key={parameter.name}>
                  <FileJson2 size={15} />
                  <span>{parameter.name}</span>
                  <small>{parameter.inferredType}</small>
                </div>
              ))}
            </div>
          </div>
        ) : output.kind === "issues" ? (
          <div className="issues">
            {output.issues.length === 0 ? (
              <div className="empty-state">No validation issues</div>
            ) : (
              output.issues.map((issue) => (
                <div
                  className={`issue ${issue.severity}`}
                  key={`${issue.path}-${issue.message}`}
                >
                  <AlertCircle size={16} />
                  <span>{issue.message}</span>
                  <small>{issue.path}</small>
                </div>
              ))
            )}
          </div>
        ) : (
          <pre>{output.text}</pre>
        )}
      </dialog>
    </div>
  );
}

function ConnectionStrip({
  activeConnectionProfile,
  entityCount,
  error,
  metadataUpdatedAt,
  orgUrl,
  status,
  userName,
  onConnect,
  onDisconnect,
  onManageCredentials,
}: {
  activeConnectionProfile: DataverseConnectionProfile | undefined;
  entityCount: number;
  error: string;
  metadataUpdatedAt: string;
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
  const profileName = activeConnectionProfile?.name || "Unsaved connection";

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
        <div className="connection-property">
          <span>Connection</span>
          <strong>{error || summary}</strong>
        </div>
        <div className="connection-property">
          <span>Profile</span>
          <strong>{profileName}</strong>
        </div>
        <div className="connection-property metadata-property">
          <span>Metadata</span>
          <strong>
            {entityCount
              ? `${entityCount.toLocaleString()} entities cached. Last updated ${formatTimestamp(
                  metadataUpdatedAt,
                )}.`
              : "No metadata cached"}
          </strong>
        </div>
      </div>
      <div className="connection-strip-actions">
        <button type="button" onClick={onManageCredentials}>
          <KeyRound size={15} />
          <span>Change</span>
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

function formatTimestamp(value: string) {
  if (!value) return "never";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "unknown";
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
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
