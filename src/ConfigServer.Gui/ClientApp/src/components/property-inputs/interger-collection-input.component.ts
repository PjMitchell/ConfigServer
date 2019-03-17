import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { uniqueItem } from '../../validators/collectionValidator';

@Component({
    selector: 'interger-collection-input',
    templateUrl: './interger-collection-input.component.html',
})
export class ConfigurationPropertyIntergerCollectionInputComponent implements OnInit {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public parentForm: FormGroup;
    public collection: any[];
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
        const newItem = 0;
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
            this.collectionForms[index].hasError('min') ? 'Value must be greater than ' + this.csDefinition.validationDefinition.min :
            this.collectionForms[index].hasError('max') ? 'Value must be less than ' + this.csDefinition.validationDefinition.max :
                '';
    }

    private setupForm() {
        let collection: number[] = this.csConfig[this.csDefinition.propertyName];
        if (!collection) {
            collection = new Array<number>();
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

    private buildControl(value: number) {
        return this.formBuilder.control(value, this.getValidators());
    }

    private getValidators() {
        const validators = new Array<ValidatorFn>();
        if (this.csDefinition.validationDefinition.isRequired) {
            validators.push(Validators.required);
        }
        const min = this.csDefinition.validationDefinition.min;
        if (min || min === 0) {
            validators.push(Validators.min(min as number));
        }
        const max = this.csDefinition.validationDefinition.max;
        if (max || max === 0) {
            validators.push(Validators.max(max as number));
        }
        return validators;
    }
}
