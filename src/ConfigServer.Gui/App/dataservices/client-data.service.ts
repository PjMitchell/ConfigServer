import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { IConfigurationClient } from '../interfaces/client';
import 'rxjs/add/operator/toPromise';


export interface IConfigurationClientDataService 
{

}

@Injectable()
export class ConfigurationClientDataService 
{
    private clientsUrl = 'Clients';  // URL to web api

    constructor(private http: Http) { }

    getClients(): Promise<IConfigurationClient[]> {
        return this.http.get(this.clientsUrl)
            .toPromise()
            .then(response => response.json() as IConfigurationClient[])
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        //console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}