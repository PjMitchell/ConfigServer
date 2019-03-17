import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UploadDataService } from '../dataservices/upload-data.service';
import { IConfigurationModelSummary } from '../interfaces/configurationModelSummary';

@Component({
    selector: 'config-overview',
    templateUrl: './configuration-overview.component.html',
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
        window.open('Manager/Api/Download/' + this.clientId + '/' + configurationSetId + '/' + configId + '.json');
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
