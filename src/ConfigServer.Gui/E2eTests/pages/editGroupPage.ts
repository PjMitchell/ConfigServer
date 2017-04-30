import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { EditGroupElement } from './elements/editGroupElement';

export class EditGroupPage {

    public readonly editElement = new EditGroupElement();

    public getPage(id: string) {
        return browser.get(Constants.angularPath('/editClientGroup/' + id));
    }

    public save() {
        return element(by.id('save-btn')).click();
    }
}
