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

        GitCommit Info(IHostingEnvironment environment, string repoName);

        /// <summary>
        /// Updates local copy of repo and switches to specified branch
        /// </summary>
        /// <param name="repoName">Name of repo to update</param>
        /// <param name="branch">Branch to switch</param>
        /// <param name="environment">Current working environment</param>
        /// <returns>
        /// List of all branches of current repo
        /// </returns>
        List<string> UpdateLocalRepo(IHostingEnvironment environment, string repoName, string branch);

        List<string> GetAllRepos();

        /// <summary>
        /// Parses git show of specified commit
        /// Note: does not switch branch!
        /// </summary>
        /// <param name="environment">Current working environment</param>
        /// <param name="repoName">Name of repo to view</param>
        /// <param name="hash">Hash of desired commits</param>
        /// <returns>
        /// Changes made in commit with specified hash
        /// </returns>
        CommitChanges GetCommitChanges(IHostingEnvironment environment, string repoName, string hash);

        int GetNumberOfCommits(string repoPath);
    }
}