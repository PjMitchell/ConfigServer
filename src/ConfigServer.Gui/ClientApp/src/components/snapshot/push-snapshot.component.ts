import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from "../../dataservices/client-data.service";
import { ConfigurationSetDataService } from "../../dataservices/configset-data.service";
import { SnapshotDataService } from '../../dataservices/snapshot-data.service';
import { IConfigurationClient } from "../../interfaces/configurationClient";
import { IConfigurationClientSetting } from "../../interfaces/configurationClientSetting";
import { IConfigurationSetSummary } from "../../interfaces/configurationSetSummary";
import { ISelectableConfigurationModelSummary } from "../../interfaces/selectableConfigurationModelSummary";
import { ISnapshotInfo } from "../../interfaces/snapshotInfo";
import { ITag } from '../../interfaces/tag';

@Component({
    templateUrl: './push-snapshot.component.html',
})
export class PushSnapshotComponent implements OnInit {

    public snapshots: ISnapshotInfo[];
    public selectedSnapshot: ISnapshotInfo;
    public configurationSets: IConfigurationSetSummary[];
    public client: IConfigurationClient;
    public canPushSnapShot: boolean;
    private clientId: string;
    constructor(private dataService: SnapshotDataService, private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private route: ActivatedRoute, private router: Router) {
        this.snapshots = new Array<ISnapshotInfo>();
        this.configurationSets = new Array<IConfigurationSetSummary>();
        this.client = { clientId: '', group: '', name: '', description: '', enviroment: '', readClaim: '', configuratorClaim: '', settings: new Array<IConfigurationClientSetting>(), tags: new Array<ITag>() };
        this.canPushSnapShot = false;
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.canPushSnapShot = false;
            this.loadInitialData();
        });
    }

    public async pushSnapshot() {
        this.canPushSnapShot = false;
        const selectedConfigs = this.flaternConfigs(this.configurationSets).filter((value) => value.isSelected).map((value) => value.id);
        await this.dataService.pushSnapshotToClient(this.selectedSnapshot.id, this.clientId, selectedConfigs);
        this.back();
    }

    public back() {
        this.router.navigate(['/client', this.clientId]);
    }

    public toggleConfigSet(configSet: IConfigurationSetSummary) {
        const result = configSet.configs.some((config) => !(config as ISelectableConfigurationModelSummary).isSelected);
        configSet.configs.forEach((config) => {
            (config as ISelectableConfigurationModelSummary).isSelected = result;
        });
    }

    private async loadInitialData() {
        this.client = await this.clientDataService.getClient(this.clientId);
        this.snapshots = await this.dataService.getSnapshots(this.client.group);
        const configurationSetModels = await this.configSetDataService.getConfigurationSets();
        configurationSetModels.forEach((configSet) => configSet.configs.forEach((config) => (config as ISelectableConfigurationModelSummary).isSelected = true));
        this.configurationSets = configurationSetModels;
        this.canPushSnapShot = true;
    }

    private flaternConfigs(values: IConfigurationSetSummary[]) {
        const result = new Array<ISelectableConfigurationModelSummary>();
        values.forEach((value) => value.configs.forEach((config) => result.push(config as ISelectableConfigurationModelSummary)));
        return result;
    }

}
