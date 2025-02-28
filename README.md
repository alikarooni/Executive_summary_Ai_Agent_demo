# Executive Summary AI Agent

Welcome to the **ExecutiveSummary_ai_agent** project—a Python-based AI tool crafted as a Minimum Viable Product (MVP) for [Savills](https://www.savills.com), a global real estate powerhouse. This application automates the generation of executive summary reports from sprawling technical due diligence documents and equips employees with an interactive chatbot to refine the results. Born from the need to streamline a 100+ page manual reporting process, it’s a testament to how AI can transform workflows while keeping humans in the loop.

![Savills Logo](savills-logo-yellow.png)

*Official Savills website: [www.savills.com](https://www.savills.com)*

## Project Overview

This tool was designed to tackle a real pain point at Savills: manually compiling lengthy executive summaries from technical reports. By automating the initial draft and adding a chatbot for real-time edits, it slashes time spent on reporting and boosts output quality—freeing up staff for higher-value tasks.

### Key Components
- **Report Generation**: Pulls data from due diligence PDFs and uses OpenAI’s GPT-3.5-turbo to craft concise summaries.
- **Chatbot Interaction**: Offers a UI-connected API for staff to ask questions, tweak, or rewrite report sections.
- **Automation**: Turns a tedious, human-driven process into a fast, AI-enhanced solution.

### Background
Before this project, Savills employees wrestled with 100+ page reports, piecing together executive summaries by hand. The MVP introduced an AI agent to draft these reports from PDFs, then layered on a chatbot so staff could polish the content interactively. It’s a practical blend of automation and human oversight, built with real estate workflows in mind.

## Features
- **PDF Processing**: Extracts text cleanly from complex documents.
- **AI Responses**: Delivers context-aware answers using GPT-3.5-turbo, grounded in the source PDF.
- **API Access**: Provides endpoints for querying the AI and checking server health.
- **User-Friendly**: Simplifies report refinement with a conversational interface.

## Project Structure
- **Backend**: Powered by Python, with Flask for APIs and Tika for PDF parsing.
- **AI Integration**: Harnesses OpenAI’s language model for generation and Q&A.
- **Demos**: Includes Jupyter Notebooks for prototyping and testing.
- **Dependencies**: Captured in `requirements.txt` for quick setup.
- **Documentation**: Fully detailed in this `README.md`.

## Getting Started
Ready to dive in? Here’s how to set it up locally:

1. **Clone the Repo**:
   ```bash
   git clone https://github.com/yourusername/ExecutiveSummary_ai_agent.git
   
2. **Run the agent**:
    ```bash
    python main.py

3. **Run the UI**:
    ```bash
    npm run start