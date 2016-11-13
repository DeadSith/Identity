using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class RepoInfoViewModel
    {
        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
        public string RepoUri { get; set; }
    }
}