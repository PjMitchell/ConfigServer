import { IConfigurationModelSummary } from "./configurationModelSummary";

export interface ISelectableConfigurationModelSummary extends IConfigurationModelSummary {
    isSelected: boolean;
}
