# pip install langchain-openai
from langchain_openai.chat_models import ChatOpenAI
from langchain.prompts import PromptTemplate
from langchain.schema.output_parser import StrOutputParser
from tika import parser

def get_document():
    pdf_file = '231201 - TDD Red Flag - TRISTAN (MAH Pont Neuf).pdf'

    # opening pdf file 
    parsed_pdf = parser.from_file(pdf_file) 
    data = parsed_pdf['content']    
    return data.strip()


document = get_document()

application_prompt = '''
Rules:
    - Only and only use provided document to answer questions
    - the question is provided in the csv format text. REMEMBER, I USE ^ AS DELIMITER.
    - Your task it to fill the gap -anything between [ and ] - in the question text. Sometimes you HAVE TO WRITE ADDITIONAL ONE OR TWO MORE SENTENCES/PROVIDE LIST ITEMS. BE CAREFUL AND DO YOUR TASK CAREFULLY
    - Your only response will be in JSON format. DO NOT SAY ONE MORE WORD.
    - Not all rows have GAPs. You will NOT TOUCH THEM.
    - YOUR REPONSE WILL BE LIKE: 
    "PARAGRAPH": "SOME TEXT [MISSING_VALUE] SOME TEXT [MISSING_VALUE]  SOME", "ROW_2": "SOME TEXT [MISSING_VALUE] SOME TEXT [MISSING_VALUE]  SOME", etc 
    - Never ever shorten the answer.
    - Remove [ and ] in the your reponse .'''
application_prompt += f'''Document: {document} \n'''

row = '''{
"PARAGRAPH:^ This report is intended for the addressee only and third parties are not permitted to rely on the contents without the express permission of Savills France.",
"PARAGRAPH:^Survey Limitations",
"PARAGRAPH:^This [Select report type] and our inspection have been undertaken and prepared in accordance with our Standard Survey Limitations (Commercial Building Surveys), which is attached as an appendix. ",
"PARAGRAPH:^No opening up or testing of the building fabric or building services installations has been undertaken unless stated to the contrary in this report.",
"PARAGRAPH:^Given that you are acquiring the freehold interest in the property, our inspection and report concentrate on significant items of disrepair.  Minor disrepair items are therefore excluded from this report",
"PARAGRAPH:^Inspection Details",
"PARAGRAPH:^This inspection was carried out by [insert surveyor name], Savills on [insert date] and we were accompanied by the [e.g. property manager, etc.]",
"PARAGRAPH:^The estate was [not] fully accessible. The inspection was undertaken on a visual basis, without proving materials or destructive investigations. Pictures are used to clarify the text. ",
"PARAGRAPH:^The weather was [e.g. heavy rainfall, clear and cool, etc.] for the duration of the inspection.  ",
"PARAGRAPH:^The elevation facing [name of street] is deemed to face [e.g. north, south, etc.], with all other directional references following this orientation.",
"PARAGRAPH:^Documents",
"PARAGRAPH:^We have [not (delete if not applicable)] been provided with access to the online data room (Espace Notarial). The documentation reviewed has been referred to as necessary in the report. The documentation is [comprehensive and complete / largely comprehensive and complete / not complete]. The “Q&A” (Questions & Answers) option within the data room has [not (delete if not applicable)] been made available for use during the due diligence process [and our questions have been posted directly (delete if not applicable)]. [The list of documents that have not been provided in the data room is enumerated in Appendix 1.]",
"PARAGRAPH:^Property Description",
"PARAGRAPH:^General Description",
"PARAGRAPH:^The subject property is [a/an] [insert age] year[s] old [warehouse/office] building situated [on ZAC] [insert ZAC name] in [Municipality] in the department of [insert Department name] ([insert number of department]). It is located around [insert distance]km [geographical location (e.g. northwest)] from [Municipality name of closest main town] city centre and [insert distance]km to the [geographical location (e.g. northwest)] of [insert name of other known main city for reference] The building is [not] accessible by public transport.",
"PARAGRAPH:^The goods delivery to the warehouse is along the [front, rear etc.] elevation with a [insert number]m deep truck yard to the front of the loading bays according to [onsite measurement/aerial plans]. The office accommodation is on the [front elevation between cells 2 and 3, etc.]. There [is/are] [one/two] battery charging room[s] located to the [insert location] (see aerial plan below).",
"PARAGRAPH:^The site is bounded by [insert street/avenue, etc., name] to the [geographical location (e.g. northwest)], [insert street/avenue, etc. 2, name] to the [geographical location (e.g. northwest)], [insert other streets or adjacent buildings, etc.]. The edge of the property is outlined in red in the aerial photograph below. ",
}'''
    
user_input = f'''user_input: Fill the gap in the following text, based on the document: {row}'''

llm = ChatOpenAI(
    base_url="http://localhost:1234/v1",
    temperature=0.7,
    max_tokens=500,
    model='lmstudio-community/Meta-Llama-3-8B-Instruct-GGUF',    
)
prompt = PromptTemplate(  
    input_variables=["user_input"],
    template=application_prompt
)
chain = prompt | llm | StrOutputParser()
# result = chain.invoke({"user_input": user_input})

#print(result)

#for streaing use
results = chain.stream({"user_input": user_input})
for chunk in results:
    print(chunk, end='')