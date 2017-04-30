import { browser, by, element } from 'protractor';
import { Constants } from '../constants';

export class ManageGroupPage {

    public getPage(groupId?: string) {
        if (groupId) {
            return browser.get(Constants.angularPath('/group/' + groupId));
        } else {
            return browser.get(Constants.angularPath('/group'));
        }
    }

    public clickManageClientButton(id: string) {
        return element(by.id('manage-client-btn-' + id)).click();
    }

    public clickEditClientButton(id: string) {
        return element(by.id('edit-client-btn-' + id)).click();
    }

    public clientPanels() {
        return element.all(by.className('client-panel'));
    }

    public clientPanelsById(id: string) {
        return element(by.id('client-panel-' + id));
    }

    public groupImage() {
        return element(by.id('group-image'));
    }

    public groupTitle() {
        return element(by.id('group-title'));
    }

    public groupId() {
        return element(by.id('group-id'));
    }
}
