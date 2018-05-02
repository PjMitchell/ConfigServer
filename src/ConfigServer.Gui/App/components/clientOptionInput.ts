﻿import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { IConfigurationModelPayload } from '../interfaces/configurationModelPayload';
import { uniqueKey } from '../validators/collectionValidator';

@Component({
    selector: 'config-option-input',
    template: `
        <table>
            <tr>
                <th *ngFor="let p of csModel.property | toIterator">{{p.propertyDisplayName}}</th>
                <th class="column-btn">
                    <app-icon-button color="accent" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></app-icon-button>
                </th>
            </tr>
            <tr *ngFor="let item of csCollection;let i= index">
                <td *ngFor="let itemProperty of csModel.property | toIterator;let c= index">
                    <config-property-item [csDefinition]="itemProperty" [csConfig]="csCollection[i]" [parentForm]="collectionForms.controls[i]" [csHasInfo]="false"></config-property-item>
                </td>
                <td class="column-btn">
                    <app-icon-button color="warn" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
                </td>
            </tr>
        </table>
        <div *ngIf="collectionForms" >
            <p class="errorMessage"  *ngIf="!collectionForms.valid">{{getErrorMessage()}}</p>
        </div>
`,
})
export class OptionInputComponent implements OnInit {
    @Input()
    public csModel: IConfigurationModelPayload;
    @Input()
    public csCollection: any[] = new Array<any>();
    @Input()
    public parentForm: FormGroup;
    public collectionForms: FormArray;
    constructor(private formBuilder: FormBuilder) {
        this.collectionForms = formBuilder.array([]);
    }

    public ngOnInit() {
        this.collectionForms.setValidators(uniqueKey(this.csModel.keyPropertyName));
        this.csCollection.forEach((value, i) => {
            this.collectionForms.setControl(i, this.formBuilder.group({}));
        });
        this.parentForm.setControl("root", this.collectionForms);
    }

    public add() {
        const newItem = new Object();
        const keys = Object.keys(this.csModel.property);
        keys.forEach((value) => {
            newItem[value] = '';
        });
        this.csCollection.push(newItem);
        const index = this.csCollection.indexOf(newItem);
        this.collectionForms.setControl(index, this.formBuilder.group({}));
    }

    public remove(item: any) {
        const index = this.csCollection.indexOf(item);
        this.csCollection.splice(index, 1);
        this.collectionForms.controls.splice(index, 1);
    }

    public customTrackBy(index: number, obj: any): any {
        return index;
    }

    public getErrorMessage() {
        if (this.collectionForms) {
            return this.collectionForms.hasError('duplicate') ? 'Duplicate Keys:' + this.collectionForms.getError('duplicate') : '';
        }
        return '';
    }
}
