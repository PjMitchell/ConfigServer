import { Component, Input } from '@angular/core';
import { SnapshotDataService } from "../../dataservices/snapshot-data.service";
@Component({
    selector: 'snapshot-input',
    templateUrl: './save-snapshot-input.component.html',
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
