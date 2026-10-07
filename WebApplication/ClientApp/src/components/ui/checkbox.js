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

import React, { useEffect, useRef, useState } from 'react';
import cx from './cx';
import { CheckboxDash16, CheckboxTick16 } from './icons';
import './checkbox.css';

/**
 * Checkbox (replaces @hig/checkbox). Like HIG, `onChange` receives the new checked state (a boolean),
 * not the event, and the box keeps its own state when `checked` isn't passed.
 */
export default function Checkbox({ checked, indeterminate = false, onChange, className, ...rest }) {
    const [ownChecked, setOwnChecked] = useState(false);
    const isChecked = checked === undefined ? ownChecked : !!checked;
    const input = useRef(null);

    // `indeterminate` is only settable from JS; re-apply after every render because a click clears it
    useEffect(() => { input.current.indeterminate = indeterminate; });

    return (
        <div className={cx('ui-checkbox', className)}>
            <input ref={input} type="checkbox" className="ui-checkbox__input" checked={isChecked} {...rest}
                onChange={e => {
                    setOwnChecked(e.target.checked);
                    if (onChange) onChange(e.target.checked);
                }} />
            <span className={cx('ui-checkbox__box', (isChecked || indeterminate) && 'ui-checkbox__box--checked')} aria-hidden="true">
                {indeterminate ? <CheckboxDash16 /> : isChecked && <CheckboxTick16 />}
            </span>
        </div>
    );
}
