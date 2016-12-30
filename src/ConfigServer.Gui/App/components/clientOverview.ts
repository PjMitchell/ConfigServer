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
                <p>Id: {{client.clientId}}</p>
                <p>{{client.enviroment}}</p>
                <p>{{client.description}}</p>
        </div>
        <h3>ConfigurationSets</h3>
        <div class="break">
        </div>
        <configSet-overview class="group" *ngFor="let configurationSet of configurationSets" [csClientId]="client.clientId" [csConfigurationSet]="configurationSet" >
        </configSet-overview>
        <button type="button" (click)="back()">Back</button>
`
})
export class ClientOverviewComponent implements OnInit {
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
                .then(returnedConfigSet => this.configurationSets = returnedConfigSet);
        });
    }

    back() {
        this.router.navigate(['/']);
    }
}