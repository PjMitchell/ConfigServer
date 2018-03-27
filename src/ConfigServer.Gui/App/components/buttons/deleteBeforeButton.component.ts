import { Component, EventEmitter, Input, Output } from '@angular/core';
@Component({
    selector: 'app-delete-before-button',
    template: `
    <mat-form-field class="full-width">
        <input matInput [(ngModel)]="inputDate" [matDatepicker]="picker">
        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
        <button type="button" matPrefix mat-raised-button color="primary" (click)="deleteBefore()">Delete before</button>
        <mat-datepicker #picker></mat-datepicker>
    </mat-form-field>
    `,
})
export class DeleteBeforeComponent {

    public inputDate: Date;
    @Output()
    public onDeleteBefore: EventEmitter<Date>;

    constructor() {
        this.inputDate = new Date();
        this.onDeleteBefore = new EventEmitter<Date>();
    }
    public deleteBefore() {
        this.onDeleteBefore.emit(this.inputDate);
    }
}
