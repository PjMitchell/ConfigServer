import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { EditGroupPage } from '../pages/editGroupPage';
import { HomePage } from '../pages/homepage';

describe('Given I am on the homepage', () => {
    const homepage = new HomePage();
    beforeEach(() => {
        homepage.getPage();
    });
    describe('I Click On edit group', () => {
        const editpage = new EditGroupPage();
        beforeEach(() => {
            homepage.clickEditGroupButton(Constants.defaultGroupId);
        });
        afterEach((done) => {
            homepage.clickEditGroupButton(Constants.defaultGroupId);
            editpage.editElement.update('My Apps')
                .then(() => {
                    editpage.save();
                    done();
                });
        });
        it('Clicking save returns to home', () => {
            editpage.save();
            expect(browser.getCurrentUrl()).toEqual(Constants.angularPath('/'));
        });
        it('I can update name', (done) => {
            const input = 'New Name';
            editpage.editElement.update(input)
                .then(() => {
                    editpage.save();
                    const groupPanel = homepage.groupPanelsById(Constants.defaultGroupId);
                    expect(groupPanel.element(by.className('home-group-name')).getText()).toEqual(input);
                    done();
                });
        });
    });
});
