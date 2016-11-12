using Identity.Data;
using Identity.Models;
using Identity.Models.HomeViewModels;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitService _gitService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _environment;

        public HomeController(IGitService gitService,
            UserManager<ApplicationUser> manager,
            SignInManager<ApplicationUser> signManager,
            ApplicationDbContext context,
            IHostingEnvironment environment)
        {
            _gitService = gitService;
            _userManager = manager;
            _context = context;
            _signInManager = signManager;
            _environment = environment;
        }

        private List<string> GetGitoliteRepos()
        {
            var sshResults = _gitService.GetAllRepos();
            var repos = new List<string>();
            var regex = new Regex(@"^[ @]*R.*\s(.*)", RegexOptions.IgnoreCase);
            foreach (var res in sshResults)
            {
                var match = regex.Match(res);
                if (match.Success)
                    repos.Add(match.Groups[1].Value);
            }
            return repos;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(HttpContext.User))
                return View();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            /* _context.Repos.Add(new GitRepo
             {
                 Author = user,
                 IsPublic = true,
                 RepoName = "Test",
                 UserId = user.Id
             });
             _context.SaveChanges();*/
            var fixedUser = _context.Users.Include(u => u.Repos).First(u => String.Equals(u.Id, user.Id));
            return View(fixedUser);
        }

        public async Task<IActionResult> ClearRepos()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var fixedUser = _context.Users.Include(u => u.Repos).First(u => String.Equals(u.Id, user.Id));
            fixedUser.Repos.Clear();
            _context.SaveChanges();   
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RepoView(string userName, string repoName, string path)
        {
            var repo =
                _context.Repos.Include(r => r.Author)
                    .First(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null || !repo.IsPublic)
                RedirectToAction("Error");
            var fullRepoName = $"{userName.ToLower()}-{repoName.ToLower()}";
            if (!Directory.Exists(_environment.WebRootPath + $"/Repos/{fullRepoName}"))
                _gitService.Clone(_environment.WebRootPath+"/Repos",fullRepoName);
            _gitService.Pull(_environment.WebRootPath + "/Repos",fullRepoName);
            var repoDirectory = $"{_environment.WebRootPath}/Repos/{fullRepoName}/{path}";
            if (!Directory.Exists(repoDirectory))
                RedirectToAction("Error");
            var content = Directory.GetDirectories(repoDirectory);
            var model = new RepoViewViewModel
            {
                RepoRootPath = $"/{userName}/{repoName}",
                InnerFolders = new List<string>(),
                InnerFiles = new List<string>(),
                Path = new List<string>(new[] {userName, repoName})
            };
            for(var i = 0;i<content.Length;i++)
            {
                var last = content[i].Split('/').Last();
                if(!String.Equals(last,".git"))
                    model.InnerFolders.Add(last);
            }
            content = Directory.GetFiles(repoDirectory);
            for(var i = 0;i<content.Length;i++)
            {
                var last = content[i].Split('/').Last();
                if(!String.Equals(last,".git"))
                    model.InnerFiles.Add(last);
            }
            if (!String.IsNullOrWhiteSpace(path))
            {
                model.Path.AddRange(path.Split('/'));
                model.FullPath = $"/{userName}/{repoName}/{path}";
            }
            else
            {
                model.FullPath = $"/{userName}/{repoName}";
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddNewRepo()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewRepo(AddNewRepoViewModel model)
        {
            ViewData["Success"] = "Repository was successfully created";
            ViewData["Error"] = "Something went wrong";
            if (!_signInManager.IsSignedIn(HttpContext.User))
                return View("Index");
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var fixedUser = _context.Users.Include(u => u.Repos).First(u => String.Equals(u.Id, user.Id));
            AddRepo(fixedUser, model.RepoName,model.IsPublic);
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }

        private void AddRepo(ApplicationUser user, string repositoryName,bool isPublic)
        {
            _context.Repos.Add(
                new GitRepo
                {
                    Author = user,
                    IsPublic = isPublic,
                    RepoName = repositoryName,
                    RepoKey = Guid.NewGuid().ToString()
                });
            _context.SaveChanges();
            if (!Directory.Exists(_environment.WebRootPath+@"/gitolite-admin"))
                _gitService.CloneMaster(_environment.WebRootPath);
            _gitService.PullMaster(_environment.WebRootPath);
            var configPath = _environment.WebRootPath + @"/gitolite-admin/conf/" + user.UserName.ToLower()+".conf";
            if (!System.IO.File.Exists(configPath))
                System.IO.File.Create(configPath);
            var stream = System.IO.File.Open(configPath,FileMode.Append);
            using (var sw = new StreamWriter(stream))
            {
                sw.WriteLine($"repo {user.UserName.ToLower()}-{repositoryName.ToLower()}");
                sw.WriteLine($"   RW+\t=\t{user.UserName.ToLower()}");
            }
            _gitService.Upload(_environment.WebRootPath);
        }
    }
}