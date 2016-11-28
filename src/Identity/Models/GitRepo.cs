using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Identity.Data;
using Identity.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Identity.Models
{
    public class GitRepo
    {
        public string RepoName { get; set; }
        public bool IsPublic { get; set; }

        [Key]
        public string RepoKey { get; set; }

        public ApplicationUser Author { get; set; }


        public bool CheckAccess(ApplicationUser user)
        {
            return this.IsPublic || String.Equals(this.Author.UserName, user.UserName);
        }

        public static void AddRepo(ApplicationDbContext context, IHostingEnvironment environment, IGitService gitService, ApplicationUser user, string repositoryName, bool isPublic)
        {
            context.Repos.Add(
                new GitRepo
                {
                    Author = user,
                    IsPublic = isPublic,
                    RepoName = repositoryName,
                    RepoKey = Guid.NewGuid().ToString()
                });
            context.SaveChanges();
            if (!Directory.Exists(environment.WebRootPath + @"/gitolite-admin"))
                gitService.CloneMaster(environment.WebRootPath);
            gitService.PullMaster(environment.WebRootPath);
            var configPath = environment.WebRootPath + @"/gitolite-admin/conf/" + user.UserName.ToLower() + ".conf";
            if (!System.IO.File.Exists(configPath))
                System.IO.File.Create(configPath);
            var stream = System.IO.File.Open(configPath, FileMode.Append);
            using (var sw = new StreamWriter(stream))
            {
                sw.WriteLine($"repo {user.UserName.ToLower()}-{repositoryName.ToLower()}");
                sw.WriteLine($"   RW+\t=\t{user.UserName.ToLower()}");
            }
            gitService.Upload(environment.WebRootPath);
        }
    }
}