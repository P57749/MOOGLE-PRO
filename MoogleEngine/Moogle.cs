namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query)
    {
        // inicializar el directorio con los documentos
        var searchEngine = new SearchEngine(@"/Applications/XAMPP/xamppfiles/htdocs/moogle/candela");
        
        var results = searchEngine.Search(query);

        if (results.Count == 0 )
        {
            string Suggestion = query;
            SearchItem[] items = new SearchItem[results.Count];
            return new SearchResult(items, Suggestion);
        
        }
        else
        {
        string Suggestion = searchEngine.Suggest(query, results);
        SearchItem[] items = new SearchItem[results.Count];
        
            for (int i = 0; i < results.Count; i++)
            {
            items[i] = results[i];
            }
        return new SearchResult(items, Suggestion);
        
        }

        /*SearchItem[] items = new SearchItem[results.Count];
        
            for (int i = 0; i < results.Count; i++)
            {
            items[i] = results[i];
            }
        return new SearchResult(items, Suggestion);*/

        
    }
}


// Suggest best query match based on search results
//            var suggestion = searchEngine.Suggest(query, results);
//            Console.WriteLine($"Did you mean: {suggestion}?");