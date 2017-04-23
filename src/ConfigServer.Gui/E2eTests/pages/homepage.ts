import { browser, element, by } from 'protractor';
import { Constants } from '../constants';

export class HomePage {

    getPage(): void {
        browser.get(Constants.angularPath('/'));
    }
    
    clickAddClientButton() {
        element(by.id('createClientBtn')).click();
    }

    clickAddGroupButton() {
        element(by.id('createGroupBtn')).click();
    }

    clickManageGroupButton(id: string) {
        element(by.id('manage-group-btn-' + id)).click();
    }

    clickManageClientsWithoutGroupButton() {
        element(by.id('manage-group-btn')).click();
    }

    clickEditGroupButton(id: string) {
        element(by.id('edit-group-btn-' + id)).click();
    }

    groupPanels() {
        return element.all(by.className('group-panel'));
    }

    groupPanelsById(id : string) {
        return element(by.id('group-panel-'+id));
    }
}