import React, {useContext, useRef, useState, useEffect} from 'react';
import {context} from '../contexts/NoteContext';
import '../assets/css/create-note-container.css';
import ReactQuill from 'react-quill';
import LoaderComponent from './LoaderComponent';
import 'react-quill/dist/quill.snow.css';

function componentDidMount() {
    document.querySelectorAll('#entrypoint input, #entrypoint textarea').forEach((element) => {
        element.addEventListener('mouseover', (e) => {
            e.target.focus();
        })
    })
}
function CreateNoteContainer(props) {
    const [editorValue, setEditorValue] = useState('');
    const editorRef = useRef(null);
    const handleChange = (value) => {
        setEditorValue(value);
    };

    const handlePaste = () => {
        const editor = editorRef.current.getEditor();
        const range = editor.getSelection();
        editor.setSelection(range.index + 1, range.length);
    };

    const {createNote, showLoader} = useContext(context);
    const [description, setDescription] = useState('');

    useEffect(() => {
        componentDidMount();
    })
        return (
            <div className={'create-note-container ext-container'}>
                { showLoader ? <LoaderComponent /> :
                    <>
                        <input type="text" className={'description'} placeholder={'Your note description...'} value={description} onChange={(e) => setDescription(e.target.value)}/>
                        <div style={{ maxHeight: '90%', overflowY: 'auto', overflowX: 'auto', width: '100%', color: 'white', marginBottom: '10px' }}>
                            <ReactQuill theme="snow" value={editorValue} onChange={handleChange} onPaste={handlePaste}  ref={editorRef} placeholder={'Your note content...'} />
                        </div>
                        <button className={'save-btn'} onClick={() => {
                            createNote(editorValue, description);
                        }}>Save</button>
                    </>
                }
            </div>
        );
}

export default CreateNoteContainer;