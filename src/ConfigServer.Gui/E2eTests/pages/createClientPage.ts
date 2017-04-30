import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { EditClientElement } from './elements/editClientElement';

export class CreateClientPage {

    public readonly imputElement = new EditClientElement();

    public getPage() {
        return browser.get(Constants.angularPath('/createClient'));
    }

    public getClientId() {
        return element(by.id('client-id')).getText();
    }

    public save() {
        return element(by.id('save-btn')).click();
    }
}
