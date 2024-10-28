// src/components/Table/Table.js
import React from 'react';
import Row from '../Row/Row';
import './Table.css';

const Table = ({ data }) => {
  console.log("Table data:", data['documentParts']);  // This will show what data actually is
  return (
    <div className="table">
      {data['documentParts'] && data['documentParts'].map((row, index) => (
        <Row key={index} leftContent={row.paragraph} rightContent={row.gpT_Reponse} updatable={row.hasToGetUpdated}/>
      ))}
    </div>
  );
};

export default Table;
