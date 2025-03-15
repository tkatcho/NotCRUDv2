class InteractiveMenu<T>
{
    private T[] options;
    private int selectedOption;
    private string title;

    private int xPos;
    private int yPos;
    private int titleLen;
    private bool exitable;
    public InteractiveMenu(string title, T[] options, bool exitable=false)
    {
        this.title = title;
        this.options = options;
        this.selectedOption = 0;
        this.exitable = exitable;
    }

    public int Display()
    {
        Console.Clear();
        DisplayMenu();

        int oldOption = selectedOption;
        ConsoleKey consoleKey;

        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            consoleKey = keyInfo.Key;

            if (consoleKey == ConsoleKey.UpArrow)
            {
                selectedOption--;
                if (selectedOption < 0)
                    selectedOption = options.Length - 1;
                RefreshOptions(oldOption, selectedOption);
                oldOption = selectedOption;
            }
            else if (consoleKey == ConsoleKey.DownArrow)
            {
                selectedOption++;
                if (selectedOption > options.Length - 1)
                    selectedOption = 0;
                RefreshOptions(oldOption, selectedOption);
                oldOption = selectedOption;
            }
            else if (consoleKey == ConsoleKey.Escape && exitable)
                break;
        } while (consoleKey != ConsoleKey.Enter);
        return selectedOption;
    }

    void DisplayMenu()
    {
        int maxLength = options.Max(o => o.ToString().Length) + 4;
        titleLen = Math.Max(maxLength, title.Length + 4);

        xPos = (Console.WindowWidth - titleLen) / 2;
        yPos = (Console.WindowHeight - options.Length + 4) / 2;

        // Dessiner le titre
        Console.SetCursorPosition(xPos, yPos);
        Console.Write("╔");
        for (int i = 0; i < titleLen - 2; i++) Console.Write("═");
        Console.Write("╗");

        Console.SetCursorPosition(xPos, yPos + 1);
        Console.Write("║");
        string titreCentre = title.PadLeft((titleLen - 2 + title.Length) / 2).PadRight(titleLen - 2);
        Console.Write(titreCentre);
        Console.Write("║");

        Console.SetCursorPosition(xPos, yPos + 2);
        Console.Write("╠");
        for (int i = 0; i < titleLen - 2; i++) Console.Write("═");
        Console.Write("╣");

        for (int i = 0; i < options.Length; i++)
        {
            ShowOption(i, i == selectedOption);
        }

        // Dessiner le bas de la boîte
        Console.SetCursorPosition(xPos, yPos + 3 + options.Length);
        Console.Write("╚");
        for (int i = 0; i < titleLen - 2; i++) Console.Write("═");
        Console.Write("╝");

        // Instructions de navigation
        Console.SetCursorPosition(xPos, yPos + 5 + options.Length);
        Console.Write("Utilisez ↑ ↓ pour naviguer et Entrée pour sélectionner");
    }

    void ShowOption(int index, bool selected)
    {
        Console.SetCursorPosition(xPos, yPos + 3 + index);
        Console.Write("║");

        // Surligner l'option sélectionnée
        if (selected)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        string optionText = $" {options[index]} ".PadRight(titleLen - 2);
        Console.Write(optionText);

        Console.ResetColor();
        Console.Write("║");
    }
    void RefreshOptions(int old, int newOpt)
    {
        ShowOption(old, false);
        ShowOption(newOpt, true);
    }
}