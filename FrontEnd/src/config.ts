import raw from '../config.json'

// Expose configuration and a helper for choosing the API base URL.
// Priority order:
// 1. VITE_API_URL env variable (set by npm script for local runs)
// 2. In production, use remoteApi from config.json
// 3. In development, use relative `/api` so Vite dev server proxy handles it
export const REMOTE_API = raw.remoteApi as string
export const LOCAL_API_PORT = raw.localApiPort as number
export const DEFAULT_LOCAL_API = `http://localhost:${LOCAL_API_PORT}`

export function getApiBase(): string {
  return (import.meta.env.VITE_API_URL as string) ?? (import.meta.env.PROD ? REMOTE_API : '/api')
}

export default {
  REMOTE_API,
  LOCAL_API_PORT,
  DEFAULT_LOCAL_API,
  getApiBase,
}
