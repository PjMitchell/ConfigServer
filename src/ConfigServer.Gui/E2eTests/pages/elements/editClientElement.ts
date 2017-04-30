import { browser, by, element } from 'protractor';
import { ClientSettingRowElement } from './clientSettingRowElement';

export class EditClientElement {

    public getClientName() {
        return element(by.id('client-name-input')).getAttribute('value');
    }

    public updateClientName(input: string) {
        const el = element(by.id('client-name-input'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public getEnviroment() {
        return element(by.id('client-enviroment-input')).getAttribute('value');
    }

    public updateEnviroment(input: string) {
        const el = element(by.id('client-enviroment-input'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public getDescription() {
        return element(by.id('client-description-input')).getAttribute('value');
    }

    public updateDescription(input: string) {
        const el = element(by.id('client-description-input'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public getGroup() {
        return element(by.id('client-group-input')).getAttribute('value');
    }

    public updateGroup(id: string) {
        const el = element(by.id('client-group-input'));
        return el.all(by.tagName('option')).filter((e) => e.getAttribute('value').then((val) => val === id)).first().click();
    }

    public clickAddClientSettingRow() {
        return element(by.id('clientsetting-input-add-btn')).click();
    }

    public getClientSettingRows() {
        return element(by.id('clientsetting-input')).all(by.className('clientsetting-row'));
    }

    public getClientSettingRowsByKey(key: string) {
        return element(by.id('clientsetting-input')).all(by.className('clientsetting-row'))
            .filter((row) => row.element(by.className('clientsetting-row-key')).getAttribute('value').then((value) => value === key));
    }
}
