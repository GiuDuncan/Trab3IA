using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Trab3IA
{
    public class LetterPairSimilarity
    {
        private readonly string _strikeAMatchPath;
        private readonly bool _clickToContinue;
        private static Stopwatch _watch;

        public LetterPairSimilarity(string strikeAMatchPath, bool clickToContinue)
        {
            _strikeAMatchPath = strikeAMatchPath;
            _clickToContinue = clickToContinue;
        }

        /// <summary>
        /// writes a file with all strike a match words in allStringslist
        /// </summary>
        /// <param name="allStringslist"></param>
        public void StrikeAMatch(List<string> allStringslist)
        {
            var dict = new Dictionary<string, List<string>>();
            _watch = Stopwatch.StartNew();
            var wordsSeen = new List<string>();
            for (var i = 0; i < allStringslist.Count; i++)
            {
                var wordList = allStringslist[i].Split(' ').ToList();
                foreach (var word in wordList)
                {
                    if (wordsSeen.Contains(word)) continue; //se a palavra já foi vista não precisa comparar
                    var wordInQuestionList = new List<string>();
                    for (var j = i; j < allStringslist.Count; j++)
                    {
                        var wordList2 = allStringslist[j].Split(' ').ToList();
                        foreach (var word2 in wordList2)
                        {
                            if (wordInQuestionList.Contains(word2) || wordsSeen.Contains(word2)) continue;
                            if (word.Length > 2 && word2.Length > 2 && Math.Abs(word.Length - word2.Length) <= 3)
                            {
                                var result = CompareStrings(word, word2);
                                if (result > 0.91 && result < 1)
                                {
                                    Console.WriteLine(word + " " + word2 + "\n");
                                    var shorterWord = word.Length < word2.Length ? word : word2;
                                    var otherword = word.Length < word2.Length ? word2 : word;
                                    if (!dict.ContainsKey(shorterWord) && !dict.ContainsKey(otherword))
                                    {
                                        dict.Add(shorterWord, new List<string>());
                                        dict[shorterWord].Add(otherword);
                                        Console.WriteLine(" {0} added {1} \n", shorterWord, otherword);
                                    }
                                    else
                                    {
                                        if (!dict.ContainsKey(otherword))
                                        {
                                            dict[shorterWord].Add(otherword);
                                            Console.WriteLine(" {0} added {1} \n", shorterWord, otherword);
                                        }
                                        else
                                        {
                                            dict[otherword].Add(shorterWord);
                                            Console.WriteLine(" {0} added {1}  \n", otherword, shorterWord);
                                        }
                                    }
                                }
                            }
                            wordInQuestionList.Add(word2);
                        }

                    }
                    wordsSeen.Add(word);
                }
            }
            Console.WriteLine("Tempo strikeamatch: " + _watch.Elapsed + "\n Aperte qualquer tecla..\n");
            if(_clickToContinue)
                Console.ReadKey();
            var writelist = dict.Select(pair => pair.Key + " " + string.Join(" ", pair.Value)).ToList();
            writelist = writelist.OrderByDescending(x => x.Length).ToList();
            File.WriteAllLines(_strikeAMatchPath, writelist);
        }

        public Dictionary<string, List<string>> GetDictionaryFromStrikeAMatchFile()
        {
            var dict = new Dictionary<string, List<string>>();
            var result = File.ReadAllLines(_strikeAMatchPath);
            foreach (var str in result)
            {
                List<string> wordList = str.Split(' ').ToList();
                var first = wordList.First();
                dict.Add(first, new List<string>());
                wordList.Remove(first);
                dict[first].AddRange(wordList);
            }

            return dict;
        }


        /** @return lexical similarity value in the range [0,1] */

        public static double CompareStrings(string str1, string str2)
        {
            var pairs1 = LetterPairs(str1.ToLower());
            var pairs2 = LetterPairs(str2.ToLower());
            var intersection = 0;
            var union = pairs1.Length + pairs2.Length;


            foreach (var pair1 in pairs1)
            {
                for (var j = 0; j < pairs2.Length; j++)
                {
                    var pair2 = pairs2[j];
                    if (!pair1.Equals(pair2)) continue;
                    intersection++;
                    pairs2[j] = "";
                    break;
                }
            }
            return (2.0*intersection)/union;
        }

        private static ArrayList WordLetterPairs(string str)
        {
            var allPairs = new ArrayList();
            // Tokenize the string and put the tokens/words into an array
            var words = str.Split(' ');
            // For each word
            foreach (var w in words)
            {
                // Find the pairs of characters
                var pairsInWord = LetterPairs(w);
                foreach (var p in pairsInWord)
                    allPairs.Add(p);
            }
            return allPairs;
        }

        private static string[] LetterPairs(string str)
        {
            var numPairs = str.Length - 1;
            var pairs = new string[numPairs];
            for (var i = 0; i < numPairs; i++)
                pairs[i] = str.Substring(i, 2);
            return pairs;
        }

    }
}