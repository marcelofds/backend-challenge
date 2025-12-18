# Finance Test Frontend

A modern web application for uploading and viewing financial transaction reports. This application allows users to upload transaction files in `.txt` format and view detailed reports with store balances and operations.

## Technologies Used

- **React 19.2.0** - UI library
- **TypeScript 5.9.3** - Type-safe JavaScript
- **Vite 7.2.4** - Build tool and development server
- **TanStack Query 5.90.12** - Server state management and data fetching
- **TanStack Router 1.141.2** - Type-safe routing
- **CSS3** - Styling with custom properties and modern features

## Features

- 📤 **File Upload**: Upload `.txt` transaction files with drag & drop support
- 📊 **Transaction Reports**: View detailed transaction reports with store balances
- 💰 **Currency Formatting**: Automatic Brazilian Real (BRL) currency formatting
- 📅 **Date Formatting**: Localized date and time display
- 🎨 **Modern UI**: Clean and responsive design with dark mode support
- ⚡ **Fast Navigation**: Type-safe routing with TanStack Router
- 🔄 **Real-time Updates**: Automatic data refresh after file upload

## Prerequisites

- Node.js (v18 or higher)
- pnpm (or npm/yarn)
- Backend API (URL configurable via environment variables)
- Docker and Docker Compose (optional, for containerized deployment)

## Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd FinanceTestFrontend
```

2. Install dependencies:
```bash
pnpm install
```

3. Configure environment variables:
```bash
cp .env.example .env
```

Edit `.env` and set your API base URL:
```env
VITE_API_BASE_URL=http://localhost:8080
```

## Development

Start the development server:

```bash
pnpm dev
```

The application will be available at `http://localhost:5173` (or the next available port).

## Building for Production

Build the application for production:

```bash
pnpm build
```

The production build will be in the `dist` directory.

Preview the production build:

```bash
pnpm preview
```

## Environment Variables

The application uses environment variables for configuration. Create a `.env` file in the root directory (you can copy from `.env.example`):

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Backend API base URL | `http://localhost:5289` |

**Note:** In Vite, environment variables must be prefixed with `VITE_` to be exposed to the client-side code.

## Docker Setup

### Using Docker Compose

The easiest way to run the application with Docker is using docker-compose:

1. Create a `.env` file (or use the existing one):
```bash
cp .env.example .env
```

2. Set the API base URL in `.env`:
```env
VITE_API_BASE_URL=http://localhost:8080
```

3. Build and run with docker-compose:
```bash
docker-compose up --build
```

The application will be available at `http://localhost:3000`.

To stop the containers:
```bash
docker-compose down
```

### Using Docker directly

1. Build the Docker image:
```bash
docker build --build-arg VITE_API_BASE_URL=http://localhost:8080/api -t finance-test-frontend .
```

2. Run the container:
```bash
docker run -p 3000:80 finance-test-frontend
```

The application will be available at `http://localhost:3000`.

## API Endpoints

The application connects to a backend API. The API URL is configurable via the `VITE_API_BASE_URL` environment variable (default: `http://localhost:5289/api`):

- **POST `/Transactions/upload`**: Upload a transaction file
  - Content-Type: `multipart/form-data`
  - Body: `file` (`.txt` file)

- **GET `/Transactions`**: Retrieve all transaction reports
  - Returns: Array of `StoreBalanceDto` objects

### Data Models

**StoreBalanceDto:**
```typescript
{
  storeName: string;
  totalBalance: number;
  operations: OperationDto[];
}
```

**OperationDto:**
```typescript
{
  type: TransactionType;
  date: string;
  value: number;
  cpf: string;
  card: string;
  signedValue: number;
}
```

## Project Structure

```
src/
├── api/
│   └── transactions.ts          # API functions and types
├── hooks/
│   └── useTransactions.ts        # TanStack Query hooks
├── components/
│   ├── FileUpload.tsx           # File upload component
│   ├── UploadPage.tsx           # Upload page
│   └── TransactionsReportPage.tsx # Report page
├── routes/
│   ├── __root.tsx               # Root route
│   ├── index.tsx                # Index route (redirects to /upload)
│   ├── upload.tsx               # Upload route
│   └── transactions.tsx         # Transactions route
├── styles/
│   └── global.css               # Global styles
├── App.tsx                      # Router configuration
└── main.tsx                     # Application entry point
```

## Usage

1. **Upload Transactions**:
   - Navigate to the upload page (default route)
   - Click or drag & drop a `.txt` file
   - Click "Upload File" to process
   - You'll be automatically redirected to the report page on success

2. **View Reports**:
   - The report page displays all stores with their balances
   - Each store shows:
     - Store name
     - Total balance (color-coded: green for positive, red for negative)
     - List of operations with details
   - Use "Refresh" to reload data
   - Use "New Upload" to upload another file

## Transaction Types

The application supports the following transaction types:
- Debit
- Credit
- Ticket
- Financing
- Loan
- Sale
- TED
- DOC
- Rent

## Styling

The application uses pure CSS with CSS custom properties for theming. It supports:
- Light and dark mode (based on system preferences)
- Responsive design for mobile and desktop
- Smooth animations and transitions
- Accessible color contrasts

## Linting

Run the linter:

```bash
pnpm lint
```

## License

This project is private and proprietary.
