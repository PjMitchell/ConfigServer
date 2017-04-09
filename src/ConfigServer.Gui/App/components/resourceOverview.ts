import { Component, Input, Output, EventEmitter} from '@angular/core';
import { ResourceInfo } from '../interfaces/resourceInfo';
import { ResourceDataService } from '../dataservices/resource-data.service';
@Component({
    selector: 'resource-overview',
    template: `
        <h3>Resources</h3>
        <div class="break">
        </div>
        <div class="row">
            <div *ngFor="let resource of resources" class="col-sm-6 col-md-4" >
                <p>{{resource.name}}</p>
                <button type="button" class="btn btn-primary" (click)="downloadResource(resource.name)"><span class="glyphicon-btn glyphicon glyphicon-download-alt"></span></button>
                <button type="button" class="btn btn-primary" (click)="delete(resource.name)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></button>
            </div>
        </div>
        <hr />
        <div class="row">        
            <resource-file-uploader [csClientId]="clientId" (onUpload)="onfileUploaded($event)" class="col-sm-6 col-md-4"> </resource-file-uploader>
        </div>        
`
})
export class ResourceOverviewComponent {
    @Input('csResources')
    resources: ResourceInfo[];
    @Input('csClientId')
    clientId: string;
    @Output('onResourcesChanged')
    onResourcesChanged: EventEmitter<any> = new EventEmitter<any>();

    constructor(private dataService: ResourceDataService) {

    }
    downloadResource(file: string) {
        window.open('resource/' + this.clientId + '/' + file);
    }

    delete(file: string) {
        var result = confirm('Are you sure you want to delete ' + file + '?');
        if (!result) {
            return
        }
        this.dataService.deleteResource(this.clientId, file).then(() => this.onResourcesChanged.emit());
    }

    onfileUploaded() {
        this.onResourcesChanged.emit();
    }
}