import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { ConfigurationModelSummary, ConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationSetDataService 
{
    private clientsUrl = 'ConfigurationSet';  // URL to web api

    constructor(private http: Http) { }

    getConfigurationSets(): Promise<ConfigurationSetSummary[]> {
        return this.http.get(this.clientsUrl)
            .toPromise()
            .then(response => response.json() as ConfigurationSetSummary[])
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        //console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}