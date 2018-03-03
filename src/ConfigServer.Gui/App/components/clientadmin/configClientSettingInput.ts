import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Subject } from 'rxjs';
import { IConfigurationClientSetting } from '../../interfaces/configurationClientSetting';

@Component({
    selector: 'edit-clientsetting-input',
    template: `
    <div class="row">
        <div class="col-sm-12 col-md-6">
            <table id="clientsetting-input">
                <tr>
                    <th>Key</th>
                    <th>Value</th>
                    <th class="column-btn">
                        <app-icon-button id="clientsetting-input-add-btn" color="accent" (click)="add()"><span class="glyphicon-btn glyphicon glyphicon-plus"></span></app-icon-button>
                    </th>
                </tr>
                <tr *ngFor="let item of csSettings" class="clientsetting-row">
                    <td>
                        <mat-form-field class="full-width">
                            <input matInput class="clientsetting-row-key" [(ngModel)]="item.key" (change)="onKeyChange()">
                        </mat-form-field>
                    </td>
                    <td>
                        <mat-form-field class="full-width">
                            <input matInput class="clientsetting-row-value" [(ngModel)]="item.value">
                        </mat-form-field>
                    </td>
                    <td class="column-btn">
                        <app-icon-button class="clientsetting-row-delete" color="warn" (click)="remove(item)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
                    </td>
                </tr>
            </table>
        </div>
        <h4 class="errorMessage" *ngIf="!csIsValid">Duplicate Keys</h4>
    <div>
`,
})
export class EditClientSettingInputComponent {
    @Input()
    public csIsValid: boolean;
    @Output()
    public csIsValidChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    public csSettings: IConfigurationClientSetting[];

    constructor() {
        this.csIsValid = true;
        this.csSettings = new Array<IConfigurationClientSetting>();
    }

    public onKeyChange() {
        this.checkIsValid();
    }

    public add() {
        const newItem: IConfigurationClientSetting = {
            key: 'key',
            value : 'value',
        };
        this.csSettings.push(newItem);
        this.checkIsValid();
    }

    public remove(item: IConfigurationClientSetting) {
        const index = this.csSettings.indexOf(item);
        this.csSettings.splice(index, 1);
        this.checkIsValid();
    }

    private checkIsValid() {
        this.csIsValid = !this.hasDuplicates();
        this.csIsValidChange.emit(this.csIsValid);
    }

    private hasDuplicates() {
        const set = new Object();
        let result = false;
        this.csSettings.forEach((value) => {
            const key = value.key.toUpperCase();
            const count = set[key];
            if (count) {
                result = true;
            } else {
                set[key] = 1;
            }
        });
        return result;
    }
}
