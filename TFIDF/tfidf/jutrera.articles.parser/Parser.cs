using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jutrera.articles.parser
{
    public class Parser
    {
        public List<Document> Documents { get; set; }
        public List<Token> Tokens { get; set; }

        public Parser()
        {
            this.Initialize();
        }

        /// <summary>
        /// read all the documents from database
        /// </summary>
        private void Initialize()
        {
            DocumentProvider documentProvider = new DocumentProvider();
            this.Documents = documentProvider.GetAll();
        }

        internal void Execute()
        {
            List<Token> tokenList = new List<Token>();

            //Insert the tokens of documents anr count
            foreach (Document doc in this.Documents)
            {
                foreach (Section secc in doc.Sections)
                {
                    List<string> tokens = Generics.ExtractTokens(secc.Text);

                    foreach (string word in tokens)
                    {
                        if (tokenList.Any(x => x.Document == doc.Title && x.Word == word))
                        {
                            tokenList.First(x => x.Document == doc.Title && x.Word == word).Count++;
                        }
                        else
                        {
                            tokenList.Add(new Token()
                            {
                                Document = doc.Title,
                                Word = word,
                                Count = 1
                            });
                        }
                    }
                }
            }

            //Save the other properties
            foreach (Token item in tokenList)
            {
                if (item.Max == 0)
                {
                    item.Max = (from elto in tokenList
                                   where elto.Document == item.Document
                                   select elto.Count).Max();
                    item.TF = Convert.ToDouble(item.Count) / Convert.ToDouble(item.Max);
                }
                item.DN = tokenList.GroupBy(x => x.Document).Count();
                item.CN = tokenList.Where(x => x.Word == item.Word).Count();
                item.IDF = Convert.ToDouble(Math.Log10(item.DN / (item.CN)));
                item.TFIDF = item.TF * item.IDF;
            }

            //Save in database
            TokenProvider tokenProvider = new TokenProvider();
            tokenList.ForEach(item => tokenProvider.Insert(item));
        }
    }
}
