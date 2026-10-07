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

import React from 'react';
import cx from './cx';
import './button.css';

/**
 * Text button (replaces @hig/button). `title` is the button text, `type` is primary | secondary,
 * `width="grow"` fills the container. HIG's `size` only changed padding in other themes, so it's ignored.
 */
export default function Button({ title, type = 'primary', size, width, icon, className, ...rest }) {
    return (
        // no type="button", as in HIG: the UI-test sign-in helper finds the profile button as the first `button[@type="button"]`
        <button className={cx('ui-button', `ui-button--${type}`, width === 'grow' && 'ui-button--grow', className)} {...rest}>
            {icon && <span className="ui-button__icon">{icon}</span>}
            <span className={icon ? 'ui-button__label--with-icon' : undefined}>{title}</span>
        </button>
    );
}
