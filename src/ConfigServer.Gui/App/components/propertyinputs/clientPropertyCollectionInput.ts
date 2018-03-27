import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'collection-input',
    template: `
<table>
    <tr>
        <th *ngFor="let p of csDefinition.childProperty | toIterator">{{p.propertyDisplayName}}</th>
        <th class="column-btn">
            <app-icon-button color="accent" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></app-icon-button>
        </th>
    </tr>
    <tr *ngFor="let item of collection;let i= index">
        <td *ngFor="let itemProperty of csDefinition.childProperty | toIterator;let c= index">
            <config-property-item [csDefinition]="itemProperty" [csConfig]="collection[i]" [parentForm]="collectionForms.controls[i]">
            </config-property-item>
        </td>
        <td class="column-btn">
            <app-icon-button color="warn" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
        </td>
    </tr>
</table>
`,
})
export class ConfigurationPropertyCollectionInputComponent implements OnInit {
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
        const collection: any[] = this.csConfig[this.csDefinition.propertyName];
        collection.forEach((value, i) => {
            this.collectionForms.setControl(i, this.formBuilder.group({}));
        });
        this.parentForm.setControl(this.csDefinition.propertyName, this.collectionForms);
        this.collection = collection;
    }

    public add() {
        const newItem = new Object();
        const keys = Object.keys(this.csDefinition.childProperty);
        keys.forEach((value) => {
            newItem[value] = '';
        });
        this.collection.push(newItem);
        const index = this.collection.indexOf(newItem);
        this.collectionForms.setControl(index, this.formBuilder.group({}));
    }

    public remove(item: any) {
        const index = this.collection.indexOf(item);
        this.collection.splice(index, 1);
        this.collectionForms.controls.splice(index, 1);
    }

    public customTrackBy(index: number, obj: any): any {
        return index;
    }
}
