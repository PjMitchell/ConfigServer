import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationDataService 
{
    constructor(private http: Http) { }
    private configSetModelUrl = 'ConfigurationSet/Value/';  // URL to web api
    getConfig(clientId: string, configId: string): Promise<any> {
        return this.http.get(this.configSetModelUrl + clientId + '/' + configId)
            .toPromise()
            .then(response => response.json())
            .catch(this.handleError);
    }

    postConfig(clientId: string, configId: string, config: any): Promise<IHttpRequestResult> {
        return this.http.post(this.configSetModelUrl + clientId + '/' + configId, config)
            .toPromise()
            .then(response => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    private handlePostSuccess(response: Response) {
        return {
            suceeded: true
        };
    }

    private handleError(error: Response): Promise<IHttpRequestResult> {
        //console.error('An error occurred', error); 
        if (error.status === 409)
            return Promise.resolve({ suceeded: false, failureMessage: error.text() })
        return Promise.reject(error.statusText || error);
    }
}