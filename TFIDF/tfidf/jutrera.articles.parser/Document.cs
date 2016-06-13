using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jutrera.articles.parser
{
    public class Document
    {
        [BsonId]
        [BsonRequired]
        public string Title { get; set; }

        [BsonElement("Sections")]
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Text")]
        public string Text { get; set; }
    }
}
