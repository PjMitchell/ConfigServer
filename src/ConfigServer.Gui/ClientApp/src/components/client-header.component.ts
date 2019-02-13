import { Component, Input } from '@angular/core';
import { IConfigurationClient } from "../interfaces/configurationClient";

@Component({
    selector: 'client-header',
    templateUrl: './client-header.component.html',
})
export class ClientHeaderComponent {
    @Input('csClient')
    public client: IConfigurationClient;
}
