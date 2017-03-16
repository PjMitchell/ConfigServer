import { Component, Input, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { IChildElement } from '../interfaces/htmlInterfaces';
import { ResourceDataService } from '../dataservices/resource-data.service';
@Component({
    selector: 'resource-file-uploader',
    template: `
    <div>
        <form #form action="/Resource/{{clientId}}/Text.txt" enctype="multipart/form-data" method="post">
            <input name="filename" type="text" [(ngModel)]="fileName">
            <input type="file" #input name="upload">
            <button type="button" (click)="upload()">Upload</button> 
        </form>
    </div>
`
})
export class ResourceFileUploaderComponent implements OnInit {
    @Input('csClientId')
    clientId : string;
    @Output()
    onUpload: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    form: IChildElement<HTMLFormElement>;
    fileName: string;
    constructor(private dataService: ResourceDataService) {
        
    }

    upload(): void {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            let data = new FormData()
            data.append('resource', files.item(0));
            this.dataService.uploadResource(this.clientId, this.fileName, data)
                .then(() => this.form.nativeElement.reset());
                
            }
        
    }

    ngOnInit(): void {
        this.input.nativeElement.addEventListener('change', () => this.onFileChange());
    }

    onFileChange() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            this.fileName = fileToUpload.name;
        } else {
            this.fileName = '';
        }
    }
}