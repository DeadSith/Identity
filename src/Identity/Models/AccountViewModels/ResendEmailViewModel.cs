using System.ComponentModel.DataAnnotations;

namespace Identity.Models.AccountViewModels
{
    public class ResendEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}