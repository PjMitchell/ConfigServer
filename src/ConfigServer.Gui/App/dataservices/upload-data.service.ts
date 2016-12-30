import { Headers, Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class UploadDataService {
    constructor(private http: Http) { }
    private configUrl = 'Upload/Configuration/';  // URL to web api
    private configSetUrl = 'Upload/ConfigurationSet/';  // URL to web api

    postConfig(clientId: string, configId: string, config: any): Promise<boolean> {
        return this.http.post(this.configUrl + clientId + '/' + configId, config)
            .toPromise()
            .then(response => response.ok)
            .catch(this.handleError);
    }

    postConfigSet(clientId: string, configSetId: string, config: any): Promise<boolean> {
        return this.http.post(this.configSetUrl + clientId + '/' + configSetId, config)
            .toPromise()
            .then(response => response.ok)
            .catch(this.handleError);
    }

    private handleError(error: Response): Promise<any> {
        //console.error('An error occurred', error); 
        //Filter actual errors
        if (error.status === 422)
            return Promise.resolve(false);
        return Promise.reject(error);
    }
}