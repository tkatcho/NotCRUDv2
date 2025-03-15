public class ObjectForm : FieldForm
{
    public List<FieldForm> Properties { get; private set; } = new List<FieldForm>();

    public ObjectForm(string id, string title, bool necessary = true)
        : base(id, title, necessary)
    {
    }

    public ObjectForm AddProperty(FieldForm property)
    {
        Properties.Add(property);
        return this;
    }

    public override bool Validate(object value)
    {
        return base.Validate(value);
    }
}
