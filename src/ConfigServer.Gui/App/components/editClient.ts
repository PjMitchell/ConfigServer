import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { Router, ActivatedRoute,  } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    template: `
        <h2>Edit client</h2>
        <div *ngIf="client">
            <edit-client-input [csAllClient]="clients" [(csClient)]="client"></edit-client-input>
            <hr />
            <div>
               <button type="button" class="btn btn-primary" (click)="back()">Back</button>
               <button type="button" class="btn btn-success" [disabled]="isDisabled" (click)="save()">Save</button>
            </div>
        </div>
`
})
export class EditClientComponent implements OnInit {
    client: ConfigurationClient;
    clients: ConfigurationClient[];

    clientId: string;
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClients()
                .then(returnedClient => this.onAllClientsReturned(returnedClient));
        });
    }

    onAllClientsReturned(returnedClients: ConfigurationClient[]) {
        this.clients = returnedClients
        returnedClients.filter((value) => value.clientId === this.clientId).forEach((value) => {
            this.client = value;
        });
    }

    save(): void {
        this.isDisabled = true;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }



    back() {
        this.router.navigate(['/']);
    }
}