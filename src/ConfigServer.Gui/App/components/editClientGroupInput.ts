import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { ResourceInfo } from '../interfaces/resourceInfo';
import { IChildElement } from '../interfaces/htmlInterfaces';


@Component({
    selector: 'edit-clientgroup-input',
    template: `
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <h4>Name:</h4>
            <input [(ngModel)]="csClientGroup.name" type="text">
            <div *ngIf="csClientGroup.imagePath" class="thumbnail"><img class="img-responsive" src="Resource/ClientGroupImages/{{csClientGroup.imagePath}}" /></div>
            <form #form>
                <button type="button" class="btn btn-primary" (click)="upload()"><span class="glyphicon glyphicon-cloud-upload"></span></button>             
                <input type="file" #input name="upload" accept="image/*">
            </form>
        </div>
        <div class="col-sm-3 col-md-2" *ngFor="let image of images" (click)="onImageClick(image)">
            <img class="img-responsive" src="Resource/ClientGroupImages/{{image.name}}" />
        </div>
    </div>
`

})
export class EditClientGroupInputComponent implements OnInit {
    @Input()
    csClientGroup: ConfigurationClientGroup;
    @Output()
    csClientGroupChange: EventEmitter<ConfigurationClientGroup> = new EventEmitter<ConfigurationClientGroup>();
    @ViewChild('input')
    input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    form: IChildElement<HTMLFormElement>;
    static imagePath = 'ClientGroupImages'; 
    constructor(private resourceDataService: ResourceDataService) {
        this.images = new Array<ResourceInfo>();
    }    

    images: ResourceInfo[];

    ngOnInit() {
        this.updateImages();
    }

    onImageClick(image: ResourceInfo) {
        this.csClientGroup.imagePath = image.name;
    }

    upload() {
        var files = this.input.nativeElement.files;
        if (files && files.length === 1) {
            let fileToUpload = files.item(0);
            let data = new FormData()
            data.append('resource', files.item(0));
            this.resourceDataService.uploadResource(EditClientGroupInputComponent.imagePath, fileToUpload.name, data)
                .then(() => {
                    this.form.nativeElement.reset();
                    this.csClientGroup.imagePath = fileToUpload.name;
                    this.updateImages();
                });
        }
    }

    updateImages() {
        this.resourceDataService.getClientResourceInfo(EditClientGroupInputComponent.imagePath)
            .then(info => this.images = info)
    }
}