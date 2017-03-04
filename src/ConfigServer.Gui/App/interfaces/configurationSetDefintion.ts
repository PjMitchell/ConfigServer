export interface ConfigurationSetModelPayload{
    configurationSetId: string;
    name: string;
    description: string;
    config: Dictionary<ConfigurationModelPayload>;
}

export interface ConfigurationModelPayload {
    name: string;
    description: string;
    isOption: boolean;
    property: Dictionary<ConfigurationPropertyPayload>;
}

export interface ConfigurationPropertyPayload {
    propertyName: string;
    propertyType: string;
    propertyDisplayName: string;
    propertyDescription: string;

    validationDefinition?: ConfigurationPropertyValidationDefinition;
    options?: Dictionary<string>;

    childProperty?: Dictionary<ConfigurationPropertyPayload>;
}

export interface ConfigurationPropertyValidationDefinition {
    min: string;
    max: string;
    maxLength?: number;
    pattern: string;
    isRequired: boolean;
}

export interface Dictionary<T> {
    [index: string]: T;
}

export interface Group<TKey, TItem> {
    key: TKey;
    items: TItem[];
}