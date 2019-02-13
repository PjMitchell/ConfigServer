import { Component, EventEmitter, Input, Output } from '@angular/core';
@Component({
    selector: 'app-icon-button',
    templateUrl: './icon-button.component.html',
    styles: ['./icon-button.component.scss'],
})
export class IconButtonComponent {

    @Input()
    public color: string;
}
