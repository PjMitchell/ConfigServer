import { IConfigurationModelSummary } from "./configurationModelSummary";

export interface IConfigurationSetSummary {
    configurationSetId: string;
    name: string;
    description: string;    
    requiredClientTag: string;
    configs: IConfigurationModelSummary[];
}
