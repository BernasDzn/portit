import axios from 'axios';
import type { AxiosInstance } from 'axios';
import type { IHttpService, Response, Headers, IGetOptions } from './IService/IHttpService';
import { api } from './api';
import { useSession } from '@/composables/session';

const session = useSession();

export class AxiosHttpService implements IHttpService {
  private axiosInstance: AxiosInstance;

  constructor(instanceOrBase?: AxiosInstance | string) {
    if (!instanceOrBase) {
      this.axiosInstance = api as AxiosInstance;
    } else {
      this.axiosInstance = axios.create(
        { 
            baseURL: instanceOrBase as string ,
        });
    }

    this.axiosInstance.interceptors.request.use(

        
      (config) => {
        // Add auth token to headers if available
        if (session.authToken) {
          config.headers.Authorization = `Bearer ${session.authToken}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );
  }

  private toResponse<T>(res: import('axios').AxiosResponse<T>): Response<T> {
    return { status: res.status, statusText: res.statusText, data: res.data, payload: res.data } as Response<T>;
  }

  async get<T>(url: string, options?: IGetOptions): Promise<Response<T>> {
    const res = await this.axiosInstance.get<T>(
        url, 
        { 
            params: options?.params, 
            headers: options?.headers
        } as any);
    return this.toResponse(res);
  }

  async post<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>> {
    const res = await this.axiosInstance.post<T>(
      url,
      data,
      {
        headers: {
            'Content-Type': 'application/json',
            ...headers,
        },
      } as any
    );
    return this.toResponse(res);
  }
  

  async put<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>> {
    const res = await this.axiosInstance.put<T>(
        url,
        data,
        {
            headers: {
                'Content-Type': 'application/json',
                ...headers,
            },
        } as any
    );
    return this.toResponse(res);
  }

  async patch<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>> {
    const res = await this.axiosInstance.patch<T>(
        url,
        data,
        {
            headers: {
                'Content-Type': 'application/json',
                ...headers,
            },
        } as any
    );
    return this.toResponse(res);
  }

  async delete<T>(url: string, headers?: Headers): Promise<Response<T>> {
    const res = await this.axiosInstance.delete<T>(url, { headers } as any);
    return this.toResponse(res);
  }
}

export default AxiosHttpService;