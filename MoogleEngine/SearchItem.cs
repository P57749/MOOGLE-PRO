namespace MoogleEngine;

public class SearchItem
{
    public SearchItem(string title, string snippet, float score)
    {
        this.Title = title;

        this.Snippet = snippet;

        this.Score = score;
    }

    public string Title { get; private set; }

    //fragmento 
    public string Snippet { get; private set; }

    public float Score { get; private set; }

}
