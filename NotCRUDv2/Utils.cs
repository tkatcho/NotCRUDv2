using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class Utils
{
    static Library library = new();

    public static void InsertDocument(int cpt=1)
    {
        string[] optionsType =
        [
        "1. Livre",
        "2. Périodique",
        "3. BD"
        ];

        InteractiveMenu<string> menu = new("Choisissez le type d'ouvrage", optionsType, true);
        int type = menu.Display();

        if (type == -1)
            return;
        var commonChoices = new List<FieldForm>
        {
            new StringForm("titre","Titre", true),
            new BooleanForm("dispo", "Disponibilite"),
            new NumericForm("prix", "Prix (€)"),
        };

        List<FieldForm> fieldForms = [.. commonChoices];
        TypeDocument typeLivre = TypeDocument.Livre;
        switch (type)
        {
            case 0: //Livre
                var bookDetailsForm = new ObjectForm("détails", "Details", true);
                bookDetailsForm.AddProperty(new NumericForm("année", "Année de publication")
                {
                    MinValue = 1400,
                    MaxValue = DateTime.Now.Year
                });
                bookDetailsForm.AddProperty(new StringForm("maison d'édition", "Maison d'édition", maxLength: 30));
                bookDetailsForm.AddProperty(new StringForm("auteur", "Auteur", maxLength: 50)
            .WithValidator(author => !string.IsNullOrWhiteSpace(author)));

                fieldForms.Add(new ArrayForm("exemplaires", "Exemplaires"));
                fieldForms.Add(bookDetailsForm);
                typeLivre = TypeDocument.Livre;
                break;
            case 1:
                var periodDetailsForm = new ObjectForm("détails", "Details", true);
                periodDetailsForm.AddProperty(new DateForm("date", "Date"));
                periodDetailsForm.AddProperty(new StringForm("periodicite", "periodicite"));
                fieldForms.Add(periodDetailsForm);
                typeLivre = TypeDocument.Periodique;
                break;
            case 2:
                var bdDetailsForm = new ObjectForm("détails", "Details", true);
                bdDetailsForm.AddProperty(new NumericForm("année", "Année de publication")
                {
                    MinValue = 1400,
                    MaxValue = DateTime.Now.Year
                });
                bdDetailsForm.AddProperty(new StringForm("maison d'édition", "Maison d'édition", maxLength: 30));
                bdDetailsForm.AddProperty(new StringForm("auteur", "Auteur", maxLength: 50));
                bdDetailsForm.AddProperty(new StringForm("dessinateur", "Dessinateur", maxLength: 50));
                fieldForms.Add(bdDetailsForm);
                typeLivre = TypeDocument.BD;
                break;

        }
        InteractiveForm interactiveMenu = new("INSERTION D'UN OUVRAGE", fieldForms);
        var val = interactiveMenu.Display();

        if (val == null)
            return;

        string titre = (string)val["titre"];
        double prix = (double)val["prix"];
        bool dispo = (string)val["dispo"] == "true";
        var details = val["détails"];

        for (int i = 0; i < cpt; i++)
        {
            if (typeLivre == TypeDocument.Livre)
            {
                var exemplaires = (string[])val["exemplaires"];
                library.AjouterOuvrage(new Document(titre, dispo, prix, details, exemplaires, typeLivre));
                return;
            }
            library.AjouterOuvrage(new Document(titre, dispo, prix, details, typeLivre));
        }
        return;

    }

    public static void RechercherPeriodiques()
    {

        var periodiques = library.RechercherParType(TypeDocument.Periodique).ToArray();

        if (periodiques.Length == 0)
        {
            Console.WriteLine("Aucun périodique trouvé.");
            return;
        }
        InteractiveMenu<Document> menu = new("RECHERCHE DE PÉRIODIQUES", periodiques, true);
        menu.Display();
    }

    public static void RechercherBDParDessinateur(string input)
    {
        var bd = library.RechercherParDessinateur(input).ToArray();
        if (bd.Length == 0)
        {
            Console.WriteLine("Aucune BD trouvée.");
            return;
        }
        InteractiveMenu<Document> menu = new("RECHERCHE DE DESSINATEURS", bd, true);
        menu.Display();
    }

    public static void PrixMoyenParOuvrage()
    {
        List<string> options = new List<string>();
        var allTypes = Enum.GetValues(typeof(TypeDocument)).Cast<TypeDocument>();

        foreach (var type in allTypes)
        {
            var documents = library.RechercherParType(type);
            if (documents.Count == 0)
            {
                continue;
            }
            double prixMoyen = documents.Average(doc => doc.Prix);
            int nombreOuvrages = documents.Count;
            options.Add($"{type}: {prixMoyen:F2}€ (sur {nombreOuvrages} ouvrages)");
        }

        var allDocuments = allTypes.SelectMany(type => library.RechercherParType(type)).ToList();
        if (allDocuments.Count > 0)
        {
            double prixMoyenTotal = allDocuments.Average(doc => doc.Prix);
            options.Add($"Prix moyen global: {prixMoyenTotal:F2}€ (sur {allDocuments.Count} ouvrages)");
        }
        else
        {
            options.Add("Aucun ouvrage dans la bibliothèque.");
        }

        if (options.Count == 0)
        {
            Console.WriteLine("Aucun ouvrage dans la bibliothèque.");
            return;
        }

        InteractiveMenu<string> menu = new("PRIX MOYEN PAR TYPE D'OUVRAGE", options.ToArray(), true);
        menu.Display();
    }

    public static void InsertMultiple(int cpt)
    {
       InsertDocument(cpt);
    }
}
