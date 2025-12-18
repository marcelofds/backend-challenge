import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getTransactions, uploadTransactionFile } from "../api/transactions";

export function useUploadTransaction() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (file: File) => uploadTransactionFile(file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["transactions"] });
    },
  });
}

export function useTransactions() {
  return useQuery({
    queryKey: ["transactions"],
    queryFn: getTransactions,
  });
}
