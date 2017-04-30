import { Component, Input, Output } from '@angular/core';
import { UploadDataService } from '../dataservices/upload-data.service';
import { IConfigurationSetSummary } from '../interfaces/configurationSetSummary';

@Component({
    selector: 'configSet-overview',
    template: `
        <div id="configset-overview-{{configurationSet.configurationSetId}}" class="group configset-overview">
            <div class="row">
                <div class="col-sm-6 col-md-8" >
                    <h3 class="configset-name">{{configurationSet.name}}</h3>
                    <p class="configset-description">Description: {{configurationSet.description}}</p>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-6 col-md-4" >
                    <json-file-uploader [(csMessage)]="uploadMessage" (onUpload)="uploadConfigSet($event)"></json-file-uploader>
                    <button type="button" class="btn btn-primary" (click)="downloadConfigSet(configurationSet.configurationSetId)"> <span class="glyphicon-btn glyphicon glyphicon-download-alt"></span> </button>
                </div>
            </div>
            <h3>Configurations</h3>
            <div class="row">
                <config-overview *ngFor="let config of configurationSet.configs" class="col-sm-6 col-md-4" [csConfig]="config" [csClientId]="clientId" [csConfigurationSetId]="configurationSet.configurationSetId">
                </config-overview>
            </div>
        </div>
`,
})
export class ConfigurationSetComponent {
    @Input('csConfigurationSet')
    public configurationSet: IConfigurationSetSummary;
    @Input('csClientId')
    public clientId: string;

    public uploadMessage: string;
    constructor(private uploadService: UploadDataService) {

    }

    public downloadConfigSet(configurationSetId: string) {
        window.open('download/' + this.clientId + '/' + configurationSetId + '.json');
    }

    public uploadConfigSet(value: any): void {
        this.uploadService.postConfigSet(this.clientId, this.configurationSet.configurationSetId, value)
                .then((success) => {
                    if (!success) {
                        this.uploadMessage = 'Invalid Config';
                    }
                });
        }
}
