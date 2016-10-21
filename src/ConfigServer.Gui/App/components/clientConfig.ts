import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ConfigurationDataService } from '../dataservices/config-data.service';

import { ActivatedRoute, Params } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';
import { ConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import { ConfigurationModelPayload, ConfigurationSetModelPayload } from '../interfaces/configurationSetDefintion';



@Component({
    template: `
        <div *ngIf="client && configModel">
            <h2>{{client.name}}: {{configModel.name}}</h2>
            <p>{{configModel.description}}</p>
        </div>
        <div *ngIf="configModel && config"> 
            <config-property *ngFor="let item of configModel.property | toIterator" [csDefinition]="item" [(csConfig)]="config" >
            </config-property>
        </div>
        <div *ngIf="configModel && config">
            <button type="button" (click)="save()">Save</button>
        </div>
`
})
export class ClientConfigComponent implements OnInit {
    clientId: string;
    configurationSetId: string;
    configurationId: string;
    sourceConfig: any;
    config: any;
    client: ConfigurationClient;
    configModel: ConfigurationModelPayload;
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private configDataService: ConfigurationDataService, private route: ActivatedRoute) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.configurationSetId = value['configurationSetId'];
            this.configurationId = value['configurationId'];

            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
            this.configSetDataService.getConfigurationSetModel(this.configurationSetId)
                .then(returnedConfigSet => this.onModelReturned(returnedConfigSet));
            this.configDataService.getConfig(this.clientId, this.configurationId)
                .then(returnedConfigSet => this.onConfigReturned(returnedConfigSet));
        })
    }

    onModelReturned(value: ConfigurationSetModelPayload) {
        this.configModel = value.config[this.configurationId]
    }

    onConfigReturned(value: any) {
        this.config = value;
    }

    save() {
        var result = this.config;
        this.configDataService.postConfig(this.clientId, this.configurationId, this.config);
    }
}