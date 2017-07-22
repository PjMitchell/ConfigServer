import { Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { ICreateSnapshotRequest } from "../interfaces/createSnapshotRequest";
import { IHttpRequestResult } from "../interfaces/httpRequestResult";

@Injectable()
export class SnapshotDataService {
    constructor(private http: Http) { }

    public saveSnapShot(request: ICreateSnapshotRequest): Promise<IHttpRequestResult> {
        return this.http.post('Snapshot', request)
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