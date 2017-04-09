import { Component, OnInit } from '@angular/core';
import { ConfigurationClientGroupDataService } from '../dataservices/clientGroup-data.service';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { Group } from '../interfaces/configurationSetDefintion';
import { Router,ActivatedRoute } from '@angular/router';

@Component({
    template: `
            <div class="row">
                <div class="col-sm-6 col-md-4">
                    <div *ngIf="group.imagePath"><img class="img-responsive" src="Resource/ClientGroupImages/{{group.imagePath}}" /></div>
                </div>
                <div class="col-sm-6 col-md-8">
                    <h3>Group: {{group.name}}</h3>
                    <h4>Id: {{group.groupId}}</h4>
                </div>
            </div>               
            <hr />
            <div class="row">
                <div class="col-sm-6 col-md-4"  *ngFor="let client of clients">
                    <div  class="thumbnail">
                        <h3>{{client.name}}</h3>
                        <p>Id: {{client.clientId}}</p>
                        <p>{{client.enviroment}}</p>
                        <p>{{client.description}}</p>
                        <button type="button" class="btn btn-primary" (click)="goToClient(client.clientId)">Manage configurations</button>
                        <button type="button" class="btn btn-primary" (click)="editClient(client.clientId)">Edit client</button>
                    </div>
                </div>
            </div>
            <button type="button" class="btn btn-primary" (click)="back()">Back</button>
`
})
export class GroupClientsComponent  implements OnInit{
    groupId : string;
    isSpecifiedGroup : boolean = false;
    clients : ConfigurationClient[];
    group : ConfigurationClientGroup;
    constructor(private clientDataService: ConfigurationClientGroupDataService, private router: Router,private route: ActivatedRoute ) {
        this.clients = new Array<ConfigurationClient>();
        this.group = {
            groupId : '',
            name: 'loading...',
            imagePath: ''
        };
    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.groupId = value['groupId'];
        });
        if(!this.groupId) {
            this.clientDataService.getClientForGroup().then((result)=> { this.clients = result; });
            this.isSpecifiedGroup = false;
            this.group = {
                        groupId : '',
                        name: 'No group',
                        imagePath: ''
            };
        } else {
            this.isSpecifiedGroup = true;
            this.clientDataService.getClientForGroup(this.groupId).then((result)=> { this.clients = result; });
            this.clientDataService.getClientGroup(this.groupId).then((result)=> { this.group = result; });            
        }
    }

    goToClient(clientId: string) {
        this.router.navigate(['/client', clientId]);
    }

    editClient(clientId: string) {
        this.router.navigate(['/editClient', clientId]);
    }

    back() {
        this.router.navigate(['/']);
    }
}