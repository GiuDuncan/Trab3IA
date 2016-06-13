using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jutrera.articles.parser
{
    public class TokenProvider
    {
        private MongoClient Cliente { get; set; }
        private MongoDatabase Db { get; set; }
        public MongoCollection Coleccion
        {
            get { return Db.GetCollection("Tokens"); }
        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public TokenProvider()
        {
            Cliente = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
            Db = Cliente.GetServer().GetDatabase("tfidf");
        }

        public void Insert(Token token)
        {
            Coleccion.Insert(token);
        }

        public List<Token> GetAll()
        {
            return Coleccion.FindAllAs<Token>().ToList<Token>();
        }
    }
}
