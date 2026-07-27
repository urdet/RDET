import { ChangeEvent, useEffect, useMemo, useState } from 'react';
import { FileText, FolderOpen, Plus, Save, Search, Trash2, Upload, UserRound, Users, X } from 'lucide-react';
import { api } from '../../api';
import { createClientId } from '../../utils/id';

type ClientDocument = {
  id: string;
  name: string;
  dataUrl: string;
  type: string;
};

type RegisterClient = {
  id: string;
  name: string;
  info: string;
  documents: ClientDocument[];
};

type RegisterPayload = {
  clients: RegisterClient[];
};

function newClient(): RegisterClient {
  return { id: createClientId(), name: '', info: '', documents: [] };
}

function readFile(file: File): Promise<ClientDocument> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve({ id: createClientId(), name: file.name, type: file.type, dataUrl: String(reader.result || '') });
    reader.onerror = () => reject(reader.error);
    reader.readAsDataURL(file);
  });
}

export function RegisterPage() {
  const [clients, setClients] = useState<RegisterClient[]>([]);
  const [draft, setDraft] = useState<RegisterClient>(newClient());
  const [query, setQuery] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    api<RegisterPayload>('/register-clients')
      .then((value) => setClients(value.clients ?? []))
      .catch((err) => setError(err instanceof Error ? err.message : 'Chargement impossible.'));
  }, []);

  const filteredClients = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    if (!normalized) return clients;
    return clients.filter((client) =>
      client.name.toLowerCase().includes(normalized) ||
      client.info.toLowerCase().includes(normalized) ||
      client.documents.some((doc) => doc.name.toLowerCase().includes(normalized))
    );
  }, [clients, query]);

  const documentCount = useMemo(() => clients.reduce((total, client) => total + client.documents.length, 0), [clients]);
  const isExisting = clients.some((client) => client.id === draft.id);

  async function persist(nextClients: RegisterClient[]) {
    setError('');
    setMessage('');
    await api<RegisterPayload>('/register-clients', {
      method: 'PATCH',
      body: JSON.stringify({ config: { clients: nextClients } }),
    });
    setClients(nextClients);
  }

  async function saveDraft() {
    if (!draft.name.trim()) {
      setError('Client name is required.');
      return;
    }
    setSaving(true);
    try {
      const cleanDraft = { ...draft, name: draft.name.trim(), info: draft.info.trim() };
      const next = isExisting
        ? clients.map((client) => client.id === draft.id ? cleanDraft : client)
        : [cleanDraft, ...clients];
      await persist(next);
      setDraft(cleanDraft);
      setMessage(isExisting ? 'Client updated.' : 'Client added to the register.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Save failed.');
    } finally {
      setSaving(false);
    }
  }

  async function uploadDocs(event: ChangeEvent<HTMLInputElement>) {
    const files = Array.from(event.target.files ?? []);
    if (!files.length) return;
    try {
      const docs = await Promise.all(files.map(readFile));
      setDraft((current) => ({ ...current, documents: [...current.documents, ...docs] }));
    } catch {
      setError('Unable to read one of the selected documents.');
    }
    event.target.value = '';
  }

  async function removeClient(client: RegisterClient) {
    if (!window.confirm(`Delete ${client.name} from the register?`)) return;
    try {
      await persist(clients.filter((item) => item.id !== client.id));
      if (draft.id === client.id) setDraft(newClient());
      setMessage('Client deleted.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Delete failed.');
    }
  }

  function startNewClient() {
    setDraft(newClient());
    setError('');
    setMessage('');
  }

  return (
    <div className="register-page register-workspace">
      <header className="register-page-header">
        <div>
          <span className="register-eyebrow"><Users /> Client register</span>
          <h1>Clients</h1>
          <p>Keep client information and documents together in one simple register.</p>
        </div>
        <button type="button" className="register-primary-button" onClick={startNewClient}><Plus /> New client</button>
      </header>

      <section className="register-summary">
        <div><span>Registered clients</span><strong>{clients.length}</strong><Users /></div>
        <div><span>Stored documents</span><strong>{documentCount}</strong><FolderOpen /></div>
        <div><span>Current record</span><strong>{isExisting ? 'Editing' : 'New'}</strong><UserRound /></div>
      </section>

      {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}

      <div className="register-main-layout">
        <aside className="register-directory">
          <div className="register-directory-head">
            <div>
              <strong>Directory</strong>
              <small>{filteredClients.length} visible</small>
            </div>
          </div>
          <label className="register-search">
            <Search />
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search clients or documents" />
            {query && <button type="button" onClick={() => setQuery('')} aria-label="Clear search"><X /></button>}
          </label>
          <div className="register-client-list">
            {filteredClients.map((client) => (
              <article className={`register-list-item ${draft.id === client.id ? 'active' : ''}`} key={client.id}>
                <button type="button" className="register-list-select" onClick={() => { setDraft(client); setError(''); setMessage(''); }}>
                  <span className="register-client-avatar">{client.name.slice(0, 2).toUpperCase()}</span>
                  <span className="register-client-summary">
                    <strong>{client.name}</strong>
                    <small>{client.documents.length} {client.documents.length === 1 ? 'document' : 'documents'}{client.info ? ` · ${client.info}` : ''}</small>
                  </span>
                </button>
                <button type="button" className="register-list-delete" title="Delete client" aria-label={`Delete ${client.name}`} onClick={() => removeClient(client)}><Trash2 /></button>
              </article>
            ))}
            {!filteredClients.length && (
              <div className="register-empty">
                <UserRound />
                <strong>No clients found</strong>
                <span>{query ? 'Try another search.' : 'Create the first client record.'}</span>
              </div>
            )}
          </div>
        </aside>

        <section className="register-editor-panel">
          <div className="register-editor-head">
            <div>
              <span>{isExisting ? 'Client record' : 'New record'}</span>
              <h2>{isExisting ? draft.name : 'Add a client'}</h2>
            </div>
            {isExisting && <button type="button" className="register-secondary-button" onClick={startNewClient}><Plus /> New</button>}
          </div>

          <div className="register-editor-form">
            <label className="register-field">
              <span>Client name <b>*</b></span>
              <input value={draft.name} placeholder="Full name or company name" onChange={(event) => setDraft((current) => ({ ...current, name: event.target.value }))} />
            </label>
            <label className="register-field">
              <span>Information <small>Optional</small></span>
              <textarea value={draft.info} placeholder="Phone, address, reference or any useful note…" onChange={(event) => setDraft((current) => ({ ...current, info: event.target.value }))} />
            </label>

            <div className="register-files-section">
              <div className="register-files-head">
                <div>
                  <strong>Documents</strong>
                  <small>Images and PDF files</small>
                </div>
                <label className="register-upload-button">
                  <Upload />
                  Add documents
                  <input type="file" multiple accept="image/*,.pdf" onChange={uploadDocs} />
                </label>
              </div>
              <div className="register-file-grid">
                {draft.documents.map((doc) => (
                  <article className="register-file-card" key={doc.id}>
                    <a href={doc.dataUrl} target="_blank" rel="noreferrer">
                      {doc.type.startsWith('image/') ? <img src={doc.dataUrl} alt="" /> : <span className="register-pdf-preview"><FileText /></span>}
                      <strong>{doc.name}</strong>
                    </a>
                    <button type="button" title="Remove document" aria-label={`Remove ${doc.name}`} onClick={() => setDraft((current) => ({ ...current, documents: current.documents.filter((item) => item.id !== doc.id) }))}><X /></button>
                  </article>
                ))}
                {!draft.documents.length && (
                  <label className="register-file-empty">
                    <Upload />
                    <strong>Drop in a document</strong>
                    <span>or click to choose files</span>
                    <input type="file" multiple accept="image/*,.pdf" onChange={uploadDocs} />
                  </label>
                )}
              </div>
            </div>

            <div className="register-editor-actions">
              <button type="button" className="register-secondary-button" onClick={startNewClient}>Clear</button>
              <button type="button" className="register-primary-button" disabled={saving || !draft.name.trim()} onClick={saveDraft}><Save /> {saving ? 'Saving…' : isExisting ? 'Save changes' : 'Add client'}</button>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}
