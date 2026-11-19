import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// In development, /api calls go to the .NET API (dotnet run --urls http://localhost:5080).
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: { '/api': 'http://localhost:5080' },
  },
  build: {
    outDir: '../api/wwwroot',
    emptyOutDir: true,
    rolldownOptions: {
      output: {
        // Long-lived vendor chunks: browsers keep them cached across app releases.
        codeSplitting: {
          groups: [
            { name: 'react', test: /node_modules[\\/](react|react-dom|scheduler|react-router|react-router-dom)[\\/]/ },
            { name: 'tanstack', test: /node_modules[\\/]@tanstack[\\/]/ },
            { name: 'motion', test: /node_modules[\\/](motion|motion-dom|motion-utils|framer-motion)[\\/]/ },
            { name: 'forms', test: /node_modules[\\/](react-hook-form|zod|@hookform)[\\/]/ },
            { name: 'ui', test: /node_modules[\\/](sweetalert2|cmdk|@radix-ui|lucide-react)[\\/]/ },
          ],
        },
      },
    },
  },
})
