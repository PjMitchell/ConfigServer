import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClient } from '../interfaces/client';
import { Group } from '../interfaces/configurationSetDefintion';
import { Router } from '@angular/router';
import { GroupTransitService } from '../dataservices/group-transit.service';

@Component({
    template: `
    <div class="container">
    <h2>Clients</h2>
    <button type="button" class="btn btn-default" (click)="createNew()">Create</button>
    <div class="row">
        <div class="col-sm-6 col-md-4"  *ngFor="let group of clients">
            <div class="thumbnail list-unstyled">
                <div><img class="img-responsive" src="Assets/img/config-monkey.jpeg" /></div>
                <div class="category"></div>
                <div class="caption">
                    <h3>{{group.key}}</h3>
                    <span *ngFor="let client of group.items">
                        {{client.name}}; 
                    </span>
                    <hr />
                    <p>
                        <button type="button" class="btn btn-primary" (click)="editGroupClients(group)">
                            Edit
                        </button>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>
`
})
export class HomeComponent implements OnInit {
    clients: Group<string, ConfigurationClient>[];

    constructor(private clientDataService: ConfigurationClientDataService, private router: Router, private groupTransitService: GroupTransitService) {

    }

    ngOnInit(): void {
        this.clientDataService.getClients()
            .then(returnedClients => this.mapClients(returnedClients));
    }

    createNew() {
        this.router.navigate(['/createClient']);
    }

    editGroupClients(selectedGroup: Group<string, ConfigurationClient>) {
        this.groupTransitService.selectedGroup = selectedGroup;
        this.router.navigate(['/editGroupClients']);
    }

    mapClients(value: ConfigurationClient[]): void {
        var grouping = new Array<ConfigurationClient[]>();
        value.forEach((item) => {
            var group: ConfigurationClient[] = grouping[item.group];
            if (!group)
                group = new Array<ConfigurationClient>();

            group.push(item);
            grouping[item.group] = group
        });
        var keys = Object.keys(grouping);
        var items = new Array<Group<string, ConfigurationClient>>()
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            var val: ConfigurationClient[] = grouping[key];
            if (key == 'null')
                key = '';
            items.push({ 'key': key, 'items': val });
        }
        this.clients = items
    }
}