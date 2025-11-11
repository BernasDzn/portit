export interface Response<T> {
	status : number;
	statusText: string;
	data: T;
}

export interface Headers {
	[key: string]: 
		| string 
		| {
			[key: string]: string;
		};
	headers: {
		[key: string]: string;
	};
}

export interface IGetOptions {
	headers?: {
		[key: string]: string;
	}
	params?: {
		[key: string]: string;
	};
}

export interface IHttpService {
	get<T>(url: string, options?: IGetOptions): Promise<Response<T>>;
    getWithoutCredentials<T>(url: string, options?: IGetOptions): Promise<Response<T>>;
	post<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>>;
	put<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>>;
	patch<T>(url: string, data: unknown, headers?: Headers): Promise<Response<T>>;
	delete<T>(url: string, headers?: Headers): Promise<Response<T>>;
}