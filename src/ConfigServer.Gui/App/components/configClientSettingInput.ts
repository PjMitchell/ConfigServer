import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationClientSetting } from '../interfaces/configurationClientSetting';



@Component({
    selector: 'edit-clientsetting-input',
    template: `
    <div class="row">
        <table>
            <tr>
                <th>Key</th>
                <th>Value</th>
                <th>
                    <button type="button" class="btn btn-success" (click)="add()">Add</button>
                </th>
            </tr>
            <tr *ngFor="let item of csSettings">
                <td>
                    <input [(ngModel)]="item.key" type="text" (change)="onKeyChange()">
                </td>
                <td>
                    <input [(ngModel)]="item.value" type="text" >
                </td>
                <td>
                    <button type="button" class="btn btn-danger" (click)="remove(item)">remove</button>
                </td>
            </tr>
        </table>
        <h4 class="errorMessage" *ngIf="!csIsValid">Duplicate Keys</h4>
    <div>
`

})
export class EditClientSettingInputComponent {
    @Input()
    csIsValid: boolean;
    @Output()
    csIsValidChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    csSettings: ConfigurationClientSetting[];

    constructor() {
        this.csIsValid = true;
        this.csSettings = new Array<ConfigurationClientSetting>();
    }

    onKeyChange() {
        this.checkIsValid();
    }

    add() {
        var newItem: ConfigurationClientSetting = {
            key: 'key',
            value : 'value'
        }
        this.csSettings.push(newItem);
        this.checkIsValid();
    }

    remove(item: ConfigurationClientSetting) {
        var index = this.csSettings.indexOf(item);
        this.csSettings.splice(index, 1);
        this.checkIsValid();
    }

    private checkIsValid() {
        this.csIsValid = !this.hasDuplicates();
        this.csIsValidChange.emit(this.csIsValid);
    }

    private hasDuplicates() {
        var set = new Object();
        var result = false;
        this.csSettings.forEach((value) => {
            var key = value.key.toUpperCase()
            var count = set[key];
            if (count) {
                result = true;
            } else {
                set[key] = 1;
            }
        })
        return result;
    }


}