using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password" , ErrorMessage = "not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        public string Telephone { get; set; }

    }
}
