import { Component, Input, Output, EventEmitter} from '@angular/core';
import { ConfigurationPropertyPayload } from '../interfaces/configurationSetDefintion';


@Component({
    selector: 'config-property',
    template: `
            <div>
                <h3>{{csDefinition.propertyDisplayName}}</h3>
                <p>{{csDefinition.propertyDescription}}</p>
                <config-property-item [ngSwitch]="csDefinition.propertyType" [csDefinition]="csDefinition" [(csConfig)]="csConfig">
                </config-property-item>
            </div>
`
})
export class ConfigurationPropertyComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();    
}