/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Inventor Automation team
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

const projectButton =  '//div[contains(@role, "button") and .//*[local-name()="img"]]';
const currentProjectName =  locate('p').inside(projectButton);

Feature('Project Switcher');

Before(({ I }) => {
    I.amOnPage('/');
});

Scenario('should check Project switcher is loaded', async ({ I }) => {
    // wait until project combo is displayed
    I.waitForElement( locators.xpComboProjects, 10);
    I.click( locators.xpComboProjects);

    // wait until project list is displayed
    I.waitForElement( locators.xpProjectList, 10);

    // check content of PROJECTS menu
    I.see("PROJECTS", locators.xpProjectList);

    // check name of the second project
    I.see("Wheel", locators.xpProjectWheel);

    // check name of the third project
    I.see("Wrench", locators.xpProjectWrench);

});

Scenario('should check Project switcher is correctly changed', async ({ I }) => {

    // click to show popup menu with list of projects
    I.selectProject('Wrench');

    // check the current project name
    I.see("Wrench", currentProjectName);

    // after selection of the project it take some minor time for popup menu to collapse
    // thus we need to put a timeout for the next selectProject to trigger
    // if we don't put this wait timeout, next selectProject action would cause a collapse of a popup instead of select
    I.wait(2);

    // click to show popup menu with list of projects
    I.selectProject('Wheel');

    // check the current project name
    I.see("Wheel", currentProjectName);
});

Scenario('should check presence of Log button', async ({ I }) => {

    // check if exists the button
    I.waitForElement( locators.xpButtonLog, 5);
    I.click( locators.xpButtonLog);

    // wait until log popup is displayed
    I.waitForElement( locators.xpPopUpLog, 5);

    // check content of the log popup
    I.see("Navigation Action", '//h3');
});

Scenario('should check presence of User button', async ({ I }) => {

    //I.amOnPage(testPage);

    // check if exists the button
    I.waitForElement('span[aria-label="Avatar for Anonymous"]', 2);

    // validate user name
    I.see("A", '//button/span/span');
});