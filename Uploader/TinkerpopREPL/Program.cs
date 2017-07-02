using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using System;
using System.Configuration;

namespace TinkerpopREPL
{
    class Program
    {
        static void Main(string[] args)
        {
            string endpoint = ConfigurationManager.AppSettings["Endpoint"];
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            using (DocumentClient client = new DocumentClient(
                                new Uri(endpoint),
                                authKey,
                                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
            {

                DocumentCollection graph = client.CreateDocumentCollectionIfNotExistsAsync(
                        UriFactory.CreateDatabaseUri("graphdb"),
                        new DocumentCollection { Id = "Movies" },
                        new RequestOptions { OfferThroughput = 1000 }).GetAwaiter().GetResult();

                while (true)
                {
                    var command = Console.ReadLine();
                    if (command == "exit")
                        break;

                    try
                    {
                        IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, command);
                        while (query.HasMoreResults)
                        {
                            foreach (var result in query.ExecuteNextAsync().GetAwaiter().GetResult())
                            {
                                Console.WriteLine(JsonConvert.SerializeObject(result));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
