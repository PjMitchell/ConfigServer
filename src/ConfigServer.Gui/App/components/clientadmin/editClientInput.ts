import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationClient } from '../../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../../interfaces/configurationClientGroup';
import { ITag } from '../../interfaces/tag';

@Component({
    selector: 'edit-client-input',
    styles: ['mat-form-field { height: 66px; }'],
    template: `
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
                <input matInput id="client-name-input" placeholder="Name" [(ngModel)]="csClient.name">
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
              <mat-select placeholder="Group" [(value)]="csClient.group">
                <mat-option *ngFor="let group of csExistingGroups" [value]="group.groupId">
                  {{ group.name }}
                </mat-option>
              </mat-select>
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
                <input matInput id="client-enviroment-input" placeholder="Enviroment" [(ngModel)]="csClient.enviroment" [matAutocomplete]="auto">
                <mat-autocomplete #auto="matAutocomplete">
                  <mat-option *ngFor="let existingEnviroment of existingEnviroments" [value]="existingEnviroment">
                    {{existingEnviroment}}
                  </mat-option>
                </mat-autocomplete>
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
                <input matInput id="client-description-input" placeholder="Description" [(ngModel)]="csClient.description">
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
                <input matInput id="client-readclaim-input" placeholder="Read claim" [(ngModel)]="csClient.readclaim">
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
            <mat-form-field class="full-width">
                <input matInput id="client-configuratorclaim-input" placeholder="Configurator claim" [(ngModel)]="csClient.configuratorClaim">
            </mat-form-field>
        </div>
        <div class="col-sm-6 col-md-4">
        <mat-form-field class="full-width">
          <mat-select placeholder="Tag" [(value)]="csClient.tags" multiple [compareWith]="compareTags">
            <mat-option *ngFor="let tag of csAvailableTags" [value]="tag">
              {{ tag.value }}
            </mat-option>
          </mat-select>
        </mat-form-field>
    </div>
    </div>
    <hr/>
    <edit-clientsetting-input [(csIsValid)]="csIsValid" [csSettings]="csClient.settings"></edit-clientsetting-input>
`,
})
export class EditClientInputComponent {

    @Input()
    public csClient: IConfigurationClient;
    @Output()
    public csClientChange: EventEmitter<IConfigurationClient> = new EventEmitter<IConfigurationClient>();

    @Output()
    public csIsValidChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    public csExistingGroups: IConfigurationClientGroup[];
    @Input()
    public csAvailableTags: ITag[];
    public existingEnviroments: string[];
    private _csIsValid: boolean;
    @Input()
    get csIsValid(): boolean { return this._csIsValid; }
    set csIsValid(value: boolean) {
        this._csIsValid = value;
        this.csIsValidChange.emit(value);
    }

    private _csAllClient: IConfigurationClient[];
    @Input()
    set csAllClient(value: IConfigurationClient[]) {
        this._csAllClient = value;
        if (value) {
            this.existingEnviroments = this.toDistinct(value.map((item) => item.enviroment));
        }
    }

    constructor() {
        this.csExistingGroups = new Array<IConfigurationClientGroup>();
        this.existingEnviroments = new Array<string>();
    }

    public compareTags(t1: ITag, t2: ITag): boolean {
        return t1 && t2 ? t1.value === t2.value : t1 === t2;
    }

    private toDistinct(values: string[]) {
        const set = new Object();
        values.forEach((value) => {
            set[value] = 1;
        });
        const keys = Object.keys(set);
        return keys.filter((value) => value !== "null");
    }
}
