import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ResourceDataService } from '../../../dataservices/resource-data.service';
import { IConfigurationClientGroup } from '../../../interfaces/configurationClientGroup';
import { IChildElement } from '../../../interfaces/htmlInterfaces';
import { IResourceInfo } from '../../../interfaces/resourceInfo';

@Component({
    selector: 'edit-clientgroup-input',
    templateUrl: './edit-client-group-input.component.html',
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
