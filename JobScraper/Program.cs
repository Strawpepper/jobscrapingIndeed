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
            string role = RoleCall();
            string location = Location();
            
            string indeedLink = $"https://se.indeed.com/jobb?q={role}&l={location}&ts=1586360992832&pts=1586157588027&rq=1&rsIdx=0&fromage=last&newcount=11";

            Console.WriteLine($"Link to results:\n{indeedLink}\n\n**********************************");

            GetAsyncHtml(indeedLink);
            Console.ReadLine();

        }

        private static async void GetAsyncHtml(string siteUrl)
        {
            var url = siteUrl;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            GenerateJobsIndeed(htmlDocument);
        }

        private static void GenerateJobsIndeed(HtmlDocument htmlDocument)
        {
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
                .Select(x => x.InnerText.Contains("dagar") || x.InnerText.Contains("dag") || x.InnerText.Contains("Nyligen") ? $"{x.InnerText}\n\n**********************************" : x.InnerText)
                .ToList();

            foreach (var job in jobsPresentation)
            {
                Console.WriteLine(job);
            }

            Console.WriteLine();
        }

        private static string RoleCall()
        {
            Console.Write("Role: ");
            string role = Console.ReadLine();

            if (role.Contains(" "))
                 role = role.Replace(' ', '+'); //If there is more than one word to the role, Indeed separetes the space with '+'

            return role;
        }

        private static string Location()
        {
            Console.Write("Location: ");
            string loc = Console.ReadLine();

            Console.Clear();

            return loc;
        }
    }
}
