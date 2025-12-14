import axios from 'axios';
import type { AxiosInstance } from 'axios';
import type { IHttpService, Response, Headers, IGetOptions } from './IService/IHttpService';
import { api } from './api';
import { useSession } from '@/composables/session';
import TYPES from '@/inversify/types';
import { inject, injectable } from 'inversify';
import devConfig from '../../config.json';

const session = useSession();

@injectable()
export class AxiosHttpService implements IHttpService {
    private axiosInstance: AxiosInstance;

    constructor() {
        
        if (this.axiosInstance) return;
        
        this.axiosInstance = axios.create(
        { 
            withCredentials: true
        });

        this.axiosInstance.interceptors.request.use(
            
            (config) => {
            // Add auth token to headers if available
            if (session.authToken) {
                config.headers.Authorization = `Bearer ${session.authToken}`;
            }

            // In production, rewrite URLs to point to the correct backend
            if (import.meta.env.PROD && config.url) {
                if (config.url.startsWith('/api/')) {
                    // Strip /api and use main backend
                    config.url = devConfig.remoteApi + config.url.substring(4);
                } else if (config.url.startsWith('/oem/')) {
                    // Strip /oem and use OEM backend
                    config.url = devConfig.remoteOemApi + config.url.substring(4);
                } else if (config.url.startsWith('/prolog/')) {
                    // Strip /prolog and use Prolog backend
                    config.url = devConfig.remotePrologApi + config.url.substring(7);
                }
            }

            return config;
            },
            (error) => {
            return Promise.reject(error);
            }
        );
  }
    
    async getWithoutCredentials<T>(url: string, options?: IGetOptions): Promise<Response<T>> {
        
        const res = await axios.get<T>(
            url, 
            { 
                params: options?.params, 
                headers: options?.headers
            } as any)
        return this.toResponse(res);
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
    console.log('POST to ', url, ' with data ', JSON.stringify(data));
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