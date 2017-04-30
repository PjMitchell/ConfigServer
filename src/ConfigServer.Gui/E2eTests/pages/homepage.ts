﻿import { browser, by, element } from 'protractor';
import { Constants } from '../constants';

export class HomePage {

    public getPage() {
        return browser.get(Constants.angularPath('/'));
    }

    public clickAddClientButton() {
        return element(by.id('createClientBtn')).click();
    }

    public clickAddGroupButton() {
        return element(by.id('createGroupBtn')).click();
    }

    public clickManageGroupButton(id: string) {
        return element(by.id('manage-group-btn-' + id)).click();
    }

    public clickManageClientsWithoutGroupButton() {
        return element(by.id('manage-group-btn')).click();
    }

    public clickEditGroupButton(id: string) {
        return element(by.id('edit-group-btn-' + id)).click();
    }

    public groupPanels() {
        return element.all(by.className('group-panel'));
    }

    public groupPanelsById(id: string) {
        return element(by.id('group-panel-' + id));
    }
}
