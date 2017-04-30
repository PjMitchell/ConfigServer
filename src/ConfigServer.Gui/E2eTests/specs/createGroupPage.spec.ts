import { browser, by, element } from 'protractor';
import { Constants } from '../constants';
import { CreateGroupPage } from '../pages/createGroupPage';
import { HomePage } from '../pages/homepage';

describe('Given I am on the homepage', () => {
    const homepage = new HomePage();
    beforeEach(() => {
        homepage.getPage();
    });
    describe('I Click On create group', () => {
        const createPage = new CreateGroupPage();
        beforeEach(() => {
            homepage.clickAddGroupButton();
        });
        it('I can add group', (done) => {
            const input = 'New Name';
            createPage.editElement.clickImage(Constants.defaultImage);
            createPage.editElement.update(input)
                .then(() => {
                    createPage.getGroupId()
                        .then((s) => {
                            createPage.save();
                            const groupPanel = homepage.groupPanelsById(s);
                            expect(groupPanel.element(by.className('home-group-id')).getText()).toEqual(s);
                            expect(groupPanel.element(by.className('home-group-img')).getAttribute('src')).toEqual(Constants.path('/Resource/ClientGroupImages/testimage.JPG'));
                            expect(groupPanel.element(by.className('home-group-name')).getText()).toEqual(input);
                            done();
                        });
                });
        });
    });
});
