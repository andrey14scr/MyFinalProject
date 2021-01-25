using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    public class EmailAttribute : Attribute
    {
        public string Mask { get; private set; }

        public EmailAttribute()
        {
            Mask = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
        }

        public EmailAttribute(string mask)
        {
            if (string.IsNullOrEmpty(mask))
            {
                throw new ArgumentNullException("Mask must be not empty value!");
            }
            Mask = mask;
        }
    }
}
