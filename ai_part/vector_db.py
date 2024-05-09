from pymilvus import connections
from pymilvus import db
from pymilvus import CollectionSchema, FieldSchema, DataType, Collection
from pymilvus import utility
from transformers import AutoTokenizer, DPRContextEncoder


class Vector_DB:
    def __init__(self) -> None:
        self.connection_id="vector_db_connection"
        self.collection_name="texts"
        self.db_name="vector_db"

        print(f'establishing the connection...')
        self.connect()
        print(f'initialiing the embedding...')
        # Load the tokenizer
        self.tokenizer = AutoTokenizer.from_pretrained("facebook/dpr-ctx_encoder-single-nq-base")
        # Load the DPR Context Encoder model
        self.embedding_model = DPRContextEncoder.from_pretrained("facebook/dpr-ctx_encoder-single-nq-base")

    def text_embedding(self, text):
        input_tokens = self.tokenizer(text, return_tensors="pt")
        embeddings = self.embedding_model(**input_tokens).pooler_output
        return embeddings[0].tolist()

    def connect(self, ):       
        connections.add_connection(
            #Specify a name for the connection
            vector_db_connection={    
                "host": "localhost",
                "port": "19530",
                "username" : "",
                "password" : ""
            })
        #Connect
        connections.connect(self.connection_id)

    def create_db(self, ):
        current_dbs=db.list_database(using=self.connection_id)
        print("Current databases: ", current_dbs)
        
        if (self.db_name not in current_dbs):
            print("Creating database :", self.db_name)
            wiki_db = db.create_database(self.db_name, using=self.connection_id) 
        #Switch to use the new database
        db.using_database(self.db_name, using=self.connection_id)

    def create_collection(self, ):
        text_id = FieldSchema(
        name="text_id",
        dtype=DataType.INT64,
        auto_id=True,
        is_primary=True,
        max_length=32)

        #Define fields
        text = FieldSchema(
            name="text",
            dtype=DataType.VARCHAR,
            max_length=4086)

        #Dim should match the embedding size 
        text_embedding = FieldSchema(
            name="text_embedding",
            dtype=DataType.FLOAT_VECTOR,
            dim=768
        )

        #Define schema
        wiki_schema=CollectionSchema(
            fields=[text_id, text, text_embedding],
            description="texts_schema",
            enable_dynamic_field=True
        )
        #Creation collection
        wiki_collection=Collection(
            name=self.collection_name,
            schema=wiki_schema,
            using=self.connection_id,
            shard_num=1
        )
        #create index
        print('building index...')
        self.build_an_index()

    def insert_into_vector_db(self, text_list, text_embedding_list):
        #Format for data input
        insert_data=[text_list, text_embedding_list]
        #Initiate a collection object and insert data
        text_collection = Collection(self.collection_name,using=self.connection_id)
        #Insert
        mr=text_collection.insert(insert_data)
        print("Inserted data. Now flushing...")
        text_collection.flush(timeout=180)
        #build the index after insertion
        

    def build_an_index(self, ):
        #Build an index
        index_params = {
            "metric_type":"L2",
            "index_type":"IVF_FLAT",
            "params" :{"nlist":1024}
        }
        text_collection = Collection(self.collection_name,using=self.connection_id)
        text_collection.create_index(field_name="text_embedding", index_params=index_params)
        utility.index_building_progress(self.collection_name,using=self.connection_id)

    def preparing_db(self):
        print('create db...')
        self.create_db()
        print('create collection...')
        self.create_collection()

    def query(self, query_text):
        text_collection = Collection(self.collection_name,using=self.connection_id)
        text_collection.load()
        search_params = {
            "metric_type": "L2", 
            "offset": 0, 
            "ignore_growing": False, 
            "params": {"nprobe": 10}
        }
        search_embed=self.text_embedding(query_text)
        #Perform search
        s_results=text_collection.search(
            data=[search_embed], #input query to search for
            anns_field="text_embedding", #field to search with ANN
            param=search_params,
            limit=1, #Limit output
            expr=None, #Use additional scalar conditions
            output_fields=["text"],
            consistency_level="Strong"
        )
        print(s_results)
        # Assuming the closest result is in s_results[0]
        if s_results[0]:
            closest_match = s_results[0][0]  # First result of the first query
            # Convert L2 distance to similarity (example conversion)
            similarity = 1 / (1 + closest_match.distance)  # Conversion may vary based on your data

            if similarity >= 0.0:
                print(f"Closest match ID: {closest_match.id}, Text: {closest_match.entity.get('text')}, Similarity: {similarity}")
                return closest_match
            else:
                print("No match found with similarity >= 0.9")
                return None
        else:
            print("No results found.")
            return None

    def drop_collection(self, ):
        utility.drop_collection(self.collection_name,using=self.connection_id)