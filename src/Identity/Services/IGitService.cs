using Identity.Models;
using System.Collections.Generic;

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

        List<string> GetAllRepos();
    }
}