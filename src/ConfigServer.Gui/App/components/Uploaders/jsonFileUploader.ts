import { Component, Input, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
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
        <p>{{message}}</p>
    </div>
`
})
export class JsonFileUploaderComponent implements OnInit{
    @Input('csMessage')
    message: string;
    @Output('csMessage')
    messageChange: EventEmitter<string> = new EventEmitter<string>();
    @Output()
    onUpload: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('input')
    input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    form: IChildElement<HTMLFormElement>;

    fileName : string;

    ngOnInit() {
        this.setFileNameToDefault()
    }

    upload(): void {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            var fileToUpload = files.item(0),
                reader = new FileReader();
            reader.onloadend = (e) => {                
                reader.removeEventListener('onloadend');
                var data = <string>reader.result;
                try {
                    var result = JSON.parse(data);
                    this.onUpload.emit(result);
                    
                } catch (e) {
                    this.message = 'File not in Json Format';
                }
                this.form.nativeElement.reset();
                this.setFileNameToDefault()
            }
            this.message = '';
            reader.readAsText(fileToUpload);
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