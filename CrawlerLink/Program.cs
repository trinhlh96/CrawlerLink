using HtmlAgilityPack;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerLink
{
    class Program
    {
        static void Main(string[] args)
        {
            StartCawler();
            Console.ReadLine();
        }
        private static async Task StartCawler()
        {
            var url = "https://vnexpress.net/suc-khoe/dinh-duong";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var articles = htmlDocument.DocumentNode.Descendants("article").ToList();
            foreach (var article in articles)
            {
                var link = article.Descendants("a").FirstOrDefault()?.ChildAttributes("href").FirstOrDefault().Value;
                if (link != null) {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    var connection = factory.CreateConnection();
                    var channel = connection.CreateModel();
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    
                    string message = link;
                       
                    var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "amqHelloQueue",
                                             basicProperties: properties,
                                             body: body);
                        Console.WriteLine(message);

                }

            }
        }
        
    }
}
