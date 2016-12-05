using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class RepoInfoViewModel: IRepoView
    {
        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
        public List<string> Branches { get; set; }
        public int CurrentBranchIndex { get; set; }
        public string RepoUri { get; set; }
        public GitCommit CurrentCommit { get; set; }
    }
}