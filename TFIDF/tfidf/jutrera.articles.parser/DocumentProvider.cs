using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace jutrera.articles.parser
{
    public class DocumentProvider
    {
        private MongoClient Cliente { get; set; }
        private MongoDatabase Db { get; set; }
        public MongoCollection Coleccion
        {
            get { return Db.GetCollection("Documents"); }
        }
        /// <summary>
        /// Propiedad que identifica a la colección
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public DocumentProvider()
        {
            Cliente = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
            Db = Cliente.GetServer().GetDatabase("tfidf");
        }

        public List<Document> GetAll()
        {
            return Coleccion.FindAllAs<Document>().ToList<Document>();
        }
    }
}
