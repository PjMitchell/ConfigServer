import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'config-property',
    template: `
            <div [class.col-md-3]="!isCollection" [class.col-md-12]="isCollection" style="min-height:140px">
                <config-property-item [csDefinition]="csDefinition" [(csConfig)]="csConfig">
                </config-property-item>
            </div>
`})
export class ConfigurationPropertyComponent {
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    public isCollection: boolean;

    private _csDefinition: IConfigurationPropertyPayload;
    @Input()
    get csDefinition(): IConfigurationPropertyPayload { return this._csDefinition; }
    set csDefinition(value: IConfigurationPropertyPayload) {
        this._csDefinition = value;
        this.isCollection = this._csDefinition.propertyType === 'Collection';
    }

}
