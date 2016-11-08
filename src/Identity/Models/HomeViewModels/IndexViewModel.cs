using System.Collections.Generic;
using Identity.Models;

namespace Identity.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public List<GitRepo> Repos = new List<GitRepo>();
    }
}