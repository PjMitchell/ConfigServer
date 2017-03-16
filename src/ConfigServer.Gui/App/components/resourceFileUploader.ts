import { Component, Input, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { IChildElement } from '../interfaces/htmlInterfaces';
import { ResourceDataService } from '../dataservices/resource-data.service';
@Component({
    selector: 'resource-file-uploader',
    template: `
    <div>
        <form #form>
            <input name="filename" type="text" [(ngModel)]="fileName">
            <p *ngIf="!isValidFilename" style="color:red;">file name is not valid</p>
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
    regrex = /^[0-9a-zA-Z\^\&\'\@\{\}\[\]\,\$\=\!\-\#\(\)\.\%\+\~\_ ]+$/;
    isValidFilename: boolean;
    constructor(private dataService: ResourceDataService) {
    }

    _fileName: string;
    get fileName(): string { return this._fileName };
    set fileName(inputValue: string) { 
        this._fileName = inputValue
        this.checkFileName();
    }


    upload(): void {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            let data = new FormData()
            data.append('resource', files.item(0));
            this.dataService.uploadResource(this.clientId, this.fileName, data)
                .then(() => {
                    this.form.nativeElement.reset();
                    this.checkFileName();
                    this.onUpload.emit();
                });
                
            }
        
    }

    ngOnInit(): void {
        this.input.nativeElement.addEventListener('change', () => this.onFileChange());
        this.checkFileName()
    }

    onFileChange() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            this.fileName = fileToUpload.name;
        } else {
            this.fileName = '';
        }

        this.checkFileName();
    }

    checkFileName() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            this.isValidFilename = this.regrex.test(this.fileName) && this.fileName && this.fileName.includes('.')
        } else {
            this.isValidFilename = true;
        }

    }
}