import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../../dataservices/resource-data.service';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'group-image-file-uploader',
    template: `
        <form #form>
            <mat-form-field class="full-width">
                <div (click)="onFileNameClicked()"><input matInput [(ngModel)]="fileName" name="filename" type="text" disabled="true"></div>
                <div matPrefix class="fileUpload" style="margin-right:5px;">
                    <span class="glyphicon-btn glyphicon glyphicon-folder-open"></span>
                    <input type="file" #input name="upload" accept="image/*" class="upload" (change)="fileChanged()">
                </div>
                <div type="button" matSuffix mat-raised-button color="primary" (click)="upload()" class="upload-btn"><span class="glyphicon-btn glyphicon glyphicon-cloud-upload"></span></div>
            </mat-form-field>
        </form>
`,
})
export class GroupImageFileUploaderComponent implements OnInit {

    private static imagePath = 'ClientGroupImages';

    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    public form: IChildElement<HTMLFormElement>;
    @Output()
    public onUpload: EventEmitter<string> = new EventEmitter<string>();
    public fileName: string;

    constructor(private resourceDataService: ResourceDataService) { }

    public ngOnInit() {
        this.setFileNameToDefault();
    }

    public upload() {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            const fileToUpload = files.item(0);
            const data = new FormData();
            data.append('resource', files.item(0));
            this.resourceDataService.uploadResource(GroupImageFileUploaderComponent.imagePath, fileToUpload.name, data)
                .then(() => {
                    this.form.nativeElement.reset();
                    this.onUpload.emit(fileToUpload.name);
                    this.setFileNameToDefault();
                });
        }
    }

    public fileChanged() {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            const fileToUpload = files.item(0);
            this.fileName = fileToUpload.name;
        }
    }
    public onFileNameClicked() {
        this.input.nativeElement.click();
    }

    private setFileNameToDefault() {
        this.fileName = 'No file';
    }
}
