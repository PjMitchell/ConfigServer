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
            <input [(ngModel)]="csClientGroup.name" type="text" class="form-control">
            <div *ngIf="csClientGroup.imagePath" class="thumbnail"><img class="img-responsive" src="Resource/ClientGroupImages/{{csClientGroup.imagePath}}" /></div>
            <h4>Upload image:</h4>
            <group-image-file-uploader (onUpload)="onImageUploaded($event)"></group-image-file-uploader>
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
    fileName : string;
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

    updateImages() {
        this.resourceDataService.getClientResourceInfo(EditClientGroupInputComponent.imagePath)
            .then(info => this.images = info)
    }
    onImageUploaded(fileName: string) {
        this.csClientGroup.imagePath = fileName;
        this.updateImages();
    }
}