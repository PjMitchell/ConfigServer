import { Component, Input, Output } from '@angular/core';
import { ConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import { UploadDataService } from '../dataservices/upload-data.service';

@Component({
    selector: 'configSet-overview',
    template: `
        <div class="group">
            <div class="row">
                <div class="col-sm-6 col-md-8" >
                    <h3>{{configurationSet.name}}</h3>
                    <p>Description: {{configurationSet.description}}</p>
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
`
})
export class ConfigurationSetComponent {
    @Input('csConfigurationSet')
    configurationSet: ConfigurationSetSummary;
    @Input('csClientId')
    clientId: string;

    uploadMessage: string;
    constructor(private uploadService: UploadDataService) {

    }

    downloadConfigSet(configurationSetId: string) {
        window.open('download/' + this.clientId + '/' + configurationSetId + '.json');
    }

    uploadConfigSet(value: any): void {
        this.uploadService.postConfigSet(this.clientId, this.configurationSet.configurationSetId, value)
                .then(success => {
                    if (!success)
                        this.uploadMessage = 'Invalid Config';
                });
        }
}