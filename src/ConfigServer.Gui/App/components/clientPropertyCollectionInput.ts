import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { ConfigurationPropertyPayload } from '../interfaces/configurationSetDefintion';


@Component({
    selector: 'collection-input',
    template: `
<table>
    <tr>
        <th *ngFor="let p of csDefinition.childProperty | toIterator">{{p.propertyDisplayName}}</th>
        <th>
            <button type="button" (click)="add()">Add</button>
        </th>
    </tr>
    <tr *ngFor="let item of collection;let i= index">
        <td *ngFor="let itemProperty of csDefinition.childProperty | toIterator">
            <config-property-item [csDefinition]="itemProperty" [(csConfig)]="collection[i]">
                </config-property-item>
        </td>
        <th>
            <button type="button" (click)="remove(item)">remove</button>
        </th>
    </tr>
</table>
`

})
export class ConfigurationPropertyCollectionInputComponent implements OnInit {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    collection: any[];

    ngOnInit() {
        this.collection = this.csConfig[this.csDefinition.propertyName];
    }

    add() {
        var newItem = new Object();
        var keys = Object.keys(this.csDefinition.childProperty);
        keys.forEach((value) => {
            newItem[value] = '';
        })
        this.collection.push(newItem);
    }

    remove(item: any) {
        var index = this.collection.indexOf(item);
        this.collection.splice(index, 1);
    }

    customTrackBy(index: number, obj: any): any {
        return index;
    }
}