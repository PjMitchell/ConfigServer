import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'json-file-uploader',
    template: `
    <div>
        <form #form>
            <div class="input-group">
                <span class="input-group-btn">
                    <div class="fileUpload btn btn-primary">
                        <span class="glyphicon-btn glyphicon glyphicon-folder-open"></span>
                        <input type="file" #input name="upload" accept=".json" class="upload" (change)="fileChanged()">
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
        <p>{{message}}</p>
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
