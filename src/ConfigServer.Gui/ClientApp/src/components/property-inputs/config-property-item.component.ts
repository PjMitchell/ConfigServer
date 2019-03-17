import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'config-property-item',
    templateUrl: './config-property-item.component.html',
})
export class ConfigurationPropertyInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csHasInfo: boolean;
    @Input()
    public csConfig: any;
    @Input()
    public parentForm: FormGroup;

}
