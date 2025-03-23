using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
public enum TypeDocument
{
    Livre,
    Periodique,
    BD
}
public class Document
{
    public int Id { get; set; }
    public string Titre { get; set; }
    public bool Dispo { get; set; }
    public double Prix { get; set; }
    public object Details { get; set; }
    public string[] Exemplaires { get; set; }
    public TypeDocument Type { get; set; }

    public Document() { }

    public Document(int id, string titre, bool dispo, double prix, object details,TypeDocument type)
    {
        Id = id;
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
        Type = type;
    }

    public Document(string titre, bool dispo, double prix, object details, TypeDocument type)
    {
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
        Type = type;
        Exemplaires = Array.Empty<string>();
    }

    [JsonConstructor]
    public Document(string titre, bool dispo, double prix, object details, string[] exemplaires, TypeDocument type)
    {
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
        Exemplaires = exemplaires;
        Type = type;
    }

    public override string ToString()
    {
        string availability = Dispo ? "Disponible" : "Non disponible";
        string exemplairesInfo = Exemplaires != null && Exemplaires.Length > 0
            ? $" - {Exemplaires.Length} exemplaire(s)"
            : "";

        return $"{Titre} - {Prix:F2}€ - {availability}{exemplairesInfo} (Type: {Type})";
    }
}