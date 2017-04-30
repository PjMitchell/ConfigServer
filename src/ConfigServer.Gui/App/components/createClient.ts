import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { GuidGenerator } from '../dataservices/guid-generator';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { IConfigurationClientSetting } from '../interfaces/configurationClientSetting';

@Component({
    template: `
        <h2>Create client</h2>
        <h4 id="client-id">{{client.clientId}}</h4>
        <div>
            <edit-client-input [csAllClient]="clients" [(csClient)]="client" [csExistingGroups]="groups" [(csIsValid)]="isValid"></edit-client-input>
            <hr />
            <div>
               <button type="button"  class="btn btn-primary"(click)="back()">Back</button>
               <button id="save-btn" [disabled]="isDisabled || !isValid" type="button" class="btn btn-success" (click)="create()">Create</button>
            </div>
        </div>
`,
})
export class CreateClientComponent {
    public client: IConfigurationClient;
    public clients: IConfigurationClient[];
    public groups: IConfigurationClientGroup[];
    public isValid: boolean = true;
    public isDisabled: boolean = false;
    constructor(private clientDataService: ConfigurationClientDataService, private clientGroupDataService: ConfigurationClientGroupDataService, private guidGenerator: GuidGenerator, private router: Router) {
        this.client = {
            clientId: '',
            name: '',
            group: '',
            enviroment: '',
            description: '',
            settings: new Array<IConfigurationClientSetting>(),
        };
    }

    public ngOnInit() {
        this.isDisabled = true;
        this.clientDataService.getClients()
            .then((returnedClient) => this.onAllClientsReturned(returnedClient));
        this.clientGroupDataService.getClientGroups().then((grp) => {this.groups = grp; });
        this.guidGenerator.getGuid()
            .then((g) => {
                this.client.clientId = g;
                this.isDisabled = false;
            });
    }

    public onAllClientsReturned(returnedClients: IConfigurationClient[]) {
        this.clients = returnedClients;
    }

    public create(): void {
        this.isDisabled = true;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }

    public back() {
        this.router.navigate(['/']);
    }
}
