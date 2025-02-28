// src/components/Header/Header.js
import React from 'react';
import './Header.css';
import userIcon from '../../asserts/user_icon.jpg'; // Adjust the path as needed

function Header() {
  return (
    <div className="header">
      <div className="user-info">
        <img src={userIcon} alt="User" className="user-icon" />
        <span>User1</span>
      </div>
      {/* Other header content */}
    </div>
  );
}

export default Header;
