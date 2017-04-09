import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
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
`

})
export class GroupImageFileUploaderComponent implements OnInit {
    @ViewChild('input')
    input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    form: IChildElement<HTMLFormElement>;
    @Output()
    onUpload: EventEmitter<string> = new EventEmitter<string>();
    fileName : string;
    
    static imagePath = 'ClientGroupImages'; 
    constructor(private resourceDataService: ResourceDataService) {
    }    

    ngOnInit() {
        this.setFileNameToDefault();
    }

    upload() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            let data = new FormData()
            data.append('resource', files.item(0));
            this.resourceDataService.uploadResource(GroupImageFileUploaderComponent.imagePath, fileToUpload.name, data)
                .then(() => {
                    this.form.nativeElement.reset();
                    this.onUpload.emit(fileToUpload.name);
                    this.setFileNameToDefault();
                });
        }
    }

    fileChanged() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            this.fileName = fileToUpload.name;
        }        
    }
    onFileNameClicked() {
        this.input.nativeElement.click();
    }

    setFileNameToDefault() {
        this.fileName = 'No file'
    }

}