import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArchiveConfigService } from '../dataservices/archiveconfig-data.service';
import { IArchivedConfigInfo } from '../interfaces/archivedConfigInfo';
import { IGroup } from "../interfaces/group";
import { IChildElement } from '../interfaces/htmlInterfaces';

@Component({
    template: `
        <div class="row">
            <h3>Config Archive</h3>
        </div>
        <div class="row" *ngFor="let group of groupedConfigs">
            <div class="row">
                <h4>{{group.key}}</h4>
            </div>
            <div class="row" groupedConfigs>
                <div *ngFor="let config of group.items" class="col-sm-6 col-md-4" >
                    <h5>{{config.name}}</h5>
                    <p>
                        ServerVersion:{{config.serverVersion}} <br/>
                        Created:{{config.timeStamp | date:"MM/dd/yy" }} <br/>
                        Archived:{{config.archiveTimeStamp | date:"MM/dd/yy" }}
                    </p>
                    <button type="button" class="btn btn-primary" (click)="downloadConfig(config.name)"><span class="glyphicon-btn glyphicon glyphicon-download-alt"></span></button>
                    <button type="button" class="btn btn-primary" (click)="delete(config.name)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></button>
                </div>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="input-group col-sm-4 col-md-3">
                <span class="input-group-btn">
                    <button type="button" class="btn btn-primary" (click)="deleteBefore()">Delete before</button>
                </span>
                <input class="form-control"  type="date" #input value="{{inputDate | date:'yyyy-MM-dd'}}"  (blur)="onBlur()">
            </div>
        </div>
        <hr />
        <div class="row">
            <button type="button" class="btn btn-primary" (click)="back()">Back</button>
        </div>
`,
})
export class ConfigArchiveComponent implements OnInit {
    public groupedConfigs: Array<IGroup<string, IArchivedConfigInfo>>;
    public clientId: string;
    public inputDate: Date;
    @ViewChild('input')
    public input: IChildElement<HTMLInputElement>;

    constructor(private dataService: ArchiveConfigService, private route: ActivatedRoute, private router: Router) {
        this.inputDate = new Date();
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.updateArchiveList();
        });
    }

    public downloadConfig(file: string) {
        window.open('Archive/' + this.clientId + '/' + file);
    }

    public async delete(file: string) {
        const result = confirm('Are you sure you want to delete ' + file + '?');
        if (!result) {
            return;
        }
        await this.dataService.deleteArchivedConfig(this.clientId, file);
        await this.updateArchiveList();
    }

    public async deleteBefore() {
        await this.dataService.deleteArchivedConfigBefore(this.clientId, this.inputDate);
        await this.updateArchiveList();
    }

    public back() {
        this.router.navigate(['/client/' + this.clientId]);
    }

    public onBlur() {
        this.inputDate = new Date(this.input.nativeElement.value);
    }

    private async updateArchiveList() {
        const configs = await this.dataService.getArchivedConfig(this.clientId);
        this.groupedConfigs = this.groupArchivedConfigsByConfigName(configs);
    }

    private groupArchivedConfigsByConfigName(configs: IArchivedConfigInfo[]) {
        const group = {};
        configs.forEach((value) => {
            if (!group[value.configuration]) {
                group[value.configuration] = new Array<IArchivedConfigInfo>();
            }
            group[value.configuration].push(value);
        });
        const result = new Array<IGroup<string, IArchivedConfigInfo>>();
        for (const propertyName in group) {
            if (group.hasOwnProperty(propertyName)) {
                result.push({key: propertyName, items: group[propertyName]});
            }
        }
        return result;
    }
}
