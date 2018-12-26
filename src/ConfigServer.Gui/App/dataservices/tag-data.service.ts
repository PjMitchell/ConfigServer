import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { Tag } from '../interfaces/tag';

@Injectable()
export class TagDataService {
    private tagsUrl = 'Tags';  // URL to web api

    constructor(private http: Http) { }

    public getTags(): Promise<Tag[]> {
        return this.http.get(this.tagsUrl)
            .toPromise()
            .then((response) => response.json() as IConfigurationClient[])
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}