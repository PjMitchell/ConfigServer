import { browser, by, element } from 'protractor';

export class EditGroupElement {
    public clickImage(name: string) {
        return element(by.id('group-image-selection-' + name)).click();
    }

    public images() {
        return element(by.className('group-image-selection'));
    }

    public update(input: string) {
        const el = element(by.id('group-name-input'));
        return el.clear().then(() => el.sendKeys(input));
    }
}
