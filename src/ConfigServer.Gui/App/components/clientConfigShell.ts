import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationDataService } from '../dataservices/config-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationModelPayload } from "../interfaces/configurationModelPayload";
import { IConfigurationSetModelPayload } from "../interfaces/configurationSetDefintion";
import { UploadDataService } from "../dataservices/upload-data.service";

@Component({
    template: `
        <div *ngIf="client && configModel">
            <h2>{{client.name}}: {{configModel.name}}</h2>
            <p>{{configModel.description}}</p>
        </div>
        <div class="row">
            <div class="col-md-3">            
                <json-file-uploader [(csMessage)]="uploadMessage" (onUpload)="uploadConfig($event)"></json-file-uploader>
            </div>
        </div>
        <div class="validationResult"></div>
        <div class="break"></div>
        <form #configForm="ngForm">
            <div *ngIf="configModel && config">
                <div [ngSwitch]="configurationModelType" >
                    <config-input *ngSwitchCase="'config'" [csModel]="configModel" [(csConfig)]="config" ></config-input>
                    <config-option-input *ngSwitchCase="'option'" [csModel]="configModel" [(csCollection)]="config"></config-option-input>
                </div>
            </div>
            <div class="break"></div>
            <div >
                <button type="button" class="btn btn-primary" (click)="back()">Back</button>                
                <button *ngIf="configModel && config" [disabled]="isDisabled" type="button" class="btn btn-primary" (click)="save()">Save</button>
            </div>
        </form>
`,
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
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private configDataService: ConfigurationDataService, private uploadDataService: UploadDataService, private route: ActivatedRoute, private router: Router) {

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
        this.configDataService.postConfig(this.clientId, this.configurationId, this.config)
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
