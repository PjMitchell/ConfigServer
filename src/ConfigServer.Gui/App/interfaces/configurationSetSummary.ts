export interface ConfigurationSetSummary {
    configurationSetId: string;
    name: string;
    description: string;
    configs: ConfigurationModelSummary[];
}

export interface ConfigurationModelSummary
{
    id: string;
    displayName: string;
    description: string;
}