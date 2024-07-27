import React, {useState, useContext, useEffect, useRef, useLayoutEffect  } from 'react';
import {context} from "../contexts/NoteContext";
import '../assets/css/search-note-container.css';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import LoaderComponent from './LoaderComponent';

function componentDidMount() {
    document.querySelectorAll('#entrypoint input, #entrypoint textarea').forEach((element) => {
        element.addEventListener('mouseover', (e) => {
            e.target.focus();
        })
    });    
}

function SearchNoteContainer(props) {
    const [editorValue, setEditorValue] = useState('');
    const [isUpdated, setIsUpdated] = useState(false);
    const [description, setDescription] = useState('');
    const {findNote, updateNote, showLoader, changeNote} = useContext(context)
    const editorRef = useRef(null);
    const editorValueRef = useRef('');
    const handleChange = (value) => {
        setEditorValue(value);
        setIsUpdated(true);
    };

    const backArrowListener = (e) => {
        changeNote(setEditorValue, setDescription, 'back');
    };
    const forwardArrowListener = (e) => {
        changeNote(setEditorValue, setDescription);
    };



    useEffect(() => {
        componentDidMount();
        editorValueRef.current = editorValue;
        document.querySelector('.MuiButtonBase-root.MuiIconButton-root[aria-label="back"]').addEventListener('click', forwardArrowListener);
        document.querySelector('.MuiButtonBase-root.MuiIconButton-root[aria-label="forward"]').addEventListener('click', backArrowListener);
        
        // This will be called when the component is rerendered
        return () => {
            if(document.querySelector('.MuiButtonBase-root.MuiIconButton-root[aria-label="back"]'))
            {
                document.querySelector('.MuiButtonBase-root.MuiIconButton-root[aria-label="back"]').removeEventListener('click', forwardArrowListener);
                document.querySelector('.MuiButtonBase-root.MuiIconButton-root[aria-label="forward"]').removeEventListener('click', backArrowListener);
            }
        };
    })

    useLayoutEffect(() => {
        // This will be called when the component is unmounted
        return () => {
            if(isUpdated)
                updateNote(editorValueRef.current);
        };
    }, [isUpdated]);

        return (
            <div className={'ext-container'}>
                <input type="text" value={description} className={'description'} placeholder={'Describe your note'} onKeyUp={(e) => {
                    if(e.key == 'Enter')
                        findNote(description, setEditorValue);
                }}
                onChange={(e) => {
                    setDescription(e.target.value);
                }}/>

                {showLoader && <LoaderComponent />}

                <div style={{ maxHeight: '90%', overflowY: 'auto', overflowX: 'auto', width: '100%' , display: 'none', color: 'white', marginBottom: '10px' }} id={'quill-editor-wrapper'}>
                    <ReactQuill id={'quill-editor'} value={editorValue} onChange={handleChange} ref={editorRef} />
                    <input type="hidden" id={'note-id'}/>
                </div>
            </div>
        );
}

export default SearchNoteContainer;