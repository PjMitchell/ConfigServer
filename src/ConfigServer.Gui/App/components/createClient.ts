import { Component } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    template: `
        <h2>Create client</h2>
        <div>
            <edit-client-input [csAllClient]="clients" [(csClient)]="client"></edit-client-input>
            <div class="break">
            </div>
            <div>
               <button type="button" (click)="back()">Back</button>
               <button [disabled]="isDisabled" type="button" (click)="create()">Create</button>
            </div>
        </div>
`
})
export class CreateClientComponent {
    client: ConfigurationClient;
    clients: ConfigurationClient[];
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService, private router: Router) {
        this.client = {
            clientId: '',
            name: '',
            group: '',
            enviroment: '',
            description: ''
        };
    }

    ngOnInit() {
       this.clientDataService.getClients()
            .then(returnedClient => this.onAllClientsReturned(returnedClient));
    }

    onAllClientsReturned(returnedClients: ConfigurationClient[]) {
        this.clients = returnedClients;
    }

    create(): void {
        this.isDisabled;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }

    back() {
        this.router.navigate(['/']);
    }
}