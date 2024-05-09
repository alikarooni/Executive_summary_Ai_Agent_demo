// src/components/Row/Row.js
import React from 'react';
import './Row.css';

const Row = ({ leftContent, rightContent }) => {
  return (
    <div className="row">
      <div className="column">{leftContent}</div>
      <textarea className="column" defaultValue={rightContent}></textarea>
    </div>
  );
};

export default Row;
