import { browser, by, element, ElementFinder } from 'protractor';

export class ConfigOverviewElement {
    constructor(private row: ElementFinder) {

    }

    public getConfigName() {
        return this.row.element(by.className('config-name')).getText();
    }

    public getConfigDescription() {
        return this.row.element(by.className('config-description')).getText();
    }

    public clickEditButton() {
        return this.row.element(by.className('config-edit-btn')).click();
    }
}
