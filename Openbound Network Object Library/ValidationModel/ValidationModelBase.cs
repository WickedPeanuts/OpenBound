using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBound_Network_Object_Library.Helper;

namespace OpenBound_Network_Object_Library.ValidationModel
{
    public class ValidationModelBase
    {
        public ICollection<ValidationResult> ValidationResults;

        public bool Validate() => FormValidationHelper.TryValidate(this, out ValidationResults);

        public string ValidationErrorsToString()
        {
            string validationErrors = "";
            foreach (ValidationResult vr in ValidationResults)
                validationErrors += vr.ErrorMessage + '\n';

            return validationErrors;
        }
    }
}
