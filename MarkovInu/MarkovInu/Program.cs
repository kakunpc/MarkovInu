using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MarkovInu.Markov;
using NMeCab;

namespace MarkovInu
{
    internal class Program
    {
        private static readonly MeCabParam MeCabParam = new MeCabParam
        {
            DicDir = @"dic\ipadic"
        };

        private static MeCabTagger _meCabTagger;

        private static void Main()
        {
            _meCabTagger = MeCabTagger.Create(MeCabParam);
            var markovDic = new MarkovDictionary();

            var reg = new Regex(@"<.*?>");
            
            using (var file = new System.IO.StreamReader(@"..\..\..\..\Sample.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    line = reg.Replace(line, "");
                    var marList = CheckMeCab(line);

                    markovDic.AddSentence(marList);
                }
            }

            var results = new List<string>();

            for (var i = 0; i < 100;)
            {
                var text = string.Join("", markovDic.BuildSentence());
                if (text.Length > 80) continue;

                results.Add(text);
                ++i;
            }

            var r = results.FindMax(c => c.Length);
            Console.WriteLine(r);

            Console.ReadLine();
        }

        private static string[] CheckMeCab(string sentence)
        {
            var node = _meCabTagger.ParseToNode(sentence);
            var resultList = new List<string>();
            while (node != null)
            {
                if (node.CharType > 0)
                {
                    resultList.Add(node.Surface);
                }
                node = node.Next;
            }

            return resultList.ToArray();
        }
    }
}
