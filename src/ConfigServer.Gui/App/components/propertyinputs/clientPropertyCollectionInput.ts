import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
            <config-property-item [csDefinition]="itemProperty" [(csConfig)]="collection[i]" (onIsValidChanged)="onValidChanged(i,c, $event)">
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
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @Output()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    public collection: any[];

    private validation: boolean[][] = new Array<boolean[]>();
    public ngOnInit() {
        this.collection = this.csConfig[this.csDefinition.propertyName];
    }

    public add() {
        const newItem = new Object();
        const keys = Object.keys(this.csDefinition.childProperty);
        keys.forEach((value) => {
            newItem[value] = '';
        });
        this.collection.push(newItem);
    }

    public remove(item: any) {
        const index = this.collection.indexOf(item);
        this.collection.splice(index, 1);
    }

    public customTrackBy(index: number, obj: any): any {
        return index;
    }

    private onValidChanged(row: number, column: number, isValid: boolean) {
        if (!this.validation[row]) {
            this.validation[row] = new Array<boolean>();
        }
        this.validation[row][column] = isValid;
        this.onIsValidChanged.emit(this.validation.every((value) => value.every((innerValue) => innerValue)));
    }
}
