import { IConfigurationModelPayload } from "./configurationModelPayload";
import { IDictionary } from "./dictionary";

export interface IConfigurationSetModelPayload {
    configurationSetId: string;
    name: string;
    description: string;
    config: IDictionary<IConfigurationModelPayload>;
}
