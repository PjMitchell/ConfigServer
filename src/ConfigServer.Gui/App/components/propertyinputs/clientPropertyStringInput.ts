import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'string-input',
    template: `<input class="form-control" [(ngModel)]="csConfig[csDefinition.propertyName]" type="text" pattern="{{csDefinition.validationDefinition.pattern}}" maxlength="{{csDefinition.validationDefinition.maxLength}}">`,
})
export class ConfigurationPropertyStringInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<string> = new EventEmitter<any>();
}
