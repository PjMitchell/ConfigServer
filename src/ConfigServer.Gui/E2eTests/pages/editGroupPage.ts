import { browser, by, element } from 'protractor';
import { Constants } from '../constants';

export class EditGroupPage {

    public getPage(id: string) {
        browser.get(Constants.angularPath('/editClientGroup/' + id));
    }

    public clickImage(name: string) {
        element(by.id('group-image-selection-' + name)).click();
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
