import { ConfigurationClientSetting} from './configurationClientSetting'
export interface ConfigurationClient
{
    clientId: string;
    name: string;
    description: string;
    group: string;
    enviroment: string
    settings: ConfigurationClientSetting[]
}
