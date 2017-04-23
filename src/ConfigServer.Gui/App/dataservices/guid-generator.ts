import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';
import { IResourceInfo } from '../interfaces/resourceInfo';
@Injectable()
export class GuidGenerator {
    constructor(private http: Http) { }

    public getGuid(): Promise<string> {
        return this.http.post('GenerateGuid', null)
            .toPromise()
            .then((response) => response.json() as string)
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
