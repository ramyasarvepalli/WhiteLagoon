using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.Web.Models.ViewModels
{
    public class RegisterVM
    {
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [Display(Name="Confirm password")]
        public required string ConfirmPassword { get; set; }

        public required string Name { get; set; }

        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
