namespace Uploader
{
    using CsvHelper;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Azure.Graphs;
    using Microsoft.Extensions.CommandLineUtils;
    using Newtonsoft.Json;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Threading.Tasks;

    public class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = "Sample movie data uploader";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            string endpoint = ConfigurationManager.AppSettings["Endpoint"];
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            app.Command("nuke", (command) =>
            {
                command.Description = "Nuke/Drop all the data in graphdb collection Movies";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    using (DocumentClient client = new DocumentClient(
                                new Uri(endpoint),
                                authKey,
                                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
                    {
                        Program prog = new Program();
                        prog.NukeCollection(client).GetAwaiter().GetResult();
                    }

                    return 0;
                });

            });

            app.Command("upload", (command) =>
            {
                command.Description = "Upload sample [data] to the graphdb database, options are movie, review";
                command.HelpOption("-?|-h|--help");

                var dataArgument = command.Argument("[data]", "What should be uploaded");

                command.OnExecute(() =>
                {
                    using (DocumentClient client = new DocumentClient(
                                new Uri(endpoint),
                                authKey,
                                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
                    {
                        Program prog = new Program();
                        switch (dataArgument.Value)
                        {                   
                            case "movie":                                                 
                                prog.UploadMovies(client).GetAwaiter().GetResult();
                                break;
                            case "user":
                                prog.UploadUsers(client).GetAwaiter().GetResult();
                                break;
                            case "review":
                                prog.UploadReviews(client).GetAwaiter().GetResult();
                                break;
                            default:
                                Console.WriteLine("Invalid/Null data option");
                                break;
                        }                
                    }
                    return 0;
                });
            });

            app.Execute(args);
        }

        private async Task UploadUsers(DocumentClient client)
        {
            try
            {
                Console.WriteLine("Uploading users");
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb" });

                DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri("graphdb"),
                    new DocumentCollection { Id = "Movies" },
                    new RequestOptions { OfferThroughput = 1000 });

                Console.WriteLine("Connected to graph Movies collection");

                Console.WriteLine("Reading users list");
                using (TextReader reader = new StreamReader("users.csv"))
                using (CsvReader csv = new CsvReader(reader))
                {
                    while (csv.Read())
                    {
                        string idField = "user" + csv.GetField<string>(0);

                        Console.WriteLine("Uploading user " + idField);

                        IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, $"g.addV('user').property('id', '{idField}')");
                        while (query.HasMoreResults)
                        {
                            await query.ExecuteNextAsync();
                        }
                    }
                }
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task UploadReviews(DocumentClient client)
        {
            try
            {
                Console.WriteLine("Uploading movie reviews");

                DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri("graphdb"),
                    new DocumentCollection { Id = "Movies" },
                    new RequestOptions { OfferThroughput = 1000 });

                Console.WriteLine("Connected to graph Movies collection");

                Console.WriteLine("Reading review list");
                using (TextReader reader = new StreamReader("ratings.csv"))
                using (CsvReader csv = new CsvReader(reader))
                {
                    while (csv.Read())
                    {
                        string userId = "user" + csv.GetField<string>(0);
                        string movieId = csv.GetField<string>(1);
                        float rating = csv.GetField<float>(2);

                        Console.WriteLine("Uploading review for user " + userId + " to " + movieId + " with rating "+ rating);
                        IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, $"g.V('user').has('id', '{userId}').addE('rates').property('weight', {rating}).to(g.V('movie').has('id', '{movieId}'))");
                        while (query.HasMoreResults)
                        {
                            var result = await query.ExecuteNextAsync();
                            foreach (var item in result)
                            {
                                Console.WriteLine(item);
                            }
                        }
                    }
                }
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task UploadMovies(DocumentClient client)
        {
            try
            {
                Console.WriteLine("Uploading movies");
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb" });

                DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri("graphdb"),
                    new DocumentCollection { Id = "Movies" },
                    new RequestOptions { OfferThroughput = 1000 });

                Console.WriteLine("Connected to graph Movies collection");

                Console.WriteLine("Reading movie list");
                using (TextReader reader = new StreamReader("movies.csv"))
                using (CsvReader csv = new CsvReader(reader))
                {
                    while (csv.Read())
                    {
                        string idField = csv.GetField<string>(0);
                        string titleField = csv.GetField<string>(1);
                        titleField = JsonConvert.ToString(titleField, '\"', StringEscapeHandling.EscapeHtml);

                        Console.WriteLine("Uploading " + titleField);

                        IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, $"g.addV('movie').property('id', '{idField}').property('title', {titleField})");
                        while(query.HasMoreResults)
                        {
                            await query.ExecuteNextAsync();
                        }
                    }
                }
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task NukeCollection(DocumentClient client)
        {
            try
            {
                Console.WriteLine("Nuking...");
                var response = await client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("graphdb", "Movies"));
                Console.WriteLine(response.StatusCode);
            }
            catch (DocumentClientException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
