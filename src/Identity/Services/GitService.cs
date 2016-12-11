using Identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public GitCommit Info(IHostingEnvironment environment, string repoName)
        {
            var repoPath = $"{environment.WebRootPath}/Repos/{repoName}";
            var gitResult = StartGit(" log -1 --numstat --date=raw", repoPath);
            var regex = new Regex(@"commit (.+)\nAuthor: (.+) <(.+)>[\n ]*Date:[\n ]*(.\d+)[ +\d]*[\n ]*(.+)[\n ]",RegexOptions.IgnoreCase);
            var match = regex.Matches(gitResult);
            if (match.Count == 0)
                return null;
            var result = new GitCommit
            {
                Hash = match[0].Groups[1].Value,
                Author = match[0].Groups[2].Value,
                AuthorEmail = match[0].Groups[3].Value,
                //Todo: fix timezones
                CommitTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(match[0].Groups[4].Value)).DateTime,
                Description = match[0].Groups[5].Value,
                Changes = new Dictionary<string, Tuple<int, int>>()
            };
            regex = new Regex(@"(\d[ \n]*\d[ \n]*.+\n)",RegexOptions.IgnoreCase);
            match = regex.Matches(gitResult);
            for (int i = 2; i < match.Count; i++)
            {
                try
                {
                    var elements = match[i].Value.Split(' ', '\t');
                    result.Changes[elements[2]] = new Tuple<int, int>(int.Parse(elements[0]), int.Parse(elements[1]));
                }
                catch (Exception ex)
                {
                    i++;
                }
            }
            return result;
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

        public CommitChanges GetCommitChanges(IHostingEnvironment environment,string repoName, string hash)
        {
            var gitResult = StartGit($" show {hash}", $"{environment.WebRootPath}/Repos/{repoName}");
            if (String.IsNullOrWhiteSpace(gitResult))
                throw new ArgumentException("No commit with specified hash");
            gitResult = gitResult + "diff --git";
            var regex = new Regex(@"\+\+\+ b\/(.*)\n(?s:(.*?))diff --git");
            var matches = regex.Matches(gitResult);
            var result = new CommitChanges {Changes = new Dictionary<string, string[]>(matches.Count)};
            foreach (Match match in matches)
            {
                result.Changes[match.Groups[1].Value] = match.Groups[2].Value.Split('\n');
            }
            regex = new Regex(@".*b\/(.*)\n(?s:deleted file mode .*?)diff --git");
            matches = regex.Matches(gitResult);
            result.DeletedFiles = new List<string>(matches.Count);
            foreach (Match match in matches)
            {
                result.DeletedFiles.Add(match.Groups[1].Value);
            }
            regex = new Regex(@".*b\/(.*)\n(?s:new file mode .*?)diff --git");
            matches = regex.Matches(gitResult);
            result.NewFiles = new List<string>(matches.Count);
            foreach (Match match in matches)
            {
                result.NewFiles.Add(match.Groups[1].Value);
            }
            return result;
        }

        public int GetNumberOfCommits(string repoPath)
        {
            throw new NotImplementedException();
        }

        public List<GitCommit> GetRepoCommitHistory(IHostingEnvironment environment, string repoName)
        {
            var repoPath = $"{environment.WebRootPath}/Repos/{repoName}";
            var gitResult = StartGit(" log --date=raw", repoPath);
            var regex = new Regex(@"commit (.+)\nAuthor: (.+) <(.+)>[\n ]*Date:[\n ]*(.\d+)[ +\d]*[\n ]*(.+)[\n ]",RegexOptions.IgnoreCase);
            var matches = regex.Matches(gitResult);
            if (matches.Count == 0)
                throw new ArgumentException();
            var result = new List<GitCommit>(matches.Count);
            result.AddRange(from Match match in matches
                select new GitCommit
                {
                    Hash = match.Groups[1].Value,
                    Author = match.Groups[2].Value,
                    AuthorEmail = match.Groups[3].Value,
                    //Todo: fix timezones
                    CommitTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(match.Groups[4].Value)).DateTime,
                    Description = match.Groups[5].Value
                });
            return result;
        }
    }
}