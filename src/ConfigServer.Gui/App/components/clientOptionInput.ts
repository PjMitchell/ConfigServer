import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationModelPayload } from '../interfaces/configurationSetDefintion';

@Component({
    selector: 'config-option-input',
    template: `
        <table>
            <tr>
                <th *ngFor="let p of csModel.property | toIterator">{{p.propertyDisplayName}}</th>
                <th>
                    <button type="button" class="btn btn-success" (click)="add()">Add</button>
                </th>
            </tr>
            <tr *ngFor="let item of csCollection;let i= index">
                <td *ngFor="let itemProperty of csModel.property | toIterator">
                    <config-property-item [csDefinition]="itemProperty" [(csConfig)]="csCollection[i]">
                        </config-property-item>
                </td>
                <th>
                    <button type="button" class="btn btn-danger" (click)="remove(item)">remove</button>
                </th>
            </tr>
        </table>
`
})
export class OptionInputComponent {
    @Input()
    csModel: ConfigurationModelPayload;
    @Input()
    csCollection: any[];
    @Output()
    csCollectionChange: EventEmitter<any[]> = new EventEmitter<any[]>();

    add() {
        var newItem = new Object();
        var keys = Object.keys(this.csModel.property);
        keys.forEach((value) => {
            newItem[value] = '';
        })
        this.csCollection.push(newItem);
    }

    remove(item: any) {
        var index = this.csCollection.indexOf(item);
        this.csCollection.splice(index, 1);
    }

    customTrackBy(index: number, obj: any): any {
        return index;
    }
}