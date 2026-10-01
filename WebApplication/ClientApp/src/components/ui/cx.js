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

/** Join the truthy class names with spaces. */
export default function cx(...names) {
    return names.filter(Boolean).join(' ');
}

/**
 * Append `suffix` to every class in `className` ("a b", "__input" -> "a__input b__input").
 * HIG derived inner-element classes this way, and app CSS / UI tests still select them.
 */
export function withSuffix(className, suffix) {
    return (className || '').split(/\s+/).filter(Boolean).map(name => name + suffix).join(' ');
}
