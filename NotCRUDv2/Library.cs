using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Library
{
    private List<Document> documents;
    private MongoDBService mongoDBService;

    public Library(string connectionString = "mongodb://localhost:27017", string databaseName = "NotCRUD")
    {
        documents = new List<Document>();
        mongoDBService = new MongoDBService(connectionString, databaseName);

        // Load data from MongoDB
        RefreshFromDatabase();
    }

    private void RefreshFromDatabase()
    {
        try
        {
            var documents = Task.Run(async () => await mongoDBService.GetAllOuvragesAsync()).Result;
            this.documents.Clear();

            foreach (var document in documents)
            {
                this.documents.Add(mongoDBService.ConvertDocumentToOuvrage(document));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing data from MongoDB: {ex.Message}");
        }
    }

    public void AjouterOuvrage(Document ouvrage)
    {
        try
        {
            var id = Task.Run(async () => await mongoDBService.InsertOuvrageAsync(ouvrage)).Result;
            documents.Add(ouvrage);
            Console.WriteLine($"Ouvrage ajouté: {ouvrage.Titre}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout de l'ouvrage: {ex.Message}");
        }
    }
}