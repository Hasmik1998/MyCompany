using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;

namespace Chess_Up.Models
{
    public class ChessResultsFetcher
    {
        public static List<List<string>> FetchChessResults()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://chess-results.com/fed.aspx?lan=1&fed=ARM");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

            string responseText;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }
            }

            // Extract the content of the table element
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(responseText);

            var rows = doc.DocumentNode.Descendants("table")
                .Where(table => table.Attributes["class"]?.Value == "CRs2")
                .SelectMany(table => table.Descendants("tr"));

            var results = new List<List<string>>();

            foreach (var row in rows)
            {
                var cells = row.Descendants("td").Select(td => td.InnerText.Trim()).ToList();
                results.Add(cells);
            }

            return results;
        }

    }
}

