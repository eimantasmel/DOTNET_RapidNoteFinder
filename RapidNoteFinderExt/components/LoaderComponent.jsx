import React from "react";
import { Spinner } from "react-bootstrap";
import '../assets/css/loading-animation.css'

export default function LoaderComponent() {
  return (
    <div className="loading-container">
      <div className="loading-animation"></div>
      <p>Loading...</p>
    </div>
  );
}
