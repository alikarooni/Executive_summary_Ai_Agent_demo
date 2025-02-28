// src/pages/MainPage/MainPage.js
import React from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../../components/Header/Header';
import './MainPage.css';

function MainPage() {
  const navigate = useNavigate();
  const projects = ["Project A", "Project B", "Project C"]; // Example projects

  const handleNewProjectClick = () => {
    navigate('/projects'); // This will navigate to the ProjectPage for creating a new project
  };

  return (
    <div className="main-page">
      <Header />
      <div className="project-controls">
        <button className="project-button" onClick={handleNewProjectClick}>New Project</button>
        <button className="project-button">Open Project</button>
      </div>
      <div className="project-list">
        <h3>Previous Projects:</h3>
        <ul>
          {projects.map((project, index) => (
            <li key={index}>{project}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default MainPage;
