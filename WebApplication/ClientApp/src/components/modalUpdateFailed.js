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

import React, { Component } from 'react';

import Modal from './ui/modal';
import { CloseMUI, Error24 } from './ui/icons';
import Typography from './ui/typography';
import './modalUpdateFailed.css';
import HyperLink from './hyperlink';
import Button from './ui/button';
import IconButton from './ui/iconButton';

export class ModalUpdateFailed extends Component {

    render() {
        const modalStyle = { // by design
            width: "371px",
            height: "263px",
            borderLeftStyle: "solid",
            borderLeftWidth: "thick",
            borderLeftColor: "#ec4a41"
        };
        return (
            <Modal
                open={this.props.open}
                title={this.props.title}
                onCloseClick={this.props.onClose}
                style={modalStyle}
                headerChildren={
                    <header id="customHeader">
                        <div className="customHeaderContent">
                            <div className="title">
                                <Error24 className="errorIcon" />
                                <Typography style={{
                                    paddingLeft: "8px",
                                    fontSize: "inherit",
                                    fontWeight: "inherit",
                                    lineHeight: "inherit"
                                }}>{this.props.title}</Typography>
                            </div>
                            <IconButton style={{ width: "24px", height: "24px", marginLeft: "auto", marginRight: "auto" }}
                                icon={<CloseMUI />}
                                onClick={this.props.onClose}
                                title=""
                            />
                        </div>
                    </header>
                }
            >
                <div className="modalFailContent">
                    <div>
                        <Typography><span className="assemblyText">Project:</span> {this.props.label ? this.props.label : "Missing label."}</Typography>
                    </div>
                    <div className="logContainer">
                        <HyperLink link="Open log file" href={this.props.url} />
                    </div>
                </div>
                <div className="modalFailButtonsContainer">
                    <Button className="button" style={
                        { width: '102px', height: '36px', borderRadius: '2px', marginLeft: '12px' }}
                        type="primary"
                        size="small"
                        title="Ok"
                        onClick={this.props.onClose}
                    />
                </div>
            </Modal>
        );
    }
}

export default ModalUpdateFailed;