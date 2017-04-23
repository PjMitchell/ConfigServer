import { browser, by, element } from 'protractor';
import { Constants } from '../constants';

export class CreateGroupPage {

    public getPage(id: string): void {
        browser.get(Constants.angularPath('/createClientGroup/' + id));
    }

    public clickImage(name: string) {
        element(by.id('group-image-selection-' + name)).click();
    }

    public getGroupId() {
        return element(by.id('groupid')).getText();
    }

    public images() {
        return element(by.className('group-image-selection'));
    }

    public update(input: string) {
        const el = element(by.id('group-name-input'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public save() {
        element(by.id('save-btn')).click();
    }
}
