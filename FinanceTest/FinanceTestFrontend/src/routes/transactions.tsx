import { createRoute } from "@tanstack/react-router";
import { rootRoute } from "./__root";
import { TransactionsReportPage } from "../components/TransactionsReportPage";

export const transactionsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/transactions",
  component: TransactionsReportPage,
});
