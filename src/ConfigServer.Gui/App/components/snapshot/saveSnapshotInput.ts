import { Component, Input } from '@angular/core';
import { SnapshotDataService } from "../../dataservices/snapshot-data.service";
@Component({
    selector: 'snapshot-input',
    template: `
        <div class="input-group snapshot-input-group">
            <input name="text" class="form-control" type="text" [(ngModel)]="snapshot" placeholder="Enter name">
            <span class="input-group-btn">
                <button type="button" class="btn btn-primary" (click)="save()" [disabled]="!snapshot">Save Snapshot</button>
            </span>
        </div>
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
