import { Component, EventEmitter, Input, Output } from '@angular/core';
@Component({
    selector: 'app-delete-before-button',
    templateUrl: './delete-before-button.component.html',
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
