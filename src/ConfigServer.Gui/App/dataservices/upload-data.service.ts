import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class UploadDataService {

    private configUrl = 'Upload/Configuration/';  // URL to web api
    private configSetUrl = 'Upload/ConfigurationSet/';  // URL to web api
    private configEditorUrl = 'Upload/Editor/';  // URL to web api

    constructor(private http: Http) { }

    public postConfig(clientId: string, configId: string, config: any): Promise<boolean> {
        return this.http.post(this.configUrl + clientId + '/' + configId, config)
            .toPromise()
            .then((response) => response.ok)
            .catch(this.handleError);
    }

    public postConfigSet(clientId: string, configSetId: string, config: any): Promise<boolean> {
        return this.http.post(this.configSetUrl + clientId + '/' + configSetId, config)
            .toPromise()
            .then((response) => response.ok)
            .catch(this.handleError);
    }

    public mapToEditor(clientId: string, configId: string, config: any): Promise<any> {
        return this.http.post(this.configEditorUrl + clientId + '/' + configId, config)
            .toPromise()
            .then((response) => response.json())
            .catch(this.handleError);
    }

    private handleError(error: Response): Promise<any> {
        // Filter actual errors
        if (error.status === 422) {
            return Promise.resolve(false);
        }
        return Promise.reject(error);
    }
}
