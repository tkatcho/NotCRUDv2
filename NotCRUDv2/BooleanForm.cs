public class BooleanForm : FieldForm
{

    public BooleanForm(string id, string title, bool necessary = true)
        : base(id, title, necessary)
    {
    }

    public override bool Validate(object value)
    {
        if (!base.Validate(value))
            return false;

        if (value is string strValue)
        {
            return strValue.ToString() == "true" || strValue.ToString() == "false";
        }

        return false;
    }
}