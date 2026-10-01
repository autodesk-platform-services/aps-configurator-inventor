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
import IconButton from './iconButton';
import Typography from './typography';
import { CloseMUI } from './icons';
import './modal.css';

let lastId = 0;

/**
 * Dialog window (replaces @hig/modal). Keeps HIG's `div[role=dialog] > article[role=document]`
 * structure, which the UI tests select. `headerChildren` replaces the default title bar.
 * Size/border overrides go through `style` (HIG's `stylesheet` callback is gone).
 * Escape calls `onCloseClick`; focus moves into the dialog and goes back when it closes.
 * Clicking the overlay does nothing, as in HIG (progress dialogs must not be dismissed by accident).
 */
export default function Modal({ open, title, onCloseClick, headerChildren, className, style, children }) {
    const [titleId] = useState(() => `ui-modal-title-${++lastId}`);
    const dialog = useRef(null);

    useEffect(() => {
        if (!open) return undefined;
        const previouslyFocused = document.activeElement;
        dialog.current.focus();
        return () => { if (previouslyFocused && previouslyFocused.focus) previouslyFocused.focus(); };
    }, [open]);

    if (!open) return null;

    const onKeyDown = e => {
        // e.g. react-select handles Escape itself to close its menu
        if (e.key === 'Escape' && !e.defaultPrevented && onCloseClick) onCloseClick(e);
    };

    return (
        <div ref={dialog} className="ui-modal" role="dialog" aria-modal="true" aria-labelledby={titleId} tabIndex={-1} onKeyDown={onKeyDown}>
            <article className={cx('ui-modal__window', className)} role="document" style={style}>
                <header id={titleId} className={headerChildren ? undefined : 'ui-modal__header'}>
                    {headerChildren ||
                        <div className="ui-modal__header-content">
                            <Typography style={{ fontWeight: 'inherit', lineHeight: 'inherit' }}>{title}</Typography>
                            <IconButton title="Close" icon={<CloseMUI />} onClick={onCloseClick} />
                        </div>
                    }
                </header>
                <section className="ui-modal__body">
                    <div className="ui-modal__body-content">{children}</div>
                </section>
            </article>
        </div>
    );
}
