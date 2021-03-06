﻿using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class CommitInfoViewModel: IRepoView
    {
        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
        public List<string> Branches { get; set; }
        public int CurrentBranchIndex { get; set; }
        public CommitChanges Changes { get; set; }
    }
}