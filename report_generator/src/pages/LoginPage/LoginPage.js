// src/pages/LoginPage/LoginPage.js
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Textbox from '../../components/Textbox/Textbox';
import Button from '../../components/Button/Button';
import './LoginPage.css';

function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();
    // Assuming login is successful
    navigate('/main'); // Redirect to the MainPage
  };

  return (
    <div className="login-container">
      <div className="form-container"> {/* Added container for form elements */}
        <form className="login-form" onSubmit={handleLogin}>
          <h2>Login</h2>
          <Textbox
            label="Username"
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <Textbox
            label="Password"
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <Button type="submit">Login</Button>
          <a href="#forget-password" className="forgot-password-link">Forgot password?</a>
        </form>
      </div>
    </div>
  );
}

export default LoginPage;
