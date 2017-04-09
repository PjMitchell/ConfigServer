﻿import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationClientSetting } from '../interfaces/configurationClientSetting';



@Component({
    selector: 'edit-clientsetting-input',
    template: `
    <div class="row">
        <div class="col-sm-12 col-md-6">
            <table>
                <tr>
                    <th>Key</th>
                    <th>Value</th>
                    <th class="column-btn">
                        <button type="button" class="btn btn-success" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></button>
                    </th>
                </tr>
                <tr *ngFor="let item of csSettings">
                    <td>
                        <input [(ngModel)]="item.key" type="text" (change)="onKeyChange()" class="form-control">
                    </td>
                    <td>
                        <input [(ngModel)]="item.value" type="text"  class="form-control">
                    </td>
                    <td class="column-btn">
                        <button type="button" class="btn btn-danger" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></button>
                    </td>
                </tr>
            </table>
        </div>
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