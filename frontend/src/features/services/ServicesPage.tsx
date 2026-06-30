import { ImagePlus, Pencil, Save, Settings, Trash2, X } from 'lucide-react';
import { useState } from 'react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { DataTable } from '../../shared/ui/DataTable';
import { Input } from '../../shared/ui/FormField';
import { Panel } from '../../shared/ui/Panel';
import { Account, Service } from '../../types';

type ServicesPageProps = {
  services: Service[];
  accounts: Account[];
  onSaved: () => void;
};

export function ServicesPage({ services, accounts, onSaved }: ServicesPageProps) {
  const [name, setName] = useState('');
  const [type, setType] = useState('IN & OUT');
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [active, setActive] = useState(true);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [error, setError] = useState('');

  async function pickImage(file: File | null) {
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => setImageUrl(typeof reader.result === 'string' ? reader.result : null);
    reader.readAsDataURL(file);
  }

  async function submit(event: React.FormEvent) {
    event.preventDefault();
    setError('');
    await api(editingId ? `/services/${editingId}` : '/services', {
      method: editingId ? 'PATCH' : 'POST',
      body: JSON.stringify({ name, transaction_type: type, image_url: imageUrl, active }),
    });
    resetForm();
    await onSaved();
  }

  function editService(service: Service) {
    setError('');
    setEditingId(service.id);
    setName(service.name);
    setType(service.transaction_type ?? service.switch_type ?? 'IN & OUT');
    setImageUrl(service.image_url);
    setActive(service.active);
  }

  function resetForm() {
    setEditingId(null);
    setName('');
    setType('IN & OUT');
    setImageUrl(null);
    setActive(true);
  }

  async function deleteService(service: Service) {
    if (!window.confirm(`Delete ${service.name}?`)) return;
    setError('');
    setDeletingId(service.id);
    try {
      await api(`/services/${service.id}`, { method: 'DELETE' });
      if (editingId === service.id) resetForm();
      await onSaved();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Service not deleted.');
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className="grid gap-4 xl:grid-cols-[1fr_330px]">
      <DataTable
        title="Services"
        headers={['ID', 'Image', 'Service', 'Type', 'Status', 'Actions']}
        rows={services.map((service) => [
          service.id,
          service.image_url ? <img className="service-table-image" src={service.image_url} alt="" /> : '-',
          service.name,
          service.switch_type ?? service.transaction_type ?? '-',
          service.active ? 'Active' : 'Inactive',
          <div className="table-action-group">
            <button
              className="table-icon-button"
              type="button"
              title="Edit service"
              onClick={() => editService(service)}
            >
              <Pencil className="h-4 w-4" />
            </button>
            <button
              className="table-icon-button danger"
              type="button"
              title="Delete service"
              disabled={deletingId === service.id}
              onClick={() => deleteService(service)}
            >
              <Trash2 className="h-4 w-4" />
            </button>
          </div>,
        ])}
      />
      <Panel title={editingId ? 'Update service' : 'Service'} icon={Settings}>
        <form onSubmit={submit}>
          <Input label="Nom" value={name} onChange={setName} />
          <label className="form-field">
            Type
            <select value={type} onChange={(event) => setType(event.target.value)}>
              <option value="IN">IN</option>
              <option value="OUT">OUT</option>
              <option value="IN & OUT">IN & OUT</option>
            </select>
          </label>
          <label className="form-field">
            Image
            <input type="file" accept="image/*" onChange={(event) => pickImage(event.target.files?.[0] ?? null)} />
          </label>
          <label className="toggle-line mt-3">
            <input type="checkbox" checked={active} onChange={(event) => setActive(event.target.checked)} />
            <span>{active ? 'Active' : 'Inactive'}</span>
          </label>
          {imageUrl && (
            <div className="service-image-preview">
              <img src={imageUrl} alt="" />
              <ImagePlus className="h-5 w-5" />
            </div>
          )}
          {error && <div className="transaction-feedback error">{error}</div>}
          <div className="mt-4 flex justify-end gap-2">
            <CircleButton title="Close" icon={X} onClick={resetForm} />
            <CircleButton title="Save" icon={Save} type="submit" />
          </div>
        </form>
      </Panel>
    </div>
  );
}
