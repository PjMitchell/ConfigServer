import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
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

    postConfig(clientId: string, configId: string, config: any): Promise<boolean> {
        return this.http.post(this.configSetModelUrl + clientId + '/' + configId, config)
            .toPromise()
            .then(response => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        //console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}