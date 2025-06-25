/**
 * @jest-environment ./src/test/custom-test-env.js
 */

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
import Enzyme, { shallow } from 'enzyme';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import { App } from './App';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('App', () => {
    it('Test that app will not call adopt with parameters', () => {
        const fetchShowParametersChanged = jest.fn();
        const detectCode = jest.fn();
        const adoptProjectWithParameters = jest.fn();

        const props = {
          fetchShowParametersChanged,
          detectCode,
          adoptProjectWithParameters,
          embeddedModeEnabled: false
        };

        shallow(<App {...props}/>);
        expect(detectCode).toHaveBeenCalled();
        expect(fetchShowParametersChanged).toHaveBeenCalled();
        expect(adoptProjectWithParameters).not.toHaveBeenCalled();
    });

    it('Sets the embedded mode when specified url property', () => {
      const url = "someurl";
      const fetchShowParametersChanged = jest.fn();
      const detectCode = jest.fn();
      const adoptProjectWithParameters = jest.fn();

      const props = {
        fetchShowParametersChanged,
        detectCode,
        adoptProjectWithParameters,
        embeddedModeEnabled: true,
        embeddedModeUrl: url
      };

      shallow(<App {...props}/>);
      expect(fetchShowParametersChanged).not.toHaveBeenCalled();
      expect(adoptProjectWithParameters).toBeCalledWith(url);
    });
  });
});