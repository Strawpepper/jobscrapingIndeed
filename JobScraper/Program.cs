using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace JobScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync("link"); //Insert Indeed URL here for jobsearch
            Console.ReadLine();
        }

        private static async void GetHtmlAsync(string siteUrl)
        {
            var url = siteUrl;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var companyNames = htmlDocument.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("company"))
                .ToList();

            var jobTitles = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("title"))
                .ToList();

            var dates = htmlDocument.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("date"))
                .ToList();

            var listOfJobs = companyNames.Select((x, i) => new { x, i })
                .Concat(jobTitles.Select((x, i) => new { x, i }))
                .Concat(dates.Select((x, i) => new { x, i }))
                .OrderBy(x => x.i)
                .Select(x => x.x)
                .ToList();

            var jobsPresentation = listOfJobs
                .Select(x => x.InnerText.Contains("dagar") ? $"{x.InnerText}\n\n**********************************" : x.InnerText)
                .ToList();

            foreach (var item in jobsPresentation)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
        }
    }
}
