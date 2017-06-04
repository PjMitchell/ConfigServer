import { IConfigurationClientSetting} from './configurationClientSetting';
export interface IConfigurationClient {
    clientId: string;
    name: string;
    description: string;
    group: string;
    enviroment: string;
    readClaim: string;
    writeClaim: string;
    settings: IConfigurationClientSetting[];
}
