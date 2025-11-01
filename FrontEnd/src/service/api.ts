import axios from 'axios'

// Q.A: Why is it "/api" ?? "/api" works because of Vite proxy settings in #vite.config.ts, I know, weird huh?
const BACKEND = import.meta.env.VITE_BACKEND_URL || '/api'

export const api = axios.create({
  baseURL: BACKEND,
})

export interface ApiResponse<T> {
  data: T
}