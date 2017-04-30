import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { asyncHelper } from '../helper';
import { CreateClientPage } from '../pages/createClientPage';
import { EditClientPage } from '../pages/editClientPage';
import { ClientSettingRowElement } from '../pages/elements/clientSettingRowElement';
import { HomePage } from '../pages/homepage';

describe('Given I am on the create client page', () => {
    const createClientPage = new CreateClientPage();
    const editClientPage = new EditClientPage();
    beforeEach(async (done) => {
        await createClientPage.getPage();
        done();
    });

    it('I can add client', (done) => asyncHelper(done, async (d) => {
        const name = 'New App';
        const enviroment = 'Test';
        const description = 'A new App for test';
        const groupId = Constants.defaultGroupId;
        await createClientPage.imputElement.updateClientName(name);
        await createClientPage.imputElement.updateEnviroment(enviroment);
        await createClientPage.imputElement.updateDescription(description);
        await createClientPage.imputElement.updateGroup(groupId);
        const clientId = await createClientPage.getClientId();

        await createClientPage.save();
        await editClientPage.getPage(clientId);
        expect(editClientPage.imputElement.getClientName()).toEqual(name);
        expect(editClientPage.imputElement.getEnviroment()).toEqual(enviroment);
        expect(editClientPage.imputElement.getDescription()).toEqual(description);
        expect(editClientPage.imputElement.getGroup()).toEqual(groupId);
    }));

    it('I can add client with setting', (done) => asyncHelper(done, async (d) =>  {
        const name = 'New App';
        const settingName = "setting";
        const settingValue = "Value";
        await createClientPage.imputElement.updateClientName(name);
        await createClientPage.imputElement.clickAddClientSettingRow();

        const settingRows = await createClientPage.imputElement.getClientSettingRows();
        expect(settingRows.length).toEqual(1);
        const rowElement = new ClientSettingRowElement(settingRows[0]);
        await rowElement.updateKey(settingName);
        await rowElement.updateValue(settingValue);
        const clientId = await createClientPage.getClientId();

        await createClientPage.save();
        await editClientPage.getPage(clientId);
        expect(editClientPage.imputElement.getClientName()).toEqual(name);
        const inspectedRows = await createClientPage.imputElement.getClientSettingRows();
        expect(inspectedRows.length).toEqual(1);
        const inspectedElement = new ClientSettingRowElement(inspectedRows[0]);
        expect(inspectedElement.getKey()).toEqual(settingName);
        expect(inspectedElement.getValue()).toEqual(settingValue);
    }));
});
