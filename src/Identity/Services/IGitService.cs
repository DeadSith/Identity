using Identity.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace Identity.Services
{
    public interface IGitService
    {
        string GitServer { get; }

        void Upload(string path);

        void PullMaster(string path);

        void CloneMaster(string path);

        void Clone(string path, string repoName);

        void Pull(string path, string repoName);

        List<string> GetBranches(string repoPath);

        void SwitchBranch(string repoPath, string branchName);

        GitCommit Info(string directory, string sha = "");

        /// <summary>
        /// Updates local copy of repo and switches to specified branch
        /// </summary>
        /// <param name="repoName">Name of repo to update</param>
        /// <param name="branch">Branch to switch</param>
        /// <returns>
        /// List of all branches of current repo
        /// </returns>
        List<string> UpdateLocalRepo(IHostingEnvironment environment, string repoName, string branch);

        List<string> GetAllRepos();
    }
}