import React, {Component, useContext} from 'react';
import {context} from '../contexts/NoteContext'
import '../assets/css/navbar.css';

function Navbar(props) {
        const {showCreateContainer, showSearchContainer} = useContext(context);
        return (
            <div className={'navbar'}>
                <button className={'nav-btn'} id={'showsearch'} onClick={showSearchContainer}>Find</button>
                <button className={'nav-btn'} id={'showcreate'} onClick={showCreateContainer}>Create</button>
            </div>
        );
}

export default Navbar;