import { IConfigurationModelSummary } from "./configurationModelSummary";

export interface IConfigurationSetSummary {
    configurationSetId: string;
    name: string;
    description: string;
    configs: IConfigurationModelSummary[];
}
