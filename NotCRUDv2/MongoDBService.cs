using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MongoDBService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<BsonDocument> _ouvragesCollection;

    public MongoDBService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        _ouvragesCollection = _database.GetCollection<BsonDocument>("ouvrages");
    }

    public async Task<List<BsonDocument>> GetAllOuvragesAsync()
    {
        return await _ouvragesCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<List<BsonDocument>> GetOuvragesByTypeAsync(TypeDocument type)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Type", type.ToString());
        return await _ouvragesCollection.Find(filter)
            .Sort(Builders<BsonDocument>.Sort.Descending("Prix"))
            .ToListAsync();
    }

    public async Task<List<BsonDocument>> GetBDsByDessinateurAsync(string dessinateur)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Type", TypeDocument.BD.ToString()),
            Builders<BsonDocument>.Filter.Regex("Dessinateur", new BsonRegularExpression(dessinateur, "i"))
        );

        return await _ouvragesCollection.Find(filter).ToListAsync();
    }

    public async Task<double> CalculateAveragePriceByTypeAsync(TypeDocument type)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("Type", type.ToString());
        var result = await _ouvragesCollection.Find(filter).ToListAsync();

        if (result.Count == 0)
            return 0;

        return result.Average(doc => doc["Prix"].AsDouble);
    }

    public async Task<int> InsertOuvrageAsync(Document ouvrage)
    {
        var document = ConvertOuvrageToDocument(ouvrage);
        await _ouvragesCollection.InsertOneAsync(document);
        return document["_id"].AsInt32;
    }

    private BsonDocument ConvertOuvrageToDocument(Document ouvrage)
    {
        // Get the next available ID (you'll need to implement this method)
        int nextId = GetNextSequenceValue("ouvrageId");

        var document = new BsonDocument
    {
        { "_id", nextId },
        { "Titre", ouvrage.Titre },
        { "Dispo", ouvrage.Dispo },
        { "Prix", ouvrage.Prix },
        { "Type", ouvrage.Type.ToString() },
        { "Details", BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(ouvrage.Details)) }
    };
        if (ouvrage.Type == TypeDocument.Livre)
        {
            if (ouvrage.Exemplaires != null)
            {
                var exemplairesArray = new BsonArray();
                foreach (var exemplaire in ouvrage.Exemplaires)
                {
                    exemplairesArray.Add(exemplaire);
                }
                document.Add("exemplaires", exemplairesArray);
            }
        }
        return document;
    }

    // Method to get the next sequence value (auto-increment ID)
    private int GetNextSequenceValue(string sequenceName)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", sequenceName);
        var update = Builders<BsonDocument>.Update.Inc("value", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var sequenceDocument = _database.GetCollection<BsonDocument>("counters")
            .FindOneAndUpdate(filter, update, options);

        return sequenceDocument["value"].AsInt32;
    }

    public Document ConvertDocumentToOuvrage(BsonDocument document)
    {
        var type = Enum.Parse<TypeDocument>(document["Type"].AsString);
        var titre = document["Titre"].AsString;
        var dispo = document["Dispo"].AsBoolean;
        var prix = document["Prix"].AsDouble;

        // Convert the BsonDocument Details back to your C# object
        var detailsJson = document["Details"].ToJson();
        var details = System.Text.Json.JsonSerializer.Deserialize<Document>(detailsJson);

        int id = document["_id"].AsInt32;

        // Update your Document constructor to accept these parameters
        Document document1 = new(id,titre, dispo, prix, details)
        {
            Id = id
        };

        return document1;
    }
}
