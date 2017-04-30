import { browser, by, element } from 'protractor';
import { Constants } from '../constants';

export class ManageClientPage {

    public getPage(clientId: string) {
        return browser.get(Constants.angularPath('/client/' + clientId));
    }

    public getClientId() {
        return element(by.id('client-id')).getText();
    }

    public getClientName() {
        return element(by.id('client-name')).getText();
    }

    public getClientEnv() {
        return element(by.id('client-env')).getText();
    }

    public getClientDesc() {
        return element(by.id('client-desc')).getText();
    }

    public getConfigSetPanel() {
        return element(by.className('configset-overview'));
    }

    public getConfigSetPanelByConfigurationSetId(id: string) {
        return element(by.id('configset-overview-' + id));
    }

    public getConfigPanelByConfiguration(configurationSetId: string, configurationId: string) {
        return element(by.id('config-' + configurationSetId + '-' + configurationId));
    }
}
