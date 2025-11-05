import axios from 'axios'
import { getApiBase } from '@/config.ts'

const BACKEND = getApiBase()

export const api = axios.create({
  baseURL: BACKEND,
  withCredentials: true
})

export interface ApiResponse<T> {
  data: T
}