import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../../dataservices/resource-data.service';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'group-image-file-uploader',
    templateUrl: './group-image-uploader.component.html',
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
