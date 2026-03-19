import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import cesium from 'vite-plugin-cesium'

const apiTarget = process.env.VITE_API_URL || 'http://localhost:5000'

export default defineConfig({
  plugins: [react(), cesium()],
  server: {
    proxy: {
      '/api': apiTarget,
      '/ws': {
        target: apiTarget.replace(/^http/, 'ws'),
        ws: true,
      },
    },
  },
})
