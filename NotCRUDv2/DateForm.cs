using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DateForm : FieldForm
{
    public DateForm(string id, string title, bool necessary = true, int maxLength = 30) : base(id, title, necessary, maxLength)
    {
        Validator = (valeur) =>
        {
            if (valeur.Length > 10) return false;
            return valeur.All(c => char.IsDigit(c) || c == '/');
        };
    }
}
