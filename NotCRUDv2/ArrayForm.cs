public class ArrayForm : FieldForm
{
    public int MaxItems { get; set; } = 100;

    public ArrayForm(string id, string title, bool necessary = true, int maxLength = 30)
        : base(id, title, necessary, maxLength)
    {
    }

    public override bool Validate(object value)
    {
        if (!base.Validate(value))
            return false;

        if (!(value is string[] strArray))
            return false;

        if (strArray.Length > MaxItems)
            return false;

        if (Validator != null)
        {
            foreach (string item in strArray)
            {
                if (!Validator(item))
                    return false;
            }
        }

        return true;
    }
}