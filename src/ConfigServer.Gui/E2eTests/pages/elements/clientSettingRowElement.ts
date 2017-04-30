import { browser, by, element, ElementFinder } from 'protractor';

export class ClientSettingRowElement {
    constructor(private row: ElementFinder) {

    }

    public getKey() {
        return this.row.element(by.className('clientsetting-row-key')).getAttribute('value');
    }

    public updateKey(input: string) {
        const el = this.row.element(by.className('clientsetting-row-key'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public getValue() {
        return this.row.element(by.className('clientsetting-row-value')).getAttribute('value');
    }

    public updateValue(input: string) {
        const el = this.row.element(by.className('clientsetting-row-value'));
        return el.clear().then(() => el.sendKeys(input));
    }

    public clickDeleteRow() {
        return this.row.element(by.className('clientsetting-row-delete')).click();
    }

}
