using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jutrera.articles.parser
{
    /// <summary>
    /// Represents a token in a document
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Document in wich a token apperas
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        [BsonElement("Document")]
        public string Document { get; set; }

        /// <summary>
        /// Token of a document
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        [BsonElement("Token")]
        public string Word { get; set; }

        /// <summary>
        /// Number of times the token appears in the document
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        [BsonRequired]
        [BsonElement("Count")]
        public int Count { get; set; }

        /// <summary>
        /// NUmber of times the most seen token of the document appears
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        [BsonRequired]
        [BsonElement("Max")]
        public int Max { get; set; }

        /// <summary>
        /// Normalized frequency of the token in document (Count / Max)
        /// </summary>
        [BsonRepresentation(BsonType.Double)]
        [BsonRequired]
        [BsonElement("TF")]
        public double TF { get; set; }

        /// <summary>
        /// Total number of documents in the collection
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        [BsonRequired]
        [BsonElement("DN")]
        public int DN { get; set; }

        /// <summary>
        /// Number of documents where the token is
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        [BsonRequired]
        [BsonElement("CN")]
        public int CN { get; set; }

        /// <summary>
        /// IDF [Log(DN/(1+CN))]
        /// </summary>
        [BsonRepresentation(BsonType.Double)]
        [BsonRequired]
        [BsonElement("IDF")]
        public double IDF { get; set; }

        /// <summary>
        /// TF-IDF value of the token in document [TF*IDF]
        /// </summary>
        [BsonRepresentation(BsonType.Double)]
        [BsonRequired]
        [BsonElement("TFIDF")]
        public double TFIDF { get; set; }
    }
}
