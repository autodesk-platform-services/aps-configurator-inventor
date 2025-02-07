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

import repo from '../Repository';
import { addError, addLog } from './notificationActions';
import { showLoginFailed } from './uiFlagsActions';

const actionTypes = {
    PROFILE_LOADED: 'PROFILE_LOADED'
};

export default actionTypes;

/** Extract access code from URL */
function extractAccessCode(url) {
    const regex = /^(?=.*code=([^&]*)|)(?=.*state=([^&]*)|)/g;
    const m = regex.exec(url);
    const code = (m && m[1]?.length) ? m[1] : undefined;
    const state = (m && m[2]?.length) ? m[2] : undefined;
    return code && state ? { 'code' : m ? m[1] : undefined, 'state' : m ? m[2] : undefined } : undefined;
}

export const detectCode = () => (dispatch) => {
    try {
        const accessCode = extractAccessCode(window.location.href.substring(1));
        if (accessCode) {
            dispatch(addLog(`Detected access code`));
            repo.setAccessCode(accessCode);

            // remove code from URL
            window.history.pushState("", document.title, window.location.pathname);
        } else {
            dispatch(addLog('Access code is not found'));
            repo.forgetAccessCode();
        }
    } catch (error) {
        dispatch(addError('Failed to detect access code. (' + error + ')'));
        repo.forgetAccessCode();
    }
};

export const updateProfile = (profile, isLoggedIn) => {
    return {
        type: actionTypes.PROFILE_LOADED,
        profile,
        isLoggedIn
    };
};

export const loadProfile = () => async (dispatch) => {
    dispatch(addLog('Load profile invoked'));
    try {
        const profile = await repo.loadProfile();
        dispatch(addLog('Load profile received'));
        const isLoggedIn = repo.hasAccessCode();
        dispatch(updateProfile(profile, isLoggedIn));
    } catch (error) {
        if (error.response && error.response.status === 403) {
            dispatch(showLoginFailed(true));
        } else {
            dispatch(addError('Failed to get profile. (' + error + ')'));
        }
    }
};
