namespace MoogleEngine;


/*public static class Moogle
{
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda

        Candela searcher = new Candela();
        string directoryPath = @"/Applications/XAMPP/xamppfiles/htdocs/moogle/candela";
        searcher.Candela(directoryPath);

        Candela resultados =  new Candela();
        resultados.Search(query);

        foreach (Dictionary<string, int> result in resultados)
        {     
        //Console.WriteLine("Palabra '{0}' encontrada {1} veces en el archivo {2}", 
        //result.Key, result.Value, "documento.txt");
        }

        Dictionary<string, double> result = new Dictionary<string, double>();
        Dictionary<string, double> result = searcher.Search(query);
        
        if (result == 0)
        {
            return new SearchResult();
        }

        SearchItem[] items = new SearchItem[result.Count];
        
        foreach (string documentTitle in result.Keys)
        {
            
        }
        return new SearchResult(items, query);
    }
}


//Candela searcher = new Candela(@"/Applications/XAMPP/xamppfiles/htdocs/moogle/candela");
//Dictionary<string, double> results = searcher.Search(query);

foreach (string documentTitle in results.Keys)
{
    Console.WriteLine("{0}: {1}", searcher.GetTitle(documentTitle), searcher.GetSnippet(documentTitle));
}

string bestSuggestion = searcher.GetBestSuggestion(query);
if (bestSuggestion != null)
{
    Console.WriteLine("Did you mean: {0}", bestSuggestion);
}*/

public static class Moogle
{
    public static SearchResult Query(string query)
    {
        // Modifique este método para responder a la búsqueda

        // inicializar el directorio con los documentos
        var searchEngine = new SearchEngine(@"/Applications/XAMPP/xamppfiles/htdocs/moogle/candela");

        var results = searchEngine.Search(query);
        SearchItem[] items = new SearchItem[results.Count];
        



            
            //items[i] = new SearchEngine();
            
            /*var document =  result.Key;
            var snippet =  result.Value;
            /*Console.WriteLine($"Title: {result.Title}");
            Console.WriteLine($"Snippet: {result.Snippet}");
            Console.WriteLine($"Score: {result.Score}");
            Console.WriteLine();*/
            for (int i = 0; i < results.Count; i++)
            {
            items[i] = results[i];
        }
        




        return new SearchResult(items, query);
    }
}