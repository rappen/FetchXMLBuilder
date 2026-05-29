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
import type { DataverseCredential } from "../store/workbenchStore";

interface CredentialsManagerProps {
  credentials: DataverseCredential[];
  activeCredentialId: string;
  testingCredentialId: string;
  onSave: (credential: DataverseCredential) => void;
  onDelete: (credentialId: string) => void;
  onUse: (credentialId: string) => void;
  onTest: (credential: DataverseCredential) => void;
}

type CredentialDraft = Pick<
  DataverseCredential,
  "id" | "name" | "orgUrl" | "clientId" | "tenantId"
>;

const emptyDraft: CredentialDraft = {
  id: "",
  name: "",
  orgUrl: "",
  clientId: "",
  tenantId: "common",
};

export function CredentialsManager({
  credentials,
  activeCredentialId,
  testingCredentialId,
  onSave,
  onDelete,
  onUse,
  onTest,
}: CredentialsManagerProps) {
  const [draft, setDraft] = useState<CredentialDraft>(emptyDraft);
  const sortedCredentials = useMemo(
    () =>
      [...credentials].sort((left, right) =>
        right.updatedAt.localeCompare(left.updatedAt),
      ),
    [credentials],
  );
  const selectedCredential = credentials.find(
    (credential) => credential.id === draft.id,
  );
  const canSave = draft.name.trim() && draft.orgUrl.trim();
  const isTestingDraft = Boolean(draft.id && testingCredentialId === draft.id);

  function startNewCredential() {
    setDraft({
      ...emptyDraft,
      id: createCredentialId(),
    });
  }

  function editCredential(credential: DataverseCredential) {
    setDraft({
      id: credential.id,
      name: credential.name,
      orgUrl: credential.orgUrl,
      clientId: credential.clientId,
      tenantId: credential.tenantId,
    });
  }

  function saveCredential() {
    if (!canSave) return;
    const now = new Date().toISOString();
    const credential: DataverseCredential = {
      id: draft.id || createCredentialId(),
      name: draft.name.trim(),
      orgUrl: draft.orgUrl.trim(),
      clientId: draft.clientId.trim(),
      tenantId: draft.tenantId.trim() || "common",
      updatedAt: now,
    };
    if (selectedCredential?.lastTestedAt) {
      credential.lastTestedAt = selectedCredential.lastTestedAt;
    }
    if (selectedCredential?.lastTestStatus) {
      credential.lastTestStatus = selectedCredential.lastTestStatus;
    }
    if (selectedCredential?.lastTestMessage) {
      credential.lastTestMessage = selectedCredential.lastTestMessage;
    }
    onSave(credential);
  }

  return (
    <section className="credentials-module" aria-label="Credentials manager">
      <div className="module-heading">
        <div>
          <h2>Credentials Manager</h2>
          <p>Saved Dataverse sign-in profiles for this browser.</p>
        </div>
        <button
          type="button"
          className="primary-action"
          onClick={startNewCredential}
        >
          <Plus size={16} />
          <span>New credential</span>
        </button>
      </div>

      <div className="credentials-layout">
        <section className="panel credentials-list-panel">
          <div className="panel-heading compact">
            <h2>Saved Credentials</h2>
            <span className="status-pill">{credentials.length}</span>
          </div>
          <div className="credentials-list">
            {sortedCredentials.length ? (
              sortedCredentials.map((credential) => (
                <article
                  className={
                    credential.id === activeCredentialId
                      ? "credential-card active"
                      : "credential-card"
                  }
                  key={credential.id}
                >
                  <div className="credential-card-main">
                    <KeyRound size={18} />
                    <div>
                      <h3>{credential.name}</h3>
                      <span>{credential.orgUrl}</span>
                    </div>
                  </div>
                  <div className="credential-meta">
                    <span>{credential.tenantId || "common"}</span>
                    <CredentialTestStatus credential={credential} />
                  </div>
                  <div className="credential-actions">
                    <button
                      type="button"
                      title="Use credential"
                      onClick={() => onUse(credential.id)}
                    >
                      <PlugZap size={15} />
                    </button>
                    <button
                      type="button"
                      title="Edit credential"
                      onClick={() => editCredential(credential)}
                    >
                      <Pencil size={15} />
                    </button>
                    <button
                      type="button"
                      title="Delete credential"
                      onClick={() => onDelete(credential.id)}
                    >
                      <Trash2 size={15} />
                    </button>
                  </div>
                </article>
              ))
            ) : (
              <div className="empty-state">No credentials saved yet.</div>
            )}
          </div>
        </section>

        <section className="panel credential-editor-panel">
          <div className="panel-heading compact">
            <h2>{draft.id ? "Credential Details" : "New Credential"}</h2>
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
                onClick={saveCredential}
              >
                <Save size={16} />
                <span>Save</span>
              </button>
              <button
                type="button"
                disabled={!canSave || isTestingDraft}
                onClick={() =>
                  onTest({
                    id: draft.id || createCredentialId(),
                    name: draft.name.trim(),
                    orgUrl: draft.orgUrl.trim(),
                    clientId: draft.clientId.trim(),
                    tenantId: draft.tenantId.trim() || "common",
                    updatedAt: selectedCredential?.updatedAt ?? "",
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

function CredentialTestStatus({
  credential,
}: {
  credential: DataverseCredential;
}) {
  if (!credential.lastTestStatus) {
    return <span>Not tested</span>;
  }
  if (credential.lastTestStatus === "success") {
    return (
      <span className="credential-test-status success">
        <CheckCircle2 size={14} />
        {credential.lastTestMessage || "Connected"}
      </span>
    );
  }
  return (
    <span className="credential-test-status error">
      <XCircle size={14} />
      {credential.lastTestMessage || "Failed"}
    </span>
  );
}

function createCredentialId() {
  if (globalThis.crypto?.randomUUID) {
    return globalThis.crypto.randomUUID();
  }
  return `credential-${Date.now()}`;
}
