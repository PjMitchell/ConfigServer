import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'date-input',
    template: `
    <input class="form-control"  type="date" #input value="{{inputDate | date:'yyyy-MM-dd'}}" min="{{csDefinition.validationDefinition.min | date:'yyyy-MM-dd'}}" max="{{csDefinition.validationDefinition.max | date:'yyyy-MM-dd'}}" (blur)="onBlur()" required="{{csDefinition.validationDefinition.isRequired}}">`,
})
export class ConfigurationPropertyDateInputComponent implements OnInit {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;

    public inputDate: string;

    public ngOnInit() {
        this.inputDate = this.csConfig[this.csDefinition.propertyName];
    }

    public onBlur() {
        this.csConfig[this.csDefinition.propertyName] = this.input.nativeElement.value;
        this.csConfigChange.emit(this.csConfig);
    }
}
