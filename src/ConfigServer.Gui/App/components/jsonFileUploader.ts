import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { IChildElement } from '../interfaces/htmlInterfaces';

@Component({
    selector: 'json-file-uploader',
    template: `
    <div>
        <form #form>
            <button type="button" class="btn btn-primary" (click)="upload()"> <span class="glyphicon glyphicon-cloud-upload"></span> </button> 
            <input type="file" #input name="upload" accept="application/json">
        </form>
        <p>{{message}}</p>
    </div>
`
})
export class JsonFileUploaderComponent {
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
            }
            this.message = '';
            reader.readAsText(fileToUpload);
        }
    }
}