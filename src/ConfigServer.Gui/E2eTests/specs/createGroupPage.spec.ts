import { HomePage } from '../pages/homepage';
import { CreateGroupPage } from '../pages/createGroupPage';
import { Constants } from '../constants'
import { browser, element, by } from 'protractor'

describe('Given I am on the homepage', () => {
    var homepage = new HomePage();
    beforeEach(() => {
        homepage.getPage();
    });
    describe('I Click On create group', () => {
        var createPage = new CreateGroupPage();
        beforeEach(() => {
            homepage.clickAddGroupButton();
        });
        it('I can add group', (done) => {
            var input = 'New Name';
            createPage.clickImage(Constants.defaultImage)
            createPage.update(input)
                .then(() => {
                    createPage.getGroupId()
                        .then(s => { 
                            createPage.save();
                            var groupPanel = homepage.groupPanelsById(s);
                            expect(groupPanel.element(by.className('home-group-id')).getText()).toEqual(s);
                            expect(groupPanel.element(by.className('home-group-img')).getAttribute('src')).toEqual(Constants.path('/Resource/ClientGroupImages/testimage.JPG'));
                            expect(groupPanel.element(by.className('home-group-name')).getText()).toEqual(input);
                            done();
                        })
                });
        });
    });
});