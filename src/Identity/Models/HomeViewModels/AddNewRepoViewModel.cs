using System.ComponentModel.DataAnnotations;

namespace Identity.Models.HomeViewModels
{
    public class AddNewRepoViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9-]+",ErrorMessage = "Name can contain only English letters, numbers and -")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Name should be 5-30 characters long")]
        [Display(Name = "Repository Name")]
        public string RepoName { get; set; }
        [Display(Name = "Is repository public?")]
        public bool IsPublic { get; set; }
    }
}