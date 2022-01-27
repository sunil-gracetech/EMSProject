using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMSProject.Models.ViewModel
{
    public class SignUp
    {

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password",ErrorMessage ="Password not matched !")]
        public string ConfirmPassword { get; set; }

        public bool IsVerified { get; set; }
    }
}
