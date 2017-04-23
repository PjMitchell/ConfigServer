import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationDataService {

    private configSetModelUrl = 'ConfigurationSet/Value/';  // URL to web api

    constructor(private http: Http) { }
    public getConfig(clientId: string, configId: string): Promise<any> {
        return this.http.get(this.configSetModelUrl + clientId + '/' + configId)
            .toPromise()
            .then((response) => response.json())
            .catch(this.handleError);
    }

    public postConfig(clientId: string, configId: string, config: any): Promise<IHttpRequestResult> {
        return this.http.post(this.configSetModelUrl + clientId + '/' + configId, config)
            .toPromise()
            .then((response) => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    private handlePostSuccess(response: Response) {
        return {
            suceeded: true,
        };
    }

    private handleError(error: Response): Promise<IHttpRequestResult> {
        if (error.status === 409) {
            return Promise.resolve({ suceeded: false, failureMessage: error.text() });
        }
        return Promise.reject(error.statusText || error);
    }
}
