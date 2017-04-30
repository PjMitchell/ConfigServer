import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UploadDataService } from '../dataservices/upload-data.service';
import { IConfigurationModelSummary } from '../interfaces/configurationModelSummary';

@Component({
    selector: 'config-overview',
    template: `
            <div id="config-{{configurationSetId}}-{{config.id}}" class="item">
                <h4 class="config-name">{{config.displayName}}</h4>
                <p class="config-description">Description: {{config.description}}</p>
                <json-file-uploader [(csMessage)]="uploadMessage" (onUpload)="uploadConfig($event)"></json-file-uploader>
                <button type="button" class="btn btn-primary" (click)="downloadConfig(configurationSetId,config.id)"><span class="glyphicon-btn glyphicon glyphicon-download-alt"></span></button>
                <button type="button" class="btn btn-primary config-edit-btn" (click)="goToConfig(configurationSetId,config.id)">Edit</button>
            </div>
`,
})
export class ConfigurationOverviewComponent {
    @Input('csConfig')
    public config: IConfigurationModelSummary;
    @Input('csClientId')
    public clientId: string;
    @Input('csConfigurationSetId')
    public configurationSetId: string;

    public uploadMessage: string;
    constructor(private uploadService: UploadDataService, private router: Router) {

    }

    public goToConfig(configurationSetId: string, configId: string): void {
        this.router.navigate(['/client', this.clientId, configurationSetId, configId]);
    }

    public downloadConfig(configurationSetId: string, configId: string) {
        window.open('download/' + this.clientId + '/' + configurationSetId + '/' + configId + '.json');
    }

    public uploadConfig(value: any): void {
        this.uploadService.postConfig(this.clientId, this.config.id, value)
            .then((success) => {
                if (!success) {
                    this.uploadMessage = 'Invalid Config';
                }
            });
    }
}
