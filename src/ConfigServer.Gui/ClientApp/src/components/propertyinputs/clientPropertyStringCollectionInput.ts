import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { uniqueItem } from '../../validators/collectionValidator';

@Component({
    selector: 'string-collection-input',
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
                <input matInput  placeholder="" [formControl]="collectionForms.controls[i]" type= "text">
                <mat-error *ngIf="collectionForms.controls[i].invalid">{{getErrorMessage(i)}}</mat-error>
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
export class ConfigurationPropertyStringCollectionInputComponent implements OnInit {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public parentForm: FormGroup;
    public collection: string[];
    public collectionForms: FormArray;
    private _config: any;
    @Input()
    public get csConfig() {
        return this._config;
    }
    public set csConfig(value: any) {
        this._config = value;
        this.setupForm();
    }

    constructor(private formBuilder: FormBuilder) {
        this.collectionForms = formBuilder.array([]);
    }

    public ngOnInit() {
        this.setupForm();
    }

    public add() {
        const newItem = '';
        const index = this.collection.push(newItem) - 1;
        this.collectionForms.setControl(index, this.buildControl(newItem) );
    }

    public remove(index: number) {
        this.collection.splice(index, 1);
        this.collectionForms.removeAt(index);
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

    public getItemErrorMessage(index: number) {
        return this.collectionForms[index].hasError('required') ? 'You must enter a value' :
            this.collectionForms[index].hasError('maxLength') ? 'Value must be fewer than ' + this.csDefinition.validationDefinition.maxLength + ' char' :
            this.collectionForms[index].hasError('pattern') ? 'Value match ' + this.csDefinition.validationDefinition.pattern :
                '';
    }

    private setupForm() {
        let collection: string[] = this.csConfig[this.csDefinition.propertyName];
        if (!collection) {
            collection = new Array<string>();
        }

        collection.forEach((value, i) => {
            this.collectionForms.setControl(i, this.buildControl(value));
        });
        this.parentForm.setControl(this.csDefinition.propertyName, this.collectionForms);
        this.collection = collection;
        if (this.csDefinition.validationDefinition && !this.csDefinition.validationDefinition.allowDuplicates) {
            this.collectionForms.setValidators(uniqueItem());
        }
    }

    private buildControl(value: string) {
        return this.formBuilder.control(value, this.getValidators());
    }

    private getValidators() {
        const validators = new Array<ValidatorFn>();
        if (this.csDefinition.validationDefinition.isRequired) {
            validators.push(Validators.required);
        }
        const pattern = this.csDefinition.validationDefinition.pattern;
        if (pattern) {
            validators.push(Validators.pattern(pattern));
        }
        const max = this.csDefinition.validationDefinition.maxLength;
        if (max || max === 0) {
            validators.push(Validators.maxLength(max as number));
        }
        return validators;
    }
}
