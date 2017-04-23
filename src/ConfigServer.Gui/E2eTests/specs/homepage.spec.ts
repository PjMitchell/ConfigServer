import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { HomePage } from '../pages/homepage';

describe('Given I am on the homepage', () => {
    const homepage = new HomePage();
    beforeEach(() => {
        homepage.getPage();
    });
    it('Group panel displays correctly', () => {
        const groupPanel = homepage.groupPanelsById(Constants.defaultGroupId);
        expect(groupPanel.element(by.className('home-group-img')).getAttribute('src')).toEqual(Constants.path('/Resource/ClientGroupImages/testimage.JPG'));
        expect(groupPanel.element(by.className('home-group-name')).getText()).toEqual('My Apps');
        expect(groupPanel.element(by.className('home-group-id')).getText()).toEqual(Constants.defaultGroupId);
    });
    it('when I click Add Client I navigate to the create client', () => {
        homepage.clickAddClientButton();
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/createClient'));
    });
    it('when I click Add Group I navigate to the create client', () => {
        homepage.clickAddGroupButton();
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/createClientGroup'));
    });

    it('when I click Edit Group I navigate to the edit client group page for that group', () => {
        homepage.clickEditGroupButton(Constants.defaultGroupId);
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/editClientGroup/' + Constants.defaultGroupId));
    });

    it('when I click manage Group I navigate to the manage client group page for that group', () => {
        homepage.clickManageGroupButton(Constants.defaultGroupId);
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/group/' + Constants.defaultGroupId));
    });

    it('when I click manage Group I navigate to the mange client group page for no client', () => {
        homepage.clickManageClientsWithoutGroupButton();
        expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/group'));
    });
});
