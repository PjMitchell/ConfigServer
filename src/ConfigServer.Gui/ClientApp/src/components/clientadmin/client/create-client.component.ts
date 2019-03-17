import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientDataService } from '../../../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../../../dataservices/clientgroup-data.service';
import { GuidGenerator } from '../../../dataservices/guid-generator';
import { TagDataService } from '../../../dataservices/tag-data.service';
import { IConfigurationClient } from '../../../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../../../interfaces/configurationClientGroup';
import { IConfigurationClientSetting } from '../../../interfaces/configurationClientSetting';
import { ITag } from '../../../interfaces/tag';

@Component({
    templateUrl: './create-client.component.html',
})
export class CreateClientComponent {
    public client: IConfigurationClient;
    public clients: IConfigurationClient[];
    public groups: IConfigurationClientGroup[];
    public tags: ITag[];
    public isValid: boolean = true;
    public isDisabled: boolean = false;
    constructor(private clientDataService: ConfigurationClientDataService, private clientGroupDataService: ConfigurationClientGroupDataService, private tagDataService: TagDataService, private guidGenerator: GuidGenerator, private router: Router) {
        this.client = {
            clientId: '',
            name: '',
            group: '',
            enviroment: '',
            description: '',
            readClaim: '',
            configuratorClaim: '',
            settings: new Array<IConfigurationClientSetting>(),
            tags: new Array<ITag>(),
        };
    }

    public ngOnInit() {
        this.isDisabled = true;
        this.clientDataService.getClients()
            .then((returnedClient) => this.onAllClientsReturned(returnedClient));
        this.clientGroupDataService.getClientGroups().then((grp) => {this.groups = grp; });
        this.tagDataService.getTags().then((results) => {this.tags = results; });
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
