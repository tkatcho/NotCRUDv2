public class NumericForm : FieldForm
{
    public double MinValue { get; set; } = double.MinValue;
    public double MaxValue { get; set; } = double.MaxValue;

    public NumericForm(string id, string title, bool necessary = true)
        : base(id, title, necessary)
    {
    }

    public override bool Validate(object value)
    {
        if (!base.Validate(value))
            return false;

        if (value is string strValue)
        {
            if (!double.TryParse(strValue, out double numValue))
                return false;

            value = numValue;
        }

        if (value is double doubleValue)
        {
            return doubleValue >= MinValue && doubleValue <= MaxValue;
        }

        if (value is int intValue)
        {
            return intValue >= MinValue && intValue <= MaxValue;
        }

        return false;
    }
}