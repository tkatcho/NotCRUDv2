public class StringForm : FieldForm
{
    public StringForm(string id, string title, bool necessary = true, int maxLength = 30)
        : base(id, title, necessary, maxLength)
    {
    }
    public override bool Validate(object value)
    {
        if (!base.Validate(value))
            return false;

        if (value is string strValue && value.ToString().Length >1)
        {
            if (double.TryParse(strValue, out double numValue))
                return false;
            return true;
        }

        return false;
    }
}