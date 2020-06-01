using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbPractice
{
    public class PersonRepo
    {

        MongoContext MongoContext { get; set; }
        IMongoCollection<Person> personCollection { get; set; }

        public PersonRepo(MongoContext mongoContext)
        {
            MongoContext = mongoContext;
            personCollection = MongoContext.Database.GetCollection<Person>("Persons");
        }

        public void Insert(Person person)
        {
            personCollection.InsertOne(person);
        }


    }
}
