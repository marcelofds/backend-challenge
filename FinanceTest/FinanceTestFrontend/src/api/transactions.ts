const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5289";

export const TransactionType = {
  Debit: 1,
  Credit: 2,
  Ticket: 3,
  Financing: 4,
  Loan: 5,
  Sale: 6,
  TED: 7,
  DOC: 8,
  Rent: 9,
} as const;

export type TransactionType =
  (typeof TransactionType)[keyof typeof TransactionType];

export interface OperationDto {
  type: TransactionType;
  date: string;
  value: number;
  cpf: string;
  card: string;
  signedValue: number;
}

export interface StoreBalanceDto {
  storeName: string;
  totalBalance: number;
  operations: OperationDto[];
}

export async function uploadTransactionFile(file: File): Promise<void> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(`${API_BASE_URL}/Transactions/upload`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    if (response.status === 400) {
      const errorText = await response.text();
      throw new Error(errorText || "Error uploading file");
    }
    throw new Error(`Error uploading file: ${response.statusText}`);
  }
}

export async function getTransactions(): Promise<StoreBalanceDto[]> {
  const response = await fetch(`${API_BASE_URL}/Transactions`);

  if (!response.ok) {
    throw new Error(`Error fetching transactions: ${response.statusText}`);
  }

  return response.json();
}
