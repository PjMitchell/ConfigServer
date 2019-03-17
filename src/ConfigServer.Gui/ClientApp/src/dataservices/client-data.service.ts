import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IConfigurationClient } from '../interfaces/configurationClient';

@Injectable()
export class ConfigurationClientDataService {
    private clientsUrl = 'Manager/Api/Clients';  // URL to web api

    constructor(private http: Http) { }

    public getClients(): Promise<IConfigurationClient[]> {
        return this.http.get(this.clientsUrl)
            .toPromise()
            .then((response) => response.json() as IConfigurationClient[])
            .catch(this.handleError);
    }

    public getClient(clientId: string): Promise<IConfigurationClient> {
        return this.http.get(this.clientsUrl + '/' + clientId)
            .toPromise()
            .then((response) => response.json() as IConfigurationClient)
            .catch(this.handleError);
    }

    public postClient(client: IConfigurationClient): Promise<boolean> {
        return this.http.post(this.clientsUrl, client)
            .toPromise()
            .then((response) => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
