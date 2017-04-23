import { Injectable } from '@angular/core';
import { Http, Response} from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';
import { IResourceInfo } from '../interfaces/resourceInfo';
@Injectable()
export class ResourceDataService {
    private resourceUrl = 'Resource';  // URL to web api

    constructor(private http: Http) { }

    public getClientResourceInfo(clientId: string): Promise<IResourceInfo[]> {
        return this.http.get(this.resourceUrl + '/' + clientId)
            .toPromise()
            .then((response) => response.json() as IResourceInfo[])
            .catch(this.handleError);
    }

    public uploadResource(clientId: string, fileName: string, data: any): Promise<IHttpRequestResult> {
        return this.http.post(this.resourceUrl + '/' + clientId + '/' + fileName, data)
            .toPromise()
            .then((response) => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    public deleteResource(clientId: string, fileName: string): Promise<IHttpRequestResult> {
        return this.http.delete(this.resourceUrl + '/' + clientId + '/' + fileName)
            .toPromise()
            .then((response) => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    private handlePostSuccess(response: Response) {
        return {
            suceeded: true,
        };
    }
    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
