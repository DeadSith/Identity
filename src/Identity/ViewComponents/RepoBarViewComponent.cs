using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Identity.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Identity.ViewComponents
{
    public class RepoBarViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IRepoView repoView,string activePage="")
        {
            if (String.Equals(activePage, "Info"))
                ViewBag.IsInfoActive = true;
            else if (String.Equals(activePage, "Repo"))
                ViewBag.IsFolderViewActive = true;
            return View(repoView);
        }
    }
}