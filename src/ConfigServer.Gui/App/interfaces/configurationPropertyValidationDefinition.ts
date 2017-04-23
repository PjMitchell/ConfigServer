export interface IConfigurationPropertyValidationDefinition {
    min: string;
    max: string;
    maxLength?: number;
    pattern: string;
    isRequired: boolean;
}
