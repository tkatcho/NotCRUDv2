class InteractiveMenu<T>
{
    private T[] options;
    private int selectedOption;
    private string title;
    private int xPos;
    private int yPos;
    private int titleLen;
    private bool exitable;
    private bool selectable;
    private int visibleOptionsCount = 6; 
    private int startIndex = 0; 

    public InteractiveMenu(string title, T[] options, bool exitable = false, bool selectable = true)
    {
        this.title = title;
        this.options = options;
        this.selectedOption = 0;
        this.exitable = exitable;
        this.selectable = selectable;
    }

    public int Display()
    {
        Console.Clear();
        DisplayMenu();
        int oldOption = selectedOption;
        ConsoleKey consoleKey;
        if (!selectable)
            return -1;
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            consoleKey = keyInfo.Key;
            if (consoleKey == ConsoleKey.UpArrow)
            {
                selectedOption--;
                if (selectedOption < 0)
                    selectedOption = options.Length - 1;

                if (selectedOption < startIndex)
                    startIndex = selectedOption;

                RefreshAllOptions();
                oldOption = selectedOption;
            }
            else if (consoleKey == ConsoleKey.DownArrow)
            {
                selectedOption++;
                if (selectedOption > options.Length - 1)
                    selectedOption = 0;

                if (selectedOption >= startIndex + visibleOptionsCount)
                    startIndex = selectedOption - visibleOptionsCount + 1;

                if (selectedOption == 0)
                    startIndex = 0;

                RefreshAllOptions();
                oldOption = selectedOption;
            }
            else if (consoleKey == ConsoleKey.Escape && exitable)
                return -1;
        } while (consoleKey != ConsoleKey.Enter);
        return selectedOption;
    }

    void DisplayMenu()
    {
        int maxLength = options.Max(o => o.ToString().Length) + 4;
        titleLen = Math.Max(maxLength, title.Length + 4);
        xPos = (Console.WindowWidth - titleLen) / 2;

        yPos = (Console.WindowHeight - visibleOptionsCount - 4) / 2;

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

        int visibleCount = Math.Min(visibleOptionsCount, options.Length);

        for (int i = 0; i < visibleCount; i++)
        {
            int optionIndex = startIndex + i;
            if (optionIndex < options.Length)
            {
                ShowOption(i, optionIndex, optionIndex == selectedOption);
            }
        }

        Console.SetCursorPosition(xPos, yPos + 3 + visibleCount);
        Console.Write("╚");
        for (int i = 0; i < titleLen - 2; i++) Console.Write("═");
        Console.Write("╝");

        Console.SetCursorPosition(xPos, yPos + 5 + visibleCount);
        Console.Write("Utilisez ↑ ↓ pour naviguer et Entrée pour sélectionner");

        if (options.Length > visibleOptionsCount)
        {
            if (startIndex > 0)
            {
                Console.SetCursorPosition(xPos + titleLen - 2, yPos + 3);
                Console.Write("▲");
            }

            if (startIndex + visibleOptionsCount < options.Length)
            {
                Console.SetCursorPosition(xPos + titleLen - 2, yPos + 2 + visibleCount);
                Console.Write("▼");
            }
        }
    }

    void ShowOption(int displayIndex, int optionIndex, bool selected)
    {
        Console.SetCursorPosition(xPos, yPos + 3 + displayIndex);
        Console.Write("║");

        if (selected)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        string optionText = $" {options[optionIndex]} ".PadRight(titleLen - 2);
        Console.Write(optionText);
        Console.ResetColor();
        Console.Write("║");
    }

    void RefreshAllOptions()
    {
        int visibleCount = Math.Min(visibleOptionsCount, options.Length);

        for (int i = 0; i < visibleCount; i++)
        {
            int optionIndex = startIndex + i;
            if (optionIndex < options.Length)
            {
                ShowOption(i, optionIndex, optionIndex == selectedOption);
            }
        }

        if (options.Length > visibleOptionsCount)
        {
            Console.SetCursorPosition(xPos + titleLen - 2, yPos + 3);
            Console.Write(" ");
            Console.SetCursorPosition(xPos + titleLen - 2, yPos + 2 + visibleCount);
            Console.Write(" ");

            if (startIndex > 0)
            {
                Console.SetCursorPosition(xPos + titleLen - 2, yPos + 3);
                Console.Write("▲");
            }

            if (startIndex + visibleOptionsCount < options.Length)
            {
                Console.SetCursorPosition(xPos + titleLen - 2, yPos + 2 + visibleCount);
                Console.Write("▼");
            }
        }
    }
}