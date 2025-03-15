using System.Text;
using System.Collections.Generic;
using System.Linq;

public class InteractiveForm
{
    private List<FieldForm> fields;
    private int selectedField;
    private string title;

    // Variables to store menu positions and dimensions
    private int startX;
    private int startY;
    private int titleWidth;
    private int width;
    private int fieldHeight = 1; // Default height for each field

    public InteractiveForm(string title, List<FieldForm> fields)
    {
        this.title = title;
        this.fields = fields;
        this.selectedField = 0;
    }

    public Dictionary<string, object> Display()
    {
        Console.Clear();
        Console.CursorVisible = false;

        DisplayCompleteMenu();

        int previousField = -1;
        bool formValid = false;

        while (!formValid)
        {
            var result = EditField(selectedField);

            if (result.Action == EditAction.NavigateUp)
            {
                if (selectedField > 0)
                {
                    previousField = selectedField;
                    selectedField--;

                    DisplayField(previousField, false);
                    DisplayField(selectedField, true);
                }
            }
            else if (result.Action == EditAction.NavigateDown)
            {
                if (selectedField < fields.Count - 1)
                {
                    previousField = selectedField;
                    selectedField++;

                    DisplayField(previousField, false);
                    DisplayField(selectedField, true);
                }
            }
            else if (result.Action == EditAction.Continue)
            {
                Console.ResetColor();
                return null;
            }
            else if (result.Action == EditAction.Validate)
            {
                if (AllFieldsAreValid())
                {
                    formValid = true;
                }
                else
                {
                    DisplayErrorMessage("Please fill all required fields");

                    int firstInvalidField = FindFirstInvalidField();
                    if (firstInvalidField >= 0 && firstInvalidField != selectedField)
                    {
                        DisplayField(selectedField, false);
                        selectedField = firstInvalidField;
                        DisplayField(selectedField, true);
                    }
                }
            }
        }

        Dictionary<string, object> values = new Dictionary<string, object>();
        foreach (var field in fields)
        {
            values[field.Id] = GetTypedValue(field);
        }
        Console.ResetColor();

        return values;
    }

    private object GetTypedValue(FieldForm field)
    {
        if (field is ArrayForm arrayField)
        {
            // Assuming the value is stored as a string array
            return field.Value as string[];
        }
        else if (field is ObjectForm objectField)
        {
            // For complex objects
            return objectField.Value;
        }
        else if (field is NumericForm numericField)
        {
            if (double.TryParse(field.Value as string, out double numericValue))
                return numericValue;
            return 0.0;
        }
        else
        {
            return field.Value;
        }
    }

    private bool AllFieldsAreValid()
    {
        foreach (var field in fields)
        {
            if (field.IsNecessary)
            {
                if (!field.Validate(field.Value))
                    return false;
            }
        }
        return true;
    }

    private int FindFirstInvalidField()
    {
        for (int i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            if (field.IsNecessary && !field.Validate(field.Value))
                return i;
        }
        return -1;
    }

    private struct EditResult
    {
        public EditAction Action { get; set; }
    }

    private enum EditAction
    {
        Continue,
        Validate,
        Cancel,
        NavigateUp,
        NavigateDown,
        NavigateTab
    }

    private void DisplayCompleteMenu()
    {
        int maxLabelLength = fields.Max(c => c.Title.Length);
        int maxValueLength = fields.Max(c => c.MaxLength);

        int minimumWidth = maxLabelLength + 2 + maxValueLength + 4; // +4 for margins and borders

        titleWidth = title.Length + 4; // +4 for margins and borders
        width = Math.Max(minimumWidth, titleWidth);

        width = Math.Min(width, Console.WindowWidth - 2);

        startX = Math.Max(0, (Console.WindowWidth - width) / 2);

        int requiredHeight = fields.Count * fieldHeight + 6; // +6 for title, borders, and instructions

        // Ensure we don't position the form higher than the console
        startY = Math.Max(0, (Console.WindowHeight - requiredHeight) / 2);

        // Draw the title
        Console.SetCursorPosition(startX, startY);
        Console.Write("╔");
        for (int i = 0; i < width - 2; i++) Console.Write("═");
        Console.Write("╗");

        Console.SetCursorPosition(startX, startY + 1);
        Console.Write("║");
        string centeredTitle = title.PadLeft((width - 2 + title.Length) / 2).PadRight(width - 2);
        Console.Write(centeredTitle);
        Console.Write("║");

        Console.SetCursorPosition(startX, startY + 2);
        Console.Write("╠");
        for (int i = 0; i < width - 2; i++) Console.Write("═");
        Console.Write("╣");

        // Draw side borders and fields
        for (int i = 0; i < fields.Count; i++)
        {
            // Left border
            Console.SetCursorPosition(startX, startY + 3 + (i * fieldHeight));
            Console.Write("║");

            // Display the field
            DisplayField(i, i == selectedField);

            // Right border
            Console.SetCursorPosition(startX + width - 1, startY + 3 + (i * fieldHeight));
            Console.Write("║");
        }

        // Draw the bottom of the box
        Console.SetCursorPosition(startX, startY + 3 + (fields.Count * fieldHeight));
        Console.Write("╚");
        for (int i = 0; i < width - 2; i++) Console.Write("═");
        Console.Write("╝");

        if (startY + 5 + (fields.Count * fieldHeight) < Console.WindowHeight)
        {
            Console.SetCursorPosition(startX, startY + 5 + (fields.Count * fieldHeight));
            Console.Write("Use ↑ ↓ to navigate, Enter to validate");
        }
    }

    private void DisplayField(int index, bool isActive)
    {
        FieldForm field = fields[index];
        int posY = startY + 3 + (index * fieldHeight);

        if (posY >= Console.WindowHeight)
            return;

        Console.SetCursorPosition(startX + 1, posY);
        Console.ResetColor();
        for (int i = 0; i < width - 2; i++) Console.Write(" ");

        Console.SetCursorPosition(startX + 1, posY);

        if (isActive)
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        string displayTitle = field.Title;
        if (displayTitle.Length > width - 10)
            displayTitle = displayTitle.Substring(0, width - 13) + "...";

        Console.Write(displayTitle + ": ");

        int valueWidth = width - 4 - displayTitle.Length;
        valueWidth = Math.Min(valueWidth, field.MaxLength);
        valueWidth = Math.Max(valueWidth, 1);

        string displayValue = GetDisplayableValue(field, valueWidth);
        Console.Write(displayValue);
        Console.ResetColor();
    }

    private string GetDisplayableValue(FieldForm field, int maxWidth)
    {
        if (field is ArrayForm arrayField)
        {
            string[] items = field.Value as string[];
            if (items == null || items.Length == 0)
                return "[Empty Array]".PadRight(maxWidth);

            string value = string.Join(", ", items);
            if (value.Length > maxWidth)
                return value.Substring(0, maxWidth - 3) + "...";

            return value.PadRight(maxWidth);
        }
        else if (field is ObjectForm objectField)
        {
            return "[JSON object]".PadRight(maxWidth);
        }
        else
        {
            string value = field.Value as string ?? "";
            if (value.Length > maxWidth)
                return value.Substring(value.Length - maxWidth);

            return value.PadRight(maxWidth);
        }
    }

    private EditResult EditField(int index)
    {
        FieldForm field = fields[index];

        int posX = startX + 1 + field.Title.Length + 2;

        // If the title is too long, adjust posX to not go off screen
        if (posX >= startX + width - 4)
            posX = startX + width / 2; // Position halfway through the form

        int posY = startY + 3 + (index * fieldHeight);

        // Make sure we're within the console bounds
        posX = Math.Min(posX, Console.WindowWidth - 5);

        // Calculate available width for the field value
        int availableWidth = (startX + width - 2) - posX;
        availableWidth = Math.Min(availableWidth, field.MaxLength);
        availableWidth = Math.Max(availableWidth, 1);

        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(posX, posY);

        if (field is ArrayForm arrayField)
        {
            return EditArrayField(arrayField, posX, posY, availableWidth);
        }
        else if (field is ObjectForm objectField)
        {

            return EditObjectField(objectField, posX, posY, availableWidth);
        }
        else
        {
            return EditSimpleField(field, posX, posY, availableWidth);
        }
    }

    private EditResult EditArrayField(ArrayForm field, int posX, int posY, int availableWidth)
    {
        // Clear the edit area
        Console.SetCursorPosition(posX, posY);
        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
        Console.SetCursorPosition(posX, posY);

        // Display current values
        string[] items = field.Value as string[] ?? new string[0];
        string currentValue = string.Join(", ", items);

        // Truncate if too long
        if (currentValue.Length > availableWidth - 1)
            currentValue = currentValue.Substring(0, availableWidth - 4) + "...";

        Console.Write(currentValue);

        // Instructions - check if there's space for them
        if (startY + fields.Count * fieldHeight + 4 < Console.WindowHeight)
        {
            Console.ResetColor();
            Console.SetCursorPosition(startX, startY + fields.Count * fieldHeight + 4);
            Console.Write("Enter values separated by commas");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.SetCursorPosition(posX + Math.Min(currentValue.Length, availableWidth - 1), posY);

        // Interactive editing
        StringBuilder input = new StringBuilder(currentValue);
        int cursorPos = Math.Min(currentValue.Length, availableWidth - 1);

        ConsoleKeyInfo keyInfo;

        while (true)
        {
            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    // Validate the array
                    string[] newItems = input.ToString().Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrWhiteSpace(s))
                                          .ToArray();
                    field.Value = newItems;
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.Validate };

                case ConsoleKey.Escape:
                    // Cancel
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.Continue };

                case ConsoleKey.UpArrow:
                    // Navigate up
                    string[] upItems = input.ToString().Split(',')
                                         .Select(s => s.Trim())
                                         .Where(s => !string.IsNullOrWhiteSpace(s))
                                         .ToArray();
                    field.Value = upItems;
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.NavigateUp };

                case ConsoleKey.DownArrow:
                    // Navigate down
                    string[] downItems = input.ToString().Split(',')
                                           .Select(s => s.Trim())
                                           .Where(s => !string.IsNullOrWhiteSpace(s))
                                           .ToArray();
                    field.Value = downItems;
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.NavigateDown };

                case ConsoleKey.Backspace:
                    if (cursorPos > 0)
                    {
                        input.Remove(cursorPos - 1, 1);
                        cursorPos--;

                        // Refresh display
                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth - 1)
                            displayText = displayText.Substring(0, availableWidth - 4) + "...";
                        Console.Write(displayText);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursorPos < input.Length)
                    {
                        input.Remove(cursorPos, 1);

                        // Refresh display
                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth - 1)
                            displayText = displayText.Substring(0, availableWidth - 4) + "...";
                        Console.Write(displayText);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorPos > 0)
                    {
                        cursorPos--;
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (cursorPos < input.Length && cursorPos < availableWidth - 1)
                    {
                        cursorPos++;
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                default:
                    // Add character if not a control character
                    if (!char.IsControl(keyInfo.KeyChar) && input.Length < field.MaxLength && cursorPos < availableWidth - 1)
                    {
                        input.Insert(cursorPos, keyInfo.KeyChar);
                        cursorPos++;

                        // Refresh display
                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth - 1)
                            displayText = displayText.Substring(0, availableWidth - 4) + "...";
                        Console.Write(displayText);

                        // Ensure cursor position is within console bounds
                        cursorPos = Math.Min(cursorPos, availableWidth - 1);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;
            }
        }
    }
    private EditResult EditObjectField(ObjectForm field, int posX, int posY, int availableWidth)
    {
        Console.SetCursorPosition(posX, posY);
        string displayText = "[Press Enter to edit]";
        if (displayText.Length > availableWidth)
            displayText = displayText.Substring(0, availableWidth);
        Console.Write(displayText);

        // Wait for a key
        ConsoleKeyInfo keyInfo;
        while (true)
        {
            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    try
                    {
                        // Reset colors before launching sub-form
                        Console.ResetColor();

                        // Launch object editing with a sub-form
                        Dictionary<string, object> result = EditSubForm(field);
                        if (result != null)
                        {
                            field.Value = result;
                        }

                        // Redraw the main form
                        Console.Clear();
                        DisplayCompleteMenu();
                    }
                    catch (Exception ex)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.WriteLine($"Error in sub-form: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);

                        // Redraw the main form
                        Console.Clear();
                        DisplayCompleteMenu();
                    }

                    return new EditResult { Action = EditAction.Validate };

                case ConsoleKey.Escape:
                    // Restore original screen
                    Console.ResetColor();
                    Console.Clear();
                    DisplayCompleteMenu();
                    return new EditResult { Action = EditAction.Continue };

                case ConsoleKey.UpArrow:
                    return new EditResult { Action = EditAction.NavigateUp };

                case ConsoleKey.DownArrow:
                    return new EditResult { Action = EditAction.NavigateDown };
            }
        }
    }
    private Dictionary<string, object> EditSubForm(ObjectForm objectField)
    {
        InteractiveForm subForm = new InteractiveForm(
            objectField.Title,
            objectField.Properties
        );

        return subForm.Display();
    }

    private EditResult EditSimpleField(FieldForm field, int posX, int posY, int availableWidth)
    {
        string value = field.Value as string ?? "";

        string displayValue = value;
        if (displayValue.Length > availableWidth)
            displayValue = displayValue.Substring(0, availableWidth);

        Console.SetCursorPosition(posX, posY);
        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
        Console.SetCursorPosition(posX, posY);

        Console.Write(displayValue);
        Console.SetCursorPosition(posX + Math.Min(displayValue.Length, availableWidth - 1), posY);

        StringBuilder input = new StringBuilder(value);
        int cursorPos = Math.Min(value.Length, availableWidth - 1);

        ConsoleKeyInfo keyInfo;

        while (true)
        {
            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    ClearErrorMessage();
                    field.Value = input.ToString();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.Validate };

                case ConsoleKey.Escape:
                    field.Value = input.ToString();
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.Continue };

                case ConsoleKey.UpArrow:
                    field.Value = input.ToString();
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.NavigateUp };

                case ConsoleKey.DownArrow:
                    field.Value = input.ToString();
                    ClearErrorMessage();
                    Console.CursorVisible = false;
                    return new EditResult { Action = EditAction.NavigateDown };

                case ConsoleKey.Backspace:
                    if (cursorPos > 0)
                    {
                        input.Remove(cursorPos - 1, 1);
                        cursorPos--;

                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);

                        // Get displayable portion of the input
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth)
                            displayText = displayText.Substring(0, availableWidth);

                        Console.Write(displayText);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursorPos < input.Length)
                    {
                        input.Remove(cursorPos, 1);

                        // Refresh display
                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);

                        // Get displayable portion of the input
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth)
                            displayText = displayText.Substring(0, availableWidth);

                        Console.Write(displayText);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorPos > 0)
                    {
                        cursorPos--;
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (cursorPos < input.Length && cursorPos < availableWidth - 1)
                    {
                        cursorPos++;
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;

                case ConsoleKey.Home:
                    cursorPos = 0;
                    Console.SetCursorPosition(posX + cursorPos, posY);
                    break;

                case ConsoleKey.End:
                    cursorPos = Math.Min(input.Length, availableWidth - 1);
                    Console.SetCursorPosition(posX + cursorPos, posY);
                    break;

                default:
                    // Add character if not a control character and if we don't exceed max length
                    if (!char.IsControl(keyInfo.KeyChar) && input.Length < field.MaxLength && cursorPos < availableWidth - 1)
                    {
                        // For input validation (if applicable)
                        if (field is StringForm stringField && stringField.Validator != null)
                        {
                            string potentialNewValue = input.ToString().Insert(cursorPos, keyInfo.KeyChar.ToString());
                            if (!stringField.Validator(potentialNewValue))
                                break;
                        }

                        input.Insert(cursorPos, keyInfo.KeyChar);
                        cursorPos++;

                        // Refresh display
                        Console.SetCursorPosition(posX, posY);
                        for (int i = 0; i < availableWidth; i++) Console.Write(" ");
                        Console.SetCursorPosition(posX, posY);

                        // Get displayable portion of the input
                        string displayText = input.ToString();
                        if (displayText.Length > availableWidth)
                            displayText = displayText.Substring(0, availableWidth);

                        Console.Write(displayText);

                        // Ensure cursor position is within console bounds
                        cursorPos = Math.Min(cursorPos, availableWidth - 1);
                        Console.SetCursorPosition(posX + cursorPos, posY);
                    }
                    break;
            }
        }
    }

    private void DisplayErrorMessage(string message)
    {
        // Check if we have space to display the error message
        if (startY + fields.Count * fieldHeight + 4 >= Console.WindowHeight)
            return;

        Console.ResetColor();

        // First clear any existing error line
        ClearErrorMessage();

        // Display the new message
        Console.SetCursorPosition(startX, startY + fields.Count * fieldHeight + 4);
        Console.ForegroundColor = ConsoleColor.Red;

        // Ensure message doesn't exceed form width
        if (message.Length > width)
        {
            message = message.Substring(0, width - 3) + "...";
        }

        Console.Write(message);
        Console.ResetColor();
    }

    private void ClearErrorMessage()
    {
        // Check if we have space to display the error message
        if (startY + fields.Count * fieldHeight + 4 >= Console.WindowHeight)
            return;

        Console.ResetColor();

        Console.SetCursorPosition(startX, startY + fields.Count * fieldHeight + 4);
        Console.Write(new string(' ', width));
    }
}