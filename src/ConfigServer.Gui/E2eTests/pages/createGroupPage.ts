import { browser, element, by } from 'protractor';
import { Constants } from '../constants';

export class CreateGroupPage {

    getPage(id: string): void {
        browser.get(Constants.angularPath('/createClientGroup/' + id));
    }

    clickImage(name: string) {
        element(by.id('group-image-selection-'+name)).click();
    }

    getGroupId() {
        return element(by.id('groupid')).getText();
    }

    images() {
        return element(by.className('group-image-selection'));
    }

    update(input: string) {
        var el = element(by.id('group-name-input'));
        return el.clear().then(() => el.sendKeys(input));
    }

    save() {
        element(by.id('save-btn')).click();
    }

}