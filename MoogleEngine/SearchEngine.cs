using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime;

namespace MoogleEngine;


    // Clase q representa el buscador 
    public class SearchEngine
    {
        public static char SeparadorDelSistema = Path.DirectorySeparatorChar;
        private readonly IDictionary<string, int> _documentFrequencies = new Dictionary<string, int>();
        private readonly IList<Document> _documents = new List<Document>();
        public static string directoryPath = //ruta de la carpeta con los documentos;
        

        public  SearchEngine()
        {
            //Cargar los documentos del directorio
            foreach (var filePath in Directory.EnumerateFiles(directoryPath))
            {
                
                var title = Path.GetFileNameWithoutExtension(filePath);
                var text = Document.RemoveDiacritics(File.ReadAllText(filePath));
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
        //Método para extraer las palabras únicas de todos los documentos 
        public static List<string> ExtractUniqueWords(string directoryPath)
        {
        // Array de las rutas del directorio
        string[] fileNames = Directory.GetFiles(directoryPath);

        // Concenter todo el contenido de los archivo en un solo string
        string allText = string.Join(" ", fileNames.Select(File.ReadAllText));

        // separa el string en palabras, ignorando los signos de puntuación y espacios
        string[] allWords = allText.Split(new char[] {' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '-', '(', ')', '[', ']', '{', '}', '<', '>', '|', '/', '\\', '\'', '\"'}, StringSplitOptions.RemoveEmptyEntries);

        // Crear un HashSet para almacenar las palabras únicas 
        HashSet<string> uniqueWords = new HashSet<string>(allWords, StringComparer.OrdinalIgnoreCase);

        

        return uniqueWords.ToList();
        }
        // Método para realizar una búsqueda y retornar una lista con los resultados de la búsqueda
    public IList<SearchItem> Search(string query)
    {
        // Eliminar los signos diacritics del query 
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
    //Método para procesar el query 
    public static List<string> Query(string query)
    {
        List<string> query1 = new();
        string palabra = "";
        //Con este for quitamos todo lo q no sea letra o número
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == ' ' && palabra == "") continue;
            if (Char.IsLetterOrDigit(query[j])) palabra += query[j];
            else if  (query[j] == ' ' || palabra != "") 
            {
            query1.Add(palabra); 
            palabra = "";
            }
        }
        //por si quedo una palabra en memoria
        if (palabra != "") query1.Add(palabra);
        return query1;
    }
    
    // Método para sugerir la mejor sugerencia basada en los resultados de la búsqueda
    public static (string, string []) Suggest(List<string> quer1, string query, List<string> uniqueWords)
    {
        //array para guardar los cálculos de levenshtein
        double[] cl = new double[uniqueWords.Count];
        //array para guardar las palabras que tengan mayor semejanza
        string[] sugerencias = new string[quer1.Count];
        //array para guardar las sugerencias con los operadores
        string[] sug_op = new string[sugerencias.Length];
        
        double distancia;
        for (int i = 0; i < quer1.Count; i++)
        {
            for (int j = 0; j < uniqueWords.Count; j++)
            {
                //comparando la palabra del query con la lista de todas las palabras de los documentos
                distancia = LevenshteinDistance.Compute(quer1[i], uniqueWords[j]);
                //almacenar el el array cl todas las distancias calculadas
                cl[j] = distancia;
            }
            //guardar las palabras que tengan menor distancia de levenshtein
            sugerencias[i] = uniqueWords[MenorDist(cl)];
        }
        //trabajar con la relación entre los operadores y la sugerencia
        for (int i = 0; i < sugerencias.Length; i++)
        {
            sug_op[i] = sugerencias[i];
        }
        string palabra = "";  int k = 0;
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == ' ' && palabra == "" || query[j] == '~') continue;
            if (Char.IsLetterOrDigit(query[j]) || query[j] == '*' || query[j] == '!' || query[j] == '~' || query[j] == '^')
            {
                palabra += query[j];
            }
            else if (query[j] == ' ' || palabra != "")
            {
                if (palabra[0] == '!') sug_op[k] = '!' + sugerencias[k];
                if (palabra[0] == '*') sug_op[k] = '*' + sugerencias[k];
                if (palabra[1] == '*') sug_op[k] = "**" + sugerencias[k];
                if (palabra[0] == '^') sug_op[k] = '^' + sugerencias[k];
                k++;  palabra = "";
            }
        }
        string sugerencia_op = "";
        for (int i = 0; i < sug_op.Length; i++)
        {
            sugerencia_op += sug_op[i] + " ";
        }
        return (sugerencia_op, sugerencias);
        }
    //Metodo para devolver la menor distancia
    private static int MenorDist(double[] cl)
    {
        double min = double.MaxValue;  int indice = 0;
        for (int i = 0; i < cl.Length; i++)
        {
            if (cl[i] < min)
            {
                min = cl[i];
                indice = i;
            }
        }
        return indice;
    }
}
// esto sirve para calcular la distancia de levenshtein
public static class LevenshteinDistance
{
    public static double Compute(string s, string t)
{
        int costo = 0;
        //matriz donde las filas representa la palabra introducida en la query y las columnas
        //las palabra del documento con la que se compara la palabra de la query 
        int[,] tabla = new int[s.Length + 1, t.Length + 1];
        //Llena la primera columna y la primera fila.
        for (int i = 0; i <= s.Length; tabla[i, 0] = i++) ;
        for (int h = 0; h <= t.Length; tabla[0, h] = h++) ;
        /// recorre la matriz llenando cada unos de los pesos.
        /// i filas, j columnas
        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                //si las letras son iguales en iguales posiciones entonces el costo es 0
                //si son diferentes el costo es 1
                costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                //eliminación, inserción y sustitución
                tabla[i, j] = Math.Min(Math.Min(tabla[i - 1, j] + 1, 
                tabla[i, j - 1] + 1),                                     
                tabla[i - 1, j - 1] + costo);                                  
            }
        }
        //porcentaje de cambios en la palabra
        if (s.Length > t.Length) return ((double)tabla[s.Length, t.Length] / (double) s.Length);
        else return ((double)tabla[s.Length, t.Length] / (double)t.Length);
    }
}

