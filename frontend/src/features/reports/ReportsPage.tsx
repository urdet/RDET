import { Download, Settings2 } from 'lucide-react';
import jsPDF from 'jspdf';
import autoTable, { RowInput } from 'jspdf-autotable';
import { useEffect, useMemo, useState } from 'react';
import { api, getAccountsScreenSettings, getAppSettings } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { AppSettings, Dashboard } from '../../types';
import { accountBalance } from '../../utils/format';

type ReportBucket = { service?: string; IN: string; OUT: string; fees: string; count: number };
type Alimentation = { id: number; amount: string; description: string | null; occurred_at: string; from_account: string; to_account: string };
type ReportData = {
  kpis: { total_in: string; total_out: string; fees: string; net: string; count: number; average: string };
  by_service: ReportBucket[];
  alimentations: Alimentation[];
};
type CashCount = { counted_on: string; counts: Record<string, number>; total: string };
type CashAdjustments = { counters: Array<{ name: string; total: string }>; entries: unknown[] };
type ReportSections = Required<NonNullable<AppSettings['reportSections']>>;
type SavedKpi = { id: string; label: string; formula: string; visible: boolean };

const defaultSections: ReportSections = { transactions: true, alimentations: true, accounts: true, balances: true, cash: true };
const denominations = ['10000', '1000', '200', '100', '50', '20', '10', '5', '2', '1', '0.5'];

function todayValue() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
}

function monthStartValue() {
  return `${todayValue().slice(0, 8)}01`;
}

function formulaValue(formula: string, dashboard: Dashboard | null) {
  const normalize = (value: string) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().replace(/\s+/g, ' ').trim();
  const values: Record<string, number> = {
    'caisse reelle': Number(dashboard?.cash_real ?? 0),
    'factures non payees': Number(dashboard?.unpaid_total ?? 0),
    'non paye': Number(dashboard?.unpaid_total ?? 0),
    'caisse calculee': Number(dashboard?.total_balance ?? 0),
    'solde total': Number(dashboard?.total_balance ?? 0),
    'total debit': Number(dashboard?.total_debit ?? 0),
    'total credit': Number(dashboard?.total_credit ?? 0),
    'versements du jour': Number(dashboard?.service_in ?? 0),
    'retraits du jour': Number(dashboard?.service_out ?? 0),
    frais: Number(dashboard?.fees ?? 0),
  };
  dashboard?.accounts.forEach((account) => { values[normalize(account.name)] = Number(account.balance ?? 0); });
  let expression = normalize(formula).replace(/\{([^}]+)\}/g, '$1');
  Object.entries(values).sort(([left], [right]) => right.length - left.length).forEach(([name, value]) => {
    expression = expression.replace(new RegExp(name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), `(${value})`);
  });
  if (!/^[\d+\-*/().\s]+$/.test(expression)) return null;
  try {
    const result = Function(`"use strict"; return (${expression});`)();
    return Number.isFinite(result) ? Number(result) : null;
  } catch {
    return null;
  }
}

export function ReportsPage({ dashboard }: { dashboard: Dashboard | null }) {
  const [fromDate, setFromDate] = useState(monthStartValue());
  const [toDate, setToDate] = useState(todayValue());
  const [report, setReport] = useState<ReportData | null>(null);
  const [cash, setCash] = useState<CashCount | null>(null);
  const [settings, setSettings] = useState<Partial<AppSettings>>({});
  const [sections, setSections] = useState<ReportSections>(defaultSections);
  const [savedKpis, setSavedKpis] = useState<SavedKpi[]>([]);
  const [accountOrder, setAccountOrder] = useState<string[]>([]);
  const [cashAdjustments, setCashAdjustments] = useState<CashAdjustments>({ counters: [], entries: [] });
  const [designerOpen, setDesignerOpen] = useState(false);
  const [error, setError] = useState('');
  const [exporting, setExporting] = useState(false);

  useEffect(() => {
    Promise.all([
      getAppSettings<Partial<AppSettings>>(),
      getAccountsScreenSettings<Record<string, unknown>>(),
    ]).then(([appSettings, screenSettings]) => {
      setSettings(appSettings);
      setSections({ ...defaultSections, ...(appSettings.reportSections ?? {}) });
      const kpis = screenSettings.__kpis;
      if (Array.isArray(kpis)) setSavedKpis(kpis as SavedKpi[]);
      const order = screenSettings.__accountOrder;
      if (Array.isArray(order)) setAccountOrder(order.map(String));
    }).catch(() => undefined);
  }, []);

  useEffect(() => {
    setError('');
    Promise.all([
      api<ReportData>(`/reports?from_date=${fromDate}&to_date=${toDate}`),
      api<CashCount>(`/cash-counts/${toDate}`).catch(() => null),
      api<CashAdjustments>('/cash-adjustments').catch(() => ({ counters: [], entries: [] })),
    ]).then(([reportData, cashData, adjustments]) => {
      setReport(reportData);
      setCash(cashData);
      setCashAdjustments(adjustments);
    }).catch((err) => setError(err instanceof Error ? err.message : 'Chargement du rapport impossible.'));
  }, [fromDate, toDate]);

  const company = settings.reportCompany ?? {};
  const visibleKpis = useMemo(() => {
    const configured = savedKpis.filter((item) => item.visible).map((item) => ({ label: item.label, value: formulaValue(item.formula, dashboard) }));
    if (configured.length) return configured;
    return [
      { label: 'Solde total', value: Number(dashboard?.total_balance ?? 0) },
      { label: 'Caisse réelle', value: Number(dashboard?.cash_real ?? 0) },
      { label: 'Non payé', value: Number(dashboard?.unpaid_total ?? 0) },
    ];
  }, [savedKpis, dashboard]);
  const orderedAccounts = useMemo(() => {
    const positions = new Map(accountOrder.map((id, index) => [id, index]));
    return [...(dashboard?.accounts ?? [])].filter((account) => account.name.trim().toLowerCase() !== 'amount').sort((left, right) => {
      const leftPosition = positions.get(String(left.id)) ?? Number.MAX_SAFE_INTEGER;
      const rightPosition = positions.get(String(right.id)) ?? Number.MAX_SAFE_INTEGER;
      return leftPosition - rightPosition;
    });
  }, [dashboard?.accounts, accountOrder]);
  const adjustmentTotal = cashAdjustments.counters.reduce((sum, counter) => sum + Number(counter.total || 0), 0);

  function toggleSection(section: keyof ReportSections) {
    setSections((current) => ({ ...current, [section]: !current[section] }));
  }

  async function exportPdf() {
    if (exporting) return;
    setExporting(true);
    setError('');
    try {
      const pdf = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      const margin = 8;
      const contentWidth = pageWidth - margin * 2;
      let y = 10;

      if (company.logo) {
        const imageType = company.logo.includes('image/png') ? 'PNG' : 'JPEG';
        pdf.addImage(company.logo, imageType, margin, y, 25, 22, undefined, 'FAST');
      }
      const companyLeft = company.logo ? margin + 30 : margin;
      pdf.setTextColor(17, 24, 39);
      pdf.setFont('helvetica', 'bold');
      pdf.setFontSize(15);
      pdf.text(company.name || 'Nom de la société', companyLeft, y + 5);
      pdf.setFont('helvetica', 'normal');
      pdf.setFontSize(9);
      const companyLines = [
        [company.address, company.city].filter(Boolean).join(' - '),
        [company.taxId && `IF ${company.taxId}`, company.ice && `ICE ${company.ice}`, company.rc && `RC ${company.rc}`].filter(Boolean).join(' · '),
        [company.phone, company.email, company.website].filter(Boolean).join(' · '),
      ].filter(Boolean);
      companyLines.forEach((line, index) => pdf.text(String(line), companyLeft, y + 10 + index * 4.5));
      pdf.setFont('helvetica', 'bold');
      pdf.setFontSize(10);
      pdf.text('Rapport du :', pageWidth - 82, y + 5);
      pdf.text(fromDate === toDate ? toDate : `${fromDate} au ${toDate}`, pageWidth - margin, y + 5, { align: 'right' });
      pdf.text("Date d'impression :", pageWidth - 82, y + 12);
      pdf.setFont('helvetica', 'normal');
      pdf.text(new Date().toLocaleString('fr-MA'), pageWidth - margin, y + 12, { align: 'right' });
      y = 38;
      pdf.setDrawColor(31, 41, 55);
      pdf.setLineWidth(0.6);
      pdf.line(margin, y, pageWidth - margin, y);
      y += 8;

      const ensureSpace = (needed: number) => {
        if (y + needed <= pageHeight - margin) return;
        pdf.addPage();
        y = margin + 4;
      };
      const sectionTitle = (title: string, left = margin) => {
        ensureSpace(16);
        pdf.setTextColor(17, 24, 39);
        pdf.setFont('helvetica', 'bold');
        pdf.setFontSize(11);
        pdf.text(title, left, y);
        y += 4;
      };
      const tableAt = (
        head: string[][],
        body: RowInput[],
        startY: number,
        left: number,
        width: number,
        numericFrom = 1,
        totalRow = -1,
      ) => {
        autoTable(pdf, {
          startY,
          margin: { left, right: pageWidth - left - width },
          tableWidth: width,
          head,
          body,
          theme: 'grid',
          pageBreak: 'avoid',
          rowPageBreak: 'avoid',
          styles: {
            font: 'helvetica',
            fontSize: 8,
            minCellHeight: 6,
            cellPadding: { top: 1.1, right: 1.5, bottom: 1.1, left: 1.5 },
            valign: 'middle',
            lineColor: [55, 65, 81],
            lineWidth: 0.25,
            textColor: [17, 24, 39],
            overflow: 'hidden',
          },
          headStyles: {
            fillColor: [220, 224, 229],
            textColor: [17, 24, 39],
            fontStyle: 'bold',
            fontSize: 8.5,
            halign: 'center',
            valign: 'middle',
            minCellHeight: 7,
            overflow: 'linebreak',
          },
          didParseCell: (data) => {
            if (data.section === 'body' && data.column.index >= numericFrom) {
              data.cell.styles.halign = 'right';
              data.cell.styles.fontStyle = 'bold';
              data.cell.styles.overflow = 'hidden';
            }
            if (data.section === 'body' && data.row.index === totalRow) {
              data.cell.styles.fontStyle = 'bold';
              data.cell.styles.fillColor = [205, 211, 218];
            }
          },
        });
        return (pdf as jsPDF & { lastAutoTable: { finalY: number } }).lastAutoTable.finalY;
      };
      const table = (head: string[][], body: RowInput[], numericFrom = 1, totalRow = -1) => {
        y = tableAt(head, body, y, margin, contentWidth, numericFrom, totalRow) + 6;
      };

      if (sections.transactions || sections.alimentations) {
        const sectionGap = 5;
        const leftWidth = contentWidth * 0.56;
        const rightLeft = margin + leftWidth + sectionGap;
        const rightWidth = contentWidth - leftWidth - sectionGap;
        const titleY = y;
        let leftFinalY = titleY;
        let rightFinalY = titleY;
        if (sections.transactions) {
          pdf.setFont('helvetica', 'bold');
          pdf.setFontSize(11);
          pdf.text('Mouvements généraux', margin, titleY);
          const tableY = titleY + 4;
          const rows = (report?.by_service ?? []).map((row) => [row.service || '', accountBalance(Number(row.IN) + Number(row.fees)), accountBalance(row.OUT)]);
          rows.push(['Total', accountBalance(Number(report?.kpis.total_in ?? 0) + Number(report?.kpis.fees ?? 0)), accountBalance(report?.kpis.total_out ?? 0)]);
          leftFinalY = tableAt([['Service', 'IN + F', 'OUT + F']], rows, tableY, margin, sections.alimentations ? leftWidth : contentWidth, 1, rows.length - 1);
        }
        if (sections.alimentations) {
          const alimentationLeft = sections.transactions ? rightLeft : margin;
          const alimentationWidth = sections.transactions ? rightWidth : contentWidth;
          pdf.setFont('helvetica', 'bold');
          pdf.setFontSize(11);
          pdf.text('Alimentations - Coffre vers Caisse calculee', alimentationLeft, titleY);
          const rows = (report?.alimentations ?? []).map((row) => [accountBalance(row.amount), new Date(row.occurred_at).toLocaleString('fr-MA')]);
          rightFinalY = tableAt([['Montant', 'Date']], rows.length ? rows : [['Aucune alimentation', '']], titleY + 4, alimentationLeft, alimentationWidth, 0);
        }
        y = Math.max(leftFinalY, rightFinalY) + 6;
      }
      if (sections.accounts) {
        sectionTitle('Extrait des comptes');
        table([orderedAccounts.map((account) => account.name)], [orderedAccounts.map((account) => accountBalance(account.balance))], 0);
      }
      if (sections.balances) {
        sectionTitle('Balances');
        table([visibleKpis.map((item) => item.label)], [visibleKpis.map((item) => item.value === null ? '—' : accountBalance(item.value))], 0);
      }
      if (sections.cash) {
        sectionTitle('Détail caisse');
        const cashRows: RowInput[] = [
          ['Nombre', ...denominations.map((value) => cash?.counts[value] ?? 0)],
          ...cashAdjustments.counters.map((counter) => [
            counter.name,
            { content: accountBalance(counter.total), colSpan: denominations.length, styles: { halign: 'center', fontStyle: 'bold' } },
          ]),
          [
            'Total caisse',
            { content: accountBalance(Number(cash?.total ?? 0) + adjustmentTotal), colSpan: denominations.length, styles: { halign: 'center', fontStyle: 'bold' } },
          ],
        ];
        table([['Pièce (DH)', ...denominations.map((value) => value === '10000' ? '10K' : value === '1000' ? '1K' : value)]], cashRows, 1, cashRows.length - 1);
      }
      pdf.save(`rapport-${fromDate}-${toDate}.pdf`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Export PDF impossible.');
    } finally {
      setExporting(false);
    }
  }

  return (
    <div className="reports-page">
      <div className="report-toolbar no-print">
        <label className="form-field">Du<input type="date" value={fromDate} onChange={(event) => setFromDate(event.target.value)} /></label>
        <label className="form-field">Au<input type="date" value={toDate} onChange={(event) => setToDate(event.target.value)} /></label>
        <CircleButton title="Configurer le rapport" icon={Settings2} onClick={() => setDesignerOpen((open) => !open)} />
        <CircleButton title={exporting ? 'Création du PDF...' : 'Télécharger PDF'} icon={Download} onClick={exportPdf} />
      </div>

      {designerOpen && (
        <div className="report-designer no-print">
          <strong>Sections du rapport</strong>
          {([
            ['transactions', 'Tableau IN / OUT'],
            ['alimentations', 'Alimentations'],
            ['accounts', 'Extrait des comptes'],
            ['balances', 'Balances / KPI'],
            ['cash', 'Détails de caisse'],
          ] as Array<[keyof ReportSections, string]>).map(([key, label]) => (
            <label key={key}><input type="checkbox" checked={sections[key]} onChange={() => toggleSection(key)} /> {label}</label>
          ))}
          <small>Les informations société se modifient dans Paramètres.</small>
        </div>
      )}

      {error && <div className="transaction-feedback error no-print">{error}</div>}

      <article className="report-document">
        <header className="report-document-header">
          <div className="report-company">
            {company.logo ? <img src={company.logo} alt="" /> : <div className="report-logo-placeholder">LOGO</div>}
            <div>
              <h1>{company.name || 'Nom de la société'}</h1>
              {(company.address || company.city) && <p>{[company.address, company.city].filter(Boolean).join(' - ')}</p>}
              {(company.taxId || company.ice || company.rc) && <p>{[
                company.taxId && `IF ${company.taxId}`,
                company.ice && `ICE ${company.ice}`,
                company.rc && `RC ${company.rc}`,
              ].filter(Boolean).join(' · ')}</p>}
              {(company.phone || company.email || company.website) && <p>{[company.phone, company.email, company.website].filter(Boolean).join(' · ')}</p>}
            </div>
          </div>
          <div className="report-meta">
            <strong>Rapport du :</strong><b>{fromDate === toDate ? toDate : `${fromDate} au ${toDate}`}</b>
            <strong>Date d’impression :</strong><span>{new Date().toLocaleString('fr-MA')}</span>
          </div>
        </header>

        <div className="report-top-tables">
          {sections.transactions && (
            <section className="report-block">
              <h2>Mouvements généraux</h2>
              <table><thead><tr><th>Service</th><th>IN + F</th><th>OUT + F</th></tr></thead>
                <tbody>
                  {(report?.by_service ?? []).map((row) => <tr key={row.service}><th>{row.service}</th><td>{accountBalance(Number(row.IN) + Number(row.fees))}</td><td>{accountBalance(row.OUT)}</td></tr>)}
                  <tr className="report-total-row"><th>Total</th><td>{accountBalance(Number(report?.kpis.total_in ?? 0) + Number(report?.kpis.fees ?? 0))}</td><td>{accountBalance(report?.kpis.total_out ?? 0)}</td></tr>
                </tbody>
              </table>
            </section>
          )}
          {sections.alimentations && (
            <section className="report-block">
              <h2>Alimentations · Coffre → Caisse calculée</h2>
              <table><thead><tr><th>Montant</th><th>Date</th></tr></thead>
                <tbody>
                  {(report?.alimentations ?? []).map((row) => <tr key={row.id}><td>{accountBalance(row.amount)}</td><td>{new Date(row.occurred_at).toLocaleString('fr-MA')}</td></tr>)}
                  {!(report?.alimentations?.length) && <tr><td colSpan={2}>Aucune alimentation</td></tr>}
                </tbody>
              </table>
            </section>
          )}
        </div>

        {sections.accounts && (
          <section className="report-block report-wide-table">
            <h2>Extrait des comptes</h2>
            <table><thead><tr>{orderedAccounts.map((account) => <th key={account.id}>{account.name}</th>)}</tr></thead>
              <tbody><tr>{orderedAccounts.map((account) => <td key={account.id}>{accountBalance(account.balance)}</td>)}</tr></tbody>
            </table>
          </section>
        )}

        {sections.balances && (
          <section className="report-block report-wide-table">
            <h2>Balances</h2>
            <table><thead><tr>{visibleKpis.map((item) => <th key={item.label}>{item.label}</th>)}</tr></thead>
              <tbody><tr>{visibleKpis.map((item) => <td key={item.label}>{item.value === null ? '—' : accountBalance(item.value)}</td>)}</tr></tbody>
            </table>
          </section>
        )}

        {sections.cash && (
          <section className="report-block report-cash-table">
            <h2>Détail caisse</h2>
            <table>
              <tbody>
                <tr><th>Pièce (DH)</th>{denominations.map((value) => <th key={value}>{value === '10000' ? '10K' : value === '1000' ? '1K' : value}</th>)}</tr>
                <tr><th>Nombre</th>{denominations.map((value) => <td key={value}>{cash?.counts[value] ?? 0}</td>)}</tr>
                {cashAdjustments.counters.map((counter) => <tr key={counter.name}><th>{counter.name}</th><td colSpan={denominations.length}>{accountBalance(counter.total)}</td></tr>)}
                <tr className="report-cash-total"><th>Total caisse</th><td colSpan={denominations.length}>{accountBalance(Number(cash?.total ?? 0) + adjustmentTotal)}</td></tr>
              </tbody>
            </table>
          </section>
        )}
      </article>
    </div>
  );
}
