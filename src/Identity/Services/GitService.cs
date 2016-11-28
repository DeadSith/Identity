﻿using Identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace Identity.Services
{
    public class GitService : IGitService
    {
        public GitService(string gitServer)
        {
            GitServer = gitServer;
        }

        public string GitServer { get; }

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
            var gitProcess = new Process { StartInfo = gitInfo };
            gitProcess.Start();
            var stderr_str = gitProcess.StandardError.ReadToEnd();
            var stdout_str = gitProcess.StandardOutput.ReadToEnd();
            Console.WriteLine();
            Console.WriteLine(stderr_str);
            Console.WriteLine(stdout_str);
            Console.WriteLine();
            gitProcess.WaitForExit();
            return stdout_str;
        }

        public void Upload(string path)
        {
            var directory = path + @"/gitolite-admin";
            StartGit(@"add .", directory);
            StartGit(@" commit -m '" + DateTime.Now.ToString("ddMMyyyyHHmm") + @"'", directory);
            StartGit(" push", directory);
        }

        public void PullMaster(string path)
        {
            StartGit(" pull", path + @"/gitolite-admin");
        }

        public void CloneMaster(string path)
        {
            StartGit(@" clone " + GitServer + @":gitolite-admin", path);
        }

        public void Clone(string path, string repoName)
        {
            StartGit(@" clone " + GitServer + $":{repoName}", path);
        }

        public void Pull(string path, string repoName)
        {
            StartGit(" pull", path + $"/{repoName}");
        }

        public List<string> GetBranches(string repoPath)
        {
            var result = StartGit(" branch -r", repoPath);
            var strings = result.Split('\n');
            var res = new List<string>();
            for (var i = 1; i < strings.Length; i++)
            {
                var repo = strings[i].Split('/').Last();
                res.Add(repo);
            }
            return res;
        }

        public void SwitchBranch(string repoPath, string branchName)
        {
            StartGit($" checkout origin/{branchName}", repoPath);
        }

        public GitCommit Info(string directory, string sha = "")
        {
            var result = StartGit(" log -1", directory);
            //Todo: parse result
            throw new NotImplementedException();
            return new GitCommit();
        }

        public List<string> UpdateLocalRepo(IHostingEnvironment environment, string repoName, string branch)
        {
            var path = environment.WebRootPath + "/Repos";
            if (!Directory.Exists($"{path}/{repoName}"))
                this.Clone(path, repoName);
            this.Pull(path, repoName);
            var res = this.GetBranches($"{path}/{repoName}");
            if(!String.Equals("HEAD",branch)&&!res.Contains(branch))
                throw new ArgumentException();
            this.SwitchBranch($"{path}/{repoName}",branch);
            return res;
        }

        public List<string> GetAllRepos()
        {
            var sshInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                Arguments = @" git@acer info",
                RedirectStandardError = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = @"/usr/bin/ssh"
            };
            var sshProcess = new Process { StartInfo = sshInfo };
            sshProcess.Start();
            var stderr_str = sshProcess.StandardError.ReadToEnd();
            var stdout_str = sshProcess.StandardOutput.ReadToEnd();
            sshProcess.WaitForExit();
            return stdout_str.Split('\n').ToList();
        }
    }
}