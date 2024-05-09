// src/App.js
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Chatbot from './components/Chatbot/Chatbot';
import LoginPage from './pages/LoginPage/LoginPage';
import MainPage from './pages/MainPage/MainPage';
import ProjectPage from './pages/ProjectPage/ProjectPage';
import DesignerPage from './pages/DesignerPage/DesignerPage';

import './App.css';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/main" element={<MainPage />} />
          <Route path="/projects" element={<ProjectPage />} />
          <Route path="/designer/:projectId" element={<DesignerPage />} />
        </Routes>
        <Chatbot />  {/* Chatbot as a sidebar across all pages */}
      </div>
    </Router>
  );
}

export default App;
