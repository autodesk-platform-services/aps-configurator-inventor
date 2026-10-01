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
import cx, { withSuffix } from './cx';
import './input.css';

/**
 * Text field (replaces @hig/input, "box" variant). `onChange` receives the event, like HIG.
 * As in HIG, `className` also goes on the inner elements with `__input` / `__halo` appended;
 * parametersContainer.css and the UI tests select `input.changedOnUpdate__input` etc.
 */
export default function Input({ className, variant, disabled, ...rest }) {
    return (
        <div className={cx('ui-input', disabled && 'ui-input--disabled', className)}>
            <input className={cx('ui-input__input', withSuffix(className, '__input'))} disabled={disabled} {...rest} />
            <div className={cx('ui-input__halo', withSuffix(className, '__halo'))} />
        </div>
    );
}
