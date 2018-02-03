import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationModelPayload } from '../interfaces/configurationModelPayload';

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
                <td *ngFor="let itemProperty of csModel.property | toIterator">
                    <config-property-item [csDefinition]="itemProperty" [(csConfig)]="csCollection[i]">
                        </config-property-item>
                </td>
                <td class="column-btn">
                    <app-icon-button color="warn" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
                </td>
            </tr>
        </table>
`,
})
export class OptionInputComponent {
    @Input()
    public csModel: IConfigurationModelPayload;
    @Input()
    public csCollection: any[];
    @Output()
    public csCollectionChange: EventEmitter<any[]> = new EventEmitter<any[]>();

    public add() {
        const newItem = new Object();
        const keys = Object.keys(this.csModel.property);
        keys.forEach((value) => {
            newItem[value] = '';
        });
        this.csCollection.push(newItem);
    }

    public remove(item: any) {
        const index = this.csCollection.indexOf(item);
        this.csCollection.splice(index, 1);
    }

    public customTrackBy(index: number, obj: any): any {
        return index;
    }
}
