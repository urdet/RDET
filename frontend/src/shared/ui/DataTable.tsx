import { ReactNode } from 'react';

type DataTableProps = {
  title?: string;
  headers: string[];
  rows: Array<Array<ReactNode>>;
};

export function DataTable({ title, headers, rows }: DataTableProps) {
  return (
    <div className="notion-card p-0">
      {title && <div className="table-caption">{title}</div>}
      <div className="overflow-x-auto">
        <table className="desktop-table">
          <thead>
            <tr>{headers.map((header) => <th key={header}>{header}</th>)}</tr>
          </thead>
          <tbody>
            {rows.length === 0 ? (
              <tr><td colSpan={headers.length} className="empty-cell">No data</td></tr>
            ) : (
              rows.map((row, index) => <tr key={index}>{row.map((cell, cellIndex) => <td key={cellIndex}>{cell}</td>)}</tr>)
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
