namespace MoogleEngine;


public static class Moogle
{
    public static SearchEngine searchEngine;


    public static SearchResult Query(string query)
    {
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
    }
    
    public static void Iniciar()
    {
        searchEngine = new SearchEngine();
    }
}