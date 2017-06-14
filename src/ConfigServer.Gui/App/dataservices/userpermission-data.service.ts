import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { IUserClientPermissions } from '../interfaces/userClientPermissions';
import { IUserPermissions } from '../interfaces/userPermissions';

@Injectable()
export class UserPermissionService {
    constructor(private http: Http) { }

    public getPermission(): Promise<IUserPermissions> {
        return this.http.get('UserPermissions')
            .toPromise()
            .then((response) => response.json() as IUserPermissions)
            .catch(this.handleError);
    }

    public getPermissionForClient(client: string): Promise<IUserClientPermissions> {
        return this.http.get('UserPermissions/' + client)
            .toPromise()
            .then((response) => response.json() as IUserClientPermissions)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
