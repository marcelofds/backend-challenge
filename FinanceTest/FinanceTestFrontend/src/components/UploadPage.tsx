import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { FileUpload } from "./FileUpload";
import { useUploadTransaction } from "../hooks/useTransactions";

export function UploadPage() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const navigate = useNavigate();
  const uploadMutation = useUploadTransaction();

  const handleFileSelect = (file: File) => {
    setSelectedFile(file);
  };

  const handleUpload = async () => {
    if (!selectedFile) return;

    try {
      await uploadMutation.mutateAsync(selectedFile);
      // Navigate to transactions page after success
      navigate({ to: "/transactions", replace: true });
    } catch (error) {
      // Error will be handled by the component through the mutation state
      console.error("Error uploading file:", error);
    }
  };

  return (
    <div className="upload-page">
      <div className="upload-container">
        <h1>Transaction Upload</h1>
        <p className="page-description">
          Upload a .txt file containing transactions for processing
        </p>

        <FileUpload
          onFileSelect={handleFileSelect}
          disabled={uploadMutation.isPending}
        />

        {uploadMutation.isError && (
          <div className="error-message">
            {uploadMutation.error instanceof Error
              ? uploadMutation.error.message
              : "Error uploading file"}
          </div>
        )}

        <button
          className="upload-button"
          onClick={handleUpload}
          disabled={!selectedFile || uploadMutation.isPending}
        >
          {uploadMutation.isPending ? "Uploading..." : "Upload File"}
        </button>

        <div className="navigation-links">
          <button
            className="link-button"
            onClick={() => navigate({ to: "/transactions", replace: false })}
          >
            View Transaction Report
          </button>
        </div>
      </div>
    </div>
  );
}
