import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import { IArchivedConfigInfo } from '../interfaces/archivedConfigInfo';
import { IHttpRequestResult } from '../interfaces/httpRequestResult';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class ArchiveConfigService {

    private configArchiveUrl = 'Archive/';  // URL to web api
    constructor(private http: Http) { }

    public getArchivedConfig(clientId: string) {
        return this.http.get(this.configArchiveUrl + clientId)
            .toPromise()
            .then((response) => this.mapInfoArray(response.json()))
            .catch(this.handleError);
    }

    public deleteArchivedConfig(clientId: string, archiveConfigName: string) {
        return this.http.delete(this.configArchiveUrl + clientId)
            .toPromise()
            .then((response) => this.handleSuccess(response))
            .catch(this.handleError);
    }

    public deleteArchivedConfigBefore(clientId: string, date: Date): Promise<IHttpRequestResult> {
        return this.http.delete(this.configArchiveUrl + clientId + '?before=' + date.toISOString())
            .toPromise()
            .then((response) => this.handleSuccess(response))
            .catch(this.handleError);
    }

    private mapInfoArray(data: any): IArchivedConfigInfo[] {
        return (data as any[]).map((value) => {
            return {
                name: value.name,
                timeStamp: new Date(value.timeStamp),
                archiveTimeStamp: new Date(value.archiveTimeStamp),
                configuration: value.configuration,
                serverVersion: value.serverVersion,
            };
        });
    }

    private handleSuccess(response: Response) {
        return {
            suceeded: true,
        };
    }

    private handleError(error: Response) {
        if (error.status === 409) {
            return Promise.resolve({ suceeded: false, failureMessage: error.text() });
        }
        return Promise.reject(error.statusText || error);
    }
}
