import devConfig from '../config.json'

// Configuration loader that works in both dev and production
let configCache: any = null

async function loadConfig() {
  if (configCache) return configCache
  
  if (import.meta.env.DEV) {
    // Development: use bundled import
    configCache = devConfig
  } else {
    // Production: fetch from public folder
    try {
      const response = await fetch('/config.json')
      if (!response.ok) throw new Error('Config not found')
      configCache = await response.json()
    } catch (e) {
      console.warn('Failed to load config.json, using defaults')
      configCache = devConfig
    }
  }
  return configCache
}

// Expose configuration and a helper for choosing the API base URL.
// Priority order:
// 1. VITE_API_URL env variable (set at build time)
// 2. In production, use remoteApi from config.json (fetched at runtime)
// 3. In development, use relative `/api` so Vite dev server proxy handles it

export const REMOTE_API = devConfig.remoteApi as string
export const LOCAL_API_PORT = devConfig.localApiPort as number
export const DEFAULT_LOCAL_API = `http://localhost:${LOCAL_API_PORT}`

export async function getConfig() {
  return await loadConfig()
}

export async function getApiBase(): Promise<string> {
  if (import.meta.env.VITE_API_URL) {
    return import.meta.env.VITE_API_URL as string
  }
  
  if (import.meta.env.PROD) {
    const config = await loadConfig()
    return config.remoteApi
  }
  
  return '/api'
}

// Synchronous version for initializing axios baseURL
// In production, returns the remote API from config (not /api which won't work)
export function getApiBaseSync(): string {
  if (import.meta.env.VITE_API_URL) {
    return import.meta.env.VITE_API_URL as string
  }
  
  if (import.meta.env.PROD) {
    // In production, use remote API directly (config.json values)
    return REMOTE_API
  }
  
  return '/api'
}

export default {
  REMOTE_API,
  LOCAL_API_PORT,
  DEFAULT_LOCAL_API,
  getConfig,
  getApiBase,
  getApiBaseSync,
}
