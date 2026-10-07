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
import './tooltip.css';

/**
 * Tooltip bubble around `children` (replaces @hig/tooltip). Shown while hovered when `openOnHover`,
 * or controlled with `open`. `anchorPoint` is the bubble's attachment point, as in HIG:
 * "top-center" puts the bubble below the anchor, "bottom-center" above it.
 * The `{className}__flyout-container` class is kept for parametersContainer.css and the UI tests.
 */
export default function Tooltip({ content, anchorPoint = 'top-center', open, openOnHover, className, children }) {
    const [hovered, setHovered] = useState(false);
    const visible = (open === undefined ? openOnHover && hovered : open) && content;

    return (
        <div className={cx('ui-tooltip', className)} onMouseEnter={() => setHovered(true)} onMouseLeave={() => setHovered(false)}>
            <div className="ui-tooltip__action">{children}</div>
            {visible &&
                <div role="tooltip" className={cx('ui-tooltip__flyout', `ui-tooltip__flyout--${anchorPoint}`, withSuffix(className, '__flyout-container'))}>
                    <div className="ui-tooltip__pointer" aria-hidden="true" />
                    <div className="ui-tooltip__panel">{typeof content === 'function' ? content() : content}</div>
                </div>
            }
        </div>
    );
}
