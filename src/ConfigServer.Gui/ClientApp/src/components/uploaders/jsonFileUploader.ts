import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'json-file-uploader',
    template: `
    <div>
        <form #form>
            <mat-form-field class="full-width">
                <div (click)="onFileNameClicked()"><input matInput [(ngModel)]="fileName" name="filename" type="text" disabled="true" ></div>
                <div matPrefix class="fileUpload" style="margin-right:5px;">
                    <span class="glyphicon-btn glyphicon glyphicon-folder-open"></span>
                    <input type="file" #input name="upload" accept=".json" class="upload" (change)="fileChanged()">
                </div>
                <div type="button" matSuffix mat-raised-button color="primary" (click)="upload()" class="upload-btn"><span class="glyphicon-btn glyphicon glyphicon-cloud-upload"></span></div>
                <mat-error *ngIf="message">
                    {{message}}
                </mat-error>
            </mat-form-field>
        </form>
    </div>
`,
})
export class JsonFileUploaderComponent implements OnInit {
    @Input('csMessage')
    public message: string;
    @Output('csMessage')
    public messageChange: EventEmitter<string> = new EventEmitter<string>();
    @Output()
    public onUpload: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    public form: IChildElement<HTMLFormElement>;

    public fileName: string;

    public ngOnInit() {
        this.setFileNameToDefault();
    }

    public upload(): void {
        const files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            const fileToUpload = files.item(0);
            const reader = new FileReader();
            const delegate = (e: ProgressEvent) => {
                reader.removeEventListener('onloadend', delegate, null);
                const data = reader.result as string;
                try {
                    const result = JSON.parse(data);
                    this.onUpload.emit(result);
                } catch (e) {
                    this.message = 'File not in Json Format';
                }
                this.form.nativeElement.reset();
                this.setFileNameToDefault();
            };
            reader.onloadend = delegate;
            this.message = '';
            reader.readAsText(fileToUpload);
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

    public setFileNameToDefault() {
        this.fileName = 'No file';
    }
}
