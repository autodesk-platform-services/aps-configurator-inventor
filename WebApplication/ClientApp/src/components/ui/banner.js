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
import IconButton from './iconButton';
import Typography from './typography';
import { CloseMUI, Info24 } from './icons';
import './banner.css';

/**
 * Info bar (replaces @hig/banner, "primary" type): icon, message, an `actions` area and a close
 * button that calls `onDismiss`. Renders nothing while `isVisible` is false.
 */
export default function Banner({ isVisible = true, onDismiss, actions, children }) {
    if (!isVisible) return null;

    return (
        <div className="ui-banner" role="alert" aria-live="polite">
            <figure className="ui-banner__icon"><Info24 /></figure>
            <div className="ui-banner__content">
                <div className="ui-banner__message"><Typography>{children}</Typography></div>
                {actions && <div className="ui-banner__actions">{actions}</div>}
            </div>
            {onDismiss &&
                <div className="ui-banner__dismiss">
                    <IconButton title="Close" icon={<CloseMUI />} onClick={onDismiss} />
                </div>
            }
        </div>
    );
}
