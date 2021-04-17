using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Helper;
using OpenBound_Network_Object_Library.Models;
using System.ComponentModel.DataAnnotations;

namespace OpenBound_Network_Object_Library.ValidationModel
{
    public class PlayerDTO : ValidationModelBase
    {
        [Required, MinLength(4), MaxLength(30), RegularExpression(@"[0-9]*[A-z ]+[0-9]*")]
        public string Nickname { get; set; }

        [Required, MinLength(3), MaxLength(30), EmailAddress]
        public string Email { get; set; }

        public Gender Gender { get; set; }

        [Required, MinLength(6), MaxLength(30), NotOrCharacter(ErrorMessage = Language.PlayerDTOPasswordErrorMessage)]
        public string Password { get; set; }

        [Required, Compare("Password"), Display(Name = Language.PlayerDTOPasswordConfirmationName)]
        public string PasswordConfirmation { get; set; }

    }
}
