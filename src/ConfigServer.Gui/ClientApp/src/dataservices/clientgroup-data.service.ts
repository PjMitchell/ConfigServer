import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Injectable()
export class ConfigurationClientGroupDataService {
    private clientGroupsUrl = 'Manager/Api/ClientGroup';  // URL to web api

    constructor(private http: Http) { }

    public getClientGroups(): Promise<IConfigurationClientGroup[]> {
        return this.http.get(this.clientGroupsUrl)
            .toPromise()
            .then((response) => response.json() as IConfigurationClientGroup[])
            .catch(this.handleError);
    }

    public getClientGroup(clientGroupId: string): Promise<IConfigurationClientGroup> {
        return this.http.get(this.clientGroupsUrl + '/' + clientGroupId)
            .toPromise()
            .then((response) => response.json() as IConfigurationClientGroup)
            .catch(this.handleError);
    }

    public getClientForGroup(clientGroupId?: string): Promise<IConfigurationClient[]> {
        let groupParam = clientGroupId;
        if (!groupParam) {
            groupParam = 'None';
        }
        return this.http.get(this.clientGroupsUrl + '/' + groupParam + '/Clients')
            .toPromise()
            .then((response) => response.json() as IConfigurationClient[])
            .catch(this.handleError);
    }

    public postClientGroup(clientGroup: IConfigurationClientGroup): Promise<boolean> {
        return this.http.post(this.clientGroupsUrl, clientGroup)
            .toPromise()
            .then((response) => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
