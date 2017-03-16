import { Component, Input, Output } from '@angular/core';
import { ResourceInfo } from '../interfaces/resourceInfo';

@Component({
    selector: 'resource-overview',
    template: `
        <h3>Resources</h3>
        <div class="break">
        </div>
        <div>
        <div *ngFor="let resource of resources" style="float:left;margin:5px;" >
            <p>{{resource.name}}</p>
            <button type="button" (click)="downloadResource(resource.name)">Download</button>
        </div>
        </div>
        <div style="clear:both;">
        </div>
        <resource-file-uploader [csClientId]="clientId"> </resource-file-uploader>

        
`
})
export class ResourceOverviewComponent {
    @Input('csResources')
    resources: ResourceInfo[];
    @Input('csClientId')
    clientId: string;
    downloadResource(file: string) {
        window.open('resource/' + this.clientId + '/' + file);
    }
}