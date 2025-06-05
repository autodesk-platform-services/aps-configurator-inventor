/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Design Automation team for Inventor
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

/* eslint-disable no-console */
/* eslint-disable no-undef */

const trustToken = process.env.TRUST_TOKEN;
const idp_opt_in_url = 'https://accounts.autodesk.com/idp-opt-in';
const captcha_bypass_url = `https://idp.auth.autodesk.com/accounts/v1/hcaptcha/bypass?trustToken=${trustToken}`;

Feature('Authentication');

Before(({ I }) => {
    I.usePlaywrightTo('goto', async ({ context }) => {
        // Opt into a new sign-in because of captcha issues in old sign-in
        await context.request.get(idp_opt_in_url);

        // Use a trust token to bypass captcha
        await context.request.get(captcha_bypass_url);
    });

    I.amOnPage('/');
});

Scenario('check Sign-in and Sign-out workflow', async ({ I }) => {
    await I.signIn();

    I.signOut();
});
