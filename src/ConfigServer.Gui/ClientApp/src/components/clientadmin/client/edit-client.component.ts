import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../../../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../../../dataservices/clientgroup-data.service';
import { TagDataService } from '../../../dataservices/tag-data.service';
import { IConfigurationClient } from '../../../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../../../interfaces/configurationClientGroup';
import { ITag } from '../../../interfaces/tag';

@Component({
    templateUrl: './edit-client.component.html',
})
export class EditClientComponent implements OnInit {
    public client: IConfigurationClient;
    public clients: IConfigurationClient[];
    public groups: IConfigurationClientGroup[];
    public tags: ITag[];
    public clientId: string;
    public isDisabled: boolean = false;
    public isValid: boolean = true;
    constructor(private clientDataService: ConfigurationClientDataService, private clientGroupDataService: ConfigurationClientGroupDataService, private tagDataService: TagDataService, private route: ActivatedRoute, private router: Router) {
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClients()
                .then((returnedClient) => this.onAllClientsReturned(returnedClient));
        });
        this.clientGroupDataService.getClientGroups().then((grp) => {this.groups = grp; });
        this.tagDataService.getTags().then((results) => {this.tags = results; });
    }

    public onAllClientsReturned(returnedClients: IConfigurationClient[]) {
        this.clients = returnedClients;
        returnedClients.filter((value) => value.clientId === this.clientId).forEach((value) => {
            this.client = value;
        });
    }

    public save(): void {
        this.isDisabled = true;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }

    public back() {
        this.router.navigate(['/']);
    }
}
