import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationDataService } from '../dataservices/config-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { UploadDataService } from "../dataservices/upload-data.service";
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationModelPayload } from "../interfaces/configurationModelPayload";
import { IConfigurationSetModelPayload } from "../interfaces/configurationSetDefintion";

@Component({
    templateUrl: './client-config-shell.component.html',
})
export class ClientConfigShellComponent implements OnInit {
    public clientId: string;
    public configurationSetId: string;
    public configurationId: string;
    public sourceConfig: any;
    public config: any;
    public isDisabled: boolean;
    public client: IConfigurationClient;
    public configModel: IConfigurationModelPayload;
    public uploadMessage: string;
    public configurationModelType: 'config' | 'option';
    public form: FormGroup;
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private configDataService: ConfigurationDataService, private uploadDataService: UploadDataService, private route: ActivatedRoute, private router: Router, private formBuilder: FormBuilder) {
        this.form = this.formBuilder.group({});
    }

    public ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.configurationSetId = value['configurationSetId'];
            this.configurationId = value['configurationId'];
            this.clientDataService.getClient(this.clientId)
                .then((returnedClient) => this.client = returnedClient);
            this.configSetDataService.getConfigurationSetModel(this.configurationSetId, this.clientId)
                .then((returnedConfigSet) => this.onModelReturned(returnedConfigSet));
            this.configDataService.getConfig(this.clientId, this.configurationId)
                .then((returnedConfigSet) => this.onConfigReturned(returnedConfigSet));
        });
    }

    public onModelReturned(value: IConfigurationSetModelPayload) {
        const model = value.config[this.configurationId];
        if (model.isOption) {
            this.configurationModelType = 'option';
        } else {
            this.configurationModelType = 'config';
        }
        this.configModel = model;
    }

    public onConfigReturned(value: any) {
        this.config = value;
    }

    public save() {
        this.isDisabled = true;
        this.configDataService.postConfig(this.clientId, this.configurationId, this.form.value)
            .then((result) => {
                if (result.suceeded) {
                    this.back();
                } else {
                    alert(result.failureMessage);
                    this.isDisabled = false;
                }
            });
    }

    public back() {
        this.router.navigate(['/client', this.clientId]);
    }

    public uploadConfig(value: any) {

        this.uploadDataService.mapToEditor(this.clientId, this.configurationId, value)
            .then((result) => {
                this.config = result;
            });
    }
}
