using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models.Authentication
{
    public class UserEdit
    {
        public string Email { get; set; }
        [DataType(DataType.Password), MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        public UserEdit() { }
        public UserEdit(AppUser appUser)
        {
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }
}
