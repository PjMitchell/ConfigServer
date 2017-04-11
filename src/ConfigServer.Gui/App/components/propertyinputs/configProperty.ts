import { Component, Input, Output, EventEmitter} from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';


@Component({
    selector: 'config-property',
    template: `
            <div [class.col-sm-4]="!isCollection" [class.col-md-3]="!isCollection" [class.col-md-12]="isCollection" style="min-height:140px">
                <h3>{{csDefinition.propertyDisplayName}}</h3>
                <p>{{csDefinition.propertyDescription}}</p>
                <config-property-item [csDefinition]="csDefinition" [(csConfig)]="csConfig">
                </config-property-item>
            </div>
`
})
export class ConfigurationPropertyComponent {
    _csDefinition : ConfigurationPropertyPayload;
    @Input()
    get csDefinition() : ConfigurationPropertyPayload { return this._csDefinition;};
    set csDefinition(value : ConfigurationPropertyPayload) {
        this._csDefinition = value;
        this.isCollection = this._csDefinition.propertyType === 'Collection';
    }
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();

    isCollection: boolean;

}