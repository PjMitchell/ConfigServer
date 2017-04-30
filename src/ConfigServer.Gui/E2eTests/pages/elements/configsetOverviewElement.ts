import { browser, by, element, ElementFinder } from 'protractor';

export class ConfigSetOverviewElement {
    constructor(private row: ElementFinder) {

    }

    public getConfigSetName() {
        return this.row.element(by.className('configset-name')).getText();
    }

    public getConfigSetDescription() {
        return this.row.element(by.className('configset-description')).getText();
    }
}
