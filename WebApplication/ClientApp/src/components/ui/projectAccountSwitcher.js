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
import Flyout from './flyout';
import { Caret10 } from './icons';
import './projectAccountSwitcher.css';

function Thumbnail({ project }) {
    return (
        <span className="ui-project-switcher__thumbnail">
            {project.image && <img className="ui-project-switcher__image" src={project.image} alt={project.label} />}
            <span className="ui-project-switcher__initial">{project.label.slice(0, 1).toUpperCase()}</span>
        </span>
    );
}

/**
 * The active project with a list to pick another (replaces @hig/project-account-switcher).
 * `onSelect(id)` is called with the picked project's id. Keeps the markup the UI tests select:
 * a `div[role=button]` anchor with an <img>, a <p> label and an <svg> caret, and
 * `li[role=menuitem] > span` items.
 */
export default function ProjectAccountSwitcher({ projects = [], activeProject, projectTitle, onSelect }) {
    const active = projects.find(project => project.id === activeProject);

    const onEnterOrSpace = action => e => {
        if (e.key !== 'Enter' && e.key !== ' ') return;
        e.preventDefault();
        action();
    };

    return (
        <Flyout panelClassName="ui-project-switcher__panel"
            anchor={({ open, toggle }) => (
                <div role="button" tabIndex={0} aria-haspopup="menu" aria-expanded={open} className="ui-project-switcher__anchor"
                    onClick={toggle} onKeyDown={onEnterOrSpace(toggle)}>
                    <div className="ui-project-switcher__current">
                        {active && <Thumbnail project={active} />}
                        {active && <p className="ui-project-switcher__current-label">{active.label}</p>}
                    </div>
                    <div className="ui-project-switcher__caret"><Caret10 /></div>
                </div>
            )}>
            {close => (
                <ul role="menu" className="ui-project-switcher__list">
                    <li role="presentation"><span className="ui-project-switcher__title">{projectTitle}</span></li>
                    {projects.map(project => {
                        const pick = () => {
                            close();
                            onSelect(project.id);
                        };
                        return (
                            <li key={project.id} role="menuitem" tabIndex={0} className="ui-project-switcher__item"
                                onClick={pick} onKeyDown={onEnterOrSpace(pick)}>
                                <Thumbnail project={project} />
                                <span className="ui-project-switcher__label">{project.label}</span>
                            </li>
                        );
                    })}
                </ul>
            )}
        </Flyout>
    );
}
