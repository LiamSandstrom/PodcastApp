using DAL;

namespace UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        IRssRepository repo = new RssRepository();
        var feed = repo.GetFeed("https://api.sr.se/api/rss/pod/itunes/3966");

        string text = "";
        int i = 20;

        foreach (var item in feed.Items)
        {
            text += (item.Title) + "\n";
            i--;
            if (i == 0) break;
        }
        MessageBox.Show(text);

        //// To customize application configuration such as set high DPI settings or default font,
        //// see https://aka.ms/applicationconfiguration.
        //ApplicationConfiguration.Initialize();
        //Application.Run(new Form1());

    }
}