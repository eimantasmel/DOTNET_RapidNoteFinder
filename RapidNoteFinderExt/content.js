import React, {Component, useContext} from 'react';
import ReactDom from 'react-dom/client';
import './assets/css/general.css';
import NoteContext, {context} from "./contexts/NoteContext";
import InteractiveBox from "./components/InteractiveBox";

function App(props) {
        const {showBox} = useContext(context);
        return (
            <>
            {
                showBox &&
                (<InteractiveBox />)
            }
            </>
        )
}

const entryPoint = document.createElement('div')
entryPoint.id = 'entrypoint';
document.body.appendChild(entryPoint);

const root = ReactDom.createRoot(document.getElementById('entrypoint'));
root.render(
    <NoteContext>
        <App />
    </NoteContext>
);