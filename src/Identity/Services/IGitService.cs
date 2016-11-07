using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Models;

namespace Identity.Services
{
    public interface IGitService
    {
        void Upload(); 
        void Pull();
        void Clone();
        GitCommit Info(string directory, string sha="");
    }
}
