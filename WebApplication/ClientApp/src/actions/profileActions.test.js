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

import actionTypes, { detectCode, loadProfile } from "./profileActions";
import notificationTypes from '../actions/notificationActions';

// prepare mock for Repository module
jest.mock('../Repository');
import repoInstance from '../Repository';

import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('detectCode', () => {

    let store;
    beforeEach(() => {
        store = mockStore({});
        repoInstance.setAccessCode.mockClear();
        repoInstance.forgetAccessCode.mockClear();
    });

    describe('success', () => {

        it.each([
            "#code=foo&state=foo2",
            "#first=second&code=foo&state=foo2",
        ])("should remember access code if it's in the url (%s)",
        (hashString) => {

            window.location.href = hashString;
            const pushStateSpy = jest.spyOn(window.history, 'pushState');

            detectCode()(store.dispatch);

            expect(repoInstance.setAccessCode).toHaveBeenCalledWith({code : 'foo', state: 'foo2'});
            expect(pushStateSpy).toHaveBeenCalled();

            pushStateSpy.mockRestore();
        });

        it.each([
            "",                     // no hash
            "#",                    // hash, but nothing in it
            "#foo=1",               // different parameter
            "#codennn=1",   // slightly different name
            "#code=",       // expected parameter, but without value
            "#state=",       // expected 2nd parameter, but without value
            "#code=foo&state=", // expected 2nd parameter, but without value
        ])('should forget code if not found in url (%s)',
        (hashString) => {

            window.location.href = hashString;

            detectCode()(store.dispatch);

            expect(repoInstance.forgetAccessCode).toHaveBeenCalled();
        });
    });

    describe('failure', () => {
        it('should log error on failure and forget access code', () => {

            // prepare to raise error during code extraction
            window.location.href = '#code=foo&state=foo2';
            repoInstance.setAccessCode.mockImplementation(() => { throw new Error('123456'); });

            // execute
            detectCode()(store.dispatch);

            // check if error is logged and code is forgotten
            expect(repoInstance.setAccessCode).toHaveBeenCalled();

            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);
            expect(logAction).toBeDefined();

            expect(repoInstance.forgetAccessCode).toHaveBeenCalled();
        });
    });
});

describe('loadProfile', () => {

    let store;
    beforeEach(() => {
        store = mockStore({});
        repoInstance.loadProfile.mockClear();
    });

    describe('success', () => {

        it('should fetch profile from repository', async () => {

            const profile = { name: "John Smith", avatarUrl: "http://johnsmith.com/avatar.jpg"};

            repoInstance.loadProfile.mockImplementation(() => profile);

            await store.dispatch(loadProfile());
            expect(repoInstance.loadProfile).toHaveBeenCalledTimes(1);

            // check the loaded profile is in store now
            const profileLoadedAction = store.getActions().find(a => a.type === actionTypes.PROFILE_LOADED);
            expect(profileLoadedAction.profile).toEqual(profile);
        });
    });

    describe('failure', () => {
        it('should log error on failure and forget access code', async () => {

            repoInstance.loadProfile.mockImplementation(() => { throw new Error(); });

            await store.dispatch(loadProfile());
            expect(repoInstance.loadProfile).toHaveBeenCalledTimes(1);

            // check the error is logged
            const logAction = store.getActions().find(a => a.type === notificationTypes.ADD_ERROR);
            expect(logAction).toBeDefined();
        });
    });
});
