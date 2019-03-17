import { Component, Input, Output } from '@angular/core';
import { UploadDataService } from '../dataservices/upload-data.service';
import { IConfigurationSetSummary } from '../interfaces/configurationSetSummary';

@Component({
    selector: 'configSet-overview',
    templateUrl: './configurationset-overview.component.html',
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
        window.open('Manager/Api/Download/' + this.clientId + '/' + configurationSetId + '.json');
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
