import { useEffect, useMemo, useState } from 'react';
import { ArrowRight, BadgeCheck, Save, Settings2 } from 'lucide-react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Account, Direction, Service, ServiceRoutingConfig } from '../../types';

type TransactionWorkflowPageProps = {
  accounts: Account[];
  services: Service[];
  onSaved: () => void;
};

function accountByNames(accounts: Account[], names: string[]) {
  const normalized = names.map((name) => name.toLowerCase());
  return accounts.find((account) => normalized.includes(account.name.toLowerCase()));
}

function serviceType(service: Service | null) {
  return service?.transaction_type ?? service?.switch_type ?? 'IN & OUT';
}

function serviceDirections(type: string): Direction[] {
  return type === 'IN & OUT' || (type.includes('IN') && type.includes('OUT')) ? ['IN', 'OUT'] : type.includes('OUT') ? ['OUT'] : ['IN'];
}

function defaultRoute(direction: Direction, cash?: Account, fundex?: Account) {
  return direction === 'IN'
    ? { from_account_id: cash?.id, to_account_id: fundex?.id }
    : { from_account_id: fundex?.id, to_account_id: cash?.id };
}

export function TransactionWorkflowPage({ accounts, services, onSaved }: TransactionWorkflowPageProps) {
  const [selectedId, setSelectedId] = useState<number | null>(services[0]?.id ?? null);
  const selectedService = services.find((service) => service.id === selectedId) ?? services[0] ?? null;
  const [selectedDirection, setSelectedDirection] = useState<Direction>('IN');
  const [routingConfig, setRoutingConfig] = useState<ServiceRoutingConfig>({});
  const [active, setActive] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const cash = useMemo(() => accountByNames(accounts, ['Caisse Calculee', 'Caisse Calculée', 'Caise Calcule', 'Caise Calculee']), [accounts]);
  const fundex = useMemo(() => accountByNames(accounts, ['Fundex']), [accounts]);

  useEffect(() => {
    if (!selectedService) return;
    const directions = serviceDirections(serviceType(selectedService));
    setSelectedDirection(directions[0]);
    setRoutingConfig(selectedService.routing_config ?? {});
    setActive(selectedService.active);
    setMessage('');
    setError('');
  }, [selectedService?.id, selectedService?.transaction_type, selectedService?.switch_type]);

  const allowedDirections = serviceDirections(serviceType(selectedService));
  const route = routingConfig[selectedDirection] ?? defaultRoute(selectedDirection, cash, fundex);
  const fromAccountId = route.from_account_id ? String(route.from_account_id) : '';
  const toAccountId = route.to_account_id ? String(route.to_account_id) : '';

  function updateRoute(direction: Direction, field: 'from_account_id' | 'to_account_id', value: string) {
    setRoutingConfig((current) => ({
      ...current,
      [direction]: {
        ...(current[direction] ?? defaultRoute(direction, cash, fundex)),
        [field]: value ? Number(value) : undefined,
      },
    }));
  }

  async function saveConfig() {
    if (!selectedService) return;
    const nextRoutingConfig = {
      ...routingConfig,
      [selectedDirection]: route,
    };
    setSaving(true);
    setMessage('');
    setError('');
    try {
      await api<Service>(`/services/${selectedService.id}`, {
        method: 'PATCH',
        body: JSON.stringify({
          name: selectedService.name,
          image_url: selectedService.image_url,
          transaction_type: serviceType(selectedService),
          routing_config: nextRoutingConfig,
          active,
        }),
      });
      setRoutingConfig(nextRoutingConfig);
      await onSaved();
      setMessage('Configuration sauvegardee.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Configuration non sauvegardee.');
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="service-workflow-page">
      <div className="workflow-page-header">
        <div>
          <div className="workflow-eyebrow"><Settings2 className="h-4 w-4" /> Service configuration</div>
          <h2>Transaction workflows</h2>
          <p>Each service uses its saved type to decide which directions can be configured, then each direction chooses its own source and target accounts.</p>
        </div>
      </div>

      <div className="service-config-layout">
        <aside className="service-config-list">
          <div className="palette-title">Services</div>
          {services.map((service) => (
            <button
              key={service.id}
              className={`service-config-item ${selectedService?.id === service.id ? 'active' : ''}`}
              onClick={() => setSelectedId(service.id)}
            >
              {service.image_url ? <img src={service.image_url} alt="" /> : <span>{service.name.slice(0, 2).toUpperCase()}</span>}
              <strong>{service.name}</strong>
              <small>{service.transaction_type ?? 'IN & OUT'}</small>
            </button>
          ))}
          {!services.length && <div className="empty-service-state">No services yet</div>}
        </aside>

        <section className="service-config-panel">
          {selectedService ? (
            <>
              <div className="service-config-header">
                <div className="service-config-image">
                  {selectedService.image_url ? <img src={selectedService.image_url} alt="" /> : selectedService.name.slice(0, 2).toUpperCase()}
                </div>
                <div>
                  <h3>{selectedService.name}</h3>
                  <p>{active ? 'Active service' : 'Inactive service'}</p>
                </div>
                <CircleButton title={saving ? 'Saving' : 'Save'} icon={Save} onClick={saveConfig} />
              </div>

              <div className="settings-grid">
                <label className="form-field">
                  Type
                  <select value={selectedDirection} onChange={(event) => setSelectedDirection(event.target.value as Direction)}>
                    {allowedDirections.map((direction) => <option key={direction} value={direction}>{direction}</option>)}
                  </select>
                </label>
                <label className="toggle-line user-active-toggle">
                  <input type="checkbox" checked={active} onChange={(event) => setActive(event.target.checked)} />
                  <span>{active ? 'Active' : 'Inactive'}</span>
                </label>
              </div>

              <div className="service-route-editor">
                <div className={`service-route-card ${selectedDirection === 'IN' ? 'in' : 'out'}`}>
                  <BadgeCheck className="h-5 w-5" />
                  <strong>{selectedDirection}</strong>
                  <span>{accounts.find((account) => String(account.id) === fromAccountId)?.name ?? 'From account'}</span>
                  <ArrowRight className="h-4 w-4" />
                  <span>{accounts.find((account) => String(account.id) === toAccountId)?.name ?? 'To account'}</span>
                </div>
                <div className="service-route-selects">
                  <label className="form-field">
                    From account
                    <select value={fromAccountId} onChange={(event) => updateRoute(selectedDirection, 'from_account_id', event.target.value)}>
                      <option value="">Select...</option>
                      {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                    </select>
                  </label>
                  <label className="form-field">
                    To account
                    <select value={toAccountId} onChange={(event) => updateRoute(selectedDirection, 'to_account_id', event.target.value)}>
                      <option value="">Select...</option>
                      {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                    </select>
                  </label>
                </div>
              </div>

              <div className="service-workflow-steps">
                <div>Service selected</div>
                <ArrowRight className="h-4 w-4" />
                <div>Direction checked</div>
                <ArrowRight className="h-4 w-4" />
                <div>Transfer saved</div>
                <ArrowRight className="h-4 w-4" />
                <div>Account actions and history saved</div>
              </div>

              {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}
            </>
          ) : (
            <div className="empty-service-state">Create a service first.</div>
          )}
        </section>
      </div>
    </div>
  );
}
