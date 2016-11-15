using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class ProfileViewModel
    {
        public List<GitRepo> PublicRepos { get; set; }
        public string UserName { get; set; }
    }
}