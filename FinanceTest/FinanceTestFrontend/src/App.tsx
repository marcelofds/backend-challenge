import { createRouter, RouterProvider } from "@tanstack/react-router";
import { rootRoute } from "./routes/__root";
import { indexRoute } from "./routes/index";
import { uploadRoute } from "./routes/upload";
import { transactionsRoute } from "./routes/transactions";

const routeTree = rootRoute.addChildren([
  indexRoute,
  uploadRoute,
  transactionsRoute,
]);

const router = createRouter({ routeTree });

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

function App() {
  return <RouterProvider router={router} />;
}

export default App;
