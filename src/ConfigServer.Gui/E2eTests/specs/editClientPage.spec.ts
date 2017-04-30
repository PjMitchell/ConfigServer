import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { asyncHelper } from '../helper';
import { CreateClientPage } from '../pages/createClientPage';
import { EditClientPage } from '../pages/editClientPage';
import { ClientSettingRowElement } from '../pages/elements/clientSettingRowElement';
import { HomePage } from '../pages/homepage';

describe('Given I am on the edit client page', () => {
    const editClientPage = new EditClientPage();
    let originalName: string;
    let originalEnv: string;
    let originalDescription: string;

    beforeEach((done) => asyncHelper(done, async (d) => {
        await editClientPage.getPage(Constants.defaultClientId);
        originalName = await editClientPage.imputElement.getClientName();
        originalEnv = await editClientPage.imputElement.getEnviroment();
        originalDescription = await editClientPage.imputElement.getDescription();
    }));

    afterEach((done) => asyncHelper(done, async (d) => {
        await editClientPage.getPage(Constants.defaultClientId);
        await editClientPage.imputElement.updateClientName(originalName);
        await editClientPage.imputElement.updateEnviroment(originalEnv);
        await editClientPage.imputElement.updateDescription(originalDescription);
        await editClientPage.save();
    }));

    it('I can add client', (done) => asyncHelper(done, async (d) => {
        const name = 'Updated Name';
        const enviroment = 'New Enviroment';
        const description = 'A new App for test';
        const groupId = Constants.defaultGroupId;
        await editClientPage.imputElement.updateClientName(name);
        await editClientPage.imputElement.updateEnviroment(enviroment);
        await editClientPage.imputElement.updateDescription(description);

        await editClientPage.save();
        await editClientPage.getPage(Constants.defaultClientId);
        expect(editClientPage.imputElement.getClientName()).toEqual(name);
        expect(editClientPage.imputElement.getEnviroment()).toEqual(enviroment);
        expect(editClientPage.imputElement.getDescription()).toEqual(description);
        expect(editClientPage.imputElement.getGroup()).toEqual(groupId);
    }));

    it('I can add new setting then remove them', (done) => asyncHelper(done, async (d) => {
        const name = 'New App';
        const settingName = "setting";
        const settingValue = "Value";
        await editClientPage.imputElement.updateClientName(name);
        await editClientPage.imputElement.clickAddClientSettingRow();

        const settingRows = await editClientPage.imputElement.getClientSettingRows();
        expect(settingRows.length).toEqual(1);
        const rowElement = new ClientSettingRowElement(settingRows[0]);
        await rowElement.updateKey(settingName);
        await rowElement.updateValue(settingValue);
        const clientId = await editClientPage.getClientId();

        await editClientPage.save();
        await editClientPage.getPage(clientId);
        expect(editClientPage.imputElement.getClientName()).toEqual(name);
        const inspectedRows = await editClientPage.imputElement.getClientSettingRows();
        expect(inspectedRows.length).toEqual(1);
        const inspectedElement = new ClientSettingRowElement(inspectedRows[0]);
        expect(inspectedElement.getKey()).toEqual(settingName);
        expect(inspectedElement.getValue()).toEqual(settingValue);

        await inspectedElement.clickDeleteRow();
        await editClientPage.save();
        await editClientPage.getPage(clientId);
        const emptiedRows = await editClientPage.imputElement.getClientSettingRows();
        expect(emptiedRows.length).toEqual(0);

    }));
});
