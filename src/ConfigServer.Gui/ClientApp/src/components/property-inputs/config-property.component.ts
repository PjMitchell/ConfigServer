import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'config-property',
    templateUrl: './config-property.component.html',
})
export class ConfigurationPropertyComponent {
    @Input()
    public csConfig: any;
    public fullWidth: boolean;
    public isCollection: boolean;

    @Input()
    public parentForm: FormGroup;

    private _csDefinition: IConfigurationPropertyPayload;
    @Input()
    get csDefinition(): IConfigurationPropertyPayload { return this._csDefinition; }
    set csDefinition(value: IConfigurationPropertyPayload) {
        this._csDefinition = value;
        this.isCollection = this._csDefinition.propertyType === 'Collection';
        this.fullWidth = this.isCollection || this._csDefinition.propertyType === 'Class';
    }
}
