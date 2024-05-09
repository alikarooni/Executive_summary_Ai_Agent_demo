// src/components/Input/Input.js
import React from 'react';
import './Textbox.css';

const Textbox = ({ label, type, value, onChange, id, ...props }) => {
  return (
    <div className="input-group">
      {label && <label htmlFor={id}>{label}</label>}
      <input
        type={type}
        id={id}
        value={value}
        onChange={onChange}
        {...props}
      />
    </div>
  );
};

export default Textbox;
