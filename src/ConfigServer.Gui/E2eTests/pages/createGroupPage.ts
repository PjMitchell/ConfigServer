import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { EditGroupElement } from './elements/editGroupElement';

export class CreateGroupPage {

    public readonly editElement = new EditGroupElement();

    public getPage(id: string) {
        return browser.get(Constants.angularPath('/createClientGroup/' + id));
    }

    public getGroupId() {
        return element(by.id('groupid')).getText();
    }

    public save() {
        return element(by.id('save-btn')).click();
    }
}
