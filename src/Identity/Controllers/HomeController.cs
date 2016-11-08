using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Identity.Services;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using Identity.Models.HomeViewModels;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitService _gitService;
        private readonly UserManager<ApplicationUser> _userManager;

        //Just for testing
        private List<string> _repos{get;}


        public HomeController(IGitService gitService, UserManager<ApplicationUser> manager)  
        {
            _gitService = gitService;
            _userManager = manager;
            var sshResults = _gitService.GetAllRepos();
            _repos = new List<string>();
            var regex = new Regex(@"^[ @]*R.*\s(.*)",RegexOptions.IgnoreCase);
            foreach(var res in sshResults)
            {
                var match = regex.Match(res);
                if(match.Success)
                    _repos.Add(match.Groups[1].Value);
            }
        }      
        public IActionResult Index()
        {         
            var model = new IndexViewModel();
            foreach(var info in _repos)
            {
                model.Repos.Add(new GitRepo{Author="sith",Name = "info"});
            }   
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Repo(string userName, string repoName)
        {
            throw new NotImplementedException();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
