from tika import parser   
from flask import Flask, request, jsonify
from flask_cors import CORS


def get_document():

    pdf_file = '231201 - TDD Red Flag - TRISTAN (MAH Pont Neuf).pdf'

    # opening pdf file 
    parsed_pdf = parser.from_file(pdf_file) 
    data = parsed_pdf['content']    
    return data.strip()

def ask_chatgpt(tables):    
    return [s.upper() for s in tables]


app = Flask(__name__)
CORS(app)  

@app.route('/api/call_chatgpt', methods=['POST'])
def call_chatgpt():
    # Parse the JSON from the incoming request
    data = request.get_json()

    # Ensure it's a list (or convert it to a list based on your logic)
    if not isinstance(data, list):
        return jsonify({'error': 'Expected a list'}), 400

    for s in data:
        print(s, '\n')
    response_data = ask_chatgpt(data)

    print('RESPONSE: ', '\n')
    print(response_data)
    # Respond with a JSON
    return jsonify(response_data)

@app.route('/api/echo', methods=['GET'])
def echo():
    return "I am alive!"

if __name__ == '__main__':
    app.run(debug=True, port=62000)