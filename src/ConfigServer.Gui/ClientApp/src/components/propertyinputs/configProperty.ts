import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'config-property',
    template: `
            <div [class.col-md-3]="!fullWidth" [class.col-md-12]="fullWidth" style="min-height:140px">
                <config-property-item [csDefinition]="csDefinition" [csConfig]="csConfig" [parentForm]="parentForm" [csHasInfo]="!isCollection">
                </config-property-item>
            </div>
`})
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
