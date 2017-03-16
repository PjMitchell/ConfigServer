import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import { ResourceInfo } from '../interfaces/resourceInfo';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ResourceDataService {
    private resourceUrl = 'Resource';  // URL to web api

    constructor(private http: Http) { }

    getClientResourceInfo(clientId: string): Promise<ResourceInfo[]> {
        return this.http.get(this.resourceUrl + '/' + clientId)
            .toPromise()
            .then(response => response.json() as ResourceInfo[])
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        // console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}