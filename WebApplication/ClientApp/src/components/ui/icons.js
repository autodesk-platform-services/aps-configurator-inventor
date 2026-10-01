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
import './icons.css';

// SVG paths copied from @hig/icons 4.1.0 (Apache-2.0, see NOTICE).
// The display names match HIG's, so Enzyme `find('Complete24')` keeps working.
function icon(name, size, paths) {
    const Icon = ({ className, ...rest }) => (
        <svg xmlns="http://www.w3.org/2000/svg" width={`${size}px`} height={`${size}px`} viewBox={`0 0 ${size} ${size}`}
            className={cx('ui-icon', className)} {...rest}>
            {paths}
        </svg>
    );
    Icon.displayName = name;
    return Icon;
}

export const Alert24 = icon('Alert24', 24, <>
    <path d="M21.84 18.1L13.07 3.75a1.25 1.25 0 0 0-2.14 0L2.16 18.1A1.25 1.25 0 0 0 3.23 20h17.54a1.25 1.25 0 0 0 1.07-1.9zm-.85.77a.24.24 0 0 1-.22.13H3.23a.24.24 0 0 1-.23-.13.23.23 0 0 1 0-.25l8.79-14.35a.24.24 0 0 1 .42 0L21 18.62a.23.23 0 0 1 0 .25z"/>
    <circle cx="12" cy="16" r="1"/>
    <path d="M11.48 13.68a.36.36 0 0 0 .34.32h.36a.36.36 0 0 0 .34-.32L13 9.19a1.68 1.68 0 0 0 0-.5.94.94 0 0 0-.94-.69h-.12a.94.94 0 0 0-.91.69 1.68 1.68 0 0 0 0 .5z"/>
</>);

export const Cloud16 = icon('Cloud16', 16,
    <path d="M13.5 8h-.05a4 4 0 1 0-7.31-3H6a3 3 0 0 0-3 3h-.5a2.5 2.5 0 0 0 0 5h11a2.5 2.5 0 0 0 0-5z"/>);

export const CloseMUI = icon('CloseMUI', 16,
    <path d="M14.4 2.4l-.8-.8L8 7.3 2.4 1.6l-.8.8L7.3 8l-5.7 5.6.8.8L8 8.7l5.6 5.7.8-.8L8.7 8l5.7-5.6z"/>);

export const Error24 = icon('Error24', 24, <>
    <path d="M12 2L2 12l10 10 10-10zM3.41 12L12 3.41 20.59 12 12 20.59z"/>
    <circle cx="12" cy="15" r="1"/>
    <path d="M11.88 13h.24a.55.55 0 0 0 .54-.5L13 8a1 1 0 0 0-2 0l.34 4.5a.55.55 0 0 0 .54.5z"/>
</>);

export const Complete24 = icon('Complete24', 24, <>
    <path d="M12 3a9 9 0 1 0 9 9 9 9 0 0 0-9-9zm0 17a8 8 0 1 1 8-8 8 8 0 0 1-8 8z"/>
    <path d="M16.29 8.73l-5.65 5.65-2.5-2.49-.71.71 3.21 3.2L17 9.43l-.71-.7z"/>
</>);

export const Upload24 = icon('Upload24', 24, <>
    <path d="M12.1 2L6 10h3v6h6v-6h3zM14 9v6h-4V9H8l4.06-5.33L16 9z" fillRule="evenodd"/>
    <path d="M17 16v2H7v-2H3v6h18v-6zm3 5H4v-4h2v2h12v-2h2z"/>
</>);

export const Trash24 = icon('Trash24', 24,
    <path d="M19 5h-3V3.13A1.08 1.08 0 0 0 15 2H8a1.11 1.11 0 0 0-1 1.13V5H4v4h1v13h13V9h1zm-4-2v2h-1V4H9v1H8V3zm2 18H6V9h11zm1-13H5V6h13z"/>);

export const Service24 = icon('Service24', 24,
    <path d="M12 2A10 10 0 0 0 2 12a9.88 9.88 0 0 0 1.62 5.43v.05c.18.26.36.53.56.78l4.92-4.92-.23-.61a4.11 4.11 0 0 1 .93-4.37 4.07 4.07 0 0 1 2.89-1.2 4.19 4.19 0 0 1 1.31.2l-2.86 2.86.14 2.4 2.44.22L16.64 10a4.11 4.11 0 0 1-5.36 5.14l-.61-.23-4.21 4.17-.72.72c.25.2.52.38.78.56h.05A9.88 9.88 0 0 0 12 22a10 10 0 0 0 0-20zm0 19a9 9 0 0 1-4.7-1.34L10.92 16a5.1 5.1 0 0 0 6-7.74l-3.5 3.51-1.12-.1-.07-1.09 3.51-3.5a5.1 5.1 0 0 0-7.74 6L4.34 16.7A9 9 0 1 1 12 21z"/>);

export const Folder24 = icon('Folder24', 24,
    <path d="M11.79 6L9 4H2v16h19V6zM3 5h6l1.8 1.42L10 7H3zm17 14H3V8h7.26l1.52-1H20z"/>);

// Used inside the ui components only.
export const Info24 = icon('Info24', 24, <>
    <path d="M12 3a9 9 0 1 0 9 9 9 9 0 0 0-9-9zm0 17a8 8 0 1 1 8-8 8 8 0 0 1-8 8z"/>
    <path d="M13 12v-1h-3v1h1v4h-1v1h4v-1h-1v-4z"/>
    <circle cx="12" cy="8" r="1.25"/>
</>);

export const Caret10 = icon('Caret10', 10,
    <path d="M8.71 3.22a.75.75 0 0 0-1.06 0L5 5.87 2.35 3.22a.75.75 0 0 0-1.06 0 .74.74 0 0 0 0 1.06l3.36 3.36a.5.5 0 0 0 .7 0l3.36-3.36a.74.74 0 0 0 0-1.06z"/>);

export const CheckboxTick16 = icon('CheckboxTick16', 16, <path d="M4.2 8.6l1.4-1.5 1.7 1.5 3.6-4 1.5 1.3-5 5.5-3.2-2.8z"/>);

export const CheckboxDash16 = icon('CheckboxDash16', 16, <path d="M4 7h8v2H4z"/>);

export const Checkmark16 = icon('Checkmark16', 16, <path d="M6.2 12L2 8.1l1.3-1.3 2.9 2.7L12.7 3 14 4.3 6.2 12z"/>);
