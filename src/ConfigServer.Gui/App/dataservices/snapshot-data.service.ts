import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { ICreateSnapshotRequest } from "../interfaces/createSnapshotRequest";
import { IHttpRequestResult } from "../interfaces/httpRequestResult";
import { ISnapshotInfo } from "../interfaces/snapshotInfo";

@Injectable()
export class SnapshotDataService {
    constructor(private http: Http) { }

    public saveSnapShot(request: ICreateSnapshotRequest): Promise<IHttpRequestResult> {
        return this.http.post('Snapshot', request)
            .toPromise()
            .then((response) => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    public getSnapshots(groupId: string): Promise<ISnapshotInfo[]> {
        return this.http.get('Snapshot/Group/' + groupId)
            .toPromise()
            .then((response) => this.mapSnapshotInfoArray(response.json() as ISnapshotInfoPayload[]))
            .catch(this.handleError);
    }

    public deleteSnapShot(snapshotId: string): Promise<IHttpRequestResult> {
        return this.http.delete('Snapshot/' + snapshotId)
            .toPromise()
            .then((response) => this.handlePostSuccess(response))
            .catch(this.handleError);
    }

    public pushSnapshotToClient(snapshotId: string, clientId: string, configsToCopy: string[]) {
        return this.http.post('Snapshot/' + snapshotId + '/to/' + clientId, { configsToCopy })
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

    private mapSnapshotInfoArray(data: ISnapshotInfoPayload[]): ISnapshotInfo[] {
        return (data).map((value) => {
            return { id: value.id, groupId: value.groupId, name: value.name, timeStamp: new Date(value.timeStamp) };
        });
    }
}

interface ISnapshotInfoPayload {
    id: string;
    name: string;
    groupId: string;
    timeStamp: string;
}

interface IPushSnapshotToClientRequest {
    configsToCopy: string[];
}
