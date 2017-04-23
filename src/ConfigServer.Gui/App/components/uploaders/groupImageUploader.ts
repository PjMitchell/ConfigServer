import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../../dataservices/resource-data.service';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'group-image-file-uploader',
    template: `
        <form #form>
            <div class="input-group">
            <span class="input-group-btn">
                <div class="fileUpload btn btn-primary">
                    <span class="glyphicon-btn glyphicon glyphicon-folder-open"></span>
                    <input type="file" #input name="upload" accept="image/*" class="upload" (change)="fileChanged()">
                </div>
            </span>
            <span class="input-group-addon upload-text" (click)="onFileNameClicked()">
                {{fileName}}
            </span>
            <span class="input-group-btn">
                <button type="button" class="btn btn-primary" (click)="upload()"><span class="glyphicon-btn glyphicon glyphicon-cloud-upload"></span></button>
            </span>
            </div>
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
