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
        var documents = Task.Run(async () => await mongoDBService.GetAllOuvragesAsync()).Result;
        this.documents.Clear();

        foreach (var document in documents)
        {
            this.documents.Add(mongoDBService.ConvertDocumentToOuvrage(document));
        }
    }

    public void AjouterOuvrage(Document ouvrage)
    {
        var id = Task.Run(async () => await mongoDBService.InsertOuvrageAsync(ouvrage)).Result;
        documents.Add(ouvrage);
    }
    public List<Document> RechercherParType(TypeDocument type)
    {
        try
        {
            var documents = Task.Run(async () => await mongoDBService.GetOuvragesByTypeAsync(type)).Result;
            return documents
                .Select(doc => mongoDBService.ConvertDocumentToOuvrage(doc))
                .OrderByDescending(o => o.Prix)
                .ToList();
        }
        catch (Exception ex)
        {
            return documents
                .Where(o => o.Type == type)
                .OrderByDescending(o => o.Prix)
                .ToList();
        }
    }

    public List<Document> RechercherParDessinateur(string dessinateur)
    {
        try
        {
            var documents = Task.Run(async () => await mongoDBService.GetBDsByDessinateurAsync(dessinateur)).Result;
            return documents
                .Select(doc => mongoDBService.ConvertDocumentToOuvrage(doc))
                .ToList();
        }
        catch (Exception ex)
        {
            // In-memory fallback search
            return documents
                .Where(o => o.Type == TypeDocument.BD &&
                       o.Details is Dictionary<string, object> details &&
                       details.TryGetValue("dessinateur", out var val) &&
                       val is string dessinateurVal &&
                       dessinateurVal.Contains(dessinateur, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    
}