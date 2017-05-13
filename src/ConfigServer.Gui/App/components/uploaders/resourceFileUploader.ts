import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../../dataservices/resource-data.service';
import { IChildElement } from '../../interfaces/htmlInterfaces';
@Component({
    selector: 'resource-file-uploader',
    template: `
    <div>
        <form #form>
            <div class="input-group">
                <span class="input-group-btn">
                    <div class="fileUpload btn btn-primary">
                        <span class="glyphicon-btn glyphicon glyphicon-folder-open"></span>
                        <input type="file" #input name="upload" class="upload" (change)="fileChanged()">
                    </div>
                </span>
                <input name="filename" class="form-control" type="text" [(ngModel)]="fileName">
                <span class="input-group-btn">
                    <button type="button" class="btn btn-primary" (click)="upload()"><span class="glyphicon-btn glyphicon glyphicon-cloud-upload"></span></button>
                </span>
            </div>
            <p *ngIf="!isValidFilename" style="color:red;">file name is not valid</p>
        </form>
    </div>
`,
})
export class ResourceFileUploaderComponent implements OnInit {

    @Input('csClientId')
    public clientId: string;
    @Output()
    public onUpload: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    public form: IChildElement<HTMLFormElement>;
    public isValidFilename: boolean;

    private regrex = /^[0-9a-zA-Z\^\&\'\@\{\}\[\]\,\$\=\!\-\#\(\)\.\%\+\~\_ ]+$/;
    private  _fileName: string;
    get fileName(): string { return this._fileName; }
    set fileName(inputValue: string) {
        this._fileName = inputValue;
        this.checkFileName();
    }
    constructor(private dataService: ResourceDataService) { }

    public upload(): void {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            const fileToUpload = files.item(0);
            const data = new FormData();
            data.append('resource', files.item(0));
            this.dataService.uploadResource(this.clientId, this.fileName, data)
                .then(() => {
                    this.form.nativeElement.reset();
                    this.checkFileName();
                    this.onUpload.emit();
                });

            }

    }

    public ngOnInit(): void {
        this.checkFileName();
    }

    public fileChanged() {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            const fileToUpload = files.item(0);
            this.fileName = fileToUpload.name;
        } else {
            this.fileName = '';
        }

        this.checkFileName();
    }

    private checkFileName() {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            this.isValidFilename = this.regrex.test(this.fileName) && this.fileName && this.fileName.indexOf('.') >= 0 ;
        } else {
            this.isValidFilename = true;
        }
    }
}
