using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MoogleEngine;


    // class para representar un documento
    class Document
    {
        public string Title { get; }
        public string Text { get; }

        public Document(string title, string text)
        {
            Title = title;
            Text = text;
        }

        // método para calcular el TF-IDF score de una palabra en los documentos
        public double CalculateTfIdf(string word, IDictionary<string, int> documentFrequencies, int numDocuments)
        {
            var tf = (double)CountOccurrences(Text, word) / Text.Split().Length;
            double idf = Math.Log((double)numDocuments / documentFrequencies[word]);
            return tf * idf;
        }

        // Método para generar un snippet de los documentos q contienen el query
        public string GenerateSnippet(string query)
        {
            var regex = new Regex($"\\b({query})\\b", RegexOptions.IgnoreCase);
            var match = regex.Match(Text);
            if (match.Success)
            {
                var start = Math.Max(0, match.Index - 50);
                var end = Math.Min(Text.Length, match.Index + 50);
                return Text.Substring(start, end - start);
            }
            else
            {
                return Text.Substring(0, Math.Min(Text.Length, 100));
            }
        }

        // Helper method for counting the number of occurrences of a word in a string
        private int CountOccurrences(string text, string word)
        {
            return new Regex(Regex.Escape(word), RegexOptions.IgnoreCase).Matches(text).Count;
        }

        // metodo para eliminar todos los signos diacriticos de un string
        public static string RemoveDiacritics(string text)
        {
            string texto = text.ToLower();
            return texto;
        }
    }
