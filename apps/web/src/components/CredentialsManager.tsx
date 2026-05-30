import type { DataverseConnectionProfile } from "@fetchxmlbuilder/storage";
import {
  CheckCircle2,
  KeyRound,
  Pencil,
  PlugZap,
  Plus,
  Save,
  Trash2,
  XCircle,
} from "lucide-react";
import { useMemo, useState } from "react";

interface CredentialsManagerProps {
  connectionProfiles: DataverseConnectionProfile[];
  activeConnectionProfileId: string;
  testingCredentialId: string;
  onSave: (profile: DataverseConnectionProfile) => void;
  onDelete: (profileId: string) => void;
  onUse: (profileId: string) => void;
  onTest: (profile: DataverseConnectionProfile) => void;
}

type ConnectionProfileDraft = Pick<
  DataverseConnectionProfile,
  "id" | "name" | "orgUrl" | "clientId" | "tenantId"
>;

const emptyDraft: ConnectionProfileDraft = {
  id: "",
  name: "",
  orgUrl: "",
  clientId: "",
  tenantId: "common",
};

export function CredentialsManager({
  connectionProfiles,
  activeConnectionProfileId,
  testingCredentialId,
  onSave,
  onDelete,
  onUse,
  onTest,
}: CredentialsManagerProps) {
  const [draft, setDraft] = useState<ConnectionProfileDraft>(emptyDraft);
  const sortedConnectionProfiles = useMemo(
    () =>
      [...connectionProfiles].sort((left, right) =>
        right.updatedAt.localeCompare(left.updatedAt),
      ),
    [connectionProfiles],
  );
  const selectedConnectionProfile = connectionProfiles.find(
    (profile) => profile.id === draft.id,
  );
  const canSave = draft.name.trim() && draft.orgUrl.trim();
  const isTestingDraft = Boolean(draft.id && testingCredentialId === draft.id);

  function startNewConnectionProfile() {
    setDraft({
      ...emptyDraft,
      id: createConnectionProfileId(),
    });
  }

  function editConnectionProfile(profile: DataverseConnectionProfile) {
    setDraft({
      id: profile.id,
      name: profile.name,
      orgUrl: profile.orgUrl,
      clientId: profile.clientId,
      tenantId: profile.tenantId,
    });
  }

  function saveConnectionProfile() {
    if (!canSave) return;
    const now = new Date().toISOString();
    const profile: DataverseConnectionProfile = {
      id: draft.id || createConnectionProfileId(),
      name: draft.name.trim(),
      orgUrl: draft.orgUrl.trim(),
      clientId: draft.clientId.trim(),
      tenantId: draft.tenantId.trim() || "common",
      updatedAt: now,
    };
    if (selectedConnectionProfile?.lastTestedAt) {
      profile.lastTestedAt = selectedConnectionProfile.lastTestedAt;
    }
    if (selectedConnectionProfile?.lastTestStatus) {
      profile.lastTestStatus = selectedConnectionProfile.lastTestStatus;
    }
    if (selectedConnectionProfile?.lastTestMessage) {
      profile.lastTestMessage = selectedConnectionProfile.lastTestMessage;
    }
    onSave(profile);
  }

  return (
    <section className="credentials-module" aria-label="Connections manager">
      <div className="module-heading">
        <div>
          <h2>Connections Manager</h2>
          <p>Saved Dataverse connection profiles for this browser.</p>
        </div>
        <button
          type="button"
          className="primary-action"
          onClick={startNewConnectionProfile}
        >
          <Plus size={16} />
          <span>New connection</span>
        </button>
      </div>

      <div className="credentials-layout">
        <section className="panel credentials-list-panel">
          <div className="panel-heading compact">
            <h2>Saved Connections</h2>
            <span className="status-pill">{connectionProfiles.length}</span>
          </div>
          <div className="credentials-list">
            {sortedConnectionProfiles.length ? (
              sortedConnectionProfiles.map((profile) => (
                <article
                  className={
                    profile.id === activeConnectionProfileId
                      ? "credential-card active"
                      : "credential-card"
                  }
                  key={profile.id}
                >
                  <div className="credential-card-main">
                    <KeyRound size={18} />
                    <div>
                      <h3>{profile.name}</h3>
                      <span>{profile.orgUrl}</span>
                    </div>
                  </div>
                  <div className="credential-meta">
                    <span>{profile.tenantId || "common"}</span>
                    <ConnectionTestStatus profile={profile} />
                  </div>
                  <div className="credential-actions">
                    <button
                      type="button"
                      title="Use connection"
                      onClick={() => onUse(profile.id)}
                    >
                      <PlugZap size={15} />
                    </button>
                    <button
                      type="button"
                      title="Edit connection"
                      onClick={() => editConnectionProfile(profile)}
                    >
                      <Pencil size={15} />
                    </button>
                    <button
                      type="button"
                      title="Delete connection"
                      onClick={() => onDelete(profile.id)}
                    >
                      <Trash2 size={15} />
                    </button>
                  </div>
                </article>
              ))
            ) : (
              <div className="empty-state">No connections saved yet.</div>
            )}
          </div>
        </section>

        <section className="panel credential-editor-panel">
          <div className="panel-heading compact">
            <h2>{draft.id ? "Connection Details" : "New Connection"}</h2>
          </div>
          <div className="credential-form">
            <label>
              <span>Name</span>
              <input
                value={draft.name}
                onChange={(event) =>
                  setDraft({ ...draft, name: event.target.value })
                }
                placeholder="Production sales"
              />
            </label>
            <label>
              <span>Org URL</span>
              <input
                value={draft.orgUrl}
                onChange={(event) =>
                  setDraft({ ...draft, orgUrl: event.target.value })
                }
                placeholder="https://org.crm.dynamics.com"
              />
            </label>
            <label>
              <span>App ID</span>
              <input
                value={draft.clientId}
                onChange={(event) =>
                  setDraft({ ...draft, clientId: event.target.value })
                }
                placeholder="Application/client ID"
              />
            </label>
            <label>
              <span>Tenant</span>
              <input
                value={draft.tenantId}
                onChange={(event) =>
                  setDraft({ ...draft, tenantId: event.target.value })
                }
                placeholder="common"
              />
            </label>
            <div className="credential-editor-actions">
              <button
                type="button"
                className="primary-action"
                disabled={!canSave}
                onClick={saveConnectionProfile}
              >
                <Save size={16} />
                <span>Save</span>
              </button>
              <button
                type="button"
                disabled={!canSave || isTestingDraft}
                onClick={() =>
                  onTest({
                    id: draft.id || createConnectionProfileId(),
                    name: draft.name.trim(),
                    orgUrl: draft.orgUrl.trim(),
                    clientId: draft.clientId.trim(),
                    tenantId: draft.tenantId.trim() || "common",
                    updatedAt:
                      selectedConnectionProfile?.updatedAt ??
                      new Date().toISOString(),
                  })
                }
              >
                <PlugZap size={16} />
                <span>{isTestingDraft ? "Testing" : "Test"}</span>
              </button>
            </div>
          </div>
        </section>
      </div>
    </section>
  );
}

function ConnectionTestStatus({
  profile,
}: {
  profile: DataverseConnectionProfile;
}) {
  if (!profile.lastTestStatus) {
    return <span>Not tested</span>;
  }
  if (profile.lastTestStatus === "success") {
    return (
      <span className="credential-test-status success">
        <CheckCircle2 size={14} />
        {profile.lastTestMessage || "Connected"}
      </span>
    );
  }
  return (
    <span className="credential-test-status error">
      <XCircle size={14} />
      {profile.lastTestMessage || "Failed"}
    </span>
  );
}

function createConnectionProfileId() {
  if (globalThis.crypto?.randomUUID) {
    return globalThis.crypto.randomUUID();
  }
  return `connection-${Date.now()}`;
}
