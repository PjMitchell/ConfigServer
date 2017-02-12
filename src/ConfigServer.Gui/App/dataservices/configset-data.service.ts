import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { ConfigurationModelSummary, ConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import { ConfigurationSetModelPayload } from '../interfaces/configurationSetDefintion';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationSetDataService 
{
    private configSetUrl = 'ConfigurationSet';  // URL to web api
    private configSetModelUrl = 'ConfigurationSet/Model/';  // URL to web api


    constructor(private http: Http) { }

    getConfigurationSets(): Promise<ConfigurationSetSummary[]> {
        return this.http.get(this.configSetUrl)
            .toPromise()
            .then(response => response.json() as ConfigurationSetSummary[])
            .catch(this.handleError);
    }

    getConfigurationSetModel(configurationSetName: string, clientId : string): Promise<ConfigurationSetModelPayload> {
        return this.http.get(this.configSetModelUrl + configurationSetName + '/' + clientId)
            .toPromise()
            .then(response => response.json() as ConfigurationSetModelPayload)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        //console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}