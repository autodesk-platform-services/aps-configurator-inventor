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

import React, { useState } from 'react';
import cx, { withSuffix } from './cx';
import Input from './input';
import { Caret10, Checkmark16 } from './icons';
import './dropdown.css';

/**
 * Single-select list of strings (replaces @hig/dropdown). `onChange` receives the chosen option.
 * Keeps HIG's markup, which the UI tests rely on: a read-only <input> that opens a
 * `div[role=listbox]` of `div[role=option] > span`. Opens on click (not on focus).
 */
export default function Dropdown({ options = [], value, onChange, disabled, className, onBlur, onFocus, onMouseOver, onMouseOut }) {
    const [open, setOpen] = useState(false);
    const [highlighted, setHighlighted] = useState(-1);

    const show = () => {
        setOpen(true);
        setHighlighted(options.indexOf(value));
    };

    const select = option => {
        setOpen(false);
        if (onChange) onChange(option);
    };

    const onKeyDown = e => {
        if (disabled) return;
        if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
            e.preventDefault();
            if (!open) {
                show();
                return;
            }
            const step = e.key === 'ArrowDown' ? 1 : -1;
            setHighlighted(Math.min(options.length - 1, Math.max(0, highlighted + step)));
        } else if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            if (open && highlighted >= 0) select(options[highlighted]);
            else show();
        } else if (e.key === 'Escape') {
            setOpen(false);
        }
    };

    return (
        <div className={cx('ui-dropdown', className)} role="combobox" aria-expanded={open} aria-haspopup="listbox">
            <div className="ui-dropdown__field">
                <Input className={withSuffix(className, '-input-wrapper')} value={value ?? ''} readOnly disabled={disabled}
                    onClick={() => {
                        if (disabled) return;
                        if (open) setOpen(false);
                        else show();
                    }}
                    onKeyDown={onKeyDown}
                    onBlur={e => {
                        setOpen(false);
                        if (onBlur) onBlur(e);
                    }}
                    onFocus={onFocus} onMouseOver={onMouseOver} onMouseOut={onMouseOut} />
                <Caret10 className={cx('ui-dropdown__caret', open && 'ui-dropdown__caret--open')} />
            </div>
            {open &&
                <div className="ui-dropdown__menu" role="listbox">
                    {options.map((option, index) => (
                        <div key={option} role="option" aria-selected={option === value}
                            className={cx('ui-dropdown__option', index === highlighted && 'ui-dropdown__option--highlighted')}
                            // keep focus in the input, so its blur doesn't close the menu before the click lands
                            onMouseDown={e => e.preventDefault()}
                            onMouseEnter={() => setHighlighted(index)}
                            onClick={() => select(option)}>
                            <span>{option}</span>
                            <div className="ui-dropdown__check"><Checkmark16 /></div>
                        </div>
                    ))}
                </div>
            }
        </div>
    );
}
