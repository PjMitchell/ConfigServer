﻿import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { IResourceInfo } from '../interfaces/resourceInfo';
@Component({
    selector: 'resource-overview',
    template: `
        <h3>Resources</h3>
        <button type="button" mat-raised-button color="primary" (click)="goToArchive()">Archive</button>
        <button type="button" mat-raised-button color="primary" *ngIf="isConfigurator" (click)="gotoCopy()">Copy</button>
        <div class="break"></div>
        <div class="row">
            <div *ngFor="let resource of resources" class="col-sm-6 col-md-4" >
                <p>{{resource.name}}</p>
                <p>Created:{{resource.timeStamp | date:"MM/dd/yy" }}</p>
                <app-icon-button color="primary" (click)="downloadResource(resource.name)"><span class="glyphicon-btn glyphicon glyphicon-download-alt"></span></app-icon-button>
                <app-icon-button color="warn" (click)="delete(resource.name)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
            </div>
        </div>
        <hr />
        <div class="row" *ngIf="isConfigurator">
            <resource-file-uploader [csClientId]="clientId" (onUpload)="onfileUploaded($event)" class="col-sm-6 col-md-4"> </resource-file-uploader>
        </div>
`,
})
export class ResourceOverviewComponent {
    @Input('csResources')
    public resources: IResourceInfo[];
    @Input('csClientId')
    public clientId: string;
    @Input('csIsConfigurator')
    public isConfigurator: boolean;
    @Output('onResourcesChanged')
    public onResourcesChanged: EventEmitter<any> = new EventEmitter<any>();

    constructor(private dataService: ResourceDataService, private router: Router) {

    }
    public downloadResource(file: string) {
        window.open('resource/' + this.clientId + '/' + file);
    }

    public delete(file: string) {
        const result = confirm('Are you sure you want to delete ' + file + '?');
        if (!result) {
            return;
        }
        this.dataService.deleteResource(this.clientId, file).then(() => this.onResourcesChanged.emit());
    }

    public onfileUploaded() {
        this.onResourcesChanged.emit();
    }

    public goToArchive(): void {
        this.router.navigate(['/resourceArchive', this.clientId]);
    }

    public gotoCopy() {
        this.router.navigate(['/copyResource', this.clientId]);
    }
}
