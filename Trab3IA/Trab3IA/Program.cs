using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Trab3IA
{
    class Program
    {
        private static readonly string Diretorio =
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\.."));

        private static readonly string FinalFilePath = Diretorio + @"\Data3000words.arff";
        private static readonly string StrikeAMatchPath = Diretorio + @"\strikeAMatch.txt";
        private static readonly string Diretorio1 = Diretorio + @"\movie_review_dataset\part1";
        private static readonly string Diretorio2 = Diretorio + @"\movie_review_dataset\part2";
        private const string Neg = @"\neg";
        private const string Pos = @"\pos";
        private const string Pattern = @"[^a-zA-Z ]|br(?= )";

        private static List<string> _allStringslist;
        private static string[] _stopwords;
        private static Dictionary<string, int> _dictionary;
        private static Dictionary<string, int> _sortedDict;
        private static Regex _regex;
        private static Stopwatch _watch;
        private static LetterPairSimilarity _letterPairSimilarity;
        private const int WordsDisplayed = 30;
        private const int ProcessedFiles = 25000; //all: 25.000
        private const int N = 3000;
        private static bool clickToContinue = false;

        private static void Main(string[] args)
        {

            var enum1Neg = Directory.EnumerateFiles(Diretorio1 + Neg, "*.txt");
            var enum1Pos = Directory.EnumerateFiles(Diretorio1 + Pos, "*.txt");
            var enum2Neg = Directory.EnumerateFiles(Diretorio2 + Neg, "*.txt");
            var enum2Pos = Directory.EnumerateFiles(Diretorio2 + Pos, "*.txt");

            Initialize();

            FillAllStringsList(enum1Neg, enum2Neg);

            FillAllStringsList(enum1Pos, enum2Pos);

            RemoveStopWords();

            /*muito lento
            //_letterPairSimilarity.StrikeAMatch(_allStringslist);// rodar só uma vez
            //ReplaceWordsFromStrikeAMatch();
            */

            CreateDictionary();
            DisplayMostAndLeastUsed("All files");

            //var list = GetNFirstWordsFromDictionary();
            var list = GetWordsFromWekaAttributeSelectionResult();
            GetAllStringListListWithMostUsedWords(list);

            WriteFinalFile();
        }

        private static void Initialize()
        {
            _allStringslist = new List<string>();
            _stopwords = new[]
            {
                "the", "and", "this", "his", "that", "for", "with", "film", "movie", "was", "are", "from", "one", "have",
                "they", "all", "you", "its", "some", "who", "has", "her", "their", "just", "about", "she", "would",
                "him", "there", "what", "but", "out", "when", "had", "story", "which", "only", "see", "time", "other",
                "can", "been", "were", "than", "how", "a", "of", "to", "is", "in", "i", "it", "br", "as", "on", "be",
                "at", "he", "so", "or", "by", "an", "if", "me", "my", "we", "do", "will", "films", "us"
            };
            _regex = new Regex(Pattern);
            _letterPairSimilarity = new LetterPairSimilarity(StrikeAMatchPath, clickToContinue);
        }

        #region Read from File

        private static void FillAllStringsList(IEnumerable<string> enum1, IEnumerable<string> enum2)
        {
            var count = 0;
            foreach (var enumFile in new[] {enum1, enum2})
                foreach (var file in enumFile)
                {
                    var frase = GetStringFromFile(file);
                    _allStringslist.Add(frase);
                    count ++;
                    if (count >= ProcessedFiles)
                        break;
                }
            Console.WriteLine("Leitura de arquivos feita\n");
        }

        private static string GetStringFromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            if (lines.Length > 1)
                Console.WriteLine("Deu ruim\n");
            for (var i = 0; i < lines.Length; i++)
            {
                var resultstr = _regex.Replace(lines[i], "");
                resultstr = NormalizeWhiteSpace(resultstr);
                lines[i] = resultstr;
            }
            return lines[0].ToLower();
        }

        public static string NormalizeWhiteSpace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var current = 0;
            char[] output = new char[input.Length];
            bool skipped = false;

            foreach (char c in input.ToCharArray())
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!skipped)
                    {
                        if (current > 0)
                            output[current++] = ' ';

                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    output[current++] = c;
                }
            }

            return new string(output, 0, current);
        }

        #endregion

        #region TFIDF

        private static void RemoveStopWords()
        {
            _watch = Stopwatch.StartNew();
            var newlist = new List<string>();
            foreach (var str in _allStringslist)
            {
                List<string> wordList = str.Split(' ').ToList();
                foreach (string word in _stopwords)
                    while (wordList.Contains(word))
                        wordList.Remove(word);
                newlist.Add(String.Join(" ", wordList));
            }
            _allStringslist = newlist;
            Console.WriteLine("Tempo remove stop words: " + _watch.Elapsed + "\n Aperte qualquer tecla..\n");
            if (clickToContinue)
                Console.ReadKey();
        }

        private static void CreateDictionary()
        {
            _dictionary = new Dictionary<string, int>();
            _watch = Stopwatch.StartNew();
            foreach (var str in _allStringslist)
            {
                List<string> wordList = str.Split(' ').ToList();
                foreach (var word in wordList)
                {
                    if (word.Length <= 1) continue; // ...check if the dictionary already has the word.
                    if (_dictionary.ContainsKey(word))
                        // If we already have the word in the dictionary, increment the count of how many times it appears
                        _dictionary[word]++;
                    else // Otherwise, if it's a new word then add it to the dictionary with an initial count of 1
                        _dictionary[word] = 1;
                }
            }
            Console.WriteLine("Tempo create dictionary: " + _watch.Elapsed + "\n Aperte qualquer tecla..\n");
            if (clickToContinue)
                Console.ReadKey();
        }

        #endregion

        #region StrikeAMatch

        private static void ReplaceWordsFromStrikeAMatch()
        {
            var dict = _letterPairSimilarity.GetDictionaryFromStrikeAMatchFile();
        }

        #endregion

        #region FinalPart

        private static List<string> GetNFirstWordsFromDictionary()
        {
            var newList = new List<string>();
            _sortedDict = new Dictionary<string, int>();
            _sortedDict =
                (from entry in _dictionary orderby entry.Value descending select entry).ToDictionary(pair => pair.Key,
                    pair => pair.Value);
            var count = 0;
            foreach (KeyValuePair<string, int> pair in _sortedDict)
            {
                newList.Add(pair.Key);
                count++;
                if (count >= N)
                    break;
            }
            return newList;
        }

        private static List<string> GetWordsFromWekaAttributeSelectionResult()
        {
            var lines = File.ReadAllLines(@"C:\Users\Giulia_Duncan\Dropbox\PUC\2016.1\Inteligencia Artificial (INF1771)\Trabalho 3\WekaResult.txt");
            return lines.ToList();
        }

        private static void GetAllStringListListWithMostUsedWords(List<string> listWithNMostUsedWords)
        {
            var newAllStringList = new List<string>();
            foreach (var str in _allStringslist)
            {
                var wordList = str.Split(' ').ToList();
                var tmp = listWithNMostUsedWords.Where(word => wordList.Contains(word)).ToList();
                newAllStringList.Add(string.Join(" ", tmp));
            }
            _allStringslist = newAllStringList;
        }

        private static void WriteFinalFile()
        {
            var allStringslist = new List<string>
            {
                "@relation Tudo\r\n @attribute frase string\r\n @attribute PouN {P,N}\r\n @data"
            };
            var count = 0;
            foreach (var str in _allStringslist)
            {
                var pouN = count++ < _allStringslist.Count/2 ? ", N" : ", P";
                allStringslist.Add("\" " + str + " \"" + pouN);
            }
            File.WriteAllLines(FinalFilePath, allStringslist);
        }

        #endregion

        #region Display

        private static void DisplayMostAndLeastUsed(string classific)
        {
            DisplayMostUsed(classific);
            DisplayLeastUsed(classific);
        }

        private static void DisplayMostUsed(string classific)
        {
            _sortedDict = new Dictionary<string, int>();
            _sortedDict =
                (from entry in _dictionary orderby entry.Value descending select entry).ToDictionary(pair => pair.Key,
                    pair => pair.Value);

            int count = 1;
            Console.WriteLine("---- Most Frequent Terms in: " + classific + " ----");
            Console.WriteLine();
            foreach (KeyValuePair<string, int> pair in _sortedDict)
            {
                Console.WriteLine(count + "\t" + pair.Key + "\t" + pair.Value);
                count++;
                if (count > WordsDisplayed)
                {
                    break;
                }
            }
            Console.WriteLine("Aperte qualquer tecla..");
            if(clickToContinue)
                Console.ReadKey();
        }

        private static void DisplayLeastUsed(string classific)
        {
            int count = 1;
            Console.WriteLine("---- Least Frequent Terms in: " + classific + " ----");
            Console.WriteLine();
            foreach (KeyValuePair<string, int> pair in _sortedDict)
            {
                count++;
                if (count > _sortedDict.Count - WordsDisplayed)
                {
                    Console.WriteLine(count + "\t" + pair.Key + "\t" + pair.Value);
                }
            }
            Console.WriteLine("Aperte qualquer tecla..");
            if(clickToContinue)
                Console.ReadKey();
        }

        #endregion

        #region Not used

        private static void AddToStopWordsEveryWordThatIsNotInNFirst()
        {
            var stopwordslist = new List<string>();

            // Create a dictionary sorted by value (i.e. how many times a word occurs)
            _sortedDict = new Dictionary<string, int>();
            _sortedDict =
                (from entry in _dictionary orderby entry.Value descending select entry).ToDictionary(pair => pair.Key,
                    pair => pair.Value);
            var count = 0;
            foreach (KeyValuePair<string, int> pair in _sortedDict)
            {
                count++;
                if (count > N)
                {
                    stopwordslist.Add(pair.Key);
                    count++;
                }
            }
            stopwordslist.AddRange(_stopwords);
            _stopwords = stopwordslist.ToArray();
            //_pattern = "\\b" + string.Join("\\b|\\b", _stopwords) + "\\b";
        }


        private static List<string> ApplyTfidf(List<string> list, bool display, string word)
        {
            _watch = Stopwatch.StartNew();
            _dictionary = new Dictionary<string, int>();
            var newlist = new List<string>();
            foreach (var str in list)
            {
                var newstr = Tfidf(str);
                newlist.Add(newstr);
            }
            _watch.Stop();
            Console.WriteLine("\nTempo para consulta: " + _watch.Elapsed + "\n Aperte qqr coisa para continuar.. \n");
            Console.ReadKey();

            if (!display) return newlist;
            DisplayMostUsed(word);
            DisplayLeastUsed(word);

            return newlist;
        }

        private static string Tfidf(string frase)
        {
            List<string> wordList = frase.Split(' ').ToList();

            //REPLACE
            //var fraseModi = _stopwords.Aggregate(frase, (current, word) => current.Replace(" " + word + " ", " "));
            //List<string> wordList = fraseModi.Split(' ').ToList();

            //REGEX
            //var fraseModific = Regex.Replace(frase, _pattern, "");
            //fraseModific = NormalizeWhiteSpace(fraseModific);
            //List<string> wordList = fraseModific.Split(' ').ToList();

            // Define and remove stopwords
            foreach (string word in _stopwords)
                // While there's still an instance of a stopword in the wordList, remove it.
                // If we don't use a while loop on this each call to Remove simply removes a single
                // instance of the stopword from our wordList, and we can't call Replace on the
                // entire string (as opposed to the individual words in the string) as it's
                // too indiscriminate (i.e. removing 'and' will turn words like 'bandage' into 'bdage'!)
                while (wordList.Contains(word))
                    wordList.Remove(word);

            // Loop over all over the words in our wordList...
            foreach (string word in wordList)
            {
                if (word.Length > 1)
                {
                    // ...check if the dictionary already has the word.
                    if (_dictionary.ContainsKey(word))
                        // If we already have the word in the dictionary, increment the count of how many times it appears
                        _dictionary[word]++;
                    else // Otherwise, if it's a new word then add it to the dictionary with an initial count of 1
                        _dictionary[word] = 1;
                } // End of word length check
            } // End of loop over each word in our input

            return String.Join(" ", wordList);
        }

        #endregion
    }
}
