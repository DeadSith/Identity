using Identity.Data;
using Identity.Models;
using Identity.Models.HomeViewModels;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

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

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(HttpContext.User))
                return View();
            var user = await GetCurrentUserWithRepos();
            /*fixedUser.ShaUploaded = true;
            _context.SaveChanges();*/
            return View(user);
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
        public async Task<IActionResult> RepoView(string userName, string repoName, string branch, string path)
        {
            var repo =
                _context.Repos.Include(r => r.Author).Include(r => r.AllowedUser)
                    .First(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null)
                return StatusCode(404);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!repo.CheckAccess(user))
                return StatusCode(403);
            var fullRepoName = $"{userName.ToLower()}-{repoName.ToLower()}";
            var branches = _gitService.UpdateLocalRepo(_environment, fullRepoName, branch);
            var repoDirectory = $"{_environment.WebRootPath}/Repos/{fullRepoName}/{path}";
            if (!Directory.Exists(repoDirectory))
                return StatusCode(404);
            var content = Directory.GetDirectories(repoDirectory);
            var model = new RepoViewViewModel
            {
                RepoRootPath = $"/{userName}/{repoName}",
                InnerFolders = new List<string>(),
                InnerFiles = new List<string>(),
                Path = new List<string>(new[] {userName, repoName}),
                Branches = branches,
                CurrentBranchIndex = branches.IndexOf(branch)
            };
            foreach (var s in content)
            {
                var last = s.Split('/').Last();
                if (!String.Equals(last, ".git"))
                    model.InnerFolders.Add(last);
            }
            content = Directory.GetFiles(repoDirectory);
            foreach (var s in content)
            {
                var last = s.Split('/').Last();
                if (!String.Equals(last, ".git"))
                    model.InnerFiles.Add(last);
            }
            if (!String.IsNullOrWhiteSpace(path))
            {
                model.Path.AddRange(path.Split('/'));
                model.FullPath = $"/{userName}/{repoName}/{branch}/{path}";
            }
            else
            {
                model.FullPath = $"/{userName}/{repoName}/{branch}";
            }
            return View(model);
        }

        //Todo: show branches and latest commit date
        [HttpGet]
        public async Task<IActionResult> RepoInfo(string userName, string repoName, string branch, string path)
        {
            var repo =
                _context.Repos.Include(r => r.Author).Include(r => r.AllowedUser)
                    .FirstOrDefault(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null)
                return StatusCode(404);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!repo.CheckAccess(user))
                return StatusCode(403);
            var branches = _gitService.UpdateLocalRepo(_environment, $"{userName.ToLower()}-{repoName.ToLower()}",
                branch);
            var model = new RepoInfoViewModel
            {
                RepoRootPath = $"/{userName}/{repoName}",
                Path = new List<string>(new[] {userName, repoName}),
                RepoUri = $"{_gitService.GitServer}:{userName.ToLower()}-{repoName.ToLower()}",
                Branches = branches,
                CurrentBranchIndex = branches.IndexOf(branch),
                CurrentCommit = _gitService.Info(_environment, $"{userName.ToLower()}-{repoName.ToLower()}")
            };
            if (!String.IsNullOrWhiteSpace(path))
            {
                model.Path.AddRange(path.Split('/'));
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
            var user = await GetCurrentUserWithRepos();
            GitRepo.AddRepo(_context, _environment, _gitService, user, model.RepoName, model.IsPublic);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddColaborator()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColaborator(AddColaboratorViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var colaborator =
                _context.Users.FirstOrDefault(u => String.Equals(u.NormalizedUserName, model.ColaboratorName.ToUpper()));
            var repo = _context.Repos.Include(r => r.Author)
                .Include(r => r.AllowedUser)
                .FirstOrDefault(r => String.Equals(r.RepoName, model.RepositoryName) &&
                                     String.Equals(r.Author.NormalizedUserName, user.NormalizedUserName));
            repo.AllowedUser.Add(colaborator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Profile(string userName)
        {
            var model = new ProfileViewModel();
//Todo: fix
            var user = _context.Users.Include(u => u.Repos).FirstOrDefault(u => String.Equals(u.UserName.ToLower(), userName.ToLower()));
            if (user == null)
                return new StatusCodeResult(404);
            model.UserName = userName;
            model.PublicRepos = user.Repos.Where(r => r.IsPublic).ToList();
            return View(model);
        }

        public async Task<IActionResult> ViewFile(string userName, string repoName, string branch, string path)
        {
            var repo =
                _context.Repos.Include(r => r.Author).Include(r => r.AllowedUser)
                    .FirstOrDefault(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null)
                return StatusCode(404);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!repo.CheckAccess(user))
                return StatusCode(403);
            var fullRepoName = $"{userName.ToLower()}-{repoName.ToLower()}";
            var branches = _gitService.UpdateLocalRepo(_environment, fullRepoName, branch);
            var file = $"{_environment.WebRootPath}/Repos/{fullRepoName}/{path}";
            if (!System.IO.File.Exists(file))
                return RedirectToRoute("Error", new {id = 700});
            var model = new ViewFileViewModel
            {
                RepoRootPath = $"/{userName}/{repoName}",
                Path = new List<string>(new[] {userName, repoName}),
                Branches = branches,
                CurrentBranchIndex = branches.IndexOf(branch)
            };
            var pathElements = path.Split('/');
            for (var i = 0; i < pathElements.Length - 1; i++)
                model.Path.Add(pathElements[i]);
            model.FileName = pathElements[pathElements.Length - 1];
            var fs = new FileStream(file, FileMode.Open);
            using (var sr = new StreamReader(fs, GetEncoding(file)))
            {
                var content = sr.ReadToEnd();
                model.FileContent = content.Split('\n');
            }
            return View(model);
        }

        public async Task<IActionResult> CommitInfo(string userName, string repoName, string branch, string hash)
        {
            if (hash.Length != 40)
                return StatusCode(404);
            var repo =
                _context.Repos.Include(r => r.Author).Include(r => r.AllowedUser)
                    .FirstOrDefault(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null)
                return StatusCode(404);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!repo.CheckAccess(user))
                return StatusCode(403);
            var fullRepoName = $"{userName.ToLower()}-{repoName.ToLower()}";
            var branches = _gitService.UpdateLocalRepo(_environment, fullRepoName, branch);
            CommitChanges changes;
            try
            {
                changes = _gitService.GetCommitChanges(_environment, fullRepoName, hash);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error", new {id = 702});
            }
            var model = new CommitInfoViewModel
            {
                Changes = changes,
                RepoRootPath = $"/{userName}/{repoName}",
                Path = new List<string>(new[] {userName, repoName}),
                Branches = branches,
                CurrentBranchIndex = branches.IndexOf(branch)
            };
            return View(model);
        }

        public async Task<IActionResult> CommitHistory(string userName, string repoName, string branch, string path)
        {
            var repo =
                _context.Repos.Include(r => r.Author).Include(r => r.AllowedUser)
                    .FirstOrDefault(
                        r =>
                            String.Equals(r.RepoName.ToLower(), repoName.ToLower()) &&
                            String.Equals(r.Author.UserName.ToLower(), userName.ToLower()));
            if (repo == null)
                return StatusCode(404);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!repo.CheckAccess(user))
                return StatusCode(403);
            var fullRepoName = $"{userName.ToLower()}-{repoName.ToLower()}";
            var branches = _gitService.UpdateLocalRepo(_environment, fullRepoName, branch);
            List<GitCommit> hisrory;
            try
            {
                hisrory = _gitService.GetRepoCommitHistory(_environment, fullRepoName);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error", new {id = 703});
            }
            var model = new CommitHistoryViewModel
            {
                Commits = hisrory,
                RepoRootPath = $"/{userName}/{repoName}",
                Path = new List<string>(new[] {userName, repoName}),
                Branches = branches,
                CurrentBranchIndex = branches.IndexOf(branch)
            };
            return View(model);
        }

        #region Helpers

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        private async Task<ApplicationUser> GetCurrentUserWithRepos()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                return _context.Users.Include(u => u.Repos).First(u => String.Equals(u.Id, user.Id));
            }
            catch (Exception)
            {
                if (user == null)
                    return null;
                user.Repos = new List<GitRepo>();
                return user;
            }
            return null;
        }

        #endregion
    }
}
