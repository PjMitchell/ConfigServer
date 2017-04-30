import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { asyncHelper } from '../helper';
import { ConfigOverviewElement } from '../pages/elements/configOverviewElement';
import { ConfigSetOverviewElement } from '../pages/elements/configsetOverviewElement';
import { ManageClientPage } from '../pages/manageClientPage';

describe('Given I am on the manage client page', () => {
    const manageClientPage = new ManageClientPage();
    beforeEach((done) => asyncHelper(done, async (d) => {
        manageClientPage.getPage(Constants.defaultClientId);
    }));
    it('Client info displayed correctly', () => {
        expect(manageClientPage.getClientName()).toEqual('Mvc App Live');
        expect(manageClientPage.getClientId()).toEqual('Id: ' + Constants.defaultClientId);
        expect(manageClientPage.getClientEnv()).toEqual('Live');
        expect(manageClientPage.getClientDesc()).toEqual('Application');
    });

    it('ConfigSet info displayed correctly', () => {
        const configSetPanel = new ConfigSetOverviewElement(manageClientPage.getConfigSetPanelByConfigurationSetId(Constants.defaultConfigSet));
        expect(configSetPanel.getConfigSetName()).toEqual('Core Configuration Set');
        expect(configSetPanel.getConfigSetDescription()).toEqual('Description: Only Configuration Set in the app');
    });

    it('Config info displayed correctly', () => {
        const configSetPanel = new ConfigOverviewElement(manageClientPage.getConfigPanelByConfiguration(Constants.defaultConfigSet, Constants.defaultConfig));
        expect(configSetPanel.getConfigName()).toEqual('Sample Config');
        expect(configSetPanel.getConfigDescription()).toEqual('Description: Basic Configuration');
    });

    it('Config Edit button navigates to config editor', (done) => asyncHelper(done, async (d) => {
        const configSetPanel = new ConfigOverviewElement(manageClientPage.getConfigPanelByConfiguration(Constants.defaultConfigSet, Constants.defaultConfig));
        await configSetPanel.clickEditButton();
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/client/' + Constants.defaultClientId + '/' + Constants.defaultConfigSet + '/' + Constants.defaultConfig));
    }));

});
