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
import Avatar from './avatar';
import Flyout from './flyout';
import IconButton from './iconButton';
import './topNav.css';

/** Top bar with the logo on the left and `rightActions` on the right (replaces @hig/top-nav). */
export default function TopNav({ logo, rightActions }) {
    return (
        <header className="ui-top-nav">
            {logo}
            <div className="ui-top-nav__spacer" aria-hidden="true" />
            {rightActions}
        </header>
    );
}

export function Logo({ link, label, children }) {
    return (
        <div className="ui-top-nav__logo">
            <a className="ui-top-nav__logo-link" href={link} aria-label={label}>{children}</a>
        </div>
    );
}

export function Interactions({ children }) {
    return <div className="ui-top-nav__interactions">{children}</div>;
}

export function Separator() {
    return <div role="presentation" aria-hidden="true" className="ui-top-nav__separator" />;
}

/** Icon button that opens a panel. UI tests select it by `//button[@title=...]`. */
export function NavAction({ title, icon, children }) {
    return (
        <div className="ui-top-nav__action">
            <Flyout align="right" panelClassName="ui-top-nav__panel"
                anchor={({ open, toggle }) => <IconButton title={title} icon={icon} aria-expanded={open} onClick={toggle} />}>
                {children}
            </Flyout>
        </div>
    );
}

/** The user's avatar, opening a panel. Keeps HIG's `button > span(avatar) > span(initials)` (toolbar.css, UI tests). */
export function ProfileAction({ avatarName, avatarImage, children }) {
    return (
        <div className="ui-top-nav__profile">
            <Separator />
            <div className="ui-top-nav__action ui-top-nav__action--profile">
                <Flyout align="right" panelClassName="ui-top-nav__panel"
                    anchor={({ open, toggle }) => (
                        <button type="button" className="ui-top-nav__profile-button" aria-expanded={open} onClick={toggle}>
                            <Avatar size="medium-32" name={avatarName} image={avatarImage} />
                        </button>
                    )}>
                    {children}
                </Flyout>
            </div>
        </div>
    );
}
