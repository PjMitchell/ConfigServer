import { Component, EventEmitter, Input, Output } from '@angular/core';
@Component({
    selector: 'app-icon-button',
    template: '<button type="button" mat-raised-button color="{{color}}" (click)="onClick($event)"><ng-content></ng-content></button>',
    styles: ['button { width: 40px; min-width: 40px; padding:0px}'],
})
export class IconButtonComponent {

    @Input()
    public color: string;
    @Output()
    public click: EventEmitter<any>;

    constructor() {
        this.click = new EventEmitter<any>();
    }
    public onClick(event: any) {
        this.click.emit(event);
    }
}
