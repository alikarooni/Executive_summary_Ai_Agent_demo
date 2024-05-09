// src/pages/ProjectPage/ProjectPage.js
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; // Import useNavigate
import Header from '../../components/Header/Header';
import Textbox from '../../components/Textbox/Textbox';
import Dropdown from '../../components/Dropdown/Dropdown';
import FileList from '../../components/FileList/FileList';
import Button from '../../components/Button/Button';
import './ProjectPage.css';

function ProjectPage() {
    const navigate = useNavigate();
    const [files, setFiles] = useState([]);
    const [projectName, setProjectName] = useState('');
    const [projectType, setProjectType] = useState('');
    const projectTypes = [
        { value: 'type1', label: 'Project Type 1' },
        { value: 'type2', label: 'Project Type 2' },
        { value: 'type3', label: 'Project Type 3' },
      ];

    const handleFileSelect = (event) => {
        setFiles([...files, ...event.target.files]);
    };

    const handleProjectNameChange = (event) => {
        setProjectName(event.target.value);
    };

    const handleProjectTypeChange = (event) => {
        setProjectType(event.target.value);
    };

    const handleSubmit = async () => {
        const formData = new FormData();
        files.forEach(file => {
            formData.append('files', file);
        });
        formData.append('projectName', projectName);
        formData.append('projectType', projectType);
        
        try {
            const response = await fetch('/api/Project/AddProject', {
              method: 'POST',
              body: formData,
            });
            if (response.ok) {
              console.log('Files uploaded successfully');
              navigate('/designer/123');
              // Handle success
            } else {
              console.error('Upload failed');
              // Handle errors
            }
          } catch (error) {
            console.error('Error uploading files', error);
            // Handle errors
          }
    };
    
    return (
        <div className="project-page">
            <Header />
            <div className="project-container">
                <div className="project-header">
                    <Textbox placeholder="Project name" className="project-name" value={projectName} onChange={handleProjectNameChange}/>
                    <Dropdown options={projectTypes} className="project-type" value={projectType} onChange={handleProjectTypeChange} />
                </div>
                <FileList items={files.map(files => files.name)} onFileSelect={handleFileSelect}/> {/* No initial files, or pass some if needed */}
                <Button onClick={handleSubmit}>Design Report</Button>
            </div>
        </div>
    );
}

export default ProjectPage;
