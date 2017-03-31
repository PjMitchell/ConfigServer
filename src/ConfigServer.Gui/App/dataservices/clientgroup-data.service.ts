import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { ConfigurationClient } from '../interfaces/configurationClient';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ConfigurationClientGroupDataService {
    private clientGroupsUrl = 'ClientGroup';  // URL to web api

    constructor(private http: Http) { }

    getClientGroups(): Promise<ConfigurationClientGroup[]> {
        return this.http.get(this.clientGroupsUrl)
            .toPromise()
            .then(response => response.json() as ConfigurationClientGroup[])
            .catch(this.handleError);
    }

    getClientGroup(clientGroupId: string): Promise<ConfigurationClientGroup> {
        return this.http.get(this.clientGroupsUrl + '/' + clientGroupId)
            .toPromise()
            .then(response => response.json() as ConfigurationClientGroup)
            .catch(this.handleError);
    }

    getClientForGroup(clientGroupId?: string): Promise<ConfigurationClient[]> {
        let groupParam = clientGroupId;
        if(!groupParam)
            groupParam = 'None'
        return this.http.get(this.clientGroupsUrl + '/' + groupParam + '/Clients')
            .toPromise()
            .then(response => response.json() as ConfigurationClient[])
            .catch(this.handleError);
    }

    postClientGroup(clientGroup: ConfigurationClientGroup): Promise<boolean> {
        return this.http.post(this.clientGroupsUrl, clientGroup)
            .toPromise()
            .then(response => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        // console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}