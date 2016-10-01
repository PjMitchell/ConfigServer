import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ActivatedRoute, Params } from '@angular/router';
import { IConfigurationClient } from '../interfaces/client';

@Component({
    template: `
        <div *ngIf="client">
            <h2>{{client.name}}</h2>
            <p>{{client.description}}</p>
        </div>
`
})
export class ClientComponent implements OnInit {
    client: IConfigurationClient;

    constructor(private clientDataService: ConfigurationClientDataService, private route: ActivatedRoute) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            let clientId = value['clientId'];
            this.clientDataService.getClient(clientId)
                .then(returnedClient => this.client = returnedClient);
        })
    }
}