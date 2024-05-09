// src/components/ListBox/ListBox.js
import React from 'react';
import './ListBox.css';

function ListBox({ items }) {
  return (
    <div className="list-box">
      <ul>
        {items.map((item, index) => (
          <li key={index}>{item}</li>
        ))}
      </ul>
    </div>
  );
}

export default ListBox;
