import React, {Component} from 'react';
import doubleClickEvent from "../scripts/doubleClick";
import axios from 'axios';
import getAssociation from "../scripts/getAssociation";
import host from '../scripts/apiHost'; 

export const context = React.createContext();
class NoteContext extends Component {

    constructor() {
        super();

        this.state = {
            showBox: false,
            showCreateCont: false,
            showSearchCont: true,
            showLoader: false,
            notes: [],
            currentNoteIndex: 0,
            lastKeyDownContainer: {lastKeyDown: null}
        }
    }
    componentDidMount() {
        document.addEventListener('keydown', (e) => {
            if(e.key == 'F8')
            {
                doubleClickEvent(this.state.lastKeyDownContainer, () => {
                    this.setState(prevState => ({
                        showBox: !prevState.showBox
                    }));
                })
            }
            else if(e.key == 'Escape')
                this.setState({
                    showBox: false
                });
        })
    }

    showSearchContainer() {
        this.setState({
            showCreateCont: false,
            showSearchCont: true,
        }, () => {
            document.querySelector('#showsearch').style.background = 'darkred'
            document.querySelector('#showcreate').style.background = 'deeppink'
        })
    }
    showCreateContainer() {
        this.setState({
            showCreateCont: true,
            showSearchCont: false,
        }, () => {
            document.querySelector('#showsearch').style.background = 'deeppink'
            document.querySelector('#showcreate').style.background = 'darkred'
        })
    }

    async createNote(content, description) {
        const associate = await getAssociation() ?? '';
        this.setState({
            showLoader: true
        });
        fetch(`${host}/api/note/add`, {
            method: 'post',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                content,
                description,
                associate
            }),
        })
        .then(response => {
            return response.json();
        })
        .then(data => {
            this.setState({
                showLoader: false
            });
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            this.setState({
                showLoader: false
            });
        });
    }

    updateNote(content) {
        const noteId = document.getElementById('note-id').dataset.noteId;
        fetch(`${host}/api/note/update`, {
            method: 'put',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              content: content,
              id: noteId,
              description: 'default'
            })
          })
          .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
          });
    }

    async findNote(description, setEditorValue) {
        this.setState({
            showLoader: true
        });
        const associate = await getAssociation() ?? '';
        axios.get(`${host}/api/note/find`, {
            headers: {
                'Content-Type': 'application/json',
            },
            params: {
                description: description,
                associate: associate
            }
        })
        .then(response => {
            this.setState({
                showLoader: false,
            });
            if(response.data)
            {
                const {content, id } = response.data[0];
                document.querySelector('.ext-container #quill-editor-wrapper').style.display = 'block';
                const noteIdElement = document.querySelector('#quill-editor-wrapper #note-id')
                noteIdElement.dataset.noteId = id;
                setEditorValue(content);
                this.setState({
                    notes: response.data
                });
            }
        })
        .catch(err => {
            console.log(err);
            this.setState({
                showLoader: false
            });
        })
    }

    changeNote(setEditorValue, setDescription, move="forward") {
        if(this.state.notes.length == 0 || this.state.notes.length == 1) return;
        if (move === 'forward') {
            this.setState((prevState) => ({
                currentNoteIndex: prevState.currentNoteIndex === this.state.notes.length - 1 ? 0 : prevState.currentNoteIndex + 1
            }), () => {
                if(document.querySelector('#quill-editor-wrapper #note-id'))
                {
                    setEditorValue(this.state.notes[this.state.currentNoteIndex].content);
                    const noteIdElement = document.querySelector('#quill-editor-wrapper #note-id')
                    noteIdElement.dataset.noteId = this.state.notes[this.state.currentNoteIndex].id;
                    setDescription(this.state.notes[this.state.currentNoteIndex].description);
                }
            });
        } else {
            this.setState((prevState) => ({
                currentNoteIndex: prevState.currentNoteIndex === 0 ? this.state.notes.length - 1 : prevState.currentNoteIndex - 1
            }), () => {
                if(document.querySelector('#quill-editor-wrapper #note-id'))
                {
                    setEditorValue(this.state.notes[this.state.currentNoteIndex].content);
                    const noteIdElement = document.querySelector('#quill-editor-wrapper #note-id')
                    noteIdElement.dataset.noteId = this.state.notes[this.state.currentNoteIndex].id;
                    setDescription(this.state.notes[this.state.currentNoteIndex].description);
                }
            });
        }
    }

    render() {
        return (
        <context.Provider value={{
            ...this.state,
            showCreateContainer: this.showCreateContainer.bind(this),
            showSearchContainer: this.showSearchContainer.bind(this),
            createNote: this.createNote.bind(this),
            findNote: this.findNote.bind(this),
            updateNote: this.updateNote.bind(this),
            changeNote: this.changeNote.bind(this),
        }}>
            {this.props.children}
        </context.Provider>
        );
    }
}

export default NoteContext;