import { Component, Input } from '@angular/core';
import { SnapshotDataService } from "../../dataservices/snapshot-data.service";
@Component({
    selector: 'snapshot-input',
    template: `
        <mat-form-field class="full-width snapshot-input-group">
            <input matInput [(ngModel)]="snapshot" type="text" placeholder="Snapshot name">
            <button type="button" matSuffix mat-raised-button color="primary" (click)="save()" [disabled]="!snapshot">Save Snapshot</button>
        </mat-form-field>
`,
})
export class SaveSnapshotInputComponent {
    @Input('csClientId')
    public clientId: string;

    public snapshot: string;

    constructor(private dataService: SnapshotDataService) {}

    public save(): void {
        this.dataService.saveSnapShot({ clientId: this.clientId, name: this.snapshot });
        this.snapshot = '';
    }
}
