import { useMemo, useState } from 'react';
import { Activity, BadgeDollarSign, CircleDot, GitBranch, Landmark, ListChecks, MousePointer2, PencilRuler, ShieldCheck } from 'lucide-react';
import { WorkflowEdge, WorkflowNode, WorkflowNodeKind } from '../../types';

type WorkflowCanvasProps = {
  title: string;
  description: string;
  initialNodes: WorkflowNode[];
  initialEdges: WorkflowEdge[];
};

const palette: Array<{ kind: WorkflowNodeKind; title: string; subtitle: string }> = [
  { kind: 'trigger', title: 'Trigger', subtitle: 'Start of workflow' },
  { kind: 'account', title: 'Compte', subtitle: 'Balance source/target' },
  { kind: 'operation', title: 'Operation', subtitle: 'Update or save action' },
  { kind: 'condition', title: 'Condition', subtitle: 'Route by type/rule' },
  { kind: 'fee', title: 'Frais', subtitle: 'Fee calculation' },
  { kind: 'audit', title: 'Audit', subtitle: 'Track line' },
];

function iconFor(kind: WorkflowNodeKind) {
  return {
    trigger: CircleDot,
    account: Landmark,
    operation: ListChecks,
    condition: GitBranch,
    fee: BadgeDollarSign,
    audit: ShieldCheck,
  }[kind];
}

function colorFor(kind: WorkflowNodeKind) {
  return {
    trigger: '#2563eb',
    account: '#7c3aed',
    operation: '#059669',
    condition: '#ca8a04',
    fee: '#dc2626',
    audit: '#475569',
  }[kind];
}

export function WorkflowCanvas({ title, description, initialNodes, initialEdges }: WorkflowCanvasProps) {
  const [nodes, setNodes] = useState(initialNodes);
  const [edges] = useState(initialEdges);
  const [selectedId, setSelectedId] = useState(initialNodes[0]?.id);
  const selectedNode = nodes.find((node) => node.id === selectedId);
  const nodeMap = useMemo(() => new Map(nodes.map((node) => [node.id, node])), [nodes]);

  function moveNode(id: string, x: number, y: number) {
    setNodes((current) => current.map((node) => node.id === id ? { ...node, x: Math.max(10, x), y: Math.max(10, y) } : node));
  }

  function addNode(kind: WorkflowNodeKind, x = 120, y = 120) {
    const template = palette.find((item) => item.kind === kind)!;
    const node: WorkflowNode = {
      id: `${kind}-${Date.now()}`,
      kind,
      title: template.title,
      subtitle: template.subtitle,
      x,
      y,
    };
    setNodes((current) => [...current, node]);
    setSelectedId(node.id);
  }

  return (
    <div className="workflow-page">
      <div className="workflow-page-header">
        <div>
          <div className="workflow-eyebrow"><PencilRuler className="h-4 w-4" /> Workflow builder</div>
          <h2>{title}</h2>
          <p>{description}</p>
        </div>
      </div>

      <div className="workflow-layout">
        <aside className="workflow-palette">
          <div className="palette-title">Blocks</div>
          {palette.map((item) => {
            const Icon = iconFor(item.kind);
            return (
              <button
                key={item.kind}
                draggable
                className="palette-item"
                onClick={() => addNode(item.kind)}
                onDragStart={(event) => event.dataTransfer.setData('application/workflow-node', item.kind)}
              >
                <Icon className="h-4 w-4" style={{ color: colorFor(item.kind) }} />
                <span>
                  <strong>{item.title}</strong>
                  <small>{item.subtitle}</small>
                </span>
              </button>
            );
          })}
        </aside>

        <div
          className="workflow-canvas"
          onDragOver={(event) => event.preventDefault()}
          onDrop={(event) => {
            const kind = event.dataTransfer.getData('application/workflow-node') as WorkflowNodeKind;
            if (!kind) return;
            const rect = event.currentTarget.getBoundingClientRect();
            addNode(kind, event.clientX - rect.left, event.clientY - rect.top);
          }}
        >
          <svg className="workflow-edges">
            {edges.map((edge) => {
              const from = nodeMap.get(edge.from);
              const to = nodeMap.get(edge.to);
              if (!from || !to) return null;
              const x1 = from.x + 220;
              const y1 = from.y + 42;
              const x2 = to.x;
              const y2 = to.y + 42;
              const mid = Math.max(40, (x2 - x1) / 2);
              return (
                <path
                  key={`${edge.from}-${edge.to}`}
                  d={`M ${x1} ${y1} C ${x1 + mid} ${y1}, ${x2 - mid} ${y2}, ${x2} ${y2}`}
                  fill="none"
                  stroke="#c7c3bb"
                  strokeWidth="2"
                />
              );
            })}
          </svg>
          {nodes.map((node) => (
            <WorkflowNodeCard key={node.id} node={node} selected={node.id === selectedId} onSelect={setSelectedId} onMove={moveNode} />
          ))}
          <div className="canvas-help"><MousePointer2 className="h-4 w-4" /> Drag blocks here, then drag nodes to arrange the workflow.</div>
        </div>

        <aside className="workflow-inspector">
          <div className="palette-title">Inspector</div>
          {selectedNode ? (
            <>
              <label className="form-field">Name<input value={selectedNode.title} onChange={(event) => setNodes((current) => current.map((node) => node.id === selectedNode.id ? { ...node, title: event.target.value } : node))} /></label>
              <label className="form-field">Description<input value={selectedNode.subtitle} onChange={(event) => setNodes((current) => current.map((node) => node.id === selectedNode.id ? { ...node, subtitle: event.target.value } : node))} /></label>
              <div className="inspector-note"><Activity className="h-4 w-4" /> This config is UI-ready. The next backend pass can persist nodes and compile them into account movement rules.</div>
            </>
          ) : (
            <div className="empty-service-state">Select a node</div>
          )}
        </aside>
      </div>
    </div>
  );
}

function WorkflowNodeCard({ node, selected, onSelect, onMove }: { node: WorkflowNode; selected: boolean; onSelect: (id: string) => void; onMove: (id: string, x: number, y: number) => void }) {
  const Icon = iconFor(node.kind);

  return (
    <div
      className={`workflow-node ${selected ? 'selected' : ''}`}
      style={{ left: node.x, top: node.y, borderColor: colorFor(node.kind) }}
      draggable
      onClick={() => onSelect(node.id)}
      onDragEnd={(event) => {
        const parent = event.currentTarget.parentElement?.getBoundingClientRect();
        if (!parent) return;
        onMove(node.id, event.clientX - parent.left - 110, event.clientY - parent.top - 40);
      }}
    >
      <div className="node-icon" style={{ background: colorFor(node.kind) }}>
        <Icon className="h-4 w-4" />
      </div>
      <div>
        <div className="node-title">{node.title}</div>
        <div className="node-subtitle">{node.subtitle}</div>
      </div>
    </div>
  );
}
