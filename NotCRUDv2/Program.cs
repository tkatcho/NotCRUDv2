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
                                    "5. Quitter",
                                    "6. Insertion de plusieurs documents",
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
                    Utils.RechercherPeriodiques();
                    break;
                case 2:
                    Console.WriteLine("Veuillez ecrire le nom du dessinateur rechercher");
                    string dessinateur = Console.ReadLine();
                    Utils.RechercherBDParDessinateur(dessinateur);
                    break;
                case 3:
                    Utils.PrixMoyenParOuvrage();
                    break;
                case 4:
                    continuer = false;
                    Console.WriteLine("Au revoir!");
                    break;
                case 5:
                    Console.WriteLine("combien de documents?");
                    if (int.TryParse(Console.ReadLine(), out int cpt))
                    {
                        Utils.InsertMultiple(cpt);
                    }
                    else
                    {
                        Console.WriteLine("Entrée invalide. Veuillez entrer un nombre.");
                    }
                    break;
            }
            if (continuer)
            {
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey(true);
            }
        }
    }
}
