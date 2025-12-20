import { fileURLToPath, URL } from 'node:url'
import fs from 'node:fs'
import path from 'node:path'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import electron from 'vite-plugin-electron/simple'

// https://vite.dev/config/
const isLocal = process.env.npm_lifecycle_event === 'local'
const isDeploy = process.env.npm_lifecycle_event === 'deploy'

// Load centralized config.json so we don't duplicate the remote host/port
const cfgPath = new URL('./config.json', import.meta.url)
const cfg = JSON.parse(fs.readFileSync(cfgPath, { encoding: 'utf-8' }))
const remoteApi = cfg.remoteApi
const localPort = cfg.localApiPort

// Allow explicit override via VITE_API_URL (set in npm script for local runs)
const proxyTarget = process.env.VITE_API_URL ?? (isLocal ? `http://localhost:${localPort}` : remoteApi);
const prologTarget = (isLocal ? `http://localhost:6000` : cfg.remotePrologApi);
const oemTarget = (isLocal ? `https://vs-gate.dei.isep.ipp.pt:30228` : cfg.remoteOemApi);

export default defineConfig({
  plugins: [
    vue({
      template: {
        compilerOptions: {
          // treat all tags with a dash as custom elements
          isCustomElement: (tag) => tag.includes('-'),
          whitespace: 'condense',
        }
      }
    }),
    // Only enable devtools in local/dev mode, not in deploy
    !isDeploy && vueDevTools(),
    // Electron plugin for desktop app
    process.env.ELECTRON && electron({
      main: {
        entry: 'electron/main.ts',
      },
      preload: {
        input: 'electron/preload.ts',
      },
      renderer: {},
    }),
  ].filter(Boolean),
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  publicDir: 'public',
  optimizeDeps: {
    // Exclude three.js examples and docs from dependency scanning
    entries: [
      'index.html',
      'public/visualizer/index.html'
    ],
  },
  server: {
    allowedHosts: true,
    // Proxy API calls to backend dev server to avoid browser TLS issues with self-signed certs
    
    proxy: {
        '/api': {
            target: proxyTarget,
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/api/, ''),
        },
        '/prolog': {
            target: prologTarget,
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/prolog/, ''),
        },
        '/oem': {
            target: oemTarget,
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/oem/, ''),
        }
    }
  },
})
