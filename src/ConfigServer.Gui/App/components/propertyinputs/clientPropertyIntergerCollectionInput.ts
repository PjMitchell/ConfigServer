import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { uniqueKey } from '../../validators/collectionValidator';

@Component({
    selector: 'interger-collection-input',
    template: `
<table>
    <tr>
        <th>{{csDefinition.propertyDisplayName}}</th>
        <th class="column-btn">
            <app-icon-button color="accent" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></app-icon-button>
        </th>
    </tr>
    <tr *ngFor="let item of collection;let i= index">
        <td>
            <mat-form-field class="full-width">
                <input matInput  placeholder="" [formControl]="collectionForms.controls[i]" type= "number">
                <mat-error *ngIf="collectionForms.controls[i].invalid">{{getErrorMessage()}}</mat-error>
            </mat-form-field>
        </td>
        <td class="column-btn">
            <app-icon-button color="warn" (click)="remove(i)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
        </td>
    </tr>
</table>
<div *ngIf="collectionForms" >
    <p class="errorMessage"  *ngIf="!collectionForms.valid">{{getErrorMessage()}}</p>
</div>
`,
})
export class ConfigurationPropertyIntergerCollectionInputComponent implements OnInit {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Input()
    public parentForm: FormGroup;
    public collection: any[];
    public collectionForms: FormArray;
    constructor(private formBuilder: FormBuilder) {
        this.collectionForms = formBuilder.array([]);
    }

    public ngOnInit() {
        let collection: number[] = this.csConfig[this.csDefinition.propertyName];
        if (!collection) {
            collection = new Array<number>();
        }
        // if (this.csDefinition.keyPropertyName) {
        //     this.collectionForms.setValidators(uniqueKey(this.csDefinition.keyPropertyName));
        // }
        collection.forEach((value, i) => {
            this.collectionForms.setControl(i, this.buildControl(value));
        });
        this.parentForm.setControl(this.csDefinition.propertyName, this.collectionForms);
        this.collection = collection;
    }

    public add() {
        const newItem = 0;
        const index = this.collection.push(newItem) - 1;
        this.collectionForms.setControl(index, this.buildControl(newItem) );
    }

    public remove(index: number) {
        this.collection.splice(index, 1);
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

    private buildControl(value: number) {
        return this.formBuilder.control(value);
    }
}
