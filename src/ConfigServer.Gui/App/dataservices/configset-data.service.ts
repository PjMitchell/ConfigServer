import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import { IConfigurationSetModelPayload } from '../interfaces/configurationSetDefintion';
import {  IConfigurationSetSummary } from '../interfaces/configurationSetSummary';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationSetDataService {
    private configSetUrl = 'ConfigurationSet';  // URL to web api
    private configSetModelUrl = 'ConfigurationSet/Model/';  // URL to web api

    constructor(private http: Http) { }

    public getConfigurationSets(): Promise<IConfigurationSetSummary[]> {
        return this.http.get(this.configSetUrl)
            .toPromise()
            .then((response) => response.json() as IConfigurationSetSummary[])
            .catch(this.handleError);
    }

    public getConfigurationSetModel(configurationSetName: string, clientId: string): Promise<IConfigurationSetModelPayload> {
        return this.http.get(this.configSetModelUrl + clientId + '/' + configurationSetName)
            .toPromise()
            .then((response) => response.json() as IConfigurationSetModelPayload)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
