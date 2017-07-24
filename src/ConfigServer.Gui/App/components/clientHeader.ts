import { Component, Input, Output } from '@angular/core';
import { IConfigurationClient } from "../interfaces/configurationClient";

@Component({
    selector: 'client-header',
    template: `
        <div>
            <h2 id="client-name">{{client.name}}</h2>
            <p id="client-id">Id: {{client.clientId}}</p>
            <p id="client-env">{{client.enviroment}}</p>
            <p id="client-desc">{{client.description}}</p>
        </div>
`,
})
export class ClientHeaderComponent {
    @Input('csClient')
    public client: IConfigurationClient;
}
