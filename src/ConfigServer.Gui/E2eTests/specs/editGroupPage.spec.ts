import { HomePage } from '../pages/homepage';
import { EditGroupPage } from '../pages/editGroupPage';
import { Constants } from '../constants'
import { browser, element, by } from 'protractor'

describe('Given I am on the homepage', () => {
    var homepage = new HomePage();
    beforeEach(() => {
        homepage.getPage();
    });
    describe('I Click On edit group', () => {
        var editpage = new EditGroupPage();
        beforeEach(() => {
            homepage.clickEditGroupButton(Constants.defaultGroupId);
        });
        afterEach((done) => {
            homepage.clickEditGroupButton(Constants.defaultGroupId);
            editpage.update('My Apps')
                .then(() => {
                    editpage.save();
                    done();
                });     
        });
        it('Clicking save returns to home', () => {
            editpage.save();
            expect(browser.getCurrentUrl()).toEqual(Constants.path('/'));
        });
        it('I can update name', (done) => {
            var input = 'New Name';
            editpage.update(input)
                .then(() => {
                    editpage.save();
                    var groupPanel = homepage.groupPanelsById(Constants.defaultGroupId);
                    expect(groupPanel.element(by.className('home-group-name')).getText()).toEqual(input);
                    done();
                });            
        });
    });
});