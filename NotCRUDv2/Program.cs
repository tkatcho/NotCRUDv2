class Program
{

    static void Main(string[] args)
    
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        bool continuer = true;

        while (continuer)
        {
            string[] optionsMenu =
            [
                    "1. Insérer un ouvrage",
                    "2. Rechercher des périodiques (triés par prix décroissant)",
                    "3. Rechercher des BD par dessinateur",
                    "4. Calculer le prix moyen par type d'ouvrage",
                    "5. Quitter"
            ];

            InteractiveMenu<string> menu = new("GESTION DE BIBLIOTHÈQUE", optionsMenu);
            int choix = menu.Display();

            Console.Clear();

            switch (choix)
            {
                case 0:
                    Utils.InsertDocument();
                    break;
                case 1:
                    //RechercherPeriodiques();
                    break;
                case 2:
                    //RechercherBDParDessinateur();
                    break;
                case 3:
                    //CalculerPrixMoyen();
                    break;
                case 4:
                    continuer = false;
                    Console.WriteLine("Au revoir!");
                    break;
            }
        }
    }
}