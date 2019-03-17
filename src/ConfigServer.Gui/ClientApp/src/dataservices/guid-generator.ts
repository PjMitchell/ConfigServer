import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class GuidGenerator {
    constructor(private http: Http) { }

    public getGuid(): Promise<string> {
        return this.http.post('Manager/Api/GenerateGuid', null)
            .toPromise()
            .then((response) => response.json() as string)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
