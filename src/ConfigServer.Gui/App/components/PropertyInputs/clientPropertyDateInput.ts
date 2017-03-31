import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'date-input',
    template: `
    <input  type="date" #input value="{{inputDate | date:'yyyy-MM-dd'}}" min="{{csDefinition.validationDefinition.min | date:'yyyy-MM-dd'}}" max="{{csDefinition.validationDefinition.max | date:'yyyy-MM-dd'}}" (blur)="onBlur()" required="{{csDefinition.validationDefinition.isRequired}}">`
})
export class ConfigurationPropertyDateInputComponent implements OnInit {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    input: IChildElement<HTMLInputElement>;

    inputDate: string;

    ngOnInit() {
        this.inputDate = this.csConfig[this.csDefinition.propertyName];
    }

    onBlur() {
        this.csConfig[this.csDefinition.propertyName] = this.input.nativeElement.value;
        this.csConfigChange.emit(this.csConfig);
    }
}