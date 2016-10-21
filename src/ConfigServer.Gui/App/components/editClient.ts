import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { Router, ActivatedRoute,  } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    template: `
        <h2>Edit client</h2>
        <div *ngIf="client">
            <edit-client-input [(csClient)]="client"></edit-client-input>
            <div>
               <button type="button" (click)="save()">Save</button>
            </div>
        </div>
`
})
export class EditClientComponent implements OnInit{
    client: ConfigurationClient;
    clientId: string;
    constructor(private clientDataService: ConfigurationClientDataService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
        })
    }

    save(): void {
        this.clientDataService.postClient(this.client);
    }
}