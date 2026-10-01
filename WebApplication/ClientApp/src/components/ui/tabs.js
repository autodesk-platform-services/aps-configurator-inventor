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

import React, { useRef } from 'react';
import cx, { withSuffix } from './cx';
import './tabs.css';

/** One tab; `Tabs` reads its `label` and renders its children while it's active. */
export function Tab() {
    return null;
}

/**
 * Tab bar plus the active tab's content (replaces @hig/tabs). `onTabChange(index)` is called on
 * click, Enter/Space, or Left/Right arrows. Falsy children (conditional tabs) are skipped.
 * UI tests select the tabs by `li` > `p` text.
 */
export default function Tabs({ className, align = 'left', activeTabIndex = 0, onTabChange, children }) {
    const tabs = React.Children.toArray(children);
    const tabElements = useRef([]);

    const select = index => { if (onTabChange) onTabChange(index); };

    const onKeyDown = (e, index) => {
        const step = { ArrowLeft: -1, ArrowRight: 1 }[e.key];
        if (step) {
            const next = (index + step + tabs.length) % tabs.length;
            tabElements.current[next].focus();
            select(next);
        } else if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            select(index);
        }
    };

    return (
        <div className={cx('ui-tabs', className)}>
            <ul role="tablist" className={cx('ui-tabs__list', `ui-tabs__list--${align}`, withSuffix(className, '__tabs'))}>
                {tabs.map((tab, index) => {
                    const active = index === activeTabIndex;
                    return (
                        <li key={tab.key} role="presentation" className="ui-tabs__item">
                            <div role="tab" aria-selected={active} tabIndex={active ? 0 : -1}
                                ref={element => { tabElements.current[index] = element; }}
                                className={cx('ui-tabs__tab', active && 'ui-tabs__tab--active')}
                                onClick={() => select(index)}
                                onKeyDown={e => onKeyDown(e, index)}>
                                <p className="ui-tabs__label" data-label={tab.props.label}>{tab.props.label}</p>
                            </div>
                        </li>
                    );
                })}
            </ul>
            <div role="tabpanel" className={cx('ui-tabs__content', withSuffix(className, '__content'))}>
                {tabs[activeTabIndex] && tabs[activeTabIndex].props.children}
            </div>
        </div>
    );
}
