using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public enum TypeDocument
{
    Livre,
    Periodique,
    BD
}
public class Document
{
    public int Id {  get; set; }
    public string Titre { get; set; }
    public bool Dispo { get; set; }
    public double Prix { get; set; }
    public object Details { get; set; }
    public string[] Exemplaires { get; set; }
    public TypeDocument Type { get; protected set; }

    public Document(int id, string titre, bool dispo, double prix, object details)
    {
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
        Id = id;
    }
    public Document(string titre, bool dispo, double prix, object details)
    {
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
    }
    public Document(string titre, bool dispo, double prix, object details, string[] exemplaires)
    {
        Titre = titre;
        Dispo = dispo;
        Prix = prix;
        Details = details;
        Exemplaires = exemplaires;
    }
    public override string ToString()
    {
        return $"{Titre} par {Dispo} - {Prix}€ (Type: {Type})";
    }
}

