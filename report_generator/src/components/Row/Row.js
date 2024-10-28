// src/components/Row/Row.js
import React from 'react';
import './Row.css';

const Row = ({ leftContent, rightContent, updatable }) => {
  return (
    <div className="row">
      <div>{updatable? "true" : "false"}</div>
      <div className="column">{leftContent}</div>
      <textarea className="column" defaultValue={rightContent}></textarea>
    </div>
  );
};

export default Row;
