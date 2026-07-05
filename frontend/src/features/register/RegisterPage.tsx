import { ChangeEvent, useEffect, useMemo, useState } from 'react';
import { FileText, Plus, Save, Search, Trash2, Upload } from 'lucide-react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
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

  async function persist(nextClients: RegisterClient[]) {
    setError('');
    setMessage('');
    await api<RegisterPayload>('/register-clients', {
      method: 'PATCH',
      body: JSON.stringify({ config: { clients: nextClients } }),
    });
    setClients(nextClients);
    setMessage('Register saved.');
  }

  async function saveDraft() {
    if (!draft.name.trim()) {
      setError('Client name is required.');
      return;
    }
    try {
      const next = clients.some((client) => client.id === draft.id)
        ? clients.map((client) => client.id === draft.id ? draft : client)
        : [{ ...draft, name: draft.name.trim() }, ...clients];
      await persist(next);
      setDraft(newClient());
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Save failed.');
    }
  }

  async function uploadDocs(event: ChangeEvent<HTMLInputElement>) {
    const files = Array.from(event.target.files ?? []);
    if (!files.length) return;
    const docs = await Promise.all(files.map(readFile));
    setDraft((current) => ({ ...current, documents: [...current.documents, ...docs] }));
    event.target.value = '';
  }

  async function removeClient(id: string) {
    try {
      await persist(clients.filter((client) => client.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Delete failed.');
    }
  }

  return (
    <div className="register-page">
      <Panel title="Register clients" icon={FileText}>
        <div className="register-toolbar">
          <label className="account-search compact">
            <Search className="h-4 w-4" />
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search client, info, document..." />
          </label>
          <CircleButton title="New client" icon={Plus} onClick={() => setDraft(newClient())} />
        </div>

        {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}

        <div className="register-editor">
          <label className="form-field">
            Client name
            <input value={draft.name} onChange={(event) => setDraft((current) => ({ ...current, name: event.target.value }))} />
          </label>
          <label className="form-field register-info-field">
            Infos
            <textarea value={draft.info} onChange={(event) => setDraft((current) => ({ ...current, info: event.target.value }))} />
          </label>
          <label className="upload-button">
            <Upload className="h-4 w-4" />
            Documents
            <input type="file" multiple accept="image/*,.pdf" onChange={uploadDocs} />
          </label>
          <div className="register-doc-list">
            {draft.documents.map((doc) => (
              <span key={doc.id}>
                {doc.name}
                <button onClick={() => setDraft((current) => ({ ...current, documents: current.documents.filter((item) => item.id !== doc.id) }))}>x</button>
              </span>
            ))}
          </div>
          <CircleButton title="Save client" icon={Save} onClick={saveDraft} />
        </div>
      </Panel>

      <div className="register-card-grid">
        {filteredClients.map((client) => (
          <article className="register-client-card" key={client.id}>
            <div className="register-card-head">
              <button onClick={() => setDraft(client)}>{client.name}</button>
              <button className="grid-delete" title="Delete" onClick={() => removeClient(client.id)}>
                <Trash2 className="h-4 w-4" />
              </button>
            </div>
            <p>{client.info || 'No info yet.'}</p>
            <div className="register-documents">
              {client.documents.map((doc) => (
                <a key={doc.id} href={doc.dataUrl} target="_blank" rel="noreferrer">
                  {doc.type.startsWith('image/') ? <img src={doc.dataUrl} alt="" /> : <FileText className="h-5 w-5" />}
                  <span>{doc.name}</span>
                </a>
              ))}
              {!client.documents.length && <small>No documents</small>}
            </div>
          </article>
        ))}
        {!filteredClients.length && <div className="transaction-empty-import">No clients found.</div>}
      </div>
    </div>
  );
}
