using System;
using System.Collections.Generic;

namespace Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class GitCommit
    {
        //public GitRepo Repo { get; set; }
        public string Hash { get; set; }
        public string Author { get; set; }
        public string AuthorEmail { get; set; }
        public string Description { get; set; }
        public DateTime CommitTime { get; set; }
        public Dictionary<string, Tuple<int,int>> Changes { get; set; }
    }
}