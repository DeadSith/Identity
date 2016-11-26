using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class RepoViewViewModel: IRepoView
    {
        public string RepoRootPath { get; set; }
        public List<string> InnerFolders { get; set; }
        public List<string> InnerFiles { get; set; }
        public List<string> Path { get; set; }
        public List<string> Branches { get; set; }
        public int CurrentBranchIndex { get; set; }
        public string FullPath { get; set; }
    }
}