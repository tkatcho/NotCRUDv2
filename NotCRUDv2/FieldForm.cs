public abstract class FieldForm
{
    public string Id { get; protected set; }
    public string Title { get; protected set; }
    public bool IsNecessary { get; protected set; }
    public int MaxLength { get; protected set; }
    public object Value { get; set; }

    public Func<string, bool> Validator { get; set; }

    protected FieldForm(string id, string title, bool necessary = true, int maxLength = 30)
    {
        Id = id;
        Title = title;
        IsNecessary = necessary;
        MaxLength = maxLength;
    }

    public FieldForm WithValidator(Func<string, bool> validator)
    {
        Validator = validator;
        return this;
    }

    public virtual bool Validate(object value)
    {
        if (IsNecessary && value == null)
            return false;

        if (value is string strValue && strValue.Length > MaxLength)
            return false;

        if (value is string str && Validator != null)
            return Validator(str);

        return true;
    }

    public override string ToString()
    {
        return Title;
    }
}