import { useParams } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import Header from '../../components/Header/Header';
import Table from '../../components/Table/Table';
import Chatbot from '../../components/Chatbot/Chatbot';
import './DesignerPage.css';

function DesignerPage() {
  const { projectId } = useParams();
  const [documentParts, setDocumentParts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await fetch(`/api/Project/GetDocumentParts/${projectId}`, {
          method: 'GET',          
        });
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        const data = await response.json();
        setDocumentParts(data); // Assume the response is the array you need
      } catch (error) {
        console.error("Failed to fetch document parts: ", error);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [projectId]); // Dependency array includes projectId to re-fetch if it changes

  return (
    <div className="designer-page">
      <Header />
      <h3>Project ID: {projectId}</h3>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="table-container">
          <Table data={documentParts} />
        </div>
      )}
      <Chatbot />
    </div>
  );
}

export default DesignerPage;
