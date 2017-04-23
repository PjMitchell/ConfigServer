import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'collection-input',
    template: `
<table>
    <tr>
        <th *ngFor="let p of csDefinition.childProperty | toIterator">{{p.propertyDisplayName}}</th>
        <th class="column-btn">
            <button type="button" class="btn btn-success" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></button>
        </th>
    </tr>
    <tr *ngFor="let item of collection;let i= index">
        <td *ngFor="let itemProperty of csDefinition.childProperty | toIterator">
            <config-property-item [csDefinition]="itemProperty" [(csConfig)]="collection[i]">
            </config-property-item>
        </td>
        <td class="column-btn">
            <button type="button" class="btn btn-danger" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></button>
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
    public collection: any[];

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
}
