using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MoogleEngine;


    // Clase q representa el buscador 
    public class SearchEngine
    {
        private readonly IDictionary<string, int> _documentFrequencies = new Dictionary<string, int>();
        private readonly IList<Document> _documents = new List<Document>();
        private string directoryPath = @"/Applications/XAMPP/xamppfiles/htdocs/moogle/candela";

        public SearchEngine()
        {
            //Cargar los documentos del directorio
            foreach (var filePath in Directory.EnumerateFiles(directoryPath))
            {
                var text = Document.RemoveDiacritics(File.ReadAllText(filePath));
                var title = Path.GetFileNameWithoutExtension(filePath);
                _documents.Add(new Document(title, text));

                // Actualizar la frecuencia del documento para cada palabra 
                foreach (var word in text.Split().Distinct())
                {
                    if (!_documentFrequencies.ContainsKey(word))
                    {
                        _documentFrequencies[word] = 0;
                    }
                    _documentFrequencies[word]++;
                }
            }
        }
        // Método para realizar una búsqueda y retornar una lista con los resultados de la búsqueda
    public IList<SearchItem> Search(string query)
    {
        // Eliminar los signos diacriticos del query 
        query = Document.RemoveDiacritics(query);

        // Calcular TF-IDF score para cada documento y su palabra con el query 
        var numDocuments = _documents.Count;
        var queryWords = query.Split();
        var scores = new Dictionary<Document, double>();
        foreach (var document in _documents)
        {
            var score = 0.0;
            foreach (var word in queryWords)
            {
                score += document.CalculateTfIdf(word, _documentFrequencies, numDocuments);
            }
            scores[document] = score;
        }

        // Almacenar los documentos por score y generar el  resultado de la búsqueda
        var results = new List<SearchItem>();
        foreach (var kvp in scores.OrderByDescending(kvp => kvp.Value))
        {
            if (kvp.Value != 0)
            {
            var document = kvp.Key;
            var score = kvp.Value;
            var snippet = document.GenerateSnippet(query);
            results.Add(new SearchItem(document.Title, snippet, (float)score));
            }

        }
        return results;
    }

    // Método para sugerir la mejor sugerencia basada en los resultados de la búsqueda
    public string Suggest(string query, IList<SearchItem> results)
    {
        // crear una lista con todas las palabras del query 
        var queryWords = new HashSet<string>(query.Split());

    // Calcular la distacnia de levenshtein para cada posible sugerencia
    var suggestions = new Dictionary<string, int>();
    foreach (var result in results)
    {
        var words = result.Snippet.Split().Select(word => word.TrimEnd(',', '.', ';', ':')).Distinct();
        foreach (var word in words)
        {
            if (!queryWords.Contains(word))
            {
                var distance = LevenshteinDistance.Compute(query, word);
                suggestions[word] = distance;
            }
        }
    }

    // retornar la sugerencia con la menor distancia
    return suggestions.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
        
    }
}

public static class LevenshteinDistance
{
    public static int Compute(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.IsNullOrEmpty(t) ? 0 : t.Length;
        }

        if (string.IsNullOrEmpty(t))
        {
            return s.Length;
        }

        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        for (var i = 0; i <= n; i++)
        {
            d[i, 0] = i;
        }

        for (var j = 0; j <= m; j++)
        {
            d[0, j] = j;
        }

        for (var j = 1; j <= m; j++)
        {
            for (var i = 1; i <= n; i++)
            {
                var cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(
                    d[i - 1, j] + 1,
                    d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
}
