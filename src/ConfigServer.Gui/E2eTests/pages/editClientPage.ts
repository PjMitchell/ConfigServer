import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { EditClientElement } from './elements/editClientElement';

export class EditClientPage {

    public readonly imputElement = new EditClientElement();

    public getPage(id: string) {
        return browser.get(Constants.angularPath('/editClient/' + id));
    }

     public getClientId() {
        return element(by.id('client-id')).getText();
    }

    public save() {
        return element(by.id('save-btn')).click();
    }
}
