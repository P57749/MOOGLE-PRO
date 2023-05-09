using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime;

namespace MoogleEngine;


public static class Moogle
{
    public static SearchEngine searchEngine;
    public static char SeparadorDelSistema = Path.DirectorySeparatorChar;
    public static string directoryPath = //ruta de la carpeta con los documentos;

    public static SearchResult Query(string query)
    {
        var results = searchEngine.Search(query);
        List<string> quer1 = SearchEngine.Query(query);
        List<string> uniqueWords = SearchEngine.ExtractUniqueWords(directoryPath);
        string Suggestion = SearchEngine.Suggest(quer1, query, uniqueWords).Item1;
        
        SearchItem[] items = new SearchItem[results.Count];
        for (int i = 0; i < results.Count; i ++)
        {
            items[i] = results[i];
        }

        int contador = 0;
        for (int i = 0; i < quer1.Count; i++)
        {
            if(uniqueWords.Contains(quer1[i]))
            contador ++;
        }
        //
        if (contador != quer1.Count){

            return new SearchResult(items, Suggestion);
        
        }
        else
        {
        
        Suggestion = "";
        return new SearchResult (items, Suggestion);
        }
    }
    
    public static void Iniciar()
    {
        searchEngine = new SearchEngine();
    }
}