import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
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
                <button type="button" (click)="goToConfig(configurationSet.configurationSetId,config.id)">Edit</button>
            </div>
        </div>
        <div>
            <button type="button" (click)="save()">Save</button>
        </div>
`
})
export class ClientComponent implements OnInit {
    client: ConfigurationClient;
    clientId: string;
    configurationSets: ConfigurationSetSummary[];
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private route: ActivatedRoute, private router: Router) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
            this.configSetDataService.getConfigurationSets()
                .then(returnedConfigSet => this.configurationSets = returnedConfigSet)
        })
    }

    goToConfig(configurationSetId: string, configId: string): void {
        this.router.navigate(['/client', this.clientId, configurationSetId, configId])
    }
    save() {
        var result = this.configurationSets;
    }
}