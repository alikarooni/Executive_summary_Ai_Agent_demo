// src/components/Dropdown/Dropdown.js
import React from 'react';
import './Dropdown.css';

function Dropdown({ options, value, onChange }) {
  return (
    <select className="dropdown"
      value={value}
      onChange={onChange}>
      {options.map((option) => (
        <option key={option.value} value={option.value}>{option.label}</option>
      ))}
    </select>
  );
}

export default Dropdown;
