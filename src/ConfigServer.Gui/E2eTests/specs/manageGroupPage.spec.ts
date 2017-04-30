import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { ManageGroupPage } from '../pages/manageGroupPage';

describe('Given I am on the group client page', () => {
    const manageGroupPage = new ManageGroupPage();
    beforeEach(() => {
        manageGroupPage.getPage(Constants.defaultGroupId);
    });

    it('Displays Group detail', () => {
        expect(manageGroupPage.groupImage().getAttribute('src')).toEqual(Constants.path('/Resource/ClientGroupImages/testimage.JPG'));
        expect(manageGroupPage.groupTitle().getText()).toEqual('Group: My Apps');
        expect(manageGroupPage.groupId().getText()).toEqual('Id: ' + Constants.defaultGroupId);
    });
    it('Displays Client detail', () => {
        const clientPage = manageGroupPage.clientPanelsById(Constants.defaultClientId);
        expect(clientPage.element(by.className('client-name')).getText()).toEqual('Mvc App Live');
        expect(clientPage.element(by.className('client-id')).getText()).toEqual('Id: ' + Constants.defaultClientId);
        expect(clientPage.element(by.className('client-enviroment')).getText()).toEqual('Live');
        expect(clientPage.element(by.className('client-description')).getText()).toEqual('Application');
    });
    it('When I click manage, I am directed to the manage client page', () => {
        manageGroupPage.clickManageClientButton(Constants.defaultClientId);
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/client/' + Constants.defaultClientId));
    });
    it('When I click edit client, I am directed to the edit client page', () => {
        manageGroupPage.clickEditClientButton(Constants.defaultClientId);
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/editClient/' + Constants.defaultClientId));
    });
});
