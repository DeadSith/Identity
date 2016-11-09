using Identity.Data;
using Identity.Models;
using Identity.Models.HomeViewModels;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitService _gitService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(IGitService gitService,
            UserManager<ApplicationUser> manager,
            SignInManager<ApplicationUser> signManager,
            ApplicationDbContext context)
        {
            _gitService = gitService;
            _userManager = manager;
            _context = context;
            _signInManager = signManager;
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
            var model = new IndexViewModel();
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
            model.Repos = fixedUser.Repos;
            return View(model);
        }

        [HttpGet]
        public IActionResult Repo(string userName, string repoName, string path)
        {
            throw new NotImplementedException();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}