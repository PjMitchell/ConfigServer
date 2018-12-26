import { IConfigurationClientSetting} from './configurationClientSetting';
import { Tag } from './tag';
export interface IConfigurationClient {
    clientId: string;
    name: string;
    description: string;
    group: string;
    enviroment: string;
    readClaim: string;
    configuratorClaim: string;
    settings: IConfigurationClientSetting[];
    tags: Tag[]
}
