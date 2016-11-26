using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Identity.Models.HomeViewModels
{
    public class ViewFileViewModel: IRepoView
    {
        public string FileName { get; set; }
        public string[] FileContent { get; set; }

        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
        public List<string> Branches { get; set; }
        public int CurrentBranchIndex { get; set; }
    }
}