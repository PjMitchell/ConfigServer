import { Http, Response} from '@angular/http';
import { Injectable } from '@angular/core';
import { ResourceInfo } from '../interfaces/resourceInfo';
import 'rxjs/add/operator/toPromise';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';
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

    uploadResource(clientId: string, fileName: string, data: any): Promise<IHttpRequestResult> {
        return this.http.post(this.resourceUrl + '/' + clientId + '/' + fileName, data)
            .toPromise()
            .then(response => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    deleteResource(clientId: string, fileName: string): Promise<IHttpRequestResult> {
        return this.http.delete(this.resourceUrl + '/' + clientId + '/' + fileName)
            .toPromise()
            .then(response => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    private handlePostSuccess(response: Response) {
        return {
            suceeded: true
        };
    }
    private handleError(error: any): Promise<any> {
        // console.error('An error occurred', error); 
        return Promise.reject(error.message || error);
    }
}