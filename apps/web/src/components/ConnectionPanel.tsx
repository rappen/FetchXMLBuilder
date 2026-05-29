import { normalizeOrganizationUrl } from "@fetchxmlbuilder/dataverse";
import { CheckCircle2, LogOut, PlugZap, UserRound } from "lucide-react";
import type { ConnectionStatus } from "../store/workbenchStore";

interface ConnectionPanelProps {
  orgUrl: string;
  clientId: string;
  tenantId: string;
  redirectUri: string;
  status: ConnectionStatus;
  error: string;
  userName: string;
  onFieldChange: (
    field: "orgUrl" | "clientId" | "tenantId",
    value: string,
  ) => void;
  onConnect: () => void;
  onDisconnect: () => void;
}

export function ConnectionPanel({
  orgUrl,
  clientId,
  tenantId,
  redirectUri,
  status,
  error,
  userName,
  onFieldChange,
  onConnect,
  onDisconnect,
}: ConnectionPanelProps) {
  const normalizedUrl = safeNormalizeUrl(orgUrl);
  const isBusy = status === "connecting" || status === "loadingMetadata";
  const isConnected = status === "connected" || status === "loadingMetadata";

  return (
    <section className="panel connection-panel" aria-label="Connection setup">
      <div className="panel-heading compact">
        <h2>Connection</h2>
        <span className={isConnected ? "status-pill ready" : "status-pill"}>
          <CheckCircle2 size={14} />
          {statusLabel(status)}
        </span>
      </div>
      <div className="connection-grid">
        <label className="wide-field">
          <span>Org URL</span>
          <input
            value={orgUrl}
            onChange={(event) => onFieldChange("orgUrl", event.target.value)}
            placeholder="https://org.crm.dynamics.com"
          />
        </label>
        <label>
          <span>App ID</span>
          <input
            value={clientId}
            onChange={(event) => onFieldChange("clientId", event.target.value)}
            placeholder="Application/client ID"
          />
        </label>
        <label>
          <span>Tenant</span>
          <input
            value={tenantId}
            onChange={(event) => onFieldChange("tenantId", event.target.value)}
            placeholder="common"
          />
        </label>
        <div className="connection-hint">
          Redirect URI: <code>{redirectUri}</code>
        </div>
        <div className="connection-summary">
          <UserRound size={16} />
          <span>{userName || normalizedUrl || "mock://dataverse"}</span>
        </div>
        <div className="connection-actions">
          <button
            className="primary-action"
            type="button"
            disabled={isBusy || !normalizedUrl}
            onClick={onConnect}
          >
            <PlugZap size={16} />
            <span>{isBusy ? "Connecting" : "Microsoft login"}</span>
          </button>
          <button type="button" disabled={!isConnected} onClick={onDisconnect}>
            <LogOut size={16} />
            <span>Disconnect</span>
          </button>
        </div>
        {error ? <p className="connection-error">{error}</p> : null}
      </div>
    </section>
  );
}

function statusLabel(status: ConnectionStatus) {
  if (status === "connecting") return "Signing in";
  if (status === "loadingMetadata") return "Metadata";
  if (status === "connected") return "Connected";
  if (status === "error") return "Error";
  return "Local";
}

function safeNormalizeUrl(url: string) {
  try {
    return url.trim() ? normalizeOrganizationUrl(url) : "";
  } catch {
    return "";
  }
}
