from tika import parser   
from flask import Flask, request, jsonify
from flask_cors import CORS
from openai import OpenAI
import json
import re

def preprocess_json(text):
        # Pattern to identify single-quoted keys and values
        # This pattern aims to replace only the outermost single quotes in JSON-like structures
        pattern = r"(?<!\\)'([^'\\]*(?:\\.[^'\\]*)*)'"

        # Replace single quotes with double quotes while trying to preserve escaped quotes and sequences
        processed_text = re.sub(pattern, r'"\1"', text)

        return processed_text

def extract_json(text):
        nest_braces = 0
        nest_brackets = 0
        json_str = ''
        in_json = False

        for char in text:
            if char in ['{', '[']:
                if char == '{':
                    nest_braces += 1
                else:
                    nest_brackets += 1
                in_json = True

            if in_json:
                json_str += char

            if char in ['}', ']'] and in_json:
                if char == '}':
                    nest_braces -= 1
                else:
                    nest_brackets -= 1

            if nest_braces == 0 and nest_brackets == 0 and in_json:
                break

        if json_str:
            try:
                processed_json_str = preprocess_json(json_str)
                return json.loads(processed_json_str)
            except json.JSONDecodeError as e:
                # log(f"Error decoding JSON: {e}")
                return None
        else:
            return None 

def get_document():

    pdf_file = '231201 - TDD Red Flag - TRISTAN (MAH Pont Neuf).pdf'

    # opening pdf file 
    parsed_pdf = parser.from_file(pdf_file) 
    data = parsed_pdf['content']    
    return data.strip()

def ask_chatgpt(tables):
    document = get_document()
    CHATGPT_API_KEY = "sk-proj-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
    client = OpenAI(api_key=CHATGPT_API_KEY)

    start = 0
    end = 0

    updated_tables = []
    for i, row in enumerate(tables):
        if(1 > 0):
            updated_tables.append(row.upper())
            continue

        completion = client.chat.completions.create(
        model="gpt-3.5-turbo",
        messages=[
            {"role": "system", "content": 
            f'''
        Rules:
            - Only and only use provided document to answer questions
            - REMEMBER, I USE ^ AS DELIMITER. Respect ^ whenever you see it!
            - Your task it to fill the text between [ and ]. Sometimes you HAVE TO WRITE ADDITIONAL sentences/list items.
            - Remove [ and ] in the your reponses.
            - Wrap your response between {{}} 
            - Never ever shorten the answer.
            - Do not say a WORD. Just return my text with your responses included
            Document: {document} \n'''},
        {"role": "user", "content": f'''Question: Fill the gap in the following text, based on the document: {row}''' }])

        content = completion.choices[0].message.content.replace('\u200c', ' ')
        
        print(f'ChatGPT request: {row}] \n')
        print(f'ChatGPT respoonse: {content}] \n')
    
        dict_content = extract_json(content)        
        if(type(dict_content) is dict):
            response = ""
            for k, v in extract_json(content).items():
                print(f'key: {k}, value: {v}')
                response += v + '\n'
            updated_tables.append(response)
        else:
            print(f'no dict: {content}')
            updated_tables.append(row)
    print(updated_tables)
    return updated_tables


app = Flask(__name__)
CORS(app)  

@app.route('/api/call_chatgpt', methods=['POST'])
def call_chatgpt():
    # Parse the JSON from the incoming request
    data = request.get_json()

    # Ensure it's a list (or convert it to a list based on your logic)
    if not isinstance(data, list):
        return jsonify({'error': 'Expected a list'}), 400

    # for s in data:
    #     print(s, '\n')
    response_data = ask_chatgpt(data)

    print('RESPONSE: ', '\n')
    # print(response_data)
    # Respond with a JSON
    return jsonify(response_data)

@app.route('/api/echo', methods=['GET'])
def echo():
    return "I am alive!"

if __name__ == '__main__':
    app.run(debug=True, port=62000)