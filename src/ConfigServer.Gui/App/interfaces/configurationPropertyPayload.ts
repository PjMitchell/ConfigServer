import { IConfigurationPropertyValidationDefinition } from "./configurationPropertyValidationDefinition";
import { IDictionary } from "./dictionary";

export interface IConfigurationPropertyPayload {
    propertyName: string;
    propertyType: string;
    propertyDisplayName: string;
    propertyDescription: string;
    keyPropertyName: string;
    validationDefinition?: IConfigurationPropertyValidationDefinition;
    options?: IDictionary<string>;

    childProperty?: IDictionary<IConfigurationPropertyPayload>;
}
