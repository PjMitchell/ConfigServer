import { Component, Input, Output } from '@angular/core';
import { ResourceInfo } from '../interfaces/resourceInfo';

@Component({
    selector: 'resource-overview',
    template: `
        <h3>Resources</h3>
        <div>
        <p *ngFor="let resource of resources" style="display:block;float:left">{{resource.name}}</p>
        </div>
        <div class="break">
        </div>
        
`
})
export class ResourceOverviewComponent {
    @Input('csResources')
    resources: ResourceInfo[];
    @Input('csClientId')
    clientId: string;
}