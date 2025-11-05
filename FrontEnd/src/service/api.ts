import axios from 'axios'

// Q.A: Why is it "/api" ?? "/api" works because of Vite proxy settings in #vite.config.ts, I know, weird huh?
const BACKEND = "https://vs-gate.dei.isep.ipp.pt:10228"; 

export const api = axios.create({
  baseURL: BACKEND,
  withCredentials: true
})

export interface ApiResponse<T> {
  data: T
}