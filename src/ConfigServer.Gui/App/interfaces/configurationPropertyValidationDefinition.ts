export interface IConfigurationPropertyValidationDefinition {
    min: string | number;
    max: string | number;
    maxLength?: number;
    pattern: string;
    isRequired: boolean;
}
