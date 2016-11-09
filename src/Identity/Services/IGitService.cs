using Identity.Models;
using System.Collections.Generic;

namespace Identity.Services
{
    public interface IGitService
    {
        void Upload(string path);

        void Pull(string path);

        void Clone(string path);

        GitCommit Info(string directory, string sha = "");

        List<string> GetAllRepos();
    }
}