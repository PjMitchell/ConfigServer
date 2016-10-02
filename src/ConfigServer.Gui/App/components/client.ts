import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';
import { ConfigurationSetSummary } from '../interfaces/configurationSetSummary';


@Component({
    template: `
        <div *ngIf="client">
            <h2>{{client.name}}</h2>
            <p>{{client.description}}</p>
        </div>
        <h3>ConfigurationSets</h3>
        <div *ngFor="let configurationSet of configurationSets">
            <h4>{{configurationSet.name}}</h4>
            <p>{{configurationSet.description}}</p>
            <h4>Configurations</h4>
            <div *ngFor="let config of configurationSet.configs">
                <h5>{{config.displayName}}</h5>
                <p>{{config.description}}</p>
            </div>
        </div>
`
})
export class ClientComponent implements OnInit {
    client: ConfigurationClient;
    configurationSets: ConfigurationSetSummary[];
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private route: ActivatedRoute) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            let clientId = value['clientId'];
            this.clientDataService.getClient(clientId)
                .then(returnedClient => this.client = returnedClient);
            this.configSetDataService.getConfigurationSets()
                .then(returnedConfigSet => this.configurationSets = returnedConfigSet)
        })
    }
}