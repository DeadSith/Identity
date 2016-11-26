using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public interface IRepoView
    {
        string RepoRootPath { get; set; }
        List<string> Path { get; set; }
        List<string> Branches { get; set; }
        int CurrentBranchNumber { get; set; } 
    }
}