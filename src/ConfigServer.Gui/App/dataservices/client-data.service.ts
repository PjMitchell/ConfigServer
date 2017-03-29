import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import { ConfigurationClient } from '../interfaces/configurationClient';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationClientDataService {
    private clientsUrl = 'Clients';  // URL to web api

    constructor(private http: Http) { }

    getClients(): Promise<ConfigurationClient[]> {
        return this.http.get(this.clientsUrl)
            .toPromise()
            .then(response => response.json() as ConfigurationClient[])
            .catch(this.handleError);
    }

    getClient(clientId: string): Promise<ConfigurationClient> {
        return this.http.get(this.clientsUrl + '/' + clientId)
            .toPromise()
            .then(response => response.json() as ConfigurationClient)
            .catch(this.handleError);
    }

    postClient(client: ConfigurationClient): Promise<boolean> {
        return this.http.post(this.clientsUrl, client)
            .toPromise()
            .then(response => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        // console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}