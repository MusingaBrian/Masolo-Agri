import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import path from "node:path";

// The C# WebView host loads this dev server in DEBUG builds,
// so the port must match WebViewHost.DevServerUrl.
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(import.meta.dirname, "./src"),
    },
  },
  server: {
    port: 5173,
  },
  build: {
    // The .NET host serves the bundled output from wwwroot.
    outDir: "../src/MasoloAgro.App/wwwroot",
    emptyOutDir: true,
  },
});
