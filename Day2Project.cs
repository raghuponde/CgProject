go to day 58 
  and add this things 
go to project in Models folder add this class 

  using System.ComponentModel.DataAnnotations;

namespace ResortAPI.Models
{
    public class RegisterUser
    {

        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}

go to day 60 and add thises things in project 
namespace ResortAPI.Models
{
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}


Now add one folder with the name Services and add one Interface with name IAuth 

and add these methods into it 

