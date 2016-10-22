import { Component } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    template: `
        <h2>Create client</h2>
        <div>
            <edit-client-input [(csClient)]="client"></edit-client-input>
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
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService, private router: Router) {
        this.client = {
            clientId: '',
            name: '',
            description: ''
        };
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