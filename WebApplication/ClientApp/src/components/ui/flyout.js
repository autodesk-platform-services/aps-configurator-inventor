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

import React, { useEffect, useLayoutEffect, useRef, useState } from 'react';
import cx from './cx';
import './flyout.css';

/**
 * A panel that opens below an anchor (shared by the top-nav actions and the project switcher).
 * `anchor({ open, toggle })` renders the anchor; `children` is the panel content, or a
 * `close => content` function. Closes on outside click and on Escape (which returns focus to the anchor).
 * `align` is the panel edge lined up with the anchor: "left" or "right" (CSS can move the panel; the pointer follows the anchor).
 */
export default function Flyout({ anchor, align = 'left', panelClassName, children }) {
    const [open, setOpen] = useState(false);
    const [pointerLeft, setPointerLeft] = useState(0);
    const root = useRef(null);
    const action = useRef(null);
    const panel = useRef(null);

    // centre the 24px pointer under the anchor, wherever the panel ended up
    useLayoutEffect(() => {
        if (!open) return;
        const anchorBox = action.current.getBoundingClientRect();
        setPointerLeft(anchorBox.left + anchorBox.width / 2 - panel.current.getBoundingClientRect().left - 12);
    }, [open]);

    useEffect(() => {
        if (!open) return undefined;
        const onMouseDown = e => { if (!root.current.contains(e.target)) setOpen(false); };
        const onKeyDown = e => {
            if (e.key !== 'Escape') return;
            setOpen(false);
            const focusable = action.current.querySelector('button, [tabindex]');
            if (focusable) focusable.focus();
        };
        document.addEventListener('mousedown', onMouseDown);
        document.addEventListener('keydown', onKeyDown);
        return () => {
            document.removeEventListener('mousedown', onMouseDown);
            document.removeEventListener('keydown', onKeyDown);
        };
    }, [open]);

    const close = () => setOpen(false);

    return (
        <div className="ui-flyout" ref={root}>
            <div className="ui-flyout__action" ref={action}>{anchor({ open, toggle: () => setOpen(!open) })}</div>
            {/* kept in the DOM while closed, as in HIG; user_details_test reads the closed profile panel */}
            <div ref={panel} hidden={!open} className={cx('ui-flyout__panel', `ui-flyout__panel--${align}`)}>
                <div className="ui-flyout__pointer" aria-hidden="true" style={{ left: pointerLeft }} />
                <div className={cx('ui-flyout__content', panelClassName)}>
                    {typeof children === 'function' ? children(close) : children}
                </div>
            </div>
        </div>
    );
}
