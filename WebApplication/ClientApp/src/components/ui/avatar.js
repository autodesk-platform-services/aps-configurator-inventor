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
import cx from './cx';
import './avatar.css';

// HIG's avatar background colours; the colour is picked from the name the same way HIG does.
const COLORS = ['#bc2b2b', '#871616', '#ce6d3c', '#527c29', '#1d6328', '#298080', '#23688a', '#3d6ac2',
    '#163b84', '#5b5bc2', '#571698', '#801d94', '#be29be', '#8f1d69', '#c84b75'];

/** Split at the last space, like HIG: "Anna Maria Smith" -> ["Anna Maria", "Smith"]. */
function splitName(name) {
    const space = name.lastIndexOf(' ');
    return space === -1 ? [name, ''] : [name.slice(0, space), name.slice(space + 1)];
}

/**
 * Round user picture with initials underneath (replaces @hig/avatar). The initials show when
 * there's no image or it fails to load. `size` is "large" (48px) or "medium-32" (32px, top nav).
 */
export default function Avatar({ name = '', image, size = 'large', className }) {
    const [failedImage, setFailedImage] = useState(null);
    const [first, last] = splitName(name);
    const letters = first + last;
    let charSum = 0;
    for (let i = 0; i < letters.length; i++) charSum += letters.charCodeAt(i);

    return (
        <span role="img" aria-label={`Avatar for ${name}`} className={cx('ui-avatar', `ui-avatar--${size}`, className)}
            // a CSS variable rather than `background-color`, so app CSS (userDetails.css, toolbar.css) can still override it
            style={{ '--ui-avatar-background': COLORS[charSum % COLORS.length] }}>
            {image && image !== failedImage &&
                <span className="ui-avatar__image-wrapper">
                    <img className="ui-avatar__image" src={image} alt={`Avatar image of ${name}`} onError={() => setFailedImage(image)} />
                </span>
            }
            <span className="ui-avatar__initials" aria-hidden="true">{(first.slice(0, 1) + last.slice(0, 1)).toUpperCase()}</span>
        </span>
    );
}
