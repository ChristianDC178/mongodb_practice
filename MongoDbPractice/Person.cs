using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDbPractice
{
    public class Person
    {


        [BsonElement("name") ]
        public string Name { get; set; }

        [BsonElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [BsonElement("lastName")]
        public string Lastname { get; set; }

        [BsonIgnore]
        public string Fullname { get { return $"{Lastname} {Name}"; } }

    }
}
