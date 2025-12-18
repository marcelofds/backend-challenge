import { useNavigate } from "@tanstack/react-router";
import { useTransactions } from "../hooks/useTransactions";
import { TransactionType } from "../api/transactions";

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value);
}

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
}

function getTransactionTypeLabel(type: TransactionType): string {
  const labels: Record<number, string> = {
    1: "Debit",
    2: "Credit",
    3: "Ticket",
    4: "Financing",
    5: "Loan",
    6: "Sale",
    7: "TED",
    8: "DOC",
    9: "Rent",
  };
  return labels[type] || `Type ${type}`;
}

export function TransactionsReportPage() {
  const navigate = useNavigate();
  const { data, isLoading, error, refetch } = useTransactions();

  if (isLoading) {
    return (
      <div className="report-page">
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Loading transactions...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="report-page">
        <div className="error-container">
          <h2>Error loading transactions</h2>
          <p>{error instanceof Error ? error.message : "Unknown error"}</p>
          <button className="retry-button" onClick={() => refetch()}>
            Try Again
          </button>
        </div>
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="report-page">
        <div className="empty-container">
          <h2>No transactions found</h2>
          <p>Upload a transaction file to view the report</p>
          <button
            className="link-button"
            onClick={() => navigate({ to: "/upload", replace: false })}
          >
            Go to Upload
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="report-page">
      <div className="report-header">
        <h1>Transaction Report</h1>
        <div className="header-actions">
          <button className="refresh-button" onClick={() => refetch()}>
            Refresh
          </button>
          <button
            className="link-button"
            onClick={() => navigate({ to: "/upload", replace: false })}
          >
            New Upload
          </button>
        </div>
      </div>

      <div className="transactions-list">
        {data.map((store, index) => (
          <div key={index} className="store-card">
            <div className="store-header">
              <h2 className="store-name">{store.storeName}</h2>
              <div className="store-balance">
                <span className="balance-label">Total Balance:</span>
                <span
                  className={`balance-value ${
                    store.totalBalance >= 0 ? "positive" : "negative"
                  }`}
                >
                  {formatCurrency(store.totalBalance)}
                </span>
              </div>
            </div>

            {store.operations.length > 0 && (
              <div className="operations-section">
                <h3>Operations ({store.operations.length})</h3>
                <div className="operations-table-container">
                  <table className="operations-table">
                    <thead>
                      <tr>
                        <th>Type</th>
                        <th>Date</th>
                        <th>Value</th>
                        <th>CPF</th>
                        <th>Card</th>
                        <th>Signed Value</th>
                      </tr>
                    </thead>
                    <tbody>
                      {store.operations.map((operation, opIndex) => (
                        <tr key={opIndex}>
                          <td>{getTransactionTypeLabel(operation.type)}</td>
                          <td>{formatDate(operation.date)}</td>
                          <td>{formatCurrency(operation.value)}</td>
                          <td>{operation.cpf}</td>
                          <td>{operation.card}</td>
                          <td
                            className={
                              operation.signedValue >= 0
                                ? "positive"
                                : "negative"
                            }
                          >
                            {formatCurrency(operation.signedValue)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
