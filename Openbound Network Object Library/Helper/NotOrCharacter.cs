using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Helper
{
    public class NotOrCharacter : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as string;
            bool isValid = true;

            if (!string.IsNullOrEmpty(inputValue))
                isValid = !inputValue.Contains("|");

            return isValid;
        }
    }
}
