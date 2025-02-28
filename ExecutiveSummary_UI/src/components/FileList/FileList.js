// src/components/ListBox/ListBox.js
import React, { useState } from 'react';
import './FileList.css';

function FileList({ items, onFileSelect }) {
  const [uploadedFiles, setUploadedFiles] = useState(items);

  const handleFileUpload = (event) => {
    // Create a new file array, add it to the current state
    const newFiles = Array.from(event.target.files);
    setUploadedFiles(prevFiles => [...prevFiles, ...newFiles.map(file => file.name)]);
  };

  return (
    <div className="file-list">
      <ul>
        {items.map((item, index) => (
          <li key={index}>{item}</li>
        ))}
      </ul>
      <input type="file" onChange={onFileSelect} multiple />
    </div>
  );
}

export default FileList;
