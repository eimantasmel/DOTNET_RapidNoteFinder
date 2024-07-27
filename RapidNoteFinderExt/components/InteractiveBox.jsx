import React, {useContext} from 'react';
import Navbar from "./Navbar";
import CreateNoteContainer from "./CreateNoteContainer";
import {context} from '../contexts/NoteContext'
import SearchNoteContainer from "./SearchNoteContainer";
import '../assets/css/interactibe-box.css';
import IconButton from '@material-ui/core/IconButton';
import { ArrowBack, ArrowForward } from '@material-ui/icons';

function InteractiveBox(props) {
        const {showCreateCont} = useContext(context)
        return (
            <div id={'interactive-box'}>
                <div style={{padding: 0, margin: 0, position: 'relative', width: '100%', height: '100%'}}>
                    <Navbar />
                    {
                        showCreateCont ?
                            ( <CreateNoteContainer />)
                            :
                            (<>
                            <SearchNoteContainer />
                            <IconButton aria-label="back">
                                <ArrowBack />
                            </IconButton>
                            <IconButton aria-label="forward">
                                <ArrowForward />
                            </IconButton>
                            </>)
                    }

                </div>
            </div>
        );
}

export default InteractiveBox;