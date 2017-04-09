import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationModelPayload } from '../interfaces/configurationSetDefintion';

@Component({
    selector: 'config-input',
    template: `
        <div class="row">
            <config-property class="configProperty" *ngFor="let item of csModel.property | toIterator" [csDefinition]="item" [(csConfig)]="csConfig" >
            </config-property>
        </div>
`
})
export class ConfigurationInputComponent {
    @Input()
    csModel: ConfigurationModelPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}