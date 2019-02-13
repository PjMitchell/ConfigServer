import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { IResourceInfo } from '../interfaces/resourceInfo';
@Component({
    selector: 'resource-overview',
    templateUrl: './resource-overview.component.html',
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
