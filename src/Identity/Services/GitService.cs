﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Identity.Models;

namespace Identity.Services
{
    public class GitService:IGitService
    {
        public GitService(string gitServer)
        {
            _gitServer = gitServer;
        }

        private string _gitServer { get; set; }

        public string StartGit(string command, string directory)
        {
            var gitInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                Arguments = command,
                WorkingDirectory = directory,
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = @"/usr/bin/git"
            };
            var gitProcess = new Process {StartInfo = gitInfo};
            gitProcess.Start();
            var stderr_str = gitProcess.StandardError.ReadToEnd();
            var stdout_str = gitProcess.StandardOutput.ReadToEnd();
            gitProcess.WaitForExit();
            return stdout_str;
        }

        public void Upload()
        {
            var directory = Directory.GetCurrentDirectory() + @"/gitolite-admin";
            StartGit(@"add .", directory);
            StartGit(" commit -m '14' ", directory);
            StartGit(" push", directory);
        }

        public void Pull()
        {
            StartGit(" pull", Directory.GetCurrentDirectory() + @"/gitolite-admin");
        }

        public void Clone()
        {
            StartGit(@" clone "+_gitServer+@":gitolite -admin", Directory.GetCurrentDirectory());
        }

        public GitCommit Info(string directory, string sha = "")
        {
            var result = StartGit(" log -1",directory);
            //Todo: parse result
            return new GitCommit();
        }
    }
}