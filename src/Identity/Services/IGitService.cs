using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Models;

namespace Identity.Services
{
    public interface IGitService
    {
        void Upload(string path); 
        void Pull(string path);
        void Clone(string path);
        GitCommit Info(string directory, string sha="");

        List<string> GetAllRepos();
    }
}
