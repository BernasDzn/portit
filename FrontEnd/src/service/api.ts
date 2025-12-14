import axios from 'axios'
import { getApiBaseSync } from '@/config.ts'

const BACKEND = getApiBaseSync()

export const api = axios.create({
  baseURL: BACKEND,
  withCredentials: true
})

export interface ApiResponse<T> {
  data: T
}