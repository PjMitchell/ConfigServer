import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'config-property-item',
    template: `
    <div [ngSwitch]="csDefinition.propertyType">
        <interger-input *ngSwitchCase="'Interger'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></interger-input>
        <float-input *ngSwitchCase="'Float'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></float-input>
        <bool-input *ngSwitchCase="'Bool'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></bool-input>
        <string-input *ngSwitchCase="'String'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></string-input>
        <date-input *ngSwitchCase="'DateTime'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></date-input>
        <enum-input *ngSwitchCase="'Enum'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></enum-input>
        <option-input *ngSwitchCase="'Option'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></option-input>
        <multiple-option-input *ngSwitchCase="'MultipleOption'" [csDefinition]="csDefinition" [(csConfig)]="csConfig" [csHasInfo]="csHasInfo"></multiple-option-input>
        <collection-input *ngSwitchCase="'Collection'" [csDefinition]="csDefinition" [(csConfig)]="csConfig"></collection-input>
        <div *ngSwitchDefault>Not Acceptable</div>
    </div>
`,
})
export class ConfigurationPropertyInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csHasInfo: boolean;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
