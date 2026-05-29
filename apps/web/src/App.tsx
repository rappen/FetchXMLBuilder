import {
  readFetchQueryModel,
  writeFetchQueryModel,
} from "@fetchxmlbuilder/core";
import {
  DEFAULT_DATAVERSE_CLIENT_ID,
  type DataverseSession,
  createDataverseSession,
} from "@fetchxmlbuilder/dataverse";
import {
  Blocks,
  Database,
  Download,
  FileUp,
  Play,
  RotateCcw,
  Sparkles,
} from "lucide-react";
import { useCallback, useEffect, useRef } from "react";
import { ConnectionPanel } from "./components/ConnectionPanel";
import { MetadataBrowser } from "./components/MetadataBrowser";
import { OutputPanel, getFormattedXml } from "./components/OutputPanel";
import { QueryBuilderPanel } from "./components/QueryBuilderPanel";
import { ResultGrid } from "./components/ResultGrid";
import { XmlEditor } from "./components/XmlEditor";
import { makeMockRows } from "./data/mockMetadata";
import {
  type WorkbenchPane,
  sampleFetchXml,
  useWorkbenchStore,
} from "./store/workbenchStore";

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
    activePane,
    orgUrl,
    clientId,
    tenantId,
    resultRows,
    connectionStatus,
    connectionError,
    userName,
    metadataEntities,
    metadataAttributesByEntity,
    loadingAttributeEntity,
    setFetchXml,
    setOutputTab,
    setActivePane,
    setConnectionField,
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
  const selectedEntity = safeReadModel(fetchXml).entity;

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
      const session = await createDataverseSession({
        organizationUrl: orgUrl,
        clientId: getDataverseClientId(clientId),
        tenantId,
      });
      dataverseSessionRef.current = session;
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
      setConnectionStatus("error", getErrorMessage(error));
    }
  }

  function disconnectFromDataverse() {
    dataverseSessionRef.current = null;
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
    const liveEntity = metadataEntities.find(
      (entity) => entity.logicalName === model.entity,
    );
    const entitySetName = liveEntity?.entitySetName;

    if (dataverseSessionRef.current?.client && entitySetName) {
      try {
        const result = await dataverseSessionRef.current.client.executeFetchXml(
          entitySetName,
          fetchXml,
        );
        setResultRows(result.rows);
        setActivePane("results");
      } catch (error) {
        setConnectionStatus("error", getErrorMessage(error));
      }
      return;
    }

    setResultRows(
      makeMockRows(
        model.entity,
        model.attributes.map((attribute) => attribute.name),
      ),
    );
    setActivePane("results");
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
            onClick={executeQuery}
          >
            <Play size={17} />
            <span>Execute</span>
          </button>
        </div>
      </header>

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

      <div className="workspace">
        <div className="left-stack">
          <ConnectionPanel
            clientId={clientId}
            error={connectionError}
            orgUrl={orgUrl}
            redirectUri={globalThis.location.origin}
            status={connectionStatus}
            tenantId={tenantId}
            userName={userName}
            onFieldChange={setConnectionField}
            onConnect={connectToDataverse}
            onDisconnect={disconnectFromDataverse}
          />
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
              fetchXml={fetchXml}
              rows={resultRows}
              onRowsChange={setResultRows}
            />
          ) : null}
        </div>
        <OutputPanel
          fetchXml={fetchXml}
          outputTab={outputTab}
          setOutputTab={setOutputTab}
        />
      </div>
    </main>
  );
}

function getErrorMessage(error: unknown) {
  return error instanceof Error ? error.message : "Unexpected Dataverse error.";
}

function safeReadModel(fetchXml: string) {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return {
      entity: "account",
      top: "50",
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
