import { browser, element, by } from 'protractor';
import { Constants } from '../constants';

export class EditGroupPage {

    getPage(id : string) {
        browser.get(Constants.path('/editClientGroup/' + id));
    }

    clickImage(name: string) {
        element(by.id('group-image-selection-' + name)).click();
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