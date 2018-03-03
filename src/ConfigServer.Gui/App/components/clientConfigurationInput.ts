import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationModelPayload } from "../interfaces/configurationModelPayload";

@Component({
    selector: 'config-input',
    template: `
        <div class="row">
            <config-property class="configProperty" *ngFor="let item of csModel.property | toIterator;let i= index" [csDefinition]="item" [(csConfig)]="csConfig" (onIsValidChanged)="onValidChanged(i, $event)">
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
    @Output()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    private validation: boolean[] = new Array<boolean>();
    private onValidChanged(index: number, isValid: boolean) {
        this.validation[index] = isValid;
        this.onIsValidChanged.emit(this.validation.every((value) => value));
    }
}
