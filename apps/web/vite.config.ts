import { URL, fileURLToPath } from "node:url";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@fetchxmlbuilder/core": fileURLToPath(
        new URL("../../packages/core/src/index.ts", import.meta.url),
      ),
      "@fetchxmlbuilder/dataverse": fileURLToPath(
        new URL("../../packages/dataverse/src/index.ts", import.meta.url),
      ),
      "@fetchxmlbuilder/storage": fileURLToPath(
        new URL("../../packages/storage/src/index.ts", import.meta.url),
      ),
    },
  },
  server: {
    host: "localhost",
    port: 5173,
    strictPort: true,
  },
  preview: {
    host: "localhost",
    port: 5173,
    strictPort: true,
  },
});
