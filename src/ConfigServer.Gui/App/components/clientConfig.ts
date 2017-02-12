import { Component, OnInit, ViewChild } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ConfigurationDataService } from '../dataservices/config-data.service';

import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';
import { ConfigurationModelPayload, ConfigurationSetModelPayload } from '../interfaces/configurationSetDefintion';



@Component({
    template: `
        <div *ngIf="client && configModel">
            <h2>{{client.name}}: {{configModel.name}}</h2>
            <p>{{configModel.description}}</p>
        </div>
        <div class="validationResult">
        </div>
        <div class="break">
        </div>
        <div *ngIf="configModel && config"> 
            <config-property class="configProperty" *ngFor="let item of configModel.property | toIterator" [csDefinition]="item" [(csConfig)]="config" >
            </config-property>
        </div>
        <div class="break">
        </div>
        <div>
            <button type="button" (click)="back()">Back</button>
            <button *ngIf="configModel && config" [disabled]="isDisabled" type="button" (click)="save()">Save</button>
        </div>
`
})
export class ClientConfigComponent implements OnInit {
    clientId: string;
    configurationSetId: string;
    configurationId: string;
    sourceConfig: any;
    config: any;
    isDisabled: boolean;
    client: ConfigurationClient;
    configModel: ConfigurationModelPayload;

    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private configDataService: ConfigurationDataService, private route: ActivatedRoute, private router: Router) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.configurationSetId = value['configurationSetId'];
            this.configurationId = value['configurationId'];

            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
            this.configSetDataService.getConfigurationSetModel(this.configurationSetId, this.clientId)
                .then(returnedConfigSet => this.onModelReturned(returnedConfigSet));
            this.configDataService.getConfig(this.clientId, this.configurationId)
                .then(returnedConfigSet => this.onConfigReturned(returnedConfigSet));
        });
    }

    onModelReturned(value: ConfigurationSetModelPayload) {
        this.configModel = value.config[this.configurationId];
    }

    onConfigReturned(value: any) {
        this.config = value;
    }

    save() {
        this.isDisabled = true;
        this.configDataService.postConfig(this.clientId, this.configurationId, this.config)
            .then(result => {
                if (result.suceeded) {
                    this.back();
                }
                else {
                    alert(result.failureMessage);
                    this.isDisabled = false;
                }
            });
    }

    back() {
        this.router.navigate(['/client', this.clientId]);
    }
}