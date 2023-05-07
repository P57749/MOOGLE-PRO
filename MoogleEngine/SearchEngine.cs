using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MoogleEngine;


    // Clase q representa el buscador 
    class SearchEngine
    {
        private readonly IDictionary<string, int> _documentFrequencies = new Dictionary<string, int>();
        private readonly IList<Document> _documents = new List<Document>();

        public SearchEngine(string directoryPath)
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

        // Calcular los scores para cada posible sugerencia 
        var suggestions = new Dictionary<string, double>();
        foreach (var result in results)
        {
            var words = result.Snippet.Split().Select(word => word.TrimEnd(',', '.', ';', ':')).Distinct();
            foreach (var word in words)
            {
                if (!queryWords.Contains(word))
                {
                    var score = _documentFrequencies.ContainsKey(word) ? _documentFrequencies[word] : 1;
                    suggestions[word] = suggestions.ContainsKey(word) ? suggestions[word] + score : score;
                }
            }
        }

        // Retorna la sugerencia con mayor score 
        return suggestions.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }
}
