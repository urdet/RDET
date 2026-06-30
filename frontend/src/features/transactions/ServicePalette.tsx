import { Service } from '../../types';

type ServicePaletteProps = {
  services: Service[];
  selectedId?: number;
  onSelect: (service: Service) => void;
};

function serviceTone(service: Service) {
  if (service.transaction_type === 'IN') return 'turquoise';
  if (service.transaction_type === 'OUT') return 'red-orange';
  return 'orange';
}

export function ServicePalette({ services, selectedId, onSelect }: ServicePaletteProps) {
  return (
    <div className="transaction-service-panel">
      <div className="service-button-flow">
        {services.map((service) => (
          <button
            key={service.id}
            className={`service-gradient-button ${serviceTone(service)} ${selectedId === service.id ? 'selected' : ''}`}
            onClick={() => onSelect(service)}
          >
            {service.image_url && <img className="service-button-image" src={service.image_url} alt="" />}
            {service.name}
          </button>
        ))}
        {services.length === 0 && <div className="empty-service-state">No services yet</div>}
      </div>
      <input className="desktop-date-input" type="date" defaultValue={new Date().toISOString().slice(0, 10)} />
    </div>
  );
}
