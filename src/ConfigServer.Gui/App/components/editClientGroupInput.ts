import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { IChildElement } from '../interfaces/htmlInterfaces';
import { IResourceInfo } from '../interfaces/resourceInfo';

@Component({
    selector: 'edit-clientgroup-input',
    template: `
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <h4>Name:</h4>
            <input id="group-name-input" [(ngModel)]="csClientGroup.name" type="text" class="form-control">
            <div *ngIf="csClientGroup.imagePath" class="thumbnail"><img class="img-responsive" src="Resource/ClientGroupImages/{{csClientGroup.imagePath}}" /></div>
            <h4>Upload image:</h4>
            <group-image-file-uploader (onUpload)="onImageUploaded($event)"></group-image-file-uploader>
        </div>
        <div id="group-image-selection-{{image.name}}" class="col-sm-3 col-md-2 group-image-selection" *ngFor="let image of images" (click)="onImageClick(image)">
            <img class="img-responsive" src="Resource/ClientGroupImages/{{image.name}}" />
        </div>
    </div>
`,

})
export class EditClientGroupInputComponent implements OnInit {
    private static imagePath = 'ClientGroupImages';
    @Input()
    public csClientGroup: IConfigurationClientGroup;
    @Output()
    public csClientGroupChange: EventEmitter<IConfigurationClientGroup> = new EventEmitter<IConfigurationClientGroup>();
    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;
    @ViewChild('form')
    public form: IChildElement<HTMLFormElement>;
    public images: IResourceInfo[];

    constructor(private resourceDataService: ResourceDataService) {
        this.images = new Array<IResourceInfo>();
    }

    public ngOnInit() {
        this.updateImages();
    }

    public onImageClick(image: IResourceInfo) {
        this.csClientGroup.imagePath = image.name;
    }

    public updateImages() {
        this.resourceDataService.getClientResourceInfo(EditClientGroupInputComponent.imagePath)
            .then((info) => this.images = info);
    }
    public onImageUploaded(fileName: string) {
        this.csClientGroup.imagePath = fileName;
        this.updateImages();
    }
}
