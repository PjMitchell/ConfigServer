import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';
import { ConfigurationSetSummary } from '../interfaces/configurationSetSummary';


@Component({
    template: `
        <div *ngIf="client">
            <h2>{{client.name}}</h2>
            <p>{{client.description}}</p>
        </div>
        <h3>ConfigurationSets</h3>
        <div class="break">
        </div>
        <div class="group" *ngFor="let configurationSet of configurationSets">
            <h4>{{configurationSet.name}}</h4>
            <p>{{configurationSet.description}}</p>
            <h4>Configurations</h4>
            <div class="item" *ngFor="let config of configurationSet.configs">
                <h5>{{config.displayName}}</h5>
                <p>{{config.description}}</p>
                <button type="button" (click)="goToConfig(configurationSet.configurationSetId,config.id)">Edit</button>
            </div>
        </div>
        <button type="button" (click)="back()">Back</button>
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

    back() {
        this.router.navigate(['/'])
    }
}