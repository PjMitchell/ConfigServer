import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationModelPayload } from "../interfaces/configurationModelPayload";

@Component({
    selector: 'config-input',
    template: `
        <div class="row">
            <config-property class="configProperty" *ngFor="let item of csModel.property | toIterator" [csDefinition]="item" [(csConfig)]="csConfig" >
            </config-property>
        </div>
`,
})
export class ConfigurationInputComponent {
    @Input()
    public csModel: IConfigurationModelPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
