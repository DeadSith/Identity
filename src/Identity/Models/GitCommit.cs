using System;


namespace Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class GitCommit
    {
        public string Author{get;set;}
        public string SHA{get;set;}
        public string Description{get;set;}
        public DateTime CommitTime{get;set;}
    }
}