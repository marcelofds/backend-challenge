import { createRoute } from "@tanstack/react-router";
import { rootRoute } from "./__root";
import { UploadPage } from "../components/UploadPage";

export const uploadRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/upload",
  component: UploadPage,
});
