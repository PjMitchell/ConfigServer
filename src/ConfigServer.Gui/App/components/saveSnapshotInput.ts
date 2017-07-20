import { Component } from '@angular/core';
@Component({
    selector: 'snapshot-input',
    template: `
        <div class="input-group">
            <input name="text" class="form-control" type="text" [(ngModel)]="snapshot">
            <span class="input-group-btn">
                <button type="button" class="btn btn-primary" (click)="save()">Save Snapshot</button>
            </span>
        </div>
`,
})
export class SaveSnapshotInputComponent {

    public snapshot: string;

    public save(): void {
        alert(this.snapshot);
    }
}
