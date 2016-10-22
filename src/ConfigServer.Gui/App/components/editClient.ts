import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { Router, ActivatedRoute,  } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    template: `
        <h2>Edit client</h2>
        <div *ngIf="client">
            <edit-client-input [(csClient)]="client"></edit-client-input>
            <div class="break">
            </div>
            <div>
               <button type="button" (click)="back()">Back</button>
               <button type="button" [disabled]="isDisabled" (click)="save()">Save</button>
            </div>
        </div>
`
})
export class EditClientComponent implements OnInit {
    client: ConfigurationClient;
    clientId: string;
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
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