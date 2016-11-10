using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class GitRepo
    {
        
        public string RepoName { get; set; }
        public bool IsPublic { get; set; }

        [Key]
        public string RepoKey { get; set; }

        public ApplicationUser Author { get; set; }
    }
}