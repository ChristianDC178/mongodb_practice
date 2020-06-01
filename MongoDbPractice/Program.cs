using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbPractice
{

    public class MongoContext
    {
        MongoClient _client;
        IMongoDatabase _dataBase;

        public MongoContext(string connString, string dbName)
        {
            _client = new MongoClient(connString);
            _dataBase = _client.GetDatabase(dbName);
        }

        public MongoClient Client
        {
            get { return _client; }
        }

        public IMongoDatabase Database
        {
            get { return _dataBase; }
        }

    }


    class Program
    {

        static MongoContext mongoContext = null;

        static void Main(string[] args)
        {
            try
            {

                mongoContext = new MongoContext("mongodb://localhost:27017", "ChrisDC");

                Console.WriteLine("Select an option:");
                Console.WriteLine("1) InsertAndQuery");
                Console.WriteLine("2) QueryByFields");
                Console.WriteLine("3) Delete Documents");
                Console.WriteLine("4) Udpate Document");
                Console.WriteLine("5) ListDatabases");
                Console.WriteLine("6) CreateIndex");
                Console.WriteLine("7) QueryNestedData");
                Console.WriteLine("8) QueryNestedDataTwoLevels");
                Console.WriteLine("9) CreateCollection");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1": InsertAndQuery(); break;
                    case "2": QueryByFields(); break;
                    case "3": DeleteDocument(); break;
                    case "4": UpdateDocument(); break;
                    case "5": ListDatabases(); break;
                    case "6": CreateIndex(); break;
                    case "7": QueryNestedData(); break;
                    case "8": QueryNestedDataTwoLevels(); break;
                    case "9":
                        Console.WriteLine("Type the collection name: ");
                        string name = Console.ReadLine();
                        CreateCollection(name);
                        break;
                    case "10":
                        PersonRepo personRepo = new PersonRepo(mongoContext);
                        personRepo.Insert(new Person()
                        {
                            BirthDate = DateTime.Now,
                            Name = "Christian",
                            Lastname = "DC"
                        });
                        Console.WriteLine("Person inserted, check the collection content");
                        Console.ReadKey();
                        break;

                    default: break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void InsertAndQuery()
        {

            var documents = new BsonDocument[]
                {
                            new BsonDocument
                            {
                                { "item", "journal" },
                                { "qty", 25 },
                                { "size", new BsonDocument { { "h", 14 }, { "w", 21 }, {  "uom", "cm"} } },
                                { "status", "A" }
                            },
                            new BsonDocument
                            {
                                { "item", "notebook" },
                                { "qty", 50 },
                                { "size", new BsonDocument { { "h",  8.5 }, { "w", 11 }, {  "uom", "in"} } },
                                { "status", "A" }
                            },
                            new BsonDocument
                            {
                                { "item", "paper" },
                                { "qty", 100 },
                                { "size", new BsonDocument { { "h",  8.5 }, { "w", 11 }, {  "uom", "in"} } },
                                { "status", "D" }
                            },
                            new BsonDocument
                            {
                                { "item", "planner" },
                                { "qty", 75 },
                                { "size", new BsonDocument { { "h", 22.85 }, { "w", 30  }, {  "uom", "cm"} } },
                                { "status", "D" }
                            },
                            new BsonDocument
                            {
                                { "item", "postcard" },
                                { "qty", 45 },
                                { "size", new BsonDocument { { "h", 10 }, { "w", 15.25 }, {  "uom", "cm"} } },
                                { "status", "A" }
                            },
                };



            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            collection.InsertMany(documents);

            var filter = Builders<BsonDocument>.Filter.Empty;
            var result = collection.Find(filter).ToList();

            foreach (var item in result)
            {
                var element = item.GetElement("item");
                Console.WriteLine(element.Value);
            }

            Console.ReadKey();

        }

        static void QueryByFields()
        {

            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            var filter1 = Builders<BsonDocument>.Filter.Eq("item", "journal");
            var filter2 = Builders<BsonDocument>.Filter.Eq("item", "notebook");
            var filter = Builders<BsonDocument>.Filter.Or(filter1, filter2);

            var result = collection.Find(filter).ToList();

            foreach (var item in result)
            {
                var element = item.GetElement("item");

                Console.WriteLine($"{element.Name} {element.Value}");
            }

            Console.ReadKey();

        }

        static void QueryNestedData()
        {
            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            var filter = Builders<BsonDocument>.Filter.Eq("size", new BsonDocument { { "h", 22.85 }, { "w", 30 }, { "uom", "cm" } });
            var result = collection.Find(filter).ToList();

            foreach (var item in result)
            {
                var element = item.GetElement("item");

                Console.WriteLine($"{element.Name} {element.Value}");
            }

            Console.ReadKey();

        }

        static void QueryNestedDataTwoLevels()
        {
            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            var filter = Builders<BsonDocument>.Filter.Eq("description.family.name", "Electronic");
            var result = collection.Find(filter).ToList();

            foreach (var item in result)
            {
                var element = item.GetElement("item");

                Console.WriteLine($"{element.Name} {element.Value}");
            }

            Console.ReadKey();
        }

        static void DeleteDocument()
        {
            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            var filter = Builders<BsonDocument>.Filter.Eq("item", "journal");
            DeleteResult result = collection.DeleteMany(filter);

            if (result.IsAcknowledged)
            {
                Console.WriteLine($"Deleted: {result.DeletedCount}");
            }
            else
            {
                Console.WriteLine($"Nothing deleted");
            }

            Console.ReadKey();
        }

        static void UpdateDocument()
        {
            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");


            var filter = Builders<BsonDocument>.Filter.Eq("item", "notebook");

            var update = Builders<BsonDocument>.Update.Set("size.uom", "cm")
                                                .Set("status", "P")
                                                .Set("price", 1000)
                                                .CurrentDate("lastModified");
            var result = collection.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                var itemResult = collection.Find(filter).FirstOrDefault();

                Console.WriteLine($"The price is {itemResult.GetElement("price")} and the status {itemResult.GetElement("status")}");
            }

            Console.ReadKey();
        }

        static void CreateIndex()
        {
            var collection = mongoContext.Database.GetCollection<BsonDocument>("MongoDomain");

            var indexKeys = Builders<BsonDocument>.IndexKeys;
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys.Ascending("item"));

            var result = collection.Indexes.CreateOne(indexModel);

            Console.WriteLine(result);
            Console.ReadKey();

        }

        static void ListDatabases()
        {

            var token = new System.Threading.CancellationToken();

            var dbs = mongoContext.Client.ListDatabases(token);

            while (dbs.MoveNext())
            {
                dbs.Current.ToList().ForEach(db =>
                {
                    Console.WriteLine($"DB Name: {db.ToJson()}");
                });
            }

            Console.ReadKey();

        }

        static void CreateCollection(string name)
        {
            mongoContext.Database.CreateCollection(name, new CreateCollectionOptions() { Capped = false });

            Console.WriteLine($"Collection {name} created");
            Console.ReadKey();
        }

    }


}
