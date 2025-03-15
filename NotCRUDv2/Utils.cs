using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class Utils
{
    static Library library = new();

    public static void InsertDocument()
    {
        string[] optionsType =
        [
        "1. Livre",
        "2. Périodique",
        "3. BD"
        ];

        InteractiveMenu<string> menu = new("Choisissez le type d'ouvrage", optionsType, true);
        int type = menu.Display();

        var commonChoices = new List<FieldForm>
        {
            new StringForm("titre","Titre", true),
            new BooleanForm("dispo", "Disponibilite"),
            new NumericForm("prix", "Prix (€)"),
        };

        List<FieldForm> fieldForms = [.. commonChoices];
        string typeLivre = "";
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
                typeLivre = "Livre";
                break;
            case 1:
                var periodDetailsForm = new ObjectForm("détails", "Details", true);
                periodDetailsForm.AddProperty(new DateForm("date", "Date"));
                periodDetailsForm.AddProperty(new StringForm("periodicite", "periodicite"));
                fieldForms.Add(periodDetailsForm);
                typeLivre = "Périodique";
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
                typeLivre = "BD";
                break;

        }
        InteractiveForm interactiveMenu = new("INSERTION D'UN OUVRAGE", fieldForms);
        var val = interactiveMenu.Display();

        if (val == null)
            return;

        string titre = (string)val["titre"];
        double prix = (double)val["prix"];
        bool dispo = (string)val["dispo"] =="true";
        var details = val["détails"];
        if (type == 0)
        {
            var exemplaires = (string[])val["exemplaires"];
            library.AjouterOuvrage(new Document(titre, dispo, prix, details,exemplaires));
            return;
        }
        library.AjouterOuvrage(new Document(titre, dispo, prix, details));

        //ajouter le livre
    }
}
